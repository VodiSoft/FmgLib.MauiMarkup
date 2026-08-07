// Portions of this file incorporate and extend code originally from Sharp.UI.
// Copyright (c) 2022 Pawel Krzywdzinski
// Licensed under the MIT License. See THIRD-PARTY-NOTICES for details.

#nullable enable

using System.Globalization;
using System.Linq.Expressions;

namespace FmgLib.MauiMarkup;

/// <summary>
/// Fluent binding builder for a bindable property.
/// <para>
/// <c>Path()</c> and <c>Getter()</c> each open a sub binding; every modifier that follows — <c>Source</c>,
/// <c>StringFormat</c>, <c>BindingMode</c>, <c>Parameter</c>, <c>Converter</c>, <c>Convert</c>,
/// <c>ConvertBack</c>, <c>FallbackValue</c>, <c>TargetNullValue</c> — applies to the sub binding that was
/// opened last.
/// </para>
/// <para>
/// One sub binding produces a plain binding and its <c>Convert()</c> result becomes the property value.
/// Several sub bindings produce a <see cref="MultiBinding"/>: each of them may convert its own value first,
/// and a closing <c>MultiConvert()</c> (or <c>MultiStringFormat()</c>, or one of the boolean aggregates)
/// combines them into the final value.
/// </para>
/// </summary>
public sealed class PropertyBindingBuilder<T> : IPropertyBuilder<T>
{
    /// <summary>
    /// Converter of a single sub binding, driven by the <c>Convert()</c> / <c>ConvertBack()</c> delegates.
    /// A direction without a delegate passes the value through untouched, so declaring only
    /// <c>ConvertBack()</c> normalises writing without affecting reading.
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

    private PropertyContext<T> _context;

