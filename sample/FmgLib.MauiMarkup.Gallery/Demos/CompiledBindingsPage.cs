using FmgLib.MauiMarkup.Gallery.Controls;
using FmgLib.MauiMarkup.Gallery.Models;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Expression bindings: the same builder, with the string path replaced by a lambda.
/// </summary>
public partial class CompiledBindingsPage : DemoPage
{
    private readonly DemoViewModel viewModel = new();

    public CompiledBindingsPage()
    {
        BindingContext = viewModel;

        this.InitializeHotReload();
    }

    protected override string DemoTitle => "Compiled Bindings";

    protected override string DemoSummary =>
        "Getter replaces Path with a typed expression: no reflection at runtime, a compiler error instead of a silent failure, and rename refactoring that actually follows.";

    protected override IView[] BuildSections() =>
    [
        GetterBasics(),
        TwoWayWithSetter(),
        CompiledMultiBinding(),
        WhatIsAllowed()
    ];

    private static IView GetterBasics()
        => Demo.Section(
            "Getter instead of Path",
            "Everything else about the builder is unchanged — Source, StringFormat, Convert and the rest all still apply.",
            Demo.Stage(
                new Label()
                    .Text(e => e.Getter(static (DemoViewModel vm) => vm.FullName))
                    .FontSize(20)
                    .FontAttributes(Bold)
                    .TextColor(AppColors.Accent),

                new Label()
                    .Text(e => e
                        .Getter(static (DemoViewModel vm) => vm.Age)
                        .StringFormat("Age: {0}"))
                    .Muted(),

                new Label()
                    .Text(e => e
                        .Getter(static (DemoViewModel vm) => vm.FirstName)
                        .Convert((string name) => $"Keep the getter simple, transform with Convert: {name?.ToUpperInvariant()}"))
                    .Muted()
                    .FontSize(12)
            ),
            Demo.Code("""
                new Label().Text(e => e.Getter(static (DemoViewModel vm) => vm.FullName))

                new Label().Text(e => e
                    .Getter(static (DemoViewModel vm) => vm.Age)
                    .StringFormat("Age: {0}"))
                """),
            Demo.Note("Mark the lambda static — it prevents accidental closure captures and states the intent: a pure property access."));

    private static IView TwoWayWithSetter()
        => Demo.Section(
            "Setter completes the round trip",
            "A compiled binding is one-way until you supply the reverse operation. Then it is two-way, still without a single string.",
            Demo.Stage(
                new Entry()
                    .Placeholder("First name")
                    .Text(e => e
                        .Getter(static (DemoViewModel vm) => vm.FirstName)
                        .Setter(static (DemoViewModel vm, string value) => vm.FirstName = value)
                        .BindingMode(BindingMode.TwoWay)),

                new Label()
                    .Text(e => e.Getter(static (DemoViewModel vm) => vm.Initials))
                    .FontSize(26)
                    .FontAttributes(Bold)
                    .TextColor(AppColors.Magenta)
                    .TextCenterHorizontal()
            ),
            Demo.Code("""
                new Entry().Text(e => e
                    .Getter(static (DemoViewModel vm) => vm.FirstName)
                    .Setter(static (DemoViewModel vm, string value) => vm.FirstName = value)
                    .BindingMode(BindingMode.TwoWay))
                """));

    private static IView CompiledMultiBinding()
        => Demo.Section(
            "Compiled multi-bindings",
            "Getter can be called more than once. Each call opens its own compiled sub-binding, they may produce different types, and the whole multi-binding stays reflection-free.",
            Demo.Stage(
                new Label()
                    .Text(e => e
                        .Getter(static (DemoViewModel vm) => vm.FullName)
                        .Getter(static (DemoViewModel vm) => vm.Age)
                        .MultiConvert((string name, int age) => $"{name} ({age})"))
                    .FontAttributes(Bold),

                new Label()
                    .Text(e => e
                        .Getter(static (DemoViewModel vm) => vm.Budget)
                        .Path(nameof(DemoViewModel.Affordable))
                        .MultiConvert((double budget, int affordable) => $"Compiled and string sub-bindings mix freely: {affordable} items under {budget:C0}"))
                    .Muted()
                    .FontSize(12)
            ),
            Demo.Code("""
                new Label().Text(e => e
                    .Getter(static (DemoViewModel vm) => vm.FullName)
                    .Getter(static (DemoViewModel vm) => vm.Age)
                    .MultiConvert((string name, int age) => $"{name} ({age})"))
                """));

    private static IView WhatIsAllowed()
        => Demo.Section(
            "What the expression may contain",
            "The getter has to be a simple property access — that restriction is what makes it compilable. Anything else belongs in Convert or in a computed view-model property.",
            new Grid()
            .ColumnDefinitions(e => e.Star().Star())
            .ColumnSpacing(Ui.Gap)
            .Children(
                new Border()
                    .Stage(12)
                    .Content(
                        new VerticalStackLayout()
                        .Spacing(Ui.GapXs)
                        .Children(
                            new Label().Text("✅ Supported").FontAttributes(Bold).TextColor(AppColors.Success),
                            new Label().Text("vm.Name").Mono().FontSize(12),
                            new Label().Text("vm.Address?.Street").Mono().FontSize(12),
                            new Label().Text("vm.PhoneNumbers[0]").Mono().FontSize(12),
                            new Label().Text("((PersonVM)label.BindingContext).Name").Mono().FontSize(12)
                        )
                    ),

                new Border()
                    .Column(1)
                    .Stage(12)
                    .Content(
                        new VerticalStackLayout()
                        .Spacing(Ui.GapXs)
                        .Children(
                            new Label().Text("🚫 Needs Convert").FontAttributes(Bold).TextColor(AppColors.Danger),
                            new Label().Text("vm.GetAddress()").Mono().FontSize(12),
                            new Label().Text("vm.Address?.ToString()").Mono().FontSize(12),
                            new Label().Text("vm.Street + \" \" + vm.City").Mono().FontSize(12),
                            new Label().Text("$\"Name: {vm.Name}\"").Mono().FontSize(12)
                        )
                    )
            ));
}
