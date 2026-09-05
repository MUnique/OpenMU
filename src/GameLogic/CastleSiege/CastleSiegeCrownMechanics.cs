// <copyright file="CastleSiegeCrownMechanics.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Implements Castle Siege Crown capture and ownership changes.
/// </summary>
public static class CastleSiegeCrownMechanics
{
    // GameContext executes periodic tasks once per second, so this permits at most two missed intervals.
    private static readonly TimeSpan MaximumProgressInterval = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Checks whether the Crown and both switches are held by the same attacking side.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="utcNow">The current UTC time.</param>
    /// <returns>A task that represents the asynchronous check operation.</returns>
    public static async ValueTask CheckMiddleWinnerAsync(CastleSiegeContext context, DateTime utcNow)
    {
        var elapsed = utcNow > context.LastCrownUpdateUtc
            ? utcNow - context.LastCrownUpdateUtc
            : TimeSpan.Zero;
        elapsed = elapsed > MaximumProgressInterval
            ? MaximumProgressInterval
            : elapsed;
        context.LastCrownUpdateUtc = utcNow;

        var crownUser = context.CrownUser;
        var previousCrownUser = context.PreviousCrownUser;
        if (previousCrownUser is not null
            && !ReferenceEquals(previousCrownUser, crownUser))
        {
            await FailAttemptAsync(context, previousCrownUser).ConfigureAwait(false);
            context.PreviousCrownUser = null;
        }

        var captureSide = GetCaptureSide(context, crownUser);
        if (captureSide is null)
        {
            if (context.PreviousCrownUser is { } interruptedUser)
            {
                await FailAttemptAsync(context, interruptedUser).ConfigureAwait(false);
                context.PreviousCrownUser = null;
            }
            else
            {
                CapAccumulatedTime(context);
            }

            return;
        }

        context.CrownAccumulatedTime += elapsed;
        context.PreviousCrownUser = crownUser;
        if (elapsed > TimeSpan.Zero)
        {
            await SendAccessStateAsync(
                    crownUser!,
                    CastleSiegeCrownAccessState.Attempt,
                    context.CrownAccumulatedTime)
                .ConfigureAwait(false);
        }

        var requiredTime = TimeSpan.FromSeconds(context.Configuration.CrownHoldTimeSeconds);
        if (context.CrownAccumulatedTime < requiredTime)
        {
            return;
        }

        await SendAccessStateAsync(
                crownUser!,
                CastleSiegeCrownAccessState.Success,
                context.CrownAccumulatedTime)
            .ConfigureAwait(false);
        await ChangeWinnerGuildAsync(context, crownUser!, captureSide.Value).ConfigureAwait(false);
    }

    /// <summary>
    /// Changes the intermediate owner after a successful Crown capture.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="crownUser">The player who captured the Crown.</param>
    /// <param name="capturingSide">The attacking side which captured the Crown.</param>
    /// <returns>A task that represents the asynchronous ownership change.</returns>
    internal static async ValueTask ChangeWinnerGuildAsync(
        CastleSiegeContext context,
        Player crownUser,
        CastleSiegeJoinSide capturingSide)
    {
        if (crownUser.GuildStatus is not { } guildStatus
            || !context.FinalGuildList.TryGetValue(guildStatus.GuildId, out var capturingGuild)
            || !IsAttackingSide(capturingSide)
            || capturingGuild.Side != capturingSide)
        {
            throw new InvalidOperationException("The Crown winner is not a selected attacking guild.");
        }

        context.MiddleOwnerGuildId = guildStatus.GuildId;
        foreach (var guild in context.FinalGuildList.Values)
        {
            if (guild.Side == capturingSide)
            {
                guild.Side = CastleSiegeJoinSide.Defense;
                continue;
            }

            if (guild.Side == CastleSiegeJoinSide.Defense)
            {
                guild.Side = capturingSide;
            }
        }

        var ownershipChanged = ApplyOwner(context, capturingGuild.PersistentGuildId);
        await context.SaveFinalGuildListAsync().ConfigureAwait(false);
        await context.SaveOwnerAsync().ConfigureAwait(false);
        if (ownershipChanged)
        {
            await CastleSiegeEconomyNotifier.BroadcastTaxRatesAsync(context).ConfigureAwait(false);
        }

        await context.SetPlayerJoinSideAsync().ConfigureAwait(false);
        await RespawnAttackersAsync(context).ConfigureAwait(false);

        context.IsCrownAvailable = false;
        foreach (var crown in context.NpcController.GetRuntimeSnapshot()
                     .Select(runtime => runtime.SpawnedInstance)
                     .OfType<CastleSiegeCrown>())
        {
            crown.State = CastleSiegeCrownState.Locked;
        }

        context.CrownAccumulatedTime = TimeSpan.Zero;
        context.CrownUser = null;
        context.PreviousCrownUser = null;
        Array.Clear(context.SwitchUsers);
        foreach (var siegeSwitch in context.NpcController.GetRuntimeSnapshot()
                     .Select(runtime => runtime.SpawnedInstance)
                     .OfType<CastleSiegeSwitch>())
        {
            siegeSwitch.Occupant = null;
        }

        await BroadcastOwnershipChangeAsync(context, capturingGuild.GuildName).ConfigureAwait(false);
    }

