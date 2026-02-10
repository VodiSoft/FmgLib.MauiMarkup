# FmgLib.MauiMarkup

### Free .NET Libraries by FmgLib.MauiMakrup

| NuGet Package | Link | Repo Info |
|--------------|------|-------|
| `FmgLib.MauiMarkup` | [![NuGet](https://img.shields.io/nuget/v/FmgLib.MauiMarkup?includePreReleases=true)](https://www.nuget.org/packages/FmgLib.MauiMarkup/) ![NuGet Downloads](https://img.shields.io/nuget/dt/FmgLib.MauiMarkup.svg) | [![GitHub Stars](https://img.shields.io/github/stars/FmgLib/FmgLib.MauiMarkup?style=flat-square&color=blue)](https://github.com/FmgLib/FmgLib.MauiMarkup/stargazers) [![GitHub Forks](https://img.shields.io/github/forks/FmgLib/FmgLib.MauiMarkup?style=flat-square&color=green)](https://github.com/FmgLib/FmgLib.MauiMarkup/forks) [![GitHub last-commit](https://img.shields.io/github/last-commit/FmgLib/FmgLib.MauiMarkup?style=flat-square)](https://github.com/FmgLib/FmgLib.MauiMarkup/commits) |
| `FmgLib.MauiMarkup.Template` | [![NuGet](https://img.shields.io/nuget/v/FmgLib.MauiMarkup.Template?includePreReleases=true)](https://www.nuget.org/packages/FmgLib.MauiMarkup.Template/) ![NuGet Downloads](https://img.shields.io/nuget/dt/FmgLib.MauiMarkup.Template.svg) | - |

<br>

FmgLib.MauiMarkup is a specialized library crafted for .NET MAUI. This library allows you to code directly in C# without the necessity of employing XAML language. It provides developers with a straightforward and flexible approach to building user interfaces using C# code. With FmgLib.MauiMarkup, you can now develop application interfaces in a code-focused manner, avoiding the complexities of dealing with XAML files. This library accelerates your development process while enabling you to write more readable and manageable code.

FmgLib.MauiMarkup provides extension methods for all properties provided for a View on the XAML side.

<br>
<br>

<details>
    <summary><b>Getting Started</b></summary>

<br>
<br>

### Creating a new FmgLib.MauiMarkup project from CLI

FmgLib provides a project template to start a new project with FmgLib.MauiMarkup.

**Install latest templates from NuGet:**
```bash
dotnet new install FmgLib.MauiMarkup.Template
```

**Create a new project:**

```bash
dotnet new fmglib-mauimarkup-app -o my-new-project
```

Optional parameters:
- `--netMajor 10|9` (default 10) to pick the .NET/MAUI version and matching package versions.
- `--includeContent true|false` (default false) to choose between a minimal home page or a richer sample experience.

<br>

### Existing Projects

Install the [FmgLib.MauiMarkup](https://www.nuget.org/packages/FmgLib.MauiMarkup/) NuGet package to your MAUI application.
```bash
dotnet add package FmgLib.MauiMarkup
```

<br>
</details>

<details>
    <summary><b>XAML to FmgLib.MauiMarkup(C#)</b></summary>

<br>
<br>
If we were to write XAML code for the Image class, it would look like this:

```xaml
<Image
    Source="dotnet_bot.png"
    HeightRequest="100"
    WidthRequest="150"
    Grid.Row="0"
    Grid.Column="1"
    Grid.RowSpan="2"
    Opacity=".8" />
```

The C# equivalent with the help of FmgLib.MauiMarkup would be as follows:

```csharp
new Image()
.Source("dotnet_bot.png")
.Row(0)
.Column(1)
.RowSpan(2)
.SizeRequest(150,100)
.Opacity(.8)
```

Similarly, we can see this for other Views. Let's write a few sample codes as an example:

```csharp
new Label()
.Text("fmglib.mauimarkup")
.FontSize(12)
.Row(1)
.TextColor(Colors.Green)
.FontAttributes(FontAttributes.Bold)
.Margin(new Thickness(5,3,0,5))
```

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
        .InvokeOnElement(ai => ai.Loaded += CheckLogin(sender, e))
    )
);
```

<br>
</details>

<details>
    <summary><b>How to assign object references</b></summary>

<br>
<br>
There are two main ways to assign objects in `FmgLib.MauiMarkup`: 

- using the `Assign` method,

The first example uses the `Assign` method to assign a label object to a variable named label. This is done using the following code:

```csharp
new Label().Assign(out var label);
new Entry().Assign(out var entry);
```
Or

```csharp
Button btnOk;

new Button()
.Assign(out btnOk);
```

<br>
</details>

<details>
    <summary><b>Attached properties</b></summary>

<br>
<br>
Attached properties are properties that are defined on a type but are intended to be used with instances of other types. In `FmgLib.MauiMarkup`, attached properties are matched with attached property fluent methods, allowing you to set their values in a more readable and fluent manner.

For example, if you want to set the `AbsoluteLayout.LayoutBounds` attached property on a Border object, you would create an instance of Border and then use the `AbsoluteLayoutBounds` fluent method to set its value, like this:

```csharp
new Border().AbsoluteLayoutBounds(new Rect(100, 100, 200, 200));
```

This would set the `AbsoluteLayout.LayoutBounds` attached property to the specified rectangle value on the `Border` object.

## Attached properties list

 | Maui bağlı özelliği | FmgLib.MauiMarkup metodu |
 |-|-|
 |`FlyoutBase.ContextFlyout`|`ContextFlyout()`|
 |`Grid.Column`|`Column()`|
 |`Grid.Row`|`Row()`|
 |`Grid.ColumnSpan`|`ColumnSpan()`|
 |`Grid.RowSpan`|`RowSpan()`|
 |`Grid.ColumnSpan`+`Grid.RowSpan`|`Span(column, row)`|
 |`VisualStateManager.VisualStateGroups`|`VisualStateGroups()`|
 |`RadioButtonGroup.GroupName`|`RadioButtonGroupGroupName()`|
 |`RadioButtonGroup.SelectedValue`|`RadioButtonGroupSelectedValue()`|
 |`AbsoluteLayout.LayoutFlags`|`AbsoluteLayoutFlags()`|
 |`AbsoluteLayout.LayoutBounds`|`AbsoluteLayoutBounds()`|
 |`BindableLayout.EmptyView`|`BindableLayoutEmptyView()`|
 |`BindableLayout.EmptyViewTemplate`|`BindableLayoutEmptyViewTemplate()`| 
 |`BindableLayout.ItemsSource`|`BindableLayoutItemsSource()`|
 |`BindableLayout.ItemTemplate`|`BindableLayoutItemTemplate()`|
 |`BindableLayout.TemplateSelector`|`BindableItemTemplateSelector()`|
 |`Shell.PresentationMode`|`ShellPresentationMode()`|
 |`Shell.BackgroundColor`|`ShellBackgroundColor()`|
 |`Shell.ForegroundColor`|`ShellForegroundColor()`|
 |`Shell.TitleColor`|`ShellTitleColor()`|
 |`Shell.DisabledColor`|`ShellDisabledColor()`|
 |`Shell.UnselectedColor`|`ShellUnselectedColor()`|
 |`Shell.NavBarHasShadow`|`ShellNavBarHasShadow()`|
 |`Shell.NavBarIsVisible`|`ShellNavBarIsVisible()`|
 |`Shell.TitleView`|`ShellTitleView()`|
 |`Shell.TabBarBackgroundColor`|`ShellTabBarBackgroundColor()`|
 |`Shell.TabBarForegroundColor`|`ShellTabBarForegroundColor()`|
 |`Shell.TabBarTitleColor`|`ShellTabBarTitleColor()`|
 |`Shell.TabBarDisabledColor`|`ShellTabBarDisabledColor()`|
 |`Shell.TabBarUnselectedColor`|`ShellTabBarUnselectedColor()`|
 |`Shell.TabBarIsVisible`|`ShellTabBarIsVisible()`|
 |`Shell.FlyoutBackdrop`|`ShellFlyoutBackdrop()`|
 |`Shell.FlyoutBehavior`|`ShellFlyoutBehavior()`|
 |`Shell.FlyoutHeight`|`ShellFlyoutHeight()`|
 |`Shell.FlyoutWidth`|`ShellFlyoutWidth()`|
 |`Shell.FlyoutItemIsVisible`|`ShellFlyoutItemIsVisible()`|
 |`Shell.BackButtonBehavior`|`ShellBackButtonBehavior()`|
 |`Shell.ItemTemplate`|`ShellItemTemplate()`|
 |`Shell.MenuItemTemplate`|`ShellMenuItemTemplate()`|
 |`Shell.SearchHandler`|`ShellSearchHandler()`|
 |`NavigationPage.HasNavigationBar`|`NavigationPageHasNavigationBar()`|
 |`NavigationPage.BackButtonTitle`|`NavigationPageBackButtonTitle()`|
 |`NavigationPage.HasBackButton`|`NavigationPageHasBackButton()`|
 |`NavigationPage.IconColor`|`NavigationPageIconColor()`|
 |`NavigationPage.TitleIconImageSource`|`NavigationPageTitleIconImageSource()`|
 |`NavigationPage.TitleView`|`NavigationPageTitleView()`|
 |`SemanticProperties.Hint`|`SemanticHint()`|
 |`SemanticProperties.Description`|`SemanticDescription()`|
 |`SemanticProperties.HeadingLevel`|`SemanticHeadingLevel()`|
 |`AutomationProperties.ExcludedWithChildren`|`AutomationExcludedWithChildren()`|
 |`AutomationProperties.IsInAccessibleTree`|`AutomationIsInAccessibleTree()`|
 |`AutomationProperties.Name`|`AutomationName()`|
 |`AutomationProperties.HelpText`|`AutomationHelpText()`|
 |`AutomationProperties.LabeledBy`|`AutomationLabeledBy()`|
 |`ToolTipProperties.Text`|`ToolTipPropertiesText()`|

</details>
<details>
    <summary><b>Behaviors</b></summary>

<br>
<br>
In `FmgLib.MauiMarkup`, you can add functionality to user interface controls using behaviors. Behaviors allow you to add functionality to controls without having to subclass them.

You can add a behavior to a control by using the `Behaviors` method and passing in an instance of the behavior class. For example:

```csharp
new Entry().Text("Click Item")
  .Behaviors(new YourCustomBehaviors());
```

<br>
</details>
<details>
    <summary><b>Binding Converters</b></summary>

<br>
<br>
This code is an example of how to use binding converters in `FmgLib.MauiMarkup`. 

A `CollectionView` is defined and for each item in the `MyNumbers` list, a label is created with text equal to the value of the item. The `BackgroundColor` property of the label is bound to the item using the `Convert` method, which takes in a function that converts the value of the item (an integer) to a color. In this case, the function checks if the number is even or odd, and returns either `Colors.Green` or `Colors.Yellow` based on the result.

```csharp
public class CustomPage : ContentPage
{
    public List<int> MyNumbers = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    public CustomPage()
    {
	    this
	    .Content(
			new VerticalStackLayout()
			Children(
				new CollectionView()
                .ItemsSource(MyNumbers)
                .ItemTemplate(() => 
                    new Label()
                        .FontSize(30)
                        .Text(e => e.Path("."))
                        .TextColor(Colors.Gray)
                        .BackgroundColor(e => e
                            .Path(".")
                            .Convert((int n) => n % 2 == 0 ? Colors.Green : Colors.Yellow)
                        )
                )
			)
		);
    }
}
```

<br>
</details>

<details>
    <summary><b>Event handlers</b></summary>

<br>
<br>
In Maui, you can add functionality to user interface controls by handling events. For each `EventHandler` in a `FmgLib.MauiMarkup` class, a fluent helper method is generated to make it easier to attach an event handler to the control.

For example, in the case of the `Clicked` event handler in the `Button` class, two fluent methods are generated:

- `OnClicked(Button sender)`
- `OnClicked(object sender, EventArgs e)`

Here's an example of how you can use the fluent helper method `OnClicked` to handle the `Clicked` event on a `Button` control:

```csharp
using FmgLib.MauiMarkup;

public class ExamPage : ContentPage
{
    int count = 0;
    public ExamPage()
    {
        this
	    .Content(
			new VerticalStackLayout()
			.Children(
	            ...
	            new Button()
				.Text("Click me")
	            .OnClicked(OnCounterClicked),
	            ...
            )
        );
    }

    private void OnCounterClicked(Button sender)
    {
        count++;
        sender.Text = $"Clicked {count} ";
        sender.Text += count == 1 ? "time" : "times";
    }
}
``` 
Or, you can use an inline function to handle the event:

```csharp
new Button()
	.Text("Click me")
    .OnClicked(button =>
    {
        count++;
        button.Text = $"Clicked {count} ";
        button.Text += count == 1 ? "time" : "times";
    })
``` 

This makes it easy to attach event handlers to controls in a concise and readable way.

<br>
</details>

<details>
    <summary><b>Gesture Recognizers</b></summary>

<br>
<br>
The following gesture recognizers are available:

- `TapGestureRecognizer`
- `PanGestureRecognizer`
- `PointerGestureRecognizer`

### Tap Gesture Recognizer

The `TapGestureRecognizer` class is used to detect tap gestures on a view. You can specify the number of taps required using the `NumberOfTapsRequired` property.

Here's an example of using the TapGestureRecognizer to detect a double-tap gesture on an image:

```csharp
new StackLayout()
.Children(
    new Label()
    .Text("Tap 2 times on the image")
    .Assign(out var label),
    new Image()
    .Source("dotnet_bot.png")
    .Assign(out var image)
	.SizeRequest(100,100)
	.GestureRecognizers(new GestureRecognizer[]
        {
            new TapGestureRecognizer()
                .NumberOfTapsRequired(2)
                .OnTapped((e, args) =>
                {
                    label.Text = "You tapped 2 times";
                })
        })
)
```

### Pan Gesture Recognizer

The `PanGestureRecognizer` class is used to detect pan gestures on a view. You can use the `OnPanUpdated` method to handle the pan gesture event and update the position of the view.

Here's an example of using the `PanGestureRecognizer` to move an image on the screen:

```csharp
public class PanGesturePage : ContentPage
{
    double x, y;

    public PanGesturePage()
    {
        this
        .Content(
	        new Grid()
	        .Children(
	            new Image()
			    .Source("dotnet_bot.png")
			    .Assign(out var image)
                .SizeRequest(100,100)
                .GestureRecognizers(new GestureRecognizer[]
                {
                    new PanGestureRecognizer()
                        .OnPanUpdated((e, args) =>
                        {
                            switch (args.StatusType)
                            {
                                case GestureStatus.Running:
                                    image.TranslationX = x + args.TotalX;
                                    image.TranslationY = y + args.TotalY;
                                    break;

                                case GestureStatus.Completed:
                                    x = image.TranslationX;
                                    y = image.TranslationY;
                                    break;
                            }
                        })
                })
            )
        );
    }
}
```

### Pointer Gesture Recognizer

The `PointerGestureRecognizer` class is used to detect pointer events such as entering, exiting, and moving on a view. You can use the `OnPointerEntered`, `OnPointerExited`, and `OnPointerMoved` methods to handle these events and update the view accordingly.

Here's an example of using the `PointerGestureRecognizer` to display the position of a pointer on an image:

```csharp
public class PointerGesturePage : ContentPage
{
    public PointerGesturePage()
    {
        this
        .Content(
	        new StackLayout()
	        .Center()
	        .Children(
	            new Label().Assign(out var label).FontSize(20),
	            new Label().Assign(out var enterExitLabel).FontSize(20).TextColor(Colors.Blue),
	            new Image()
				    .Source("dotnet_bot.png")
				    .Assign(out var image)
	                .SizeRequest(300,300)
	                .GestureRecognizers(new GestureRecognizer[]
	                {
	                    new PointerGestureRecognizer()
	                        .OnPointerEntered((e, args) =>
	                        {
	                            enterExitLabel.Text = "Entered";
	                        })
	                        .OnPointerExited((e, args) =>
	                        {
	                            enterExitLabel.Text = "Exited";
	                        })
	                        .OnPointerMoved((e, args) =>
	                        {
	                            var pos = args.GetPosition(relativeTo: image).Value;
	                            label.Text = $"point: {pos.X}, {pos.Y}";
	                        })
	                })
	          )  
        };
    }
}
```

<br>
</details>

<details>
    <summary><b>Gradients</b></summary>
    
<br>
<br>
`FmgLib.MauiMarkup` provides a way to create visual effects using gradient brushes in curly braces. There are two defined types of gradient brushes: 

- LinearGradientBrush 
- RadialGradientBrush.

### Example

Here is an example of a `Border` element with a `LinearGradientBrush` as its background. The gradient effect goes from the top-left corner to the bottom-right corner.

```csharp
new Border()
.Background(
	new LinearGradientBrush()
	.StartPoint(new Point(0,0))
	.EndPoint(new Point(1,1))
	.GradientStops(
		new List<GradientStop>(){
			new GradientStop(Colors.Yellow, 0.0),
		    new GradientStop(Colors.Red, 0.25),
		    new GradientStop(Colors.Blue, 0.75),
		    new GradientStop(Colors.LimeGreen, 1.0)
	    }
	)
)
```

<br>
</details>

<details>
    <summary><b>Grid Definition</b></summary>
    
<br>
<br>
The `Grid` element allows you to create complex, multi-row and multi-column layout using Row and Column definitions. You can define the number and size of the rows and columns using the `RowDefinitions` and `ColumnDefinitions` methods, respectively.

You can set the position of a child element within the grid using the `Row()`, `Column()`, `ColumnSpan()`, and `RowSpan()` methods. These methods match the attached properties `Grid.Row`, `Grid.Column`, `Grid.ColumnSpan`, and `Grid.RowSpan`, respectively.

### Row and column definition

Defining the number and size of rows and columns is done using the `RowDefinitions` and `ColumnDefinitions` methods, respectively. These methods take a lambda function that defines the properties for the row or column.

In the following example, you're defining a `Grid` element with four rows and two columns:

```csharp
new Grid()
.RowDefinitions(e => e.Star(2).Star(0.5, count: 3)))
.ColumnDefinitions(e => e.Absolute(100).Star())
.Children(
	...
)
``` 

Here's what the code is doing:

The `RowDefinitions` method is defining four rows with different sizes. The first row takes up 2 stars, which means it will take up twice as much vertical space as any other row in the Grid. The second, third, and fourth rows each take up 0.5 stars. The count parameter is optional and specifies how many rows of the same size should be added to the Grid. In this case, it adds 3 rows of size 0.5 stars.

The `ColumnDefinitions` method is defining two columns. The first column is set to a fixed width of 100 pixels using the `Absolute` method, and the second column takes up the remaining space using the `Star` method.


### Example

Here is a full example of a grid definition:

```csharp
new Grid()
.RowDefinitions(e => e.Star(2).Star())
.ColumnDefinitions(e => e.Absolute(200).Star()))
.Children(
    new BoxView().Color(Colors.Green),
    new Label().Text("Column 0, Row 0"),

    new BoxView().Color(Colors.Blue).Column(1).Row(0),
    new Label().Text("Column 1, Row 0").Column(1).Row(0),

    new BoxView().Color(Colors.Teal).Column(0).Row(1),
    new Label().Text("Column 0, Row 1").Column(0).Row(1),

    new BoxView().Color(Colors.Purple).Column(1).Row(1),
    new Label().Text("Column 1, Row 1").Column(1).Row(1),
)
```

<br>
</details>

<details>
    <summary><b>ITextAlignment interface extension methods</b></summary>

<br>
<br>

In `FmgLib.MauiMarkup`, all classes that implement the `ITextAlignment` interface get the following extension methods:

 - `TextCenterHorizontal`
 - `TextCenterVertical`
 - `TextCenter`
 - `TextLeft`
 - `TextRight`
 - `TextBottom`
 - `TextBottomLeft`
 - `TextBottomRight`
 - `TextTop`
 - `TextTopLeft`
 - `TextTopRight`
 - `TextTopCenter`
 - `TextBottomCenter`
 - `TextCenterLeft`
 - `TextCenterRight`
 - `AlignText`

## Usage

To use the extension methods, create a `Label` object (or any object that implements `ITextAlignment`), and call the desired method:

```csharp
new Label().TextCenter()
```

This example centers the text both horizontally and vertically within the label's containing element.

<br>
</details>

<details>
    <summary><b>Layout options</b></summary>

In `FmgLib.MauiMarkup`, you can layout every view in their container using the following extension methods:

 - `CenterHorizontal`
 - `CenterVertical`
 - `Center`
 - `AlignLeft`
 - `AlignRight`
 - `AlignTop`
 - `AlignTopLeft`
 - `AlignTopRight`
 - `AlignBottom`
 - `AlignBottomLeft`
 - `AlignBottomRight`
 - `FillHorizontally`
 - `FillVertically`
 - `FillBothDirections`
 - `AlignTopCenter`
 - `AlignTopFill`
 - `AlignBottomCenter`
 - `AlignBottomFill`
 - `AlignCenterLeft`
 - `AlignCenterRight`
 - `AlignCenterFill`
 - `AlignFillLeft`
 - `AlignFillRight`
 - `AlignFillCenter`
 - `AlignLayout`

## Usage

To use the layout options, create a container view , add the view you want to layout to the container, and call the desired method:

```csharp
new StackLayout()
.Children(
    new Label().Text("Hello, World!").Center()
)
```

This example centers a Label inside a `StackLayout` container. You can use the same method with other container views, and with any view that you want to lay out within its containing element.

</details>


<details>
    <summary><b>Menus in FmgLib.MauiMarkup</b></summary>

<br>
<br>

### Context menu

Here is an example of creating a context menu for an image. The context menu has options for copying and pasting, and also for changing the background color of a grid.

```csharp
new Grid()
.Assign(out var grid)
.Children(
    new Image()
	    .Source("dotnet_bot.png")
        .ContextFlyout(new MenuFlyout()
			{
	            new MenuFlyoutItem()
	            .Text("Copy")
				.OnClicked(e => Console.WriteLine("Copy")),
	            new MenuFlyoutItem()
	            .Text("Paste")
	            .OnClicked(e => Console.WriteLine("Paste")),
	            new MenuFlyoutSubItem()
	            {
	                new MenuFlyoutItem()
	                .Text("Blue")
	                .OnClicked(e => grid.BackgroundColor = Colors.Blue),
	                new MenuFlyoutItem()
	                .Text("Red")
	                .OnClicked(e => grid.BackgroundColor = Colors.Red),
	                new MenuFlyoutItem()
	                .Text("Black")
	                .OnClicked(e => grid.BackgroundColor = Colors.Black)
	            }
	            .Text("Background color")
	        }
        )
)
```

### Menu bar

Here is an example of creating a menu bar for a `ContentPage`. The menu bar has three options: My Menu, Edit, and Theme.

```csharp
public class MenuPage : ContentPage
{
    public MenuPage()
    {
        this.MenuBarItems(new MenuBarItem[]
        {
            new MenuBarItem()
            {
                new MenuFlyoutItem()
		            .Text("Exit")
                    .OnClicked(e => Application.Current.Quit()),
            }
            .Text("My Menu"),
            new MenuBarItem()
            {
                new MenuFlyoutItem()
		            .Text("Copy")
                    .OnClicked(e => Console.WriteLine("Copy"))
                    .KeyboardAccelerators(
                        new KeyboardAccelerator()
                        .Key("C")
                        .Modifiers(KeyboardAcceleratorModifiers.Ctrl)
                    ),
                new MenuFlyoutItem()
		            .Text("Paste")
                    .OnClicked(e => Console.WriteLine("Paste"))
                    .KeyboardAccelerators(
                        new KeyboardAccelerator()
                        .Key("V")
                        .Modifiers(KeyboardAcceleratorModifiers.Ctrl)
                    ),
            }
            .Text("Edit"),
            new MenuBarItem()
            {
                new MenuFlyoutItem()
	                .Text("Blue")
                    .OnClicked(e => this.BackgroundColor = Colors.Blue),
                ...
            }
            .Text("Theme")
        });

        ...
    }
}
```

<br>
</details>

<details>
    <summary><b>Properties and Fluent Methods</b></summary>

<br>
<br>
`FmgLib.MauiMarkup` provides a convenient way to set properties for UI elements by matching properties with fluent helper methods. This makes it easier and more readable to define the interface of your application.

Here is an example of using fluent methods to set properties on a `Label`:

```csharp
new Label()
    .Text("This is a test")
    .Padding(20)
    .FontSize(30)
    .Center())
```


`FmgLib.MauiMarkup` also provides a way to set property values based on device idiom, platform, or app theme. Here is an example of setting the font size and text color of a `Label` based on the current device or theme:

```csharp
new Label()
	.Text("Hello")
    .FontSize(e => e.OnDesktop(80).OnPhone(30).Default(50))
    .TextColor(e => e.OnLight(Colors.Black).OnDark(Colors.Teal))
```

<br>
</details>

<details>
    <summary><b>Property Bindings</b></summary>

<br>
<br>
`FmgLib.MauiMarkup` provides a simple way to bind properties of an element to a source, so that when the source changes, the property changes as well. You can bind a property by using the fluent method e.g. `Text()`, `TextColor()` etc. and then using lambda call the method `Path()` to specify the property you want to bind to.


```csharp
public class SimpleBindings : ContentPage
{
    public SimpleBindings()
    {
        this.Content(
	        new StackLayout()
	        Children(
	            new Slider()
	            .Assign(out var slider)
                .Minimum(1)
                .Maximum(20),

	            new Label()
                .Text(e => e.Path("Value").Source(slider).StringFormat("Slider value: {0}"))
                .FontSize(28)
	        )
        );
    }
}
``` 

In this example, the text property of the label is bound to the `Value` property of a `Slider` element named `slider`. When the value of the slider changes, the text of the label will automatically update to reflect the new value.

You can also bind a property to an object that is not part of the visual tree. This is useful when you have a separate data source, such as a model or a view model, that you want to bind to a visual element.

<br>
</details>

<details>
    <summary><b>Property MultiBinding</b></summary>

<br>
<br>

You can easily use multibinding with FmgLib.MauiMarkup. You can add as many BindingBases as you want with the e.Bindings(...) method.

Example usage is as follows:
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
                .IsChecked(e => 
                    e.Bindings(
                        new Binding().Path("Employee.IsOver16"),
                        new Binding().Path("Employee.HasPassedTest"),
                        new Binding().Path("Employee.IsSuspended").Converter(new InverterConverter())
                    )
                    .Converter(new AllTrueMultiConverter())
                    .FallbackValue("Is Error.")
                    .TargetNullValue("Is Null.")
                ),

                new Label()
                .Text(e => 
                    e.Bindings(
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

<br>
</details>

<details>
	<summary><b>Compiled Bindings in Code</b></summary>

 <br>

Bindings written in code typically use string paths that are resolved at runtime with reflection, and the overhead of doing this varies from platform to platform. `FmgLib.MauiMarkup` introduces an additional Getter extension method that defines bindings using a Func argument instead of a string path:

```csharp
// in before
new Label().Text(e => e.Path("Text"));

// in now
new Label().Text(e => e.Getter(static (Entry entry) => entry.Text));
```

This compiled binding approach provides the following benefits:
  - Improved data binding performance by resolving binding expressions at compile-time rather than runtime.
  - A better developer troubleshooting experience because invalid bindings are reported as build errors.
  - Intellisense while editing.

Not all methods can be used to define a compiled binding. The expression must be a simple property access expression. The following examples show valid and invalid binding expressions:

```csharp
// Valid: Property access
static (PersonViewModel vm) => vm.Name;
static (PersonViewModel vm) => vm.Address?.Street;

// Valid: Array and indexer access
static (PersonViewModel vm) => vm.PhoneNumbers[0];
static (PersonViewModel vm) => vm.Config["Font"];

// Valid: Casts
static (Label label) => (label.BindingContext as PersonViewModel).Name;
static (Label label) => ((PersonViewModel)label.BindingContext).Name;

// Invalid: Method calls
static (PersonViewModel vm) => vm.GetAddress();
static (PersonViewModel vm) => vm.Address?.ToString();

// Invalid: Complex expressions
static (PersonViewModel vm) => vm.Address?.Street + " " + vm.Address?.City;
static (PersonViewModel vm) => $"Name: {vm.Name}";
```

You can easily use multibinding with FmgLib.MauiMarkup. You can add as many BindingBases as you want with the e.Bindings(...) method. In addition, .NET MAUI 9 adds a BindingBase.Create method that sets the binding directly on the object with a Func, and returns the binding object instance:

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
                .IsChecked(e => 
                    e.Bindings(
			Binding.Create(static (MainPageViewModel model) => model.IsOver16, source: RelativeBindingSource.Self),
		        Binding.Create(static (MainPageViewModel model) => model.HasPassedTest, source: RelativeBindingSource.Self),
		        Binding.Create(static (MainPageViewModel model) => model.IsSuspended, source: RelativeBindingSource.Self)
                    )
                    .Converter(new AllTrueMultiConverter())
                    .FallbackValue("Is Error.")
                    .TargetNullValue("Is Null.")
                ),

                new Label()
                .Text(e => 
                    e.Bindings(
			Binding.Create(static (MainPageViewModel model) => model.Id, source: RelativeBindingSource.Self),
		        Binding.Create(static (MainPageViewModel model) => model.Name, source: RelativeBindingSource.Self),
		        Binding.Create(static (MainPageViewModel model) => model.IsSuspended, source: RelativeBindingSource.Self)
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
 
</details>

<details>
    <summary><b>Shell Application</b></summary>

<br>
<br>
Here's an example of a simple shell-based application:

```csharp
using FmgLib.MauiMarkup;

public partial class App : Application
{
    public App()
    {
        this.MainPage(
	        new Shell()
	        .ItemTemplate(() => new ShellItemTemplate())
            .Resources(new ResourceDictionary().MergedDictionaries(AppStyles.Default))
	        .Items(
				new FlyoutItem()
				.FlyoutDisplayOptions(FlyoutDisplayOptions.AsMultipleItems)
				.Items(
	                new Tab()
	                .Title("Main")
	                .Items(
	                    new ShellContent()
		                .Title("Hello Page")
		                .ContentTemplate(new HelloWorldPage()),
	                    new ShellContent()
		                .Title("ExamplePage")
		                .ContentTemplate(new ExamplePage()),
	                ),

	                new ShellContent()
	                .Title("Grid")
	                .ContentTemplate(new GridPage()),
	                ...
	            )
			)
        );
    }
}
```

You can customize the appearance of the `FlyoutItem` by defining a custom content view and setting the `ItemTemplate` property on the `Shell` element.

Here's an example of defining the appearance of a `FlyoutItem`:

```csharp
public class ShellItemTemplate : ContentView
{
    public ShellItemTemplate()
    {
        this
        .Content(
	        new Grid()
	        .ColumnDefinitions(e => e.Star(0.2).Star(0.8))
	        .Children(
	            new Image()
	                .Source(e => e.Path("FlyoutIcon"))
	                .Margin(5)
	                .HeightRequest(45),

	            new Label()
	                .GridColumn(1)
	                .Text(e => e.Path("Title"))
	                .FontSize(20)
	                .FontAttributes(FontAttributes.Italic)
	                .CenterVertically()
	        )
		);
    }
}
```

<br>
</details>

<details>
    <summary><b>Application Styling</b></summary>

<br>

`FmgLib.MauiMarkup` provides a way to define the styles of elements using the `Style<T>` class. Here's an example of how to define the style of a button: 


```csharp
new Style<Button>(e => e
    .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
    .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))
    .FontFamily("OpenSansRegular")
    .FontSize(14)
    .CornerRadius(8)
    .Padding(new Thickness(14,10))
    .MinimumHeightRequest(44)
    .MinimumWidthRequest(44))
