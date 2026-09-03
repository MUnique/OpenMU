// <copyright file="CastleSiegeSummonLifeStoneAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

/// <summary>
/// Places a guild's Life Stone during a Castle Siege battle.
/// </summary>
public static class CastleSiegeSummonLifeStoneAction
{
    /// <summary>
    /// Tries to summon a Life Stone at the player's current position.
    /// </summary>
    /// <param name="player">The player using the Life Stone item.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns><see langword="true"/> when the Life Stone was placed; otherwise, <see langword="false"/>.</returns>
    public static async ValueTask<bool> SummonAsync(Player player, CastleSiegeContext? context)
    {
        if (context is not { Configuration.Enabled: true })
        {
            return false;
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            var side = context.GetPlayerJoinSide(player);
            if (context.CurrentState != CastleSiegeState.Start
                || !player.IsAlive
                || player.GuildStatus is not { } guildStatus
                || side == CastleSiegeJoinSide.None
                || context.Configuration.CastleSiegeMapDefinition?.Number != player.CurrentMap?.Definition.Number
                || context.LifeStones.Any(lifeStone => lifeStone.OwnerGuildId == guildStatus.GuildId))
            {
                return false;
            }

            return await context.CreateLifeStoneAsync(player, guildStatus.GuildId, side).ConfigureAwait(false) is not null;
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }
}
