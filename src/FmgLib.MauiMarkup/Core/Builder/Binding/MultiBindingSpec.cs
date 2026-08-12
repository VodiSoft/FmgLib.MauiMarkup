#nullable enable

using System.Globalization;

namespace FmgLib.MauiMarkup;

/// <summary>
/// Multi value converter produced by the fluent <c>MultiConvert</c> family.
/// <para>
/// MAUI applies a binding immediately inside <c>SetBinding</c>, so the converter runs once while the target
/// still has no binding context and every sub binding is empty. Returning <see cref="Binding.DoNothing"/>
/// then keeps the target property at its current value; returning <see langword="null"/> instead would push
/// a null through and clear the property.
/// </para>
/// <para>
/// <b>That hold only applies where an empty slot is distinguishable from a real value.</b>
/// <see cref="BindingValues.IsMissing"/> holds for <see cref="BindableProperty.UnsetValue"/> always, and for
/// <see langword="null"/> when the delegate parameter is a non nullable value type — handing such a
/// parameter a <c>default(int)</c> it never asked for would be worse than waiting. For a REFERENCE TYPE
/// parameter <see langword="null"/> is a legitimate source value, so it is passed straight to the delegate
/// and the delegate has to be null safe:
/// </para>
/// <code>
/// // throws during page construction, before the sources resolve:
/// .MultiConvert((string first, string last) =&gt; $"{last.ToUpperInvariant()}, {first}")
///
/// // safe:
/// .MultiConvert((string first, string last) =&gt; $"{last?.ToUpperInvariant()}, {first}")
/// </code>
/// <para>
/// The same rule applies to the single binding <c>Convert</c> family. Both halves of this contract are
/// pinned by tests (<c>ConverterReceivesNullForUnresolvedReferenceTypedSubBindings</c> and
/// <c>ConverterIsNotCalledForUnresolvedValueTypedSubBindings</c>).
/// </para>
/// </summary>
internal sealed class FluentMultiValueConverter<T> : IMultiValueConverter
{
    public Func<object?[], object?>? ConvertFunction;

    public Func<T, object?[]>? ConvertBackFunction;

    /// <summary>Delegate parameter types, per position. Set for the fixed arity overloads.</summary>
    public Type[]? ExpectedTypes;

    /// <summary>Single delegate parameter type shared by every position, for the dynamic arity overloads.</summary>
    public Type? UniformExpectedType;

    public BindingSite? Site;

    private Type? ExpectedAt(int index)
    {
        if (ExpectedTypes is not null)
            return index >= 0 && index < ExpectedTypes.Length ? ExpectedTypes[index] : null;

        return UniformExpectedType;
    }

    public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null || ConvertFunction is null)
            return Binding.DoNothing;

        for (var i = 0; i < values.Length; i++)
        {
            if (BindingValues.IsMissing(values[i], ExpectedAt(i)))
                return Binding.DoNothing;
        }

        return ConvertFunction(values);
    }

    public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        if (ConvertBackFunction is null)
            return null;

        return ConvertBackFunction(BindingValues.Unbox<T>(value, Site, ConverterDirection.ConvertBack));
    }
}

/// <summary>
/// Multi binding state shared by <see cref="PropertyBindingBuilder{T}"/> and
/// <see cref="PropertySettersBindingBuilder{T}"/>: everything that belongs to the <see cref="MultiBinding"/>
/// itself rather than to one of its sub bindings, plus the fluent converter overloads.
/// </summary>
internal sealed class MultiBindingSpec<T>
{
    private const int DynamicArity = -1;

    public BindingMode Mode = BindingMode.Default;

    public string? StringFormat;

    public object? ConverterParameter;

    public object? FallbackValue;

    public object? TargetNullValue;

    /// <summary>Converter supplied by the caller. Mutually exclusive with the fluent overloads.</summary>
    public IMultiValueConverter? UserConverter;

    public FluentMultiValueConverter<T>? FluentConverter;

    /// <summary>Number of sub bindings the fluent converter expects, or -1 when it is arity agnostic.</summary>
    public int Arity = DynamicArity;

    public readonly BindingSite Site = new();

    /// <summary><see langword="true"/> once anything forces the builder to produce a multi binding.</summary>
    public bool IsConfigured =>
        UserConverter is not null || FluentConverter is not null || StringFormat is not null;

    public IMultiValueConverter? EffectiveConverter => UserConverter ?? FluentConverter;

    public bool HasFixedArity => Arity != DynamicArity;

    public void SetUserConverter(IMultiValueConverter converter)
    {
        if (FluentConverter is not null)
            throw new InvalidOperationException(
                "MultiConverter(...) cannot be combined with the MultiConvert(...) family. Use either your own " +
                "IMultiValueConverter or the fluent overloads.");

        UserConverter = converter;
    }

