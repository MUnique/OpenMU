// <copyright file="ZenConsumptionHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Tests for <see cref="ZenConsumptionHandler"/>.
/// </summary>
[TestFixture]
public class ZenConsumptionHandlerTests
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
    /// Tests that Zen is deducted when the pay interval has passed.
    /// </summary>
    [Test]
    public async ValueTask DeductsZenWhenIntervalPassedAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        player.TryAddMoney(1_000_000);
        player.Attributes![Stats.Level] = 100;

        var handler = new ZenConsumptionHandler(player);
        player.StartTimestamp = DateTime.UtcNow.AddMinutes(-2);

        // Act
        await handler.DeductZenAsync().ConfigureAwait(false);

        // Assert
        Assert.That(player.Money, Is.LessThan(1_000_000));
    }

    /// <summary>
    /// Tests that Zen is not deducted when the pay interval has not passed.
    /// </summary>
    [Test]
    public async ValueTask DoesNotDeductZenWhenIntervalNotPassedAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        player.TryAddMoney(1_000_000);

        var handler = new ZenConsumptionHandler(player);

        // Act
        await handler.DeductZenAsync().ConfigureAwait(false);

        // Assert
        Assert.That(player.Money, Is.EqualTo(1_000_000));
    }

    /// <summary>
    /// Tests that offline player stops when the player has insufficient Zen.
    /// </summary>
    [Test]
    public async ValueTask StopsOfflineLevelingWhenInsufficientZenAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        player.TryAddMoney(100);
        player.Attributes![Stats.Level] = 100;

        var handler = new ZenConsumptionHandler(player);
        player.StartTimestamp = DateTime.UtcNow.AddMinutes(-2);

        // Act
        await handler.DeductZenAsync().ConfigureAwait(false);

        // Assert
        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.Finished));
    }

    private async ValueTask<OfflinePlayer> CreateOfflinePlayerAsync()
    {
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
        player.Attributes![Stats.MasterLevel] = 0;
        return player;
    }
}
