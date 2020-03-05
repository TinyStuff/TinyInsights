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
        private Tracker tracker;
        
        public bool IsTrackErrorsEnabled { get; set; } = true;
        public bool IsTrackPageViewsEnabled { get; set; } = true;
        public bool IsTrackEventsEnabled { get; set; } = true;
        public bool IsTrackDependencyEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public GoogleAnalyticsProvider(string trackingId, bool catchUnhandledExceptions = true)
        {
            instance = Android.Gms.Analytics.GoogleAnalytics.GetInstance(Android.App.Application.Context);

            tracker = instance.NewTracker(trackingId);
            tracker.EnableExceptionReporting(catchUnhandledExceptions);
            tracker.EnableAutoActivityTracking(false);
            tracker.SetLanguage(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        }

        public virtual async Task TrackErrorAsync(Exception ex)
        {
            await TrackErrorAsync(ex , null);
        }

        public virtual async Task TrackErrorAsync(Exception ex, Dictionary<string, string> properties)
        {
            if (IsTrackErrorsEnabled)
            {
                var builder = new HitBuilders.ExceptionBuilder();
                builder.SetDescription(ex.Message);
                builder.SetFatal(false);

                if(properties != null)
                {
                    foreach(var property in properties)
                    {
                        builder.Set(property.Key, property.Value);
                    }
                }

                var exceptionToTrack = builder.Build();

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

            if(properties != null)
            {
                foreach(var property in properties)
                {
                    if(property.Key != "action" && property.Key != "label" && property.Key != "number")
                    {
                        eventToTrack.Add(property.Key, property.Value);
                    }
                }
            }

            tracker.Send(eventToTrack);
        }

        public virtual async Task TrackPageViewAsync(string viewName)
        {
            await TrackPageViewAsync(viewName, null);
        }

        public virtual async Task TrackPageViewAsync(string viewName, Dictionary<string, string> properties)
        {
            tracker.SetScreenName(viewName);

            var viewToTrack = new HitBuilders.ScreenViewBuilder().Build();

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    viewToTrack.Add(property.Key, property.Value);
                }
            }

            tracker.Send(viewToTrack);
        }

        public async Task TrackDependencyAsync(string depenencyType, string dependencyName, TimeSpan duration)
        {
            var builder = new HitBuilders.TimingBuilder();
            builder.SetCategory(depenencyType);
            builder.SetVariable(dependencyName);
            builder.SetValue(duration.Milliseconds);

            var dependencyToTrack = builder.Build();

            tracker.Send(dependencyToTrack);
        }
    }
}
