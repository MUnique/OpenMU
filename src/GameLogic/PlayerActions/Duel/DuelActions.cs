// <copyright file="DuelActions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Duel;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Actions regarding duels.
/// </summary>
public class DuelActions
{
    /// <summary>
    /// Handles a duel request.
    /// </summary>
    /// <param name="player">The player which sends the request.</param>
    /// <param name="target">The target player which receives the request.</param>
    public async ValueTask HandleDuelRequestAsync(Player player, Player target)
    {
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(target);

        if (player.DuelRoom is not null)
        {
            await player.ShowMessageAsync("You are already in a duel.").ConfigureAwait(false);
            return;
        }

        if (target.DuelRoom is not null)
        {
            await player.ShowMessageAsync("The other player is already in a duel.").ConfigureAwait(false);
            return;
        }

        if (!await CheckIfDuelCanBeStartedAsync(player, target).ConfigureAwait(false))
        {
            return;
        }

        if (await player.GameContext.DuelRoomManager.GetFreeDuelRoomAsync(player, target).ConfigureAwait(false) is not { } duelRoom)
        {
            await player.InvokeViewPlugInAsync<IShowDuelRequestResultPlugIn>(p => p.ShowDuelRequestResultAsync(DuelStartResult.FailedByNoFreeRoom, target)).ConfigureAwait(false);
            return;
        }

        duelRoom.State = DuelState.DuelRequested;
        player.DuelRoom = duelRoom;
        target.DuelRoom = duelRoom;

        await target.InvokeViewPlugInAsync<IShowDuelRequestPlugIn>(p => p.ShowDuelRequestAsync(player)).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles the duel response.
    /// </summary>
    /// <param name="player">The player which responds to the request.</param>
    /// <param name="target">The target, which is the requester of the duel.</param>
    /// <param name="accepted">If set to <c>true</c> the duel request was accepted by the player.</param>
    public async ValueTask HandleDuelResponseAsync(Player player, Player target, bool accepted)
    {
        if (player.DuelRoom is not { } duelRoom)
        {
            player.Logger.LogWarning($"Player {player.Name} sent duel response, but has no context.");
            return;
        }

        if (duelRoom != target.DuelRoom || duelRoom.Requester != target)
        {
            player.Logger.LogWarning($"Player {player.Name} sent duel response, but the duel contexts didn't match.");
            return;
        }

        if (!await CheckIfDuelCanBeStartedAsync(player, target).ConfigureAwait(false))
        {
            await duelRoom.ResetAndDisposeAsync(DuelStartResult.FailedByError).ConfigureAwait(false);
            return;
        }

        if (!accepted)
        {
            await duelRoom.ResetAndDisposeAsync(DuelStartResult.Refused).ConfigureAwait(false);
            return;
        }

        var duelConfig = player.GameContext.Configuration.DuelConfiguration;
        if (duelConfig is null)
        {
            player.Logger.LogError("Duel configuration is not set.");
            await duelRoom.ResetAndDisposeAsync(DuelStartResult.FailedByError).ConfigureAwait(false);
            return;
        }

        if (duelConfig.DuelAreas.FirstOrDefault(area => area.Index == duelRoom.Index) is not { } duelArea)
        {
            player.Logger.LogError("Duel area with index {index} was not found.", duelRoom.Index);
            await duelRoom.ResetAndDisposeAsync(DuelStartResult.FailedByError).ConfigureAwait(false);
            return;
        }

        if (duelArea.FirstPlayerGate is null || duelArea.SecondPlayerGate is null)
        {
            player.Logger.LogError("Duel area with index {index} has missing exit gates.", duelRoom.Index);
            await duelRoom.ResetAndDisposeAsync(DuelStartResult.FailedByError).ConfigureAwait(false);
            return;
        }

        // we risk that the money is not sufficient anymore,
        // but we don't care anymore if it fails.
        player.TryRemoveMoney(duelConfig.EntranceFee);
        target.TryRemoveMoney(duelConfig.EntranceFee);
        duelRoom.State = DuelState.DuelAccepted;

        await duelRoom.Requester.WarpToAsync(duelArea.FirstPlayerGate).ConfigureAwait(false);
        await duelRoom.Opponent.WarpToAsync(duelArea.SecondPlayerGate).ConfigureAwait(false);

        _ = Task.Run(duelRoom.RunDuelAsync);
    }

    /// <summary>
    /// Handles the duel stop request of a player.
    /// </summary>
    /// <param name="player">The player which requests to stop the duel.</param>
    public async ValueTask HandleStopDuelRequestAsync(Player player)
    {
        var duelRoom = player.DuelRoom;
        if (duelRoom is null)
        {
            player.Logger.LogWarning($"Player {player.Name} sent request to stop the duel, but it has no duel active.");
            return;
        }

        if (duelRoom.State is DuelState.DuelFinished)
        {
            player.Logger.LogWarning($"Player {player.Name} sent request to stop the duel, but it is already finished.");
            return;
        }

        await duelRoom.CancelDuelAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Handles the duel channel join request asynchronous.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="requestedDuelIndex">Index of the requested duel.</param>
    /// <returns>The value task with the result.</returns>
    public async ValueTask HandleDuelChannelJoinRequestAsync(Player player, byte requestedDuelIndex)
    {
        var config = player.GameContext.Configuration.DuelConfiguration;

        if (config is null)
        {
            player.Logger.LogError("Duel configuration is not set.");
            return;
        }

        if (player.GameContext.DuelRoomManager.GetRoomByIndex(requestedDuelIndex) is not { } duelRoom)
        {
            player.Logger.LogWarning($"Player {player.Name} tried to join duel channel with index {requestedDuelIndex}, but it doesn't exist.");
            return;
        }

        if (duelRoom.Spectators.Count >= config.MaximumSpectatorsPerDuelRoom)
        {
            player.Logger.LogWarning($"Player {player.Name} tried to join duel channel with index {requestedDuelIndex}, but it is full.");
            await player.ShowMessageAsync("The duel channel is full.").ConfigureAwait(false);
            return;
        }

        // move to duel map
        var spectatorsGate = config.DuelAreas.FirstOrDefault(area => area.Index == requestedDuelIndex)?.SpectatorsGate;
        if (spectatorsGate is null)
        {
            player.Logger.LogError("Duel area with index {index} was not found or has missing spectators gate.", requestedDuelIndex);
            return;
        }

        if (!await duelRoom.TryAddSpectatorAsync(player).ConfigureAwait(false))
        {
            await player.ShowMessageAsync("The duel channel is full.").ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Handles the duel channel quit request asynchronous.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The value task with the result.</returns>
    public async ValueTask HandleDuelChannelQuitRequestAsync(Player player)
    {
        if (player.DuelRoom is not { } duelRoom)
        {
            return;
        }

        if (duelRoom.IsDuelist(player))
        {
            // todo: log this attempt?
            return;
        }

        if (player.GameContext.Configuration.DuelConfiguration is not { } config)
        {
            return;
        }

        if (config.DuelAreas.FirstOrDefault(area => area.Index == duelRoom.Index) is not { } area)
        {
            return;
        }

        var isInDuelMap = area.SpectatorsGate?.Map == player.CurrentMap?.Definition;
        if (!isInDuelMap)
        {
            return;
        }

        if (config.Exit is { Map: not null } exitGate)
        {
            await player.WarpToAsync(exitGate).ConfigureAwait(false);
        }
        else
        {
            await player.WarpToSafezoneAsync().ConfigureAwait(false);
        }

        await player.RemoveInvisibleEffectAsync().ConfigureAwait(false);
        await duelRoom.RemoveSpectatorAsync(player).ConfigureAwait(false);
    }

    private static async ValueTask<bool> CheckIfDuelCanBeStartedAsync(Player player, Player target)
    {
        if (player == target)
        {
            player.Logger.LogError("Player requested a duel with himself.");
            return false;
        }

        if (player.SelectedCharacter is not { } selectedCharacter)
        {
            player.Logger.LogError("Player requested a duel while not selecting a character.");
            return false;
        }

        if (target.SelectedCharacter is not { } targetCharacter)
        {
            // target logged out in the mean time.
            return false;
        }

        if (player.CurrentMap != target.CurrentMap)
        {
            await player.ShowMessageAsync("You can only duel players which are in the same map.").ConfigureAwait(false);
            return false;
        }

        if (player.CurrentMiniGame is not null)
        {
            await player.ShowMessageAsync("You cannot start a duel during a mini game.").ConfigureAwait(false);
            return false;
        }

        if (selectedCharacter.State >= HeroState.PlayerKiller2ndStage)
        {
            await player.ShowMessageAsync("You cannot start a duel while you are a player killer.").ConfigureAwait(false);
            return false;
        }

        if (targetCharacter.State >= HeroState.PlayerKiller2ndStage)
        {
            await player.ShowMessageAsync("You cannot start a duel with a player killer.").ConfigureAwait(false);
            return false;
        }

        if (player.GuildWarContext?.State is GuildWarState.Requested or GuildWarState.Started
            || target.GuildWarContext?.State is GuildWarState.Requested or GuildWarState.Started)
        {
            await player.ShowMessageAsync("You cannot start a duel during guild war.").ConfigureAwait(false);
            return false;
        }

        if (player.IsAnySelfDefenseActive()
            || target.IsAnySelfDefenseActive())
        {
            await player.ShowMessageAsync("You cannot start a duel with active self-defense.").ConfigureAwait(false);
            return false;
        }

        if (player.OpenedNpc is not null
            || target.OpenedNpc is not null)
        {
            await player.ShowMessageAsync("You cannot start a duel when a NPC dialog is opened.").ConfigureAwait(false);
            return false;
        }

        if (player.PlayerState.CurrentState != PlayerState.EnteredWorld
            || target.PlayerState.CurrentState != PlayerState.EnteredWorld)
        {
            await player.ShowMessageAsync("You cannot start a duel when one of the players has the wrong state.").ConfigureAwait(false);
            return false;
        }

        var duelConfig = player.GameContext.Configuration.DuelConfiguration;
        if (player.Level < duelConfig?.MinimumCharacterLevel || target.Level < duelConfig?.MinimumCharacterLevel)
        {
            await player.InvokeViewPlugInAsync<IShowDuelRequestResultPlugIn>(p => p.ShowDuelRequestResultAsync(DuelStartResult.FailedByTooLowLevel, target)).ConfigureAwait(false);
            return false;
        }

        if (player.Money < duelConfig?.EntranceFee || target.Money < duelConfig?.EntranceFee)
        {
            await player.InvokeViewPlugInAsync<IShowDuelRequestResultPlugIn>(p => p.ShowDuelRequestResultAsync(DuelStartResult.FailedByNotEnoughMoney, target)).ConfigureAwait(false);
            return false;
        }

        return true;
    }
}