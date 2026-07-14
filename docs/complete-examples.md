# Complete Examples

Full, realistic pages combining the techniques from the other chapters. Each example is self-contained and annotated with links to the relevant docs.

## 1. Login Page — layout, references, events, gradients

```csharp
using FmgLib.MauiMarkup;

public partial class LoginPage : ContentPage, IFmgLibHotReload
{
    public LoginPage() => this.InitializeHotReload();

    public void Build()
    {
        this
        .ShellNavBarIsVisible(false)
        .Content(
            new Grid()
            .RowDefinitions(e => e.Star(4).Star(6))
            .Children(
                // Top: gradient hero
                new Border()
                .StrokeThickness(0)
                .Background(
                    new LinearGradientBrush()
                    .StartPoint(new Point(0, 0))
                    .EndPoint(new Point(1, 1))
                    .GradientStops(new List<GradientStop>
                    {
                        new GradientStop("#4C3BCF".ToColor(), 0f),
                        new GradientStop("#4B70F5".ToColor(), 1f)
                    }))
                .Content(
                    new Label()
                        .Text("Welcome Back")
                        .FontSize(34)
                        .TextColor(Colors.White)
                        .Center()
                ),

                // Bottom: form
                new VerticalStackLayout()
                .Row(1)
                .Padding(24)
                .Spacing(14)
                .Children(
                    new Entry()
                        .Assign(out var email)
                        .Placeholder("E-mail")
                        .Keyboard(Keyboard.Email),

                    new Entry()
                        .Assign(out var password)
                        .Placeholder("Password")
                        .IsPassword(true),

                    new Button()
                        .Assign(out var loginBtn)
                        .Text("Sign In")
                        .IsEnabled(false)
                        .OnClicked(async b => await SignInAsync(email.Text, password.Text)),

                    new Label()
                        .Text("Forgot password?")
                        .TextDecorations(TextDecorations.Underline)
                        .CenterHorizontal()
                        .GestureRecognizers(
                            new TapGestureRecognizer()
                                .OnTapped((s, e) => Shell.Current.GoToAsync("reset"))
                        )
                )
            )
        );

        // Enable the button only when both fields have text
        void Validate(object? s, TextChangedEventArgs e) =>
            loginBtn.IsEnabled =
                !string.IsNullOrWhiteSpace(email.Text) &&
                !string.IsNullOrWhiteSpace(password.Text);

        email.TextChanged += Validate;
        password.TextChanged += Validate;
    }

    Task SignInAsync(string user, string pass) => Task.CompletedTask; // your auth logic
}
```

Techniques: [Grid](grid.md), [Gradients](gradients-and-brushes.md), [Assign](assign-and-references.md), [Events](event-handlers.md), [Gestures](gesture-recognizers.md), [Shell attached props](attached-properties.md).

## 2. Product List Page — MVVM, CollectionView, bindings, converters

The view model (standard MAUI MVVM, e.g. with `CommunityToolkit.Mvvm`):

```csharp
public partial class CatalogViewModel : ObservableObject
{
    [ObservableProperty] ObservableCollection<ProductVM> products = new();
    [ObservableProperty] bool isBusy;

    [RelayCommand] async Task RefreshAsync() { /* load products */ }
    [RelayCommand] void AddToCart(ProductVM p) { /* ... */ }
    [RelayCommand] void ToggleFavorite(ProductVM p) => p.IsFavorite = !p.IsFavorite;
}
```

The page, using the typed MVVM base class from [Hot Reload](hot-reload.md):

```csharp
public class CatalogPage : FmgLibContentPage<CatalogViewModel>
{
    public CatalogPage(CatalogViewModel vm) : base(vm) { }

    public override void Build()
    {
        this
        .Title("Catalog")
        .Content(
            new RefreshView()
            .IsRefreshing(e => e.Getter(static (CatalogViewModel v) => v.IsBusy))
            .Command(BindingContext.RefreshCommand)
            .Content(
                new CollectionView()
                .ItemsSource(e => e.Getter(static (CatalogViewModel v) => v.Products))
                .SelectionMode(SelectionMode.None)
                .ItemsLayout(new GridItemsLayout(2, ItemsLayoutOrientation.Vertical)
                    .VerticalItemSpacing(8)
                    .HorizontalItemSpacing(8))
                .EmptyView(
                    new Label()
                        .Text("No products found.")
                        .TextColor(Colors.Gray)
                        .Center())
                .ItemTemplate(() => ProductCard())
            )
        );
    }

    View ProductCard() =>
        new Border()
        .Padding(10)
        .StrokeShape(new RoundRectangle().CornerRadius(12))
        .BackgroundColor(e => e.OnLight(Colors.White).OnDark("#22222E".ToColor()))
        .Content(
            new Grid()
            .RowDefinitions(e => e.Auto().Star().Auto().Auto())
            .RowSpacing(6)
            .Children(
                // favorite toggle
                new ImageButton()
                    .Bind(ImageButton.SourceProperty, nameof(ProductVM.IsFavorite),
                          convert: (bool fav) => (ImageSource)(fav ? "heart_filled.png" : "heart.png"))
                    .BackgroundColor(Colors.Transparent)
                    .SizeRequest(28, 28)
                    .AlignRight()
                    .Command(BindingContext.ToggleFavoriteCommand)
                    .Bind(ImageButton.CommandParameterProperty, "."),

                new Image()
                    .Row(1)
                    .Source(e => e.Getter(static (ProductVM p) => p.Image))
                    .SizeRequest(90, 90)
                    .CenterHorizontal(),

                new Label()
                    .Row(2)
                    .Text(e => e.Getter(static (ProductVM p) => p.Name))
                    .FontAttributes(FontAttributes.Bold)
                    .LineBreakMode(LineBreakMode.TailTruncation),

                new Grid()
                .Row(3)
                .ColumnDefinitions(e => e.Star().Auto())
                .Children(
                    new Label()
                        .Text(e => e.Getter(static (ProductVM p) => p.Price).StringFormat("{0:C}"))
                        .TextColor(Colors.SeaGreen)
                        .CenterVertical(),

                    new Button()
                        .Column(1)
                        .Text("Add")
                        .HeightRequest(34)
                        .Padding(10, 0)
                        .Command(BindingContext.AddToCartCommand)
                        .Bind(Button.CommandParameterProperty, ".")
                )
            )
        );
}
```

