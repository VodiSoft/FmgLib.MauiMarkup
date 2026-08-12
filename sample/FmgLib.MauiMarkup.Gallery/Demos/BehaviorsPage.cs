using FmgLib.MauiMarkup.Gallery.Controls;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Behaviors: reusable control logic, attached fluently, with no subclassing.
/// </summary>
public partial class BehaviorsPage : DemoPage
{
    public BehaviorsPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Behaviors";

    protected override string DemoSummary =>
        "A behavior packages control logic once and attaches it anywhere. Because behaviors are BindableObjects, they can be configured — and even generated a fluent API of their own.";

    protected override IView[] BuildSections() =>
    [
        Validation(),
        Configurable(),
        WhenToUse()
    ];

    private static IView Validation()
        => Demo.Section(
            "A validation behavior",
            "The entry knows nothing about validation; the behavior subscribes on attach and unsubscribes on detach. Type letters and watch the colour change.",
            Demo.Stage(
                new Entry()
                    .Placeholder("Numbers stay green, anything else turns red")
                    .Keyboard(Keyboard.Text)
                    .Behaviors(new NumericValidationBehavior())
            ),
            Demo.Code("""
                public sealed class NumericValidationBehavior : Behavior<Entry>
                {
                    protected override void OnAttachedTo(Entry entry)
                    {
                        entry.TextChanged += OnTextChanged;
                        base.OnAttachedTo(entry);
                    }

                    protected override void OnDetachingFrom(Entry entry)
                    {
                        entry.TextChanged -= OnTextChanged;
                        base.OnDetachingFrom(entry);
                    }
                }

                new Entry().Behaviors(new NumericValidationBehavior())
                """),
            Demo.Note("Always undo in OnDetachingFrom what you did in OnAttachedTo — a behavior on a recycled list row attaches and detaches many times."));

    private static IView Configurable()
    {
        var strength = new Label().Text("Minimum 8 characters.").Muted();
        var meter = new ProgressBar().Progress(0);

        var behavior = new MinLengthBehavior { MinLength = 8 };

        behavior.ValidityChanged += (_, isValid) =>
        {
            strength.Text = isValid ? "Long enough ✓" : "Minimum 8 characters.";
            strength.TextColor = isValid ? AppColors.Success : AppColors.MutedLight;
            meter.ProgressColor = isValid ? AppColors.Success : AppColors.Warning;
        };

        var entry = new Entry()
            .Placeholder("Password")
            .IsPassword(true)
            .Behaviors(behavior)
            .OnTextChanged((_, e) => meter.Progress = Math.Clamp((e.NewTextValue?.Length ?? 0) / 8.0, 0, 1));

        return Demo.Section(
            "Behaviors with configuration",
            "A bindable property makes the behavior reusable across screens with different rules — and bindable to a view model like any other property.",
            Demo.Stage(entry, meter, strength),
            Demo.Code("""
                public sealed class MinLengthBehavior : Behavior<Entry>
                {
                    public static readonly BindableProperty MinLengthProperty =
                        BindableProperty.Create(nameof(MinLength), typeof(int), typeof(MinLengthBehavior), 6);

                    public int MinLength
                    {
                        get => (int)GetValue(MinLengthProperty);
                        set => SetValue(MinLengthProperty, value);
                    }
                }

                new Entry().Behaviors(new MinLengthBehavior { MinLength = 8 })
                """),
            Demo.Note("Opt the behavior into the source generator with [MauiMarkup(typeof(MinLengthBehavior))] and it gets fluent methods too: .Behaviors(new MinLengthBehavior().MinLength(8)).", "⚙️"));
    }

    private static IView WhenToUse()
        => Demo.Section(
            "Behavior, trigger or handler?",
            "All three react to something. They differ in where the logic lives and how reusable it is.",
            new VerticalStackLayout()
            .Spacing(Ui.GapSm)
            .Children(
                Row("Behavior", "Reusable logic that needs state or subscriptions", AppColors.Accent),
                Row("Trigger", "Declarative property change on a condition", AppColors.Violet),
                Row("On<Event>", "A one-off response on this control only", AppColors.Magenta),
                Row("Visual state", "Appearance for a named interaction state", AppColors.Info)
            ));

    private static View Row(string name, string use, Color tint)
        => new Grid()
            .ColumnDefinitions(e => e.Absolute(112).Star())
            .ColumnSpacing(Ui.Gap)
            .Children(
                new Border()
                    .Pill(tint)
                    .Margin(0)
                    .Content(new Label().Text(name).FontSize(12).FontAttributes(Bold).TextColor(tint).TextCenterHorizontal()),

                new Label().Column(1).Text(use).Muted().CenterVertical()
            );
}
