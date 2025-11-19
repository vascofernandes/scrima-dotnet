using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Scrima.Core.Query;
using Scrima.Core.Query.Expressions;

namespace Scrima.Queryable;

public static partial class ScrimaExtensions
{
    public static IQueryable<TSource> Select<TSource>(this IQueryable<TSource> source, SelectQueryOption selectQueryOption)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (selectQueryOption == null) throw new ArgumentNullException(nameof(selectQueryOption));
        
        if (selectQueryOption.Expression is null) return source;

        var selectClause = BuildSelectLambdaExpression(
            source.ElementType,
            selectQueryOption);

        source = source.Select((Expression<Func<TSource, TSource>>) selectClause);
        
        return source;
    }
    
    public static IQueryable<TResult> Select<TSource,TResult>(this IQueryable<TSource> source, SelectQueryOption selectQueryOption)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (selectQueryOption == null) throw new ArgumentNullException(nameof(selectQueryOption));
        
        if (selectQueryOption.Expression is null) return source as IQueryable<TResult>;

        var selectClause = BuildSelectLambdaExpression(
            source.ElementType,
            typeof(TResult),
            selectQueryOption);

        var results = source.Select((Expression<Func<TSource, TResult>>) selectClause);
        
        return results;
    }

    internal static LambdaExpression BuildSelectLambdaExpression(
        Type itemType,
        SelectQueryOption selectClause)
    {
        var arg = Expression.Parameter(itemType, "arg");
        var body = BuildMemberInit(itemType, arg, selectClause);
        var funcType = typeof(Func<,>).MakeGenericType(itemType, itemType);

        // arg => new Item { ... }
        return Expression.Lambda(funcType, body, arg);
    }
    
    internal static LambdaExpression BuildSelectLambdaExpression(
        Type itemType,
        Type resultType,
        SelectQueryOption selectClause)
    {
        var arg = Expression.Parameter(itemType, "arg");
        var body = BuildMemberInit(resultType, arg, selectClause);
        var funcType = typeof(Func<,>).MakeGenericType(itemType, resultType);

        // arg => new Item { ... }
        return Expression.Lambda(funcType, body, arg);
    }
    
    private static MemberInitExpression BuildMemberInit(
        Type projectionType,
        Expression source,
        SelectQueryOption select)
    {
        var sourceType = source?.Type ?? throw new ArgumentNullException(nameof(source));

        var propAccessNode = select?.Expression as PropertyAccessNode;
        var selectAllProperties = select == null ||
                                  select.IsStarSelect ||
                                  propAccessNode == null ||
                                  propAccessNode.Properties.Count == 0;

        var sourcePropertiesByName = sourceType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name, p => p, StringComparer.Ordinal);

        var memberBindings = new List<MemberBinding>();

        foreach (var property in projectionType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (IsNavigationProperty(property, out _, out _))
            {
                memberBindings.Add(Expression.Bind(property, Expression.Constant(null, property.PropertyType)));
                continue;
            }

            if (!sourcePropertiesByName.TryGetValue(property.Name, out var sourceProperty))
            {
                continue;
            }

            if (!selectAllProperties && !(propAccessNode?.PropertiesMap.ContainsKey(sourceProperty) ?? false))
            {
                continue;
            }

            Expression valueExpression = Expression.MakeMemberAccess(source, sourceProperty);

            if (!TryCreateAssignmentExpression(property.PropertyType, ref valueExpression))
            {
                continue;
            }

            memberBindings.Add(Expression.Bind(property, valueExpression));
        }

        return Expression.MemberInit(Expression.New(projectionType), memberBindings);
    }

    internal static bool IsNavigationProperty(PropertyInfo property, out Type itemType, out bool isCollection)
    {
        // 1:many
        if (ReflectionHelper.IsEnumerable(property.PropertyType, out itemType))
        {
            isCollection = true;
            return true;
        }

        // * : 0..1
        if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
        {
            itemType = property.PropertyType;
            isCollection = false;
            return true;
        }

        itemType = null;
        isCollection = false;
        return false;
    }

    private static bool TryCreateAssignmentExpression(Type targetType, ref Expression valueExpression)
    {
        if (targetType == null) throw new ArgumentNullException(nameof(targetType));
        if (valueExpression == null) throw new ArgumentNullException(nameof(valueExpression));

        var sourceType = valueExpression.Type;

        if (targetType == sourceType)
        {
            return true;
        }

        if (targetType.IsAssignableFrom(sourceType))
        {
            valueExpression = Expression.Convert(valueExpression, targetType);
            return true;
        }

        var nullableTarget = Nullable.GetUnderlyingType(targetType);
        if (nullableTarget != null && nullableTarget == sourceType)
        {
            valueExpression = Expression.Convert(valueExpression, targetType);
            return true;
        }

        return false;
    }
}
