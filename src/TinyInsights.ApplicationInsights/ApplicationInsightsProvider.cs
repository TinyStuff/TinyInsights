using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace TinyInsightsLib.ApplicationInsights
{
    public class ApplicationInsightsProvider : ITinyInsightsProvider
    {
        private const string crashLogFilename = "crashes.tinyinsights";

        private TelemetryClient client;

        public bool IsTrackErrorsEnabled { get; set; } = true;
        public bool IsTrackPageViewsEnabled { get; set; } = true;
        public bool IsTrackEventsEnabled { get; set; } = true;

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

        private async Task SendCrashes()
        {
            var crashes = ReadCrashes();

            if(crashes != null)
            {
                var properties = new Dictionary<string, string>();
                properties.Add("IsCrash", "true");

                foreach(var crash in crashes)
                {
                    await TrackErrorAsync(crash, properties);
                }
            }

            var file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(crashLogFilename, CreationCollisionOption.OpenIfExists);
            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        private List<Exception> ReadCrashes()
        {
            List<Exception> crashes;

            var fileTask = ApplicationData.Current.LocalCacheFolder.CreateFileAsync(crashLogFilename, CreationCollisionOption.OpenIfExists).AsTask<StorageFile>();
            fileTask.Wait();
            var file = fileTask.Result;

            var readTask = FileIO.ReadTextAsync(file).AsTask<string>();
            readTask.Wait();
            var json = readTask.Result;

            if (string.IsNullOrWhiteSpace(json))
            {
                crashes = new List<Exception>();
            }
            else
            {
                crashes = JsonConvert.DeserializeObject<List<Exception>>(json);
            }

            return crashes;
        }

        private void HandleCrash(Exception ex)
        {
            var crashes = ReadCrashes();

            crashes.Add(ex);

            var json = JsonConvert.SerializeObject(crashes);

            var fileTask = ApplicationData.Current.LocalCacheFolder.CreateFileAsync(crashLogFilename, CreationCollisionOption.OpenIfExists).AsTask<StorageFile>();
            fileTask.Wait();
            var file = fileTask.Result;

            var writeTask = FileIO.WriteTextAsync(file, json).AsTask();
            writeTask.Wait();
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
    }
}
