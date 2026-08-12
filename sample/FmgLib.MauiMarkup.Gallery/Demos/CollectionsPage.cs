using FmgLib.MauiMarkup.Gallery.Controls;
using FmgLib.MauiMarkup.Gallery.Models;
using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// Item-based controls: an items source plus a template lambda, everywhere.
/// </summary>
public partial class CollectionsPage : DemoPage
{
    private readonly DemoViewModel viewModel = new();

    public CollectionsPage()
    {
        BindingContext = viewModel;

        this.InitializeHotReload();
    }

    protected override string DemoTitle => "Collections & Templates";

    protected override string DemoSummary =>
        "ItemTemplate takes a lambda that builds the row — the DataTemplate of the markup world. Inside it, each view's BindingContext is the item.";

    protected override IView[] BuildSections() =>
    [
        ListWithTemplate(),
        GridLayout(),
        BindableLayoutTags(),
        Carousel()
    ];

    private static IView ListWithTemplate()
        => Demo.Section(
            "A searchable list",
            "The list is a CollectionView with an ItemTemplate lambda; the search box writes to the view model, which refills the ObservableCollection. EmptyView covers the no-results case.",
            Demo.Stage(
                new SearchBar()
                    .Placeholder("Filter products…")
                    .Text(e => e.Path(nameof(DemoViewModel.Search)).BindingMode(BindingMode.TwoWay)),

                new CollectionView()
                    .ItemsSource(e => e.Path(nameof(DemoViewModel.Products)))
                    .SelectionMode(SelectionMode.None)
                    .HeightRequest(280)
                    .EmptyView(
                        new VerticalStackLayout()
                        .Center()
                        .Padding(Ui.GapLg)
                        .Spacing(Ui.GapXs)
                        .Children(
                            new Label().Text("🔍").FontSize(28).TextCenterHorizontal(),
                            new Label().Text("No products match that search.").Muted().TextCenterHorizontal()
                        )
                    )
                    .ItemTemplate(() => ProductRow())
            ),
            Demo.Code("""
                new CollectionView()
                    .ItemsSource(e => e.Path("Products"))
                    .EmptyView(new Label().Text("No products match."))
                    .ItemTemplate(() =>
                        new Grid()
                        .Children(
                            new Label().Text(e => e.Path("Name")),
                            new Label().Text(e => e.Path("Price").StringFormat("{0:C}"))
                        ))
                """));

    private static View ProductRow()
        => new Grid()
            .ColumnDefinitions(e => e.Auto().Star().Auto())
            .ColumnSpacing(Ui.Gap)
            .Padding(0, Ui.GapSm)
            .Children(
                new Label()
                    .Text(e => e.Path(nameof(Product.Glyph)))
                    .FontSize(24)
                    .CenterVertical(),

                new VerticalStackLayout()
                .Column(1)
                .CenterVertical()
                .Children(
                    new Label()
                        .Text(e => e.Path(nameof(Product.Name)))
                        .FontAttributes(Bold),

                    new Label()
                        .Text(e => e
                            .Path(nameof(Product.Category))
                            .Path(nameof(Product.Rating))
                            .MultiConvert((string category, double rating) => $"{category} · ★ {rating:F1}"))
                        .Muted()
                        .FontSize(12)
                ),

                new VerticalStackLayout()
                .Column(2)
                .CenterVertical()
                .Children(
                    new Label()
                        .Text(e => e.Path(nameof(Product.Price)).StringFormat("{0:C}"))
                        .FontAttributes(Bold)
                        .TextColor(AppColors.Accent)
                        .TextRight(),

                    new Label()
                        .Text(e => e.Path(nameof(Product.InStock)).Convert((bool inStock) => inStock ? "in stock" : "sold out"))
                        .FontSize(11)
                        .TextRight()
                        .TextColor(e => e.Path(nameof(Product.InStock)).Convert((bool inStock) => inStock ? AppColors.Success : AppColors.Danger))
                )
            );

