# XAML'den C#'a

FmgLib.MauiMarkup, **XAML'de atayabildiğiniz her özellik için** bir fluent genişletme metodu sağlar. Bir avuç eşleme kuralını içselleştirdikten sonra herhangi bir XAML parçasını mekanik olarak çevirebilirsiniz.

## Eşleme Kuralları

| XAML kavramı | FmgLib.MauiMarkup karşılığı |
|---|---|
| `<Label ... />` öğesi | `new Label()` |
| Öznitelik `Text="Hi"` | `.Text("Hi")` |
| Attached property `Grid.Row="1"` | `.Row(1)` (bkz. [Attached Property'ler](attached-properties.md)) |
| Property element `<Label.Content>…` | Nesneyi alan aynı fluent metot: `.Content(new … )` |
| Çocuk koleksiyonu (örtük içerik) | Konteynere göre `.Children(...)`, `.Items(...)`, `.Content(...)` |
| `{Binding Path=Name}` | `.Text(e => e.Path("Name"))` (bkz. [Özellik Bağlama](data-binding.md)) |
| `{StaticResource key}` | Doğrudan C# nesnesini/sabitini kullanın |
| `{DynamicResource key}` | `.BackgroundColor(e => e.DynamicResource("key"))` |
| `{OnPlatform ...}` | `.FontSize(e => e.OnAndroid(20).OniOS(24).Default(22))` |
| `{OnIdiom ...}` | `.FontSize(e => e.OnPhone(20).OnDesktop(40).Default(30))` |
| `{AppThemeBinding Light=…, Dark=…}` | `.TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White))` |
| `x:Name="myLabel"` | `.Assign(out var myLabel)` |
| Olay özniteliği `Clicked="OnClicked"` | `.OnClicked(OnClicked)` |
| `<Style TargetType="Button">` | `new Style<Button>(e => e …)` (bkz. [Stiller](styling.md)) |
| `<DataTemplate>` | `.ItemTemplate(() => new …)` — görünümü üreten bir lambda |

## Yan Yana Örnekler

### Basit kontrol

**XAML**

```xml
<Image
    Source="dotnet_bot.png"
    HeightRequest="100"
    WidthRequest="150"
    Grid.Row="0"
    Grid.Column="1"
    Grid.RowSpan="2"
    Opacity=".8" />
```

**C#**

```csharp
new Image()
.Source("dotnet_bot.png")
.Row(0)
.Column(1)
.RowSpan(2)
.SizeRequest(150, 100)   // kolaylık: WidthRequest + HeightRequest'i tek seferde ayarlar
.Opacity(.8)
```

> `SizeRequest(width, height)`, kütüphanenin 1:1 özellik eşlemesinin ötesinde eklediği *kolaylık kısayollarından* biridir. Elbette `.WidthRequest(150).HeightRequest(100)` da yazabilirsiniz.

### Metin biçimlendirme

**XAML**

```xml
<Label Text="fmglib.mauimarkup"
       FontSize="12"
       Grid.Row="1"
       TextColor="Green"
       FontAttributes="Bold"
       Margin="5,3,0,5" />
```

**C#**

```csharp
new Label()
.Text("fmglib.mauimarkup")
.FontSize(12)
.Row(1)
.TextColor(Colors.Green)
.FontAttributes(FontAttributes.Bold)
.Margin(new Thickness(5, 3, 0, 5))
```

`Margin` ve `Padding` daha pratik overload'lara da sahiptir:

```csharp
.Margin(10)                    // tüm kenarlar
.Margin(10, 5)                 // yatay, dikey
.Margin(5, 3, 0, 5)            // sol, üst, sağ, alt
.Margin(left: 5, bottom: 5)    // isimli — belirtilmeyen kenarlar 0 olur
```

### İç içe içerikli bir sayfa

**XAML**

```xml
<ContentPage BackgroundImageSource="background.jpg">
    <StackLayout HorizontalOptions="Center" VerticalOptions="Center">
        <ActivityIndicator IsRunning="True"
                           HeightRequest="70"
                           WidthRequest="70" />
    </StackLayout>
</ContentPage>
```

**C# (sayfa sınıfının içinde)**

```csharp
this
.BackgroundImageSource("background.jpg")
.Content(
    new StackLayout()
    .Center()
    .Children(
        new ActivityIndicator()
        .IsRunning(true)
        .HeightRequest(70)
        .WidthRequest(70)
        .Center()
    )
);
```

### Binding'ler

**XAML**

```xml
<Label Text="{Binding Value, Source={x:Reference slider}, StringFormat='Slider: {0}'}" />
```

**C#**

```csharp
new Slider().Assign(out var slider).Minimum(1).Maximum(20),

new Label()
.Text(e => e.Path("Value").Source(slider).StringFormat("Slider: {0}"))
```

### DataTemplate'ler

**XAML**

```xml
<CollectionView ItemsSource="{Binding Products}">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Label Text="{Binding Name}" FontSize="20" />
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

**C#**

```csharp
new CollectionView()
.ItemsSource(e => e.Path("Products"))
.ItemTemplate(() =>
    new Label()
    .Text(e => e.Path("Name"))
    .FontSize(20)
)
```

`ItemTemplate`'e verilen lambda `<DataTemplate>` öğesinin rolünü üstlenir: gerçekleşen her öğe için bir kez çağrılır ve içindeki binding'ler öğenin `BindingContext`'ine göre çözülür.

### Kaynaklar ve stiller

**XAML**

```xml
<ContentPage.Resources>
    <Style TargetType="Entry">
        <Setter Property="TextColor" Value="White" />
    </Style>
</ContentPage.Resources>
```

**C#**

```csharp
this.Resources(
    new ResourceDictionary
    {
        new Style<Entry>(e => e.TextColor(Colors.White))
    }
)
```

## XAML'e Göre Kazanımlar

- **Tek dil** — mantık ve UI bir arada; code-behind/x:Name tesisatı yok.
- **Derleme zamanı güvenliği** — özellik adındaki yazım hatası çalışma zamanı çökmesi değil, derleme hatasıdır.
- **Tam IntelliSense ve refactoring** — view model özelliğini yeniden adlandırın, [derlenmiş binding'lerle](compiled-bindings.md) yazılmış tüm "binding"ler otomatik takip eder.
- **Gerçek kontrol akışı** — UI kurarken `if`, döngü, LINQ, local function ve yardımcı metotlar kullanın:

  ```csharp
  new VerticalStackLayout()
  .Children(
      menuItems.Select(item =>
          (IView)new Button().Text(item.Title).OnClicked(_ => Navigate(item))
      ).ToArray()
  )
  ```

- **Kompozisyon** — herhangi bir alt ağacı sıradan bir metoda çıkarın:

  ```csharp
  static Border Card(string title, View content) =>
      new Border()
      .StrokeShape(new RoundRectangle().CornerRadius(12))
      .Padding(16)
      .Content(
          new VerticalStackLayout().Spacing(8).Children(
              new Label().Text(title).FontAttributes(FontAttributes.Bold),
              content
          )
      );
  ```

## İlgili Konular

- [Fluent Özellikler](fluent-properties.md) — özellik metotlarının kabul ettiği her şey (değerler, binding'ler, platform builder'ları…)
- [Attached Property'ler](attached-properties.md) — tam XAML attached property → metot tablosu
- [Olay İşleyiciler](event-handlers.md) — `On<Event>` metot kalıpları
