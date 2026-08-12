using FmgLib.MauiMarkup.Gallery.Controls;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// The library's core idea: every bindable property becomes a family of fluent methods.
/// </summary>
public partial class FluentPropertiesPage : DemoPage
{
    public FluentPropertiesPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Fluent Properties";

    protected override string DemoSummary =>
        "Every bindable property gets the same four overload shapes. Learn them once and the whole library — including generated third-party controls — follows.";

    protected override IView[] BuildSections() =>
    [
        DirectValues(),
        BuilderValues(),
        Shorthands(),
        EscapeHatches()
    ];

    private static IView DirectValues()
        => Demo.Section(
            "1 — Direct values",
            "The everyday shape. Each method returns the concrete control type, so the chain never loses type information.",
            Demo.Stage(
                new Label()
                    .Text("Chained, not configured")
                    .FontSize(22)
                    .FontAttributes(Bold)
                    .TextColor(AppColors.Accent)
                    .TextCenterHorizontal(),

                new Label()
                    .Text("After .Text(...) this is still a Label, so Label-only methods stay available.")
                    .Muted()
                    .TextCenterHorizontal()
            ),
            Demo.Code("""
                new Label()
                    .Text("Chained, not configured")
                    .FontSize(22)
                    .FontAttributes(Bold)
                    .TextColor(AppColors.Accent)
                    .TextCenterHorizontal()
                """));

    private static IView BuilderValues()
        => Demo.Section(
            "2 — The property builder",
            "The lambda overload is the gateway to everything that is not a constant: bindings, theme values, platform and idiom values, dynamic resources.",
            Demo.Stage(
                new Label()
                    .Text("Theme-aware — flip the theme from the home page and watch this repaint.")
                    .TextColor(e => e.OnLight(AppColors.AccentDeep).OnDark(AppColors.Warning))
                    .FontAttributes(Bold),

                new Label()
                    .Text("Idiom-aware — this text is larger on desktop than on a phone.")
                    .FontSize(e => e.OnPhone(13.0).OnTablet(16.0).OnDesktop(18.0).Default(14.0)),

                new Label()
                    .Text($"Platform-aware — the margin below differs per platform (running on {DeviceInfo.Platform}).")
                    .Margin(e => e
                        .OniOS(new Thickness(0, 0, 0, 12))
                        .OnAndroid(new Thickness(0, 0, 0, 6))
                        .Default(new Thickness(0)))
            ),
            Demo.Code("""
                new Label()
                    .TextColor(e => e.OnLight(AppColors.AccentDeep).OnDark(AppColors.Warning))
                    .FontSize(e => e.OnPhone(13.0).OnTablet(16.0).OnDesktop(18.0).Default(14.0))
                    .Margin(e => e.OniOS(new Thickness(0, 0, 0, 12)).Default(new Thickness(0)))
                """),
            Demo.Note("OnLight/OnDark produces a real AppThemeBinding — the value follows later theme changes instead of being resolved once."));

    private static IView Shorthands()
        => Demo.Section(
            "Shorthands",
            "Beyond 1:1 property mapping, the library adds the combinations you would otherwise write by hand.",
            Demo.WrapStage(
                Demo.Chip(".SizeRequest(w, h)", AppColors.Accent),
                Demo.Chip(".Margin(h, v)", AppColors.Violet),
                Demo.Chip(".Center()", AppColors.Magenta),
                Demo.Chip(".TextCenter()", AppColors.Info),
                Demo.Chip(".GridSpan(2, 1)", AppColors.Success),
                Demo.Chip("\"#6366F1\".ToColor()", AppColors.Warning)
            ),
            Demo.Code("""
                new BoxView().SizeRequest(64, 64)          // Width + HeightRequest
                new Label().Margin(16, 8)                  // Thickness built for you
                new Label().Center()                       // both LayoutOptions
                new Label().TextCenter()                   // both TextAlignments
                new Border().GridSpan(column: 2, row: 1)   // both Grid spans
                """));

    private static IView EscapeHatches()
        => Demo.Section(
            "Never blocked",
            "Two escape hatches guarantee anything the fluent API does not cover is still one line, inside the chain.",
            Demo.Stage(
                new Entry()
                    .Placeholder("InvokeOnElement set my ReturnType and keyboard")
                    .InvokeOnElement(entry =>
                    {
                        entry.ReturnType = ReturnType.Done;
                        entry.Keyboard = Keyboard.Text;
                    })
            ),
            Demo.Code("""
                new Entry()
                    .Placeholder("Name")
                    .InvokeOnElement(entry => entry.ReturnType = ReturnType.Done)

                // …or just keep the reference and assign afterwards — views are plain objects.
                var entry = new Entry().Placeholder("Name");
                entry.ReturnType = ReturnType.Done;
                """));
}
