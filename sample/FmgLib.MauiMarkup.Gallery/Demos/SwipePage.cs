using System.Collections.ObjectModel;
using FmgLib.MauiMarkup.Gallery.Controls;
using FmgLib.MauiMarkup.Gallery.Models;

namespace FmgLib.MauiMarkup.Gallery.Demos;

/// <summary>
/// SwipeView: the swipe-to-action row, with fluent swipe items.
/// </summary>
public partial class SwipePage : DemoPage
{
    private readonly ObservableCollection<Product> inbox = [.. DemoViewModel.Catalogue.Take(5)];

    private Label status = null!;

    public SwipePage() => this.InitializeHotReload();

    protected override string DemoTitle => "SwipeView";

    protected override string DemoSummary =>
        "SwipeView wraps a row and reveals actions when swiped. Items are MenuItems, so text, icon, colour, command and OnInvoked are all fluent.";

    protected override IView[] BuildSections() =>
    [
        SwipeList(),
        Modes(),
        CustomContent()
    ];

    private IView SwipeList()
    {
        status = new Label().Text("Swipe a row left or right.").Muted();

        return Demo.Section(
            "Swipe to act",
            "RightItems appear when swiping left and LeftItems when swiping right — the opposite of what most people guess first.",
            Demo.Stage(
                new CollectionView()
                    .ItemsSource(inbox)
                    .SelectionMode(SelectionMode.None)
                    .HeightRequest(260)
                    .ItemTemplate(() => SwipeRow()),
                status
            ),
            Demo.Code("""
                new SwipeView()
                .RightItems(new SwipeItems
                {
                    new SwipeItem()
                        .Text("Delete")
                        .BackgroundColor(Colors.Red)
                        .IsDestructive(true)
                        .OnInvoked((s, e) => Remove(item)),
                })
                .Content(row)
                """));
    }

    private View SwipeRow()
        => new SwipeView()
            .RightItems(
                new SwipeItems
                {
                    new SwipeItem()
                        .Text("Archive")
                        .BackgroundColor(AppColors.Info)
                        .OnInvoked((sender, _) => Report(sender, "Archived")),

                    new SwipeItem()
                        .Text("Delete")
                        .BackgroundColor(AppColors.Danger)
                        .IsDestructive(true)
                        .OnInvoked((sender, _) => Remove(sender)),
                }
            )
            .LeftItems(
                new SwipeItems
                {
                    new SwipeItem()
                        .Text("Pin")
                        .BackgroundColor(AppColors.Warning)
                        .OnInvoked((sender, _) => Report(sender, "Pinned")),
                }
            )
            .Threshold(90)
            .Content(
                new Grid()
                .ColumnDefinitions(e => e.Auto().Star().Auto())
                .ColumnSpacing(Ui.Gap)
                .Padding(Ui.Gap)
                .BackgroundColor(e => e.OnLight(AppColors.SurfaceLight).OnDark(AppColors.SurfaceDark))
                .Children(
                    new Label().Text(e => e.Path(nameof(Product.Glyph))).FontSize(22).CenterVertical(),

                    new VerticalStackLayout()
                    .Column(1)
                    .CenterVertical()
                    .Children(
                        new Label().Text(e => e.Path(nameof(Product.Name))).FontAttributes(Bold),
                        new Label().Text(e => e.Path(nameof(Product.Category))).Muted().FontSize(12)
                    ),

                    new Label()
                        .Column(2)
                        .Text("‹ swipe ›")
                        .Muted()
                        .FontSize(11)
                        .CenterVertical()
                )
            );

    private void Report(object? sender, string action)
    {
        if (sender is BindableObject { BindingContext: Product product })
            status.Text = $"{action}: {product.Name}";
    }

    private void Remove(object? sender)
    {
        if (sender is not BindableObject { BindingContext: Product product })
            return;

        inbox.Remove(product);
        status.Text = $"Deleted: {product.Name}";
    }

    private static IView Modes()
        => Demo.Section(
            "Reveal or execute",
            "SwipeItems.Mode decides whether a full swipe reveals the buttons or runs the first one immediately — the difference between a mail app and a to-do list.",
            new VerticalStackLayout()
            .Spacing(Ui.GapSm)
            .Children(
                new SwipeView()
                    .RightItems(
                        new SwipeItems
                        {
                            new SwipeItem().Text("Done").BackgroundColor(AppColors.Success)
                        }
                        .Mode(SwipeMode.Execute)
                        .SwipeBehaviorOnInvoked(SwipeBehaviorOnInvoked.Close)
                    )
                    .Content(
                        new Border()
                            .Stage(12)
                            .Content(new Label().Text("Mode.Execute — a full swipe runs the action").FontSize(13))
                    ),

                new SwipeView()
                    .RightItems(
                        new SwipeItems
                        {
                            new SwipeItem().Text("Done").BackgroundColor(AppColors.Accent)
                        }
                        .Mode(SwipeMode.Reveal)
                    )
                    .Content(
                        new Border()
                            .Stage(12)
                            .Content(new Label().Text("Mode.Reveal (default) — swipe shows the button, tap runs it").FontSize(13))
                    )
            ),
            Demo.Code("""
                new SwipeItems { new SwipeItem().Text("Done") }
                    .Mode(SwipeMode.Execute)
                    .SwipeBehaviorOnInvoked(SwipeBehaviorOnInvoked.Close)
                """));

    private static IView CustomContent()
        => Demo.Section(
            "Fully custom swipe content",
            "SwipeItemView replaces the button with any view at all, so the revealed area can be as rich as the row itself.",
            new SwipeView()
                .RightItems(
                    new SwipeItems
                    {
                        new SwipeItemView()
                            .WidthRequest(96)
                            .Content(
                                new VerticalStackLayout()
                                .Center()
                                .Spacing(2)
                                .BackgroundColor(AppColors.Violet)
                                .Children(
                                    new Label().Text("💬").FontSize(20).TextCenterHorizontal(),
                                    new Label().Text("Reply").FontSize(11).TextColor(Colors.White).TextCenterHorizontal()
                                )
                            )
                    }
                )
                .Content(
                    new Border()
                        .Stage(12)
                        .HeightRequest(64)
                        .Content(new Label().Text("Swipe left for a custom action view").FontSize(13).CenterVertical())
                ),
            Demo.Code("""
                new SwipeItems
                {
                    new SwipeItemView()
                        .Command(vm.ReplyCommand)
                        .Content(new VerticalStackLayout().Children(
                            new Label().Text("💬"),
                            new Label().Text("Reply")))
                }
                """),
            Demo.Note("On phones prefer swipe actions to context menus; on desktop the reverse — see the Menus section of the docs."));
}
