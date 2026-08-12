using System.Collections.ObjectModel;
using FmgLib.MauiMarkup.Gallery.Controls;
using FmgLib.MauiMarkup.Gallery.Models;
using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup.Gallery;

/// <summary>
/// The gallery index: a hero, a live filter, and a card grid that reflows from one column on a phone
/// to four on a wide desktop window.
/// </summary>
/// <remarks>
/// The grid is a <c>CollectionView</c> rather than a stack of rows so it stays virtualized, and the
/// hero rides along as its <c>Header</c> — one scroll surface, no nested scrolling.
/// </remarks>
public partial class HomePage : ContentPage, IFmgLibHotReload
{
    private readonly ObservableCollection<DemoInfo> visible = [];

    private string query = string.Empty;
    private string category = AllCategories;

    private GridItemsLayout itemsLayout = null!;
    private HorizontalStackLayout chipsRow = null!;

    private const string AllCategories = "All";

    public HomePage()
    {
        ApplyFilter();

        SizeChanged += (_, _) => UpdateSpan();

        this.InitializeHotReload();
    }

    /// <inheritdoc/>
    public void Build()
    {
        itemsLayout = new GridItemsLayout(SpanForWidth(Width), ItemsLayoutOrientation.Vertical)
            .VerticalItemSpacing(Ui.GapMd)
            .HorizontalItemSpacing(Ui.GapMd);

        this
        .PageSurface()
        .Title("Gallery")
        .Content(
            new CollectionView()
            .ItemsSource(visible)
            .ItemsLayout(itemsLayout)
            .SelectionMode(SelectionMode.None)
            .Header(HeroHeader())
            .Footer(Footer())
            .EmptyView(EmptyState())
            .ItemTemplate(() => DemoCard())
        );
    }

    // ---- header --------------------------------------------------------------------------------

    private View HeroHeader()
        => new ContentView()
            .Padding(e => e
                .OnPhone(new Thickness(Ui.GapMd, Ui.GapMd, Ui.GapMd, Ui.Gap))
                .Default(new Thickness(Ui.GapLg, Ui.GapLg, Ui.GapLg, Ui.GapMd)))
            .Content(
                new VerticalStackLayout()
                .Spacing(Ui.GapMd)
                .MaximumWidthRequest(Ui.ReadableWidth)
                .CenterHorizontal()
                .Children(
                    Hero(),
                    SearchRow(),
                    CategoryChips()
                )
            );

    private View Hero()
        => new Border()
            .StrokeThickness(0)
            .StrokeShape(new RoundRectangle().CornerRadius(24))
            .Background(Ui.BrandGradient())
            .Padding(e => e.OnPhone(new Thickness(20)).Default(new Thickness(32)))
            .SoftShadow()
            .Content(
                new Grid()
                .ColumnDefinitions(e => e.Star().Auto())
                .ColumnSpacing(Ui.GapMd)
                .Children(
                    new VerticalStackLayout()
                    .Spacing(Ui.GapSm)
                    .CenterVertical()
                    .Children(
                        new Label()
                            .Text("FmgLib.MauiMarkup")
                            .FontSize(e => e.OnPhone(26.0).Default(38.0))
                            .FontAttributes(Bold)
                            .TextColor(Colors.White),

                        new Label()
                            .Text("Every feature of the library, one page at a time — all of it written in C#.")
                            .FontSize(e => e.OnPhone(13.5).Default(16.0))
                            .TextColor(Colors.White.WithAlpha(0.86f)),

                        new HorizontalStackLayout()
                        .Spacing(Ui.GapSm)
                        .Margin(0, Ui.GapSm, 0, 0)
                        .Children(
                            HeroStat($"{DemoCatalog.All.Count}", "demos"),
                            HeroStat($"{DemoCatalog.Categories.Count}", "topics"),
                            HeroStat("0", "lines of XAML")
                        )
                    ),

                    ThemeToggle().Column(1).AlignTopRight()
                )
            );

