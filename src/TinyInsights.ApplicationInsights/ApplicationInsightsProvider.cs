using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TinyInsightsLib.ApplicationInsights
{
    public class ApplicationInsightsProvider : ITinyInsightsProvider
    {
        private const string crashLogFilename = "crashes.tinyinsights";
        private readonly string logPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private TelemetryClient client;

        public bool IsTrackErrorsEnabled { get; set; } = true;
        public bool IsTrackPageViewsEnabled { get; set; } = true;
        public bool IsTrackEventsEnabled { get; set; } = true;
        public bool IsTrackDependencyEnabled { get; set; } = true;

#if XAMARINIOS || MONODROID
        public ApplicationInsightsProvider(string key)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            client = new TelemetryClient();
            client.InstrumentationKey = key;

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

#elif UAP
        public ApplicationInsightsProvider(Application app, string key)
        {
            app.UnhandledException += App_UnhandledException;

            client = new TelemetryClient();
            client.InstrumentationKey = key;

            Task.Run(SendCrashes);
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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
            var path = Path.Combine(logPath, crashLogFilename);

            var json = File.ReadAllText(path);

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

            var path = Path.Combine(logPath, crashLogFilename);

            System.IO.File.WriteAllText(path, json);

        }

        public async Task TrackErrorAsync(Exception ex)
        {
            await TrackErrorAsync(ex, null);
        }

        public async Task TrackErrorAsync(Exception ex, Dictionary<string, string> properties)
        {
            client.TrackException(ex, properties);
            client.Flush();
        }

        public async Task TrackEventAsync(string eventName)
        {
            await TrackEventAsync(eventName, null);
        }

        public async Task TrackEventAsync(string eventName, Dictionary<string, string> properties)
        {
            client.TrackEvent(eventName, properties);
            client.Flush();
        }

        public async Task TrackPageViewAsync(string viewName)
        {
            await TrackPageViewAsync(viewName, null);
        }

        public async Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties)
        {
            client.TrackPageView(viewName);
            client.Flush();
        }

        public async Task TrackDependencyAsync(string dependencyType, string dependencyName, DateTimeOffset startTime, TimeSpan duration, bool success)
        {
            client.TrackDependency(dependencyType, dependencyName, null, startTime, duration, success);
        }
    }
}
