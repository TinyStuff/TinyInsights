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
        public bool IsTrackErrorsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsTrackPageViewsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsTrackEventsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task TrackErrorAsync(Exception ex)
        {
            throw new NotImplementedException();
        }

        public Task TrackEventAsync(string eventName)
        {
            throw new NotImplementedException();
        }

        public Task TrackEventAsync(string eventName, Dictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        public Task TrackPageViewAsync(string viewName)
        {
            throw new NotImplementedException();
        }

        public Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }
    }
}
