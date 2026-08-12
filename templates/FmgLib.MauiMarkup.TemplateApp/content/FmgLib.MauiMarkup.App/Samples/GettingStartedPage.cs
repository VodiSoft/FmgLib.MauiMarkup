using System.Reflection;
using Microsoft.Maui.Graphics;
using FmgLib.MauiMarkup;
using static Microsoft.Maui.Graphics.Colors;
using static Microsoft.Maui.Controls.FontAttributes;
using Microsoft.Maui.Controls.Shapes;

namespace FmgLib.MauiMarkup.App.Samples;

public partial class GettingStartedPage : ContentPage, IFmgLibHotReload
{
    public GettingStartedPage()
    {
        this.InitializeHotReload();
    }

    public void Build()
    {
        var version = typeof(MauiApp).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        var accent = "#512BD4".ToColor();
        var secondary = "#8B5CF6".ToColor();

        this
        // Bound rather than assigned, so the navigation bar agrees with the tab title after a
        // language switch. The body of this page stays in English on purpose — the Settings page is
        // where localization is demonstrated.
        .Title(e => e.Translate("Nav_GettingStarted"))
        .Content(
            new ScrollView()
            .Content(
                new VerticalStackLayout()
                .Spacing(24)
                .Padding(20)
                .Children(
                    // Welcome Header
                    new VerticalStackLayout()
                    .Spacing(8)
                    .Children(
                        new Label()
                        .Text("Welcome to FmgLib MAUI Markup 🎉")
                        .FontSize(28)
                        .FontAttributes(Bold)
                        .TextColor(accent),

                        new Label()
                        .Text("Build beautiful .NET MAUI apps with fluent C# markup - No XAML required!")
                        .FontSize(15)
                        .TextColor("#6B7280".ToColor())
                        .Margin(0, 0, 0, 16)
                    ),

                    // Feature Cards
                    new Grid()
                    .RowDefinitions(e => e.Auto().Auto())
                    .ColumnDefinitions(e => e.Star().Star())
                    .RowSpacing(16)
                    .ColumnSpacing(16)
                    .Children(
                        FeatureCard("🎨", "Modern Design", "Beautiful UI components", accent).Row(0).Column(0),
                        FeatureCard("⚡", "Hot Reload", "Live code updates", secondary).Row(0).Column(1),
                        FeatureCard("📱", "Cross Platform", "iOS, Android & more", "#EC4899".ToColor()).Row(1).Column(0),
                        FeatureCard("🚀", "MVVM Ready", "Clean architecture", "#F59E0B".ToColor()).Row(1).Column(1)
                    ),

                    // Version Info
                    new Border()
                    .Padding(16)
                    .StrokeThickness(1)
                    .Stroke("#E5E7EB".ToColor())
                    .BackgroundColor("#F9FAFB".ToColor())
                    .StrokeShape(new RoundRectangle().CornerRadius(12))
                    .Content(
                        new HorizontalStackLayout()
                        .Spacing(12)
                        .Children(
                            new Label()
                            .Text("ℹ️")
                            .FontSize(24),

                            new VerticalStackLayout()
                            .Spacing(4)
                            .Children(
                                new Label()
                                .Text($".NET Version: {version ?? "N/A"}")
                                .FontSize(14)
                                .FontAttributes(Bold),

                                new Label()
                                .Text("Powered by FmgLib.MauiMarkup")
                                .FontSize(12)
                                .TextColor(Colors.Gray)
                            )
                        )
                    ),

                    // Sample Pages Section
                    new VerticalStackLayout()
                    .Spacing(12)
                    .Children(
                        new Label()
                        .Text("📚 Explore Sample Pages")
                        .FontSize(20)
                        .FontAttributes(Bold)
                        .TextColor("#1F2937".ToColor()),

                        new Label()
                        .Text("Check out the other pages to see MVVM patterns, animations, and modern UI designs in action!")
                        .FontSize(14)
                        .TextColor("#6B7280".ToColor())
                        .Margin(0, 0, 0, 8),

                        NavigationCard("📊", "Dashboard", "Modern dashboard with gradient cards", accent)
                        .GestureRecognizers(
                            new TapGestureRecognizer()
                            .OnTapped(async (s,e) => await AppShell.Current.GoToAsync(nameof(Samples.Pages.DashboardPage)))
                        ),
                        NavigationCard("🛍️", "Products", "MVVM list with search & filter", secondary)
                        .GestureRecognizers(
                            new TapGestureRecognizer()
                            .OnTapped(async (s,e) => await AppShell.Current.GoToAsync(nameof(Samples.Pages.ProductsPage)))
                        ),
                        NavigationCard("✨", "Animations", "Smooth UI animations showcase", "#EC4899".ToColor())
                        .GestureRecognizers(
                            new TapGestureRecognizer()
                            .OnTapped(async (s,e) => await AppShell.Current.GoToAsync(nameof(Samples.Pages.AnimationsPage)))
                        ),
                        NavigationCard("🌍", "Settings", "Live language (EN/TR/AR + RTL) and theme switching", "#0EA5E9".ToColor())
                        .GestureRecognizers(
                            new TapGestureRecognizer()
                            .OnTapped(async (s,e) => await AppShell.Current.GoToAsync(nameof(Samples.Pages.SettingsPage)))
                        )
                    ),

                    // Quick Tips
                    new Border()
                    .Padding(16)
                    .StrokeThickness(1)
                    .Stroke("#E5E7EB".ToColor())
                    .BackgroundColor(Colors.White)
                    .StrokeShape(new RoundRectangle().CornerRadius(12))
                    .Content(
                        new VerticalStackLayout()
                        .Spacing(8)
                        .Children(
                            new Label()
                            .Text("💡 Quick Tips")
                            .FontSize(16)
                            .FontAttributes(Bold)
                            .TextColor("#1F2937".ToColor()),

                            TipItem("Use fluent API for clean, readable code"),
                            TipItem("Hot Reload works with markup changes"),
                            TipItem("Combine with MVVM for best results"),
                            TipItem("Explore FmgLib extensions for more power")
                        )
                    )
                )
            )
        );
    }

