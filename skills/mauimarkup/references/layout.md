# FmgLib.MauiMarkup — Layout Reference

## Grid definitions

`RowDefinitions` / `ColumnDefinitions` take a builder with three methods, each with an optional
`count:` that repeats the definition:

| Builder method | XAML |
|---|---|
| `Star(double value = 1.0, int count = 1)` | `*`, `2*`, `0.5*` |
| `Auto(int count = 1)` | `Auto` |
| `Absolute(double value, int count = 1)` | `100` |

```csharp
new Grid()
.RowDefinitions(e => e.Star(2).Star(0.5, count: 3))   // == RowDefinitions="2*,0.5*,0.5*,0.5*"
.ColumnDefinitions(e => e.Absolute(100).Star())        // == ColumnDefinitions="100,*"
```

`e => e.Star(1, count: 7)` builds a seven-row calendar strip in one call. Collection overloads also
exist: `.ColumnDefinitions(new ColumnDefinitionCollection().Absolute(100).Star())`.

Notes:
- `Auto` rows measure their content on every pass — fine for headers/footers, wrong for long lists.
  Put a `CollectionView` in a `Star` row instead.
- Children in the same cell stack in declaration order; `.ZIndex(int)` overrides that.
- A `Grid` with no definitions is a single implicit cell — the idiomatic overlay container.

## Placement

| Method | XAML |
|---|---|
| `.Row(int)` | `Grid.Row` |
| `.Column(int)` | `Grid.Column` |
| `.RowSpan(int)` | `Grid.RowSpan` |
| `.ColumnSpan(int)` | `Grid.ColumnSpan` |
| `.GridSpan(column, row)` | both spans |

Row and column default to 0.

## Layout options — position inside the parent

Single axis:

| Method | Sets |
|---|---|
| `CenterHorizontal()` | `HorizontalOptions = Center` |
| `CenterVertical()` | `VerticalOptions = Center` |
| `Center()` | both `Center` |
| `AlignLeft()` / `AlignRight()` | `HorizontalOptions = Start` / `End` |
| `AlignTop()` / `AlignBottom()` | `VerticalOptions = Start` / `End` |
| `FillHorizontal()` / `FillVertical()` | `Fill` on that axis |
| `FillBothDirections()` | both `Fill` |

Two axes — `Align{Vertical}{Horizontal}()`:

| Method | Vertical | Horizontal |
|---|---|---|
| `AlignTopLeft()` | Start | Start |
| `AlignTopCenter()` | Start | Center |
| `AlignTopRight()` | Start | End |
| `AlignTopFill()` | Start | Fill |
| `AlignCenterLeft()` | Center | Start |
| `AlignCenterRight()` | Center | End |
| `AlignCenterFill()` | Center | Fill |
| `AlignBottomLeft()` | End | Start |
| `AlignBottomCenter()` | End | Center |
| `AlignBottomRight()` | End | End |
| `AlignBottomFill()` | End | Fill |
| `AlignFillLeft()` | Fill | Start |
| `AlignFillCenter()` | Fill | Center |
| `AlignFillRight()` | Fill | End |

General form for computed values:
`.AlignLayout(vertical: LayoutOptions.End, horizontal: LayoutOptions.Center)`.
The raw `.HorizontalOptions(...)` / `.VerticalOptions(...)` always work too.

`Expand` options are obsolete in MAUI and deliberately absent — use `Grid` star sizing or `FlexLayout`
grow factors.

## Text alignment — position inside the control

`TextCenter()`, `TextTopLeft()`, `TextBottomRight()`, … set `HorizontalTextAlignment` /
`VerticalTextAlignment` on any `ITextAlignment`.

**The classic confusion:** `Center()` centers the *label in its parent*; `TextCenter()` centers the
*text in the label*. A full-width, center-aligned title needs both:
`new Label().Text("Title").FillHorizontal().TextCenter()`.

Grid children default to `Fill` in their cell — use the alignment helpers to position within a cell.

## FlexLayout per-item properties

Available on any `View`, including templated ones:

```csharp
new Frame()
    .FlexBasis(FlexBasis.Auto)
    .FlexGrow(1)
    .FlexShrink(0)
    .FlexAlignSelf(FlexAlignSelf.Center)
    .FlexOrder(2)
```

Container side: `.Direction(...)`, `.Wrap(FlexWrap.Wrap)`, `.JustifyContent(FlexJustify.Start)`,
`.AlignItems(...)`, `.AlignContent(...)`.

## Attached properties — complete map