    /// <summary>
    /// Applies and persists the final Castle Siege result.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns>A task that represents the asynchronous result operation.</returns>
    internal static async ValueTask CheckResultAsync(CastleSiegeContext context)
    {
        CastleSiegeGuildParticipant? winner = null;
        if (context.MiddleOwnerGuildId is { } middleOwnerGuildId)
        {
            if (context.FinalGuildList.TryGetValue(middleOwnerGuildId, out var participant))
            {
                winner = participant;
            }
            else
            {
                context.GameContext.LoggerFactory
                    .CreateLogger(typeof(CastleSiegeCrownMechanics))
                    .LogWarning(
                        "The intermediate Castle Siege owner {guildId} is not in the selected guild list. The persisted owner is retained.",
                        middleOwnerGuildId);
            }
        }

        var ownershipChanged = winner is not null && ApplyOwner(context, winner.PersistentGuildId);

        await context.SaveOwnerAsync().ConfigureAwait(false);
        if (ownershipChanged)
        {
            await CastleSiegeEconomyNotifier.BroadcastTaxRatesAsync(context).ConfigureAwait(false);
        }

        var ownerName = winner?.GuildName
                        ?? await GetOwnerGuildNameAsync(context).ConfigureAwait(false)
                        ?? string.Empty;
        await BroadcastOwnershipChangeAsync(context, ownerName).ConfigureAwait(false);
    }

    /// <summary>
    /// Applies a new owner and resets the economy inherited from the preceding tenure.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="ownerGuildId">The persistent identifier of the new owner guild.</param>
    /// <returns>A value indicating whether the owner changed.</returns>
    internal static bool ApplyOwner(CastleSiegeContext context, Guid ownerGuildId)
    {
        if (context.SiegeData.IsOccupied
            && context.SiegeData.OwnerGuildId == ownerGuildId)
        {
            return false;
        }

        // A successful seal changes the castle lord immediately. The previous ownership tenure's economy must not
        // survive that handover, even when the former defender captures the Crown again before the battle ends.
        context.SiegeData.OwnerGuildId = ownerGuildId;
        context.SiegeData.IsOccupied = true;
        context.SiegeData.TaxChaos = 0;
        context.SiegeData.TaxStore = 0;
        context.SiegeData.TaxHunt = 0;
        context.SiegeData.TributeMoney = 0;
        context.SiegeData.IsHuntZoneEnabled = false;
        return true;
    }

