// Portions of this file incorporate and extend code originally from Sharp.UI.
// Copyright (c) 2022 Pawel Krzywdzinski
// Licensed under the MIT License. See THIRD-PARTY-NOTICES for details.

#nullable enable

namespace FmgLib.MauiMarkup;

/// <summary>
/// Shortcuts for the binding shapes that would otherwise need a converter delegate for nothing.
/// <para>
/// The multi binding aggregates run on the dynamic arity path, so the number of sub bindings is not
/// validated: <c>MultiAtLeast(3)</c> over two sub bindings is simply always <see langword="false"/>.
/// Every sub binding has to produce a <see cref="bool"/>, either directly or through its own
/// <c>Convert()</c>.
/// </para>
/// </summary>
public static class PropertyBindingBuilderExtension
{
    /// <summary>Inverts the bound value in both directions.</summary>
    public static PropertyBindingBuilder<bool> Negate(this PropertyBindingBuilder<bool> self)
        => self.Convert<bool, bool>(value => !value).ConvertBack<bool, bool>(value => !value);

    /// <summary>True when every sub binding is true.</summary>
    public static PropertyBindingBuilder<bool> MultiAll(this PropertyBindingBuilder<bool> self)
        => self.MultiConvertRaw<bool>(BooleanAggregates.All);

    /// <summary>True when at least one sub binding is true.</summary>
    public static PropertyBindingBuilder<bool> MultiAny(this PropertyBindingBuilder<bool> self)
        => self.MultiConvertRaw<bool>(BooleanAggregates.Any);

    /// <summary>True when no sub binding is true.</summary>
    public static PropertyBindingBuilder<bool> MultiNone(this PropertyBindingBuilder<bool> self)
        => self.MultiConvertRaw<bool>(values => !BooleanAggregates.Any(values));

    /// <summary>True when at least <paramref name="count"/> sub bindings are true.</summary>
    public static PropertyBindingBuilder<bool> MultiAtLeast(this PropertyBindingBuilder<bool> self, int count)
        => self.MultiConvertRaw<bool>(values => BooleanAggregates.Count(values) >= count);

    /// <summary>True when exactly <paramref name="count"/> sub bindings are true.</summary>
    public static PropertyBindingBuilder<bool> MultiExactly(this PropertyBindingBuilder<bool> self, int count)
        => self.MultiConvertRaw<bool>(values => BooleanAggregates.Count(values) == count);
}

/// <summary>
/// The style setter counterpart of <see cref="PropertyBindingBuilderExtension"/>.
/// </summary>
public static class PropertySettersBindingBuilderExtension
{
    /// <summary>Inverts the bound value in both directions.</summary>
    public static PropertySettersBindingBuilder<bool> Negate(this PropertySettersBindingBuilder<bool> self)
        => self.Convert<bool, bool>(value => !value).ConvertBack<bool, bool>(value => !value);

    /// <summary>True when every sub binding is true.</summary>
    public static PropertySettersBindingBuilder<bool> MultiAll(this PropertySettersBindingBuilder<bool> self)
        => self.MultiConvertRaw<bool>(BooleanAggregates.All);

    /// <summary>True when at least one sub binding is true.</summary>
    public static PropertySettersBindingBuilder<bool> MultiAny(this PropertySettersBindingBuilder<bool> self)
        => self.MultiConvertRaw<bool>(BooleanAggregates.Any);

    /// <summary>True when no sub binding is true.</summary>
    public static PropertySettersBindingBuilder<bool> MultiNone(this PropertySettersBindingBuilder<bool> self)
        => self.MultiConvertRaw<bool>(values => !BooleanAggregates.Any(values));

    /// <summary>True when at least <paramref name="count"/> sub bindings are true.</summary>
    public static PropertySettersBindingBuilder<bool> MultiAtLeast(this PropertySettersBindingBuilder<bool> self, int count)
        => self.MultiConvertRaw<bool>(values => BooleanAggregates.Count(values) >= count);

