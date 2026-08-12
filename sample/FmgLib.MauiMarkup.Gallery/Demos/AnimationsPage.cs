using FmgLib.MauiMarkup.Gallery.Controls;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Generated <c>Animate…To</c> helpers, plus the MAUI built-ins they sit alongside.
/// </summary>
public partial class AnimationsPage : DemoPage
{
    public AnimationsPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Animations";

    protected override string DemoSummary =>
        "Every animatable property gets an Animate…To helper returning Task<bool>, so animations compose with async/await — sequentially or in parallel.";

    protected override IView[] BuildSections() =>
    [
        Generated(),
        Sequencing(),
        Entrance(),
        Performance()
    ];

    private static IView Generated()
    {
        var target = new Border()
            .SizeRequest(120, 90)
            .StrokeThickness(0)
            .BackgroundColor(AppColors.Accent)
            .Center()
            .Content(new Label().Text("target").FontSize(12).TextColor(Colors.White).TextCenter());

        var label = new Label()
            .Text("Animate me")
            .FontSize(16)
            .TextCenterHorizontal();

        return Demo.Section(
            "Animate<Property>To",
            "The generator emits one of these for every animatable bindable property — colours, doubles, sizes — interpolating from the current value to the target.",
            Demo.Stage(
                target,
                label,
                new HorizontalStackLayout()
                .Spacing(Ui.GapSm)
                .CenterHorizontal()
                .Children(
                    new Button()
                        .Text("Colour")
                        .OnClicked(async _ =>
                        {
                            await target.AnimateBackgroundColorTo(AppColors.Magenta, 400);
                            await target.AnimateBackgroundColorTo(AppColors.Accent, 400);
                        }),

                    new Button()
                        .Text("Size")
                        .OnClicked(async _ =>
                        {
                            await target.AnimateSizeRequestTo(200, 130, 350);
                            await target.AnimateSizeRequestTo(120, 90, 350);
                        }),

                    new Button()
                        .Text("Font")
                        .OnClicked(async _ =>
                        {
                            await label.AnimateFontSizeTo(34, 300);
                            await label.AnimateFontSizeTo(16, 300);
                        })
                )
            ),
            Demo.Code("""
                await box.AnimateBackgroundColorTo(Colors.Teal, 500);
                await view.AnimateSizeRequestTo(200, 120);
                await label.AnimateFontSizeTo(40);
                await progressBar.AnimateProgressTo(0.8);
                """));
    }

    private static IView Sequencing()
    {
        var dot1 = Dot(AppColors.Accent);
        var dot2 = Dot(AppColors.Violet);
        var dot3 = Dot(AppColors.Magenta);

        return Demo.Section(
            "Sequential or parallel",
            "Because every helper returns a Task, await chains them; Task.WhenAll runs them together. That is the whole composition story.",
            Demo.Stage(
                new HorizontalStackLayout()
                .Spacing(Ui.Gap)
                .CenterHorizontal()
                .Children(dot1, dot2, dot3),

                new HorizontalStackLayout()
                .Spacing(Ui.GapSm)
                .CenterHorizontal()
                .Children(
                    new Button()
                        .Text("Sequential")
                        .OnClicked(async _ =>
                        {
                            foreach (var dot in new[] { dot1, dot2, dot3 })
                            {
                                await dot.TranslateToAsync(0, -26, 160, Easing.CubicOut);
                                await dot.TranslateToAsync(0, 0, 160, Easing.CubicIn);
                            }
                        }),

                    new Button()
                        .Text("Parallel")
                        .OnClicked(async _ =>
                        {
                            await Task.WhenAll(
                                dot1.AnimateOpacityTo(0.25, 300),
                                dot2.AnimateOpacityTo(0.25, 300),
                                dot3.AnimateOpacityTo(0.25, 300));

                            await Task.WhenAll(
                                dot1.AnimateOpacityTo(1, 300),
                                dot2.AnimateOpacityTo(1, 300),
                                dot3.AnimateOpacityTo(1, 300));
                        })
                )
            ),
            Demo.Code("""
                // sequential
                await image.AnimateOpacityTo(0, 150);
                image.Source = "next.png";
                await image.AnimateOpacityTo(1, 150);

                // parallel
                await Task.WhenAll(
                    card.AnimateBackgroundColorTo(Colors.LightYellow, 300),
                    card.TranslateToAsync(0, -8, 300, Easing.CubicOut));
                """));
    }

    private static View Dot(Color tint)
        => new Border()
            .SizeRequest(44, 44)
            .StrokeThickness(0)
            .BackgroundColor(tint)
            .Content(new Label().Text("●").TextColor(Colors.White).TextCenter());

    private static IView Entrance()
        => Demo.Section(
            "Entrance animations",
            "OnLoaded is the right hook: it fires once the view is on screen, and — unlike Build() — it does not re-run on every hot reload.",
            new Border()
            .Stage()
            .Content(
                new VerticalStackLayout()
                .Spacing(Ui.GapSm)
                .Children(
                    Staggered("First", 0),
                    Staggered("Second", 120),
                    Staggered("Third", 240)
                )
            ),
            Demo.Code("""
                new VerticalStackLayout()
                    .Opacity(0)
                    .TranslationY(24)
                    .OnLoaded(async v => await Task.WhenAll(
                        v.AnimateOpacityTo(1, 350),
                        v.TranslateToAsync(0, 0, 350, Easing.CubicOut)))
                """));

    private static View Staggered(string text, int delay)
        => new Border()
            .Stage(10)
            .Opacity(0)
            .TranslationX(-24)
            .Content(new Label().Text(text).FontAttributes(Bold))
            .OnLoaded(async border =>
            {
                await Task.Delay(delay);
                await Task.WhenAll(
                    border.AnimateOpacityTo(1, 320),
                    border.TranslateToAsync(0, 0, 320, Easing.CubicOut));
            });

    private static IView Performance()
        => Demo.Section(
            "What to animate",
            "Transform properties do not trigger layout; size requests do, every frame. Prefer the first, keep the second short.",
            new VerticalStackLayout()
            .Spacing(Ui.GapSm)
            .Children(
                Demo.Note("Cheap: TranslationX/Y, Scale, Rotation, Opacity — composited, no relayout.", "✅"),
                Demo.Note("Costly: Animate…RequestTo size animations — they re-measure the tree each frame.", "⚠️"),
                Demo.Note("Do not start animations inside Build(): it re-runs on every hot reload. Use OnLoaded or OnAppearing.", "🔥"),
                Demo.Note("Cancel long loops when leaving: this.OnDisappearing(p => p.AbortAnimation(\"pulse\")).", "🧹")
            ));
}
