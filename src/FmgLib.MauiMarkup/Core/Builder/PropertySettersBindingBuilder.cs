using System.Globalization;
using System.Linq.Expressions;
using Microsoft.Maui.Controls.Internals;

namespace FmgLib.MauiMarkup;

public sealed class PropertySettersBindingBuilder<T> : IPropertySettersBuilder<T>
{
    public class ValueConverter : IValueConverter
    {
        internal Func<object, T> ConvertFunction;

        internal Func<T, object> ConvertBackFunction;

        /// <summary>
        /// Executes the <c>Convert</c> operation.
        /// </summary>
        /// <param name="value">The value used for <paramref name="value"/>.</param>
        /// <param name="targetType">The value used for <paramref name="targetType"/>.</param>
        /// <param name="parameter">The value used for <paramref name="parameter"/>.</param>
        /// <param name="culture">The value used for <paramref name="culture"/>.</param>
        /// <returns>The result produced by the operation.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && ConvertFunction != null)
            {
                return ConvertFunction(value);
            }

            return null;
        }

        /// <summary>
        /// Executes the <c>ConvertBack</c> operation.
        /// </summary>
        /// <param name="value">The value used for <paramref name="value"/>.</param>
        /// <param name="targetType">The value used for <paramref name="targetType"/>.</param>
        /// <param name="parameter">The value used for <paramref name="parameter"/>.</param>
        /// <param name="culture">The value used for <paramref name="culture"/>.</param>
        /// <returns>The result produced by the operation.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && ConvertBackFunction != null)
            {
                return ConvertBackFunction((T)value);
            }

            return null;
        }
    }

    private Expression<Func<object, T>> getter;
    
    private Action<object, T> setter;
    
    private string path;

    private BindingMode bindingMode;

    private IValueConverter converter;

    private ValueConverter valueConverter;

    private string converterParameter;

    private string stringFormat;

    private object source;

    private object fallbackValue;

    private object targetNullValue;

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
        if (getter != null)
        {
            var getterFunc = TypedBindingExtensions.ConvertExpressionToFunc(getter);
            var handlers = new (Func<object, object?>, string)[]
                { ((object b) => b, TypedBindingExtensions.GetMemberName(getter)) };
            Context.XamlSetters.Add(
                new Setter
                {
                    Property = Context.Property,
                    Value = new TypedBinding<object, T>(bindingContext => (getterFunc(bindingContext), true), setter, handlers.Select(x => x.ToTuple()).ToArray())
                    {
                        Mode = bindingMode,
                        Converter = converter,
                        ConverterParameter = converterParameter,
                        StringFormat = stringFormat,
                        Source = source,
                        TargetNullValue = targetNullValue,
                        FallbackValue = fallbackValue
                    }
                }
            );
            return true;
        }
        else if (path != null)
        {
            Context.XamlSetters.Add(
                new Setter
                {
                    Property = Context.Property,
                    Value = new Binding(path, bindingMode, converter, converterParameter, stringFormat, source)
                            .FallbackValue(fallbackValue)
                            .TargetNullValue(targetNullValue)
                }
            );
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// Gets the value produced by the <c>Getter</c> operation.
    /// </summary>
    /// <param name="getter">The value used for <paramref name="getter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Getter<TContext>(Expression<Func<TContext, T>> getter)
    {
        var parameter = Expression.Parameter(typeof(object), "obj");
        var convertedParam = Expression.Convert(parameter, typeof(TContext));
    
        var body = getter.Body;
    
        var newBody = new ParameterReplacer(getter.Parameters[0], convertedParam).Visit(body);
    
        this.getter = Expression.Lambda<Func<object, T>>(newBody, parameter);
    
        return this;
    }

    /// <summary>
    /// Sets the value handled by the <c>Setter</c> operation.
    /// </summary>
    /// <param name="setter">The value used for <paramref name="setter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Setter<TContext>(Action<TContext, T> setter)
    {
        this.setter = (obj, value) =>
        {
            var contextObj = (TContext)obj;
            setter(contextObj, value);
        };
    
        return this;
    }

    /// <summary>
    /// Executes the <c>Path</c> operation.
    /// </summary>
    /// <param name="path">The value used for <paramref name="path"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Path(string path)
    {
        this.path = path;
        return this;
    }

    /// <summary>
    /// Executes the <c>StringFormat</c> operation.
    /// </summary>
    /// <param name="stringFormat">The value used for <paramref name="stringFormat"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> StringFormat(string stringFormat)
    {
        this.stringFormat = stringFormat;
        return this;
    }

    /// <summary>
    /// Executes the <c>BindingMode</c> operation.
    /// </summary>
    /// <param name="bindingMode">The value used for <paramref name="bindingMode"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> BindingMode(BindingMode bindingMode)
    {
        this.bindingMode = bindingMode;
        return this;
    }

    /// <summary>
    /// Executes the <c>Converter</c> operation.
    /// </summary>
    /// <param name="converter">The value used for <paramref name="converter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Converter(IValueConverter converter)
    {
        this.converter = converter;
        return this;
    }

    /// <summary>
    /// Executes the <c>Parameter</c> operation.
    /// </summary>
    /// <param name="converterParameter">The value used for <paramref name="converterParameter"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Parameter(string converterParameter)
    {
        this.converterParameter = converterParameter;
        return this;
    }

    /// <summary>
    /// Executes the <c>Source</c> operation.
    /// </summary>
    /// <param name="source">The value used for <paramref name="source"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Source(object source)
    {
        this.source = source;
        return this;
    }

    /// <summary>
    /// Executes the <c>FallbackValue</c> operation.
    /// </summary>
    /// <param name="fallbackValue">The value used for <paramref name="fallbackValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> FallbackValue(object fallbackValue)
    {
        this.fallbackValue = fallbackValue;
        return this;
    }

    /// <summary>
    /// Executes the <c>TargetNullValue</c> operation.
    /// </summary>
    /// <param name="targetNullValue">The value used for <paramref name="targetNullValue"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> TargetNullValue(object targetNullValue)
    {
        this.targetNullValue = targetNullValue;
        return this;
    }

    /// <summary>
    /// Executes the <c>Convert</c> operation.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> Convert<Q>(Func<Q, T> convert)
    {
        if (valueConverter == null)
        {
            valueConverter = new ValueConverter();
        }

        valueConverter.ConvertFunction = (object e) => convert((Q)e);
        converter = valueConverter;
        return this;
    }

    /// <summary>
    /// Executes the <c>ConvertBack</c> operation.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <returns>The current builder instance for fluent chaining.</returns>
    public PropertySettersBindingBuilder<T> ConvertBack<Q>(Func<T, Q> convert)
    {
        if (valueConverter == null)
        {
            valueConverter = new ValueConverter();
        }

        valueConverter.ConvertBackFunction = (T e) => convert(e);
        converter = valueConverter;
        return this;
    }
}
