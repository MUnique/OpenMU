// <copyright file="IActorFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// Creates the actors the <see cref="ActorRegistry"/> manages.
/// </summary>
public interface IActorFactory
{
    /// <summary>
    /// Creates an actor and lets it enter the world.
    /// </summary>
    /// <param name="context">The game server context to spawn on.</param>
    /// <param name="loginName">The login name of an existing account.</param>
    /// <param name="characterSlot">The character slot to animate; <c>null</c> takes the lowest slot.</param>
    /// <returns>The actor, or <c>null</c> when it could not enter the world.</returns>
    ValueTask<ScriptedPlayer?> CreateAsync(IGameServerContext context, string loginName, byte? characterSlot);
}

/// <summary>
/// Creates real actors which load their account from the database, like a bot does.
/// </summary>
public sealed class ActorFactory : IActorFactory
{
    /// <inheritdoc />
    public async ValueTask<ScriptedPlayer?> CreateAsync(IGameServerContext context, string loginName, byte? characterSlot)
    {
        var actor = new ScriptedPlayer(context);
        if (await actor.InitializeAsync(loginName, characterSlot).ConfigureAwait(false))
        {
            return actor;
        }

        await actor.DisposeAsync().ConfigureAwait(false);
        return null;
    }
}
