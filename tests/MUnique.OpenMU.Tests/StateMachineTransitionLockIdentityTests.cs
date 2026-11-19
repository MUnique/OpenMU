// <copyright file="StateMachineTransitionLockIdentityTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.Views.Login;

/// <summary>The existing Nito exclusion identity is lazy/thread-safe and independent of selected-character/map identity.</summary>
[TestFixture]
public class StateMachineTransitionLockIdentityTests
{
    /// <summary>Concurrent first reads agree without allocating another state or changing it.</summary>
    [Test]
    public async Task ConcurrentFirstReadsAreStableAsync()
    {
        var state = new StateMachine(PlayerState.EnteredWorld);
        var ids = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => Task.Run(() => state.TransitionLockId))).ConfigureAwait(false);
        var other = new StateMachine(PlayerState.EnteredWorld);
        Assert.Multiple(() =>
        {
            Assert.That(ids.Distinct().Count(), Is.EqualTo(1));
            Assert.That(ids[0], Is.Not.Zero);
            Assert.That(state.TransitionLockId, Is.EqualTo(ids[0]));
            Assert.That(other.TransitionLockId, Is.Not.EqualTo(ids[0]));
            Assert.That(state.CurrentState, Is.SameAs(PlayerState.EnteredWorld));
        });
    }

    /// <summary>Actual logout/map-id release/reselection retain the same exclusion identity.</summary>
    [Test]
    public async Task ReselectionKeepsExclusionIdentityAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var character = player.SelectedCharacter!;
        character.Name = "Lock152";
        Mock.Get(player.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
        var attributes = player.Attributes;
        var identity = player.PlayerState.TransitionLockId;
        var mapIdDuringRemoval = ushort.MaxValue;
        var identityDuringRemoval = 0;
        player.CurrentMap!.ObjectRemoved += removed =>
        {
            if (ReferenceEquals(removed.Object, player))
            {
                mapIdDuringRemoval = player.Id;
                identityDuringRemoval = player.PlayerState.TransitionLockId;
            }

            return ValueTask.CompletedTask;
        };
        try
        {
            await Task.Run(async () => await player.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(player, LogoutType.BackToCharacterSelection)).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(mapIdDuringRemoval, Is.Zero);
                Assert.That(identityDuringRemoval, Is.EqualTo(identity));
                Assert.That(player.PlayerState.TransitionLockId, Is.EqualTo(identity));
            });
            await Task.Run(async () => await player.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(player)).ConfigureAwait(false)).ConfigureAwait(false);
            await Task.Run(async () => await player.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(player, character.Name)).ConfigureAwait(false)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(player.Attributes, Is.Not.SameAs(attributes));
                Assert.That(player.PlayerState.TransitionLockId, Is.EqualTo(identity));
            });
        }
        finally
        {
            await player.DisposeAsync().ConfigureAwait(false);
        }
    }
}
