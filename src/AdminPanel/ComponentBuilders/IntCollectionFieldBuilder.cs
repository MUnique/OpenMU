// <copyright file="IntCollectionFieldBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.ComponentBuilders
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.AspNetCore.Components.Rendering;
    using MUnique.OpenMU.AdminPanel.Components.Form;

    /// <summary>
    /// A <see cref="IComponentBuilder" /> for <see cref="ICollection{T}"/> of <see cref="int"/> fields.
    /// </summary>
    public class IntCollectionFieldBuilder : BaseComponentBuilder, IComponentBuilder
    {
        /// <inheritdoc/>
        public int BuildComponent(object model, PropertyInfo propertyInfo, RenderTreeBuilder builder, int currentIndex) => this.BuildField<ICollection<int>>(model, typeof(IntCollectionField), propertyInfo, builder, currentIndex);

        /// <inheritdoc/>
        public bool CanBuildComponent(PropertyInfo propertyInfo) => propertyInfo.PropertyType == typeof(ICollection<int>);
    }
}