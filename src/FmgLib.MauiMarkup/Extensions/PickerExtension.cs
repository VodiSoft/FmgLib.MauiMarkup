namespace FmgLib.MauiMarkup;

public static partial class PickerExtension
{
    public static T Open<T>(this T self)
        where T : Picker
    {
        self.Open();
        return self;
    }

    public static T Close<T>(this T self)
        where T : Picker
    {
        self.Close();
        return self;
    }

}
