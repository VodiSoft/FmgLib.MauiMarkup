# Kodda Derlenmiş Binding'ler

String yollu binding'ler (`e.Path("Name")`) çalışma zamanında reflection ile çözülür. **Derlenmiş binding'ler** string'i tipli bir expression ile değiştirir ve şunları kazandırır:

- **Performans** — binding ifadesi derleme zamanında çözülür; güncelleme başına reflection yok.
- **Derleme zamanı güvenliği** — geçersiz yol sessiz çalışma zamanı hatası değil, derleyici hatasıdır.
- **IntelliSense ve refactoring** — VM özelliğini yeniden adlandırın, binding takip eder.

## `Getter` — derlenmiş `Path`

`Path` yazacağınız her yerde `Getter` yazabilirsiniz:

```csharp
// önce — string yol, çalışma zamanı reflection
new Label().Text(e => e.Path("Text").Source(entry));

// sonra — derlenmiş
new Label().Text(e => e.Getter(static (Entry entry) => entry.Text).Source(entry));
```

View model ile:

```csharp
new Label()
    .Text(e => e.Getter(static (PersonViewModel vm) => vm.Name))

new Label()
    .Text(e => e
        .Getter(static (PersonViewModel vm) => vm.Address.City)
        .StringFormat("Şehir: {0}"))
```

Diğer tüm builder metotları (`Source`, `BindingMode`, `StringFormat`, `Converter`, `Convert`, `FallbackValue`, `TargetNullValue`…) `Getter` ile aynen `Path`'teki gibi birleşir.

> Lambda'yı `static` işaretleyin — yanlışlıkla closure yakalamayı önler ve niyeti (saf özellik erişimi) açıkça belirtir.

## `Setter` — iki yönlü güncellemeleri etkinleştirme

`TwoWay`/`OneWayToSource` derlenmiş binding'ler için ters işlemi sağlayın:

```csharp
new Entry()
    .Text(e => e
        .Getter(static (PersonViewModel vm) => vm.Name)
        .Setter(static (PersonViewModel vm, string value) => vm.Name = value)
        .BindingMode(BindingMode.TwoWay))
```

## Hangi İfadeler Geçerli?

Getter **basit bir özellik erişim ifadesi** olmalıdır. Desteklenenler:

```csharp
// Özellik erişimi (null-conditional zincirler dahil)
static (PersonViewModel vm) => vm.Name;
static (PersonViewModel vm) => vm.Address?.Street;

// Dizi / indeksleyici erişimi
static (PersonViewModel vm) => vm.PhoneNumbers[0];
static (PersonViewModel vm) => vm.Config["Font"];

// Cast'ler
static (Label label) => (label.BindingContext as PersonViewModel).Name;
static (Label label) => ((PersonViewModel)label.BindingContext).Name;
```

Desteklenmeyenler (bunlar converter veya hesaplanmış VM özelliği gerektirir):

```csharp
// Metot çağrıları
static (PersonViewModel vm) => vm.GetAddress();
static (PersonViewModel vm) => vm.Address?.ToString();

// Karmaşık ifadeler
static (PersonViewModel vm) => vm.Address?.Street + " " + vm.Address?.City;
static (PersonViewModel vm) => $"Name: {vm.Name}";
```

Dönüşüm gerekiyorsa getter'ı basit tutup `Convert` ekleyin:

```csharp
new Label()
    .Text(e => e
        .Getter(static (PersonViewModel vm) => vm.Name)
        .Convert((string name) => $"Ad: {name}"))
```

## `Binding.Create` (MAUI 9+)

.NET MAUI 9, bir `Func`'tan doğrudan tipli binding nesnesi kuran `BindingBase.Create`'i ekledi. FmgLib'in `Bindings(...)` metoduyla derlenmiş **multi-binding** için mükemmel eşleşir:

```csharp
public partial class MainPage : ContentPage, IFmgLibHotReload
{
    private readonly MainPageViewModel viewModel;

    public MainPage()
    {
        viewModel = new MainPageViewModel();
        this.InitializeHotReload();
    }

    public void Build()
    {
        this
        .BindingContext(viewModel)
        .Content(
            new VerticalStackLayout()
            .Spacing(20)
            .Children(
                new CheckBox()
                .IsChecked(e => e
                    .Bindings(
                        Binding.Create(static (MainPageViewModel m) => m.IsOver16),
                        Binding.Create(static (MainPageViewModel m) => m.HasPassedTest),
                        Binding.Create(static (MainPageViewModel m) => m.IsSuspended)
                    )
                    .Converter(new AllTrueMultiConverter())
                    .FallbackValue("Is Error.")
                    .TargetNullValue("Is Null.")
                ),

                new Label()
                .Text(e => e
                    .Bindings(
                        Binding.Create(static (MainPageViewModel m) => m.Id),
                        Binding.Create(static (MainPageViewModel m) => m.Name),
                        Binding.Create(static (MainPageViewModel m) => m.IsSuspended)
                    )
                    .StringFormat("{0} : {1} : {2}")
                )
            )
        );
    }
}
```

## Şablonlarda Derlenmiş Binding'ler

Şablonların içinde binding context öğedir; lambda'yı buna göre tipleyin:

```csharp
new CollectionView()
.ItemsSource(vm.Products)
.ItemTemplate(() =>
    new VerticalStackLayout().Children(
        new Label().Text(e => e.Getter(static (ProductVM p) => p.Name)),
        new Label().Text(e => e
            .Getter(static (ProductVM p) => p.Price)
            .StringFormat("{0:C}"))
    )
)
```

## Geçiş Kopya Kağıdı

| String binding | Derlenmiş binding |
|---|---|
| `e.Path("Name")` | `e.Getter(static (VM vm) => vm.Name)` |
| `e.Path("Address.City")` | `e.Getter(static (VM vm) => vm.Address.City)` |
| `e.Path("Text").Source(entry)` | `e.Getter(static (Entry x) => x.Text).Source(entry)` |
| `e.Path("Name").BindingMode(TwoWay)` | `.Setter(static (VM vm, string v) => vm.Name = v)` ekleyin |
| `Bindings(...)` içindeki `new Binding().Path("X")` | `Binding.Create(static (VM vm) => vm.X)` |

## İlgili Konular

- [Özellik Bağlama](data-binding.md)
- [MultiBinding](multi-binding.md)
- [Binding Converter'ları](binding-converters.md)
