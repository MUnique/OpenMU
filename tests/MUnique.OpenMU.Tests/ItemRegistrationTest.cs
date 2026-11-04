// <copyright file="ItemRegistrationTest.cs" company="MUnique">
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
using MUnique.OpenMU.GameLogic.PlayerActions.Items.ItemRegistration;
using MUnique.OpenMU.GameLogic.PlugIns.ItemRegistration;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Unit tests for the <see cref="ItemRegistrationAction"/>.
/// </summary>
[TestFixture]
public class ItemRegistrationTest
{
    private ItemRegistrationAction _action = null!;

    /// <summary>
    /// Sets up the test fixture.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._action = new ItemRegistrationAction();
    }

    /// <summary>
    /// Tests that nothing happens when the player has no NPC opened.
    /// </summary>
    [Test]
    public async Task RegisterAsync_NoNpcOpened_DoesNothingAsync()
    {
        var player = await CreatePlayerWithPlugInAsync().ConfigureAwait(false);
        player.OpenedNpc = null;

        await this._action.RegisterAsync(player).ConfigureAwait(false);

        var registeredStat = player.SelectedCharacter!.Attributes
            .FirstOrDefault(a => a.Definition == GameLogic.Attributes.Stats.RegisteredRenas);
        Assert.That(registeredStat?.Value ?? 0, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that a message is displayed when the player opens the NPC but has no matching items in inventory.
    /// </summary>
    [Test]
    public async Task RegisterAsync_NoItemsInInventory_ShowsMessageAsync()
    {
        var player = await CreatePlayerWithPlugInAsync().ConfigureAwait(false);
        var npcMock = new Mock<NonPlayerCharacter>(new MonsterSpawnArea(), new MonsterDefinition { Number = 236 }, player.CurrentMap!);
        player.OpenedNpc = npcMock.Object;

        await this._action.RegisterAsync(player).ConfigureAwait(false);

        var registeredStat = player.SelectedCharacter!.Attributes
            .FirstOrDefault(a => a.Definition == GameLogic.Attributes.Stats.RegisteredRenas);
        Assert.That(registeredStat?.Value ?? 0, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that registering an item successfully increments the registered items count.
    /// </summary>
    [Test]
    public async Task RegisterAsync_WithItem_IncreasesRegisteredCountAsync()
    {
        var player = await CreatePlayerWithPlugInAsync(requiredItemsCount: 5, rewardZen: 1000000).ConfigureAwait(false);
        var npcMock = new Mock<NonPlayerCharacter>(new MonsterSpawnArea(), new MonsterDefinition { Number = 236 }, player.CurrentMap!);
        player.OpenedNpc = npcMock.Object;

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

        var registeredStat = player.SelectedCharacter!.Attributes
            .FirstOrDefault(a => a.Definition == GameLogic.Attributes.Stats.RegisteredRenas);
        var totalStat = player.SelectedCharacter!.Attributes
            .FirstOrDefault(a => a.Definition == GameLogic.Attributes.Stats.TotalRegisteredRenas);

        Assert.That(registeredStat?.Value ?? 0, Is.EqualTo(1));
        Assert.That(totalStat?.Value ?? 0, Is.EqualTo(1));
        Assert.That(player.Inventory.ItemStorage.Items, Is.Not.Contains(renaItem));
    }

    /// <summary>
    /// Tests that when the required item threshold is reached, the counter resets and Zen is rewarded.
    /// </summary>
    [Test]
    public async Task RegisterAsync_ReachesThreshold_ResetsCounterAndRewardsZenAsync()
    {
        var player = await CreatePlayerWithPlugInAsync(requiredItemsCount: 5, rewardZen: 2000000).ConfigureAwait(false);
        var npcMock = new Mock<NonPlayerCharacter>(new MonsterSpawnArea(), new MonsterDefinition { Number = 236 }, player.CurrentMap!);
        player.OpenedNpc = npcMock.Object;

        player.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;

        // Pre-set the registered count to 4 (one away from threshold of 5)
        var registeredStat = new AttributeSystem.StatAttribute(GameLogic.Attributes.Stats.RegisteredRenas, 4);
        var totalStat = new AttributeSystem.StatAttribute(GameLogic.Attributes.Stats.TotalRegisteredRenas, 4);
        player.SelectedCharacter!.Attributes.Add(registeredStat);
        player.SelectedCharacter.Attributes.Add(totalStat);
        if (player.Attributes != null)
        {
            player.Attributes[GameLogic.Attributes.Stats.RegisteredRenas] = 4;
            player.Attributes[GameLogic.Attributes.Stats.TotalRegisteredRenas] = 4;
        }

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

        var finalRegistered = player.SelectedCharacter.Attributes
            .FirstOrDefault(a => a.Definition == GameLogic.Attributes.Stats.RegisteredRenas);
        var finalTotal = player.SelectedCharacter.Attributes
            .FirstOrDefault(a => a.Definition == GameLogic.Attributes.Stats.TotalRegisteredRenas);

        Assert.That(finalRegistered?.Value ?? 0, Is.EqualTo(0));
        Assert.That(finalTotal?.Value ?? 0, Is.EqualTo(5));
        Assert.That(player.Money, Is.EqualTo(initialMoney + 2000000));
    }

    /// <summary>
    /// Tests that when the Zen reward would exceed the maximum inventory money, the item is not
    /// consumed and the registered counter is not incremented (instead of silently losing both).
    /// </summary>
    [Test]
    public async Task RegisterAsync_MoneyLimitWouldBeExceeded_DoesNotConsumeItemOrLoseRewardAsync()
    {
        var player = await CreatePlayerWithPlugInAsync(rewardZen: 1000).ConfigureAwait(false);
        var npcMock = new Mock<NonPlayerCharacter>(new MonsterSpawnArea(), new MonsterDefinition { Number = 236 }, player.CurrentMap!);
        player.OpenedNpc = npcMock.Object;

        player.GameContext.Configuration.MaximumInventoryMoney = 1000;
        player.Money = 500;

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
            ItemSlot = 20,
        };

        await player.Inventory!.AddItemAsync(20, renaItem).ConfigureAwait(false);

        await this._action.RegisterAsync(player).ConfigureAwait(false);

        var registeredStat = player.SelectedCharacter!.Attributes
            .FirstOrDefault(a => a.Definition == GameLogic.Attributes.Stats.RegisteredRenas);

        Assert.That(registeredStat?.Value ?? 0, Is.EqualTo(0), "Counter should not be incremented when the reward can't fit.");
        Assert.That(player.Inventory.ItemStorage.Items, Does.Contain(renaItem), "Item should not be consumed when the reward can't fit.");
        Assert.That(player.Money, Is.EqualTo(500), "Money should stay unchanged.");
    }

    /// <summary>
    /// Tests that registration does nothing when the ItemRegistrationFeaturePlugIn is not active,
    /// instead of silently falling back to a default configuration.
    /// </summary>
    [Test]
    public async Task RegisterAsync_FeatureNotActive_DoesNothingAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.PlugInManager.RegisterPlugIn<IItemRegistrationStrategy, GoldenArcherRegistrationStrategy>();

        var npcMock = new Mock<NonPlayerCharacter>(new MonsterSpawnArea(), new MonsterDefinition { Number = 236 }, player.CurrentMap!);
        player.OpenedNpc = npcMock.Object;

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
            ItemSlot = 25,
        };

        await player.Inventory!.AddItemAsync(25, renaItem).ConfigureAwait(false);

        var success = await this._action.RegisterAsync(player).ConfigureAwait(false);

        Assert.That(success, Is.False, "Registration should not succeed when the feature plug-in is not active.");
        Assert.That(player.Inventory.ItemStorage.Items, Does.Contain(renaItem), "Item should not be consumed when the feature plug-in is not active.");
    }

    /// <summary>
    /// Tests that registration does nothing when the ItemRegistrationFeaturePlugIn is active but its
    /// Configuration is null, instead of silently falling back to a default (Golden Archer) configuration.
    /// </summary>
    [Test]
    public async Task RegisterAsync_FeatureConfigurationIsNull_DoesNothingAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.FeaturePlugIns.AddPlugIn(
            new ItemRegistrationFeaturePlugIn { Configuration = null }, true);
        player.GameContext.PlugInManager.RegisterPlugIn<IItemRegistrationStrategy, GoldenArcherRegistrationStrategy>();

        var npcMock = new Mock<NonPlayerCharacter>(new MonsterSpawnArea(), new MonsterDefinition { Number = 236 }, player.CurrentMap!);
        player.OpenedNpc = npcMock.Object;

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
            ItemSlot = 25,
        };

        await player.Inventory!.AddItemAsync(25, renaItem).ConfigureAwait(false);

        var success = await this._action.RegisterAsync(player).ConfigureAwait(false);

        Assert.That(success, Is.False, "Registration should not succeed when the feature's configuration is null.");
        Assert.That(player.Inventory.ItemStorage.Items, Does.Contain(renaItem), "Item should not be consumed when the feature's configuration is null.");
    }

    /// <summary>
    /// Creates a player with the ItemRegistrationFeaturePlugIn already registered.
    /// </summary>
    private static async Task<Player> CreatePlayerWithPlugInAsync(
        short npcNumber = 236,
        byte acceptedItemGroup = 14,
        short acceptedItemNumber = 21,
        int requiredItemsCount = 1,
        int rewardZen = 5000000,
        DropItemGroup? rewardDropItemGroup = null)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);

        var config = new ItemRegistrationConfiguration
        {
            Rules = new List<NpcItemRegistrationRule>
            {
                new NpcItemRegistrationRule
                {
                    NpcNumber = npcNumber,
                    AcceptedItemGroup = acceptedItemGroup,
                    AcceptedItemNumber = acceptedItemNumber,
                    RequiredItemsCount = requiredItemsCount,
                    RewardZen = rewardZen,
                    RewardDropItemGroup = rewardDropItemGroup,
                },
            },
        };

        player.GameContext.FeaturePlugIns.AddPlugIn(
            new ItemRegistrationFeaturePlugIn { Configuration = config }, true);

        player.GameContext.PlugInManager.RegisterPlugIn<IItemRegistrationStrategy, GoldenArcherRegistrationStrategy>();

        return player;
    }
}
