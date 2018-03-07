using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Linq;

namespace TinyInsightsLib.AppCenter
{
    public class AppCenterProvider : ITinyInsightsProvider
    {
        public AppCenterProvider(string iOSkey, string androidKey, string uwpKey)
        {
            Microsoft.AppCenter.AppCenter.Start($"ios={iOSkey};uwp={uwpKey};android={androidKey}",
                                                      typeof(Analytics), typeof(Crashes));
        }


        public bool IsTrackErrorsEnabled { get; set; } = true;
        public bool IsTrackPageViewsEnabled { get; set; } = true;
        public bool IsTrackEventsEnabled { get; set; } = true;

        public async Task TrackErrorAsync(Exception ex)
        {
            if(IsTrackEventsEnabled)
            {
                Crashes.TrackError(ex);
            }
        }

        public async Task TrackEventAsync(string eventName)
        {
            await TrackEventAsync(eventName, null);
        }

        public async Task TrackEventAsync(string eventName, Dictionary<string, string> properties)
        {
            if (IsTrackEventsEnabled)
            {
                Analytics.TrackEvent(eventName, properties);
            }
        }

        public async Task TrackPageViewAsync(string viewName)
        {
            await TrackPageViewAsync(viewName, null);
        }

        public async Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties)
        {
            if(IsTrackPageViewsEnabled)
            {
                if(properties == null)
                {
                    properties = new Dictionary<string, string>();
                }

                properties.Add("PageName", viewName);

                Analytics.TrackEvent("PageView", properties);
            }
        }
    }
}
