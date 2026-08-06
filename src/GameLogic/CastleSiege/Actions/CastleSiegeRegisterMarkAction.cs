// <copyright file="CastleSiegeRegisterMarkAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Validates and processes Emblem of Lord submissions.
/// </summary>
public class CastleSiegeRegisterMarkAction
{
    private const byte EmblemGroup = 14;
    private const short EmblemNumber = 21;
    private const byte EmblemLevel = 3;

    /// <summary>
    /// Tries to submit the Emblem of Lord from an inventory slot.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context, if initialized.</param>
    /// <param name="inventorySlot">The inventory slot.</param>
    public async ValueTask RegisterMarkAsync(Player player, CastleSiegeContext? context, byte inventorySlot)
    {
        var (success, guildName, marks) = await RegisterMarkCoreAsync(player, context, inventorySlot).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeMarkRegistrationResultPlugIn>(
            view => view.ShowMarkRegistrationResultAsync(success, guildName, marks)).ConfigureAwait(false);
    }

    private static async ValueTask<(bool Success, string GuildName, int Marks)> RegisterMarkCoreAsync(
        Player player,
        CastleSiegeContext? context,
        byte inventorySlot)
    {
        if (context is not { Configuration.Enabled: true })
        {
            return (false, string.Empty, 0);
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (context.CurrentState != CastleSiegeState.RegisterMark)
            {
                return (false, string.Empty, 0);
            }

            var (guild, _) = await CastleSiegeGuildResolver.ResolveAuthorizedGuildAsync(player, context).ConfigureAwait(false);
            if (guild is null
                || !context.RegisteredGuilds.TryGetValue(guild.PersistentId, out var registration))
            {
                return (false, guild?.Name ?? string.Empty, 0);
            }

            var emblem = player.Inventory?.GetItem(inventorySlot);
            if (emblem is null
                || emblem.Definition is not { Group: EmblemGroup, Number: EmblemNumber }
                || emblem.Level != EmblemLevel)
            {
                return (false, guild.Name, registration.Marks);
            }

            await player.DestroyInventoryItemAsync(emblem).ConfigureAwait(false);
            var marks = await context.IncrementMarksAsync(registration).ConfigureAwait(false);
            return (true, guild.Name, marks);
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }
}
