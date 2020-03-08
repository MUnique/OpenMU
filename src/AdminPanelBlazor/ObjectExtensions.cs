// <copyright file="ObjectExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanelBlazor
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// Extensions for objects.
    /// </summary>
    internal static class ObjectExtensions
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo> IdProperties = new ConcurrentDictionary<Type, PropertyInfo>();
        private static readonly ConcurrentDictionary<Type, PropertyInfo> NameProperties = new ConcurrentDictionary<Type, PropertyInfo>();

        /// <summary>
        /// Gets the guid identifier of an object, which has the name "Id".
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The guid identifier of an object, which has the name "Id".</returns>
        public static Guid GetId(this object item)
        {
            if (!IdProperties.TryGetValue(item.GetType(), out var idProperty))
            {
                idProperty = item.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals("Id") && p.PropertyType == typeof(Guid));
                IdProperties.TryAdd(item.GetType(), idProperty);
            }

            if (idProperty == null)
            {
                return Guid.Empty;
            }

            return (Guid)idProperty.GetValue(item);
        }

        /// <summary>
        /// Gets the name of an object, which has the name "Id".
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The guid identifier of an object, which has the name "Id".</returns>
        public static string GetName(this object item)
        {
            if (!NameProperties.TryGetValue(item.GetType(), out var nameProperty))
            {
                var properties = item.GetType().GetProperties().Where(p => p.PropertyType == typeof(string));
                nameProperty = properties.FirstOrDefault(p => p.Name.Equals("Name"))
                    ?? properties.FirstOrDefault(p => p.Name.Equals("Caption"))
                    ?? properties.FirstOrDefault(p => p.Name.Equals("Designation"))
                    ?? properties.FirstOrDefault(p => p.Name.Equals("Description"));
                NameProperties.TryAdd(item.GetType(), nameProperty);
            }

            if (nameProperty == null)
            {
                return item.ToString();
            }

            return (string)nameProperty.GetValue(item);
        }

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
}