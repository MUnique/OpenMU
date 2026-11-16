// <copyright file="ActorFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

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
