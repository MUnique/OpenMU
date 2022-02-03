// <copyright file="LookupFieldBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.ComponentBuilders;

using System.Reflection;
using Microsoft.AspNetCore.Components.Rendering;
using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.Web.AdminPanel.Components.Form;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// A <see cref="IComponentBuilder"/> for lookup fields which are not a member of the aggregate.
/// </summary>
public class LookupFieldBuilder : BaseComponentBuilder, IComponentBuilder
{
    /// <inheritdoc/>
    public int BuildComponent(object model, PropertyInfo propertyInfo, RenderTreeBuilder builder, int currentIndex, IChangeNotificationService notificationService) => this.BuildGenericField(model, typeof(LookupField<>), builder, propertyInfo, currentIndex, notificationService);

    /// <inheritdoc/>
    public bool CanBuildComponent(PropertyInfo propertyInfo) =>
        propertyInfo.PropertyType.IsClass
        && !propertyInfo.PropertyType.IsArray
        && propertyInfo.GetCustomAttribute<MemberOfAggregateAttribute>() is null;
}