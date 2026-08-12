using FmgLib.MauiMarkup.Gallery.Demos;

namespace FmgLib.MauiMarkup.Gallery.Models;

/// <summary>One entry in the gallery.</summary>
/// <param name="Route">Shell route, also used as the registration key.</param>
/// <param name="Title">Card title.</param>
/// <param name="Summary">One-line description shown on the card.</param>
/// <param name="Glyph">Emoji shown on the card.</param>
/// <param name="Category">Grouping used by the home filter.</param>
/// <param name="Tint">Accent colour of the card.</param>
/// <param name="PageType">Page opened when the card is tapped.</param>
public sealed record DemoInfo(
    string Route,
    string Title,
    string Summary,
    string Glyph,
    string Category,
    Color Tint,
    Type PageType);

/// <summary>
/// The single source of truth for the gallery: the home screen renders it, and the shell registers
/// its routes from it, so adding a demo is one entry in one list.
/// </summary>
public static class DemoCatalog
{
    public const string Fundamentals = "Fundamentals";
    public const string Data = "Data";
    public const string Layout = "Layout";
    public const string Interaction = "Interaction";
    public const string Appearance = "Appearance";
    public const string Platform = "Platform";

    public static IReadOnlyList<string> Categories { get; } =
        [Fundamentals, Layout, Data, Interaction, Appearance, Platform];

    public static IReadOnlyList<DemoInfo> All { get; } =
    [
        new("fluent", "Fluent Properties", "The four overload shapes every property gets, plus the shorthands.",
            "✍️", Fundamentals, AppColors.Accent, typeof(FluentPropertiesPage)),

        new("events", "Events", "On<Event> handlers in both shapes — typed sender or full event args.",
            "⚡", Fundamentals, AppColors.Violet, typeof(EventsPage)),

        new("assign", "Assign & References", "x:Name without XAML: capture controls mid-chain and wire them together.",
            "🔗", Fundamentals, AppColors.Cyan, typeof(AssignPage)),

        new("custom", "Custom Extensions", "Grow your own vocabulary — this gallery's design system is one.",
            "🧩", Fundamentals, AppColors.Magenta, typeof(CustomExtensionsPage)),

        new("layout", "Layout & Alignment", "Positioning views and their text, without touching LayoutOptions.",
            "📐", Layout, AppColors.Success, typeof(LayoutPage)),

        new("grid", "Grid", "Row and column builders, spans, and the grid as an overlay container.",
            "🧮", Layout, AppColors.Info, typeof(GridPage)),

        new("responsive", "Responsive & Adaptive", "One page that reshapes itself for phone, tablet and desktop.",
            "📱", Layout, AppColors.Warning, typeof(ResponsivePage)),

        new("binding", "Data Binding", "Paths, sources, modes, formats and inline converters.",
            "🔌", Data, AppColors.Accent, typeof(BindingPage)),

        new("multibinding", "MultiBinding", "Several sources combined into one property.",
            "🧵", Data, AppColors.Violet, typeof(MultiBindingPage)),

        new("compiled", "Compiled Bindings", "Expression bindings — no reflection, no magic strings.",
            "⚙️", Data, AppColors.Cyan, typeof(CompiledBindingsPage)),

        new("collections", "Collections & Templates", "CollectionView, templates, empty views and bindable layouts.",
            "📋", Data, AppColors.Success, typeof(CollectionsPage)),

        new("gestures", "Gestures", "Tap, pan, pinch, swipe and pointer, all fluent.",
            "👆", Interaction, AppColors.Magenta, typeof(GesturesPage)),

        new("behaviors", "Behaviors", "Reusable control logic without subclassing.",
            "🧠", Interaction, AppColors.Info, typeof(BehaviorsPage)),

        new("triggers", "Triggers", "Property, data and multi triggers — reactions without handlers.",
            "🎛️", Interaction, AppColors.Warning, typeof(TriggersPage)),

        new("swipe", "SwipeView", "Swipe-to-action rows with fluent swipe items.",
            "↔️", Interaction, AppColors.Accent, typeof(SwipePage)),

        new("styling", "Styling", "Style<T>, implicit and explicit styles, inheritance.",
            "🎨", Appearance, AppColors.Violet, typeof(StylingPage)),

        new("visualstates", "Visual States", "State-driven appearance, with animations on entry.",
            "🔀", Appearance, AppColors.Cyan, typeof(VisualStatesPage)),

        new("animations", "Animations", "Generated Animate…To helpers, sequential and parallel.",
            "🎬", Appearance, AppColors.Magenta, typeof(AnimationsPage)),

        new("shapes", "Shapes & Paths", "Vector shapes, geometries, clipping and transforms.",
            "🔺", Appearance, AppColors.Success, typeof(ShapesPage)),

        new("brushes", "Gradients & Brushes", "Linear and radial gradients as first-class fluent objects.",
            "🌈", Appearance, AppColors.Info, typeof(BrushesPage)),

        new("text", "Formatted Text", "Mixed styles, and tappable spans, inside one label.",
            "🔤", Appearance, AppColors.Warning, typeof(FormattedTextPage)),

        new("theming", "Theming", "AppThemeBinding and dynamic resources — repaint without rebuilding.",
            "🌗", Platform, AppColors.Accent, typeof(ThemingPage)),

        new("localization", "Localization", "Live language switching, formatted translations and RTL.",
            "🌍", Platform, AppColors.Violet, typeof(LocalizationPage)),

        new("hotreload", "Hot Reload", "How Build() turns .NET hot reload into live UI editing.",
            "🔥", Platform, AppColors.Danger, typeof(HotReloadPage))
    ];

    /// <summary>Registers every demo route with the shell.</summary>
    public static void RegisterRoutes()
    {
        foreach (var demo in All)
            Routing.RegisterRoute(demo.Route, demo.PageType);
    }
}