```

In the example, the properties of a button are set using fluent extension methods.

You can also use different values depending on the app theme, device idiom, or platform:

Additionally, you can define visual states for elements:

```csharp
new Style<Button>(e => e...)
{
    ...
    new VisualState<Button> (VisualStateEnum.VisualElement.Normal, e => e
        .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
        .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))),

    new VisualState<Button> (VisualStateEnum.VisualElement.Disabled, e => e
        .TextColor(e => e.OnLight(AppColors.Gray950).OnDark(AppColors.Gray200))
        .BackgroundColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray600))),
},
```

You can also use visual states to define animations:

```csharp
new Style<Button>(e => e...)
{
    ...
    
    new VisualState<Button>(VisualStates.Button.Normal, e => e
        .FontSize(33)
        .TextColor(AppColors.Gray200))
    {
        async button => {
            await button.RotateTo(0);   // create animation inside VisualState
        }
    },

    new VisualState<Button>(VisualStates.Button.Disabled, e => e
        .FontSize(20)
        .TextColor(AppColors.Gray600))
    {
        async button => {
            await button.RotateTo(180);
        }
    },
}
```

Finally, all defined styles can be placed in a `ResourceDictionary`:

```csharp
new ResourceDictionary
{
    ...

    new Style<Button>(e => e
        .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
        .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))
        .FontFamily("OpenSansRegular")
        .FontSize(14)
        .CornerRadius(8)
        .Padding(new Thickness(14,10))
        .MinimumHeightRequest(44)
        .MinimumWidthRequest(44))
    {
        new VisualState<Button> (VisualStateEnum.VisualElement.Normal, e => e
            .TextColor(e => e.OnLight(Colors.White).OnDark(AppColors.Primary))
            .BackgroundColor(e => e.OnLight(AppColors.Primary).OnDark(Colors.White))),

        new VisualState<Button> (VisualStateEnum.VisualElement.Disabled, e => e
            .TextColor(e => e.OnLight(AppColors.Gray950).OnDark(AppColors.Gray200))
            .BackgroundColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray600)))
    },      
        
    new Style<Frame>(e => e
        .HasShadow(false)
        .BorderColor(e => e.OnLight(AppColors.Gray200).OnDark(AppColors.Gray950))
        .CornerRadius(8)),
    ...
};
```


</details>

<details>
    <summary><b>User defined extension methods</b></summary>

<br>
<br>
In `FmgLib:mauiMarkup`, you can create your own extension methods by defining a static method within a static class.

Here's an example of extension methods that set a label font size:

```csharp
public static T FontSize<T>(this T self,
    double fontSize)
    where T : Microsoft.Maui.Controls.Label
{
    self.SetValue(Microsoft.Maui.Controls.Label.FontSizeProperty, fontSize);
    return self;
}
        
