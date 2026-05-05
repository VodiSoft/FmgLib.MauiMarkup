namespace FmgLib.MauiMarkup;

public class RowDefinitionBuilder
{
    public List<RowDefinition> Items { get; } = new List<RowDefinition>();


    /// <summary>
    /// Executes the <c>Auto</c> operation.
    /// </summary>
    /// <param name="count">The value used for <paramref name="count"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public RowDefinitionBuilder Auto(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Items.Add(new RowDefinition(new GridLength(0.0, GridUnitType.Auto)));
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>Star</c> operation.
    /// </summary>
    /// <param name="height">The value used for <paramref name="height"/>.</param>
    /// <param name="count">The value used for <paramref name="count"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public RowDefinitionBuilder Star(double height = 1.0, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Items.Add(new RowDefinition(new GridLength(height, GridUnitType.Star)));
        }

        return this;
    }

    /// <summary>
    /// Executes the <c>Absolute</c> operation.
    /// </summary>
    /// <param name="height">The value used for <paramref name="height"/>.</param>
    /// <param name="count">The value used for <paramref name="count"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public RowDefinitionBuilder Absolute(double height, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Items.Add(new RowDefinition(new GridLength(height, GridUnitType.Absolute)));
        }

        return this;
    }
}
