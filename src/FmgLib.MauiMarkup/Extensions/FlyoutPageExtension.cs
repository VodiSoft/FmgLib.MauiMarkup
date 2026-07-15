namespace FmgLib.MauiMarkup;

public static partial class FlyoutPageExtension
{
    
    public static T OnBackButtonPressed<T>(this T self, EventHandler<BackButtonPressedEventArgs> handler)
        where T : FlyoutPage
    {
        self.BackButtonPressed += handler;
        return self;
    }
    
    public static T OnBackButtonPressed<T>(this T self, Action<T> action)
        where T : FlyoutPage
    {
        self.BackButtonPressed += (o, arg) => action(self);
        return self;
    }
    
}