    private static View HeroStat(string value, string caption)
        => new Border()
            .BackgroundColor(Colors.White.WithAlpha(0.16f))
            .StrokeThickness(0)
            .StrokeShape(new RoundRectangle().CornerRadius(12))
            .Padding(12, 8)
            .Content(
                new HorizontalStackLayout()
                .Spacing(6)
                .Children(
                    new Label().Text(value).FontSize(15).FontAttributes(Bold).TextColor(Colors.White),
                    new Label().Text(caption).FontSize(12).TextColor(Colors.White.WithAlpha(0.8f)).CenterVertical()
                )
            );

    /// <summary>
    /// Flips <c>UserAppTheme</c>. Nothing is rebuilt when it is tapped: every colour in the gallery
    /// comes from an <c>OnLight/OnDark</c> builder, which is a real AppThemeBinding.
    /// </summary>
    private static View ThemeToggle()
        => new Border()
            .BackgroundColor(Colors.White.WithAlpha(0.18f))
            .StrokeThickness(0)
            .StrokeShape(new RoundRectangle().CornerRadius(999))
            .Padding(12, 10)
            .Content(new Label().Text("🌗").FontSize(18))
            .GestureRecognizers(
                new TapGestureRecognizer().OnTapped((_, _) =>
                {
                    if (Application.Current is not { } app)
                        return;

                    app.UserAppTheme = app.RequestedTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
                })
            );

    private View SearchRow()
        => new Border()
            .Card(16)
            .Padding(Ui.GapSm, 0)
            .Content(
                new SearchBar()
                    .Placeholder("Search the gallery…")
                    .OnTextChanged((_, e) =>
                    {
                        query = e.NewTextValue ?? string.Empty;
                        ApplyFilter();
                    })
            );

    private View CategoryChips()
    {
        chipsRow = new HorizontalStackLayout().Spacing(Ui.GapSm);
        RenderChips();

        return new ScrollView()
            .Orientation(ScrollOrientation.Horizontal)
            .HorizontalScrollBarVisibility(ScrollBarVisibility.Never)
            .Content(chipsRow);
    }

    private void RenderChips()
    {
        chipsRow.Children.Clear();

        foreach (var name in new[] { AllCategories }.Concat(DemoCatalog.Categories))
        {
            var isSelected = category == name;
            var label = name;

            var text = new Label()
                .Text(label)
                .FontSize(13)
                .FontAttributes(isSelected ? Bold : None);

            var chip = new Border()
                .StrokeThickness(isSelected ? 0 : 1)
                .Stroke(e => e
                    .OnLight(new SolidColorBrush(AppColors.BorderLight))
                    .OnDark(new SolidColorBrush(AppColors.BorderDark)))
                .StrokeShape(new RoundRectangle().CornerRadius(999))
                .Padding(14, 8)
                .Content(text)
                .GestureRecognizers(
                    new TapGestureRecognizer().OnTapped((_, _) =>
                    {
                        category = label;
                        RenderChips();
                        ApplyFilter();
                    })
                );

            // The selected chip is accent-on-white in both themes, so it takes plain values; the
            // unselected one has to follow the theme, so it takes the OnLight/OnDark builder.
            if (isSelected)
            {
                chip.BackgroundColor(AppColors.Accent);
                text.TextColor(Colors.White);
            }
            else
            {
                chip.BackgroundColor(e => e.OnLight(AppColors.SurfaceLight).OnDark(AppColors.SurfaceDark));
                text.TextColor(e => e.OnLight(AppColors.MutedLight).OnDark(AppColors.MutedDark));
            }

            chipsRow.Children.Add(chip);
        }
    }

    // ---- cards ---------------------------------------------------------------------------------

