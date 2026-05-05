namespace FmgLib.MauiMarkup;

public class ColumnDefinitionBuilder
{
    public List<ColumnDefinition> Items { get; } = new List<ColumnDefinition>();


    /// <summary>
    /// Executes the <c>Auto</c> operation.
    /// </summary>
    /// <param name="count">The value used for <paramref name="count"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public ColumnDefinitionBuilder Auto(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Items.Add(new ColumnDefinition(new GridLength(0.0, GridUnitType.Auto)));
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>Star</c> operation.
    /// </summary>
    /// <param name="width">The value used for <paramref name="width"/>.</param>
    /// <param name="count">The value used for <paramref name="count"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public ColumnDefinitionBuilder Star(double width = 1.0, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Items.Add(new ColumnDefinition(new GridLength(width, GridUnitType.Star)));
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>Absolute</c> operation.
    /// </summary>
    /// <param name="width">The value used for <paramref name="width"/>.</param>
    /// <param name="count">The value used for <paramref name="count"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public ColumnDefinitionBuilder Absolute(double width, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Items.Add(new ColumnDefinition(new GridLength(width, GridUnitType.Absolute)));
        }

        return this;
    }
}
