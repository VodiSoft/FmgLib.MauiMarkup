using FmgLib.MauiMarkup.Gallery.Controls;
using FmgLib.MauiMarkup.Gallery.Models;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Declarative reactions: property, data, multi and event triggers.
/// </summary>
public partial class TriggersPage : DemoPage
{
    private readonly DemoViewModel viewModel = new();

    public TriggersPage()
    {
        BindingContext = viewModel;

        this.InitializeHotReload();
    }

    protected override string DemoTitle => "Triggers";

    protected override string DemoSummary =>
        "A trigger changes properties when a condition holds — no event handler, and it reverts by itself when the condition stops holding.";

    protected override IView[] BuildSections() =>
    [
        PropertyTrigger(),
        DataTriggerSection(),
        MultiTriggerSection(),
        EventTriggerSection()
    ];

    private static IView PropertyTrigger()
        => Demo.Section(
            "Property triggers",
            "React to the control's own property. Focus either field: the trigger sets the background, and undoes it on blur without any code.",
            Demo.Stage(
                new Entry()
                    .Placeholder("Focus me")
                    .Triggers(
                        new Trigger(typeof(Entry))
                            .Property(Entry.IsFocusedProperty)
                            .Value(true)
                            .Setters(new Setters<Entry>(e => e
                                .BackgroundColor(AppColors.Accent.WithAlpha(0.14f))))
                    ),

                new Entry()
                    .Placeholder("…or me")
                    .Triggers(
                        new Trigger(typeof(Entry))
                            .Property(Entry.IsFocusedProperty)
                            .Value(true)
                            .Setters(new Setters<Entry>(e => e
                                .BackgroundColor(AppColors.Magenta.WithAlpha(0.14f))))
                    )
            ),
            Demo.Code("""
                new Entry().Triggers(
                    new Trigger(typeof(Entry))
                        .Property(Entry.IsFocusedProperty)
                        .Value(true)
                        .Setters(new Setters<Entry>(e => e.BackgroundColor(Colors.Yellow)))
                )
                """),
            Demo.Note("Put the same trigger inside a Style<Entry> and every entry in the app gets it — the DRY version of this demo."));

    private static IView DataTriggerSection()
        => Demo.Section(
            "Data triggers",
            "The condition is a binding, so it can watch another control or the view model. Empty the field and the button disables itself.",
            Demo.Stage(
                new Entry()
                    .Assign(out var required)
                    .Placeholder("Required field")
                    .Text("filled in"),

                new Button()
                    .Text("Continue")
                    .CenterHorizontal()
                    .Triggers(
                        new DataTrigger(typeof(Button))
                            .Binding(e => e.Path("Text.Length").Source(required))
                            .Value(0)
                            .Setters(new Setters<Button>(e => e.IsEnabled(false)))
                    )
            ),
            Demo.Code("""
                new Button()
                    .Text("Continue")
                    .Triggers(
                        new DataTrigger(typeof(Button))
                            .Binding(e => e.Path("Text.Length").Source(required))
                            .Value(0)
                            .Setters(new Setters<Button>(e => e.IsEnabled(false)))
                    )
                """));

    private static IView MultiTriggerSection()
        => Demo.Section(
            "Multi triggers",
            "Every condition must hold. Conditions come in two flavours: PropertyCondition for the control's own properties, BindingCondition for anything else.",
            Demo.Stage(
                new Entry().Assign(out var email).Placeholder("E-mail"),
                new Entry().Assign(out var phone).Placeholder("Phone"),

                new Label()
                    .Text("The hint below shows only while BOTH fields are empty — fill either one and it disappears.")
                    .Muted()
                    .FontSize(12),

                new Border()
                    .Pill(AppColors.Warning)
                    .CenterHorizontal()
                    .Opacity(0)
                    .Content(new Label().Text("Start by entering a contact ✎").FontSize(13).TextColor(AppColors.Warning))
                    .Triggers(
                        new MultiTrigger(typeof(Border))
                            .Conditions(
                                new BindingCondition()
                                    .Binding(e => e.Path("Text.Length").Source(email))
                                    .Value(0),
                                new BindingCondition()
                                    .Binding(e => e.Path("Text.Length").Source(phone))
                                    .Value(0)
                            )
                            .Setters(new Setters<Border>(e => e.Opacity(1.0)))
                    )
            ),
            Demo.Code("""
                new MultiTrigger(typeof(Border))
                    .Conditions(
                        new BindingCondition().Binding(e => e.Path("Text.Length").Source(email)).Value(0),
                        new BindingCondition().Binding(e => e.Path("Text.Length").Source(phone)).Value(0)
                    )
                    .Setters(new Setters<Border>(e => e.Opacity(1.0)))
                """),
            Demo.Note("A condition's Binding lambda builds a raw Binding — Path, Source and Converter are available there, but the inline Convert of the property builder is not."));

    private static IView EventTriggerSection()
        => Demo.Section(
            "Event triggers",
            "An event trigger runs TriggerActions instead of setting properties — and unlike the others it reverts nothing, because the action decides what happens each time.",
            Demo.Stage(
                new Entry()
                    .Placeholder("Type lowercase — it will be upper-cased")
                    .Triggers(
                        new EventTrigger()
                            .Event("TextChanged")
                            .Actions(new UppercaseTriggerAction())
                    )
            ),
            Demo.Code("""
                public sealed class UppercaseTriggerAction : TriggerAction<Entry>
                {
                    protected override void Invoke(Entry entry) => entry.Text = entry.Text?.ToUpperInvariant();
                }

                new Entry().Triggers(
                    new EventTrigger().Event("TextChanged").Actions(new UppercaseTriggerAction())
                )
                """));
}
