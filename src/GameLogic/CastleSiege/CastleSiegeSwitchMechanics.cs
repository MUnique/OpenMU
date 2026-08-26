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
        var switchInfos = context.ActiveNpcs
            .Select(runtime => runtime.SpawnedInstance)
            .OfType<CastleSiegeSwitch>()
            .OrderBy(candidate => candidate.SwitchIndex)
            .Select(siegeSwitch => CreateSwitchInfo(
                context,
                siegeSwitch,
                context.SwitchUsers[siegeSwitch.SwitchIndex]))
            .ToList();

        context.IsCrownAvailable = AreSwitchesHeldBySameAttackingSide(context);
        foreach (var crown in context.ActiveNpcs
                     .Select(runtime => runtime.SpawnedInstance)
                     .OfType<CastleSiegeCrown>())
        {
            crown.State = context.IsCrownAvailable
                ? CastleSiegeCrownState.Idle
                : CastleSiegeCrownState.Locked;
        }

        await context.ForEachSiegePlayerAsync(async player =>
        {
            foreach (var switchInfo in switchInfos)
            {
                await player.InvokeViewPlugInAsync<ICastleSiegeSwitchInfoPlugIn>(
                        plugIn => plugIn.ShowSwitchInfoAsync(switchInfo))
                    .ConfigureAwait(false);
            }

            await player.InvokeViewPlugInAsync<ICastleSiegeCrownStatePlugIn>(
                    plugIn => plugIn.ShowCrownStateAsync(context.IsCrownAvailable))
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
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
        return new(
            siegeSwitch.Id,
            occupant is not null,
            side,
            guildName,
            occupant?.Name ?? string.Empty);
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
