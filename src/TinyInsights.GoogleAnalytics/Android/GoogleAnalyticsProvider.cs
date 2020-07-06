using Android.Gms.Analytics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;



namespace TinyInsightsLib.GoogleAnalytics
{
    public class GoogleAnalyticsProvider : ITinyInsightsProvider
    {
        private Android.Gms.Analytics.GoogleAnalytics instance;
        protected Tracker Tracker { get; private set; }
        
        public bool IsTrackErrorsEnabled { get; set; } = true;
        public bool IsTrackPageViewsEnabled { get; set; } = true;
        public bool IsTrackEventsEnabled { get; set; } = true;
        public bool IsTrackDependencyEnabled { get; set; } = true;

        public GoogleAnalyticsProvider(string trackingId, bool catchUnhandledExceptions = true)
        {
            instance = Android.Gms.Analytics.GoogleAnalytics.GetInstance(Android.App.Application.Context);

            Tracker = instance.NewTracker(trackingId);
            Tracker.EnableExceptionReporting(catchUnhandledExceptions);
            Tracker.EnableAutoActivityTracking(false);
            Tracker.SetLanguage(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        }

        public virtual async Task TrackErrorAsync(Exception ex)
        {
            await TrackErrorAsync(ex , null);
        }

        public virtual async Task TrackErrorAsync(Exception ex, Dictionary<string, string> properties)
        {
            try
            {
                if (IsTrackErrorsEnabled)
                {
                    var builder = new HitBuilders.ExceptionBuilder();
                    builder.SetDescription(ex.Message);
                    builder.SetFatal(false);

                    if (properties != null)
                    {
                        foreach (var property in properties)
                        {
                            builder.Set(property.Key, property.Value);
                        }
                    }

                    var exceptionToTrack = builder.Build();

                    Tracker.Send(exceptionToTrack);
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

                if (properties != null && properties.ContainsKey("label"))
                {
                    label = properties["label"];
                }

                if (properties != null && properties.ContainsKey("number"))
                {
                    int.TryParse(properties["number"], out number);
                }

                var builder = new HitBuilders.EventBuilder();
                builder.SetCategory(eventName.ToLower());
                builder.SetAction(action.ToLower());
                builder.SetLabel(label.ToLower());
                builder.SetValue(number);

                var eventToTrack = builder.Build();

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
                Tracker.SetScreenName(viewName);

                var viewToTrack = new HitBuilders.ScreenViewBuilder().Build();

                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        viewToTrack.Add(property.Key, property.Value);
                    }
                }

                Tracker.Send(viewToTrack);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        public async Task TrackDependencyAsync(string dependencyType, string dependencyName, DateTimeOffset startTime, TimeSpan duration, bool success, int resultCode = 0, Exception exception = null)
        {
            try
            {
                var builder = new HitBuilders.TimingBuilder();
                builder.SetCategory(dependencyType);
                builder.SetVariable(dependencyName);
                builder.SetValue(duration.Milliseconds);
                builder.SetLabel(success.ToString());

                var dependencyToTrack = builder.Build();

                dependencyToTrack.Add("ResultCode", resultCode.ToString());

                if (exception != null)
                {
                    dependencyToTrack.Add("Exception message", exception.Message);
                    dependencyToTrack.Add("StackTrace", exception.StackTrace);

                    if (exception.InnerException != null)
                    {
                        dependencyToTrack.Add("Inner exception message", exception.InnerException.Message);
                        dependencyToTrack.Add("Inner exception stackTrace", exception.InnerException.StackTrace);
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
