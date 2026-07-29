// Portions of this file incorporate and extend code originally from CommunityToolkit.Maui.Markup.
// Copyright (c) .NET Foundation and Contributors
// Licensed under the MIT License. See THIRD-PARTY-NOTICES for details.

using System.Linq.Expressions;

namespace FmgLib.MauiMarkup;

public static partial class TypedBindingExtensions
{
	
	public static TBindable Bind<TBindable, TBindingContext, TSource>(
		this TBindable bindable,
		BindableProperty targetProperty,
		Expression<Func<TBindingContext, TSource>> getter,
		Action<TBindingContext, TSource>? setter = null,
		BindingMode mode = BindingMode.Default,
		string? stringFormat = null,
		TBindingContext? source = default) where TBindable : BindableObject
	{
		return Bind<TBindable, TBindingContext, TSource, object?, object?>(
					bindable,
					targetProperty,
					getter,
					setter,
					mode,
					null,
					null,
					null,
					stringFormat,
					source,
					null,
					null);
	}

	public static TBindable Bind<TBindable, TBindingContext, TSource, TDest>(
		this TBindable bindable,
		BindableProperty targetProperty,
		Expression<Func<TBindingContext, TSource>> getter,
		Action<TBindingContext, TSource>? setter = null,
		BindingMode mode = BindingMode.Default,
		Func<TSource?, TDest>? convert = null,
		Func<TDest?, TSource>? convertBack = null,
		string? stringFormat = null,
		TBindingContext? source = default,
		TDest? targetNullValue = default,
		TDest? fallbackValue = default) where TBindable : BindableObject
	{
		return Bind<TBindable, TBindingContext, TSource, object?, TDest>(
					bindable,
					targetProperty,
					getter,
					setter,
					mode,
					convert is null ? null : (source, _) => convert(source),
					convertBack is null ? null : (dest, _) => convertBack(dest),
					null,
					stringFormat,
					source,
					targetNullValue,
					fallbackValue);
	}

	public static TBindable Bind<TBindable, TBindingContext, TSource, TDest>(
		this TBindable bindable,
		BindableProperty targetProperty,
		Expression<Func<TBindingContext, TSource>> getter,
		Action<TBindingContext, TSource>? setter = null,
		BindingMode mode = BindingMode.Default,
		IValueConverter? converter = null,
		string? stringFormat = null,
		TBindingContext? source = default,
		TDest? targetNullValue = default,
		TDest? fallbackValue = default) where TBindable : BindableObject
	{
		return Bind<TBindable, TBindingContext, TSource, object?, TDest>(
					bindable,
					targetProperty,
					getter,
					setter,
					mode,
					converter,
					null,
					stringFormat,
					source,
					targetNullValue,
					fallbackValue);
	}

	public static TBindable Bind<TBindable, TBindingContext, TSource, TDest>(
		this TBindable bindable,
		BindableProperty targetProperty,
		Func<TBindingContext, TSource> getter,
		(Func<TBindingContext, object?>, string)[] handlers,
		Action<TBindingContext, TSource>? setter = null,
		BindingMode mode = BindingMode.Default,
		IValueConverter? converter = null,
		string? stringFormat = null,
		TBindingContext? source = default,
		TDest? targetNullValue = default,
		TDest? fallbackValue = default) where TBindable : BindableObject
	{
		return Bind<TBindable, TBindingContext, TSource, object?, TDest>(
					bindable,
					targetProperty,
					getter,
					handlers,
					setter,
					mode,
					converter,
					null,
					stringFormat,
					source,
					targetNullValue,
					fallbackValue);
	}

	public static TBindable Bind<TBindable, TBindingContext, TSource, TParam, TDest>(
		this TBindable bindable,
		BindableProperty targetProperty,
		Expression<Func<TBindingContext, TSource>> getter,
		Action<TBindingContext, TSource>? setter = null,
		BindingMode mode = BindingMode.Default,
		Func<TSource?, TParam?, TDest>? convert = null,
		Func<TDest?, TParam?, TSource>? convertBack = null,
		TParam? converterParameter = default,
		string? stringFormat = null,
		TBindingContext? source = default,
		TDest? targetNullValue = default,
		TDest? fallbackValue = default) where TBindable : BindableObject
	{
		var getterFunc = ConvertExpressionToFunc(getter);

		return Bind(
				bindable,
				targetProperty,
				getterFunc,
				new (Func<TBindingContext, object?>, string)[] { ((TBindingContext b) => b, GetMemberName(getter)) },
				setter,
				mode,
				convert,
				convertBack,
				converterParameter,
				stringFormat,
				source,
				targetNullValue,
				fallbackValue);
	}

	public static TBindable Bind<TBindable, TBindingContext, TSource, TParam, TDest>(
		this TBindable bindable,
		BindableProperty targetProperty,
		Expression<Func<TBindingContext, TSource>> getter,
		Action<TBindingContext, TSource>? setter = null,
		BindingMode mode = BindingMode.Default,
		IValueConverter? converter = null,
		TParam? converterParameter = default,
		string? stringFormat = null,
		TBindingContext? source = default,
		TDest? targetNullValue = default,
		TDest? fallbackValue = default) where TBindable : BindableObject
	{
		var getterFunc = ConvertExpressionToFunc(getter);

		return Bind(
				bindable,
				targetProperty,
				getterFunc,
				new (Func<TBindingContext, object?>, string)[] { ((TBindingContext b) => b, GetMemberName(getter)) },
				setter,
				mode,
				converter,
				converterParameter,
				stringFormat,
				source,
				targetNullValue,
				fallbackValue);
	}

	public static Func<TBindingContext, TSource> ConvertExpressionToFunc<TBindingContext, TSource>(in Expression<Func<TBindingContext, TSource>> expression) => expression.Compile();

	public static string GetMemberName<T>(in Expression<T> expression) => expression.Body switch
	{
		MemberExpression m => m.Member.Name,
		UnaryExpression u when u.Operand is MemberExpression m => m.Member.Name,
		_ => throw new InvalidOperationException("Could not retreive member name")
	};
}
