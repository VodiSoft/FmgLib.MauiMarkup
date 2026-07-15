# Attached Property'ler

Attached property'ler bir tipte tanımlanıp başka tiplerin örneklerinde ayarlanan özelliklerdir — `Grid.Row`, `Shell.TitleColor`, `SemanticProperties.Hint` vb. FmgLib.MauiMarkup her birini normal özellik gibi okunan bir fluent metoda eşler.

## Hızlı Örnek

```csharp
// XAML:  <Border AbsoluteLayout.LayoutBounds="100,100,200,200" />
new Border().AbsoluteLayoutBounds(new Rect(100, 100, 200, 200));

// Rect kurmadan kolaylık overload'u:
new Border().AbsoluteLayoutBounds(100, 100, 200, 200);
```

## Tam Eşleme Tablosu

### Grid

| MAUI attached property | Fluent metot |
|-|-|
| `Grid.Column` | `Column()` |
| `Grid.Row` | `Row()` |
| `Grid.ColumnSpan` | `ColumnSpan()` |
| `Grid.RowSpan` | `RowSpan()` |
| `Grid.ColumnSpan` + `Grid.RowSpan` | `GridSpan(column, row)` |

```csharp
new Image().Row(0).Column(1).RowSpan(2)
```

### AbsoluteLayout

| MAUI attached property | Fluent metot |
|-|-|
| `AbsoluteLayout.LayoutFlags` | `AbsoluteLayoutFlags()` |
| `AbsoluteLayout.LayoutBounds` | `AbsoluteLayoutBounds()` |

```csharp
new AbsoluteLayout().Children(
    new BoxView()
        .Color(Colors.Teal)
        .AbsoluteLayoutFlags(AbsoluteLayoutFlags.PositionProportional)
        .AbsoluteLayoutBounds(new Rect(0.5, 0.5, 100, 100))   // ortalı, 100×100
)
```

### BindableLayout (herhangi bir layout'ta)

| MAUI attached property | Fluent metot |
|-|-|
| `BindableLayout.ItemsSource` | `BindableLayoutItemsSource()` |
| `BindableLayout.ItemTemplate` | `BindableLayoutItemTemplate()` |
| `BindableLayout.TemplateSelector` | `BindableItemTemplateSelector()` |
| `BindableLayout.EmptyView` | `BindableLayoutEmptyView()` |
| `BindableLayout.EmptyViewTemplate` | `BindableLayoutEmptyViewTemplate()` |

Tam örnekler için bkz. [Koleksiyonlar ve Şablonlar](collections-and-templates.md).

### RadioButton grupları

| MAUI attached property | Fluent metot |
|-|-|
| `RadioButtonGroup.GroupName` | `RadioButtonGroupGroupName()` |
| `RadioButtonGroup.SelectedValue` | `RadioButtonGroupSelectedValue()` |

```csharp
new VerticalStackLayout()
    .RadioButtonGroupGroupName("Sizes")
    .RadioButtonGroupSelectedValue(e => e.Path("SelectedSize").BindingMode(BindingMode.TwoWay))
    .Children(
        new RadioButton().Content("S").Value("S"),
        new RadioButton().Content("M").Value("M"),
        new RadioButton().Content("L").Value("L")
    )
```

### Flyout / bağlam menüleri

| MAUI attached property | Fluent metot |
|-|-|
| `FlyoutBase.ContextFlyout` | `ContextFlyout()` |

Bkz. [Menüler](menus.md).

### VisualStateManager

| MAUI attached property | Fluent metot |
|-|-|
| `VisualStateManager.VisualStateGroups` | `VisualStateGroups()` |

