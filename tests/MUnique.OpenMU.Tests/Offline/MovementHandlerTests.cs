// <copyright file="MovementHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameServer.RemoteView.MuHelper;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests for <see cref="MovementHandler"/>.
/// </summary>
[TestFixture]
public class MovementHandlerTests
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
    /// Tests that <see cref="MovementHandler.RegroupAsync"/> does nothing when within range.
    /// </summary>
    [Test]
    public async ValueTask RegroupAsync_DoesNothingWhenWithinRangeAsync()
    {
        // Arrange
        var origin = new Point(100, 100);
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        player.Position = new Point(100, 101); // Within RegroupDistanceThreshold (1)

        var config = new MuHelperSettings
        {
            ReturnToOriginalPosition = true,
            HuntingRange = 5,
        };

        var handler = new MovementHandler(player, config, origin);

        // Act
        var result = await handler.RegroupAsync().ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(player.IsWalking, Is.False);
    }

    private async ValueTask<OfflinePlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }
}
