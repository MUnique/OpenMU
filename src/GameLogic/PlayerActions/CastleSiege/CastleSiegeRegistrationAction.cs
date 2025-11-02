// <copyright file="CastleSiegeRegistrationAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.CastleSiege;

using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to register a guild alliance for castle siege.
/// </summary>
public class CastleSiegeRegistrationAction
{
    /// <summary>
    /// Registers the player's guild alliance for castle siege.
    /// </summary>
    /// <param name="player">The player (must be alliance master).</param>
    public async ValueTask RegisterForSiegeAsync(Player player)
    {
        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext serverContext
            || player.CurrentMap is not { } currentMap)
        {
            await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationResultPlugIn>(
                p => p.ShowResultAsync(CastleSiegeRegistrationResult.NotInGuild)).ConfigureAwait(false);
            return;
        }

        if (guildStatus.Position != GuildPosition.GuildMaster)
        {
            await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationResultPlugIn>(
                p => p.ShowResultAsync(CastleSiegeRegistrationResult.NotTheGuildMaster)).ConfigureAwait(false);
            return;
        }

        // Get the alliance master guild ID
        var allianceMasterGuildId = await serverContext.GuildServer.GetAllianceMasterGuildIdAsync(guildStatus.GuildId).ConfigureAwait(false);

        // If not in an alliance or is an alliance member (not master), cannot register
        if (allianceMasterGuildId == 0)
        {
            await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationResultPlugIn>(
                p => p.ShowResultAsync(CastleSiegeRegistrationResult.NotInAlliance)).ConfigureAwait(false);
            return;
        }

        if (allianceMasterGuildId != guildStatus.GuildId)
        {
            await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationResultPlugIn>(
                p => p.ShowResultAsync(CastleSiegeRegistrationResult.NotAllianceMaster)).ConfigureAwait(false);
            return;
        }

        var castleSiegeContext = currentMap.CastleSiegeContext;
        if (castleSiegeContext is null || castleSiegeContext.State != CastleSiegeState.RegistrationOpen)
        {
            await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationResultPlugIn>(
                p => p.ShowResultAsync(CastleSiegeRegistrationResult.RegistrationClosed)).ConfigureAwait(false);
            return;
        }

        if (!castleSiegeContext.RegisterAlliance(allianceMasterGuildId))
        {
            await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationResultPlugIn>(
                p => p.ShowResultAsync(CastleSiegeRegistrationResult.AlreadyRegistered)).ConfigureAwait(false);
            return;
        }

        await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationResultPlugIn>(
            p => p.ShowResultAsync(CastleSiegeRegistrationResult.Success)).ConfigureAwait(false);
    }

    /// <summary>
    /// Unregisters the player's guild alliance from castle siege.
    /// </summary>
    /// <param name="player">The player (must be alliance master).</param>
    public async ValueTask UnregisterFromSiegeAsync(Player player)
    {
        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext serverContext)
        {
            await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationResultPlugIn>(
                p => p.ShowResultAsync(CastleSiegeRegistrationResult.NotInGuild)).ConfigureAwait(false);
            return;
        }

        if (guildStatus.Position != GuildPosition.GuildMaster)
        {
            await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationResultPlugIn>(
                p => p.ShowResultAsync(CastleSiegeRegistrationResult.NotTheGuildMaster)).ConfigureAwait(false);
            return;
        }

        var allianceMasterGuildId = await serverContext.GuildServer.GetAllianceMasterGuildIdAsync(guildStatus.GuildId).ConfigureAwait(false);

        if (allianceMasterGuildId == 0 || allianceMasterGuildId != guildStatus.GuildId)
        {
            await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationResultPlugIn>(
                p => p.ShowResultAsync(CastleSiegeRegistrationResult.NotAllianceMaster)).ConfigureAwait(false);
            return;
        }

        var castleSiegeContext = player.CurrentMap?.CastleSiegeContext;
        if (castleSiegeContext is null || !castleSiegeContext.UnregisterAlliance(allianceMasterGuildId))
        {
            await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationResultPlugIn>(
                p => p.ShowResultAsync(CastleSiegeRegistrationResult.NotRegistered)).ConfigureAwait(false);
            return;
        }

        await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationResultPlugIn>(
            p => p.ShowResultAsync(CastleSiegeRegistrationResult.Unregistered)).ConfigureAwait(false);
    }

    /// <summary>
    /// Submits a guild mark for castle siege registration.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="itemSlot">The inventory slot containing the guild mark.</param>
    public async ValueTask SubmitGuildMarkAsync(Player player, byte itemSlot)
    {
        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext serverContext)
        {
            return;
        }

        var allianceMasterGuildId = await serverContext.GuildServer.GetAllianceMasterGuildIdAsync(guildStatus.GuildId).ConfigureAwait(false);

        if (allianceMasterGuildId == 0)
        {
            await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationResultPlugIn>(
                p => p.ShowResultAsync(CastleSiegeRegistrationResult.NotInAlliance)).ConfigureAwait(false);
            return;
        }

        // Validate that the player has an item in the specified slot (guild mark)
        var guildMark = player.Inventory?.GetItem(itemSlot);
        if (guildMark is null)
        {
            player.Logger.LogWarning("Player tried to submit guild mark but no item found at slot {0}. Character: [{1}], Account: [{2}]",
                itemSlot, player.SelectedCharacter?.Name, player.Account?.LoginName);
            return;
        }

        // Validate that the item is actually a guild mark (Sign of Lord: Group 14, Number 18)
        if (guildMark.Definition?.Group != 14 || guildMark.Definition?.Number != 18)
        {
            player.Logger.LogWarning("Player tried to submit invalid item as guild mark. Item: Group {0}, Number {1}. Character: [{2}], Account: [{3}]",
                guildMark.Definition?.Group, guildMark.Definition?.Number, player.SelectedCharacter?.Name, player.Account?.LoginName);
            return;
        }

        // Remove the guild mark from inventory
        await player.DestroyInventoryItemAsync(guildMark).ConfigureAwait(false);

        var castleSiegeContext = player.CurrentMap?.CastleSiegeContext;
        var totalMarks = castleSiegeContext?.AddGuildMarks(allianceMasterGuildId, 1) ?? 0;

        await player.InvokeViewPlugInAsync<IShowCastleSiegeMarkSubmittedPlugIn>(
            p => p.ShowMarkSubmittedAsync(totalMarks)).ConfigureAwait(false);
    }
}

/// <summary>
/// Result of castle siege registration operation.
/// </summary>
public enum CastleSiegeRegistrationResult
{
    /// <summary>
    /// Registration was successful.
    /// </summary>
    Success,

    /// <summary>
    /// Unregistration was successful.
    /// </summary>
    Unregistered,

    /// <summary>
    /// Player is not in a guild.
    /// </summary>
    NotInGuild,

    /// <summary>
    /// Player is not the guild master.
    /// </summary>
    NotTheGuildMaster,

    /// <summary>
    /// Guild is not in an alliance.
    /// </summary>
    NotInAlliance,

    /// <summary>
    /// Player is not the alliance master.
    /// </summary>
    NotAllianceMaster,

    /// <summary>
    /// Registration period is closed.
    /// </summary>
    RegistrationClosed,

    /// <summary>
    /// Alliance is already registered.
    /// </summary>
    AlreadyRegistered,

    /// <summary>
    /// Alliance is not registered.
    /// </summary>
    NotRegistered,
}
