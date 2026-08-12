namespace FmgLib.MauiMarkup.Gallery.Controls;

/// <summary>
/// Turns an entry red while its text is not a number. The canonical behavior: reusable control logic
/// with no subclassing and no code in the page.
/// </summary>
public sealed class NumericValidationBehavior : Behavior<Entry>
{
    /// <inheritdoc/>
    protected override void OnAttachedTo(Entry entry)
    {
        entry.TextChanged += OnTextChanged;
        base.OnAttachedTo(entry);
    }

    /// <inheritdoc/>
    protected override void OnDetachingFrom(Entry entry)
    {
        entry.TextChanged -= OnTextChanged;
        base.OnDetachingFrom(entry);
    }

    private static void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry)
            return;

        var isEmpty = string.IsNullOrWhiteSpace(e.NewTextValue);
        var isValid = isEmpty || double.TryParse(e.NewTextValue, out _);

        entry.TextColor = isValid ? AppColors.Success : AppColors.Danger;
    }
}

/// <summary>
/// A behavior with its own bindable property — so it can be configured, bound, and (once opted into
/// the source generator) configured fluently like any other control.
/// </summary>
public sealed class MinLengthBehavior : Behavior<Entry>
{
    public static readonly BindableProperty MinLengthProperty =
        BindableProperty.Create(nameof(MinLength), typeof(int), typeof(MinLengthBehavior), 6);

    /// <summary>Characters required before the entry is considered satisfied.</summary>
    public int MinLength
    {
        get => (int)GetValue(MinLengthProperty);
        set => SetValue(MinLengthProperty, value);
    }

    /// <summary>Raised whenever the satisfied state changes, so a page can react without polling.</summary>
    public event EventHandler<bool>? ValidityChanged;

    /// <inheritdoc/>
    protected override void OnAttachedTo(Entry entry)
    {
        entry.TextChanged += OnTextChanged;
        base.OnAttachedTo(entry);
    }

    /// <inheritdoc/>
    protected override void OnDetachingFrom(Entry entry)
    {
        entry.TextChanged -= OnTextChanged;
        base.OnDetachingFrom(entry);
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
        => ValidityChanged?.Invoke(this, (e.NewTextValue?.Length ?? 0) >= MinLength);
}

/// <summary>
/// A trigger action for the EventTrigger demo. Unlike a behavior it has no attach/detach lifecycle —
/// it just runs when the event it is wired to fires.
/// </summary>
public sealed class UppercaseTriggerAction : TriggerAction<Entry>
{
    /// <inheritdoc/>
    protected override void Invoke(Entry entry)
    {
        var upper = entry.Text?.ToUpperInvariant();

        if (entry.Text != upper)
            entry.Text = upper;
    }
}
