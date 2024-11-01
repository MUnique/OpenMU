// <copyright file="IMapEventStateUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// The events on map.
/// </summary>
public enum MapEventType
{
    /// <summary>
    /// Red Dragons start flying.
    /// </summary>
    RedDragonInvasion,

    /// <summary>
    /// Golden Dragons start flying.
    /// </summary>
    GoldenDragonInvasion,
}

/// <summary>
/// Interface of a view whose implementation informs about the status of a map event.
/// </summary>
public interface IMapEventStateUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the map event of the current map.
    /// </summary>
    /// <param name="enabled">If set to <c>true</c> this event enabled; Otherwise disabled.</param>
    /// <param name="mapEventType">The event type.</param>
    ValueTask UpdateStateAsync(bool enabled, MapEventType mapEventType);
}