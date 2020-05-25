using Android.App;
using Firebase.Analytics;
using Crashlytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace TinyInsightsLib.Firebase
{
    public class FirebaseProvider : ITinyInsightsProvider
    {
        public bool IsTrackErrorsEnabled { get; set; }
        public bool IsTrackPageViewsEnabled { get; set; }
        public bool IsTrackEventsEnabled { get; set; }
        public bool IsTrackDependencyEnabled { get; set; }


        public async Task TrackDependencyAsync(string dependencyType, string dependencyName, DateTimeOffset startTime, TimeSpan duration, bool success, int resultCode = 0, Exception exception = null)
        {
            await Task.Run(() =>
            {
                if(IsTrackDependencyEnabled)
                {
                    var properties = new Dictionary<string, string>();

                    properties.Add("DependencyType", dependencyType);
                    properties.Add("DependencyName", dependencyName);
                    properties.Add("Duration", duration.ToString());
                    properties.Add("StartTime", startTime.ToString());
                    properties.Add("Success", success.ToString());
                    properties.Add("ResultCode", resultCode.ToString());

                    if (exception != null)
                    {
                        properties.Add("Exception message", exception.Message);
                        properties.Add("StackTrace", exception.StackTrace);

                        if (exception.InnerException != null)
                        {
                            properties.Add("Inner exception message", exception.InnerException.Message);
                            properties.Add("Inner exception stackTrace", exception.InnerException.StackTrace);
                        }
                    }

                    FirebaseAnalytics.GetInstance(Application.Context).LogEvent("Dependency", properties.ToBundle());
                }
            });
        }

        public async Task TrackErrorAsync(Exception ex, Dictionary<string, string> properties = null)
        {
            await Task.Run(() =>
            {
                if(IsTrackErrorsEnabled)
                {
                    if (properties == null)
                    {
                        properties = new Dictionary<string, string>();
                    }

                    properties.Add("Message", ex.Message);
                    properties.Add("StackTrace", ex.StackTrace);

                    if (ex.InnerException != null)
                    {
                        properties.Add("InnerMessage", ex.InnerException.Message);
                        properties.Add("InnerStackTrace", ex.InnerException.StackTrace);
                    }

                    Crashlytics.Crashlytics.LogException(MonoExceptionHelper.FromException(ex));
                }
            });
        }

        public async Task TrackEventAsync(string eventName, Dictionary<string, string> properties = null)
        {
            await Task.Run(() =>
            {
                if (IsTrackEventsEnabled)
                {
                    if (properties == null)
                    {
                        properties = new Dictionary<string, string>();
                    }

                    FirebaseAnalytics.GetInstance(Application.Context).LogEvent(eventName, properties.ToBundle());
                }
            });
        }

        public async Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties = null)
        {
            await Task.Run(() =>
            {
                if (IsTrackPageViewsEnabled)
                {
                    if (properties == null)
                    {
                        properties = new Dictionary<string, string>();
                    }

                    properties.Add("PageName", viewName);

                    FirebaseAnalytics.GetInstance(Application.Context).LogEvent("PageView", properties.ToBundle());
                }
            });
        }
    }
}
