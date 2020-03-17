using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

#if WINDOWS_UWP
using Windows.Storage;
using Windows.UI.Xaml;
#endif

namespace TinyInsightsLib.ApplicationInsights
{
    public class ApplicationInsightsProvider : ITinyInsightsProvider
    {
        public const string userIdKey = nameof(userIdKey);

        private const string crashLogFilename = "crashes.tinyinsights";
#if __IOS__ || __ANDROID__
        private readonly string logPath = FileSystem.CacheDirectory;

#endif

        private TelemetryClient client;

        public bool IsTrackErrorsEnabled { get; set; } = true;
        public bool IsTrackPageViewsEnabled { get; set; } = true;
        public bool IsTrackEventsEnabled { get; set; } = true;
        public bool IsTrackDependencyEnabled { get; set; } = true;

#if __IOS__ || __ANDROID__
        public ApplicationInsightsProvider(string key)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            var configuration = new TelemetryConfiguration(key);

            try
            {
                client = new TelemetryClient(configuration);
                client.Context.Device.OperatingSystem = DeviceInfo.Platform.ToString();
                client.Context.Device.Model = DeviceInfo.Model;
                
                if(Preferences.ContainsKey(userIdKey))
                {
                    var userId = Preferences.Get(userIdKey, string.Empty);

                    client.Context.User.Id = userId;
                }
                else
                {
                    var userId = Guid.NewGuid().ToString();
                    Preferences.Set(userIdKey, userId);

                    client.Context.User.Id = userId;
                }

                client.Context.GlobalProperties.Add("Language", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                client.Context.GlobalProperties.Add("Manufacturer", DeviceInfo.Manufacturer);
                client.Context.GlobalProperties.Add("AppVersion", AppInfo.VersionString);
                client.Context.GlobalProperties.Add("AppBuildNumber", AppInfo.BuildString);
            }
            catch (Exception)
            {
            }

            Task.Run(SendCrashes);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleCrash(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleCrash((Exception)e.ExceptionObject);
        }

#elif WINDOWS_UWP
        public ApplicationInsightsProvider(Application app, string key)
        {
            app.UnhandledException += App_UnhandledException;

            client = new TelemetryClient();
            client.InstrumentationKey = key;

            Task.Run(SendCrashes);
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            HandleCrash(e.Exception);
        }
#endif

        private async Task SendCrashes()
        {
            var crashes = ReadCrashes();

            if (crashes != null)
            {
                var properties = new Dictionary<string, string>();
                properties.Add("IsCrash", "true");

                foreach (var crash in crashes)
                {
                    await TrackErrorAsync(crash, properties);
                }
            }
        }

        private List<Exception> ReadCrashes()
        {
#if __IOS__ || __ANDROID__
            var path = Path.Combine(logPath, crashLogFilename);
            
            if (!File.Exists(path))
            {
                return new List<Exception>();
            }

            var json = File.ReadAllText(path);
#elif WINDOWS_UWP
            
            var fileTask = ApplicationData.Current.LocalCacheFolder.CreateFileAsync(crashLogFilename, CreationCollisionOption.OpenIfExists).AsTask<StorageFile>();
            fileTask.Wait();
            var file = fileTask.Result;

            var readTask = FileIO.ReadTextAsync(file).AsTask<string>();
            readTask.Wait();
            var json = readTask.Result;
#endif
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Exception>();
            }

            return JsonConvert.DeserializeObject<List<Exception>>(json);

        }

        private void HandleCrash(Exception ex)
        {
            var crashes = ReadCrashes();

            crashes.Add(ex);

            var json = JsonConvert.SerializeObject(crashes);

#if __IOS__ || __ANDROID__
            var path = Path.Combine(logPath, crashLogFilename);

            if(!File.Exists(path))
            {
                File.Create(path);
            }

            System.IO.File.WriteAllText(path, json);
#elif UWP
             var fileTask = ApplicationData.Current.LocalCacheFolder.CreateFileAsync(crashLogFilename, CreationCollisionOption.OpenIfExists).AsTask<StorageFile>();
            fileTask.Wait();
            var file = fileTask.Result;

            var writeTask = FileIO.WriteTextAsync(file, json).AsTask();
            writeTask.Wait();
#endif
        }

        public async Task TrackErrorAsync(Exception ex, Dictionary<string, string> properties = null)
        {
            client.TrackException(ex, properties);
            client.Flush();
        }

        public async Task TrackEventAsync(string eventName, Dictionary<string, string> properties = null)
        {
            client.TrackEvent(eventName, properties);
            client.Flush();
        }

        public async Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties = null)
        {
            client.TrackPageView(viewName);
            client.Flush();
        }

        public async Task TrackDependencyAsync(string dependencyType, string dependencyName, DateTimeOffset startTime, TimeSpan duration, bool success, int resultCode = 0, Exception exception = null)
        {
            var dependency = new DependencyTelemetry(dependencyType, dependencyName, startTime, duration, success)
            {
                ResultCode = resultCode.ToString()
            };

            if(exception != null)
            {
                var properties = new Dictionary<string, string>();
                properties.Add("Exception message", exception.Message);
                properties.Add("StackTrace", exception.StackTrace);

                if(exception.InnerException != null)
                {
                    properties.Add("Inner exception message", exception.InnerException.Message);
                    properties.Add("Inner exception stackTrace", exception.InnerException.StackTrace);
                }
            }

            client.TrackDependency(dependency);
        }
    }
}
