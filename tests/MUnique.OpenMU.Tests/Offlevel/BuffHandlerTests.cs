// <copyright file="BuffHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offlevel;

using NUnit.Framework;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.OfflineLeveling;
using MUnique.OpenMU.GameServer.RemoteView.MuHelper;

/// <summary>
/// Tests for <see cref="BuffHandler"/>.
/// </summary>
[TestFixture]
public class BuffHandlerTests
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
    /// Tests that <see cref="BuffHandler.PerformBuffsAsync"/> returns true immediately
    /// when no buff skills are configured.
    /// </summary>
    [Test]
    public async ValueTask ReturnsTrueWhenNoBuffsConfiguredAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var config = new MuHelperSettings
        {
            BuffSkill0Id = 0,
            BuffSkill1Id = 0,
            BuffSkill2Id = 0,
        };

        var handler = new BuffHandler(player, config);

        // Act
        var result = await handler.PerformBuffsAsync().ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that <see cref="BuffHandler.PerformBuffsAsync"/> returns true when config is null.
    /// </summary>
    [Test]
    public async ValueTask ReturnsTrueWhenConfigNullAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var handler = new BuffHandler(player, null);

        // Act
        var result = await handler.PerformBuffsAsync().ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.True);
    }

    private async ValueTask<OfflineLevelingPlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }

}