public static T FontSize<T>(this T self, Func<PropertyContext<double>, IPropertyBuilder<double>> configure)
    where T : Microsoft.Maui.Controls.Label
{
    var context = new PropertyContext<double>(self, Microsoft.Maui.Controls.Label.FontSizeProperty);
    configure(context).Build();
    return self;
}
        
public static SettersContext<T> FontSize<T>(this SettersContext<T> self,
    double fontSize)
    where T : Microsoft.Maui.Controls.Label
{
    self.XamlSetters.Add(new Setter { Property = Microsoft.Maui.Controls.Label.FontSizeProperty, Value = fontSize });
    return self;
}
        
public static SettersContext<T> FontSize<T>(this SettersContext<T> self, Func<PropertySettersContext<double>, IPropertySettersBuilder<double>> configure)
    where T : Microsoft.Maui.Controls.Label
{
    var context = new PropertySettersContext<double>(self.XamlSetters, Microsoft.Maui.Controls.Label.FontSizeProperty);
    configure(context).Build();
    return self;
}
        
public static Task<bool> AnimateFontSizeTo<T>(this T self, double value, uint length = 250, Easing? easing = null)
    where T : Microsoft.Maui.Controls.Label
{
    double fromValue = self.FontSize;
    var transform = (double t) => Transformations.DoubleTransform(fromValue, value, t);
    var callback = (double actValue) => { self.FontSize = actValue; };
    return Transformations.AnimateAsync<double>(self, "AnimateFontSizeTo", transform, callback, length, easing);
}
```

it allows you the following usage:

```csharp
new Label().FontSize(28)
```

or:

```csharp
new Label().FontSize(e => e.Path("MyFontSize").Source(myModel))
new Label().FontSize(e => e.OnPhone(30).OnTablet(50).Default(40))
```

or use it in a style context:

```csharp
new Style<Label>(e => e
    .FontSize(20)
    .CenterVertically()
    .CenterHorizontally())
