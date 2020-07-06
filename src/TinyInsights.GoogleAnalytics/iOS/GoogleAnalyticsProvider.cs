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
        protected ITracker Tracker { get; private set; }

        public bool IsTrackErrorsEnabled { get; set; } = true;
        public bool IsTrackPageViewsEnabled { get; set; } = true;
        public bool IsTrackEventsEnabled { get; set; } = true;
        public bool IsTrackDependencyEnabled { get; set; } = true;

        public GoogleAnalyticsProvider(string trackingId, bool catchUnhandledExceptions = true)
        {
            Gai.SharedInstance.DispatchInterval = 20;
            Gai.SharedInstance.TrackUncaughtExceptions = true;

            Tracker = Gai.SharedInstance.GetTracker(trackingId);
            Tracker.Set(GaiConstants.Language, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        }

        public virtual async Task TrackErrorAsync(Exception ex)
        {
            await TrackErrorAsync(ex, null);
        }

        public virtual async Task TrackErrorAsync(Exception ex, Dictionary<string, string> properties)
        {
            try
            {
                if (IsTrackErrorsEnabled)
                {
                    var builder = DictionaryBuilder.CreateException(ex.Message, 0);

                    if (properties != null)
                    {
                        foreach (var property in properties)
                        {
                            builder.Set(property.Key, property.Value);
                        }
                    }

                    Tracker.Send(builder.Build());
                }
            }
            catch (Exception)
            {
            }
        }

        public virtual async Task TrackEventAsync(string eventName)
        {
            await TrackEventAsync(eventName, null);
        }

        public virtual async Task TrackEventAsync(string eventName, Dictionary<string, string> properties)
        {
            try
            {
                string action = string.Empty;
                string label = string.Empty;
                int number = 0;

                if (properties != null && properties.ContainsKey("action"))
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
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        public virtual async Task TrackPageViewAsync(string viewName)
        {
            await TrackPageViewAsync(viewName, null);
        }

        public virtual async Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties)
        {
            try
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
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        public virtual async Task TrackDependencyAsync(string dependencyType, string dependencyName, DateTimeOffset startTime, TimeSpan duration, bool success, int resultCode = 0, Exception exception = null)
        {
            try
            {
                var dependencyToTrack = DictionaryBuilder.CreateTiming(dependencyType, duration.Milliseconds, dependencyName, success.ToString()).Build();

                dependencyToTrack.Add((NSString)"ResultCode", (NSString)resultCode.ToString());

                if (exception != null)
                {
                    dependencyToTrack.Add(NSObject.FromObject("Exception message"), NSObject.FromObject(exception.Message));
                    dependencyToTrack.Add(NSObject.FromObject("StackTrace"), NSObject.FromObject(exception.StackTrace));

                    if (exception.InnerException != null)
                    {
                        dependencyToTrack.Add(NSObject.FromObject("Inner exception message"), NSObject.FromObject(exception.InnerException.Message));
                        dependencyToTrack.Add(NSObject.FromObject("Inner exception stackTrace"), NSObject.FromObject(exception.InnerException.StackTrace));
                    }
                }

                Tracker.Send(dependencyToTrack);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
    }
}
