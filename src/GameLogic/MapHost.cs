// <copyright file="MapHost.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// A game server which hosts a game map for the players of other game servers.
/// See <see cref="IMapHostLocator"/>.
/// </summary>
/// <param name="ServerId">The id of the hosting game server.</param>
/// <param name="Map">
/// The instance of the hosted map, if the hosting game server runs in the same process, as in the
/// all-in-one deployment. A player of another game server simply enters this instance, while it
/// stays connected to its own game server.
/// It's <c>null</c>, if the hosting game server runs in another process, as in a distributed
/// deployment. The game client would then have to switch to the hosting game server, which is not
/// supported yet.
/// </param>
public sealed record MapHost(byte ServerId, GameMap? Map);
