// Portions of this file incorporate and extend code originally from Sharp.UI.
// Copyright (c) 2022 Pawel Krzywdzinski
// Licensed under the MIT License. See THIRD-PARTY-NOTICES for details.

#nullable enable

using System.Globalization;
using System.Linq.Expressions;

namespace FmgLib.MauiMarkup;

/// <summary>
/// Fluent binding builder for a style setter. It mirrors <see cref="PropertyBindingBuilder{T}"/>: every
/// <c>Path()</c> or <c>Getter()</c> opens a sub binding, and several sub bindings combined by a closing
/// <c>MultiConvert()</c> produce a <see cref="MultiBinding"/> stored in the setter.
/// </summary>
public sealed class PropertySettersBindingBuilder<T> : IPropertySettersBuilder<T>
{
    /// <summary>
    /// Converter of a single sub binding, driven by the <c>Convert()</c> / <c>ConvertBack()</c> delegates.
    /// A direction without a delegate passes the value through untouched.
    /// </summary>
    public class ValueConverter : IValueConverter
    {
        internal Func<object?, object?>? ConvertFunction;

        internal Func<object?, object?>? ConvertBackFunction;

        /// <summary>
        /// Executes the <c>Convert</c> operation.
        /// </summary>
        /// <param name="value">The value used for <paramref name="value"/>.</param>
        /// <param name="targetType">The value used for <paramref name="targetType"/>.</param>
        /// <param name="parameter">The value used for <paramref name="parameter"/>.</param>
        /// <param name="culture">The value used for <paramref name="culture"/>.</param>
        /// <returns>The result produced by the operation.</returns>
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
            => ConvertFunction is null ? value : ConvertFunction(value);

        /// <summary>
        /// Executes the <c>ConvertBack</c> operation.
        /// </summary>
        /// <param name="value">The value used for <paramref name="value"/>.</param>
        /// <param name="targetType">The value used for <paramref name="targetType"/>.</param>
        /// <param name="parameter">The value used for <paramref name="parameter"/>.</param>
        /// <param name="culture">The value used for <paramref name="culture"/>.</param>
        /// <returns>The result produced by the operation.</returns>
        public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
            => ConvertBackFunction is null ? value : ConvertBackFunction(value);
    }

    private readonly List<BindingEntry> _entries = new();

    private readonly MultiBindingSpec<T> _multi = new();

    private BindingEntry? _current;

    public PropertySettersContext<T> Context { get; set; }

    /// <summary>
    /// Initializes a new instance of the <c>PropertySettersBindingBuilder</c> class.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    public PropertySettersBindingBuilder(PropertySettersContext<T> context)
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

    // ---- sub bindings --------------------------------------------------------------------------

    /// <summary>
    /// Opens a sub binding on <paramref name="path"/>, resolved at runtime.
    /// </summary>
    /// <param name="path">The value used for <paramref name="path"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Path(string path)
        => Open(new PathBindingEntry { Path = path });

    /// <summary>
    /// Opens a compiled sub binding that produces <typeparamref name="TValue"/>.
    /// </summary>
    /// <param name="getter">The value used for <paramref name="getter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Getter<TContext, TValue>(Expression<Func<TContext, TValue>> getter)
        => Open(BindingEntryFactory.Typed<TContext, TValue>(getter));

    /// <summary>
    /// Supplies the reverse operation of the current compiled sub binding, enabling two way updates.
    /// </summary>
    /// <param name="setter">The value used for <paramref name="setter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Setter<TContext, TValue>(Action<TContext, TValue> setter)
    {
        BindingEntryFactory.AttachSetter(Current(), setter);
        return this;
    }

    /// <summary>
    /// Adds ready made bindings as sub bindings, so hand written and fluent sub bindings can be mixed
    /// inside the same multi binding.
    /// </summary>
    /// <param name="bindings">The value used for <paramref name="bindings"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Bindings(params BindingBase[] bindings)
    {
        foreach (var binding in bindings)
            Open(new RawBindingEntry(binding));

        return this;
    }

