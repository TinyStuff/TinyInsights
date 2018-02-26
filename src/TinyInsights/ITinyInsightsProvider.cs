using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyInsightsLib
{
    public interface ITinyInsightsProvider
    {
        bool IsTrackErrorsEnabled { get; set; }
        bool IsTrackPageViewsEnabled  { get; set; }
        bool IsTrackEventsEnabled  { get; set; }

        Task TrackErrorAsync(Exception ex);

        Task TrackPageViewAsync(string viewName);
        Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties);

        Task TrackEventAsync(string eventName);
        Task TrackEventAsync(string eventName, Dictionary<string, string> properties);
    }
}
