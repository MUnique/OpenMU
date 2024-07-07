// <copyright file="IObjectRemovedFromMapPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugin interface which is called when any <see cref="ILocateable"/> was removed from to the game map.
/// </summary>
[Guid("400CF19E-8EC2-49F0-80A1-F6E89ADD26DC")]
[PlugInPoint("Object removed from map", "Plugins which will be executed when any object is removed from the game map.")]
public interface IObjectRemovedFromMapPlugIn
{
    /// <summary>
    /// Is called when any <see cref="ILocateable"/> was removed from to the game map.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <param name="removedObject">The removed object.</param>
    ValueTask ObjectRemovedFromMapAsync(GameMap map, ILocateable removedObject);
}