# MultiBinding

**MultiBinding**, tek bir hedef özelliği aynı anda birden çok kaynaktan besler. FmgLib.MauiMarkup'ta bunu normal binding'de kullandığınız property builder ile kurarsınız: `.Path(...)` (ya da `.Getter(...)`) çağrısını birden fazla kez yapın ve zinciri, toplanan değerleri birleştiren bir metotla kapatın.

```csharp
new Label()
    .Text(e => e
        .Path(nameof(Person.FirstName))
        .Path(nameof(Person.LastName))
        .MultiConvert((string first, string last) => $"{first} {last}"))
```

Her `.Path()` kendi alt binding'ini açar. Ardından gelen her şey — `.Source()`, `.BindingMode()`, `.StringFormat()`, `.Converter()`, `.Parameter()`, `.Convert()`, `.ConvertBack()`, `.FallbackValue()`, `.TargetNullValue()` — en son açılan alt binding'e aittir. `Multi…` metotları ise multi binding'in tamamına aittir.

Tek bir `.Path()` hâlâ düz bir `Binding` üretir; yani sıradan binding'lerde hiçbir şey değişmez. Multi binding yalnızca ikinci bir kaynak tanımladığınızda ya da `Multi…` metotlarından birini çağırdığınızda devreye girer.

## Değerleri birleştirmek

`MultiConvert`, değerleri tanımlanma sırasıyla alır. Parametre tipleri her alt binding'in ürettiği tiple eşleşmeli, dönüş tipi ise hedef özelliğin tipi olmalıdır.

```csharp
new VerticalStackLayout()
.Children(
    new Slider().Assign(out var width).Minimum(1).Maximum(300),
    new Slider().Assign(out var height).Minimum(1).Maximum(300),

    new Label()
        .Text(e => e
            .Path(nameof(Slider.Value)).Source(width)
            .Path(nameof(Slider.Value)).Source(height)
            .MultiConvert((double w, double h) => $"{w:F0} × {h:F0} = {w * h:F0} px²"))
)
```

2 ile 9 alt binding için overload'lar vardır. `.Path()` sayısı ile delege parametre sayısı uyuşmazsa hata, binding kurulurken her iki sayıyı da içeren bir mesajla bildirilir.

### Tek bir kaynağı önceden dönüştürmek

`.Convert()`, kendisinden önce gelen alt binding'e aittir; böylece bir kaynak `MultiConvert`'e ulaşmadan önce şekillendirilebilir:

```csharp
new Label()
    .Text(e => e
        .Path(nameof(Person.Age)).Convert((int age) => age >= 18)
        .Path(nameof(Person.FirstName))
        .MultiConvert((bool adult, string name) => adult ? name : $"{name} (reşit değil)"))
```

Birinci alt binding kendi `.Convert()`'i sayesinde `bool`, ikincisi ham `string` gönderir. İki isim iki rolü ayırır: **`Convert` her zaman bir path'e aittir, `MultiConvert` her zaman zinciri kapatır.**

### Converter olmadan biçimlendirme

Birleştirme sadece biçimlendirmeden ibaretse `MultiStringFormat` yeterlidir:

```csharp
new Label()
    .Text(e => e
        .Path(nameof(Person.FirstName))
        .Path(nameof(Person.LastName))
        .MultiStringFormat("{0} {1}"))
```

## Derlenmiş (compiled) multi binding

`.Getter(...)` derlenmiş bir alt binding açar: reflection yok, string path yok ve özellik adlarını derleyici denetler. Alt binding'ler farklı tipler üretebilir:

```csharp
new Label()
    .Text(e => e
        .Getter(static (PersonViewModel vm) => vm.FirstName)
        .Getter(static (PersonViewModel vm) => vm.Age)
        .MultiConvert((string name, int age) => $"{name} ({age})"))
```

Derlenmiş ve string tabanlı alt binding'ler aynı multi binding içinde serbestçe karıştırılabilir; `.Setter(...)` de kendisinden önce gelen derlenmiş alt binding'in ters işlemini sağlamaya devam eder. Getter ifadesinin uyması gereken kurallar için bkz. [Compiled Bindings](compiled-bindings.md).

## Two-way multi binding

`MultiConvertBack`, elemanları tanımlanma sırasıyla kaynağa yazılan bir tuple döndürür. Her eleman, varsa kendi alt binding'inin `ConvertBack()`'inden de geçer.

```csharp
new Entry()
    .Text(e => e
        .Path(nameof(Person.FirstName))
        .Path(nameof(Person.LastName))
        .MultiMode(BindingMode.TwoWay)
        .MultiConvert((string first, string last) => $"{first} {last}")
        .MultiConvertBack((string full) =>
        {
            var parts = full.Split(' ');
            return (parts[0], parts.Length > 1 ? parts[1] : string.Empty);
        }))
```

