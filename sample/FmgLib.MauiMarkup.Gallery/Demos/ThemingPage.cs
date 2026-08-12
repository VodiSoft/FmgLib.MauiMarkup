using FmgLib.MauiMarkup.Gallery.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Theming: AppThemeBinding for light/dark, dynamic resources for user-chosen accents.
/// </summary>
public partial class ThemingPage : DemoPage
{
    private const string AccentKey = "GalleryAccent";

    public ThemingPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Theming";

    protected override string DemoSummary =>
        "OnLight/OnDark produces a real AppThemeBinding — the value follows later theme changes instead of being resolved once. DynamicResource does the same for values you choose at runtime.";

    protected override IView[] BuildSections() =>
    [
        ThemeSwitch(),
        ThemeAware(),
        DynamicResources(),
        Strategies()
    ];

    private static IView ThemeSwitch()
        => Demo.Section(
            "Switch the theme",
            "Nothing on this page is rebuilt when you press these. Every colour is a binding, so the controls already on screen repaint themselves.",
            Demo.Stage(
                new HorizontalStackLayout()
                .Spacing(Ui.GapSm)
                .CenterHorizontal()
                .Children(
                    ThemeButton("☀️ Light", AppTheme.Light),
                    ThemeButton("🌙 Dark", AppTheme.Dark),
                    ThemeButton("🌗 System", AppTheme.Unspecified)
                ),

                new Label()
                    .Text("The gallery's entire look is one implicit ResourceDictionary in App.cs — every style in it uses OnLight/OnDark.")
                    .Muted()
                    .FontSize(12)
                    .TextCenterHorizontal()
            ),
            Demo.Code("""
                Application.Current.UserAppTheme = AppTheme.Dark;

                // …and every colour in the app already follows it:
                new Label().TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White))
                """));

    private static View ThemeButton(string text, AppTheme theme)
        => new Button()
            .Text(text)
            .OnClicked(_ =>
            {
                if (Application.Current is { } app)
                    app.UserAppTheme = theme;
            });

    private static IView ThemeAware()
        => Demo.Section(
            "Theme-aware values",
            "OnLight, OnDark and Default cover every Brush, Color, Thickness or double — anything a property takes.",
            Demo.WrapStage(
                Demo.Swatch("BackgroundColor", border => border
                    .BackgroundColor(e => e.OnLight(AppColors.Accent).OnDark(AppColors.Warning))),

                Demo.Swatch("Background brush", border => border
                    .Background(e => e
                        .OnLight(Ui.BrandGradient())
                        .OnDark(new LinearGradientBrush()
                            .StartPoint(new Point(0, 0))
                            .EndPoint(new Point(1, 1))
                            .GradientStops(
                                new GradientStop(AppColors.Success, 0f),
                                new GradientStop(AppColors.Cyan, 1f))))),

                Demo.Swatch("Stroke", border => border
                    .BackgroundColor(Colors.Transparent)
                    .StrokeThickness(3)
                    .Stroke(e => e
                        .OnLight(new SolidColorBrush(AppColors.Magenta))
                        .OnDark(new SolidColorBrush(AppColors.Cyan))))
            ),
            Demo.Code("""
                new Border()
                    .BackgroundColor(e => e.OnLight(AppColors.Accent).OnDark(AppColors.Warning))
                    .Stroke(e => e
                        .OnLight(new SolidColorBrush(Colors.Magenta))
                        .OnDark(new SolidColorBrush(Colors.Cyan)))
                """),
            Demo.Note("A nested builder — .OnDark(l => l.DynamicResource(\"X\")) — cannot be carried by a theme binding, so it resolves once. Keep plain values wherever the theme must switch at runtime."));

    private static IView DynamicResources()
    {
        // The resource has to exist before anything binds to it.
        if (Application.Current is { } app && !app.Resources.ContainsKey(AccentKey))
            app.Resources[AccentKey] = AppColors.Accent;

        return Demo.Section(
            "Dynamic resources",
            "A theme the USER picks is not light/dark — it is an arbitrary value swapped at runtime. DynamicResource tracks the key, so replacing the resource updates every control bound to it.",
            Demo.Stage(
                new Border()
                    .StrokeThickness(0)
                    .StrokeShape(new RoundRectangle().CornerRadius(14))
                    .HeightRequest(76)
                    .BackgroundColor(e => e.DynamicResource(AccentKey))
                    .Content(new Label().Text("BackgroundColor → DynamicResource").TextColor(Colors.White).FontAttributes(Bold).TextCenter()),

                new Label()
                    .Text("Pick an accent — both this text and the panel above are bound to the same key.")
                    .Muted()
                    .FontSize(12)
                    .TextColor(e => e.DynamicResource(AccentKey)),

                new HorizontalStackLayout()
                .Spacing(Ui.GapSm)
                .CenterHorizontal()
                .Children(
                    AccentButton(AppColors.Accent),
                    AccentButton(AppColors.Magenta),
                    AccentButton(AppColors.Success),
                    AccentButton(AppColors.Warning),
                    AccentButton(AppColors.Cyan)
                )
            ),
            Demo.Code("""
                new Border().BackgroundColor(e => e.DynamicResource("GalleryAccent"))

                // later, anywhere:
                Application.Current.Resources["GalleryAccent"] = Colors.Purple;
                """));
    }

    private static View AccentButton(Color color)
        => new Border()
            .SizeRequest(38, 38)
            .StrokeThickness(0)
            .StrokeShape(new RoundRectangle().CornerRadius(999))
            .BackgroundColor(color)
            .GestureRecognizers(
                new TapGestureRecognizer().OnTapped((_, _) =>
                {
                    if (Application.Current is { } app)
                        app.Resources[AccentKey] = color;
                })
            );

    private static IView Strategies()
        => Demo.Section(
            "Which tool for which job",
            "The three theming mechanisms are complementary, not alternatives.",
            new VerticalStackLayout()
            .Spacing(Ui.GapSm)
            .Children(
                Demo.Note("OnLight/OnDark — the OS light/dark switch, and Application.UserAppTheme. Follows changes by itself.", "🌗"),
                Demo.Note("DynamicResource — values the user picks: accents, densities, brand palettes. Swap the resource, the UI follows.", "🎨"),
                Demo.Note("Merged dictionaries — swapping a whole style set at once, e.g. compact vs. comfortable.", "📚")
            ));
}
