#nullable enable

using System.Linq.Expressions;
using Microsoft.Maui.Controls.Internals;

namespace FmgLib.MauiMarkup;

/// <summary>
/// One sub binding opened by <c>Path()</c>, <c>Getter()</c> or <c>Bindings()</c> on a fluent binding
/// builder. A builder that holds a single entry produces a plain binding; several entries produce a
/// <see cref="MultiBinding"/> in declaration order.
/// </summary>
internal abstract class BindingEntry
{
    public BindingMode Mode = BindingMode.Default;

    public IValueConverter? Converter;

    public object? ConverterParameter;

    public string? StringFormat;

    public object? Source;

    public object? FallbackValue;

    public object? TargetNullValue;

    /// <summary>Diagnostics of the fluent converter attached to this entry, when one was created.</summary>
    public BindingSite? Site;

    /// <summary><see langword="false"/> while the entry has neither a path nor a getter yet.</summary>
    public abstract bool HasSource { get; }

    /// <summary>Path of the entry, for diagnostics. <see langword="null"/> for compiled and raw entries.</summary>
    public abstract string? DiagnosticPath { get; }

    public abstract BindingBase CreateBinding();

    /// <summary>
    /// Takes over the settings of a placeholder entry, created when a modifier such as <c>Source()</c> was
    /// called before the <c>Path()</c> or <c>Getter()</c> it belongs to.
    /// </summary>
    public void Adopt(BindingEntry placeholder)
    {
        Mode = placeholder.Mode;
        Converter = placeholder.Converter;
        ConverterParameter = placeholder.ConverterParameter;
        StringFormat = placeholder.StringFormat;
        Source = placeholder.Source;
        FallbackValue = placeholder.FallbackValue;
        TargetNullValue = placeholder.TargetNullValue;
        Site = placeholder.Site;
    }
}

/// <summary>String path sub binding, resolved at runtime.</summary>
internal sealed class PathBindingEntry : BindingEntry
{
    public string? Path;

    public override bool HasSource => !string.IsNullOrWhiteSpace(Path);

    public override string? DiagnosticPath => Path;

    public override BindingBase CreateBinding()
    {
        var binding = new Binding(Path!, Mode, Converter, ConverterParameter, StringFormat, Source);

        if (FallbackValue is not null)
            binding.FallbackValue = FallbackValue;

        if (TargetNullValue is not null)
            binding.TargetNullValue = TargetNullValue;

        return binding;
    }
}

/// <summary>
/// Compiled sub binding built from an expression, so the value is read through a delegate instead of
/// reflection. <typeparamref name="TValue"/> is the type produced by the getter, which is not necessarily
/// the type of the bound property: inside a multi binding every entry may contribute its own type.
/// </summary>
internal sealed class TypedBindingEntry<TValue> : BindingEntry
{
    public TypedBindingEntry(Func<object, TValue> getter, string memberName)
    {
        Getter = getter;
        MemberName = memberName;
    }

    public Func<object, TValue> Getter { get; }

    public string MemberName { get; }

    public Action<object, TValue>? Setter;

    public override bool HasSource => true;

    public override string? DiagnosticPath => MemberName;

    public override BindingBase CreateBinding()
    {
        var handlers = new[]
        {
            Tuple.Create<Func<object, object?>, string>(source => source, MemberName)
        };

        return new TypedBinding<object, TValue>(source => (Getter(source), true), Setter, handlers)
        {
            Mode = Mode,
            Converter = Converter,
            ConverterParameter = ConverterParameter,
            StringFormat = StringFormat,
            Source = Source,
            FallbackValue = FallbackValue,
            TargetNullValue = TargetNullValue
        };
    }
}

/// <summary>
/// A ready made <see cref="BindingBase"/> handed to the builder through <c>Bindings(...)</c>, so hand
/// written and fluent sub bindings can be mixed inside the same multi binding.
/// </summary>
internal sealed class RawBindingEntry : BindingEntry
{
    public RawBindingEntry(BindingBase binding) => Binding = binding;

    public BindingBase Binding { get; }

    public override bool HasSource => true;

    public override string? DiagnosticPath => (Binding as Binding)?.Path;

    public override BindingBase CreateBinding() => Binding;
}

/// <summary>
/// Creates the compiled entries of the fluent builders, so both the property and the style setter builder
/// rewrite getter expressions the same way.
/// </summary>
internal static class BindingEntryFactory
{
    /// <summary>
    /// Rebinds the getter expression to an <see cref="object"/> parameter and compiles it. The binding
    /// context is not statically known at this point, so the cast happens inside the compiled delegate.
    /// </summary>
    public static TypedBindingEntry<TValue> Typed<TContext, TValue>(Expression<Func<TContext, TValue>> getter)
    {
        var memberName = TypedBindingExtensions.GetMemberName(getter);

        var parameter = Expression.Parameter(typeof(object), "source");
        var body = new ParameterReplacer(getter.Parameters[0], Expression.Convert(parameter, typeof(TContext)))
            .Visit(getter.Body);

        var compiled = Expression.Lambda<Func<object, TValue>>(body!, parameter).Compile();

        return new TypedBindingEntry<TValue>(compiled, memberName);
    }

    /// <summary>
    /// Attaches the reverse operation to a compiled entry. The setter is skipped when the binding context
    /// turns out to be of another type, which is what makes a setter safe on a reused template.
    /// </summary>
    public static void AttachSetter<TContext, TValue>(BindingEntry entry, Action<TContext, TValue> setter)
    {
        if (entry is not TypedBindingEntry<TValue> typed)
            throw new InvalidOperationException(
                $"Setter() applies to the compiled sub binding opened by the preceding Getter() and has to use " +
                $"its value type ({typeof(TValue).Name} was given).");

        typed.Setter = (source, value) =>
        {
            if (source is TContext context)
                setter(context, value);
        };
    }
}

/// <summary>
/// Turns the entries collected by a fluent builder into the binding that is finally applied: a plain
/// binding for a single entry, a <see cref="MultiBinding"/> as soon as several entries are declared or a
/// multi level converter is configured.
/// </summary>
internal static class FluentBindingFactory
{
    public static BindingBase? Create<T>(
        List<BindingEntry> entries,
        MultiBindingSpec<T> multi,
        Type? ownerType,
        BindableProperty? property)
    {
        var declared = entries.Where(entry => entry.HasSource).ToList();
        if (declared.Count == 0)
            return null;

        foreach (var entry in declared)
            Describe(entry.Site, ownerType, property, [entry.DiagnosticPath]);

        if (declared.Count == 1 && !multi.IsConfigured)
            return declared[0].CreateBinding();

        Describe(multi.Site, ownerType, property, declared.Select(entry => entry.DiagnosticPath).ToArray());
        multi.Validate(declared.Count);

        var multiBinding = new MultiBinding
        {
            Bindings = declared.Select(entry => entry.CreateBinding()).ToList(),
            Converter = multi.EffectiveConverter,
            ConverterParameter = multi.ConverterParameter,
            Mode = multi.Mode,
            StringFormat = multi.StringFormat
        };

        if (multi.FallbackValue is not null)
            multiBinding.FallbackValue = multi.FallbackValue;

        if (multi.TargetNullValue is not null)
            multiBinding.TargetNullValue = multi.TargetNullValue;

        return multiBinding;
    }

    private static void Describe(BindingSite? site, Type? ownerType, BindableProperty? property, string?[] paths)
    {
        if (site is null)
            return;

        site.OwnerType = ownerType ?? property?.DeclaringType;
        site.PropertyName = property?.PropertyName;
        site.PropertyType = property?.ReturnType;
        site.Paths = paths;
    }
}
