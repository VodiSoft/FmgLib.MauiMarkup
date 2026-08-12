using System.Reflection;

namespace FmgLib.MauiMarkup.App;

public partial class MainPage : ContentPage, IFmgLibHotReload
{
    public MainPage()
    {
        this.InitializeHotReload();
    }

    public void Build()
    {
        var version = typeof(MauiApp).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        this
        .Content(
            new ScrollView()
            .Content(
                new Grid()
                .RowDefinitions(e => e.Star(90).Star(10))
                .Children(
                    new StackLayout()
                    .Spacing(25)
                    .Children(
                        new Label()
                        .Text("Hello, World!")
                        .FontSize(32)
                        .CenterHorizontal()
                        .SemanticHeadingLevel(SemanticHeadingLevel.Level1),

                        new Label()
                        .Text("Welcome to FmgLib .NET MAUI Markup App")
                        .FontSize(18)
                        .CenterHorizontal()
                        .SemanticDescription("Welcome to dot net Multi platform App U I")
                        .SemanticHeadingLevel(SemanticHeadingLevel.Level1),

                        new Label()
                        .Text("Current count: 0")
                        .FontSize(18)
                        .FontAttributes(Bold)
                        .CenterHorizontal()
                        .Assign(out CounterLabel),

                        new Button()
                        .Text("Click me")
                        .CenterHorizontal()
                        .OnClicked(OnCounterClicked)
                        .SemanticHint("Counts the number of times you click"),

                        new Image()
                        .Source("dotnet_bot.png")
                        .SizeRequest(250,310)
                        .CenterHorizontal()
                        .SemanticDescription("Cute dot net bot waving hi to you!")
                    ),

                    new Grid()
                    .BackgroundColor(AppColors.Primary)
                    .Row(1)
                    .Children(
                        new Label()
                        .Text($"dotNet version: {version}")
                        .TextColor(White)
                        .Center()
                    )
                )
            )
        );
    }

    private int _count = 0;

    // Populated by .Assign(out CounterLabel) in Build(), which runs from InitializeHotReload() —
    // the compiler cannot see that through the out parameter, so the field is marked null-forgiving
    // instead of leaving every new project with a CS8618 warning on first build.
    private Label CounterLabel = null!;


    private void OnCounterClicked(object? sender, EventArgs e)
    {
        _count++;
        CounterLabel.Text = $"Current count: {_count}";

        SemanticScreenReader.Announce(CounterLabel.Text);
    }
}
