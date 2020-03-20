// <copyright file="NavigationExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Reflection;
    using Microsoft.EntityFrameworkCore.Metadata;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// Extensions for <see cref="INavigation"/>s.
    /// </summary>
    public static class NavigationExtensions
    {
        /// <summary>
        /// Determines whether the specified navigation is a member of the aggregate.
        /// </summary>
        /// <param name="navigation">The navigation.</param>
        /// <returns>
        ///   <c>true</c> if the specified navigation is a member of the aggregate; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMemberOfAggregate(this INavigation navigation)
        {
            var propertyInfo = navigation.PropertyInfo;
            if (propertyInfo.Name.StartsWith("Raw"))
            {
                propertyInfo = propertyInfo.DeclaringType?.GetProperty(propertyInfo.Name.Substring(3), BindingFlags.Instance | BindingFlags.Public);
            }

            return propertyInfo?.GetCustomAttribute<MemberOfAggregateAttribute>() is { };
        }
    }
}
