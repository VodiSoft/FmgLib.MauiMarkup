# MultiBinding

**MultiBinding**, birden çok binding'i tek bir hedef özellik değerinde birleştirir. FmgLib.MauiMarkup bunu property builder'ın `Bindings(...)` metoduyla sunar — istediğiniz kadar `BindingBase` ekleyebilirsiniz.

## Builder Sözdizimi

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
                // Üç bool kaynağı tek IsChecked değerinde birleştir
                new CheckBox()
                .IsChecked(e => e
                    .Bindings(
                        new Binding().Path("Employee.IsOver16"),
                        new Binding().Path("Employee.HasPassedTest"),
                        new Binding().Path("Employee.IsSuspended").Converter(new InverterConverter())
                    )
                    .Converter(new AllTrueMultiConverter())
                    .FallbackValue("Is Error.")
                    .TargetNullValue("Is Null.")
                ),

                // Üç değeri tek biçimli string'de birleştir — converter gerekmez
                new Label()
                .Text(e => e
                    .Bindings(
                        new Binding().Path("Employee.Id"),
                        new Binding().Path("Employee.Name"),
                        new Binding().Path("Employee.IsSuspended")
                    )
                    .StringFormat("{0} : {1} : {2}")
                    .FallbackValue("Is Error.")
                    .TargetNullValue("Is Null.")
                )
            )
        );
    }
}
```

### Multi-binding builder metotları

| Metot | Açıklama |
|---|---|
| `.Bindings(params BindingBase[])` | Alt binding'ler (sınırsız sayıda). |
| `.StringFormat(string)` | Alt değerleri konumsal biçimler (`{0}`, `{1}`, …). Ayarlanırsa converter isteğe bağlıdır. |
| `.Converter(IMultiValueConverter)` | Alt değerleri tek sonuca dönüştürür. |
| `.Parameter(string)` | Multi-converter için `ConverterParameter`. |
| `.BindingMode(mode)` | Multi-binding'in modu. |
| `.FallbackValue(object)` / `.TargetNullValue(object)` | Tekli binding'lerdeki gibi. |

Her **alt** `Binding`'in kütüphanenin fluent `Binding` genişletmeleriyle yapılandırıldığına dikkat edin: `.Path(…)`, `.Source(…)`, `.Converter(…)`, `.ConverterParameter(…)`, `.UpdateSourceEventName(…)`. (Diğer `Binding` üyeleri, örn. `Mode`, nesne başlatıcıyla ayarlanabilir: `new Binding { Mode = BindingMode.OneWay }.Path("X")`.)

## `IMultiValueConverter` Yazmak

```csharp
public class AllTrueMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Any(v => v is not bool))
            return BindableProperty.UnsetValue;

        return values.OfType<bool>().All(b => b);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
```

`BindableProperty.UnsetValue` döndürmek binding'i `FallbackValue`'ya düşürür.

## Pratik Örnekler

**İki özellikten tam ad:**

```csharp
new Label()
    .Text(e => e
        .Bindings(
            new Binding().Path("FirstName"),
            new Binding().Path("LastName"))
        .StringFormat("{0} {1}"))
```

**Butonu yalnızca form geçerliyken etkinleştir:**

```csharp
new Entry().Assign(out var user).Placeholder("Kullanıcı"),
new Entry().Assign(out var pass).Placeholder("Şifre").IsPassword(true),

new Button()
    .Text("Giriş yap")
    .IsEnabled(e => e
        .Bindings(
            new Binding().Path("Text.Length").Source(user),
            new Binding().Path("Text.Length").Source(pass))
        .Converter(new AllPositiveMultiConverter()))
```

**Derlenmiş alt binding'ler** (bkz. [Derlenmiş Binding'ler](compiled-bindings.md)) — .NET MAUI 9+ ile alt binding'leri `Binding.Create` kullanarak expression'lardan kurabilir, multi-binding içinde derleme zamanı güvenliğini koruyabilirsiniz:

```csharp
new CheckBox()
.IsChecked(e => e
    .Bindings(
        Binding.Create(static (MainPageViewModel vm) => vm.IsOver16),
        Binding.Create(static (MainPageViewModel vm) => vm.HasPassedTest),
        Binding.Create(static (MainPageViewModel vm) => vm.IsSuspended)
    )
    .Converter(new AllTrueMultiConverter()))
```

## `Bind()` ile Tipli Multi-Binding

Converter sınıfı olmadan func tabanlı birleştirmeyi tercih ederseniz, düşük seviyeli `Bind()` genişletmesinin değerleri tuple olarak gelen 2, 3 veya 4 kaynaklı tipli overload'ları vardır:

```csharp
new Label()
    .Bind<Label, string, string, string>(Label.TextProperty,
        new Binding("FirstName"),
        new Binding("LastName"),
        convert: n => $"{n.Item1} {n.Item2}")

new Button()
    .Bind<Button, bool, bool, bool, bool>(Button.IsEnabledProperty,
        new Binding("HasUser"),
        new Binding("HasPassword"),
        new Binding("AcceptedTerms"),
        convert: v => v.Item1 && v.Item2 && v.Item3)
```

`converterParameter` alan ve iki yönlü senaryolar için `convertBack` fonksiyonu alan varyantlar da vardır.

## MultiBinding Ne Zaman?

- Hedef özellik gerçekten **birbirinden bağımsız değişen birden çok kaynağa** bağlıysa.
- View model'e hesaplanmış özellik ekleyemiyor (veya eklemek istemiyor)sanız.

Aksi hâlde `PropertyChanged` tetikleyen hesaplanmış bir VM özelliğini tercih edin — test etmesi ve debug etmesi converter tesisatından kolaydır.

## İlgili Konular

- [Özellik Bağlama](data-binding.md)
- [Binding Converter'ları](binding-converters.md)
- [Derlenmiş Binding'ler](compiled-bindings.md)