    /// <summary>Validates the collected sub bindings against the converter that was declared.</summary>
    public void Validate(int entryCount)
    {
        if (EffectiveConverter is null && StringFormat is null)
            throw new InvalidOperationException(
                $"{entryCount} sub bindings were declared, but nothing combines them. Add MultiConvert(...) with " +
                $"{entryCount} parameters, MultiConvertRaw(...) for a dynamic number of sub bindings, " +
                "MultiStringFormat(...) or MultiConverter(...).");

        if (FluentConverter is not null && HasFixedArity && Arity != entryCount)
            throw new InvalidOperationException(
                $"{entryCount} sub bindings were declared, but the MultiConvert delegate takes {Arity} parameters.");
    }

    private FluentMultiValueConverter<T> Fluent()
    {
        if (UserConverter is not null)
            throw new InvalidOperationException(
                "The MultiConvert(...) family cannot be combined with a converter supplied through MultiConverter(...).");

        return FluentConverter ??= new FluentMultiValueConverter<T> { Site = Site };
    }

    private void Forward(Type[] expected, Func<object?[], object?> convert)
    {
        var converter = Fluent();
        converter.ExpectedTypes = expected;
        converter.UniformExpectedType = null;
        converter.ConvertFunction = convert;
        Arity = expected.Length;
    }

    private FluentMultiValueConverter<T> Backward(int arity)
    {
        if (FluentConverter?.ConvertFunction is null)
            throw new InvalidOperationException(
                "MultiConvertBack(...) needs a preceding MultiConvert(...), which defines the reading direction " +
                "and the number of sub bindings.");

        if (!HasFixedArity)
            throw new InvalidOperationException(
                "This builder uses MultiConvertRaw(...), whose arity is dynamic. Pass the reverse delegate as its " +
                "second argument instead of calling MultiConvertBack(...).");

        if (Arity != arity)
            throw new InvalidOperationException(
                $"The MultiConvertBack delegate returns {arity} values, but MultiConvert takes {Arity} parameters.");

        return FluentConverter;
    }

    private TValue At<TValue>(object?[] values, int index)
        => BindingValues.Unbox<TValue>(values[index], Site, ConverterDirection.Convert, index);

    // ---- reading: fixed arity ------------------------------------------------------------------

    public void Use<Q1, Q2>(Func<Q1, Q2, T> convert)
        => Forward([typeof(Q1), typeof(Q2)],
            v => convert(At<Q1>(v, 0), At<Q2>(v, 1)));

    public void Use<Q1, Q2, Q3>(Func<Q1, Q2, Q3, T> convert)
        => Forward([typeof(Q1), typeof(Q2), typeof(Q3)],
            v => convert(At<Q1>(v, 0), At<Q2>(v, 1), At<Q3>(v, 2)));

    public void Use<Q1, Q2, Q3, Q4>(Func<Q1, Q2, Q3, Q4, T> convert)
        => Forward([typeof(Q1), typeof(Q2), typeof(Q3), typeof(Q4)],
            v => convert(At<Q1>(v, 0), At<Q2>(v, 1), At<Q3>(v, 2), At<Q4>(v, 3)));

    public void Use<Q1, Q2, Q3, Q4, Q5>(Func<Q1, Q2, Q3, Q4, Q5, T> convert)
        => Forward([typeof(Q1), typeof(Q2), typeof(Q3), typeof(Q4), typeof(Q5)],
            v => convert(At<Q1>(v, 0), At<Q2>(v, 1), At<Q3>(v, 2), At<Q4>(v, 3), At<Q5>(v, 4)));

    public void Use<Q1, Q2, Q3, Q4, Q5, Q6>(Func<Q1, Q2, Q3, Q4, Q5, Q6, T> convert)
        => Forward([typeof(Q1), typeof(Q2), typeof(Q3), typeof(Q4), typeof(Q5), typeof(Q6)],
            v => convert(At<Q1>(v, 0), At<Q2>(v, 1), At<Q3>(v, 2), At<Q4>(v, 3), At<Q5>(v, 4), At<Q6>(v, 5)));

    public void Use<Q1, Q2, Q3, Q4, Q5, Q6, Q7>(Func<Q1, Q2, Q3, Q4, Q5, Q6, Q7, T> convert)
        => Forward([typeof(Q1), typeof(Q2), typeof(Q3), typeof(Q4), typeof(Q5), typeof(Q6), typeof(Q7)],
            v => convert(At<Q1>(v, 0), At<Q2>(v, 1), At<Q3>(v, 2), At<Q4>(v, 3), At<Q5>(v, 4), At<Q6>(v, 5), At<Q7>(v, 6)));

