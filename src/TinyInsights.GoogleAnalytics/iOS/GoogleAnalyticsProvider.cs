using Foundation;
using Google.Analytics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

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

        public virtual async Task TrackErrorAsync(Exception ex, Dictionary<string, string> properties)
        {
            // TODO: Implement
        }

        public virtual async Task TrackEventAsync(string eventName)
        {
            await TrackEventAsync(eventName, null);
        }

        public virtual async Task TrackEventAsync(string eventName, Dictionary<string, string> properties)
        {
            string action = string.Empty;
            string label = string.Empty;
            int number = 0;

            if(properties != null && properties.ContainsKey("action"))
            {
                action = properties["action"];
            }

            if (properties != null && properties.ContainsKey("number"))
            {
                int.TryParse(properties["number"], out number);
            }

            if (properties != null && properties.ContainsKey("label"))
            {
                label = properties["label"];
            }

            var eventToTrack = DictionaryBuilder.CreateEvent(eventName.ToLower(), action.ToLower(), label.ToLower(), number).Build();

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    if (property.Key != "action" && property.Key != "label" && property.Key != "number")
                    {
                        eventToTrack.Add(NSObject.FromObject(property.Key), NSObject.FromObject(property.Value));
                    }
                }
            }

            Tracker.Send(eventToTrack);
        }

        public virtual async Task TrackPageViewAsync(string viewName)
        {
            await TrackPageViewAsync(viewName, null);
        }

        public virtual async Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties)
        {
            Tracker.Set(GaiConstants.ScreenName, viewName);

            var viewToTrack = DictionaryBuilder.CreateScreenView().Build();

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    viewToTrack.Add(NSObject.FromObject(property.Key), NSObject.FromObject(property.Value));
                }
            }

            Tracker.Send(viewToTrack);
        }
    }
}
