using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TinyInsights;
using TinyInsightsLib;


namespace TinyInsights.GoogleAnalytics.Droid
{
    public class GoogleAnalyticsProvider : ITinyInsightsProvider
    {
       

        public bool IsTrackErrorsEnabled { get; set; }
        public bool IsTrackPageViewsEnabled { get; set; }
        public bool IsTrackEventsEnabled { get; set; }

        public GoogleAnalyticsProvider(string trackingId, bool catchUnhandledExceptions = true)
        {
           
        }

        public async Task TrackErrorAsync(Exception ex)
        {
            if (IsTrackErrorsEnabled)
            {
                
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

            if (properties != null && properties.ContainsKey("action"))
            {
                action = properties["action"];
            }

            if (properties != null && properties.ContainsKey("label"))
            {
                action = properties["label"];
            }

           
        }

        public async Task TrackPageViewAsync(string viewName)
        {
            await TrackPageViewAsync(viewName, null);
        }

        public async Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties)
        {
           
        }
    }
}
