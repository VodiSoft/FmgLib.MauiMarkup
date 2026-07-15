namespace FmgLib.MauiMarkup;

public static partial class SelectableItemsViewExtension
{
    
    public static T SelectedItems<T>(this T self,
        params object[] selectedItems)
        where T : SelectableItemsView
    {
        foreach (var item in selectedItems)
            self.SelectedItems.Add(item);
        return self;
    }

    public static T SelectedItems<T>(this T self,
        Func<object[]> configure)
        where T : SelectableItemsView
    {
        var selectedItems = configure();
        foreach (var item in selectedItems)
            self.SelectedItems.Add(item);
        return self;
    }
    
}
