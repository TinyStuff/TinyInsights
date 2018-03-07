
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
            if(IsTrackErrorsEnabled)
            {
                var exceptionToTrack = HitBuilder.CreateException(ex.Message, false).Build();

                tracker.Send(exceptionToTrack);
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

            var eventToTrack = HitBuilder.CreateCustomEvent(eventName, action, label).Build();

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

            tracker.Send(viewToTrack);
        }
    }
}