Naming rule: **owner + property**, concatenated. Grid placement is the single exception (prefix
dropped).

### AbsoluteLayout

| MAUI | Fluent |
|---|---|
| `AbsoluteLayout.LayoutFlags` | `AbsoluteLayoutFlags()` |
| `AbsoluteLayout.LayoutBounds` | `AbsoluteLayoutBounds()` — also `(x, y, w, h)` |

### BindableLayout (on any layout)

| MAUI | Fluent |
|---|---|
| `BindableLayout.ItemsSource` | `BindableLayoutItemsSource()` |
| `BindableLayout.ItemTemplate` | `BindableLayoutItemTemplate()` |
| `BindableLayout.TemplateSelector` | `BindableItemTemplateSelector()` |
| `BindableLayout.EmptyView` | `BindableLayoutEmptyView()` |
| `BindableLayout.EmptyViewTemplate` | `BindableLayoutEmptyViewTemplate()` |

### RadioButtonGroup

| MAUI | Fluent |
|---|---|
| `RadioButtonGroup.GroupName` | `RadioButtonGroupGroupName()` |
| `RadioButtonGroup.SelectedValue` | `RadioButtonGroupSelectedValue()` |

### Shell (set on pages and shell items)

`ShellPresentationMode`, `ShellBackgroundColor`, `ShellForegroundColor`, `ShellTitleColor`,
`ShellDisabledColor`, `ShellUnselectedColor`, `ShellNavBarHasShadow`, `ShellNavBarIsVisible`,
`ShellTitleView`, `ShellTabBarBackgroundColor`, `ShellTabBarForegroundColor`, `ShellTabBarTitleColor`,
`ShellTabBarDisabledColor`, `ShellTabBarUnselectedColor`, `ShellTabBarIsVisible`, `ShellFlyoutBackdrop`,
`ShellFlyoutBehavior`, `ShellFlyoutHeight`, `ShellFlyoutWidth`, `ShellFlyoutItemIsVisible`,
`ShellBackButtonBehavior`, `ShellItemTemplate`, `ShellMenuItemTemplate`, `ShellSearchHandler`.

### NavigationPage (set on pages)

`NavigationPageHasNavigationBar`, `NavigationPageBackButtonTitle`, `NavigationPageHasBackButton`,
`NavigationPageIconColor`, `NavigationPageTitleIconImageSource`, `NavigationPageTitleView`.

### Accessibility

| MAUI | Fluent |
|---|---|
| `SemanticProperties.Hint` | `SemanticHint()` |
| `SemanticProperties.Description` | `SemanticDescription()` |
| `SemanticProperties.HeadingLevel` | `SemanticHeadingLevel()` |
| `AutomationProperties.Name` | `AutomationName()` |
| `AutomationProperties.HelpText` | `AutomationHelpText()` |
| `AutomationProperties.LabeledBy` | `AutomationLabeledBy()` |
| `AutomationProperties.IsInAccessibleTree` | `AutomationIsInAccessibleTree()` |
| `AutomationProperties.ExcludedWithChildren` | `AutomationExcludedWithChildren()` |
| `ToolTipProperties.Text` | `ToolTipPropertiesText()` |

### Others

| MAUI | Fluent |
|---|---|
| `FlyoutBase.ContextFlyout` | `ContextFlyout()` |
| `VisualStateManager.VisualStateGroups` | `VisualStateGroups()` |

Attached-property methods accept the builder lambda like any other property:

```csharp
new ContentPage()
    .ShellTabBarIsVisible(e => e.Path("IsTabBarVisible"))
    .ShellBackgroundColor(e => e.OnLight(Colors.White).OnDark(Colors.Black))
```

## Worked example — product card

```csharp
new Grid()
.RowDefinitions(e => e.Star(1).Star(6).Star(2).Star(1))
.Padding(5)
.Children(
    new Grid().Row(0).ColumnDefinitions(e => e.Star(6).Star(4)).Children(
        new ImageButton().Source("heart.png").AlignLeft().SizeRequest(30, 30),
        new Frame().Column(1).CornerRadius(20).BackgroundColor(Colors.Red)
            .Content(new Label().Text("-50%").TextColor(Colors.White).Center())),

    new Image().Source("product.png").SizeRequest(80, 80).Row(1).CenterHorizontal(),

    new VerticalStackLayout().Row(2).Children(
        new Label().Text("Sourdough Bread").FontAttributes(FontAttributes.Bold),
        new Label().Text("$4.90").TextColor(Colors.Green)),

    new Button().Row(3).Text("Add to cart").HeightRequest(35)
)
```
