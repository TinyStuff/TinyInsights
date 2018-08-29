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
    }
}