```


or use in an animation context.

<br>
</details>

<details>
    <summary><b>Triggers</b></summary>

<br>
<br>
Triggers allow you to set properties in response to certain conditions or events.

### Property Triggers

A property trigger sets a property in response to a change in the value of another property.

Here is an example of using a property trigger to change the background color and text color of an `Entry` element when it gets focused:

```csharp
using FmgLib:mauiMarkup;

public class PropertyTriggerPage : ContentPage
{
    public PropertyTriggerPage()
    {
	    this
        .Resources(
	        new ResourceDictionary
	        {
	            new Style<Entry>
	            {
	                Entry.BackgroundColorProperty.Set(Colors.Black),
	                Entry.TextColorProperty.Set(Colors.White),

	                new Trigger(typeof(Entry))
	                    .Property(Entry.IsFocusedProperty)
	                    .Value(true)
	                    .Setters(
	                        new Setters<Entry>(e => e
	                            .BackgroundColor(Colors.Yellow)
	                            .TextColor(Colors.Black))
	                    ),
	            }
	        }
        )
        .Content(
			new StackLayout()
			.Children(
				new Entry().Text("Enter name"),
	            new Entry().Text("Enter password"),
	            new Entry().Text("Enter address")
			)
		);
    }
}
```

### Data Triggers

A data trigger sets a property in response to a change in the value of a data source.

Here is an example of using a data trigger to disable a button if the text length of an `Entry` element is zero:

```csharp
this.Content(new StackLayout()
	.Children(
	    new Entry().Assign(out var entry).Text("Enter text..."),
	    new Button()
		    .Text("Save")
	        .Triggers(
	            new DataTrigger(typeof(Button))
	                .Binding(e => e.Path("Text.Length").Source(entry))
	                .Value(0)
	                .Setters(new Setters<Entry>(e => e.IsEnabled(false)))
	        ),
	)
)
```

### Event Triggers

An event trigger sets a property in response to an event.

Here is an example of using an event trigger to validate the input of an `Entry` element as a number:

```csharp
this
.Content(new StackLayout()
	.Children(
	    new Entry()
		    .Assign(out var entry)
		    .Text("Enter text...")
	        .Triggers(
	            new EventTrigger()
	                .Event("TextChanged")
	                .Actions(new NumericValidationTriggerAction())
	        )
	)
)
```

And here is the definition of the `NumericValidationTriggerAction` class:

```csharp
public class NumericValidationTriggerAction : TriggerAction<Entry>
{
    protected override void Invoke(Entry entry)
    {
        double result;
        bool isValid = Double.TryParse(entry.Text, out result);
        entry.TextColor = isValid ? Colors.Black : Colors.Red;
    }
}
```

And here is the definition of the `NumericValidationTriggerAction` class:

```csharp
public class NumericValidationTriggerAction : TriggerAction<Entry>
{
    protected override void Invoke(Entry entry)
    {
        double result;
        bool isValid = Double.TryParse(entry.Text, out result);
        entry.TextColor = isValid ? Colors.Black : Colors.Red;
    }
}
```

<br>
</details>

<details>
    <summary><b>.NET built-in Hot-Reload</b></summary>

<br>
<br>
Additionally, the FmgLib.MauiMarkup library includes hot reload support to make the development process faster and more efficient.

If you want to enhance your page with fast reload, your page needs to implement the IFmgLibHotReload interface. Then, you should trigger the this.InitializeHotReload(); function within the constructor.

And you can write your design code inside the `Build()` method to enhance it with fast reload.

Example usage is as follows:

```csharp
public partial class ExamplePage : ContentPage, IFmgLibHotReload
{
    public ExamplePage()
    {
        this.InitializeHotReload();
    }
    public void Build()
    {
        this
        .Content(
            new Label()
            .Text("FmgLib.MauiMarkup")
            .CharacterSpacing(2)
            .FontSize(30)
            .FontAttributes(Italic)
            .TextColor(Green)
            .TextCenter()
        );
    }
}
```

<br>
</details>

<details>
    <summary><b>Extensions for 3rd Party Controls</b></summary>
    
<br>
<br>
FmgLib.MauiMarkup library can also generate extension methods for controls from third-party libraries. To achieve this, you should utilize the `MauiMarkupAttribute` provided by FmgLib.MauiMarkup.

Simply specify the control for which you want to create extension methods as `[MauiMarkup(typeof(YourControl))]`.

The constructor method of the `MauiMarkup()` attribute automatically generates extension methods for BindableProperties and Events found within the type provided as an argument. **You can provide a minimum of 1 type inside the constructor method, and there is no maximum limit.** **Multiple MauiMarkup attributes can be added to a single class.**

If you want to enable automatic generation for third-party MAUI controls without adding `MauiMarkupAttribute`, set the following property in your app `.csproj`:

```xml
<PropertyGroup>
  <MauiMarkupSourceGenerator>true</MauiMarkupSourceGenerator>
