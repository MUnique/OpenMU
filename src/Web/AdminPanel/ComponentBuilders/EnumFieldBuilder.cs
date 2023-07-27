// <copyright file="EnumFieldBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.ComponentBuilders;

using System.Reflection;
using Microsoft.AspNetCore.Components.Rendering;
using MUnique.OpenMU.Web.AdminPanel.Components.Form;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// A <see cref="IComponentBuilder"/> for enum fields.
/// </summary>
public class EnumFieldBuilder : BaseComponentBuilder, IComponentBuilder
{
    /// <inheritdoc/>
    public int BuildComponent(object model, PropertyInfo propertyInfo, RenderTreeBuilder builder, int currentIndex, IChangeNotificationService notificationService)
    {
        return this.BuildGenericField(model, typeof(EnumField<>), builder, propertyInfo, currentIndex, notificationService);
    }

    /// <inheritdoc/>
    public bool CanBuildComponent(PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.IsEnum && propertyInfo.PropertyType.GetCustomAttribute<FlagsAttribute>() is null;
    }
}