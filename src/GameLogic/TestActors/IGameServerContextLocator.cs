// <copyright file="IGameServerContextLocator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Resolves the game server contexts which run in this process.
/// </summary>
public interface IGameServerContextLocator
{
    /// <summary>
    /// Gets every game server context of this process, by server id.
    /// </summary>
    IReadOnlyList<(int ServerId, IGameServerContext Context)> Contexts { get; }

    /// <summary>
    /// Gets the context of the game server with the given id.
    /// </summary>
    /// <param name="serverId">The server id.</param>
    /// <returns>The context, or <c>null</c> when this process does not host that server.</returns>
    IGameServerContext? GetContext(int serverId);
}
