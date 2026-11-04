// <copyright file="PlayerRemovalOnDisposeTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Tests that a player never stays in the game context's player list after its teardown.
/// <para>
/// A player which is counted but no longer playing occupies a slot of the server's maximum player
/// count forever, which turns real clients away once enough of them piled up - and keeps its whole
/// object graph alive. Both paths below used to leave such a player behind.
/// </para>
/// </summary>
[TestFixture]
public class PlayerRemovalOnDisposeTests
{
    /// <summary>
    /// Tests that a player which is disposed WITHOUT having been disconnected is removed from the
    /// game context. That is what happens when a login or a bot spawn fails after the player was
    /// already added to the game: the failure path disposes it, and no disconnect event is ever
    /// raised which could take it out of the player list.
    /// </summary>
    [Test]
    public async ValueTask DisposeWithoutDisconnectRemovesPlayerFromGameContextAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var gameContext = player.GameContext;
        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);
        Assert.That(gameContext.PlayerCount, Is.EqualTo(1));

        await player.DisposeAsync().ConfigureAwait(false);

        Assert.That(gameContext.PlayerCount, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that a player whose teardown throws is still removed from the game context. The state
    /// machine has already advanced by then, so a later <see cref="Player.DisconnectAsync"/> is a
    /// no-op - if the removal is skipped here, nothing can ever remove the player again.
    /// </summary>
    [Test]
    public async ValueTask FailingDisconnectStillRemovesPlayerFromGameContextAsync()
    {
        var template = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var gameContext = template.GameContext;
        var player = new ThrowingOfflinePlayer(gameContext) { Account = template.Account };
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(template.SelectedCharacter!).ConfigureAwait(false);

        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);
        Assert.That(gameContext.PlayerCount, Is.EqualTo(1));

        await player.DisconnectAsync().ConfigureAwait(false);

        Assert.That(gameContext.PlayerCount, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that a <see cref="Player.PlayerDisconnected"/> subscriber which throws does not keep the
    /// later subscribers from running. The subscribers are what remove the player from the game and,
    /// for a remote player, dispose it - and the handler list is dropped before the invocation, so a
    /// skipped subscriber could never be re-run.
    /// </summary>
    [Test]
    public async ValueTask FailingDisconnectSubscriberDoesNotSkipLaterSubscribersAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var gameContext = player.GameContext;
        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);

        var laterSubscriberRan = false;
        player.PlayerDisconnected += _ => throw new InvalidOperationException("First subscriber fails.");
        player.PlayerDisconnected += _ =>
        {
            laterSubscriberRan = true;
            return ValueTask.CompletedTask;
        };

        await player.DisconnectAsync().ConfigureAwait(false);

        Assert.That(laterSubscriberRan, Is.True);
        Assert.That(gameContext.PlayerCount, Is.EqualTo(0));
    }

    /// <summary>
    /// An offline player whose teardown fails, like a bot which lost the race on one of the engine's
    /// non-thread-safe lists during its logout.
    /// </summary>
    private sealed class ThrowingOfflinePlayer : OfflinePlayer
    {
        public ThrowingOfflinePlayer(IGameContext gameContext)
            : base(gameContext)
        {
        }

        protected override ValueTask InternalDisconnectAsync()
            => throw new InvalidOperationException("Teardown failed.");
    }
}
