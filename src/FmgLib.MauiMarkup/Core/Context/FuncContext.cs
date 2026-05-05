using System.Globalization;

namespace FmgLib.MauiMarkup;

public class FuncContext<TSource, TDest, TParam> : IValueConverter
{
    readonly Func<TSource?, TDest?>? convert;
    readonly Func<TDest?, TSource?>? convertBack;

    readonly Func<TSource?, TParam?, TDest?>? convertWithParam;
    readonly Func<TDest?, TParam?, TSource?>? convertBackWithParam;

    readonly Func<TSource?, TParam?, CultureInfo?, TDest?>? convertWithParamAndCulture;
    readonly Func<TDest?, TParam?, CultureInfo?, TSource?>? convertBackWithParamAndCulture;

    /// <summary>
    /// Initializes a new instance of the <c>FuncContext</c> class.
    /// </summary>
    /// <param name="convertWithParamAndCulture">The value used for <paramref name="convertWithParamAndCulture"/>.</param>
    /// <param name="convertBackWithParamAndCulture">The value used for <paramref name="convertBackWithParamAndCulture"/>.</param>
    public FuncContext(Func<TSource?, TParam?, CultureInfo?, TDest>? convertWithParamAndCulture = null, Func<TDest?, TParam?, CultureInfo?, TSource>? convertBackWithParamAndCulture = null)
    {
        this.convertWithParamAndCulture = convertWithParamAndCulture;
        this.convertBackWithParamAndCulture = convertBackWithParamAndCulture;
    }

    /// <summary>
    /// Initializes a new instance of the <c>FuncContext</c> class.
    /// </summary>
    /// <param name="convertWithParam">The value used for <paramref name="convertWithParam"/>.</param>
    /// <param name="convertBackWithParam">The value used for <paramref name="convertBackWithParam"/>.</param>
    public FuncContext(Func<TSource?, TParam?, TDest>? convertWithParam = null, Func<TDest?, TParam?, TSource>? convertBackWithParam = null)
    {
        this.convertWithParam = convertWithParam;
        this.convertBackWithParam = convertBackWithParam;
    }

    /// <summary>
    /// Initializes a new instance of the <c>FuncContext</c> class.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <param name="convertBack">The value used for <paramref name="convertBack"/>.</param>
    public FuncContext(Func<TSource?, TDest?>? convert = null, Func<TDest?, TSource?>? convertBack = null)
    {
        this.convert = convert;
        this.convertBack = convertBack;
    }

    /// <summary>
    /// Executes the <c>Convert</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <param name="targetType">The value used for <paramref name="targetType"/>.</param>
    /// <param name="parameter">The value used for <paramref name="parameter"/>.</param>
    /// <param name="culture">The value used for <paramref name="culture"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        if (convert != null)
        {
            return convert.Invoke(
                value != null ? (TSource)value : default(TSource));
        }

        if (convertWithParam != null)
        {
            return convertWithParam.Invoke(
                value != null ? (TSource)value : default(TSource),
                parameter != null ? (TParam)parameter : default(TParam));
        }

        if (convertWithParamAndCulture != null)
        {
            return convertWithParamAndCulture.Invoke(
                value != null ? (TSource)value : default(TSource),
                parameter != null ? (TParam)parameter : default(TParam),
                culture);
        }

        return default(TDest);
    }

    /// <summary>
    /// Executes the <c>ConvertBack</c> operation.
    /// </summary>
    /// <param name="value">The value used for <paramref name="value"/>.</param>
    /// <param name="targetType">The value used for <paramref name="targetType"/>.</param>
    /// <param name="parameter">The value used for <paramref name="parameter"/>.</param>
    /// <param name="culture">The value used for <paramref name="culture"/>.</param>
    /// <returns>The result produced by the operation.</returns>
    public object? ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        if (convertBack != null)
        {
            return convertBack.Invoke(
                value != null ? (TDest)value : default(TDest));
        }

        if (convertBackWithParam != null)
        {
            return convertBackWithParam.Invoke(
                value != null ? (TDest)value : default(TDest),
                parameter != null ? (TParam)parameter : default(TParam));
        }

        if (convertBackWithParamAndCulture != null)
        {
            return convertBackWithParamAndCulture.Invoke(
                value != null ? (TDest)value : default(TDest),
                parameter != null ? (TParam)parameter : default(TParam),
                culture);
        }

        return default(TSource);
    }
}

public class FuncContext<TSource, TDest> : FuncContext<TSource, TDest, object>
{
    /// <summary>
    /// Initializes a new instance of the <c>FuncContext</c> class.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <param name="convertBack">The value used for <paramref name="convertBack"/>.</param>
    public FuncContext(Func<TSource?, TDest>? convert = null, Func<TDest?, TSource>? convertBack = null)
        : base(convert, convertBack)
    {
    }
}

public class FuncContext<TSource> : FuncContext<TSource, object, object>
{
    /// <summary>
    /// Initializes a new instance of the <c>FuncContext</c> class.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <param name="convertBack">The value used for <paramref name="convertBack"/>.</param>
    public FuncContext(Func<TSource?, object>? convert = null, Func<object?, TSource>? convertBack = null)
        : base(convert, convertBack)
    {
    }
}

public class FuncContext : FuncContext<object, object, object>
{
    /// <summary>
    /// Initializes a new instance of the <c>FuncContext</c> class.
    /// </summary>
    /// <param name="convert">The value used for <paramref name="convert"/>.</param>
    /// <param name="convertBack">The value used for <paramref name="convertBack"/>.</param>
    public FuncContext(Func<object?, object>? convert = null, Func<object?, object>? convertBack = null)
        : base(convert, convertBack)
    {
    }
}

public class ToStringConverter : FuncContext<object, string>
{
    /// <summary>
    /// Initializes a new instance of the <c>ToStringConverter</c> class.
    /// </summary>
    /// <param name="format">The value used for <paramref name="format"/>.</param>
    public ToStringConverter(string format = "{0}")
        : base(o => string.Format(CultureInfo.InvariantCulture, format, o))
    {
    }
}

public class NotConverter : FuncContext<bool, bool>
{
    static readonly Lazy<NotConverter> instance = new(() => new NotConverter());

    public static NotConverter Instance => instance.Value;

    /// <summary>
    /// Initializes a new instance of the <c>NotConverter</c> class.
    /// </summary>
    /// <param name="t">The value used for <paramref name="t"/>.</param>
    public NotConverter() : base(t => !t, t => !t)
    {
    }
}
