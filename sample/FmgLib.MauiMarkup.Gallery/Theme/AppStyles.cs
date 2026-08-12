using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup.Gallery.Theme;

/// <summary>
/// App-wide implicit styles.
///
/// The same fluent methods used on live controls define the setters here — one API, two contexts.
/// Colours go through <c>OnLight/OnDark</c>, so every styled control follows the theme by itself;
/// the gallery's theme switch repaints the running UI without rebuilding a single page.
/// </summary>
public static class AppStyles
{
    public static ResourceDictionary Default { get; } = new()
    {
        new Style<Page>(e => e
            .BackgroundColor(e => e.OnLight(AppColors.PageLight).OnDark(AppColors.PageDark))),

        new Style<Label>(e => e
            .FontFamily("OpenSansRegular")
            .FontSize(15)
            .TextColor(e => e.OnLight(AppColors.TextLight).OnDark(AppColors.TextDark))),

        new Style<Button>(e => e
            .FontFamily("OpenSansSemibold")
            .FontSize(14)
            .TextColor(Colors.White)
            .BackgroundColor(AppColors.Accent)
            .CornerRadius(12)
            .Padding(new Thickness(18, 12))
            .MinimumHeightRequest(44))
        {
            new VisualState<Button>(VisualStates.Button.Normal, e => e
                .BackgroundColor(AppColors.Accent)
                .Opacity(1.0)),

            new VisualState<Button>(VisualStates.Button.PointerOver, e => e
                .BackgroundColor(AppColors.AccentDeep)),

            new VisualState<Button>(VisualStates.Button.Pressed, e => e
                .Opacity(0.85)),

            new VisualState<Button>(VisualStates.Button.Disabled, e => e
                .BackgroundColor(e => e.OnLight(AppColors.BorderLight).OnDark(AppColors.BorderDark))
                .TextColor(e => e.OnLight(AppColors.MutedLight).OnDark(AppColors.MutedDark))),
        },

        new Style<Entry>(e => e
            .FontFamily("OpenSansRegular")
            .FontSize(15)
            .TextColor(e => e.OnLight(AppColors.TextLight).OnDark(AppColors.TextDark))
            .PlaceholderColor(e => e.OnLight(AppColors.MutedLight).OnDark(AppColors.MutedDark))
            .BackgroundColor(Colors.Transparent)
            .MinimumHeightRequest(44)),

        new Style<Editor>(e => e
            .FontFamily("OpenSansRegular")
            .FontSize(15)
            .TextColor(e => e.OnLight(AppColors.TextLight).OnDark(AppColors.TextDark))
            .PlaceholderColor(e => e.OnLight(AppColors.MutedLight).OnDark(AppColors.MutedDark))
            .BackgroundColor(Colors.Transparent)),

        new Style<SearchBar>(e => e
            .FontFamily("OpenSansRegular")
            .FontSize(15)
            .TextColor(e => e.OnLight(AppColors.TextLight).OnDark(AppColors.TextDark))
            .PlaceholderColor(e => e.OnLight(AppColors.MutedLight).OnDark(AppColors.MutedDark))
            .CancelButtonColor(AppColors.Accent)
            .BackgroundColor(Colors.Transparent)),

        new Style<Border>(e => e
            .Stroke(new SolidColorBrush(AppColors.BorderLight))
            .StrokeThickness(1)
            .StrokeShape(new RoundRectangle().CornerRadius(16))),

        new Style<Switch>(e => e
            .OnColor(AppColors.Accent)
            .ThumbColor(Colors.White)),

        new Style<Slider>(e => e
            .MinimumTrackColor(AppColors.Accent)
            .MaximumTrackColor(e => e.OnLight(AppColors.BorderLight).OnDark(AppColors.BorderDark))
            .ThumbColor(AppColors.Accent)),

        new Style<ActivityIndicator>(e => e
            .Color(AppColors.Accent)),

        new Style<ProgressBar>(e => e
            .ProgressColor(AppColors.Accent)),

        new Style<CheckBox>(e => e
            .Color(AppColors.Accent)),

        new Style<RadioButton>(e => e
            .TextColor(e => e.OnLight(AppColors.TextLight).OnDark(AppColors.TextDark))),
    };
}