    /// <summary>True when exactly <paramref name="count"/> sub bindings are true.</summary>
    public static PropertySettersBindingBuilder<bool> MultiExactly(this PropertySettersBindingBuilder<bool> self, int count)
        => self.MultiConvertRaw<bool>(values => BooleanAggregates.Count(values) == count);
}

/// <summary>
/// The boolean aggregates for the multi binding builder opened with <c>Bindings(...)</c>.
/// </summary>
public static class PropertyMultiBindingBuilderExtension
{
    /// <summary>True when every child binding is true.</summary>
    public static PropertyMultiBindingBuilder<bool> MultiAll(this PropertyMultiBindingBuilder<bool> self)
        => self.MultiConvertRaw<bool>(BooleanAggregates.All);

    /// <summary>True when at least one child binding is true.</summary>
    public static PropertyMultiBindingBuilder<bool> MultiAny(this PropertyMultiBindingBuilder<bool> self)
        => self.MultiConvertRaw<bool>(BooleanAggregates.Any);

    /// <summary>True when no child binding is true.</summary>
    public static PropertyMultiBindingBuilder<bool> MultiNone(this PropertyMultiBindingBuilder<bool> self)
        => self.MultiConvertRaw<bool>(values => !BooleanAggregates.Any(values));

    /// <summary>True when at least <paramref name="count"/> child bindings are true.</summary>
    public static PropertyMultiBindingBuilder<bool> MultiAtLeast(this PropertyMultiBindingBuilder<bool> self, int count)
        => self.MultiConvertRaw<bool>(values => BooleanAggregates.Count(values) >= count);

    /// <summary>True when exactly <paramref name="count"/> child bindings are true.</summary>
    public static PropertyMultiBindingBuilder<bool> MultiExactly(this PropertyMultiBindingBuilder<bool> self, int count)
        => self.MultiConvertRaw<bool>(values => BooleanAggregates.Count(values) == count);
}

/// <summary>
/// The style setter counterpart of <see cref="PropertyMultiBindingBuilderExtension"/>.
/// </summary>
public static class PropertySettersMultiBindingBuilderExtension
{
    /// <summary>True when every child binding is true.</summary>
    public static PropertySettersMultiBindingBuilder<bool> MultiAll(this PropertySettersMultiBindingBuilder<bool> self)
        => self.MultiConvertRaw<bool>(BooleanAggregates.All);

    /// <summary>True when at least one child binding is true.</summary>
    public static PropertySettersMultiBindingBuilder<bool> MultiAny(this PropertySettersMultiBindingBuilder<bool> self)
        => self.MultiConvertRaw<bool>(BooleanAggregates.Any);

    /// <summary>True when no child binding is true.</summary>
    public static PropertySettersMultiBindingBuilder<bool> MultiNone(this PropertySettersMultiBindingBuilder<bool> self)
        => self.MultiConvertRaw<bool>(values => !BooleanAggregates.Any(values));

    /// <summary>True when at least <paramref name="count"/> child bindings are true.</summary>
    public static PropertySettersMultiBindingBuilder<bool> MultiAtLeast(this PropertySettersMultiBindingBuilder<bool> self, int count)
        => self.MultiConvertRaw<bool>(values => BooleanAggregates.Count(values) >= count);

    /// <summary>True when exactly <paramref name="count"/> child bindings are true.</summary>
    public static PropertySettersMultiBindingBuilder<bool> MultiExactly(this PropertySettersMultiBindingBuilder<bool> self, int count)
        => self.MultiConvertRaw<bool>(values => BooleanAggregates.Count(values) == count);
}

internal static class BooleanAggregates
{
    public static bool All(IReadOnlyList<bool> values)
    {
        for (var i = 0; i < values.Count; i++)
        {
            if (!values[i])
                return false;
        }

        return true;
    }

    public static bool Any(IReadOnlyList<bool> values)
    {
        for (var i = 0; i < values.Count; i++)
        {
            if (values[i])
                return true;
        }

        return false;
    }

    public static int Count(IReadOnlyList<bool> values)
    {
        var count = 0;

        for (var i = 0; i < values.Count; i++)
        {
            if (values[i])
                count++;
        }

        return count;
    }
}