    /// <summary>
    /// Broadcasts a Castle Siege ownership change to players on the siege map.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="guildName">The name of the new owner guild.</param>
    /// <returns>A task that represents the asynchronous broadcast.</returns>
    internal static async ValueTask BroadcastOwnershipChangeAsync(CastleSiegeContext context, string guildName)
    {
        await context.ForEachSiegePlayerAsync(async player =>
        {
            await player.InvokeViewPlugInAsync<ICastleSiegeOwnershipChangePlugIn>(
                    plugIn => plugIn.ShowOwnershipChangeAsync(guildName))
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    private static CastleSiegeJoinSide? GetCaptureSide(CastleSiegeContext context, Player? crownUser)
    {
        if (crownUser is not { IsAlive: true, GuildStatus: not null }
            || context.SwitchUsers[0] is not { IsAlive: true, GuildStatus: not null } firstSwitchUser
            || context.SwitchUsers[1] is not { IsAlive: true, GuildStatus: not null } secondSwitchUser)
        {
            return null;
        }

        var crownSide = context.GetPlayerJoinSide(crownUser);
        return IsAttackingSide(crownSide)
               && context.GetPlayerJoinSide(firstSwitchUser) == crownSide
               && context.GetPlayerJoinSide(secondSwitchUser) == crownSide
            ? crownSide
            : null;
    }

    private static bool IsAttackingSide(CastleSiegeJoinSide side)
    {
        return side is not CastleSiegeJoinSide.None and not CastleSiegeJoinSide.Defense;
    }

    private static async ValueTask FailAttemptAsync(CastleSiegeContext context, Player player)
    {
        CapAccumulatedTime(context);
        await SendAccessStateAsync(
                player,
                CastleSiegeCrownAccessState.Fail,
                context.CrownAccumulatedTime)
            .ConfigureAwait(false);
    }

    private static void CapAccumulatedTime(CastleSiegeContext context)
    {
        // Crown progress is shared across interrupted attempts and attacking sides by design.
        var maximumSeconds = Math.Max(context.Configuration.CrownHoldTimeSeconds, 1) - 1;
        var maximumTime = TimeSpan.FromSeconds(maximumSeconds);
        if (context.CrownAccumulatedTime > maximumTime)
        {
            context.CrownAccumulatedTime = maximumTime;
        }
    }

    private static ValueTask SendAccessStateAsync(
        Player player,
        CastleSiegeCrownAccessState state,
        TimeSpan accumulatedTime)
    {
        return player.InvokeViewPlugInAsync<ICastleSiegeCrownAccessStatePlugIn>(
            plugIn => plugIn.ShowCrownAccessStateAsync(state, accumulatedTime));
    }

    private static async ValueTask RespawnAttackersAsync(CastleSiegeContext context)
    {
        if (context.Configuration.AttackRespawnArea is not { } respawnArea
            || context.Configuration.CastleSiegeMapDefinition is not { } siegeMap)
        {
            return;
        }

        var respawnGate = new ExitGate
        {
            Map = siegeMap,
            X1 = respawnArea.X1,
            Y1 = respawnArea.Y1,
            X2 = respawnArea.X2,
            Y2 = respawnArea.Y2,
            Direction = Direction.South,
        };
        foreach (var player in context.GetSiegePlayers())
        {
            if (IsAttackingSide(context.GetPlayerJoinSide(player)))
            {
                await player.RespawnAtAsync(respawnGate).ConfigureAwait(false);
            }
        }
    }

    private static async ValueTask<string?> GetOwnerGuildNameAsync(CastleSiegeContext context)
    {
        if (context.SiegeData.OwnerGuildId is not { } ownerGuildId)
        {
            return null;
        }

        var selectedOwner = context.FinalGuildList.Values
            .FirstOrDefault(guild => guild.PersistentGuildId == ownerGuildId);
        if (selectedOwner is not null)
        {
            return selectedOwner.GuildName;
        }

        if (context.GameContext is not IGameServerContext gameServerContext)
        {
            return null;
        }

        var runtimeGuildId = await gameServerContext.GuildServer
            .GetGuildIdAsync(ownerGuildId)
            .ConfigureAwait(false);
        return runtimeGuildId == 0
            ? null
            : (await gameServerContext.GuildServer
                    .GetGuildAsync(runtimeGuildId)
                    .ConfigureAwait(false))
                ?.Name;
    }
}
