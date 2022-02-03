// <copyright file="ExitGateFieldBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.ComponentBuilders;

using System.Reflection;
using Microsoft.AspNetCore.Components.Rendering;
using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Web.AdminPanel.Components.Form;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// A <see cref="IComponentBuilder"/> for exit gate fields which are not a member of the aggregate.
/// </summary>
public class ExitGateFieldBuilder : BaseComponentBuilder, IComponentBuilder
{
    /// <inheritdoc/>
    public int BuildComponent(object model, PropertyInfo propertyInfo, RenderTreeBuilder builder, int currentIndex, IChangeNotificationService notificationService)
    {
        return this.BuildField<ExitGate>(model, typeof(ExitGateField), propertyInfo, builder, currentIndex, notificationService);
    }

    /// <inheritdoc/>
    public bool CanBuildComponent(PropertyInfo propertyInfo) =>
        propertyInfo.PropertyType == typeof(ExitGate)
        && propertyInfo.GetCustomAttribute<MemberOfAggregateAttribute>() is null;
}