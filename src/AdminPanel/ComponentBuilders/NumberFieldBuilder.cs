// <copyright file="NumberFieldBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.ComponentBuilders
{
    using System.Reflection;
    using Microsoft.AspNetCore.Components.Rendering;
    using MUnique.OpenMU.AdminPanel.Components.Form;

    /// <summary>
    /// A <see cref="IComponentBuilder" /> for number fields.
    /// </summary>
    /// <typeparam name="TNumber">The type of the number.</typeparam>
    public class NumberFieldBuilder<TNumber> : BaseComponentBuilder, IComponentBuilder
    {
        /// <inheritdoc/>
        public int BuildComponent(object model, PropertyInfo propertyInfo, RenderTreeBuilder builder, int currentIndex) => this.BuildField<TNumber, NumberField<TNumber>>(model, propertyInfo, builder, currentIndex);

        /// <inheritdoc/>
        public bool CanBuildComponent(PropertyInfo propertyInfo) => propertyInfo.PropertyType == typeof(TNumber);
    }
}