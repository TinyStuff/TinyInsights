# TinyInsights

## Build status
<img src="https://io2gamelabs.visualstudio.com/_apis/public/build/definitions/be16d002-5786-41a1-bf3b-3e13d5e80aa0/8/badge" alt="build status" />

## About
The idea behind TinyInsights is to build a cross platform library to use with your favorite Analytics provider. Right now there are a providers for AppCenter and Google Analytics. And you are of course welcome to contribute with your favorite provider.

## Get started
Install the Nuget package for the provider you want to use in the platform project, in other projects use the TinyInsights package.


For Azure App Center install the package TinyInsights.AppCenter in your platform projects.

```
Ìnstall-Package TinyInsights.AppCenter
```

For other projects, you can install just the TinyInsights package.

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
TinyInsights.Configure(appCenterProvider, provider 2, provider3);
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
