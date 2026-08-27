// <copyright file="CastleSiegeSwitchMechanics.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Broadcasts Castle Siege Crown switch occupants and Crown availability.
/// </summary>
public static class CastleSiegeSwitchMechanics
{
    /// <summary>
    /// Sends the current switch and Crown state to all players on the Castle Siege map.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns>A task that represents the asynchronous broadcast operation.</returns>
    public static async ValueTask SendSwitchInfoAsync(CastleSiegeContext context)
    {
        var switchInfos = CreateSwitchInfos(context);
        var crownAvailability = UpdateCrownState(context);
        var changedSwitches = switchInfos
            .Where(switchInfo => !context.LastBroadcastSwitchInfos.TryGetValue(switchInfo.ObjectId, out var previous)
                                 || !Equals(switchInfo, previous))
            .ToList();
        var crownStateChanged = context.LastBroadcastCrownAvailability != crownAvailability;
        if (changedSwitches.Count == 0 && !crownStateChanged)
        {
            return;
        }

        await context.ForEachSiegePlayerAsync(async player =>
        {
            foreach (var switchInfo in changedSwitches)
            {
                await player.InvokeViewPlugInAsync<ICastleSiegeSwitchInfoPlugIn>(
                        plugIn => plugIn.ShowSwitchInfoAsync(switchInfo))
                    .ConfigureAwait(false);
            }

            if (crownStateChanged)
            {
                await player.InvokeViewPlugInAsync<ICastleSiegeCrownStatePlugIn>(
                        plugIn => plugIn.ShowCrownStateAsync(crownAvailability))
                    .ConfigureAwait(false);
            }
        }).ConfigureAwait(false);

        context.LastBroadcastSwitchInfos.Clear();
        foreach (var switchInfo in switchInfos)
        {
            context.LastBroadcastSwitchInfos[switchInfo.ObjectId] = switchInfo;
        }

        context.LastBroadcastCrownAvailability = crownAvailability;
    }

    /// <summary>
    /// Sends the current switch and Crown state to one player who entered the siege map.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="player">The player to synchronize.</param>
    /// <returns>A task that represents the asynchronous synchronization operation.</returns>
    public static async ValueTask SynchronizePlayerAsync(CastleSiegeContext context, Player player)
    {
        foreach (var switchInfo in CreateSwitchInfos(context))
        {
            await player.InvokeViewPlugInAsync<ICastleSiegeSwitchInfoPlugIn>(
                    plugIn => plugIn.ShowSwitchInfoAsync(switchInfo))
                .ConfigureAwait(false);
        }

        var crownAvailability = AreSwitchesHeldBySameAttackingSide(context);
        await player.InvokeViewPlugInAsync<ICastleSiegeCrownStatePlugIn>(
                plugIn => plugIn.ShowCrownStateAsync(crownAvailability))
            .ConfigureAwait(false);
    }

    private static List<CastleSiegeSwitchInfo> CreateSwitchInfos(CastleSiegeContext context)
    {
        return context.NpcController.GetRuntimeSnapshot()
            .Select(runtime => runtime.SpawnedInstance)
            .OfType<CastleSiegeSwitch>()
            .OrderBy(candidate => candidate.SwitchIndex)
            .Select(siegeSwitch => CreateSwitchInfo(
                context,
                siegeSwitch,
                context.SwitchUsers[siegeSwitch.SwitchIndex]))
            .ToList();
    }

    private static CastleSiegeSwitchInfo CreateSwitchInfo(
        CastleSiegeContext context,
        CastleSiegeSwitch siegeSwitch,
        Player? occupant)
    {
        var side = occupant is null
            ? CastleSiegeJoinSide.None
            : context.GetPlayerJoinSide(occupant);
        var guildName = occupant?.GuildStatus is { } guildStatus
                        && context.FinalGuildList.TryGetValue(guildStatus.GuildId, out var participant)
            ? participant.GuildName
            : string.Empty;

        // MuMain resolves the legacy switch-index field through its visible-object table, so the object id is required.
        return new(
            siegeSwitch.Id,
            occupant is not null,
            side,
            guildName,
            occupant?.Name ?? string.Empty);
    }

    private static bool UpdateCrownState(CastleSiegeContext context)
    {
        context.IsCrownAvailable = AreSwitchesHeldBySameAttackingSide(context);
        foreach (var crown in context.NpcController.GetRuntimeSnapshot()
                     .Select(runtime => runtime.SpawnedInstance)
                     .OfType<CastleSiegeCrown>())
        {
            crown.State = context.IsCrownAvailable
                ? CastleSiegeCrownState.Idle
                : CastleSiegeCrownState.Locked;
        }

        return context.IsCrownAvailable;
    }

    private static bool AreSwitchesHeldBySameAttackingSide(CastleSiegeContext context)
    {
        if (context.SwitchUsers[0] is not { IsAlive: true, GuildStatus: not null } firstSwitchUser
            || context.SwitchUsers[1] is not { IsAlive: true, GuildStatus: not null } secondSwitchUser)
        {
            return false;
        }

        var firstSide = context.GetPlayerJoinSide(firstSwitchUser);
        return firstSide is not CastleSiegeJoinSide.None and not CastleSiegeJoinSide.Defense
               && context.GetPlayerJoinSide(secondSwitchUser) == firstSide;
    }
}