`MultiMode()` multi binding'in kendi modunu belirler. Tek bir alt binding bunu kendi `.BindingMode()`'u ile ezebilir; two-way bir multi binding içinde bir kaynağı salt okunur tutmanın yolu budur.

## Boolean toplayıcılar

"Şunların hepsi doğruysa etkinleştir" gibi yaygın durumlarda hiç delege yazmanız gerekmez:

```csharp
new Button()
    .Text("Kayıt ol")
    .IsEnabled(e => e
        .Path(nameof(SignUpViewModel.AcceptedTerms))
        .Path(nameof(SignUpViewModel.AcceptedPrivacy))
        .Path(nameof(SignUpViewModel.IsEmailVerified))
        .MultiAll())
```

| Metot | Şu durumda `true` |
|---|---|
| `.MultiAll()` | tüm alt binding'ler `true` |
| `.MultiAny()` | en az bir alt binding `true` |
| `.MultiNone()` | hiçbir alt binding `true` değil |
| `.MultiAtLeast(n)` | en az `n` alt binding `true` |
| `.MultiExactly(n)` | tam olarak `n` alt binding `true` |

`bool` tipinde bir özellik için geçerlidirler ve her alt binding'in `bool` üretmesini beklerler — doğrudan ya da o alt binding'in kendi `.Convert()`'i üzerinden:

```csharp
new Button()
    .IsEnabled(e => e
        .Path(nameof(Entry.Text)).Source(nameEntry).Convert((string s) => !string.IsNullOrWhiteSpace(s))
        .Path(nameof(Entry.Text)).Source(mailEntry).Convert((string s) => !string.IsNullOrWhiteSpace(s))
        .MultiAll())
```

Toplayıcılar herhangi bir sayıda alt binding ile çalıştığı için sayı doğrulanmaz: iki alt binding üzerinde `MultiAtLeast(3)` her zaman `false` verir.

## Sayısı önceden belli olmayan alt binding'ler

Alt binding'ler bir döngüde üretiliyorsa ve sayıları önceden bilinmiyorsa `MultiConvertRaw` kullanın. Tipli biçim, her değeri tek bir tipe açar ve uyuşmazlığı `MultiConvert` ile aynı şekilde raporlar:

```csharp
new Label()
    .Text(e => e
        .Path("Basket.Food").Convert((decimal v) => (double)v)
        .Path("Basket.Drinks").Convert((decimal v) => (double)v)
        .Path("Basket.Delivery").Convert((decimal v) => (double)v)
        .MultiConvertRaw<double>(values => values.Sum().ToString("C")))
```

Tipsiz biçim size ham `object?[]` dizisini tanımlanma sırasıyla verir; dönüşümler sizin sorumluluğunuzdadır:

```csharp
.MultiConvertRaw(
    values => Describe(values),
    value => Split(value))
```

## Multi binding metotları

| Metot | Açıklama |
|---|---|
| `.MultiConvert(...)` | 2–9 alt binding değerini hedef özelliğin değerine dönüştürür. |
| `.MultiConvertBack(...)` | `MultiConvert`'in tersi; tanımlanma sırasıyla bir tuple döndürür. |
| `.MultiConvertRaw<Q>(...)` / `.MultiConvertRaw(...)` | Sayısı değişken alt binding'ler; tipli veya ham. |
| `.MultiStringFormat(string)` | Converter yerine konumsal biçimlendirme (`{0}`, `{1}`, …). |
| `.MultiConverter(IMultiValueConverter)` | Kendi multi value converter'ınız. |
| `.MultiParameter(object)` | Multi binding'in `ConverterParameter`'ı. |
| `.MultiMode(BindingMode)` | Multi binding'in modu. |
| `.MultiFallbackValue(object)` / `.MultiTargetNullValue(object)` | Tekil binding'lerdeki gibi, multi binding seviyesinde. |
| `.MultiAll()` / `.MultiAny()` / `.MultiNone()` / `.MultiAtLeast(n)` / `.MultiExactly(n)` | Boolean toplayıcılar. |

## Değerler henüz gelmemişken

Multi binding, alt binding'lerden ilki çözülür çözülmez değerlendirilir; bu sırada diğer yuvalar hâlâ boş olabilir. Bu durumda hedef özellik `null` ile ezilmek yerine mevcut değerini korur: herhangi bir alt binding çözülmemişken ya da kaynak `null` iken ve karşılık gelen delege parametresi nullable olmayan bir value type iken güncelleme atlanır. Parametrenin kabul ettiği bir `null` (`string`, nullable value type, herhangi bir referans tipi) her zamanki gibi iletilir.

Bir delege parametresi, binding'in hiç üretmediği bir tiple tanımlanmışsa fırlatılan hata; MAUI binding altyapısının içinde bir yerde patlamak yerine özelliği, sorunlu path'i ve değerin sırasını isimleriyle bildirir.

## Kendi `IMultiValueConverter`'ınızı kullanmak

