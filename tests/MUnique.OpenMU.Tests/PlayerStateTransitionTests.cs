// <copyright file="PlayerStateTransitionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests for the state transitions of a <see cref="Player"/>.
/// </summary>
[TestFixture]
public class PlayerStateTransitionTests
{
    /// <summary>
    /// Tests that a warp puts the player into the <see cref="PlayerState.ChangingMap"/> state,
    /// so that it's not considered to be in the world while the client loads the map.
    /// </summary>
    [Test]
    public async ValueTask WarpAdvancesToChangingMapAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));

        await player.WarpToAsync(CreateGate(player)).ConfigureAwait(false);

        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
    }

    /// <summary>
    /// Tests that the player is in the world again after the client signaled that it's ready.
    /// </summary>
    [Test]
    public async ValueTask ClientReadyAfterMapChangeReturnsToEnteredWorldAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await player.WarpToAsync(CreateGate(player)).ConfigureAwait(false);

        await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
    }

    /// <summary>
    /// Tests that a map change is observable by the <see cref="IPlayerStateChangedPlugIn"/>,
    /// in both directions.
    /// </summary>
    [Test]
    public async ValueTask MapChangeIsReportedToStateChangedPlugInAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = new StateChangeRecordingPlugIn();
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerStateChangedPlugIn>(plugIn);

        await player.WarpToAsync(CreateGate(player)).ConfigureAwait(false);
        await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);

        Assert.That(plugIn.Changes, Does.Contain((PlayerState.EnteredWorld, PlayerState.ChangingMap)));
        Assert.That(plugIn.Changes, Does.Contain((PlayerState.ChangingMap, PlayerState.EnteredWorld)));
    }

    /// <summary>
    /// Tests that a map change can be cancelled by the <see cref="IPlayerStateChangingPlugIn"/>.
    /// </summary>
    [Test]
    public async ValueTask MapChangeCanBeCancelledByPlugInAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerStateChangingPlugIn>(new MapChangeCancellingPlugIn());

        await player.WarpToAsync(CreateGate(player)).ConfigureAwait(false);

        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
    }

    /// <summary>
    /// Tests that the logout back to the character selection leaves the entered world state.
    /// Previously, the transition wasn't defined and failed silently, so the player stayed
    /// in the <see cref="PlayerState.EnteredWorld"/> state without a selected character.
    /// </summary>
    [Test]
    public async ValueTask LogoutBackToCharacterSelectionAdvancesToAuthenticatedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);

        await new LogoutAction().LogoutAsync(player, LogoutType.BackToCharacterSelection).ConfigureAwait(false);

        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.Authenticated));
    }

    /// <summary>
    /// Tests that the logout works as well when the player is in another in-game state,
    /// for example with an opened NPC dialog.
    /// </summary>
    [Test]
    public async ValueTask LogoutFromNpcDialogAdvancesToAuthenticatedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.NpcDialogOpened).ConfigureAwait(false);

        await new LogoutAction().LogoutAsync(player, LogoutType.BackToCharacterSelection).ConfigureAwait(false);

        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.Authenticated));
    }

    private static ExitGate CreateGate(Player player)
    {
        return new ExitGate
        {
            Map = player.CurrentMap!.Definition,
            X1 = 100,
            X2 = 100,
            Y1 = 100,
            Y2 = 100,
            Direction = Direction.West,
        };
    }

    [Guid("2E1B1E62-3A0A-4E52-9E6E-2F7EF0F98B2E")]
    private sealed class StateChangeRecordingPlugIn : IPlayerStateChangedPlugIn
    {
        public List<(State Previous, State Current)> Changes { get; } = new();

        public ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
        {
            this.Changes.Add((previousState, currentState));
            return ValueTask.CompletedTask;
        }
    }

    [Guid("7C9E0D8C-6C6C-4E58-9E1B-4B0C2B2E0E4A")]
    private sealed class MapChangeCancellingPlugIn : IPlayerStateChangingPlugIn
    {
        public ValueTask PlayerStateChangingAsync(Player player, StateMachine.StateChangeEventArgs eventArgs)
        {
            if (eventArgs.NextState == PlayerState.ChangingMap)
            {
                eventArgs.Cancel = true;
            }

            return ValueTask.CompletedTask;
        }
    }
}
