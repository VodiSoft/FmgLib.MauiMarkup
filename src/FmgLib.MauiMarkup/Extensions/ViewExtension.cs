using Microsoft.Maui.Layouts;

namespace FmgLib.MauiMarkup;

public static partial class ViewExtension
{
    public static T GridSpan<T>(this T self, int column = 1, int row = 1) where T : View
    {
        self.SetValue(Grid.ColumnSpanProperty, column);
        self.SetValue(Grid.RowSpanProperty, row);
        return self;
    }

    public static T Padding<T>(this T self, double horizontalSize, double verticalSize) where T : Border
    {
        self.SetValue(Border.PaddingProperty, new Thickness(horizontalSize, verticalSize));
        return self;
    }

    public static T Padding<T>(this T self, double left, double top, double right, double bottom) where T : Border
    {
        self.SetValue(Border.PaddingProperty, new Thickness(left, top, right, bottom));
        return self;
    }

    public static T Padding<T>(this T self, object _ = null, double left = 0.0, double top = 0.0, double right = 0.0, double bottom = 0.0) where T : Border
    {
        self.SetValue(Border.PaddingProperty, new Thickness(left, top, right, bottom));
        return self;
    }

    public static T Margin<T>(this T self, double horizontalSize, double verticalSize) where T : View
    {
        self.SetValue(View.MarginProperty, new Thickness(horizontalSize, verticalSize));
        return self;
    }

    public static T Margin<T>(this T self, double left, double top, double right, double bottom) where T : View
    {
        self.SetValue(View.MarginProperty, new Thickness(left, top, right, bottom));
        return self;
    }

    public static T Margin<T>(this T self, object _ = null, double left = 0.0, double top = 0.0, double right = 0.0, double bottom = 0.0) where T : View
    {
        self.SetValue(View.MarginProperty, new Thickness(left, top, right, bottom));
        return self;
    }

    public static T AbsoluteLayoutBounds<T>(this T self, double x, double y, double width, double height) where T : View
    {
        self.SetValue(AbsoluteLayout.LayoutBoundsProperty, new Rect(x, y, width, height));
        return self;
    }