    private static View DemoCard()
        => new Border()
            .Card(20)
            .Padding(Ui.GapMd)
            .SoftShadow()
            .VisualStateGroups(
                new VisualStateGroupList
                {
                    new VisualState<Border>(VisualStates.VisualElement.Normal)
                    {
                        async border => await border.ScaleToAsync(1, 90)
                    },
                    new VisualState<Border>(VisualStates.VisualElement.PointerOver)
                    {
                        async border => await border.ScaleToAsync(1.02, 90)
                    },
                })
            .GestureRecognizers(
                new TapGestureRecognizer().OnTapped(async (sender, _) =>
                {
                    if (sender is BindableObject { BindingContext: DemoInfo demo })
                        await Shell.Current.GoToAsync(demo.Route);
                })
            )
            .Content(
                new Grid()
                .ColumnDefinitions(e => e.Auto().Star())
                .RowDefinitions(e => e.Auto().Auto().Auto())
                .ColumnSpacing(Ui.Gap)
                .RowSpacing(Ui.GapXs)
                .Children(
                    // Glyph badge, tinted from the item itself — a binding, not a hard-coded colour.
                    new Border()
                        .RowSpan(2)
                        .SizeRequest(46, 46)
                        .StrokeThickness(0)
                        .StrokeShape(new RoundRectangle().CornerRadius(14))
                        .BackgroundColor(e => e.Path(nameof(DemoInfo.Tint)).Convert((Color tint) => (tint ?? AppColors.Accent).WithAlpha(0.16f)))
                        .Content(
                            new Label()
                                .Text(e => e.Path(nameof(DemoInfo.Glyph)))
                                .FontSize(20)
                                .TextCenter()
                        ),

                    new Label()
                        .Column(1)
                        .Text(e => e.Path(nameof(DemoInfo.Title)))
                        .FontSize(16)
                        .FontAttributes(Bold)
                        .TextColor(e => e.OnLight(AppColors.TextLight).OnDark(AppColors.TextDark)),

                    new Label()
                        .Column(1)
                        .Row(1)
                        .Text(e => e.Path(nameof(DemoInfo.Summary)))
                        .Muted()
                        .FontSize(13)
                        .MaxLines(3)
                        .LineBreakMode(LineBreakMode.TailTruncation),

                    new Label()
                        .Row(2)
                        .ColumnSpan(2)
                        .Margin(0, Ui.GapSm, 0, 0)
                        .Text(e => e.Path(nameof(DemoInfo.Category)))
                        .Overline()
                        .TextColor(e => e.Path(nameof(DemoInfo.Tint)))
                )
            );

    private static View EmptyState()
        => new VerticalStackLayout()
            .Spacing(Ui.GapSm)
            .Padding(Ui.GapXl)
            .Center()
            .Children(
                new Label().Text("🫥").FontSize(34).TextCenterHorizontal(),
                new Label().Text("Nothing matches that search.").Muted().TextCenterHorizontal()
            );

    private static View Footer()
        => new Label()
            .Text("Built entirely with FmgLib.MauiMarkup — no XAML anywhere in this project.")
            .Muted()
            .FontSize(12)
            .TextCenterHorizontal()
            .Padding(Ui.GapLg, Ui.GapMd, Ui.GapLg, Ui.GapXl);

    // ---- filtering & responsiveness ------------------------------------------------------------

    private void ApplyFilter()
    {
        var matches = DemoCatalog.All.Where(demo =>
            (category == AllCategories || demo.Category == category) &&
            (query.Length == 0 ||
             demo.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
             demo.Summary.Contains(query, StringComparison.OrdinalIgnoreCase) ||
             demo.Category.Contains(query, StringComparison.OrdinalIgnoreCase)));

        visible.Clear();

        foreach (var demo in matches)
            visible.Add(demo);
    }

    /// <summary>
    /// Column count from the actual window width rather than the device idiom, so a desktop window
    /// dragged narrow reflows exactly like a phone.
    /// </summary>
    /// <param name="width">Current page width.</param>
    /// <returns>Number of columns.</returns>
    private static int SpanForWidth(double width) => width switch
    {
        < 640 => 1,
        < 960 => 2,
        < 1320 => 3,
        _ => 4
    };

    private void UpdateSpan()
    {
        if (itemsLayout is null)
            return;

        var span = SpanForWidth(Width);

        if (itemsLayout.Span != span)
            itemsLayout.Span = span;
    }
}