    private static IView GridLayout()
        => Demo.Section(
            "Grid layout and spacing",
            "ItemsLayout swaps the list for a grid — or a horizontal strip — without touching the template.",
            Demo.Stage(
                new CollectionView()
                    .ItemsSource(DemoViewModel.Catalogue)
                    .SelectionMode(SelectionMode.None)
                    .HeightRequest(230)
                    .ItemsLayout(
                        new GridItemsLayout(2, ItemsLayoutOrientation.Vertical)
                            .VerticalItemSpacing(Ui.GapSm)
                            .HorizontalItemSpacing(Ui.GapSm))
                    .ItemTemplate(() =>
                        new Border()
                            .Card(12)
                            .Padding(Ui.Gap)
                            .Content(
                                new VerticalStackLayout()
                                .Spacing(2)
                                .Children(
                                    new Label().Text(e => e.Path(nameof(Product.Glyph))).FontSize(22),
                                    new Label().Text(e => e.Path(nameof(Product.Name))).FontSize(13).FontAttributes(Bold),
                                    new Label().Text(e => e.Path(nameof(Product.Price)).StringFormat("{0:C}")).Muted().FontSize(12)
                                )
                            ))
            ),
            Demo.Code("""
                .ItemsLayout(new GridItemsLayout(span: 2, ItemsLayoutOrientation.Vertical)
                    .VerticalItemSpacing(8)
                    .HorizontalItemSpacing(8))

                // …or a horizontal strip:
                .ItemsLayout(new LinearItemsLayout(ItemsLayoutOrientation.Horizontal).ItemSpacing(10))
                """));

    private static IView BindableLayoutTags()
        => Demo.Section(
            "BindableLayout — templates in any layout",
            "For a handful of items inside a regular layout, attach the items source to the layout itself. No virtualization, so keep it to dozens — but for a tag cloud that is exactly right.",
            new Border()
            .Stage()
            .Content(
                new FlexLayout()
                    .Wrap(FlexWrap.Wrap)
                    .JustifyContent(FlexJustify.Start)
                    .BindableLayoutItemsSource(e => e.Path(nameof(DemoViewModel.Tags)))
                    .BindableLayoutItemTemplate(new DataTemplate(() =>
                        new Border()
                            .Margin(0, 0, Ui.GapSm, Ui.GapSm)
                            .Padding(12, 6)
                            .StrokeThickness(0)
                            .StrokeShape(new RoundRectangle().CornerRadius(999))
                            .BackgroundColor(AppColors.Accent.WithAlpha(0.14f))
                            .FlexBasis(FlexBasis.Auto)
                            .Content(
                                new Label()
                                    .Text(e => e.Path("."))
                                    .FontSize(12)
                                    .TextColor(AppColors.Accent)
                            )))
            ),
            Demo.Code("""
                new FlexLayout()
                    .Wrap(FlexWrap.Wrap)
                    .BindableLayoutItemsSource(e => e.Path("Tags"))
                    .BindableLayoutItemTemplate(new DataTemplate(() =>
                        new Border().Content(new Label().Text(e => e.Path(".")))))
                """),
            Demo.Note("Path(\".\") binds to the item itself — the way to render a collection of plain strings."));

    private static IView Carousel()
        => Demo.Section(
            "CarouselView + IndicatorView",
            "Two controls wired together with Assign and InvokeOnElement, because IndicatorView is set on the carousel rather than the other way around.",
            Demo.Stage(
                new CarouselView()
                    .Assign(out var carousel)
                    .ItemsSource(DemoViewModel.Catalogue.Take(4).ToList())
                    .HeightRequest(150)
                    .PeekAreaInsets(new Thickness(30, 0))
                    .ItemTemplate(() =>
                        new Border()
                            .Card(16)
                            .Margin(Ui.GapSm, 0)
                            .Content(
                                new VerticalStackLayout()
                                .Center()
                                .Spacing(Ui.GapXs)
                                .Children(
                                    new Label().Text(e => e.Path(nameof(Product.Glyph))).FontSize(34).TextCenterHorizontal(),
                                    new Label().Text(e => e.Path(nameof(Product.Name))).FontAttributes(Bold).TextCenterHorizontal(),
                                    new Label().Text(e => e.Path(nameof(Product.Price)).StringFormat("{0:C}")).Muted().TextCenterHorizontal()
                                )
                            )),

                new IndicatorView()
                    .IndicatorColor(e => e.OnLight(AppColors.BorderLight).OnDark(AppColors.BorderDark))
                    .SelectedIndicatorColor(AppColors.Accent)
                    .CenterHorizontal()
                    .InvokeOnElement(indicator => carousel.IndicatorView = indicator)
            ),
            Demo.Code("""
                new CarouselView().Assign(out var carousel).ItemsSource(items).ItemTemplate(() => …),

                new IndicatorView()
                    .SelectedIndicatorColor(AppColors.Accent)
                    .InvokeOnElement(indicator => carousel.IndicatorView = indicator)
                """));
}
