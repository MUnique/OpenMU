// <copyright file="ObjectCollectionFieldBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.ComponentBuilders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Components.Rendering;
    using MUnique.OpenMU.AdminPanel.Components.Form;

    /// <summary>
    /// A <see cref="IComponentBuilder"/> for collections of objects.
    /// </summary>
    public class ObjectCollectionFieldBuilder : BaseComponentBuilder, IComponentBuilder
    {
        /// <inheritdoc/>
        public int BuildComponent(object model, PropertyInfo propertyInfo, RenderTreeBuilder builder, int currentIndex) => this.BuiltItemTableField(model, builder, propertyInfo, currentIndex);

        /// <inheritdoc/>
        public bool CanBuildComponent(PropertyInfo propertyInfo) =>
            propertyInfo.PropertyType.IsInterface
            && propertyInfo.PropertyType.IsGenericType
            && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
            && !propertyInfo.PropertyType.GenericTypeArguments[0].IsValueType;

        private int BuiltItemTableField(object model, RenderTreeBuilder builder, PropertyInfo propertyInfo, int i)
        {
            var method = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(m => m.Name == nameof(this.BuildField))
                .First(m => m.ContainsGenericParameters && m.GetGenericArguments().Length == 2)
                .MakeGenericMethod(propertyInfo.PropertyType, typeof(ItemTable<>).MakeGenericType(propertyInfo.PropertyType.GenericTypeArguments[0]));
            var parameters = new[] { model, propertyInfo, builder, i };
            return (int)method.Invoke(this, parameters);
        }
    }
}