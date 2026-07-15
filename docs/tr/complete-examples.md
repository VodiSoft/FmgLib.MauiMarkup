# Tam Örnekler

Diğer bölümlerdeki teknikleri birleştiren eksiksiz, gerçekçi sayfalar. Her örnek kendi kendine yeterlidir ve ilgili dokümanlara bağlantı verir.

## 1. Giriş Sayfası — yerleşim, referanslar, olaylar, gradyanlar

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
                // Üst: gradyanlı hero
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
                        .Text("Tekrar Hoş Geldiniz")
                        .FontSize(34)
                        .TextColor(Colors.White)
                        .Center()
                ),

                // Alt: form
                new VerticalStackLayout()
                .Row(1)
                .Padding(24)
                .Spacing(14)
                .Children(
                    new Entry()
                        .Assign(out var email)
                        .Placeholder("E-posta")
                        .Keyboard(Keyboard.Email),

                    new Entry()
                        .Assign(out var password)
                        .Placeholder("Şifre")
                        .IsPassword(true),

                    new Button()
                        .Assign(out var loginBtn)
                        .Text("Giriş Yap")
                        .IsEnabled(false)
                        .OnClicked(async b => await SignInAsync(email.Text, password.Text)),

                    new Label()
                        .Text("Şifremi unuttum?")
                        .TextDecorations(TextDecorations.Underline)
                        .CenterHorizontal()
                        .GestureRecognizers(
                            new TapGestureRecognizer()
                                .OnTapped((s, e) => Shell.Current.GoToAsync("reset"))
                        )
                )
            )
        );

        // Butonu yalnızca iki alan da doluyken etkinleştir
        void Validate(object? s, TextChangedEventArgs e) =>
            loginBtn.IsEnabled =
                !string.IsNullOrWhiteSpace(email.Text) &&
                !string.IsNullOrWhiteSpace(password.Text);

        email.TextChanged += Validate;
        password.TextChanged += Validate;
    }

    Task SignInAsync(string user, string pass) => Task.CompletedTask; // kimlik doğrulama mantığınız
}
```

Teknikler: [Grid](grid.md), [Gradyanlar](gradients-and-brushes.md), [Assign](assign-and-references.md), [Olaylar](event-handlers.md), [Jestler](gesture-recognizers.md), [Shell attached prop'ları](attached-properties.md).

## 2. Ürün Listesi Sayfası — MVVM, CollectionView, binding'ler, converter'lar

View model (standart MAUI MVVM, örn. `CommunityToolkit.Mvvm` ile):

```csharp
public partial class CatalogViewModel : ObservableObject
{
    [ObservableProperty] ObservableCollection<ProductVM> products = new();
    [ObservableProperty] bool isBusy;

    [RelayCommand] async Task RefreshAsync() { /* ürünleri yükle */ }
    [RelayCommand] void AddToCart(ProductVM p) { /* ... */ }
    [RelayCommand] void ToggleFavorite(ProductVM p) => p.IsFavorite = !p.IsFavorite;
}
```

Sayfa, [Hot Reload](hot-reload.md)'daki tipli MVVM taban sınıfıyla:

```csharp
public class CatalogPage : FmgLibContentPage<CatalogViewModel>
{
    public CatalogPage(CatalogViewModel vm) : base(vm) { }

    public override void Build()
    {
        this
        .Title("Katalog")
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
                        .Text("Ürün bulunamadı.")
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
                // favori düğmesi
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
                        .Text("Ekle")
                        .HeightRequest(34)
                        .Padding(10, 0)
                        .Command(BindingContext.AddToCartCommand)
                        .Bind(Button.CommandParameterProperty, ".")
                )
            )
        );
}
```

Teknikler: [Derlenmiş binding'ler](compiled-bindings.md), [Koleksiyonlar](collections-and-templates.md), [Bind API + converter'lar](binding-converters.md), alt ağaçları metotlara çıkarma.

## 3. Ayarlar Sayfası — stiller, switch'ler, yerelleştirme, tema değişimi

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
                    SectionHeader("Görünüm"),

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

                    SectionHeader("Dil"),

                    new Border().Content(
                        new Picker()
                            .Title("Dil")
                            .ItemsSource(new[] { "en-US", "tr-TR" })
                            .SelectedItem(Translator.Instance.CurrentCulture.Name)
                            .OnSelectedIndexChanged(p =>
                            {
                                if (p.SelectedItem is string lang)
                                    Translator.Instance.ChangeCulture(
                                        CultureInfo.GetCultureInfo(lang));
                            })),

                    SectionHeader("Hakkında"),

                    new Border().Content(
                        new VerticalStackLayout().Spacing(4).Children(
                            new Label().Text($"Sürüm {AppInfo.VersionString}"),
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

Teknikler: [Stiller](styling.md), [Yerelleştirme](localization-json.md), [Tema değerleri](fluent-properties.md), tekrarlayan UI için yardımcı metotlar.

## 4. Yeniden Kullanılabilir Bileşen — bindable özellikli özel `ContentView`

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

// onun için fluent Title()/Value() metotları üret:
[MauiMarkup(typeof(StatTile))]
public class MarkupTargets { }

// kullanım — view model'e binding dahil:
new HorizontalStackLayout().Spacing(10).Children(
    new StatTile().Title("Siparişler").Value(e => e.Path("OrderCount")),
    new StatTile().Title("Gelir").Value(e => e.Path("Revenue"))
)
```

Teknikler: [Üçüncü parti/kendi kontrol üretimi](third-party-controls.md), `Source(this)` öz-binding, bileşen kompozisyonu.

## Daha Fazla Örnek Kod

Deponun [`sample/`](../../sample) klasörü FmgLib.MauiMarkup ile kurulmuş eksiksiz uygulamalar içerir — 2048 oyunu, F1 TV tarayıcı, sipariş uygulaması (`MyOrderApp`, yukarıdaki ürün-kartı desenlerinin kaynağı) ve üçüncü parti entegrasyonlar için generator test uygulamaları.

## İlgili Konular

- [İpuçları ve Sorun Giderme](tips-and-troubleshooting.md)
- [Hot Reload](hot-reload.md)
