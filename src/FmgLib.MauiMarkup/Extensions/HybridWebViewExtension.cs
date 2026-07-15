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