</PropertyGroup>
```

`CompilerVisibleProperty` is injected automatically by the package via `buildTransitive` props.

When this property is enabled, the source generator scans referenced third-party assemblies and creates fluent extension methods for eligible public `BindableObject` controls automatically.


**NOTE: Suppose that if a property inherited from base classes has been redefined with the `new` keyword in the class where you want to create a Fluent Method, the name of the new Fluent Method will be `PropertyName + New`.**

For example, since the **Background** property in the **SfAvatarView** class is redefined with `new` and is the same as the **Background** in **VisualElement**, to prevent errors, the Fluent Method name for the **Background** property of the **SfAvatarView** class will be `BackgroundNew`.

Let's look at an example:

```csharp
using FmgLib.MauiMarkup;

namespace GeneratedExam;

[MauiMarkup(typeof(ZXing.Net.Maui.Controls.BarcodeGeneratorView))]
public class MyBarcodeGeneratorView { }

[MauiMarkup(typeof(ZXing.Net.Maui.Controls.CameraView))]
public class MyCameraView { }


[MauiMarkup(typeof(ZXing.Net.Maui.Controls.CameraBarcodeReaderView))]
public class MyCameraBarcodeReaderView { }

[MauiMarkup(typeof(SkiaSharp.Extended.UI.Controls.SKLottieView))]
public class MySkLottieView { }

