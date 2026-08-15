---
title: AI Skills
description: Yapay zekâ ajanlarına doğru FmgLib.MauiMarkup kodu yazmayı öğreten, kurulabilir on skill paketi.
badge: Yeni
---

# AI Skills

Herhangi bir yapay zekâ ajanından "C# markup ile bir MAUI sayfası yaz" isteyin; kulağa doğru gelen ama
derlenmeyen kod alırsınız: uydurulmuş metot adları, birebir taşınmış XAML alışkanlıkları, `Build()`
içinde oluşturulan view model'ler, `ContentTemplate(() => page)` yerine `ContentTemplate(page)`.

Sorun modelde değil: kütüphanenin kuralları basit ama tahmin edilebilir değil. `Foo` özelliği
`.Foo(...)` olur. `Bar` olayı `.OnBar(...)` olur. `Grid.Row` önekini düşürür ama `Shell.TitleColor`
düşürmez. `Build()` her hot reload'da yeniden çalışır, dolayısıyla durum (state) alanlarda yaşamalıdır.
Bunların hiçbiri yalnızca tip sisteminden çıkarılamaz.

**AI Skills, bu kuralları doğrudan anlatan on Markdown paketidir.** Bir kez kurun; ajanınız tahmin
etmeyi bıraksın.

```csharp
// skill'ler olmadan bir ajanın yazdığı
new Label().SetText("Merhaba").SetFontSize(30).HorizontalAlign("Center")   // hiçbiri yok

// skill'lerle yazdığı
new Label().Text("Merhaba").FontSize(30).CenterHorizontal()
```

