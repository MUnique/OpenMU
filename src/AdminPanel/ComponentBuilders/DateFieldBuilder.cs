// <copyright file="DateFieldBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.ComponentBuilders
{
    using System;
    using System.Reflection;
    using Microsoft.AspNetCore.Components.Rendering;
    using MUnique.OpenMU.AdminPanel.Components.Form;

    /// <summary>
    /// A <see cref="IComponentBuilder"/> for date fields.
    /// </summary>
    public class DateFieldBuilder : BaseComponentBuilder, IComponentBuilder
    {
        /// <inheritdoc/>
        public int BuildComponent(object model, PropertyInfo propertyInfo, RenderTreeBuilder builder, int currentIndex) => this.BuildField<DateTime, DateField>(model, propertyInfo, builder, currentIndex);

        /// <inheritdoc/>
        public bool CanBuildComponent(PropertyInfo propertyInfo) => propertyInfo.PropertyType == typeof(DateTime);
    }
}