```

Or instead of dealing with it like this, it can be used like this:

```csharp
using Microsoft.Extensions.Logging;
using FmgLib.MauiMarkup;
using SkiaSharp.Extended.UI.Controls;
using ZXing.Net.Maui.Controls;
using UraniumUI.Material.Controls;
namespace MauiApp1
{
    [MauiMarkup(typeof(CameraView))]
    [MauiMarkup(typeof(SKLottieView), typeof(SKFileLottieImageSource), typeof(DataGrid))]
    [MauiMarkup(typeof(SKConfettiView), typeof(BarcodeGeneratorView),typeof(InputField),typeof(EditorField),typeof(TextField))]
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
```

CameraView class from ZXing.Net.Maui.Controls library:
```csharp
﻿using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;

namespace ZXing.Net.Maui.Controls
{
	public partial class CameraView : View, ICameraView
	{
		public event EventHandler<CameraFrameBufferEventArgs> FrameReady;

		void ICameraFrameAnalyzer.FrameReady(CameraFrameBufferEventArgs e)
			=> FrameReady?.Invoke(this, e);

		public static readonly BindableProperty IsTorchOnProperty =
			BindableProperty.Create(nameof(IsTorchOn), typeof(bool), typeof(CameraView), defaultValue: true);

		public bool IsTorchOn
		{
			get => (bool)GetValue(IsTorchOnProperty);
			set => SetValue(IsTorchOnProperty, value);
		}

		public static readonly BindableProperty CameraLocationProperty =
			BindableProperty.Create(nameof(CameraLocation), typeof(CameraLocation), typeof(CameraView), defaultValue: CameraLocation.Rear);

		public CameraLocation CameraLocation
		{
			get => (CameraLocation)GetValue(CameraLocationProperty);
			set => SetValue(CameraLocationProperty, value);
		}

		public void AutoFocus()
			=> StrongHandler?.Invoke(nameof(AutoFocus), null);

		public void Focus(Point point)
			=> StrongHandler?.Invoke(nameof(Focus), point);

		CameraViewHandler StrongHandler 
			=> Handler as CameraViewHandler;
	}
}
```


The CameraViewExtension class created using the MauiMarkup attribute will be as follows:

```csharp
//
// <auto-generated-fmglib-mauimarkup-generator />
//


namespace FmgLib.MauiMarkup;

public static partial class CameraViewExtension
{
        public static T CameraLocation<T>(this T self,
            ZXing.Net.Maui.CameraLocation cameraLocation)
            where T : ZXing.Net.Maui.Controls.CameraView
        {
            self.SetValue(ZXing.Net.Maui.Controls.CameraView.CameraLocationProperty, cameraLocation);
            return self;
        }
        
        public static T CameraLocation<T>(this T self, Func<PropertyContext<ZXing.Net.Maui.CameraLocation>, IPropertyBuilder<ZXing.Net.Maui.CameraLocation>> configure)
            where T : ZXing.Net.Maui.Controls.CameraView
        {
            var context = new PropertyContext<ZXing.Net.Maui.CameraLocation>(self, ZXing.Net.Maui.Controls.CameraView.CameraLocationProperty);
            configure(context).Build();
            return self;
        }
        
        public static SettersContext<T> CameraLocation<T>(this SettersContext<T> self,
            ZXing.Net.Maui.CameraLocation cameraLocation)
            where T : ZXing.Net.Maui.Controls.CameraView
        {
            self.XamlSetters.Add(new Setter { Property = ZXing.Net.Maui.Controls.CameraView.CameraLocationProperty, Value = cameraLocation });
            return self;
        }
        
        public static SettersContext<T> CameraLocation<T>(this SettersContext<T> self, Func<PropertySettersContext<ZXing.Net.Maui.CameraLocation>, IPropertySettersBuilder<ZXing.Net.Maui.CameraLocation>> configure)
            where T : ZXing.Net.Maui.Controls.CameraView
        {
            var context = new PropertySettersContext<ZXing.Net.Maui.CameraLocation>(self.XamlSetters, ZXing.Net.Maui.Controls.CameraView.CameraLocationProperty);
            configure(context).Build();
            return self;
        }
        
        public static T IsTorchOn<T>(this T self,
            bool isTorchOn)
            where T : ZXing.Net.Maui.Controls.CameraView
        {
            self.SetValue(ZXing.Net.Maui.Controls.CameraView.IsTorchOnProperty, isTorchOn);
            return self;
        }
        
        public static T IsTorchOn<T>(this T self, Func<PropertyContext<bool>, IPropertyBuilder<bool>> configure)
            where T : ZXing.Net.Maui.Controls.CameraView
        {
            var context = new PropertyContext<bool>(self, ZXing.Net.Maui.Controls.CameraView.IsTorchOnProperty);
            configure(context).Build();
            return self;
        }
        
        public static SettersContext<T> IsTorchOn<T>(this SettersContext<T> self,
            bool isTorchOn)
            where T : ZXing.Net.Maui.Controls.CameraView
        {
            self.XamlSetters.Add(new Setter { Property = ZXing.Net.Maui.Controls.CameraView.IsTorchOnProperty, Value = isTorchOn });
            return self;
        }
        
        public static SettersContext<T> IsTorchOn<T>(this SettersContext<T> self, Func<PropertySettersContext<bool>, IPropertySettersBuilder<bool>> configure)
            where T : ZXing.Net.Maui.Controls.CameraView
        {
            var context = new PropertySettersContext<bool>(self.XamlSetters, ZXing.Net.Maui.Controls.CameraView.IsTorchOnProperty);
            configure(context).Build();
            return self;
        }
        
        public static T OnFrameReady<T>(this T self, System.EventHandler<ZXing.Net.Maui.CameraFrameBufferEventArgs> handler)
            where T : ZXing.Net.Maui.Controls.CameraView
        {
            self.FrameReady += handler;
            return self;
        }
        
