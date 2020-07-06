# TinyInsights

## Build status
<img src="https://io2gamelabs.visualstudio.com/_apis/public/build/definitions/be16d002-5786-41a1-bf3b-3e13d5e80aa0/8/badge" alt="build status" />

## About
The idea behind TinyInsights is to build a cross platform library to use with your favorite Analytics provider. Right now there are a providers for AppCenter, Application Insights and Google Analytics. And you are of course welcome to contribute with your favorite provider.

## Release notes
1.1 - Introducing an Application Insigts provider and a new method for tracking dependencies. Read more in this blog post, https://danielhindrikes.se/index.php/2020/03/10/application-insights-for-xamarin-and-uwp-apps-with-tinyinsights/

## Get started
Install the Nuget package for the provider you want to use in the platform project, in other projects use the TinyInsights package.


For Microsoft AppCenter install the package TinyInsights.AppCenter.

```
Ìnstall-Package TinyInsights.AppCenter
```

For Azure ApplicationInsights install the package TinyInsights.ApplicationInsights.

```
Ìnstall-Package TinyInsights.ApplicationInsights
```

If you have more projects in yout solution and want to use TinyInsights in them, you can install just the TinyInsights package. The "provider package" is just needed in the procject handling the configuration.

```
Ìnstall-Package TinyInsights
```

### Configure TinyInsights
```csharp
var appCenterProvider = new AppCenterProvider(iOSKey, AndroidKey, UWPKey)

TinyInsights.Configure(appCenterProvider);
```

When multiple providers will be available you can use them simultaneously, just  use configure for all of the providers.

```csharp
var appInsightsProvider = new ApplicationInsightsProvider("{InstrumentationKey}");

TinyInsights.Configure(appCenterProvider, appInsightsProvider);
```

If you have multiple providers you can configure what to track with each provider. By default everything is tracked.
```csharp
appCenterProvider.IsTrackPageViewsEnabled = false;
appCenterProvider.IsTrackEventsEnabled = false;
appCenterProvider.IsTrackDependencyEnabled = false;

appInsightsProvider.IsTrackErrorsEnabled = false;
```

### Track errors

```csharp
catch(Ecception ex)
{
     await TinyInsights.TrackErrorAsync(ex);
}

//with properties
var properties = new  Dictionarty<string, string>();
properties.Add("MyFirstProperty", "MyFirstValue");
properties.Add("MySecondProperty", "MySeconndValue");

catch(Ecception ex)
{
     await TinyInsights.TrackErrorAsync(ex, properties);
}
```

### Track page views
```csharp
await TinyInsights.TrackPageViewAsync("SuperCoolView");

//with properties
var properties = new  Dictionarty<string, string>();
properties.Add("MyFirstProperty", "MyFirstValue");
properties.Add("MySecondProperty", "MySeconndValue");

await TinyInsights.TrackPageViewAsync("SuperCoolView", properties);
```

### Track custom events
```csharp
await TinyInsights.TrackEventAsync("SuperCoolEvent");

//with properties
var properties = new  Dictionarty<string, string>();
properties.Add("MyFirstProperty", "MyFirstValue");
properties.Add("MySecondProperty", "MySeconndValue");

await TinyInsights.TrackEventAsync("SuperCoolEvent", properties);
```

### Track dependencies
There are a two of ways to track dependencies with TinyInsights.
The first and the basic method is <strong>TrackDependencyAsync</strong>, and is also used in the background by the other way to do it.

```csharp
var startTime = DateTimeOffset.Now;

var success = await GetData();

var duration = DateTimeOffset.Now - startTime

await TinyInsights.TrackDependencyAsync("api.mydomain.se", "https://api/mydomain.se/v1/data/get", startTime, duration, success);
```

The second way is to create a TinyDependency object that handles most of the tracking for you. You will do that by just by wrapping your code for the dependency in a using statement. 
```csharp
using (var tracker = TinyInsights.CreateDependencyTracker("api.mydomain.se", "https://api/mydomain.se/v1/data/get"))
{
     await GetData();
}
```

If the dependency succeded that is fine, but if it not you need to handle that on your own, using the <strong>Finish</strong> method of the TinyDependency object.
```csharp
using (var tracker = TinyInsights.CreateDependencyTracker("api.mydomain.se", "https://api/mydomain.se/v1/data/get"))
{
     try
     {
          var repsonse = await GetData();
     
          if(!response.IsSuccessStatusCode)
          {
               await tracker.Finish(false, (int)response.StatusCode);
          }
     }
     catch(Exception ex)
     {
          tracker.Finish(false, ex);
     }
}
```
