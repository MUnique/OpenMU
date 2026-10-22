// <copyright file="CastleSiegeStatusAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Returns the current Castle Siege status, e.g. when a player talks to the guardsman.
/// </summary>
public class CastleSiegeStatusAction
{
    /// <summary>
    /// Sends the current Castle Siege status to the player.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context, if initialized.</param>
    /// <returns>A task which represents the asynchronous operation.</returns>
    public async ValueTask ShowStatusAsync(Player player, CastleSiegeContext? context)
    {
        var status = await GetStatusAsync(context).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeStatusPlugIn>(view => view.ShowStatusAsync(status)).ConfigureAwait(false);
    }

    private static async ValueTask<CastleSiegeStatus?> GetStatusAsync(CastleSiegeContext? context)
    {
        if (context is not { Configuration.Enabled: true, IsInitialized: true })
        {
            return null;
        }

        CastleSiegeState state;
        DateTime stateStartUtc;
        DateTime stateEndUtc;
        DateTime siegeStartUtc;
        TimeSpan remainingTime;
        Guid? ownerGuildId;
        string? selectedOwnerName;
        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            var utcNow = context.TimeProvider.GetUtcNow().UtcDateTime;
            state = context.CurrentState;
            stateStartUtc = context.StateStartTimeUtc;
            stateEndUtc = context.StateEndTimeUtc;
            remainingTime = context.GetRemainingTime(utcNow);

            // The client shows the battle start before the battle, and the remaining time during and after it.
            siegeStartUtc = state == CastleSiegeState.Start
                ? stateStartUtc
                : context.Schedule.GetNextStateStartUtc(CastleSiegeState.Start, utcNow) ?? stateEndUtc;
            ownerGuildId = context.SiegeData.IsOccupied ? context.SiegeData.OwnerGuildId : null;
            selectedOwnerName = ownerGuildId is { } id
                ? context.FinalGuildList.Values.FirstOrDefault(guild => guild.PersistentGuildId == id)?.GuildName
                : null;
        }
        finally
        {
            context.ExecutionLock.Release();
        }

        var (ownerGuildName, ownerGuildMasterName) = await GetOwnerNamesAsync(context, ownerGuildId, selectedOwnerName).ConfigureAwait(false);
        var timeZone = context.GameContext.ServerTimeZone;
        return new CastleSiegeStatus(
            state,
            ToServerTime(stateStartUtc, timeZone),
            ToServerTime(stateEndUtc, timeZone),
            ToServerTime(siegeStartUtc, timeZone),
            ownerGuildName,
            ownerGuildMasterName,
            remainingTime);
    }

    private static async ValueTask<(string GuildName, string GuildMasterName)> GetOwnerNamesAsync(
        CastleSiegeContext context,
        Guid? ownerGuildId,
        string? selectedOwnerName)
    {
        if (ownerGuildId is not { } persistentGuildId
            || context.GameContext is not IGameServerContext gameServerContext)
        {
            return (string.Empty, string.Empty);
        }

        var guildServer = gameServerContext.GuildServer;
        var runtimeGuildId = await guildServer.GetGuildIdAsync(persistentGuildId).ConfigureAwait(false);
        if (runtimeGuildId == 0)
        {
            return (selectedOwnerName ?? string.Empty, string.Empty);
        }

        var guildName = selectedOwnerName
                        ?? (await guildServer.GetGuildAsync(runtimeGuildId).ConfigureAwait(false))?.Name
                        ?? string.Empty;
        var members = await guildServer.GetGuildListAsync(runtimeGuildId).ConfigureAwait(false);
        var masterName = members.FirstOrDefault(member => member.PlayerPosition == GuildPosition.GuildMaster)?.PlayerName
                         ?? string.Empty;
        return (guildName, masterName);
    }

    private static DateTime ToServerTime(DateTime utc, TimeZoneInfo timeZone)
    {
        if (utc == DateTime.MinValue || utc == DateTime.MaxValue)
        {
            return utc;
        }

        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), timeZone);
    }
}
