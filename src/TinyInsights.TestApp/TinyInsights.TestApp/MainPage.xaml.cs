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
	}
}