    public static T CenterHorizontal<T>(this T self) where T : View
    {
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Center);
        return self;
    }

    public static T CenterVertical<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Center);
        return self;
    }

    public static T Center<T>(this T self) where T : View
    {
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Center);
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Center);
        return self;
    }

    public static T AlignTop<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Start);
        return self;
    }

    public static T AlignTopLeft<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Start);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Start);
        return self;
    }

    public static T AlignTopRight<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Start);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.End);
        return self;
    }

    public static T AlignTopCenter<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Start);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Center);
        return self;
    }

    public static T AlignTopFill<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Start);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Fill);
        return self;
    }

    public static T AlignBottom<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.End);
        return self;
    }

    public static T AlignBottomLeft<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.End);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Start);
        return self;
    }

    public static T AlignBottomRight<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.End);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.End);
        return self;
    }

    public static T AlignBottomCenter<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.End);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Center);
        return self;
    }

    public static T AlignBottomFill<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.End);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Fill);
        return self;
    }

    public static T AlignCenterLeft<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Center);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Start);
        return self;
    }

    public static T AlignCenterRight<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Center);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.End);
        return self;
    }

    public static T AlignCenterFill<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Center);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Fill);
        return self;
    }

    public static T AlignFillLeft<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Fill);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Start);
        return self;
    }

    public static T AlignFillRight<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Fill);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.End);
        return self;
    }

    public static T AlignFillCenter<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Fill);
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Center);
        return self;
    }

    public static T AlignLeft<T>(this T self) where T : View
    {
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Start);
        return self;
    }

    public static T AlignRight<T>(this T self) where T : View
    {
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.End);
        return self;
    }

    public static T FillHorizontal<T>(this T self) where T : View
    {
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Fill);
        return self;
    }

    public static T FillVertical<T>(this T self) where T : View
    {
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Fill);
        return self;
    }

    public static T FillBothDirections<T>(this T self) where T : View
    {
        self.SetValue(View.HorizontalOptionsProperty, LayoutOptions.Fill);
        self.SetValue(View.VerticalOptionsProperty, LayoutOptions.Fill);
        return self;
    }

    public static T AlignLayout<T>(this T self, LayoutOptions vertical, LayoutOptions horizontal) where T : View
    {
        self.SetValue(View.HorizontalOptionsProperty, horizontal);
        self.SetValue(View.VerticalOptionsProperty, vertical);
        return self;
    }

    public static SettersContext<T> GridSpan<T>(this SettersContext<T> self, int column = 1, int row = 1) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = Grid.ColumnSpanProperty,
            Value = column
        });
        self.XamlSetters.Add(new Setter
        {
            Property = Grid.RowSpanProperty,
            Value = row
        });
        return self;
    }

    public static SettersContext<T> Padding<T>(this SettersContext<T> self, double horizontalSize, double verticalSize) where T : Border
    {
        self.XamlSetters.Add(new Setter
        {
            Property = Border.PaddingProperty,
            Value = new Thickness(horizontalSize, verticalSize)
        });
        return self;
    }

    public static SettersContext<T> Padding<T>(this SettersContext<T> self, double left, double top, double right, double bottom) where T : Border
    {
        self.XamlSetters.Add(new Setter
        {
            Property = Border.PaddingProperty,
            Value = new Thickness(left, top, right, bottom)
        });
        return self;
    }

    public static SettersContext<T> Padding<T>(this SettersContext<T> self, object _ = null, double left = 0.0, double top = 0.0, double right = 0.0, double bottom = 0.0) where T : Border
    {
        self.XamlSetters.Add(new Setter
        {
            Property = Border.PaddingProperty,
            Value = new Thickness(left, top, right, bottom)
        });
        return self;
    }

    public static SettersContext<T> Margin<T>(this SettersContext<T> self, double horizontalSize, double verticalSize) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.MarginProperty,
            Value = new Thickness(horizontalSize, verticalSize)
        });
        return self;
    }

    public static SettersContext<T> Margin<T>(this SettersContext<T> self, double left, double top, double right, double bottom) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.MarginProperty,
            Value = new Thickness(left, top, right, bottom)
        });
        return self;
    }

    public static SettersContext<T> Margin<T>(this SettersContext<T> self, object _ = null, double left = 0.0, double top = 0.0, double right = 0.0, double bottom = 0.0) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.MarginProperty,
            Value = new Thickness(left, top, right, bottom)
        });
        return self;
    }

    public static SettersContext<T> AbsoluteLayoutBounds<T>(this SettersContext<T> self, double x, double y, double width, double height) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = AbsoluteLayout.LayoutBoundsProperty,
            Value = new Rect(x, y, width, height)
        });
        return self;
    }

    public static SettersContext<T> CenterHorizontal<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Center
        });
        return self;
    }

    public static SettersContext<T> CenterVertical<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Center
        });
        return self;
    }

    public static SettersContext<T> Center<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Center
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Center
        });
        return self;
    }

    public static SettersContext<T> AlignTop<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Start
        });
        return self;
    }

    public static SettersContext<T> AlignTopLeft<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Start
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Start
        });
        return self;
    }

    public static SettersContext<T> AlignTopRight<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Start
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.End
        });
        return self;
    }

    public static SettersContext<T> AlignBottom<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.End
        });
        return self;
    }

    public static SettersContext<T> AlignBottomLeft<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.End
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Start
        });
        return self;
    }

    public static SettersContext<T> AlignBottomRight<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.End
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.End
        });
        return self;
    }

    public static SettersContext<T> AlignLeft<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Start
        });
        return self;
    }

    public static SettersContext<T> AlignRight<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.End
        });
        return self;
    }

    public static SettersContext<T> AlignTopCenter<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Start
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Center
        });
        return self;
    }

    public static SettersContext<T> AlignTopFill<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Start
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Fill
        });
        return self;
    }

    public static SettersContext<T> AlignBottomCenter<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.End
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Center
        });
        return self;
    }

    public static SettersContext<T> AlignBottomFill<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.End
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Fill
        });
        return self;
    }

    public static SettersContext<T> AlignCenterLeft<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Center
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Start
        });
        return self;
    }

    public static SettersContext<T> AlignCenterRight<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Center
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.End
        });
        return self;
    }

    public static SettersContext<T> AlignCenterFill<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Center
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Fill
        });
        return self;
    }

    public static SettersContext<T> AlignFillLeft<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Fill
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Start
        });
        return self;
    }

    public static SettersContext<T> AlignFillRight<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Fill
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.End
        });
        return self;
    }

    public static SettersContext<T> AlignFillCenter<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Fill
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Center
        });
        return self;
    }

    public static SettersContext<T> FillHorizontal<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Fill
        });
        return self;
    }

    public static SettersContext<T> FillVertical<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Fill
        });
        return self;
    }

    public static SettersContext<T> FillBothDirections<T>(this SettersContext<T> self) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = LayoutOptions.Fill
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = LayoutOptions.Fill
        });
        return self;
    }

    public static SettersContext<T> AlignLayout<T>(this SettersContext<T> self, LayoutOptions vertical, LayoutOptions horizontal) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = View.HorizontalOptionsProperty,
            Value = horizontal
        });
        self.XamlSetters.Add(new Setter
        {
            Property = View.VerticalOptionsProperty,
            Value = vertical
        });
        return self;
    }

    public static T Column<T>(this T self, int column) where T : View
    {
        self.SetValue(Grid.ColumnProperty, column);
        return self;
    }

    public static T Column<T>(this T self, Func<PropertyContext<int>, IPropertyBuilder<int>> configure) where T : View
    {
        PropertyContext<int> arg = new PropertyContext<int>(self, Grid.ColumnProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> Column<T>(this SettersContext<T> self, int column) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = Grid.ColumnProperty,
            Value = column
        });
        return self;
    }

    public static SettersContext<T> Column<T>(this SettersContext<T> self, Func<PropertySettersContext<int>, IPropertySettersBuilder<int>> configure) where T : View
    {
        PropertySettersContext<int> arg = new PropertySettersContext<int>(self.XamlSetters, Grid.ColumnProperty);
        configure(arg).Build();
        return self;
    }

    public static int GetColumnValue<T>(this T self) where T : View
    {
        return (int)self.GetValue(Grid.ColumnProperty);
    }

    public static T Row<T>(this T self, int row) where T : View
    {
        self.SetValue(Grid.RowProperty, row);
        return self;
    }

    public static T Row<T>(this T self, Func<PropertyContext<int>, IPropertyBuilder<int>> configure) where T : View
    {
        PropertyContext<int> arg = new PropertyContext<int>(self, Grid.RowProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> Row<T>(this SettersContext<T> self, int row) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = Grid.RowProperty,
            Value = row
        });
        return self;
    }

    public static SettersContext<T> Row<T>(this SettersContext<T> self, Func<PropertySettersContext<int>, IPropertySettersBuilder<int>> configure) where T : View
    {
        PropertySettersContext<int> arg = new PropertySettersContext<int>(self.XamlSetters, Grid.RowProperty);
        configure(arg).Build();
        return self;
    }

    public static int GetRowValue<T>(this T self) where T : View
    {
        return (int)self.GetValue(Grid.RowProperty);
    }

    public static T ColumnSpan<T>(this T self, int columnSpan) where T : View
    {
        self.SetValue(Grid.ColumnSpanProperty, columnSpan);
        return self;
    }

    public static T ColumnSpan<T>(this T self, Func<PropertyContext<int>, IPropertyBuilder<int>> configure) where T : View
    {
        PropertyContext<int> arg = new PropertyContext<int>(self, Grid.ColumnSpanProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> ColumnSpan<T>(this SettersContext<T> self, int columnSpan) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = Grid.ColumnSpanProperty,
            Value = columnSpan
        });
        return self;
    }

    public static SettersContext<T> ColumnSpan<T>(this SettersContext<T> self, Func<PropertySettersContext<int>, IPropertySettersBuilder<int>> configure) where T : View
    {
        PropertySettersContext<int> arg = new PropertySettersContext<int>(self.XamlSetters, Grid.ColumnSpanProperty);
        configure(arg).Build();
        return self;
    }

    public static int GetColumnSpanValue<T>(this T self) where T : View
    {
        return (int)self.GetValue(Grid.ColumnSpanProperty);
    }

    public static T RowSpan<T>(this T self, int rowSpan) where T : View
    {
        self.SetValue(Grid.RowSpanProperty, rowSpan);
        return self;
    }

    public static T RowSpan<T>(this T self, Func<PropertyContext<int>, IPropertyBuilder<int>> configure) where T : View
    {
        PropertyContext<int> arg = new PropertyContext<int>(self, Grid.RowSpanProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> RowSpan<T>(this SettersContext<T> self, int rowSpan) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = Grid.RowSpanProperty,
            Value = rowSpan
        });
        return self;
    }

    public static SettersContext<T> RowSpan<T>(this SettersContext<T> self, Func<PropertySettersContext<int>, IPropertySettersBuilder<int>> configure) where T : View
    {
        PropertySettersContext<int> arg = new PropertySettersContext<int>(self.XamlSetters, Grid.RowSpanProperty);
        configure(arg).Build();
        return self;
    }

    public static int GetRowSpanValue<T>(this T self) where T : View
    {
        return (int)self.GetValue(Grid.RowSpanProperty);
    }

    public static T AbsoluteLayoutFlags<T>(this T self, AbsoluteLayoutFlags absoluteLayoutFlags) where T : View
    {
        self.SetValue(AbsoluteLayout.LayoutFlagsProperty, absoluteLayoutFlags);
        return self;
    }

    public static T AbsoluteLayoutFlags<T>(this T self, Func<PropertyContext<AbsoluteLayoutFlags>, IPropertyBuilder<AbsoluteLayoutFlags>> configure) where T : View
    {
        PropertyContext<AbsoluteLayoutFlags> arg = new PropertyContext<AbsoluteLayoutFlags>(self, AbsoluteLayout.LayoutFlagsProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> AbsoluteLayoutFlags<T>(this SettersContext<T> self, AbsoluteLayoutFlags absoluteLayoutFlags) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = AbsoluteLayout.LayoutFlagsProperty,
            Value = absoluteLayoutFlags
        });
        return self;
    }

    public static SettersContext<T> AbsoluteLayoutFlags<T>(this SettersContext<T> self, Func<PropertySettersContext<AbsoluteLayoutFlags>, IPropertySettersBuilder<AbsoluteLayoutFlags>> configure) where T : View
    {
        PropertySettersContext<AbsoluteLayoutFlags> arg = new PropertySettersContext<AbsoluteLayoutFlags>(self.XamlSetters, AbsoluteLayout.LayoutFlagsProperty);
        configure(arg).Build();
        return self;
    }

    public static AbsoluteLayoutFlags GetAbsoluteLayoutFlagsValue<T>(this T self) where T : View
    {
        return (AbsoluteLayoutFlags)self.GetValue(AbsoluteLayout.LayoutFlagsProperty);
    }

    public static T AbsoluteLayoutBounds<T>(this T self, Rect absoluteLayoutBounds) where T : View
    {
        self.SetValue(AbsoluteLayout.LayoutBoundsProperty, absoluteLayoutBounds);
        return self;
    }

    public static T AbsoluteLayoutBounds<T>(this T self, Func<PropertyContext<Rect>, IPropertyBuilder<Rect>> configure) where T : View
    {
        PropertyContext<Rect> arg = new PropertyContext<Rect>(self, AbsoluteLayout.LayoutBoundsProperty);
        configure(arg).Build();
        return self;
    }

    public static SettersContext<T> AbsoluteLayoutBounds<T>(this SettersContext<T> self, Rect absoluteLayoutBounds) where T : View
    {
        self.XamlSetters.Add(new Setter
        {
            Property = AbsoluteLayout.LayoutBoundsProperty,
            Value = absoluteLayoutBounds
        });
        return self;
    }

    public static SettersContext<T> AbsoluteLayoutBounds<T>(this SettersContext<T> self, Func<PropertySettersContext<Rect>, IPropertySettersBuilder<Rect>> configure) where T : View
    {
        PropertySettersContext<Rect> arg = new PropertySettersContext<Rect>(self.XamlSetters, AbsoluteLayout.LayoutBoundsProperty);
        configure(arg).Build();
        return self;
    }

    public static Rect GetAbsoluteLayoutBoundsValue<T>(this T self) where T : View
    {
        return (Rect)self.GetValue(AbsoluteLayout.LayoutBoundsProperty);
    }

}
