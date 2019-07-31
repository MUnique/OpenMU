// <copyright file="ILoadByProperty.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Collections;
    using Microsoft.EntityFrameworkCore.Metadata;

    /// <summary>
    /// Interface to load data by property.
    /// </summary>
    internal interface ILoadByProperty
    {
        /// <summary>
        /// Loads objects by property.
        /// </summary>
        /// <param name="property">The property of the object which should be compared.</param>
        /// <param name="propertyValue">The value of the property.</param>
        /// <returns>The enumeration of the loaded objects.</returns>
        IEnumerable LoadByProperty(IProperty property, object propertyValue);
    }
}