    // ---- current sub binding -------------------------------------------------------------------

    /// <summary>
    /// Executes the <c>StringFormat</c> operation.
    /// </summary>
    /// <param name="stringFormat">The value used for <paramref name="stringFormat"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> StringFormat(string stringFormat)
    {
        Current().StringFormat = stringFormat;
        return this;
    }

    /// <summary>
    /// Executes the <c>BindingMode</c> operation.
    /// </summary>
    /// <param name="bindingMode">The value used for <paramref name="bindingMode"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> BindingMode(BindingMode bindingMode)
    {
        Current().Mode = bindingMode;
        return this;
    }

    /// <summary>
    /// Executes the <c>Converter</c> operation.
    /// </summary>
    /// <param name="converter">The value used for <paramref name="converter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Converter(IValueConverter converter)
    {
        var entry = Current();

        if (entry.Converter is ValueConverter)
            throw new InvalidOperationException(
                "Converter(...) cannot be combined with Convert()/ConvertBack() on the same sub binding. " +
                "Use either your own IValueConverter or the fluent delegates.");

        entry.Converter = converter;
        return this;
    }

    /// <summary>
    /// Executes the <c>Parameter</c> operation.
    /// </summary>
    /// <param name="converterParameter">The value used for <paramref name="converterParameter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Parameter(string converterParameter)
    {
        Current().ConverterParameter = converterParameter;
        return this;
    }

    /// <summary>
    /// Executes the <c>Source</c> operation.
    /// </summary>
    /// <param name="source">The value used for <paramref name="source"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Source(object source)
    {
        Current().Source = source;
        return this;
    }

    /// <summary>
    /// Executes the <c>FallbackValue</c> operation.
    /// </summary>
    /// <param name="fallbackValue">The value used for <paramref name="fallbackValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> FallbackValue(object fallbackValue)
    {
        Current().FallbackValue = fallbackValue;
        return this;
    }

    /// <summary>
    /// Executes the <c>TargetNullValue</c> operation.
    /// </summary>
    /// <param name="targetNullValue">The value used for <paramref name="targetNullValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> TargetNullValue(object targetNullValue)
    {
        Current().TargetNullValue = targetNullValue;
        return this;
    }

    /// <summary>
    /// Converts the value of the current sub binding into <typeparamref name="R"/>. Inside a multi binding
    /// that is the value this sub binding hands to <c>MultiConvert()</c>.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Convert<Q, R>(Func<Q, R> convert)
    {
        var entry = Current();
        var converter = FluentConverter(entry);
        var site = SiteOf(entry);

        converter.ConvertFunction = value => BindingValues.IsMissing(value, typeof(Q))
            ? BindableProperty.UnsetValue
            : convert(BindingValues.Unbox<Q>(value, site, ConverterDirection.Convert));

        return this;
    }

    /// <summary>
    /// Reverse of <c>Convert</c> for the current sub binding. Valid on its own, without a preceding
    /// <c>Convert</c>, to normalise values on the way back to the source.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> ConvertBack<R, Q>(Func<R, Q> convert)
    {
        var entry = Current();
        var converter = FluentConverter(entry);
        var site = SiteOf(entry);

        converter.ConvertBackFunction = value => BindingValues.IsMissing(value, typeof(R))
            ? BindableProperty.UnsetValue
            : convert(BindingValues.Unbox<R>(value, site, ConverterDirection.ConvertBack));

        return this;
    }

    // ---- multi binding -------------------------------------------------------------------------

    /// <summary>
    /// Mode of the multi binding as a whole. A single sub binding can still override it with
    /// <c>BindingMode()</c>.
    /// </summary>
    /// <param name="bindingMode">The value used for <paramref name="bindingMode"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> MultiMode(BindingMode bindingMode)
    {
        _multi.Mode = bindingMode;
        return this;
    }

    /// <summary>
    /// Formats the sub binding values positionally (<c>{0}</c>, <c>{1}</c>, …).
    /// </summary>
    /// <param name="stringFormat">The value used for <paramref name="stringFormat"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> MultiStringFormat(string stringFormat)
    {
        _multi.StringFormat = stringFormat;
        return this;
    }