Techniques: [Compiled bindings](compiled-bindings.md), [Collections](collections-and-templates.md), [Bind API + converters](binding-converters.md), extracting subtrees into methods.

## 3. Settings Page — styles, switches, localization, theme switching

```csharp
public class SettingsPage : FmgLibContentPage
{
    public override void Build()
    {
        this
        .Title(e => e.Translate("SettingsTitle"))
        .Resources(
            new ResourceDictionary
            {
                new Style<Label>(e => e.FontSize(16))
                {
                    // section headers via explicit style below
                },
                new Style<Border>(e => e
                    .Padding(16)
                    .StrokeThickness(0)
                    .BackgroundColor(e => e.OnLight(Colors.White).OnDark("#1D1D28".ToColor()))),
            })
        .Content(
            new ScrollView()
            .Content(
                new VerticalStackLayout()
                .Padding(16)
                .Spacing(12)
                .Children(
                    SectionHeader("Appearance"),

                    new Border().Content(
                        new Grid()
                        .ColumnDefinitions(e => e.Star().Auto())
                        .Children(
                            new Label().Text(e => e.Translate("DarkMode")).CenterVertical(),
                            new Switch()
                                .Column(1)
                                .IsToggled(Application.Current!.UserAppTheme == AppTheme.Dark)
                                .OnToggled((s, e) =>
                                    Application.Current!.UserAppTheme =
                                        e.Value ? AppTheme.Dark : AppTheme.Light)
                        )),

                    SectionHeader("Language"),

                    new Border().Content(
                        new Picker()
                            .Title("Language")
                            .ItemsSource(new[] { "en-US", "tr-TR" })
                            .SelectedItem(Translator.Instance.CurrentCulture.Name)
                            .OnSelectedIndexChanged(p =>
                            {
                                if (p.SelectedItem is string lang)
                                    Translator.Instance.ChangeCulture(
                                        CultureInfo.GetCultureInfo(lang));
                            })),

                    SectionHeader("About"),

                    new Border().Content(
                        new VerticalStackLayout().Spacing(4).Children(
                            new Label().Text($"Version {AppInfo.VersionString}"),
                            new Label().Text("© 2026 MyCompany").TextColor(Colors.Gray).FontSize(12)
                        ))
                )
            )
        );
    }

    static Label SectionHeader(string text) =>
        new Label()
            .Text(text)
            .FontSize(13)
            .TextColor(Colors.Gray)
            .Margin(4, 8, 0, 0);
}
```

Techniques: [Styling](styling.md), [Localization](localization-json.md), [Theme values](fluent-properties.md), helper methods for repeated UI.

## 4. Reusable Component — custom `ContentView` with bindable properties

```csharp
public class StatTile : ContentView, IFmgLibHotReload
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(StatTile), "");
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(string), typeof(StatTile), "");

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

    public StatTile() => this.InitializeHotReload();

    public void Build() =>
        this.Content(
            new Border()
            .Padding(16)
            .StrokeShape(new RoundRectangle().CornerRadius(14))
            .Content(
                new VerticalStackLayout().Spacing(4).Children(
                    new Label()
                        .Text(e => e.Path(nameof(Value)).Source(this))
                        .FontSize(28)
                        .FontAttributes(FontAttributes.Bold),
                    new Label()
                        .Text(e => e.Path(nameof(Title)).Source(this))
                        .FontSize(13)
                        .TextColor(Colors.Gray)
                )
            )
        );
}

// generate fluent Title()/Value() methods for it:
[MauiMarkup(typeof(StatTile))]
public class MarkupTargets { }

// usage — including bindings to a view model:
new HorizontalStackLayout().Spacing(10).Children(
    new StatTile().Title("Orders").Value(e => e.Path("OrderCount")),
    new StatTile().Title("Revenue").Value(e => e.Path("Revenue"))
)
```

Techniques: [Third-party/own controls generation](third-party-controls.md), `Source(this)` self-binding, component composition.

## More Sample Code

The repository's [`sample/`](../sample) folder contains complete applications built with FmgLib.MauiMarkup — including a 2048 game, an F1 TV browser, an order app (`MyOrderApp`, the source of the product-card patterns above), and generator test apps for third-party integrations (`TestUraniumUI`, `GeneratedExam`).

## Related Topics

- [Tips & Troubleshooting](tips-and-troubleshooting.md)
- [Hot Reload](hot-reload.md)
