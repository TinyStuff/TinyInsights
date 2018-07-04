
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using System.Linq;
using GoogleAnalytics;

namespace TinyInsightsLib.GoogleAnalytics
{
    public class GoogleAnalyticsProvider : ITinyInsightsProvider
    {
        private Tracker tracker;
        public bool IsTrackErrorsEnabled { get; set; } = true;
        public bool IsTrackPageViewsEnabled { get; set; } = true;
        public bool IsTrackEventsEnabled { get; set; } = true;

        public GoogleAnalyticsProvider(string trackingId, bool catchUnhandledExceptions = true)
        {
            tracker = AnalyticsManager.Current.CreateTracker(trackingId);
            AnalyticsManager.Current.ReportUncaughtExceptions = catchUnhandledExceptions;
        }

        public virtual async Task TrackErrorAsync(Exception ex)
        {
            await TrackErrorAsync(ex, null);
        }

        public virtual async Task TrackErrorAsync(Exception ex, Dictionary<string, string> properties)
        {
            if (IsTrackErrorsEnabled)
            {
                var builder = HitBuilder.CreateException(ex.Message, false);

                if(properties != null)
                {
                    foreach (var property in properties)
                    {
                        builder.Set(property.Key, property.Value);
                    }
                }

                tracker.Send(builder.Build());
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
            int number = 0;

            if(properties != null && properties.ContainsKey("action"))
            {
                action = properties["action"];
            }

            if (properties != null && properties.ContainsKey("label"))
            {
                label = properties["label"];
            }

            if (properties != null && properties.ContainsKey("number"))
            {
                int.TryParse(properties["number"], out number);
            }

            var eventToTrack = HitBuilder.CreateCustomEvent(eventName, action, label, number).Build();

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    if (property.Key != "action" && property.Key != "label" && property.Key != "number")
                    {
                        eventToTrack.Add(property.Key, property.Value);
                    }
                }
            }

            tracker.Send(eventToTrack);
        }

        public virtual async Task TrackPageViewAsync(string viewName)
        {
            await TrackPageViewAsync(viewName, null);
        }

        public virtual async Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties)
        {
            tracker.ScreenName = viewName;

            var viewToTrack = HitBuilder.CreateScreenView().Build();

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    viewToTrack.Add(property.Key, property.Value);
                }
            }

            tracker.Send(viewToTrack);
        }

        
    }
}
