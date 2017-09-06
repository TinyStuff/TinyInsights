using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyInsights
{
    public interface ITinyInsights
    {
        Task TrackErrorAsync(Exception ex);

        Task TrackPageViewAsync(string viewname);
        Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties);

        Task TrackEventAsync(string eventName);
        Task TrackEventAsync(string eventName, Dictionary<string, string> properties);
    }
}
