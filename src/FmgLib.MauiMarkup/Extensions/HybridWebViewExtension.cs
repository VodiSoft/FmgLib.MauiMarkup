// HybridWebView exists in .NET MAUI 9, but its initialization events were only added in .NET MAUI 10.
#if NET10_0_OR_GREATER

namespace FmgLib.MauiMarkup;

public static partial class HybridWebViewExtension
{

    public static T OnWebViewInitializing<T>(this T self, Action<T, WebViewInitializingEventArgs> action)
        where T : HybridWebView
    {
        self.WebViewInitializing += (o, args) => action(self, args);
        return self;
    }

    public static T OnWebViewInitialized<T>(this T self, Action<T, WebViewInitializedEventArgs> action)
        where T : HybridWebView
    {
        self.WebViewInitialized += (o, args) => action(self, args);
        return self;
    }
}

#endif