    private Border FeatureCard(string icon, string title, string description, Color accentColor)
    {
        return new Border()
            .StrokeThickness(0)
            .BackgroundColor(accentColor)
            .StrokeShape(new RoundRectangle().CornerRadius(16))
            .HeightRequest(120)
            .Shadow(new Shadow()
                .Brush(new SolidColorBrush($"#40{accentColor.ToHex().TrimStart('#')}".ToColor()))
                .Offset(new Point(0, 4))
                .Radius(8)
            )
            .Content(
                new VerticalStackLayout()
                .Padding(16)
                .Spacing(8)
                .Children(
                    new Label()
                    .Text(icon)
                    .FontSize(32),

                    new Label()
                    .Text(title)
                    .FontSize(15)
                    .FontAttributes(Bold)
                    .TextColor(Colors.White),

                    new Label()
                    .Text(description)
                    .FontSize(12)
                    .TextColor("#F0F0F0".ToColor())
                )
            );
    }

    private Border NavigationCard(string icon, string title, string description, Color accentColor)
    {
        return new Border()
            .Padding(16)
            .Margin(0, 4)
            .StrokeThickness(1)
            .Stroke("#E5E7EB".ToColor())
            .BackgroundColor(Colors.White)
            .StrokeShape(new RoundRectangle().CornerRadius(12))
            .Content(
                new Grid()
                .ColumnDefinitions(e => e.Auto().Star().Auto())
                .ColumnSpacing(12)
                .Children(
                    new Border()
                    .Column(0)
                    .WidthRequest(48).HeightRequest(48)
                    .BackgroundColor($"#20{accentColor.ToHex().TrimStart('#')}".ToColor())
                    .StrokeThickness(0)
                    .StrokeShape(new RoundRectangle().CornerRadius(12))
                    .Content(
                        new Label()
                        .Text(icon)
                        .FontSize(24)
                        .Center()
                    ),

                    new VerticalStackLayout()
                    .Column(1)
                    .Spacing(4)
                    .CenterVertical()
                    .Children(
                        new Label()
                        .Text(title)
                        .FontSize(15)
                        .FontAttributes(Bold)
                        .TextColor("#1F2937".ToColor()),

                        new Label()
                        .Text(description)
                        .FontSize(13)
                        .TextColor("#6B7280".ToColor())
                    ),

                    new Label()
                    .Column(2)
                    .Text("→")
                    .FontSize(24)
                    .TextColor("#D1D5DB".ToColor())
                    .CenterVertical()
                )
            );
    }

    private HorizontalStackLayout TipItem(string text)
    {
        return new HorizontalStackLayout()
            .Spacing(8)
            .Children(
                new Label()
                .Text("•")
                .FontSize(14)
                .TextColor("#512BD4".ToColor())
                .FontAttributes(Bold),

                new Label()
                .Text(text)
                .FontSize(13)
                .TextColor("#4B5563".ToColor())
            );
    }
}
