# Attached Properties

Attached properties are defined on one type but set on instances of other types — `Grid.Row`, `Shell.TitleColor`, `SemanticProperties.Hint`, and so on. FmgLib.MauiMarkup maps each one to a fluent method so they read like normal properties.

## Quick Example

```csharp
// XAML:  <Border AbsoluteLayout.LayoutBounds="100,100,200,200" />
new Border().AbsoluteLayoutBounds(new Rect(100, 100, 200, 200));

// convenience overload without constructing a Rect:
new Border().AbsoluteLayoutBounds(100, 100, 200, 200);
```

## Complete Mapping Table

### Grid

| MAUI attached property | Fluent method |
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

| MAUI attached property | Fluent method |
|-|-|
| `AbsoluteLayout.LayoutFlags` | `AbsoluteLayoutFlags()` |
| `AbsoluteLayout.LayoutBounds` | `AbsoluteLayoutBounds()` |

```csharp
new AbsoluteLayout().Children(
    new BoxView()
        .Color(Colors.Teal)
        .AbsoluteLayoutFlags(AbsoluteLayoutFlags.PositionProportional)
        .AbsoluteLayoutBounds(new Rect(0.5, 0.5, 100, 100))   // centered, 100×100
)
```

### BindableLayout (on any layout)

| MAUI attached property | Fluent method |
|-|-|
| `BindableLayout.ItemsSource` | `BindableLayoutItemsSource()` |
| `BindableLayout.ItemTemplate` | `BindableLayoutItemTemplate()` |
| `BindableLayout.TemplateSelector` | `BindableItemTemplateSelector()` |
| `BindableLayout.EmptyView` | `BindableLayoutEmptyView()` |
| `BindableLayout.EmptyViewTemplate` | `BindableLayoutEmptyViewTemplate()` |

See [Collections & Templates](collections-and-templates.md) for full examples.

### RadioButton groups

| MAUI attached property | Fluent method |
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

### Flyout / context menus

| MAUI attached property | Fluent method |
|-|-|
| `FlyoutBase.ContextFlyout` | `ContextFlyout()` |

See [Menus](menus.md).

### VisualStateManager

| MAUI attached property | Fluent method |
|-|-|
| `VisualStateManager.VisualStateGroups` | `VisualStateGroups()` |

See [Visual States](visual-states.md).

### Shell (set on pages/items)

| MAUI attached property | Fluent method |
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

Example — hide the nav bar and tab bar on a login page:

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

### NavigationPage (set on pages)

| MAUI attached property | Fluent method |
|-|-|
| `NavigationPage.HasNavigationBar` | `NavigationPageHasNavigationBar()` |
| `NavigationPage.BackButtonTitle` | `NavigationPageBackButtonTitle()` |
| `NavigationPage.HasBackButton` | `NavigationPageHasBackButton()` |
| `NavigationPage.IconColor` | `NavigationPageIconColor()` |
| `NavigationPage.TitleIconImageSource` | `NavigationPageTitleIconImageSource()` |
| `NavigationPage.TitleView` | `NavigationPageTitleView()` |

### Accessibility — Semantic & Automation

| MAUI attached property | Fluent method |
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
    .SemanticDescription("Company logo")
    .SemanticHint("Decorative image at the top of the page")

new Label()
    .Text("Settings")
    .SemanticHeadingLevel(SemanticHeadingLevel.Level1)
```

### Tooltips

| MAUI attached property | Fluent method |
|-|-|
| `ToolTipProperties.Text` | `ToolTipPropertiesText()` |

```csharp
new Button().Text("?").ToolTipPropertiesText("Opens the help center")
```

## Naming Convention

The rule of thumb for guessing a method name:

- **Grid placement** drops the owner prefix: `Grid.Row` → `Row()`.
- Everything else concatenates **owner + property**: `Shell.TitleColor` → `ShellTitleColor()`, `SemanticProperties.Hint` → `SemanticHint()`, `AutomationProperties.Name` → `AutomationName()`.

The same convention is used for [third-party attached properties](third-party-controls.md) generated by `[MauiMarkupAttachedProp]` — e.g. InputKit's `FormView.IsSubmitButton` becomes `FormViewIsSubmitButton()`.

## Builder Support

Attached-property methods accept the property builder lambda like any other property, so bindings and theme/platform values work:

```csharp
new ContentPage()
    .ShellTabBarIsVisible(e => e.Path("IsTabBarVisible"))
    .ShellBackgroundColor(e => e.OnLight(Colors.White).OnDark(Colors.Black))
```

## Related Topics

- [Grid](grid.md)
- [Shell Applications](shell-navigation.md)
- [Third-Party Controls](third-party-controls.md) — generating methods for external attached properties