    /// <summary>
    /// Combines the sub binding values with a hand written converter.
    /// </summary>
    /// <param name="converter">The value used for <paramref name="converter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> MultiConverter(IMultiValueConverter converter)
    {
        _multi.SetUserConverter(converter);
        return this;
    }

    /// <summary>
    /// <c>ConverterParameter</c> of the multi binding.
    /// </summary>
    /// <param name="converterParameter">The value used for <paramref name="converterParameter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> MultiParameter(object converterParameter)
    {
        _multi.ConverterParameter = converterParameter;
        return this;
    }

    /// <summary>
    /// <c>FallbackValue</c> of the multi binding.
    /// </summary>
    /// <param name="fallbackValue">The value used for <paramref name="fallbackValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> MultiFallbackValue(object fallbackValue)
    {
        _multi.FallbackValue = fallbackValue;
        return this;
    }

    /// <summary>
    /// <c>TargetNullValue</c> of the multi binding.
    /// </summary>
    /// <param name="targetNullValue">The value used for <paramref name="targetNullValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> MultiTargetNullValue(object targetNullValue)
    {
        _multi.TargetNullValue = targetNullValue;
        return this;
    }

    /// <summary>
    /// Combines the sub binding values into the property value, in declaration order.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> MultiConvert<Q1, Q2>(Func<Q1, Q2, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersBindingBuilder<T> MultiConvert<Q1, Q2, Q3>(Func<Q1, Q2, Q3, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4>(Func<Q1, Q2, Q3, Q4, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5>(Func<Q1, Q2, Q3, Q4, Q5, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5, Q6>(Func<Q1, Q2, Q3, Q4, Q5, Q6, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5, Q6, Q7>(Func<Q1, Q2, Q3, Q4, Q5, Q6, Q7, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8>(Func<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertySettersBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9>(Func<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <summary>
    /// Reverse of <c>MultiConvert</c>. The returned tuple is written back in declaration order.
    /// </summary>
    /// <param name="convertBack">The value used for <paramref name="convertBack"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> MultiConvertBack<Q1, Q2>(Func<T, (Q1, Q2)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3>(Func<T, (Q1, Q2, Q3)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4>(Func<T, (Q1, Q2, Q3, Q4)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5>(Func<T, (Q1, Q2, Q3, Q4, Q5)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5, Q6>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5, Q6, Q7>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6, Q7)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertySettersBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <summary>
    /// Combines an arbitrary number of sub bindings whose values share one type.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> MultiConvertRaw<Q>(Func<IReadOnlyList<Q>, T> convert)
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
    public PropertySettersBindingBuilder<T> MultiConvertRaw(Func<object?[], T> convert, Func<T, object?[]>? convertBack = null)
    {
        _multi.UseRaw(convert, convertBack);
        return this;
    }

    // ---- internals -----------------------------------------------------------------------------

    private PropertySettersBindingBuilder<T> Open(BindingEntry entry)
    {
        if (_current is not null && !_current.HasSource)
        {
            entry.Adopt(_current);
            _entries[_entries.Count - 1] = entry;
        }
        else
        {
            _entries.Add(entry);
        }

        _current = entry;
        return this;
    }

    private BindingEntry Current()
    {
        if (_current is null)
            Open(new PathBindingEntry());

        return _current!;
    }

    private static BindingSite SiteOf(BindingEntry entry) => entry.Site ??= new BindingSite();

    private static ValueConverter FluentConverter(BindingEntry entry)
    {
        switch (entry.Converter)
        {
            case null:
                var created = new ValueConverter();
                entry.Converter = created;
                return created;

            case ValueConverter existing:
                return existing;

            default:
                throw new InvalidOperationException(
                    "Convert()/ConvertBack() cannot be combined with a converter supplied through Converter(...). " +
                    "Use either your own IValueConverter or the fluent delegates for a given sub binding.");
        }
    }
}
