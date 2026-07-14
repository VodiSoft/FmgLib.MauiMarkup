# From XAML to C#

FmgLib.MauiMarkup provides a fluent extension method for **every property that you can set in XAML**. Once you internalize a handful of mapping rules, you can translate any XAML snippet mechanically.

## The Mapping Rules

| XAML concept | FmgLib.MauiMarkup equivalent |
|---|---|
| `<Label ... />` element | `new Label()` |
| Attribute `Text="Hi"` | `.Text("Hi")` |
| Attached property `Grid.Row="1"` | `.Row(1)` (see [Attached Properties](attached-properties.md)) |
| Property element `<Label.Content>…` | The same fluent method taking the object: `.Content(new … )` |
| Child collection (implicit content) | `.Children(...)`, `.Items(...)`, `.Content(...)` depending on the container |
| `{Binding Path=Name}` | `.Text(e => e.Path("Name"))` (see [Property Bindings](data-binding.md)) |
| `{StaticResource key}` | Just use the C# object/constant directly |
| `{DynamicResource key}` | `.BackgroundColor(e => e.DynamicResource("key"))` |
| `{OnPlatform ...}` | `.FontSize(e => e.OnAndroid(20).OniOS(24).Default(22))` |
| `{OnIdiom ...}` | `.FontSize(e => e.OnPhone(20).OnDesktop(40).Default(30))` |
| `{AppThemeBinding Light=…, Dark=…}` | `.TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.White))` |
| `x:Name="myLabel"` | `.Assign(out var myLabel)` |
| Event attribute `Clicked="OnClicked"` | `.OnClicked(OnClicked)` |
| `<Style TargetType="Button">` | `new Style<Button>(e => e …)` (see [Styling](styling.md)) |
| `<DataTemplate>` | `.ItemTemplate(() => new …)` — a lambda producing the view |

## Side-by-Side Examples

### Simple control

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
.SizeRequest(150, 100)   // convenience: sets WidthRequest + HeightRequest at once
.Opacity(.8)
```

> `SizeRequest(width, height)` is one of several *convenience shorthands* the library adds beyond 1:1 property mapping. You can of course also write `.WidthRequest(150).HeightRequest(100)`.

### Text formatting

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

`Margin` and `Padding` also have friendlier overloads:

```csharp
.Margin(10)                    // all sides
.Margin(10, 5)                 // horizontal, vertical
.Margin(5, 3, 0, 5)            // left, top, right, bottom
.Margin(left: 5, bottom: 5)    // named — unspecified sides default to 0
```

### A page with nested content

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

**C# (inside the page class)**

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

### Bindings

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

### DataTemplates

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

The lambda passed to `ItemTemplate` plays the role of the `<DataTemplate>` element: it is invoked once per realized item, and bindings inside it resolve against the item's `BindingContext`.

### Resources and styles

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

## What You Gain Over XAML

- **One language** — logic and UI live together; no code-behind/x:Name plumbing.
- **Compile-time safety** — a typo in a property name is a build error, not a runtime crash.
- **Full IntelliSense & refactoring** — rename a view model property and every "binding" written with [compiled bindings](compiled-bindings.md) follows.
- **Real control flow** — use `if`, loops, LINQ, local functions and helper methods while building UI:

  ```csharp
  new VerticalStackLayout()
  .Children(
      menuItems.Select(item =>
          (IView)new Button().Text(item.Title).OnClicked(_ => Navigate(item))
      ).ToArray()
  )
  ```

- **Composability** — extract any subtree into a plain method:

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

## Related Topics

- [Fluent Properties](fluent-properties.md) — everything property methods accept (values, bindings, per-platform builders…)
- [Attached Properties](attached-properties.md) — full XAML attached-property → method table
- [Event Handlers](event-handlers.md) — `On<Event>` method patterns
