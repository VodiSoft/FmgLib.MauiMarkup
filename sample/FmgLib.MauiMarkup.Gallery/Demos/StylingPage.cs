using FmgLib.MauiMarkup.Gallery.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// <c>Style&lt;T&gt;</c>: the same fluent methods, producing setters instead of touching a control.
/// </summary>
public partial class StylingPage : DemoPage
{
    public StylingPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Styling";

    protected override string DemoSummary =>
        "Inside a Style<T> the property methods you already know define setters. One API, two contexts — and Style<T> converts implicitly to a MAUI Style, so it drops in anywhere.";

    protected override IView[] BuildSections() =>
    [
        Explicit(),
        Inheritance(),
        Implicit(),
        WithStates()
    ];

    private static readonly Style<Button> Ghost = new(e => e
        .BackgroundColor(Colors.Transparent)
        .TextColor(AppColors.Accent)
        .BorderColor(AppColors.Accent)
        .BorderWidth(1)
        .CornerRadius(12)
        .Padding(new Thickness(18, 12)));

    private static readonly Style<Button> Danger = new(e => e
        .BackgroundColor(AppColors.Danger)
        .TextColor(Colors.White)
        .CornerRadius(12)
        .Padding(new Thickness(18, 12)));

    private static IView Explicit()
        => Demo.Section(
            "Explicit styles",
            "Declare a style once, apply it by reference. A static styles class keeps the whole visual language in one file.",
            Demo.WrapStage(
                new Button().Text("Default").Margin(0, 0, Ui.GapSm, Ui.GapSm),
                new Button().Text("Ghost").Style(Ghost).Margin(0, 0, Ui.GapSm, Ui.GapSm),
                new Button().Text("Danger").Style(Danger).Margin(0, 0, Ui.GapSm, Ui.GapSm)
            ),
            Demo.Code("""
                private static readonly Style<Button> Ghost = new(e => e
                    .BackgroundColor(Colors.Transparent)
                    .TextColor(AppColors.Accent)
                    .BorderColor(AppColors.Accent)
                    .BorderWidth(1));

                new Button().Text("Ghost").Style(Ghost)
                """));

    private static IView Inheritance()
    {
        var baseText = new Style<Label>(e => e.FontFamily("OpenSansRegular").FontSize(14));
        var heading = new Style<Label>(baseText, e => e.FontSize(24).FontAttributes(Bold));
        var quiet = new Style<Label>(baseText, e => e.TextColor(AppColors.MutedLight).FontSize(12));

        return Demo.Section(
            "BasedOn",
            "A style can extend another, exactly like BasedOn in XAML — the constructor takes the parent first.",
            Demo.Stage(
                new Label().Text("Heading — inherits the family, overrides the size").Style(heading),
                new Label().Text("Base — 14pt OpenSansRegular").Style(baseText),
                new Label().Text("Quiet — inherits the family, overrides colour and size").Style(quiet)
            ),
            Demo.Code("""
                var baseText = new Style<Label>(e => e.FontFamily("OpenSansRegular").FontSize(14));
                var heading  = new Style<Label>(baseText, e => e.FontSize(24).FontAttributes(Bold));

                // apply to derived types too:
                var allButtons = new Style<Button>(applyToDerivedTypes: true, e => e.CornerRadius(8));
                """));
    }

    private static IView Implicit()
        => Demo.Section(
            "Implicit styles and scoping",
            "A style added to a ResourceDictionary applies to every control of that type in scope. Scope it to a layout and only that subtree changes — the same rules as XAML.",
            new Border()
            .Stage()
            .Resources(
                new ResourceDictionary
                {
                    new Style<Label>(e => e
                        .TextColor(AppColors.Magenta)
                        .FontAttributes(Italic)),

                    new Style<Border>(e => e
                        .Stroke(new SolidColorBrush(AppColors.Magenta))
                        .StrokeThickness(1)
                        .StrokeShape(new RoundRectangle().CornerRadius(10))),
                }
            )
            .Content(
                new VerticalStackLayout()
                .Spacing(Ui.GapSm)
                .Children(
                    new Label().Text("Every Label inside this Border is magenta and italic…"),
                    new Border()
                        .Padding(Ui.Gap)
                        .Content(new Label().Text("…including nested ones, and the Border itself is restyled too.").FontSize(12))
                )
            ),
            Demo.Code("""
                // App-wide, in App.cs:
                this.Resources(new ResourceDictionary().MergedDictionaries(AppStyles.Default));

                // …or scoped to one subtree:
                new Border().Resources(new ResourceDictionary
                {
                    new Style<Label>(e => e.TextColor(AppColors.Magenta)),
                })
                """),
            Demo.Note("This gallery's own look is one implicit dictionary in App.cs — see Theme/AppStyles.cs."));

    private static IView WithStates()
        => Demo.Section(
            "Styles carry more than setters",
            "A Style<T> supports collection-initializer syntax, so visual states, triggers and even Action<T> entries live inside it — which is how the gallery's buttons get their hover and pressed states.",
            Demo.Stage(
                new HorizontalStackLayout()
                .Spacing(Ui.GapSm)
                .Children(
                    new Button().Text("Hover me"),
                    new Button().Text("Disabled").IsEnabled(false)
                )
            ),
            Demo.Code("""
                new Style<Button>(e => e.CornerRadius(12).BackgroundColor(AppColors.Accent))
                {
                    new VisualState<Button>(VisualStates.Button.PointerOver, e => e
                        .BackgroundColor(AppColors.AccentDeep)),

                    new VisualState<Button>(VisualStates.Button.Disabled, e => e
                        .BackgroundColor(e => e.OnLight(AppColors.BorderLight).OnDark(AppColors.BorderDark))),
                }
                """));
}
