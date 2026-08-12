using FmgLib.MauiMarkup.Gallery.Controls;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Every event of every control becomes an <c>On&lt;EventName&gt;</c> method, in two shapes.
/// </summary>
public partial class EventsPage : DemoPage
{
    private int clicks;

    public EventsPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Events";

    protected override string DemoSummary =>
        "On<Event> methods attach handlers inline. Take the typed sender when that is all you need, or the full event args when you need them.";

    protected override IView[] BuildSections() =>
    [
        TypedSender(),
        FullArgs(),
        MethodGroup(),
        Lifecycle()
    ];

    private IView TypedSender()
    {
        clicks = 0;

        return Demo.Section(
            "Shape 1 — the typed sender",
            "The everyday shape: no unused (object sender, EventArgs e) boilerplate, and the parameter is already the concrete control type.",
            Demo.Stage(
                new Button()
                    .Text("Click me")
                    .CenterHorizontal()
                    .OnClicked(button =>
                    {
                        clicks++;
                        button.Text = clicks == 1 ? "Clicked 1 time" : $"Clicked {clicks} times";
                    })
            ),
            Demo.Code("""
                new Button()
                    .Text("Click me")
                    .OnClicked(button =>          // button is a Button, not an object
                    {
                        clicks++;
                        button.Text = $"Clicked {clicks} times";
                    })
                """));
    }

    private static IView FullArgs()
    {
        // Declared before the Entry that writes to them — declaration order is independent of
        // display order, which is the simplest way to satisfy C#'s definite-assignment rules.
        var echo = new Label().Text("TextChanged has not fired yet.").Muted();
        var count = new Label().Text("0 characters").Mono().TextColor(AppColors.Accent);

        var entry = new Entry()
            .Placeholder("Type something…")
            .OnTextChanged((_, e) =>
            {
                echo.Text = $"'{e.OldTextValue}' → '{e.NewTextValue}'";
                count.Text = $"{e.NewTextValue?.Length ?? 0} characters";
            });

        return Demo.Section(
            "Shape 2 — the full event args",
            "When the arguments carry the information you need, take the classic signature.",
            Demo.Stage(entry, echo, count),
            Demo.Code("""
                var echo = new Label().Text("TextChanged has not fired yet.");

                new Entry()
                    .OnTextChanged((sender, e) =>
                        echo.Text = $"'{e.OldTextValue}' → '{e.NewTextValue}'")
                """));
    }

    private static IView MethodGroup()
        => Demo.Section(
            "Method groups",
            "Handlers do not have to be lambdas — a method group keeps long handlers out of the layout.",
            Demo.Stage(
                new Slider()
                    .Assign(out var slider)
                    .Minimum(0)
                    .Maximum(100)
                    .Value(35),

                new Label()
                    .Text(e => e.Path(nameof(Slider.Value)).Source(slider).StringFormat("Value: {0:F0}"))
                    .Mono()
                    .TextCenterHorizontal()
            ),
            Demo.Code("""
                private void OnValueChanged(Slider slider) => …;

                new Slider().OnValueChanged(OnValueChanged)
                """),
            Demo.Note("The label above is not an event handler at all — it binds straight to the slider captured with Assign."));

    private static IView Lifecycle()
        => Demo.Section(
            "Lifecycle events",
            "Loaded, Appearing and friends are ordinary events, so they get the same treatment — the natural place for entrance animations.",
            Demo.Stage(
                new Border()
                    .Stage(10)
                    .Opacity(0)
                    .TranslationY(16)
                    .Content(new Label().Text("I faded in when I loaded.").TextCenterHorizontal())
                    .OnLoaded(async border =>
                    {
                        await Task.WhenAll(
                            border.AnimateOpacityTo(1, 420),
                            border.TranslateToAsync(0, 0, 420, Easing.CubicOut));
                    })
            ),
            Demo.Code("""
                new Border()
                    .Opacity(0)
                    .TranslationY(16)
                    .OnLoaded(async border => await Task.WhenAll(
                        border.AnimateOpacityTo(1, 420),
                        border.TranslateToAsync(0, 0, 420, Easing.CubicOut)))
                """));
}
