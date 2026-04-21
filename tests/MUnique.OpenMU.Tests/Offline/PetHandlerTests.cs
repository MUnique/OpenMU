// <copyright file="PetHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameServer.RemoteView.MuHelper;
using MUnique.OpenMU.GameLogic.Pet;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using Item = MUnique.OpenMU.Persistence.BasicModel.Item;
using ItemDefinition = MUnique.OpenMU.Persistence.BasicModel.ItemDefinition;
using ItemSlotType = MUnique.OpenMU.Persistence.BasicModel.ItemSlotType;

/// <summary>
/// Tests for <see cref="PetHandler"/>.
/// </summary>
[TestFixture]
public class PetHandlerTests
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
    /// Tests that <see cref="PetHandler.InitializeAsync"/> sets the correct Dark Raven behavior.
    /// </summary>
    [Test]
    [TestCase(1, PetBehaviour.AttackRandom)]
    [TestCase(2, PetBehaviour.AttackWithOwner)]
    public async ValueTask InitializeAsync_SetsDarkRavenBehaviorAsync(int mode, PetBehaviour expectedBehaviour)
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var petCommandManagerMock = new Mock<IPetCommandManager>();

        var config = new MuHelperSettings
        {
            UseDarkRaven = true,
            DarkRavenMode = (byte)mode,
        };

        var handler = new PetHandler(player, config, petCommandManagerMock.Object);

        // Act
        await handler.InitializeAsync().ConfigureAwait(false);

        // Assert
        petCommandManagerMock.Verify(m => m.SetBehaviourAsync(expectedBehaviour, null), Times.Once);
    }

    /// <summary>
    /// Tests that <see cref="PetHandler.CheckPetDurabilityAsync"/> sets pet behavior to Idle when durability is 0.
    /// </summary>
    [Test]
    public async ValueTask CheckPetDurabilityAsync_SetsIdleWhenDurabilityIsZeroAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var petCommandManagerMock = new Mock<IPetCommandManager>();

        var petItem = new Item
        {
            Definition = new ItemDefinition
            {
                ItemSlot = new ItemSlotType(),
                Width = 1,
                Height = 1,
            },
            Durability = 0,
        };
        await player.Inventory!.AddItemAsync(InventoryConstants.PetSlot, petItem).ConfigureAwait(false);

        var handler = new PetHandler(player, new MuHelperSettings(), petCommandManagerMock.Object);

        // Act
        await handler.CheckPetDurabilityAsync().ConfigureAwait(false);

        // Assert
        petCommandManagerMock.Verify(m => m.SetBehaviourAsync(PetBehaviour.Idle, null), Times.Once);
    }

    /// <summary>
    /// Tests that <see cref="PetHandler.StopAsync"/> sets pet behavior to Idle.
    /// </summary>
    [Test]
    public async ValueTask StopAsync_SetsIdleAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var petCommandManagerMock = new Mock<IPetCommandManager>();

        var handler = new PetHandler(player, new MuHelperSettings(), petCommandManagerMock.Object);

        // Act
        await handler.StopAsync().ConfigureAwait(false);

        // Assert
        petCommandManagerMock.Verify(m => m.SetBehaviourAsync(PetBehaviour.Idle, null), Times.Once);
    }

    private async ValueTask<OfflinePlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }
}
