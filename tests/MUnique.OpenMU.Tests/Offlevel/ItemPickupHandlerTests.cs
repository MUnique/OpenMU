// <copyright file="ItemPickupHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offlevel;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.OfflineLeveling;
using MUnique.OpenMU.GameServer.RemoteView.MuHelper;

/// <summary>
/// Tests for <see cref="ItemPickupHandler"/>.
/// </summary>
[TestFixture]
public class ItemPickupHandlerTests
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
    /// Tests that pickup does nothing when all pickup options are disabled.
    /// </summary>
    [Test]
    public async ValueTask DoesNothingWhenAllDisabledAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var config = new MuHelperSettings
        {
            PickAllItems = false,
            PickJewel = false,
            PickAncient = false,
            PickZen = false,
            PickExcellent = false,
        };

        var handler = new ItemPickupHandler(player, config);

        // Act
        await handler.PickupItemsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that pickup does nothing when config is null.
    /// </summary>
    [Test]
    public async ValueTask DoesNothingWhenConfigNullAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var handler = new ItemPickupHandler(player, null);

        // Act
        await handler.PickupItemsAsync().ConfigureAwait(false);
    }

    private async ValueTask<OfflineLevelingPlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }

}
