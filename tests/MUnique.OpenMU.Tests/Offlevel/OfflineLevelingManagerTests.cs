// <copyright file="OfflineLevelingManagerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offlevel;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.OfflineLeveling;

/// <summary>
/// Tests for <see cref="OfflineLevelingManager"/>.
/// </summary>
[TestFixture]
public class OfflineLevelingManagerTests
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
    /// Tests that <see cref="OfflineLevelingManager.StartAsync"/> returns true on success.
    /// </summary>
    [Test]
    public async ValueTask StartAsync_ReturnsTrueOnSuccessAsync()
    {
        // Arrange
        var manager = new OfflineLevelingManager();
        var realPlayer = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        realPlayer.Account!.LoginName = "testuser";
        realPlayer.TryAddMoney(1_000_000);
        realPlayer.Attributes![Stats.Level] = 100;

        // Act
        var result = await manager.StartAsync(realPlayer, "testuser").ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(manager.IsActive("testuser"), Is.True);
    }

    /// <summary>
    /// Tests that <see cref="OfflineLevelingManager.StartAsync"/> fails if the player has insufficient Zen.
    /// </summary>
    [Test]
    public async ValueTask StartAsync_FailsOnInsufficientZenAsync()
    {
        // Arrange
        var manager = new OfflineLevelingManager();
        var realPlayer = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        realPlayer.Account!.LoginName = "testuser";
        realPlayer.Money = 0; // No money
        realPlayer.Attributes![Stats.Level] = 100;

        // Act
        var result = await manager.StartAsync(realPlayer, "testuser").ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(manager.IsActive("testuser"), Is.False);
    }

    /// <summary>
    /// Tests that <see cref="OfflineLevelingManager.StopAsync"/> successfully stops a session.
    /// </summary>
    [Test]
    public async ValueTask StopAsync_StopsActiveSessionAsync()
    {
        // Arrange
        var manager = new OfflineLevelingManager();
        var realPlayer = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        realPlayer.Account!.LoginName = "testuser";
        realPlayer.TryAddMoney(1_000_000);
        realPlayer.Attributes![Stats.Level] = 100;

        await manager.StartAsync(realPlayer, "testuser").ConfigureAwait(false);
        Assert.That(manager.IsActive("testuser"), Is.True);

        // Act
        await manager.StopAsync("testuser").ConfigureAwait(false);

        // Assert
        Assert.That(manager.IsActive("testuser"), Is.False);
    }
}
