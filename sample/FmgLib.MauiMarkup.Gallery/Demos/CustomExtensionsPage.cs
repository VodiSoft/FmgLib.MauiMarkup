using FmgLib.MauiMarkup.Gallery.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Extending the library with your own fluent vocabulary — the gallery's own design system is the
/// worked example.
/// </summary>
public partial class CustomExtensionsPage : DemoPage
{
    public CustomExtensionsPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Custom Extensions";

    protected override string DemoSummary =>
        "Because every fluent method is generic over T and returns T, your own helpers compose exactly like the built-in ones.";

    protected override IView[] BuildSections() =>
    [
        Composition(),
        ThisGallery(),
        FullProperty()
    ];

    private static IView Composition()
        => Demo.Section(
            "Level 1 — composition shorthands",
            "Wrap a recurring chain in a static method. Keep the generic parameter and the constraint, and the concrete type flows straight through.",
            Demo.Stage(
                new Label().Text("A plain Label").FontSize(15),
                new Label().Text("The same Label with .Heading()").Heading(),
                new Label().Text("…and with .Overline()").Overline()
            ),
            Demo.Code("""
                public static T Heading<T>(this T self) where T : Label
                    => self
                        .FontSize(19)
                        .FontAttributes(Bold)
                        .TextColor(e => e.OnLight(AppColors.TextLight).OnDark(AppColors.TextDark));

                new Label().Text("Section").Heading().TextCenter()   // still a Label
                """));

    private static IView ThisGallery()
        => Demo.Section(
            "This app is the example",
            "Every surface in the gallery is one of a handful of helpers. The pages then read as intent instead of as a wall of colours and paddings.",
            Demo.Stage(
                new Label().Text("Ui.Card()").Mono(),
                new Border().Card(12).Padding(Ui.Gap).Content(new Label().Text("A card").Muted()),

                new Label().Text("Ui.Stage()").Mono().Margin(0, Ui.GapSm, 0, 0),
                new Border().Stage(12).Content(new Label().Text("A stage").Muted()),

                new Label().Text("Ui.Pill() + Ui.BrandGradient()").Mono().Margin(0, Ui.GapSm, 0, 0),
                new HorizontalStackLayout()
                    .Spacing(Ui.GapSm)
                    .Children(
                        new Border().Pill(AppColors.Success).Content(new Label().Text("pill").FontSize(12).TextColor(AppColors.Success)),
                        new Border()
                            .StrokeThickness(0)
                            .StrokeShape(new RoundRectangle().CornerRadius(999))
                            .Padding(14, 6)
                            .Background(Ui.BrandGradient())
                            .Content(new Label().Text("gradient").FontSize(12).TextColor(Colors.White))
                    )
            ),
            Demo.Note("Theme/Ui.cs holds the whole vocabulary — roughly 120 lines for the entire look of this app."));

    private static IView FullProperty()
        => Demo.Section(
            "Level 2 — a real property method",
            "Implement the four overloads and your property behaves like a native one: direct values, the builder lambda, and style setters.",
            Demo.Code("""
                // 1. Direct value
                public static T FontSize<T>(this T self, double fontSize) where T : Label
                {
                    self.SetValue(Label.FontSizeProperty, fontSize);
                    return self;
                }

                // 2. Property builder — enables e.Path(...), e.OnLight(...), e.DynamicResource(...)
                public static T FontSize<T>(this T self,
                    Func<PropertyContext<double>, IPropertyBuilder<double>> configure) where T : Label
                {
                    var context = new PropertyContext<double>(self, Label.FontSizeProperty);
                    configure(context).Build();
                    return self;
                }

                // 3 & 4. The same two shapes again, on SettersContext<T>, for use inside Style<T>.
                """),
            Demo.Note("For a whole third-party control you do not write these by hand — the source generator emits all four shapes for every bindable property. See the library's third-party controls guide.", "⚙️"));
}