Bkz. [Visual State'ler](visual-states.md).

### Shell (sayfalarda/öğelerde ayarlanır)

| MAUI attached property | Fluent metot |
|-|-|
| `Shell.PresentationMode` | `ShellPresentationMode()` |
| `Shell.BackgroundColor` | `ShellBackgroundColor()` |
| `Shell.ForegroundColor` | `ShellForegroundColor()` |
| `Shell.TitleColor` | `ShellTitleColor()` |
| `Shell.DisabledColor` | `ShellDisabledColor()` |
| `Shell.UnselectedColor` | `ShellUnselectedColor()` |
| `Shell.NavBarHasShadow` | `ShellNavBarHasShadow()` |
| `Shell.NavBarIsVisible` | `ShellNavBarIsVisible()` |
| `Shell.TitleView` | `ShellTitleView()` |
| `Shell.TabBarBackgroundColor` | `ShellTabBarBackgroundColor()` |
| `Shell.TabBarForegroundColor` | `ShellTabBarForegroundColor()` |
| `Shell.TabBarTitleColor` | `ShellTabBarTitleColor()` |
| `Shell.TabBarDisabledColor` | `ShellTabBarDisabledColor()` |
| `Shell.TabBarUnselectedColor` | `ShellTabBarUnselectedColor()` |
| `Shell.TabBarIsVisible` | `ShellTabBarIsVisible()` |
| `Shell.FlyoutBackdrop` | `ShellFlyoutBackdrop()` |
| `Shell.FlyoutBehavior` | `ShellFlyoutBehavior()` |
| `Shell.FlyoutHeight` | `ShellFlyoutHeight()` |
| `Shell.FlyoutWidth` | `ShellFlyoutWidth()` |
| `Shell.FlyoutItemIsVisible` | `ShellFlyoutItemIsVisible()` |
| `Shell.BackButtonBehavior` | `ShellBackButtonBehavior()` |
| `Shell.ItemTemplate` | `ShellItemTemplate()` |
| `Shell.MenuItemTemplate` | `ShellMenuItemTemplate()` |
| `Shell.SearchHandler` | `ShellSearchHandler()` |

Örnek — giriş sayfasında nav bar ve tab bar'ı gizle:

```csharp
public partial class LoginPage : ContentPage, IFmgLibHotReload
{
    public LoginPage() => this.InitializeHotReload();

    public void Build() =>
        this
        .ShellNavBarIsVisible(false)
        .ShellTabBarIsVisible(false)
        .Content(/* ... */);
}
```

### NavigationPage (sayfalarda ayarlanır)

| MAUI attached property | Fluent metot |
|-|-|
| `NavigationPage.HasNavigationBar` | `NavigationPageHasNavigationBar()` |
| `NavigationPage.BackButtonTitle` | `NavigationPageBackButtonTitle()` |
| `NavigationPage.HasBackButton` | `NavigationPageHasBackButton()` |
| `NavigationPage.IconColor` | `NavigationPageIconColor()` |
| `NavigationPage.TitleIconImageSource` | `NavigationPageTitleIconImageSource()` |
| `NavigationPage.TitleView` | `NavigationPageTitleView()` |

### Erişilebilirlik — Semantic ve Automation

| MAUI attached property | Fluent metot |
|-|-|
| `SemanticProperties.Hint` | `SemanticHint()` |
| `SemanticProperties.Description` | `SemanticDescription()` |
| `SemanticProperties.HeadingLevel` | `SemanticHeadingLevel()` |
| `AutomationProperties.ExcludedWithChildren` | `AutomationExcludedWithChildren()` |
| `AutomationProperties.IsInAccessibleTree` | `AutomationIsInAccessibleTree()` |
| `AutomationProperties.Name` | `AutomationName()` |
| `AutomationProperties.HelpText` | `AutomationHelpText()` |
| `AutomationProperties.LabeledBy` | `AutomationLabeledBy()` |

```csharp
new Image()
    .Source("logo.png")
    .SemanticDescription("Şirket logosu")
    .SemanticHint("Sayfanın üstündeki dekoratif görsel")

new Label()
    .Text("Ayarlar")
    .SemanticHeadingLevel(SemanticHeadingLevel.Level1)
```

### Araç ipuçları

| MAUI attached property | Fluent metot |
|-|-|
| `ToolTipProperties.Text` | `ToolTipPropertiesText()` |

```csharp
new Button().Text("?").ToolTipPropertiesText("Yardım merkezini açar")
```

## Adlandırma Kuralı

Metot adını tahmin etmenin temel kuralı:

- **Grid yerleşimi** sahip önekini atar: `Grid.Row` → `Row()`.
- Diğer her şey **sahip + özellik** birleşimidir: `Shell.TitleColor` → `ShellTitleColor()`, `SemanticProperties.Hint` → `SemanticHint()`, `AutomationProperties.Name` → `AutomationName()`.

Aynı kural `[MauiMarkupAttachedProp]` ile üretilen [üçüncü parti attached property'ler](third-party-controls.md) için de geçerlidir — örn. InputKit'in `FormView.IsSubmitButton`'ı `FormViewIsSubmitButton()` olur.

## Builder Desteği

Attached-property metotları da her özellik gibi property builder lambda'sı kabul eder; binding'ler ve tema/platform değerleri çalışır:

```csharp
new ContentPage()
    .ShellTabBarIsVisible(e => e.Path("IsTabBarVisible"))
    .ShellBackgroundColor(e => e.OnLight(Colors.White).OnDark(Colors.Black))
```

## İlgili Konular

- [Grid](grid.md)
- [Shell Uygulamaları](shell-navigation.md)
- [Üçüncü Parti Kontroller](third-party-controls.md) — dış attached property'ler için metot üretimi
