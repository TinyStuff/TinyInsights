# TinyInsights

## Build status
<img src="https://io2gamelabs.visualstudio.com/_apis/public/build/definitions/be16d002-5786-41a1-bf3b-3e13d5e80aa0/8/badge" alt="build status" />

## About
The idea behind TinyInsights is to build a cross platform library to use with your favorite Analytics provider. Right now there are a provider for App Center, but more will come. And your ofcourse welcome to contribute with your favorite provider.

## Get started
Install the Nuget package for the provider you want to use in the platform project, in other projects use the TinyInsights package.


For Azure App Center install the package TinyInsights.AppCenter in your platform projects.

```
Ìnstall-package TinyInsights.AppCenter
```

For other projects, you can install just the TinyInsights package.

```
Ìnstall-package TinyInsights
```

### Configure TinyInsights
```csharp
var appCenterProvider = new AppCenterProvider(iOSKey, AndroidKey, UWPKey)

TinyInsights.Configure(appCenterProvider);
```

