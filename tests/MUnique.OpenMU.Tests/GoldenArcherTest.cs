// <copyright file="GoldenArcherTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading.Tasks;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Unit tests for the <see cref="GoldenArcherRegistrationAction"/>.
/// </summary>
[TestFixture]
public class GoldenArcherTest
{
    private GoldenArcherRegistrationAction _action = null!;

    /// <summary>
    /// Sets up the test fixture.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._action = new GoldenArcherRegistrationAction();
    }

    /// <summary>
    /// Tests that nothing happens when the player has no NPC opened.
    /// </summary>
    [Test]
    public async Task RegisterAsync_NoNpcOpened_DoesNothingAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.OpenedNpc = null;

        await this._action.RegisterAsync(player).ConfigureAwait(false);

        Assert.That(player.SelectedCharacter!.RegisteredRenas, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that a message is displayed when player opens Golden Archer but has no Renas in inventory.
    /// </summary>
    [Test]
    public async Task RegisterAsync_NoRenasInInventory_ShowsMessageAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var npcMock = new Mock<NonPlayerCharacter>(new MonsterSpawnArea(), new MonsterDefinition { Number = 236 }, player.CurrentMap!);
        player.OpenedNpc = npcMock.Object;

        await this._action.RegisterAsync(player).ConfigureAwait(false);

        Assert.That(player.SelectedCharacter!.RegisteredRenas, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that registering a Rena successfully increments the registered renas count.
    /// </summary>
    [Test]
    public async Task RegisterAsync_WithRena_IncreasesRegisteredRenasAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var npcMock = new Mock<NonPlayerCharacter>(new MonsterSpawnArea(), new MonsterDefinition { Number = 236 }, player.CurrentMap!);
        player.OpenedNpc = npcMock.Object;

        player.GameContext.Configuration.GoldenArcherConfiguration = new GoldenArcherConfiguration
        {
            RequiredRenas = 5,
            RewardZen = 1000000,
            ItemDropChance = 100.0,
        };

        var renaDefinition = new ItemDefinition
        {
            Group = 14,
            Number = 21,
            Width = 1,
            Height = 1,
        };

        var renaItem = new Item
        {
            Definition = renaDefinition,
            ItemSlot = 12,
        };

        await player.Inventory!.AddItemAsync(12, renaItem).ConfigureAwait(false);

        await this._action.RegisterAsync(player).ConfigureAwait(false);

        Assert.That(player.SelectedCharacter!.RegisteredRenas, Is.EqualTo(1));
        Assert.That(player.SelectedCharacter!.TotalRegisteredRenas, Is.EqualTo(1));
        Assert.That(player.Inventory.ItemStorage.Items, Is.Not.Contains(renaItem));
    }

    /// <summary>
    /// Tests that when the required rena threshold is reached, the registered renas counter resets and Zen is rewarded.
    /// </summary>
    [Test]
    public async Task RegisterAsync_ReachesThreshold_ResetsCounterAndRewardsZenAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var npcMock = new Mock<NonPlayerCharacter>(new MonsterSpawnArea(), new MonsterDefinition { Number = 236 }, player.CurrentMap!);
        player.OpenedNpc = npcMock.Object;

        player.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
        player.GameContext.Configuration.GoldenArcherConfiguration = new GoldenArcherConfiguration
        {
            RequiredRenas = 5,
            RewardZen = 2000000,
            ItemDropChance = 0.0, // Disable item drop for clean zen test
        };

        player.SelectedCharacter!.RegisteredRenas = 4;
        player.SelectedCharacter.TotalRegisteredRenas = 4;
        var initialMoney = player.Money;

        var renaDefinition = new ItemDefinition
        {
            Group = 14,
            Number = 21,
            Width = 1,
            Height = 1,
        };

        var renaItem = new Item
        {
            Definition = renaDefinition,
            ItemSlot = 15,
        };

        await player.Inventory!.AddItemAsync(15, renaItem).ConfigureAwait(false);

        await this._action.RegisterAsync(player).ConfigureAwait(false);

        Assert.That(player.SelectedCharacter.RegisteredRenas, Is.EqualTo(0));
        Assert.That(player.SelectedCharacter.TotalRegisteredRenas, Is.EqualTo(5));
        Assert.That(player.Money, Is.EqualTo(initialMoney + 2000000));
    }
}
