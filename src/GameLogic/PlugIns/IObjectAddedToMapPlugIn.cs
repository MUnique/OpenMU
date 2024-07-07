// <copyright file="IObjectAddedToMapPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugin interface which is called when any <see cref="ILocateable"/> was added to the game map.
/// </summary>
[Guid("8EE3E80E-C8FD-4857-B8C6-C7CFFC8DA702")]
[PlugInPoint("Object added to map", "Plugins which will be executed when any object is added to the game map.")]
public interface IObjectAddedToMapPlugIn
{
    /// <summary>
    /// Is called when any <see cref="ILocateable"/> was added to the game map.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <param name="addedObject">The added object.</param>
    ValueTask ObjectAddedToMapAsync(GameMap map, ILocateable addedObject);
}