// <copyright file="IGameEventPublishedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a <see cref="GameEvent"/> was published to the whole server.
/// </summary>
/// <remarks>
/// It's called by the <see cref="GameEvents.GameEventPlugInListener"/>, in the process which listens for the game events:
/// the all-in-one server, or the host of the Discord integration in the distributed deployment.
/// Implementations must return quickly, because the game server may wait for them.
/// </remarks>
[Guid("7F608827-2F99-43D4-AFA2-231F01B304DC")]
[PlugInPoint("Game event published", "Plugins which will be executed when an event of the game was published to the whole server, e.g. to forward it to an external system.")]
public interface IGameEventPublishedPlugIn
{
    /// <summary>
    /// Is called when a game event was published.
    /// </summary>
    /// <param name="gameEvent">The game event.</param>
    ValueTask GameEventPublishedAsync(GameEvent gameEvent);
}
