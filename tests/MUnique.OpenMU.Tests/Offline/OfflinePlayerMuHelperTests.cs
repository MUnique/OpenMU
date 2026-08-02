// <copyright file="OfflinePlayerMuHelperTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Tests for <see cref="OfflinePlayerMuHelper"/>.
/// </summary>
[TestFixture]
public class OfflinePlayerMuHelperTests
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
        var intelligence = new OfflinePlayerMuHelper(player);
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
        var intelligence = new OfflinePlayerMuHelper(player);
        intelligence.Start();

        // Act & Assert
        intelligence.Dispose();
        intelligence.Dispose();
    }

    private async ValueTask<OfflinePlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }

}
