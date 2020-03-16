// <copyright file="EmbeddedFormFieldBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.ComponentBuilders
{
    using System.Reflection;
    using Microsoft.AspNetCore.Components.Rendering;
    using MUnique.OpenMU.AdminPanel.Components.Form;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// A <see cref="IComponentBuilder"/> for properties which are marked with the <see cref="MemberOfAggregateAttribute"/>.
    /// </summary>
    public class EmbeddedFormFieldBuilder : BaseComponentBuilder, IComponentBuilder
    {
        /// <inheritdoc/>
        public int BuildComponent(object model, PropertyInfo propertyInfo, RenderTreeBuilder builder, int currentIndex) => this.BuildGenericField(model, typeof(MemberOfAggregateField<>), builder, propertyInfo, currentIndex);

        /// <inheritdoc/>
        public bool CanBuildComponent(PropertyInfo propertyInfo) =>
            propertyInfo.PropertyType.IsClass
            && !propertyInfo.PropertyType.IsArray
            && propertyInfo.GetCustomAttribute<MemberOfAggregateAttribute>() is { };
    }
}