[Agent Skills](https://code.claude.com/docs/en/skills) biçimini kullanırlar — YAML başlıklı düz
Markdown — bu yüzden Claude Code, Claude uygulamaları, Agent SDK ve `SKILL.md` okuyabilen her ajanla
çalışırlar. Tamamı MIT lisanslıdır ve kütüphaneyle birlikte sürümlenir; böylece bir API değişikliği ile
skill güncellemesi aynı commit'te gider.

## Kurulum

### Otomatik

Ajanınıza şunu söyleyin:

> https://mauimarkup.fmglib.dev/llms.txt adresini getir ve FmgLib.MauiMarkup AI skill'lerini kur.

Bu sayfayı bulacak, aşağıdaki katalogdan okuyacak ve her skill'i yerine indirecektir.

### Elle

Her skill, içinde `SKILL.md` bulunan bir klasördür. İki yerden birine koyun:

| Kapsam | Konum |
|---|---|
| Kişisel — her projede geçerli | `~/.claude/skills/<skill-adı>/SKILL.md` |
| Proje — depoyla birlikte commit'lenir, ekiple paylaşılır | `<repo>/.claude/skills/<skill-adı>/SKILL.md` |

Çekirdek skill ayrıca bir `references/` klasörü içerir; iç bağlantılarının çözülebilmesi için göreli
yolları koruyun.

```bash
# çekirdek skill, doğrudan GitHub'dan
mkdir -p ~/.claude/skills/mauimarkup/references
BASE=https://raw.githubusercontent.com/VodiSoft/FmgLib.MauiMarkup/master/skills/mauimarkup
curl -fsSL $BASE/SKILL.md -o ~/.claude/skills/mauimarkup/SKILL.md
for f in cheatsheet bindings layout styling-theming pitfalls; do
  curl -fsSL $BASE/references/$f.md -o ~/.claude/skills/mauimarkup/references/$f.md
done
```

Diğer tüm skill'ler tek dosyadır:

```bash
NAME=mauimarkup-mvvm     # katalogdaki herhangi bir ad
mkdir -p ~/.claude/skills/$NAME
curl -fsSL https://raw.githubusercontent.com/VodiSoft/FmgLib.MauiMarkup/master/skills/$NAME/SKILL.md \
     -o ~/.claude/skills/$NAME/SKILL.md
```

Ekipler için önerilen yol klasörü `<repo>/.claude/skills/` altına commit'lemektir: böylece her
geliştirici ve her CI ajanı aynı talimatlarla çalışır.

## On skill

Tümünün kaynağı:
[`skills/`](https://github.com/VodiSoft/FmgLib.MauiMarkup/tree/master/skills) ·
ham (raw) taban adresi
`https://raw.githubusercontent.com/VodiSoft/FmgLib.MauiMarkup/master/skills/<ad>/SKILL.md`

| Skill | Öğrettiği |
|---|---|
| **`mauimarkup`** *(zorunlu)* | Fluent model, sayfa iskeleti, dört özellik overload'ı, yerleşim, olaylar, `Assign`/`InvokeOnElement`, metot adı türetme, `Build()` disiplini. Beş dosyalık `references/` paketi ile gelir: API özeti, binding'ler, yerleşim tabloları, stil ve tema, tuzaklar |
| `mauimarkup-xaml-migration` | Sayfa bazlı taşıma prosedürü, 30 satırlık XAML→C# eşleme tablosu ve çeviri değil muhakeme gerektiren yapılar — `x:Reference` sırası, `StaticResource`, converter'lar, `RelativeSource` |
| `mauimarkup-mvvm` | `FmgLibContentPage<TViewModel>`, tipli `BindingContext`, derlenmiş `Getter`/`Setter` binding'leri, komutlar, bağımlılık enjeksiyonu, CommunityToolkit.Mvvm |
| `mauimarkup-shell` | Shell, `FlyoutItem`, `Tab`, `TabBar`, `ContentTemplate` lambda'ları, flyout şablonları, sayfa bazlı Shell attached property'leri, rotalar, pencereler, menü çubukları |
| `mauimarkup-collections` | `ItemsSource`/`ItemTemplate`, şablon seçiciler, item layout'ları, `EmptyView`, sonsuz kaydırma, aşağı çekip yenileme, `BindableLayout` ve *ne zaman kullanılmayacağı* |
| `mauimarkup-styling` | `Style<T>`, kaynak organizasyonu, `AppThemeBinding` ile koyu tema, visual state'ler, trigger'lar, gradyanlar, gölgeler, `Animate…To` |
| `mauimarkup-localization` | JSON ve RESX kurulumu, `Translate`/`TranslateFormat`, canlı dil değişimi, fallback zincirleri, eksik anahtar politikaları, RTL |
| `mauimarkup-thirdparty` | `[MauiMarkup]`, `[MauiMarkupAttachedProp]`, otomatik generator modu, taban sınıf üretimi, `New` son eki kuralı, bilinçli olarak atlanan üyeler |
| `mauimarkup-hotreload` | `IFmgLibHotReload`, handler seçenekleri, `dotnet watch` ve IDE kanalları, reload'a dayanıklı `Build()`, tam sorun giderme matrisi |
| `mauimarkup-review` | Hazır ripgrep sorgularıyla dokuz denetim geçişi, önem derecesi modeli ve raporlama biçimi |

Onunu birden kurmak sorun değil — ajan, bir skill'in gövdesini yalnızca görev tanımına uyduğunda okur;
kullanılmayan skill'lerin maliyeti yoktur.

### Önerilen setler

| Durumunuz | Kurun |
|---|---|
| Yeni uygulamaya başlıyorum | `mauimarkup` + `mauimarkup-shell` + `mauimarkup-mvvm` + `mauimarkup-hotreload` |
| Mevcut XAML uygulamasını taşıyorum | `mauimarkup` + `mauimarkup-xaml-migration` + `mauimarkup-styling` |
| Veri yoğun uygulama geliştiriyorum | `mauimarkup` + `mauimarkup-mvvm` + `mauimarkup-collections` |
| Birden çok pazara çıkıyorum | `mauimarkup-localization` ekleyin |
| Syncfusion / UraniumUI / SkiaSharp / ZXing kullanıyorum | `mauimarkup-thirdparty` ekleyin |
| Devraldığım kod tabanını temizliyorum | `mauimarkup` + `mauimarkup-review` |

## Pratikte ne değişiyor

Skill'lerin kayıt altına aldığı düzeltmelerden birkaçı — her biri ajanların skill'siz olarak düzenli
şekilde yaptığı hatalar:

| Skill'siz | Skill'li |
|---|---|
| `.SetText()`, `.HorizontalAlign()` uydurur | Özellik adından `.Text()`, `.CenterHorizontal()` türetir |
| `.xaml` + `.xaml.cs` çifti üretir | Tek `.cs` dosyası, `InitializeComponent()` yok |
| `Build()` içinde `new MyViewModel()` | View model constructor alanında — durum hot reload'ı atlatır |
| `.ContentTemplate(new HomePage())` | `.ContentTemplate(() => new HomePage())` |
| `.TextColor(isDark ? white : black)` | `.TextColor(e => e.OnLight(black).OnDark(white))` — canlı tema binding'i |
| Her yerde `e.Path("UserName")` | `e.Getter(static (VM vm) => vm.UserName)` — derleme zamanında denetlenir |
| 5000 öğelik liste için `BindableLayout` | `CollectionView`, çünkü yalnızca o sanallaştırma yapar |
| `builder.UseFmgLibMauiMarkup()` ekler | Böyle bir kayıt çağrısı olmadığını bilir |
| Syncfusion kontrolü için elle extension yazar | `[MauiMarkup(typeof(SfButton))]` yazıp işi generator'a bırakır |

## Kurulumu doğrulama

Skill'lerin netleştirdiği bir şey isteyin:

> MauiMarkup ile bir giriş sayfası yaz: e-posta alanı, parola alanı ve ikisi de doldurulana kadar pasif
> kalan bir gönder butonu.

Doğru yanıt; `IFmgLibHotReload` uygulayan, ağacı `Build()` içinde kuran, alanları `.Assign(out var …)`
ile yakalayan ve hiç XAML içermeyen tek bir `.cs` dosyasıdır.

## Güncel tutmak

Skill'ler kütüphane deposunda yaşar; yani anlattıkları kodla birlikte gözden geçirilirler. Bir skill,
kütüphanenin artık yapmadığı bir şeyi öğretiyorsa lütfen
[konu açın](https://github.com/VodiSoft/FmgLib.MauiMarkup/issues) — eski bilgiyi kendinden emin şekilde
tekrarlayan bir ajan, hiç skill'i olmayandan daha kötüdür.

## İlgili Konular

- [Başlarken](getting-started.md) — skill'lerin anlattığı kütüphaneyi kurun
- [XAML'den C#'a](xaml-to-csharp.md) — taşıma skill'inin insan sürümü
- [İpuçları ve Sorun Giderme](tips-and-troubleshooting.md) — tuzaklar referansının insan sürümü
