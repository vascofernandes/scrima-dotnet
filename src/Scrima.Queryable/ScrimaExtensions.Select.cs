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
        Type itemType,
        Expression source,
        SelectQueryOption select)
    {
        var selectAllProperties = select == null ||
                                  select.IsStarSelect ||
                                  select.Expression == null ||
                                  select.Expression is not PropertyAccessNode ||
                                  (select.Expression is PropertyAccessNode temp && temp.Properties.Count == 0);

        var propAccessNode = select?.Expression as PropertyAccessNode;
        
        var memberBindings = new List<MemberBinding>();

        foreach (var property in itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (IsNavigationProperty(property, out _, out _))
            {
                // navigation properties that are not expanded, are not loaded
                // Prop1 = null
                memberBindings.Add(Expression.Bind(property, Expression.Constant(null, property.PropertyType)));
            }
            else if (selectAllProperties || (propAccessNode?.PropertiesMap.ContainsKey(property) ?? false))
            {
                // Prop1 = arg.prop1
                memberBindings.Add(Expression.Bind(property, Expression.MakeMemberAccess(source, property)));
            }
        }

        return Expression.MemberInit(
            Expression.New(itemType),
            memberBindings
        );
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
}
