// <copyright file="OfflinePlayerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Tests for <see cref="OfflinePlayer"/>.
/// </summary>
[TestFixture]
public class OfflinePlayerTests
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
    /// Tests that the offline player is created and started successfully.
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
    /// Tests that <see cref="OfflinePlayer.StopAsync"/> cleans up resources.
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

    private async ValueTask<OfflinePlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }
}
