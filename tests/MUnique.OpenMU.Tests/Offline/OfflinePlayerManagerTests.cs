// <copyright file="OfflinePlayerManagerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Tests for <see cref="OfflinePlayerManager"/>.
/// </summary>
[TestFixture]
public class OfflinePlayerManagerTests
{
    private const string TestUserLoginName = "test";

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
    /// Tests that <see cref="OfflinePlayerManager.StartAsync"/> returns true on success.
    /// </summary>
    [Test]
    public async ValueTask StartAsync_WithValidPlayer_ReturnsTrueAsync()
    {
        // Arrange
        var manager = new OfflinePlayerManager();
        var realPlayer = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        realPlayer.Account!.LoginName = TestUserLoginName;
        realPlayer.TryAddMoney(1_000_000);
        realPlayer.Attributes![Stats.Level] = 100;

        // Act
        var result = await manager.StartAsync(realPlayer, TestUserLoginName).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(manager.IsActive(TestUserLoginName), Is.True);
    }

    /// <summary>
    /// Tests that <see cref="OfflinePlayerManager.StartAsync"/> fails if the player has insufficient Zen.
    /// </summary>
    [Test]
    public async ValueTask StartAsync_WithInsufficientZen_ReturnsFalseAsync()
    {
        // Arrange
        var manager = new OfflinePlayerManager();
        var realPlayer = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        realPlayer.Account!.LoginName = TestUserLoginName;
        realPlayer.Money = 0; // No money
        realPlayer.Attributes![Stats.Level] = 100;

        // Act
        var result = await manager.StartAsync(realPlayer, TestUserLoginName).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(manager.IsActive(TestUserLoginName), Is.False);
    }

    /// <summary>
    /// Tests that <see cref="OfflinePlayerManager.StopAsync"/> successfully stops a session.
    /// </summary>
    [Test]
    public async ValueTask StopAsync_WhenSessionActive_StopsSuccessfullyAsync()
    {
        // Arrange
        var manager = new OfflinePlayerManager();
        var realPlayer = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        realPlayer.Account!.LoginName = TestUserLoginName;
        realPlayer.TryAddMoney(1_000_000);
        realPlayer.Attributes![Stats.Level] = 100;

        await manager.StartAsync(realPlayer, TestUserLoginName).ConfigureAwait(false);
        Assert.That(manager.IsActive(TestUserLoginName), Is.True);

        // Act
        await manager.StopAsync(TestUserLoginName).ConfigureAwait(false);

        // Assert
        Assert.That(manager.IsActive(TestUserLoginName), Is.False);
    }
}
