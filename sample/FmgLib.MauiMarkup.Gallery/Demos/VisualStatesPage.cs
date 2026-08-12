using FmgLib.MauiMarkup.Gallery.Controls;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// The Visual State Manager, strongly typed — including animations that run on state entry.
/// </summary>
public partial class VisualStatesPage : DemoPage
{
    public VisualStatesPage() => this.InitializeHotReload();

    protected override string DemoTitle => "Visual States";

    protected override string DemoSummary =>
        "VisualState<T> takes the state name and the same setters lambda used everywhere else. States can also run animations when entered, and be driven by conditions instead of interaction.";

    protected override IView[] BuildSections() =>
    [
        OnAControl(),
        WithAnimations(),
        CustomGroups(),
        StateNames()
    ];

    private static IView OnAControl()
        => Demo.Section(
            "States on a control",
            "VisualStateGroups takes a VisualStateGroupList; states written straight into it land in the CommonStates group. Focus the entries to see it.",
            Demo.Stage(
                new Entry()
                    .Placeholder("Focus changes my background")
                    .VisualStateGroups(
                        new VisualStateGroupList
                        {
                            new VisualState<Entry>(VisualStates.VisualElement.Normal, e => e
                                .BackgroundColor(Colors.Transparent)),

                            new VisualState<Entry>(VisualStates.VisualElement.Focused, e => e
                                .BackgroundColor(AppColors.Accent.WithAlpha(0.14f))),
                        }),

                new Entry()
                    .Placeholder("…and this one turns magenta")
                    .VisualStateGroups(
                        new VisualStateGroupList
                        {
                            new VisualState<Entry>(VisualStates.VisualElement.Normal, e => e
                                .BackgroundColor(Colors.Transparent)),

                            new VisualState<Entry>(VisualStates.VisualElement.Focused, e => e
                                .BackgroundColor(AppColors.Magenta.WithAlpha(0.14f))),
                        })
            ),
            Demo.Code("""
                new Entry().VisualStateGroups(
                    new VisualStateGroupList
                    {
                        new VisualState<Entry>(VisualStates.VisualElement.Normal, e => e
                            .BackgroundColor(Colors.Transparent)),
                        new VisualState<Entry>(VisualStates.VisualElement.Focused, e => e
                            .BackgroundColor(AppColors.Accent.WithAlpha(0.14f))),
                    })
                """),
            Demo.Note("Always define Normal. The VSM only restores properties some state sets, so without it there is nothing to return to."));

    private static IView WithAnimations()
        => Demo.Section(
            "Animations on state entry",
            "A VisualState<T> accepts Action<T> entries in its initializer, and they run when the state is entered — which turns an async MAUI animation into a state transition.",
            Demo.Stage(
                new Border()
                    .Stage(14)
                    .HeightRequest(110)
                    .Content(new Label().Text("Hover / press me").TextCenter().FontAttributes(Bold))
                    .VisualStateGroups(
                        new VisualStateGroupList
                        {
                            new VisualState<Border>(VisualStates.VisualElement.Normal, e => e
                                .BackgroundColor(e => e.OnLight(AppColors.SurfaceAltLight).OnDark(AppColors.SurfaceAltDark)))
                            {
                                async border => await border.ScaleToAsync(1, 120, Easing.CubicOut)
                            },

                            new VisualState<Border>(VisualStates.VisualElement.PointerOver, e => e
                                .BackgroundColor(AppColors.Accent.WithAlpha(0.16f)))
                            {
                                async border => await border.ScaleToAsync(1.03, 120, Easing.CubicOut)
                            },
                        })
            ),
            Demo.Code("""
                new VisualState<Border>(VisualStates.VisualElement.PointerOver, e => e
                    .BackgroundColor(AppColors.Accent.WithAlpha(0.16f)))
                {
                    async border => await border.ScaleToAsync(1.03, 120, Easing.CubicOut)
                }
                """));

    private static IView CustomGroups()
    {
        var card = new Border()
            .Stage(14)
            .HeightRequest(96)
            .Content(new Label().Assign(out var caption).Text("Unselected").TextCenter().FontAttributes(Bold))
            .VisualStateGroups(
                new VisualStateGroupList
                {
                    new VisualStateGroup()
                        .Name("SelectionStates")
                        .States(
                            new VisualState<Border>("Unselected", e => e
                                .BackgroundColor(e => e.OnLight(AppColors.SurfaceAltLight).OnDark(AppColors.SurfaceAltDark))),

                            new VisualState<Border>("Selected", e => e
                                .BackgroundColor(AppColors.Success.WithAlpha(0.18f))))
                });

        var selected = false;

        return Demo.Section(
            "Custom groups and manual transitions",
            "States do not have to come from interaction. Name your own group, then move between its states from code with VisualStateManager.GoToState.",
            Demo.Stage(
                card,
                new Button()
                    .Text("Toggle selection")
                    .CenterHorizontal()
                    .OnClicked(_ =>
                    {
                        selected = !selected;
                        caption.Text = selected ? "Selected" : "Unselected";
                        VisualStateManager.GoToState(card, selected ? "Selected" : "Unselected");
                    })
            ),
            Demo.Code("""
                new Border().VisualStateGroups(
                    new VisualStateGroupList
                    {
                        new VisualStateGroup()
                            .Name("SelectionStates")
                            .States(
                                new VisualState<Border>("Unselected", e => e.BackgroundColor(Colors.White)),
                                new VisualState<Border>("Selected", e => e.BackgroundColor(Colors.LightBlue)))
                    });

                VisualStateManager.GoToState(card, "Selected");
                """),
            Demo.Note("VisualStateGroup holds its states in a States property rather than implementing IEnumerable — use the fluent .States(...) as above."));
    }

    private static IView StateNames()
        => Demo.Section(
            "The VisualStates constants",
            "Built-in state names ship as constants, so a typo is a compile error instead of a state that silently never activates.",
            Demo.WrapStage(
                Demo.Chip("VisualStates.VisualElement.Normal", AppColors.Accent),
                Demo.Chip("…Focused", AppColors.Accent),
                Demo.Chip("…PointerOver", AppColors.Accent),
                Demo.Chip("…Disabled", AppColors.Accent),
                Demo.Chip("VisualStates.Button.Pressed", AppColors.Violet),
                Demo.Chip("VisualStates.Switch.On / Off", AppColors.Magenta),
                Demo.Chip("VisualStates.CheckBox.IsChecked", AppColors.Info),
                Demo.Chip("VisualStates.CollectionView.Selected", AppColors.Success)
            ),
            Demo.Note("Each control's class inherits the common VisualElement states, so VisualStates.Button.Focused is valid too."));
}
