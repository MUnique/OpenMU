// <copyright file="ExpressionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace MUnique.OpenMU.Web.AdminPanel;

using System.Linq.Expressions;
using System.Reflection;
using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// Extensions for expressions.
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// Creates an expression for a function which accesses the specified property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="owner">The owner of the property.</param>
    /// <param name="propertyInfo">The property information.</param>
    /// <returns>The created expression for a function which accesses the specified property.</returns>
    public static Expression<Func<TProperty>> CreatePropertyExpression<TProperty>(this object owner, PropertyInfo propertyInfo)
    {
        var constantExpr = Expression.Constant(owner, owner.GetType());
        var memberExpr = Expression.Property(constantExpr, propertyInfo.Name);
        var delegateType = typeof(Func<>).MakeGenericType(typeof(TProperty));
        return (Expression<Func<TProperty>>)Expression.Lambda(delegateType, memberExpr);
    }

    /// <summary>
    /// Determines whether the accessed member is marked with <see cref="MemberOfAggregateAttribute"/>.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns>
    ///   <c>true</c> if  the accessed member is marked with <see cref="MemberOfAggregateAttribute"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAccessToMemberOfAggregate<TProperty>(this Expression<Func<TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.GetCustomAttribute(typeof(MemberOfAggregateAttribute)) is { };
        }

        throw new ArgumentException("Expression Body must be a MemberExpression", nameof(expression));
    }

    /// <summary>
    /// Determines whether the accessed member is marked with <see cref="ScaffoldColumnAttribute"/>.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns>
    ///   <c>true</c> if  the accessed member is marked with <see cref="ScaffoldColumnAttribute"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool ScaffoldColumn<TProperty>(this Expression<Func<TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.GetCustomAttribute<ScaffoldColumnAttribute>() is { Scaffold: true };
        }

        throw new ArgumentException("Expression Body must be a MemberExpression", nameof(expression));
    }

    /// <summary>
    /// Determines whether the accessed member is marked with <see cref="TransientAttribute"/>.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns>
    ///   <c>true</c> if  the accessed member is marked with <see cref="TransientAttribute"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAccessToTransientProperty<TProperty>(this Expression<Func<TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.GetCustomAttribute(typeof(TransientAttribute)) is { };
        }

        throw new ArgumentException("Expression Body must be a MemberExpression", nameof(expression));
    }

    /// <summary>
    /// Gets the type of the accessed member.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns>The type of the accessed member.</returns>
    public static Type GetAccessedMemberType<TProperty>(this Expression<Func<TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            if (memberExpression.Type.IsGenericType)
            {
                return memberExpression.Type.GenericTypeArguments[0];
            }

            return memberExpression.Type;
        }

        throw new ArgumentException("Expression Body must be a MemberExpression", nameof(expression));
    }
}