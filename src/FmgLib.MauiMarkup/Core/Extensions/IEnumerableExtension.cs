// Portions of this file incorporate and extend code originally from Sharp.UI.
// Copyright (c) 2022 Pawel Krzywdzinski
// Licensed under the MIT License. See THIRD-PARTY-NOTICES for details.

using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace FmgLib.MauiMarkup.Core;

public static partial class IEnumerableExtension
{
    public static void Add<T>(this T self, Func<T, T> configure)
        where T : BindableObject, IEnumerable
    {
        configure(self);
    }

    public static void Add<T>(this T self, Action<T> configure)
        where T : Layout, IEnumerable
    {
        configure(self);
    }

    public static TModel AddRangeMarkup<TModel, TCollection, TItem>(this TModel self, Expression<Func<TModel, TCollection>> propertyExpression, params TItem[] items) where TCollection : ICollection<TItem>
        => AddRangeMarkup(self, propertyExpression, items);

    public static TModel AddRangeMarkup<TModel, TCollection, TItem>(this TModel self, Expression<Func<TModel, TCollection>> propertyExpression, IEnumerable<TItem> items) where TCollection : ICollection<TItem>
    {
        try
        {
            var property = propertyExpression.Compile()(self);

            if (property == null)
            {
                property = (TCollection)Activator.CreateInstance(typeof(TCollection));

                var memberExpression = (MemberExpression)propertyExpression.Body;
                var propertyInfo = (PropertyInfo)memberExpression.Member;
                propertyInfo.SetValue(self, property);
            }

            foreach (var item in items)
                property.Add(item);

            return self;
        }
        catch (Exception)
        {
            return self;
        }
    }
}
