using FmgLib.MauiMarkup.Gallery.Controls;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Capturing control references without leaving the fluent chain — the <c>x:Name</c> replacement.
/// </summary>
public partial class AssignPage : DemoPage
{
    public AssignPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Assign & References";

    protected override string DemoSummary =>
        "Assign captures a control into a variable and returns it, so one control can bind to or drive another — with compile-time safety instead of string names.";

    protected override IView[] BuildSections() =>
    [
        BindToAnotherControl(),
        DriveFromHandler(),
        ForwardReference()
    ];

    private static IView BindToAnotherControl()
        => Demo.Section(
            "Bind one control to another",
            "The most common use: capture a control, then use it as the Source of a later binding. No view model needed.",
            Demo.Stage(
                new Slider()
                    .Assign(out var slider)
                    .Minimum(12)
                    .Maximum(48)
                    .Value(24),

                new Label()
                    .Text(e => e.Path(nameof(Slider.Value)).Source(slider).StringFormat("Font size: {0:F0}"))
                    .Muted()
                    .TextCenterHorizontal(),

                new Label()
                    .Text("Live preview")
                    .FontAttributes(Bold)
                    .TextColor(AppColors.Accent)
                    .TextCenterHorizontal()
                    .FontSize(e => e.Path(nameof(Slider.Value)).Source(slider))
            ),
            Demo.Code("""
                new Slider().Assign(out var slider).Minimum(12).Maximum(48),

                new Label()
                    .Text("Live preview")
                    .FontSize(e => e.Path(nameof(Slider.Value)).Source(slider))
                """));

    private static IView DriveFromHandler()
        => Demo.Section(
            "Mutate a sibling from a handler",
            "A captured reference is an ordinary local — event handlers close over it.",
            Demo.Stage(
                new Label()
                    .Assign(out var status)
                    .Text("Double-tap the card below.")
                    .Muted()
                    .TextCenterHorizontal(),

                new Border()
                    .Stage(12)
                    .HeightRequest(96)
                    .Content(new Label().Text("👋").FontSize(38).TextCenter())
                    .GestureRecognizers(
                        new TapGestureRecognizer()
                            .NumberOfTapsRequired(2)
                            .OnTapped((_, _) => status.Text = "Double-tapped — the label above was updated.")
                    )
            ),
            Demo.Code("""
                new Label().Assign(out var status),

                new Border().GestureRecognizers(
                    new TapGestureRecognizer()
                        .NumberOfTapsRequired(2)
                        .OnTapped((s, e) => status.Text = "Double-tapped")
                )
                """));

    private static IView ForwardReference()
    {
        // The Entry needs the Button, but the Button appears later in the tree. Declaring it up front
        // is the documented way around C#'s "use before assignment" rule.
        Button submit = null!;

        var name = new Entry()
            .Placeholder("Type a name to enable the button")
            .OnTextChanged((_, e) => submit.IsEnabled = !string.IsNullOrWhiteSpace(e.NewTextValue));

        submit = new Button()
            .Text("Submit")
            .IsEnabled(false)
            .CenterHorizontal();

        return Demo.Section(
            "Forward references",
            "When the control you need appears later in the tree, declare the variable first and Assign into it.",
            Demo.Stage(name, submit),
            Demo.Code("""
                Button submit = null!;

                var name = new Entry()
                    .OnTextChanged((s, e) => submit.IsEnabled = !string.IsNullOrWhiteSpace(e.NewTextValue));

                submit = new Button().Text("Submit").IsEnabled(false);
                """),
            Demo.Note("Inside a Build() that hot-reloads, prefer locals over fields — Build() re-runs and would reassign fields on every reload."));
    }
}
