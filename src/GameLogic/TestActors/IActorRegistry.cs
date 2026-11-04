// <copyright file="IActorRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// Keeps the actors of this process, keyed by the login name of the account they animate.
/// </summary>
public interface IActorRegistry
{
    /// <summary>
    /// Spawns an actor for the given account on the given game server.
    /// </summary>
    /// <param name="serverId">The id of the game server to spawn on.</param>
    /// <param name="loginName">The login name of an existing account.</param>
    /// <param name="characterSlot">The character slot to animate; <c>null</c> takes the lowest slot.</param>
    /// <returns>The spawned actor, or the reason why it was refused.</returns>
    ValueTask<ActorCommandResult> SpawnAsync(int serverId, string loginName, byte? characterSlot);

    /// <summary>
    /// Stops an actor through the normal logout path, so its progress is saved.
    /// </summary>
    /// <param name="loginName">The login name of the actor.</param>
    /// <returns>The result; a failure when no such actor is animated.</returns>
    ValueTask<ActorCommandResult> StopAsync(string loginName);

    /// <summary>
    /// Stops every actor. Used on shutdown, before the game servers stop.
    /// </summary>
    /// <returns>The number of actors which were stopped.</returns>
    ValueTask<int> StopAllAsync();

    /// <summary>
    /// Gets the currently animated actors.
    /// </summary>
    /// <returns>The actors.</returns>
    ValueTask<IReadOnlyList<ScriptedPlayer>> ListAsync();

    /// <summary>
    /// Gets the actor which animates the given account, if any.
    /// </summary>
    /// <param name="loginName">The login name.</param>
    /// <returns>The actor, or <c>null</c>.</returns>
    ValueTask<ScriptedPlayer?> FindAsync(string loginName);
}
