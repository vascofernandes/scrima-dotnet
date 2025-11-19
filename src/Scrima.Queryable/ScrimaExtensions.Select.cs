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
        SelectQueryOption select,
        bool forceSelectAll = false)
    {
        var sourceType = source?.Type ?? throw new ArgumentNullException(nameof(source));

        var propAccessNode = select?.Expression as PropertyAccessNode;
        var selectAllProperties = forceSelectAll ||
                                  select == null ||
                                  select.IsStarSelect ||
                                  propAccessNode == null ||
                                  propAccessNode.Properties.Count == 0;

        var sourcePropertiesByName = sourceType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name, p => p, StringComparer.Ordinal);

        var memberBindings = new List<MemberBinding>();

        foreach (var property in projectionType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!sourcePropertiesByName.TryGetValue(property.Name, out var sourceProperty))
            {
                continue;
            }

            if (IsNavigationProperty(property, out var itemType, out var isCollection))
            {
                var isSelected = selectAllProperties || (propAccessNode?.PropertiesMap.ContainsKey(sourceProperty) ?? false);

                if (!isSelected)
                {
                    memberBindings.Add(Expression.Bind(property, Expression.Constant(null, property.PropertyType)));
                    continue;
                }

                var shouldSelectAllChildren = selectAllProperties;
                if (!shouldSelectAllChildren && propAccessNode != null)
                {
                    var anyChildSelected = false;
                    foreach (var childProp in itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (propAccessNode.PropertiesMap.ContainsKey(childProp))
                        {
                            anyChildSelected = true;
                            break;
                        }
                    }
                    shouldSelectAllChildren = !anyChildSelected;
                }

                Expression valueExpression = Expression.MakeMemberAccess(source, sourceProperty);

                if (isCollection)
                {
                    var childParam = Expression.Parameter(itemType, "child");
                    var childInit = BuildMemberInit(itemType, childParam, select, shouldSelectAllChildren);
                    var selector = Expression.Lambda(childInit, childParam);

                    var selectMethod = typeof(Enumerable)
                        .GetMethods()
                        .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(itemType, itemType);

                    Expression projectedValue = Expression.Call(selectMethod, valueExpression, selector);

                    if (!property.PropertyType.IsAssignableFrom(projectedValue.Type))
                    {
                        if (property.PropertyType.IsArray)
                        {
                            var toArrayMethod = typeof(Enumerable)
                                .GetMethod("ToArray")
                                .MakeGenericMethod(itemType);
                            projectedValue = Expression.Call(toArrayMethod, projectedValue);
                        }
                        else if (typeof(List<>).MakeGenericType(itemType).IsAssignableFrom(property.PropertyType))
                        {
                            var toListMethod = typeof(Enumerable)
                                .GetMethod("ToList")
                                .MakeGenericMethod(itemType);
                            projectedValue = Expression.Call(toListMethod, projectedValue);
                        }
                    }
                    
                    memberBindings.Add(Expression.Bind(property, projectedValue));
                }
                else
                {
                    var inlineInit = BuildMemberInit(itemType, valueExpression, select, shouldSelectAllChildren);
                    var nullCheck = Expression.Equal(valueExpression, Expression.Constant(null, valueExpression.Type));
                    var projectedValue = Expression.Condition(nullCheck, Expression.Constant(null, property.PropertyType), inlineInit);
                    
                    memberBindings.Add(Expression.Bind(property, projectedValue));
                }
                
                continue;
            }

            if (!selectAllProperties && !(propAccessNode?.PropertiesMap.ContainsKey(sourceProperty) ?? false))
            {
                continue;
            }

            Expression valueExpression1 = Expression.MakeMemberAccess(source, sourceProperty);

            if (!TryCreateAssignmentExpression(property.PropertyType, ref valueExpression1))
            {
                continue;
            }

            memberBindings.Add(Expression.Bind(property, valueExpression1));
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
