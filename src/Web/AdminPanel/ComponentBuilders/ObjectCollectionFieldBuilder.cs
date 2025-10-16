﻿// <copyright file="ObjectCollectionFieldBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.ComponentBuilders;

using System.Reflection;
using Microsoft.AspNetCore.Components.Rendering;
using MUnique.OpenMU.Web.AdminPanel.Components.Form;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// A <see cref="IComponentBuilder"/> for collections of objects.
/// </summary>
public class ObjectCollectionFieldBuilder : BaseComponentBuilder, IComponentBuilder
{
    /// <inheritdoc/>
    public int BuildComponent(object model, PropertyInfo propertyInfo, RenderTreeBuilder builder, int currentIndex, IChangeNotificationService notificationService)
        => this.BuiltItemTableField(model, builder, propertyInfo, currentIndex, notificationService);

    /// <inheritdoc/>
    public bool CanBuildComponent(PropertyInfo propertyInfo) =>
        propertyInfo.PropertyType.IsGenericType
        && propertyInfo.PropertyType.GetGenericTypeDefinition() is { } genericType
        && (genericType == typeof(ICollection<>) || genericType == typeof(IList<>) || genericType == typeof(List<>))
        && !propertyInfo.PropertyType.GenericTypeArguments[0].IsValueType;

    private int BuiltItemTableField(object model, RenderTreeBuilder builder, PropertyInfo propertyInfo, int i, IChangeNotificationService notificationService)
    {
        var elementType = propertyInfo.PropertyType.GenericTypeArguments[0];
        var valueType = typeof(ICollection<>).MakeGenericType(elementType);
        var componentGeneric = typeof(ItemTable<>).MakeGenericType(elementType);

        var method = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            .Where(m => m.Name == nameof(this.BuildField))
            .First(m => m.ContainsGenericParameters && m.GetGenericArguments().Length == 2)
            .MakeGenericMethod(valueType, componentGeneric);
        var parameters = new[] { model, propertyInfo, builder, i, notificationService };
        return (int)method.Invoke(this, parameters)!;
    }
}
