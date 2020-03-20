// <copyright file="IComponentBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.ComponentBuilders
{
    using System.Reflection;
    using Microsoft.AspNetCore.Components.Rendering;

    /// <summary>
    /// Interface for a component builder for a specific property;
    /// </summary>
    public interface IComponentBuilder
    {
        /// <summary>
        /// Determines whether this instance can build a component for the specified property.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>
        ///   <c>true</c> if this instance can build a component for the specified property; otherwise, <c>false</c>.
        /// </returns>
        bool CanBuildComponent(PropertyInfo propertyInfo);

        /// <summary>
        /// Builds the component for the specified property and binds it to the model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="treeBuilder">The tree builder.</param>
        /// <param name="currentIndex">The current index in the render tree.</param>
        /// <returns>The updated index in the render tree.</returns>
        int BuildComponent(object model, PropertyInfo propertyInfo, RenderTreeBuilder treeBuilder, int currentIndex);
    }
}