using Google.Analytics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using TinyInsights;
using TinyInsightsLib;
using System.Linq;

namespace TinyInsights.GoogleAnalytics.iOS
{
    public class GoogleAnalyticsProvider : ITinyInsightsProvider
    {
        private ITracker Tracker;

        public bool IsTrackErrorsEnabled { get; set; }
        public bool IsTrackPageViewsEnabled { get; set; }
        public bool IsTrackEventsEnabled { get; set; }

        public GoogleAnalyticsProvider(string trackingId, bool catchUnhandledExceptions = true)
        {
            Gai.SharedInstance.DispatchInterval = 20;
            Gai.SharedInstance.TrackUncaughtExceptions = true;

            Tracker = Gai.SharedInstance.GetTracker(trackingId);
            Tracker.Set(GaiConstants.Language, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        }

        public async Task TrackErrorAsync(Exception ex)
        {
            if(IsTrackErrorsEnabled)
            {
                Tracker.Send(DictionaryBuilder.CreateException(ex.Message, 0).Build());
            }
        }

        public async Task TrackEventAsync(string eventName)
        {
            await TrackEventAsync(eventName, null);
        }

        public async Task TrackEventAsync(string eventName, Dictionary<string, string> properties)
        {
            string action = string.Empty;
            string label = string.Empty;

            if(properties != null && properties.ContainsKey("action"))
            {
                action = properties["action"];
            }

            if (properties != null && properties.ContainsKey("label"))
            {
                action = properties["label"];
            }

            var eventToTrack = DictionaryBuilder.CreateEvent(eventName, action, label, 1).Build();
            
            Tracker.Send(eventToTrack);
        }

        public async Task TrackPageViewAsync(string viewName)
        {
            await TrackPageViewAsync(viewName, null);
        }

        public async Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties)
        {
            Tracker.Set(GaiConstants.ScreenName, viewName);
           
            Tracker.Send(DictionaryBuilder.CreateScreenView().Build());
        }
    }
}
