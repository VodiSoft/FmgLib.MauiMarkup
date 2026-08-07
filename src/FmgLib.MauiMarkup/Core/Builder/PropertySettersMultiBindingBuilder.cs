#nullable enable

namespace FmgLib.MauiMarkup;

/// <summary>
/// Multi binding builder for a style setter, opened with <c>Bindings(...)</c>. Every child is a ready made
/// <see cref="BindingBase"/>; the values they produce are combined either by a converter of your own or by
/// the fluent <c>MultiConvert</c> family.
/// </summary>
public sealed class PropertySettersMultiBindingBuilder<T> : IPropertySettersBuilder<T>
{
    private readonly List<BindingEntry> _entries = new();

    private readonly MultiBindingSpec<T> _multi = new();

    public PropertySettersContext<T> Context { get; set; }

    /// <summary>
    /// Initializes a new instance of the <c>PropertySettersMultiBindingBuilder</c> class.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    public PropertySettersMultiBindingBuilder(PropertySettersContext<T> context)
    {
        Context = context;
    }

    /// <summary>
    /// Builds the configuration for the <c>Build</c> operation.
    /// </summary>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    public bool Build()
    {
        var binding = FluentBindingFactory.Create(_entries, _multi, null, Context.Property);

        if (binding is null)
            return false;

        Context.XamlSetters.Add(new Setter
        {
            Property = Context.Property,
            Value = binding
        });

        return true;
    }


    /// <summary>
    /// Adds child bindings, in the order they are combined.
    /// </summary>
    /// <param name="bindings">The value used for <paramref name="bindings"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersMultiBindingBuilder<T> Bindings(params BindingBase[] bindings)
    {
        foreach (var binding in bindings)
            _entries.Add(new RawBindingEntry(binding));

        return this;
    }

    /// <summary>
    /// Formats the child values positionally (<c>{0}</c>, <c>{1}</c>, …), which combines them without a
    /// converter.
    /// </summary>
    /// <param name="stringFormat">The value used for <paramref name="stringFormat"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersMultiBindingBuilder<T> StringFormat(string stringFormat)
    {
        _multi.StringFormat = stringFormat;
        return this;
    }

    /// <summary>
    /// Mode of the multi binding.
    /// </summary>
    /// <param name="bindingMode">The value used for <paramref name="bindingMode"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersMultiBindingBuilder<T> BindingMode(BindingMode bindingMode)
    {
        _multi.Mode = bindingMode;
        return this;
    }

    /// <summary>
    /// Combines the child values with a hand written converter.
    /// </summary>
    /// <param name="converter">The value used for <paramref name="converter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersMultiBindingBuilder<T> Converter(IMultiValueConverter converter)
    {
        _multi.SetUserConverter(converter);
        return this;
    }

    /// <summary>
    /// <c>ConverterParameter</c> of the multi binding.
    /// </summary>
    /// <param name="converterParameter">The value used for <paramref name="converterParameter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersMultiBindingBuilder<T> Parameter(string converterParameter)
    {
        _multi.ConverterParameter = converterParameter;
        return this;
    }

    /// <summary>
    /// <c>FallbackValue</c> of the multi binding.
    /// </summary>
    /// <param name="fallbackValue">The value used for <paramref name="fallbackValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersMultiBindingBuilder<T> FallbackValue(object fallbackValue)
    {
        _multi.FallbackValue = fallbackValue;
        return this;
    }

    /// <summary>
    /// <c>TargetNullValue</c> of the multi binding.
    /// </summary>
    /// <param name="targetNullValue">The value used for <paramref name="targetNullValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersMultiBindingBuilder<T> TargetNullValue(object targetNullValue)
    {
        _multi.TargetNullValue = targetNullValue;
        return this;
    }

    /// <summary>
    /// Combines the sub binding values into the property value, in declaration order.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersMultiBindingBuilder<T> MultiConvert<Q1, Q2>(Func<Q1, Q2, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvert<Q1, Q2, Q3>(Func<Q1, Q2, Q3, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4>(Func<Q1, Q2, Q3, Q4, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5>(Func<Q1, Q2, Q3, Q4, Q5, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5, Q6>(Func<Q1, Q2, Q3, Q4, Q5, Q6, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5, Q6, Q7>(Func<Q1, Q2, Q3, Q4, Q5, Q6, Q7, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8>(Func<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9>(Func<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <summary>
    /// Reverse of <c>MultiConvert</c>. The returned tuple is written back in declaration order.
    /// </summary>
    /// <param name="convertBack">The value used for <paramref name="convertBack"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersMultiBindingBuilder<T> MultiConvertBack<Q1, Q2>(Func<T, (Q1, Q2)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3>(Func<T, (Q1, Q2, Q3)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4>(Func<T, (Q1, Q2, Q3, Q4)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5>(Func<T, (Q1, Q2, Q3, Q4, Q5)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5, Q6>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5, Q6, Q7>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6, Q7)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersMultiBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <summary>
    /// Combines an arbitrary number of sub bindings whose values share one type.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersMultiBindingBuilder<T> MultiConvertRaw<Q>(Func<IReadOnlyList<Q>, T> convert)
    {
        _multi.UseRaw(convert);
        return this;
    }

    /// <summary>
    /// Combines an arbitrary number of sub bindings of mixed types, boxed and in declaration order.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <param name="convertBack">The value used for <paramref name="convertBack"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersMultiBindingBuilder<T> MultiConvertRaw(Func<object?[], T> convert, Func<T, object?[]>? convertBack = null)
    {
        _multi.UseRaw(convert, convertBack);
        return this;
    }
}
