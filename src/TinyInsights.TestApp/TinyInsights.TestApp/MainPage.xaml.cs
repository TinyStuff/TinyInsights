using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TinyInsights.TestApp
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            TinyInsightsLib.TinyInsights.TrackPageViewAsync("MainPage");
		}

        private void Crash_Clicked(object sender, EventArgs e)
        {
            throw new Exception("CRASH :(");
        }

        private void Throw_Clicked(object sender, EventArgs e)
        {
            try
            {
                throw new NullReferenceException("This is handled");
            }
            catch(Exception ex)
            {
                TinyInsightsLib.TinyInsights.TrackErrorAsync(ex);
            }
        }

        int count = 0;

        private void PageView_Clicked(object sender, EventArgs e)
        {
            TinyInsightsLib.TinyInsights.TrackPageViewAsync("MainPage"+count);

            count++;
        }

        private async void Dependency_Clicked(object sender, EventArgs e)
        {

            using (var tracker = TinyInsightsLib.TinyInsights.CreateDependencyTracker("DELAY", "Random"))
            {
                var random = new Random();
                var delay = random.Next(200, 500);

                await Task.Delay(delay);

                await tracker.Finish(false);
            }
        }

        private async void DependencyFail_Clicked(object sender, EventArgs e)
        {
            var startTime = DateTimeOffset.Now;

            var random = new Random();
            var delay = random.Next(200, 500);

            await Task.Delay(delay);

            var duration = DateTimeOffset.Now - startTime;

            await TinyInsightsLib.TinyInsights.TrackDependencyAsync("DELAY", "Random", startTime, duration, false);
        }
    }
}
