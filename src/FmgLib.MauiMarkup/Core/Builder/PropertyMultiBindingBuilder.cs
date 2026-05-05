namespace FmgLib.MauiMarkup;

public sealed class PropertyMultiBindingBuilder<T> : IPropertyBuilder<T>
{
    private BindingMode bindingMode;

    private IMultiValueConverter converter;

    private string converterParameter;

    private string stringFormat;

    private IList<BindingBase> bindings;

    private object fallbackValue;

    private object targetNullValue;

    public PropertyContext<T> Context { get; set; }

    /// <summary>
    /// Initializes a new instance of the <c>PropertyMultiBindingBuilder</c> class.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    public PropertyMultiBindingBuilder(PropertyContext<T> context)
    {
        Context = context;
        bindings = new List<BindingBase>();
    }

    /// <summary>
    /// Builds the configuration for the <c>Build</c> operation.
    /// </summary>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    public bool Build()
    {
        if (bindings != null && bindings.Count > 0)
        {
            Context.BindableObject.SetBinding(Context.Property, new MultiBinding()
            {
                Bindings = bindings,
                Converter = converter,
                ConverterParameter = converterParameter,
                Mode = bindingMode,
                StringFormat = stringFormat,
                FallbackValue = fallbackValue,
                TargetNullValue = targetNullValue
            });
            return true;
        }

        return false;
    }

    /// <summary>
    /// Executes the <c>Bindings</c> operation.
    /// </summary>
    /// <param name="bindings">The value used for <paramref name="bindings"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyMultiBindingBuilder<T> Bindings(params BindingBase[] bindings)
    {
        this.bindings ??= new List<BindingBase>();
        foreach (var binding in bindings)
            this.bindings.Add(binding);
        return this;
    }

    /// <summary>
    /// Executes the <c>StringFormat</c> operation.
    /// </summary>
    /// <param name="stringFormat">The value used for <paramref name="stringFormat"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyMultiBindingBuilder<T> StringFormat(string stringFormat)
    {
        this.stringFormat = stringFormat;
        return this;
    }

    /// <summary>
    /// Executes the <c>BindingMode</c> operation.
    /// </summary>
    /// <param name="bindingMode">The value used for <paramref name="bindingMode"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyMultiBindingBuilder<T> BindingMode(BindingMode bindingMode)
    {
        this.bindingMode = bindingMode;
        return this;
    }

    /// <summary>
    /// Executes the <c>Converter</c> operation.
    /// </summary>
    /// <param name="converter">The value used for <paramref name="converter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyMultiBindingBuilder<T> Converter(IMultiValueConverter converter)
    {
        this.converter = converter;
        return this;
    }

    /// <summary>
    /// Executes the <c>Parameter</c> operation.
    /// </summary>
    /// <param name="converterParameter">The value used for <paramref name="converterParameter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyMultiBindingBuilder<T> Parameter(string converterParameter)
    {
        this.converterParameter = converterParameter;
        return this;
    }

    /// <summary>
    /// Executes the <c>FallbackValue</c> operation.
    /// </summary>
    /// <param name="fallbackValue">The value used for <paramref name="fallbackValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyMultiBindingBuilder<T> FallbackValue(object fallbackValue)
    {
        this.fallbackValue = fallbackValue;
        return this;
    }

    /// <summary>
    /// Executes the <c>TargetNullValue</c> operation.
    /// </summary>
    /// <param name="targetNullValue">The value used for <paramref name="targetNullValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyMultiBindingBuilder<T> TargetNullValue(object targetNullValue)
    {
        this.targetNullValue = targetNullValue;
        return this;
    }
}
