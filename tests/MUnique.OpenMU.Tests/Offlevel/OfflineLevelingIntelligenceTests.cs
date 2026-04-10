// <copyright file="OfflineLevelingIntelligenceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offlevel;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.OfflineLeveling;

/// <summary>
/// Tests for <see cref="OfflineLevelingIntelligence"/>.
/// </summary>
[TestFixture]
public class OfflineLevelingIntelligenceTests
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
    /// Tests that the intelligence can be created and started.
    /// </summary>
    [Test]
    public async ValueTask StartsWithoutExceptionAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);

        // Act
        var intelligence = new OfflineLevelingIntelligence(player);
        intelligence.Start();

        intelligence?.Dispose();
    }

    /// <summary>
    /// Tests that disposing the intelligence twice does not throw.
    /// </summary>
    [Test]
    public async ValueTask DisposeTwiceDoesNotThrowAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var intelligence = new OfflineLevelingIntelligence(player);
        intelligence.Start();

        // Act & Assert
        intelligence.Dispose();
        intelligence.Dispose();
    }

    private async ValueTask<OfflineLevelingPlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }

}