    public PropertyContext<T> Context
    {
        get => _context;
        set => _context = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Initializes a new instance of the <c>PropertyBindingBuilder</c> class.
    /// </summary>
    /// <param name="context">The value used for <paramref name="context"/>.</param>
    public PropertyBindingBuilder(PropertyContext<T> context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Builds the configuration for the <c>Build</c> operation.
    /// </summary>
    /// <returns><see langword="true"/> when the operation succeeds; otherwise, <see langword="false"/>.</returns>
    public bool Build()
    {
        var binding = FluentBindingFactory.Create(
            _entries, _multi, Context.BindableObject.GetType(), Context.Property);

        if (binding is null)
            return false;

        Context.BindableObject.SetBinding(Context.Property, binding);
        return true;
    }

    // ---- sub bindings --------------------------------------------------------------------------

    /// <summary>
    /// Opens a sub binding on <paramref name="path"/>, resolved at runtime.
    /// </summary>
    /// <param name="path">The value used for <paramref name="path"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> Path(string path)
        => Open(new PathBindingEntry { Path = path });

    /// <summary>
    /// Opens a compiled sub binding that produces <typeparamref name="TValue"/>. Inside a multi binding
    /// every sub binding may contribute a different type; the closing <c>MultiConvert()</c> combines them.
    /// </summary>
    /// <param name="getter">The value used for <paramref name="getter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> Getter<TContext, TValue>(Expression<Func<TContext, TValue>> getter)
        => Open(BindingEntryFactory.Typed<TContext, TValue>(getter));

    /// <summary>
    /// Supplies the reverse operation of the current compiled sub binding, enabling two way updates.
    /// </summary>
    /// <param name="setter">The value used for <paramref name="setter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> Setter<TContext, TValue>(Action<TContext, TValue> setter)
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
    public PropertyBindingBuilder<T> Bindings(params BindingBase[] bindings)
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
    public PropertyBindingBuilder<T> StringFormat(string stringFormat)
    {
        Current().StringFormat = stringFormat;
        return this;
    }

    /// <summary>
    /// Executes the <c>BindingMode</c> operation.
    /// </summary>
    /// <param name="bindingMode">The value used for <paramref name="bindingMode"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> BindingMode(BindingMode bindingMode)
    {
        Current().Mode = bindingMode;
        return this;
    }

    /// <summary>
    /// Executes the <c>Converter</c> operation.
    /// </summary>
    /// <param name="converter">The value used for <paramref name="converter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> Converter(IValueConverter converter)
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
    public PropertyBindingBuilder<T> Parameter(object converterParameter)
    {
        Current().ConverterParameter = converterParameter;
        return this;
    }

    /// <summary>
    /// Executes the <c>Source</c> operation.
    /// </summary>
    /// <param name="source">The value used for <paramref name="source"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> Source(object source)
    {
        Current().Source = source;
        return this;
    }

    /// <summary>
    /// Executes the <c>FallbackValue</c> operation.
    /// </summary>
    /// <param name="fallbackValue">The value used for <paramref name="fallbackValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> FallbackValue(object fallbackValue)
    {
        Current().FallbackValue = fallbackValue;
        return this;
    }

    /// <summary>
    /// Executes the <c>TargetNullValue</c> operation.
    /// </summary>
    /// <param name="targetNullValue">The value used for <paramref name="targetNullValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> TargetNullValue(object targetNullValue)
    {
        Current().TargetNullValue = targetNullValue;
        return this;
    }

    /// <summary>
    /// Converts the value of the current sub binding into <typeparamref name="R"/>. With a single sub
    /// binding that is the property value; inside a multi binding it is the value this sub binding hands
    /// to <c>MultiConvert()</c>.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> Convert<Q, R>(Func<Q, R> convert)
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
    public PropertyBindingBuilder<T> ConvertBack<R, Q>(Func<R, Q> convert)
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
    public PropertyBindingBuilder<T> MultiMode(BindingMode bindingMode)
    {
        _multi.Mode = bindingMode;
        return this;
    }

    /// <summary>
    /// Formats the sub binding values positionally (<c>{0}</c>, <c>{1}</c>, …), which combines them
    /// without any converter.
    /// </summary>
    /// <param name="stringFormat">The value used for <paramref name="stringFormat"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> MultiStringFormat(string stringFormat)
    {
        _multi.StringFormat = stringFormat;
        return this;
    }

    /// <summary>
    /// Combines the sub binding values with a hand written converter.
    /// </summary>
    /// <param name="converter">The value used for <paramref name="converter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> MultiConverter(IMultiValueConverter converter)
    {
        _multi.SetUserConverter(converter);
        return this;
    }

    /// <summary>
    /// <c>ConverterParameter</c> of the multi binding.
    /// </summary>
    /// <param name="converterParameter">The value used for <paramref name="converterParameter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> MultiParameter(object converterParameter)
    {
        _multi.ConverterParameter = converterParameter;
        return this;
    }

    /// <summary>
    /// <c>FallbackValue</c> of the multi binding.
    /// </summary>
    /// <param name="fallbackValue">The value used for <paramref name="fallbackValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> MultiFallbackValue(object fallbackValue)
    {
        _multi.FallbackValue = fallbackValue;
        return this;
    }

    /// <summary>
    /// <c>TargetNullValue</c> of the multi binding.
    /// </summary>
    /// <param name="targetNullValue">The value used for <paramref name="targetNullValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> MultiTargetNullValue(object targetNullValue)
    {
        _multi.TargetNullValue = targetNullValue;
        return this;
    }

    /// <summary>
    /// Combines the sub binding values into the property value. The parameters match the values produced by
    /// the sub bindings, in declaration order: either the raw source type, or the result of that sub
    /// binding's own <c>Convert()</c>.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> MultiConvert<Q1, Q2>(Func<Q1, Q2, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertyBindingBuilder<T> MultiConvert<Q1, Q2, Q3>(Func<Q1, Q2, Q3, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertyBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4>(Func<Q1, Q2, Q3, Q4, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertyBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5>(Func<Q1, Q2, Q3, Q4, Q5, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertyBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5, Q6>(Func<Q1, Q2, Q3, Q4, Q5, Q6, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertyBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5, Q6, Q7>(Func<Q1, Q2, Q3, Q4, Q5, Q6, Q7, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertyBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8>(Func<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <inheritdoc cref="MultiConvert{Q1, Q2}(Func{Q1, Q2, T})"/>
    public PropertyBindingBuilder<T> MultiConvert<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9>(Func<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9, T> convert)
    {
        _multi.Use(convert);
        return this;
    }

    /// <summary>
    /// Reverse of <c>MultiConvert</c>. The returned tuple is written back in declaration order, and each
    /// element still passes through the <c>ConvertBack()</c> of its own sub binding, if one was declared.
    /// </summary>
    /// <param name="convertBack">The value used for <paramref name="convertBack"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> MultiConvertBack<Q1, Q2>(Func<T, (Q1, Q2)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertyBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3>(Func<T, (Q1, Q2, Q3)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertyBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4>(Func<T, (Q1, Q2, Q3, Q4)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertyBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5>(Func<T, (Q1, Q2, Q3, Q4, Q5)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertyBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5, Q6>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertyBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5, Q6, Q7>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6, Q7)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertyBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <inheritdoc cref="MultiConvertBack{Q1, Q2}(Func{T, ValueTuple{Q1, Q2}})"/>
    public PropertyBindingBuilder<T> MultiConvertBack<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9)> convertBack)
    {
        _multi.UseBack(convertBack);
        return this;
    }

    /// <summary>
    /// Combines an arbitrary number of sub bindings whose values share one type. Unlike <c>MultiConvert</c>
    /// the number of sub bindings is not checked, so this is the entry point for aggregates built at runtime.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> MultiConvertRaw<Q>(Func<IReadOnlyList<Q>, T> convert)
    {
        _multi.UseRaw(convert);
        return this;
    }

    /// <summary>
    /// Combines an arbitrary number of sub bindings of mixed types. The values arrive boxed, in declaration
    /// order, and the optional reverse delegate has to return the same number of values in the same order.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <param name="convertBack">The value used for <paramref name="convertBack"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertyBindingBuilder<T> MultiConvertRaw(Func<object?[], T> convert, Func<T, object?[]>? convertBack = null)
    {
        _multi.UseRaw(convert, convertBack);
        return this;
    }

    // ---- internals -----------------------------------------------------------------------------

    private PropertyBindingBuilder<T> Open(BindingEntry entry)
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
