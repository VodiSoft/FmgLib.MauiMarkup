namespace FmgLib.MauiMarkup;

public static partial class GridExtension
{
    public static T RowDefinitions<T>(this T self, Action<RowDefinitionBuilder> configure) where T : Grid
    {
        RowDefinitionBuilder rowDefinitionBuilder = new RowDefinitionBuilder();
        configure(rowDefinitionBuilder);
        foreach (RowDefinition ıtem in rowDefinitionBuilder.Items)
        {
            self.RowDefinitions.Add(ıtem);
        }

        return self;
    }

    public static T ColumnDefinitions<T>(this T self, Action<ColumnDefinitionBuilder> configure) where T : Grid
    {
        ColumnDefinitionBuilder columnDefinitionBuilder = new ColumnDefinitionBuilder();
        configure(columnDefinitionBuilder);
        foreach (ColumnDefinition ıtem in columnDefinitionBuilder.Items)
        {
            self.ColumnDefinitions.Add(ıtem);
        }

        return self;
    }

    public static T Spacing<T>(this T self, double columnSpacing, double rowSpacing) where T : Grid
    {
        self.SetValue(Grid.ColumnSpacingProperty, columnSpacing);
        self.SetValue(Grid.RowSpacingProperty, rowSpacing);
        return self;
    }

    public static T Spacing<T>(this T self, double spacing) where T : Grid
    {
        self.SetValue(Grid.ColumnSpacingProperty, spacing);
        self.SetValue(Grid.RowSpacingProperty, spacing);
        return self;
    }

    public static T ColumnDefinitions<T>(this T self, IList<ColumnDefinition> columnDefinitions) where T : Grid
    {
        foreach (ColumnDefinition columnDefinition in columnDefinitions)
        {
            self.ColumnDefinitions.Add(columnDefinition);
        }

        return self;
    }

    public static T ColumnDefinitions<T>(this T self, params ColumnDefinition[] columnDefinitions) where T : Grid
    {
        foreach (ColumnDefinition item in columnDefinitions)
        {
            self.ColumnDefinitions.Add(item);
        }

        return self;
    }

    public static T RowDefinitions<T>(this T self, IList<RowDefinition> rowDefinitions) where T : Grid
    {
        foreach (RowDefinition rowDefinition in rowDefinitions)
        {
            self.RowDefinitions.Add(rowDefinition);
        }

        return self;
    }

    public static T RowDefinitions<T>(this T self, params RowDefinition[] rowDefinitions) where T : Grid
    {
        foreach (RowDefinition item in rowDefinitions)
        {
            self.RowDefinitions.Add(item);
        }

        return self;
    }

}