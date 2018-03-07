using Google.Analytics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using System.Linq;

namespace TinyInsightsLib.GoogleAnalytics
{
    public class GoogleAnalyticsProvider : ITinyInsightsProvider
    {
        private ITracker Tracker;

        public bool IsTrackErrorsEnabled { get; set; } = true;
        public bool IsTrackPageViewsEnabled { get; set; } = true;
        public bool IsTrackEventsEnabled { get; set; } = true;

        public GoogleAnalyticsProvider(string trackingId, bool catchUnhandledExceptions = true)
        {
            Gai.SharedInstance.DispatchInterval = 20;
            Gai.SharedInstance.TrackUncaughtExceptions = true;

            Tracker = Gai.SharedInstance.GetTracker(trackingId);
            Tracker.Set(GaiConstants.Language, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        }

        public virtual async Task TrackErrorAsync(Exception ex)
        {
            if(IsTrackErrorsEnabled)
            {
                Tracker.Send(DictionaryBuilder.CreateException(ex.Message, 0).Build());
            }
        }

        public virtual async Task TrackEventAsync(string eventName)
        {
            await TrackEventAsync(eventName, null);
        }

        public virtual async Task TrackEventAsync(string eventName, Dictionary<string, string> properties)
        {
            string action = string.Empty;
            string label = string.Empty;

            if(properties != null && properties.ContainsKey("action"))
            {
                action = properties["action"];
            }

            if (properties != null && properties.ContainsKey("label"))
            {
                label = properties["label"];
            }

            var eventToTrack = DictionaryBuilder.CreateEvent(eventName.ToLower(), action.ToLower(), label.ToLower(), 1).Build();
            
            Tracker.Send(eventToTrack);
        }

        public virtual async Task TrackPageViewAsync(string viewName)
        {
            await TrackPageViewAsync(viewName, null);
        }

        public virtual async Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties)
        {
            Tracker.Set(GaiConstants.ScreenName, viewName);
           
            Tracker.Send(DictionaryBuilder.CreateScreenView().Build());
        }
    }
}
