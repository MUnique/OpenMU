﻿// <copyright file="ValueListFieldBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.ComponentBuilders;

using System.Reflection;
using Microsoft.AspNetCore.Components.Rendering;
using MUnique.OpenMU.Web.AdminPanel.Components.Form;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// A <see cref="IComponentBuilder"/> for lists of value types.
/// </summary>
public class ValueListFieldBuilder : BaseComponentBuilder, IComponentBuilder
{
    /// <inheritdoc/>
    public int BuildComponent(object model, PropertyInfo propertyInfo, RenderTreeBuilder builder, int currentIndex, IChangeNotificationService notificationService)
        => this.BuiltItemTableField(model, builder, propertyInfo, currentIndex, notificationService);

    /// <inheritdoc/>
    public bool CanBuildComponent(PropertyInfo propertyInfo) =>
        propertyInfo.PropertyType.IsInterface
        && propertyInfo.PropertyType.IsGenericType
        && propertyInfo.PropertyType.GetGenericTypeDefinition() is { } genericTypeDefinition
        && (genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(List<>))
        && propertyInfo.PropertyType.GenericTypeArguments[0] is { IsValueType: true, } valueType
        && valueType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IParsable<>));

    private int BuiltItemTableField(object model, RenderTreeBuilder builder, PropertyInfo propertyInfo, int i, IChangeNotificationService notificationService)
    {
        var method = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            .Where(m => m.Name == nameof(this.BuildField))
            .First(m => m.ContainsGenericParameters && m.GetGenericArguments().Length == 2)
            .MakeGenericMethod(propertyInfo.PropertyType, typeof(ValueTable<>).MakeGenericType(propertyInfo.PropertyType.GenericTypeArguments[0]));
        var parameters = new[] { model, propertyInfo, builder, i, notificationService };
        return (int)method.Invoke(this, parameters)!;
    }
}