        public static T OnFrameReady<T>(this T self, System.Action<T> action)
            where T : ZXing.Net.Maui.Controls.CameraView
        {
            self.FrameReady += (o, arg) => action(self);
            return self;
        }
        
}

```

For example, let's write the sample code for the **TextField and SKLottieView Controls**:

```csharp
new TextField()
.Title("Password")
.TitleColor(Colors.LightGray)
.AccentColor(Colors.CadetBlue)
.TextColor(Colors.White)
.IsPassword(true),

new SKLottieView()
.Source(new SKFileLottieImageSource().File("iconapp.json"))
.RepeatCount(-1)
.HeightRequest(250)
.WidthRequest(250)
```

With the **MauiMarkupAttachedPropAttribute**, you can easily create extension methods for **AttachedProperties** within Control classes from third-party libraries.
**The constructor's first parameter takes the type of the Control class, the second parameter takes the name of the AttachedProperty, the third parameter takes the value type that the AttachedProperty can accept, and the fourth parameter takes the type to which the AttachedProperty will be applied.**

Example usage:

```csharp
[MauiMarkupAttachedProp(typeof(InputKit.Shared.Controls.FormView), nameof(InputKit.Shared.Controls.FormView.IsSubmitButtonProperty), typeof(bool), typeof(Button))]
[MauiMarkup(typeof(InputKit.Shared.Controls.FormView))]
public class MyFormView { }
```
OR
```csharp

using Microsoft.Extensions.Logging;
using FmgLib.MauiMarkup;

namespace MauiApp1
{
    [MauiMarkupAttachedProp(typeof(InputKit.Shared.Controls.FormView), nameof(InputKit.Shared.Controls.FormView.IsSubmitButtonProperty), typeof(bool), typeof(Button))]
    [MauiMarkup(typeof(InputKit.Shared.Controls.FormView))]
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
```

The usage of the AttachedProperty in the Button class will be as follows:
```csharp
new Button()
.Text("Login")
.FontAttributes(Bold)
.FormViewIsSubmitButton(true)
```

<br>
</details>

<details>
    <summary><b>Localization (Json File)</b></summary>

<br>
<br>
In the MauiProgram.cs file,

```CSharp
builder
    .UseMauiApp<App>()
    .UseMauiMarkupLocalization();
```
should be added.

In your main project you should have a language file of type json. The translation will be read from this file and imported.
If you do not specify the path to the file(s) in the parameter ( 

```CSharp
builder
    .UseMauiApp<App>()
    .UseMauiMarkupLocalization();
  //.UseMauiMarkupLocalization(defaultLang:"en-US"); // or
  //.UseMauiMarkupLocalization(defaultLang:"en-US", "Loc1.json", "Loc2.json"); // or
```
), will look for a json file named `Localization.json` in the home directory.

if you give one or more parameters like
```CSharp
builder
    .UseMauiApp<App>()
    .UseMauiMarkupLocalization(filePaths: "Localization1.json", "Localization2.json", "/Languages/Temp1.json");
```

it will read json files in given file paths.

**The critical point here is to select ```Build Action: MauiAsset``` for json files.**

Proper json format:

```json
{
  "Hello": {
    "tr-TR": "Merhaba Dünya!",
    "en-US": "Hello World!"
  },
  "Msg": {
    "tr-TR": "Deneme amaçlı yapılmıştır.",
    "en-US": "It was made for testing purposes."
  }
}
```

Instead of 'keyWord' keywords, you can use any word or phrase(s) you want. You don't have any Regex limitations.

You can also change the 'tr-TR' and 'en-US' language keys with words or sentences as you wish. But it is recommended to use expressions such as 'en-US', 'tr-TR', 'fr-FR'.

You can change the language of the system with the following statement: `Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));`


You can simply use 
```CSharp
new Label()
.Text(e => e.Translate("Hello"))
.FontSize(32)
.CenterHorizontal()
.SemanticHeadingLevel(SemanticHeadingLevel.Level1),

new Label()
.Text(e => e.Translate("Msg"))
.FontSize(18)
.CenterHorizontal()
.SemanticDescription(e => e.Translate("Msg"))
.SemanticHeadingLevel(SemanticHeadingLevel.Level1),

new VerticalStackLayout()
.Center()
.Children(
    new RadioButton()
    .IsChecked(Translator.Instance.CurrentCulture.Name == "tr-TR")
    .Content("tr-TR")
    .OnCheckedChanged((sender, e) =>
    {
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));
    }),
    new RadioButton()
    .IsChecked(Translator.Instance.CurrentCulture.Name == "en-US")
    .Content("en-US")
    .OnCheckedChanged((sender, e) =>
    {
        Translator.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
    })
)
``` 
in the code.


</details>

<details>
    <summary><b>Localization (Resx File)</b></summary>
    
<br>
<br>
In the MauiProgram.cs file,

```CSharp
builder
    .UseMauiApp<App>()
    .UseMauiMarkupLocalizationWithResx(AppResources.ResourceManager);
    // .UseMauiMarkupLocalizationWithResx(AppResources.ResourceManager, "en-US"); // set default lang
```
should be added.

You should have source files with the extension resx in your project. The translation will be read from this file and imported.
Then, in **MauiProgram.cs**, pass your `ResourceManager` value as a parameter to `UseMauiMarkupLocalizationWithResx`.


You can also change the 'tr-TR' and 'en-US' language keys with words or sentences as you wish. But it is recommended to use expressions such as 'en-US', 'tr-TR', 'fr-FR'.

You can change the language of the system with the following statement: `TranslatorResx.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));`

You can simply use 
```CSharp
new Label()
.Text(e => e.TranslateResx("Hello"))
.FontSize(32)
.CenterHorizontal()
.SemanticHeadingLevel(SemanticHeadingLevel.Level1),

new Label()
.Text(e => e.TranslateResx(nameof(AppResource.Msg)))
.FontSize(18)
.CenterHorizontal()
.SemanticDescription(e => e.TranslateResx("Msg"))
.SemanticHeadingLevel(SemanticHeadingLevel.Level1),

new VerticalStackLayout()
.Center()
.Children(
    new RadioButton()
    .IsChecked(TranslatorResx.Instance.CurrentCulture.Name == "tr-TR")
    .Content("tr-TR")
    .OnCheckedChanged((sender, e) =>
    {
        TranslatorResx.Instance.ChangeCulture(CultureInfo.GetCultureInfo("tr-TR"));
    }),
    new RadioButton()
    .IsChecked(TranslatorResx.Instance.CurrentCulture.Name == "en-US")
    .Content("en-US")
    .OnCheckedChanged((sender, e) =>
    {
        TranslatorResx.Instance.ChangeCulture(CultureInfo.GetCultureInfo("en-US"));
    })
)
``` 
in the code.

</details>

<details>
    <summary><b>General Example Code</b></summary>

<br>
<br>

```csharp

using Microsoft.Maui.Layouts;
using FmgLib.MauiMarkup;

namespace MyOrderApp.Views;

public partial class HomePage : BasePage<HomePageViewModel>
{
    public HomePage(HomePageViewModel viewModel) : base(viewModel, "Home Page")
    {
    }

