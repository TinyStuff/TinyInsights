using TinyInsightsLib.AppCenter;
using Xamarin.Forms;
using TinyInsightsLib;
using TinyInsightsLib.ApplicationInsights;

namespace TinyInsights.TestApp
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

			//var appCenterProvider = new AppCenterProvider("e1163c6a-3f60-4bfe-9039-7ee45392677c", "1e822e4e-ba5d-40c2-9371-3771c5ddd689", "497da7dd-9c59-4a14-8b35-ed1bcb602091");
			//var googleAnalyticsProvider = new GoogleAnalyticsProvider("UA-115289930-1");

			var appInsightsProvider = new ApplicationInsightsProvider("73b84b05-1c99-48cf-82fe-d6c595a13c7f");


			TinyInsightsLib.TinyInsights.Configure(appInsightsProvider);

			MainPage = new NavigationPage(new TinyInsights.TestApp.MainPage());
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