    public void Use<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8>(Func<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, T> convert)
        => Forward([typeof(Q1), typeof(Q2), typeof(Q3), typeof(Q4), typeof(Q5), typeof(Q6), typeof(Q7), typeof(Q8)],
            v => convert(At<Q1>(v, 0), At<Q2>(v, 1), At<Q3>(v, 2), At<Q4>(v, 3), At<Q5>(v, 4), At<Q6>(v, 5), At<Q7>(v, 6), At<Q8>(v, 7)));

    public void Use<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9>(Func<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9, T> convert)
        => Forward([typeof(Q1), typeof(Q2), typeof(Q3), typeof(Q4), typeof(Q5), typeof(Q6), typeof(Q7), typeof(Q8), typeof(Q9)],
            v => convert(At<Q1>(v, 0), At<Q2>(v, 1), At<Q3>(v, 2), At<Q4>(v, 3), At<Q5>(v, 4), At<Q6>(v, 5), At<Q7>(v, 6), At<Q8>(v, 7), At<Q9>(v, 8)));

    // ---- writing: fixed arity ------------------------------------------------------------------

    public void UseBack<Q1, Q2>(Func<T, (Q1, Q2)> convertBack)
    {
        var converter = Backward(2);
        converter.ConvertBackFunction = value =>
        {
            var (q1, q2) = convertBack(value);
            return [q1, q2];
        };
    }

    public void UseBack<Q1, Q2, Q3>(Func<T, (Q1, Q2, Q3)> convertBack)
    {
        var converter = Backward(3);
        converter.ConvertBackFunction = value =>
        {
            var (q1, q2, q3) = convertBack(value);
            return [q1, q2, q3];
        };
    }

    public void UseBack<Q1, Q2, Q3, Q4>(Func<T, (Q1, Q2, Q3, Q4)> convertBack)
    {
        var converter = Backward(4);
        converter.ConvertBackFunction = value =>
        {
            var (q1, q2, q3, q4) = convertBack(value);
            return [q1, q2, q3, q4];
        };
    }

    public void UseBack<Q1, Q2, Q3, Q4, Q5>(Func<T, (Q1, Q2, Q3, Q4, Q5)> convertBack)
    {
        var converter = Backward(5);
        converter.ConvertBackFunction = value =>
        {
            var (q1, q2, q3, q4, q5) = convertBack(value);
            return [q1, q2, q3, q4, q5];
        };
    }

    public void UseBack<Q1, Q2, Q3, Q4, Q5, Q6>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6)> convertBack)
    {
        var converter = Backward(6);
        converter.ConvertBackFunction = value =>
        {
            var (q1, q2, q3, q4, q5, q6) = convertBack(value);
            return [q1, q2, q3, q4, q5, q6];
        };
    }

    public void UseBack<Q1, Q2, Q3, Q4, Q5, Q6, Q7>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6, Q7)> convertBack)
    {
        var converter = Backward(7);
        converter.ConvertBackFunction = value =>
        {
            var (q1, q2, q3, q4, q5, q6, q7) = convertBack(value);
            return [q1, q2, q3, q4, q5, q6, q7];
        };
    }

    public void UseBack<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8)> convertBack)
    {
        var converter = Backward(8);
        converter.ConvertBackFunction = value =>
        {
            var (q1, q2, q3, q4, q5, q6, q7, q8) = convertBack(value);
            return [q1, q2, q3, q4, q5, q6, q7, q8];
        };
    }

    public void UseBack<Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9>(Func<T, (Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9)> convertBack)
    {
        var converter = Backward(9);
        converter.ConvertBackFunction = value =>
        {
            var (q1, q2, q3, q4, q5, q6, q7, q8, q9) = convertBack(value);
            return [q1, q2, q3, q4, q5, q6, q7, q8, q9];
        };
    }

    // ---- dynamic arity -------------------------------------------------------------------------

    public void UseRaw(Func<object?[], T> convert, Func<T, object?[]>? convertBack)
    {
        var converter = Fluent();
        converter.ExpectedTypes = null;
        converter.UniformExpectedType = null;
        converter.ConvertFunction = values => convert(values);
        converter.ConvertBackFunction = convertBack;
        Arity = DynamicArity;
    }

    public void UseRaw<Q>(Func<IReadOnlyList<Q>, T> convert)
    {
        var converter = Fluent();
        converter.ExpectedTypes = null;
        converter.UniformExpectedType = typeof(Q);
        converter.ConvertFunction = values =>
        {
            var typed = new Q[values.Length];
            for (var i = 0; i < values.Length; i++)
                typed[i] = At<Q>(values, i);

            return convert(typed);
        };
        Arity = DynamicArity;
    }
}