    public override void Build()
    {
        this
        .Content(
            new VerticalStackLayout()
            .Padding(10)
            .Children(
                new SearchBar()
                .Placeholder("Ürünlerde Ara.")
                .Margin(10)
                .Assign(out var search)
                .SearchCommand(BindingContext.SearchCommand)
                .Bind(SearchBar.SearchCommandParameterProperty, "Text", source: search),

                new Frame()
                .CornerRadius(15)
                .BackgroundColor(Colors.Blue)
                .BorderColor(Colors.Blue)
                .HeightRequest(150)
                .Margin(15,7)
                .Padding(0)
                .Content(
                    new Grid()
                    .ColumnDefinitions(e => e.Star(5).Star(5))
                    .RowDefinitions(e => e.Star(5).Star(5))
                    .Children(
                        new Frame()
                        .Row(0)
                        .Column(0)
                        .Margin(0,20,0,0)
                        .Padding(0)
                        .CornerRadius(0)
                        .BackgroundColor(Colors.DarkBlue)
                        .BorderColor(Colors.DarkBlue)
                        .Content(
                            new Label()
                            .Text("%50 İndirim")
                            .TextColor(Colors.White)
                            .FontAttributes(FontAttributes.Bold)
                            .FontSize(20)
                            .Center()
                        ),

                        new Label()
                        .Text("Tüm Unlu Mamüllerde her gün saat 21:00'dan sonra!")
                        .FontSize(12)
                        .Row(1)
                        .Column(0)
                        .TextColor(Colors.White)
                        .FontAttributes(FontAttributes.Italic)
                        .Margin(10,3,0,0),

                        new Image()
                        .Source("white_board.png")
                        .Row(0)
                        .Column(1)
                        .RowSpan(2)
                        .SizeRequest(150,100)
                        .Opacity(.8)
                    )
                ),

                new Grid()
                .ColumnDefinitions(e => e.Star(7).Star(3))
                .FillHorizontal()
                .Padding(10)
                .Children(
                    new Label()
                    .Text("Son Ürünler")
                    .FontAttributes(FontAttributes.Bold)
                    .FontSize(18)
                    .CenterVertical()
                    .Column(0)
                    .AlignLeft(),


                    new Label()
                    .Text("Tümünü Gör")
                    .FontSize(15)
                    .CenterVertical()
                    .Column(1)
                    .AlignRight()
                    .TextDecorations(TextDecorations.Underline)
                    .GestureRecognizers(
                        new TapGestureRecognizer()
                        .Command(BindingContext.GotoAllProductsCommand)
                    )
                ),

                new CollectionView()
                .SelectionMode(SelectionMode.None)
                .Bind(CollectionView.ItemsSourceProperty, "Products")
                .ItemsLayout(new LinearItemsLayout(ItemsLayoutOrientation.Horizontal).ItemSpacing(10))
                .EmptyView(
                    new VerticalStackLayout()
                    .Children(
                        new Label()
                        .Text("Kayıt Yoktur.")
                        .TextColor(Colors.Red)
                        .FontAttributes(FontAttributes.Bold)
                        .FontSize(18)
                    )
                    .Center()
                )
                .ItemTemplate(() =>
                    new Frame()
                    .CornerRadius(15)
                    .BorderColor(Colors.LightGray)
                    .BackgroundColor(Colors.LightGray)
                    .MinimumHeightRequest(200)
                    .MaximumWidthRequest(200)
                    .Padding(5)
                    .Content(
                        new Grid()
                        .RowDefinitions(e => e.Star(1).Star(6).Star(2).Star(1))
                        .Padding(5)
                        .Children(
                            new Grid()
                            .Row(0)
                            .ColumnDefinitions(e => e.Star(6).Star(4))
                            .Children(
                                new ImageButton()
                                .Bind(ImageButton.SourceProperty, nameof(ProductVM.IsFavorite), converter: new BoolToFavoriteImageConverter())
                                .BackgroundColor(Colors.Transparent)
                                .AlignLeft()
                                .SizeRequest(30, 30)
                                .Command(BindingContext.ChangeFavoriteCommand)
                                .Bind(ImageButton.CommandParameterProperty, "."),

                                new Frame()
                                .CornerRadius(20)
                                .HeightRequest(25)
                                .WidthRequest(50)
                                .Padding(0)
                                .BackgroundColor(Colors.Red)
                                .BorderColor(Colors.Red)
                                .Column(1)
                                .Bind(Microsoft.Maui.Controls.Frame.IsVisibleProperty, nameof(ProductVM.IsDiscount))
                                .Content(
                                    new Label()
                                    .Bind(Label.TextProperty, nameof(ProductVM.DiscountRate))
                                    .FontSize(11)
                                    .FontAttributes(FontAttributes.Bold)
                                    .TextColor(Colors.White)
                                    .Center()
                                )
                            ),

                            new Image()
                            .Bind(Image.SourceProperty, nameof(ProductVM.Image))
                            .SizeRequest(80,80)
                            .Row(1)
                            .CenterHorizontal(),

                            new VerticalStackLayout()
                            .Row(2)
                            .Children(
                                new Label()
                                .Bind(Label.TextProperty, nameof(ProductVM.Name))
                                .FontAttributes(FontAttributes.Bold)
                                .FontSize(11)
                                .AlignLeft()
                                .LineBreakMode(LineBreakMode.TailTruncation)
                                .FontAutoScalingEnabled(true),

                                new HorizontalStackLayout()
                                .Spacing(2)
                                .Children(
                                    new Label()
                                    .Bind(Label.TextProperty, nameof(ProductVM.Price))
                                    .Bind(Label.TextDecorationsProperty, nameof(ProductVM.IsDiscount), converter: new BoolToTextDecorationConverter())
                                    .Bind(Label.FontSizeProperty, nameof(ProductVM.IsDiscount), converter: new BoolToFontSizeConverter())
                                    .FontAttributes(FontAttributes.Bold)
                                    .CenterVertical(),

                                    new Label()
                                    .TextColor(Colors.Red)
                                    .FontAttributes(FontAttributes.Bold)
                                    .CenterVertical()
                                    .Bind(Label.IsVisibleProperty, nameof(ProductVM.IsDiscount))
                                    .Bind(Label.TextProperty, nameof(ProductVM.DiscountPrice)),

                                    new Label()
                                    .Text("/")
                                    .FontSize(10)
                                    .CenterVertical()
                                    .TextColor(Colors.DarkSlateGray),

                                    new Label()
                                    .FontSize(10)
                                    .CenterVertical()
                                    .TextColor(Colors.DarkSlateGray)
                                    .Bind(Label.TextProperty, nameof(ProductVM.Unit))
                                )
                            ),

                            new Button()
                            .Row(3)
                            .Margin(new Thickness(0,5,0,0))
                            .Padding(0)
                            .Text("Sepete Ekle")
                            .BackgroundColor(Colors.Green)
                            .FontSize(12)
                            .FontAttributes(FontAttributes.Bold)
                            .CenterHorizontal()
                            .HeightRequest(35)
                            .WidthRequest(100)
                            .Command(BindingContext.AddProductBasketCommand)
                            .Bind(Button.CommandParameterProperty, ".")
                        )
                    )
                ),

                new Grid()
                .ColumnDefinitions(e => e.Star(7).Star(3))
                .FillHorizontal()
                .Padding(10)
                .Children(
                    new Label()
                    .Text("Kategoriler")
                    .FontAttributes(FontAttributes.Bold)
                    .FontSize(18)
                    .CenterVertical()
                    .Column(0)
                    .AlignLeft(),


                    new Label()
                    .Text("Tümünü Gör")
                    .FontSize(15)
                    .CenterVertical()
                    .Column(1)
                    .AlignRight()
                    .TextDecorations(TextDecorations.Underline)
                    .GestureRecognizers(
                        new TapGestureRecognizer()
                        .Command(BindingContext.GotoAllCategoriesCommand)
                    )
                ),

                new FlexLayout()
                .ItemsSources(BindingContext.Categories)
                .Assign(out var flex)
                .Wrap(FlexWrap.Wrap)
                .FlexBasis(FlexBasis.Auto)
                .ItemTemplates(new DataTemplate(() => 
                    new Frame()
                    .CornerRadius(15)
                    .BorderColor(Colors.LightGray)
                    .BackgroundColor(Colors.LightGray)
                    .MinimumHeightRequest(30)
                    .WidthRequest(180)
                    .Padding(0)
                    .Margin(new Thickness(1,0,5,5))
                    .FlexBasis(FlexBasis.Auto)
                    .Content(
                        new Grid()
                        .ColumnDefinitions(e => e.Star(3).Star(7))
                        .Padding(5)
                        .Children(
                            new Image()
                            .Bind(Image.SourceProperty, nameof(SubCategoryVM.Icon))
                            .SizeRequest(30,30)
                            .Column(0)
                            .CenterVertical(),

                            new Label()
                            .Bind(Label.TextProperty, nameof(SubCategoryVM.Name))
                            .TextColor(Colors.CornflowerBlue)
                            .FontAttributes(FontAttributes.Bold)
                            .FontSize(12)
                            .Column(1)
                            .FontAutoScalingEnabled(true)
                            .CenterVertical()
                        )
                    )
                ))
            )
            .FillHorizontal()
        );
    }
}

```

</details>
