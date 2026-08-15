// <copyright file="CastleSiegeRegisterMarkAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Validates and processes Sign of Lord submissions.
/// </summary>
public class CastleSiegeRegisterMarkAction
{
    /// <summary>
    /// Tries to submit the Sign of Lord from an inventory slot.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context, if initialized.</param>
    /// <param name="inventorySlot">The inventory slot.</param>
    /// <returns>A task which represents the asynchronous operation.</returns>
    public async ValueTask RegisterMarkAsync(Player player, CastleSiegeContext? context, byte inventorySlot)
    {
        var (result, guildName, marks) = await RegisterMarkCoreAsync(player, context, inventorySlot).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeMarkRegistrationResultPlugIn>(
            view => view.ShowMarkRegistrationResultAsync(result, guildName, marks)).ConfigureAwait(false);
    }

    private static async ValueTask<(CastleSiegeMarkRegistrationResult Result, string GuildName, int Marks)> RegisterMarkCoreAsync(
        Player player,
        CastleSiegeContext? context,
        byte inventorySlot)
    {
        if (context is not { Configuration.Enabled: true })
        {
            return (CastleSiegeMarkRegistrationResult.Failed, string.Empty, 0);
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (context.CurrentState != CastleSiegeState.RegisterMark)
            {
                return (CastleSiegeMarkRegistrationResult.Failed, string.Empty, 0);
            }

            var (guild, _) = await CastleSiegeGuildResolver.ResolveAuthorizedGuildAsync(player).ConfigureAwait(false);
            if (guild is null
                || !context.RegisteredGuilds.TryGetValue(guild.PersistentId, out var registration))
            {
                return (CastleSiegeMarkRegistrationResult.GuildNotRegistered, guild?.Name ?? string.Empty, 0);
            }

            var signOfLord = player.Inventory?.GetItem(inventorySlot);
            if (signOfLord is null
                || signOfLord.Definition != context.Configuration.SignOfLordItemDefinition
                || signOfLord.Level != context.Configuration.SignOfLordItemLevel)
            {
                return (CastleSiegeMarkRegistrationResult.IncorrectItem, guild.Name, registration.Marks);
            }

            // Persist first so a failed registration never consumes the player's item. If item removal fails afterward,
            // the durable mark is preferred over losing an item without receiving credit.
            if (await context.IncrementMarksAsync(registration).ConfigureAwait(false) is not { } marks)
            {
                return (CastleSiegeMarkRegistrationResult.GuildNotRegistered, guild.Name, registration.Marks);
            }

            await player.DestroyInventoryItemAsync(signOfLord).ConfigureAwait(false);
            return (CastleSiegeMarkRegistrationResult.Success, guild.Name, marks);
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }
}
