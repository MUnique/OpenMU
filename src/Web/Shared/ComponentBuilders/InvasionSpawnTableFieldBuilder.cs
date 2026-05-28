// <copyright file="InvasionSpawnTableFieldBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.ComponentBuilders;

using System.Reflection;
using Microsoft.AspNetCore.Components.Rendering;
using MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;
using MUnique.OpenMU.Web.Shared.Components.Form;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// A <see cref="IComponentBuilder" /> for <see cref="IList{InvasionSpawnConfiguration}"/> fields.
/// </summary>
public class InvasionSpawnTableFieldBuilder : BaseComponentBuilder, IComponentBuilder
{
    /// <inheritdoc/>
    public int BuildComponent(object model, PropertyInfo propertyInfo, RenderTreeBuilder builder, int currentIndex, IChangeNotificationService notificationService)
        => this.BuildField<IList<InvasionSpawnConfiguration>>(model, typeof(InvasionSpawnTable), propertyInfo, builder, currentIndex, notificationService);

    /// <inheritdoc/>
    public bool CanBuildComponent(PropertyInfo propertyInfo)
        => propertyInfo.PropertyType == typeof(IList<InvasionSpawnConfiguration>);
}
