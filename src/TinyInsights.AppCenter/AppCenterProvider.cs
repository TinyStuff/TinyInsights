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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="iOSkey">The AppCenter key for iOS</param>
		/// <param name="androidKey">The AppCenter key for Android</param>
		/// <param name="uwpKey">The AppCenter key for uwp</param>
		/// <param name="additonalServices">Analytics and Crashes are default</param>
		public AppCenterProvider(string iOSkey, string androidKey, string uwpKey, params Type[] additonalServices)
        {
			var services = new List<Type>()
			{
				typeof(Analytics),
				typeof(Crashes)
			};

            Microsoft.AppCenter.AppCenter.Start($"ios={iOSkey};uwp={uwpKey};android={androidKey}", services.ToArray());
        }

        public bool IsTrackErrorsEnabled { get; set; } = true;
        public bool IsTrackPageViewsEnabled { get; set; } = true;
        public bool IsTrackEventsEnabled { get; set; } = true;
        public bool IsTrackDependencyEnabled { get; set; }

        public async Task TrackDependencyAsync(string dependencyType, string dependencyName, DateTimeOffset startTime, TimeSpan duration, bool success)
        {
            if(IsTrackDependencyEnabled)
            {

                var properties = new Dictionary<string, string>();

                properties.Add("DependencyType", dependencyType);
                properties.Add("DependencyName", dependencyName);
                properties.Add("Duration", duration.ToString());
                properties.Add("StartTime", startTime.ToString());
                properties.Add("Success", success.ToString());

                Analytics.TrackEvent("Dependency", properties);
            }
        }

        public virtual async Task TrackErrorAsync(Exception ex, Dictionary<string, string> properties)
        {
            if (IsTrackEventsEnabled)
            {
                Crashes.TrackError(ex, properties);
            }
        }

        public virtual async Task TrackEventAsync(string eventName, Dictionary<string, string> properties)
        {
            if (IsTrackEventsEnabled)
            {
                Analytics.TrackEvent(eventName, properties);
            }
        }

        public virtual async Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties)
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
