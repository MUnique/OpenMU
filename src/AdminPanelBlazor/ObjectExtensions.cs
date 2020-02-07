// <copyright file="ObjectExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanelBlazor
{
    using System;
    using System.Linq;

    /// <summary>
    /// Extensions for objects.
    /// </summary>
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Gets the guid identifier of an object, which has the name "Id".
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The guid identifier of an object, which has the name "Id".</returns>
        public static Guid GetId(this object item)
        {
            // TODO: Add cache
            var idProperty = item.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals("Id") && p.PropertyType == typeof(Guid));
            if (idProperty == null)
            {
                return Guid.Empty;
            }

            return (Guid)idProperty.GetValue(item);
        }
    }
}