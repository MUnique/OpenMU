// <copyright file="OfflineLevelingPlayerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offlevel;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.OfflineLeveling;

/// <summary>
/// Tests for <see cref="OfflineLevelingPlayer"/>.
/// </summary>
[TestFixture]
public class OfflineLevelingPlayerTests
{
    private IGameContext _gameContext = null!;

    /// <summary>
    /// Sets up a fresh game context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
    }

    /// <summary>
    /// Tests that the offline leveling player is created and started successfully.
    /// </summary>
    [Test]
    public async ValueTask InitializesSuccessfullyAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);

        // Assert
        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        Assert.That(player.SelectedCharacter, Is.Not.Null);
    }

    /// <summary>
    /// Tests that <see cref="OfflineLevelingPlayer.StopAsync"/> cleans up resources.
    /// </summary>
    [Test]
    public async ValueTask StopAsync_CleansUpAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);

        // Act
        await player.StopAsync().ConfigureAwait(false);

        // Assert
        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.Finished));
    }

    private async ValueTask<OfflineLevelingPlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }
}