```csharp
new CheckBox()
    .IsChecked(e => e
        .Path("Employee.IsOver16")
        .Path("Employee.HasPassedTest")
        .Path("Employee.IsSuspended").Convert((bool suspended) => !suspended)
        .MultiConverter(new AllTrueMultiConverter())
        .MultiFallbackValue(false))
```

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

`BindableProperty.UnsetValue` döndürmek binding'i `MultiFallbackValue`'ya düşürür.

## Hazır `BindingBase` nesneleri

Elinizde nesne olarak bulunan alt binding'ler — `Binding.Create` ile oluşturulan derlenmiş binding'ler dâhil — `.Bindings(...)` ile eklenebilir:

```csharp
new CheckBox()
    .IsChecked(e => e
        .Bindings(
            Binding.Create(static (MainPageViewModel vm) => vm.IsOver16),
            Binding.Create(static (MainPageViewModel vm) => vm.HasPassedTest))
        .MultiConvert((bool over16, bool passed) => over16 && passed))
```

Zincire `.Bindings(...)` ile başlamak, hazır alt binding'lere ayrılmış bir builder açar. Buradaki `.Converter(IMultiValueConverter)`, `.Parameter()`, `.StringFormat()`, `.BindingMode()`, `.FallbackValue()` ve `.TargetNullValue()` doğrudan multi binding'e uygulanır; dolayısıyla önceki sürümlere göre yazılmış kod değişmeden çalışmaya devam eder:

```csharp
new CheckBox()
    .IsChecked(e => e
        .Bindings(
            new Binding().Path("Employee.IsOver16"),
            new Binding().Path("Employee.HasPassedTest"))
        .Converter(new AllTrueMultiConverter())
        .FallbackValue(false))
```

`MultiConvert`, `MultiConvertBack`, `MultiConvertRaw` ve boolean toplayıcılar burada da kullanılabilir; yani converter sınıfı yazmak hiçbir zaman zorunlu değildir. Yeni kodda `.Path()` / `.Getter()` tercih edin: değerler tipli kalır ve her kaynak tek tek şekillendirilebilir.

## `Bind()` ile tipli multi binding

Alt seviyedeki `Bind()` extension'ının 2, 3 veya 4 kaynak için, değerleri tuple olarak veren tipli overload'ları da vardır:

```csharp
new Label()
    .Bind<Label, string, string, string>(Label.TextProperty,
        new Binding("FirstName"),
        new Binding("LastName"),
        convert: n => $"{n.Item1} {n.Item2}")
```

`converterParameter` ve two-way senaryolar için `convertBack` alan varyantları da mevcuttur.

## Ne zaman MultiBinding kullanmalı?

- Hedef özellik gerçekten **bağımsız olarak değişen birden çok kaynağa** bağlıysa.
- View model'e hesaplanmış bir özellik ekleyemiyorsanız (ya da eklemek istemiyorsanız).

Aksi hâlde `PropertyChanged` tetikleyen hesaplanmış bir view model özelliği tercih edin — test etmesi ve hata ayıklaması binding tesisatından kolaydır.

## Önceki sürümlerden geçiş

Multi binding API'si 10.2.0 ile geldi; tekil binding'ler tamamen eskisi gibi davranır. Dört çağrı biçimi değişti:

| Önce | Şimdi |
|---|---|
| `.Convert<double>(v => v > 10)` | `.Convert((double v) => v > 10)` — ya da `.Convert<double, bool>(...)` |
| `.ConvertBack<int>(v => …)` | `.ConvertBack((string v) => …)` — ya da `.ConvertBack<string, int>(...)` |
| `.Getter<PersonViewModel>(vm => vm.Name)` | `.Getter(static (PersonViewModel vm) => vm.Name)` |
| `.Setter<PersonViewModel>((vm, v) => vm.Name = v)` | `.Setter(static (PersonViewModel vm, string v) => vm.Name = v)` |

Dokümantasyon boyunca kullanılan, tiplerin çıkarsandığı biçim hiç değişmedi; yalnızca açıkça yazılmış tip
argümanlarının kaldırılması ya da tamamlanması gerekiyor, çünkü bu metotların her biri artık ürettiği değer
için ikinci bir tip parametresi taşıyor.

İki davranış da değişti; ikisi de daha önce sessizce yanlış çalışan durumlardı:

- `.Path()`'i iki kez çağırmak eskiden yalnızca son path'i tutuyordu. Artık iki alt binding açar ve
  birleştirme metodu olmayan bir multi binding, kurulurken bunu bildirir.
- Aynı alt binding'de `.Converter(...)` ile `.Convert(...)`'ı birlikte kullanmak eskiden birini diğerinin
  üzerine sessizce yazıyordu; artık hata fırlatır.

## İlgili Konular

- [Property Bindings](data-binding.md)
- [Binding Converters](binding-converters.md)
- [Compiled Bindings](compiled-bindings.md)
