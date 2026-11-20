// <copyright file="QuestRewardSpaceTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using MUnique.OpenMU.GameLogic.Views.Quest;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests that a quest isn't completed while its rewards don't fit, and the check if items fit into a storage.
/// </summary>
[TestFixture]
public class QuestRewardSpaceTest
{
    private const byte FirstInventorySlot = 12;

    private const int InventoryCells = 64;

    private readonly ItemDefinition _handInDefinition = new () { Width = 1, Height = 1, Durability = 1 };

    /// <summary>
    /// Tests that items which fit are reported as fitting, and that the storage doesn't change.
    /// </summary>
    [Test]
    public void ItemsFitIntoEmptyStorage()
    {
        var storage = new Storage(16, 0, 0, CreateItemStorage());

        Assert.That(storage.CanAddItems([CreateItem(2, 2), CreateItem(2, 2), CreateItem(4, 2)]), Is.True);
        Assert.That(storage.FreeSlots.Count(), Is.EqualTo(16));
    }

    /// <summary>
    /// Tests that items don't fit when the second one finds no place after the first one took its place.
    /// </summary>
    [Test]
    public void ItemsDontFitTogether()
    {
        // 2 rows: one 8x2 item takes everything.
        var storage = new Storage(16, 0, 0, CreateItemStorage());

        Assert.That(storage.CanAddItems([CreateItem(8, 2)]), Is.True);
        Assert.That(storage.CanAddItems([CreateItem(8, 2), CreateItem(1, 1)]), Is.False);
    }

    /// <summary>
    /// Tests that the place of the removed items counts as free.
    /// </summary>
    [Test]
    public void RemovedItemsMakeRoom()
    {
        var items = Enumerable.Range(0, 16).Select(slot => CreateItem(1, 1, (byte)slot)).ToList();
        var storage = new Storage(16, 0, 0, CreateItemStorage([.. items]));

        Assert.That(storage.CanAddItems([CreateItem(1, 1)]), Is.False);
        Assert.That(storage.CanAddItems([CreateItem(1, 1)], [items[5]]), Is.True);
        Assert.That(storage.CanAddItems([CreateItem(2, 2)], [items[0], items[1], items[8]]), Is.False);
        Assert.That(storage.CanAddItems([CreateItem(2, 2)], [items[0], items[1], items[8], items[9]]), Is.True);
        Assert.That(storage.FreeSlots, Is.Empty);
    }

    /// <summary>
    /// Tests that removed items which another storage holds free no cells, also when their slot is in this storage.
    /// </summary>
    [Test]
    public void RemovedItemsOfOtherStoragesFreeNoCells()
    {
        var items = Enumerable.Range(0, 16).Select(slot => CreateItem(1, 1, (byte)slot)).ToList();
        var storage = new Storage(16, 0, 0, CreateItemStorage([.. items]));
        var itemOfOtherStorage = CreateItem(1, 1, 5);

        Assert.That(storage.CanAddItems([CreateItem(1, 1)], [itemOfOtherStorage]), Is.False);
    }

    /// <summary>
    /// Tests that the inventory extensions count, too.
    /// </summary>
    [Test]
    public void ExtensionsCount()
    {
        var items = Enumerable.Range(0, 16).Select(slot => CreateItem(1, 1, (byte)slot)).ToList();
        var storage = new StorageWithExtension(new Storage(16, 0, 0, CreateItemStorage([.. items])), new Storage(8, 0, 16, CreateItemStorage()));

        Assert.That(storage.CanAddItems([CreateItem(2, 1), CreateItem(2, 1), CreateItem(4, 1)]), Is.True);
        Assert.That(storage.CanAddItems([CreateItem(2, 1), CreateItem(2, 1), CreateItem(4, 1), CreateItem(1, 1)]), Is.False);
    }

    /// <summary>
    /// Tests that the place of removed items in an extension counts as free, when the main storage is full.
    /// </summary>
    [Test]
    public void RemovedItemsInExtensionsMakeRoom()
    {
        var items = Enumerable.Range(0, 16).Select(slot => CreateItem(1, 1, (byte)slot)).ToList();
        var extensionItems = Enumerable.Range(16, 8).Select(slot => CreateItem(1, 1, (byte)slot)).ToList();
        var storage = new StorageWithExtension(new Storage(16, 0, 0, CreateItemStorage([.. items])), new Storage(8, 0, 16, CreateItemStorage([.. extensionItems])));

        Assert.That(storage.CanAddItems([CreateItem(2, 1)]), Is.False);
        Assert.That(storage.CanAddItems([CreateItem(2, 1)], [extensionItems[2], extensionItems[3]]), Is.True);
        Assert.That(storage.CanAddItems([CreateItem(2, 1)], [extensionItems[2], extensionItems[4]]), Is.False);
    }

    /// <summary>
    /// Tests that an extension which isn't a <see cref="Storage"/>, e.g. of a plug-in, takes the items which don't fit
    /// into the full main storage when it says it can add them.
    /// </summary>
    /// <param name="extensionTakesThem">Whether the extension can add the items.</param>
    [TestCase(true)]
    [TestCase(false)]
    public void OtherExtensionsCount(bool extensionTakesThem)
    {
        var items = Enumerable.Range(0, 16).Select(slot => CreateItem(1, 1, (byte)slot)).ToList();
        var extension = new Mock<IStorage>();
        extension.Setup(e => e.CanAddItems(It.IsAny<IEnumerable<Item>>(), It.IsAny<ICollection<Item>?>())).Returns(extensionTakesThem);
        var storage = new StorageWithExtension(new Storage(16, 0, 0, CreateItemStorage([.. items])), extension.Object);

        Assert.That(storage.CanAddItems([CreateItem(1, 1)]), Is.EqualTo(extensionTakesThem));
    }

    /// <summary>
    /// Tests that a storage without its own implementation of <see cref="IStorage.CanAddItems"/>, e.g. of a plug-in,
    /// checks a single item with the default of <see cref="IStorage.FitsAfterRemoval"/>, i.e. whether it fits now.
    /// </summary>
    /// <param name="hasSpace">Whether the storage has space for the item.</param>
    [TestCase(true)]
    [TestCase(false)]
    public void CanAddItemsByDefaultChecksASingleItem(bool hasSpace)
    {
        var checkedDefinitions = new List<ItemDefinition?>();
        var storage = new Mock<IStorage> { CallBase = true };
        storage.Setup(s => s.CheckInvSpace(It.IsAny<Item>()))
            .Callback<Item>(item => checkedDefinitions.Add(item.Definition))
            .Returns(hasSpace ? (byte)0 : null);
        var item = CreateItem(2, 2);

        Assert.That(storage.Object.CanAddItems([item]), Is.EqualTo(hasSpace));
        Assert.That(checkedDefinitions, Is.EqualTo(new[] { item.Definition }));
        Assert.That(storage.Object.CanAddItems([]), Is.True);
    }

    /// <summary>
    /// Tests that an item without definition is refused, without an exception, by a <see cref="Storage"/> and by the
    /// default of <see cref="IStorage.CanAddItems"/>.
    /// </summary>
    [Test]
    public void ItemWithoutDefinitionIsRefused()
    {
        var itemWithoutDefinition = new BasicModel.Item();
        var defaultStorage = new Mock<IStorage> { CallBase = true };
        defaultStorage.Setup(s => s.CheckInvSpace(It.IsAny<Item>())).Returns(0);

        Assert.That(new Storage(16, CreateItemStorage()).CanAddItems([itemWithoutDefinition]), Is.False);
        Assert.That(defaultStorage.Object.CanAddItems([itemWithoutDefinition]), Is.False);
    }

    /// <summary>
    /// Tests that a storage without its own implementation of <see cref="IStorage.CanAddItems"/>, e.g. of a plug-in,
    /// refuses two items although each of them fits alone, because it isn't known whether they fit together.
    /// </summary>
    [Test]
    public void CanAddItemsByDefaultRefusesMoreThanOneItem()
    {
        var storage = new Mock<IStorage> { CallBase = true };
        storage.Setup(s => s.CheckInvSpace(It.IsAny<Item>())).Returns(0);

        Assert.That(storage.Object.CanAddItems([CreateItem(1, 1), CreateItem(1, 1)]), Is.False);
    }

    /// <summary>
    /// Tests that an extension which isn't a <see cref="Storage"/> and has no own implementation of
    /// <see cref="IStorage.CanAddItems"/>, e.g. of a plug-in, takes one item which doesn't fit into the full main
    /// storage, but not two of them, although it has a free place.
    /// </summary>
    [Test]
    public void OtherExtensionWithTheDefaultTakesOneItemOnly()
    {
        var items = Enumerable.Range(0, 16).Select(slot => CreateItem(1, 1, (byte)slot)).ToList();
        var extension = new Mock<IStorage> { CallBase = true };
        extension.Setup(e => e.CheckInvSpace(It.IsAny<Item>())).Returns(16);
        var storage = new StorageWithExtension(new Storage(16, 0, 0, CreateItemStorage([.. items])), extension.Object);

        Assert.That(storage.CanAddItems([CreateItem(1, 1)]), Is.True);
        Assert.That(storage.CanAddItems([CreateItem(1, 1), CreateItem(1, 1)]), Is.False);
    }

    /// <summary>
    /// Tests that a quest is completed when its item reward fits into the place of the handed in item, and that the
    /// reward is put there, not dropped.
    /// </summary>
    [Test]
    public async ValueTask QuestIsCompletedWhenRewardFitsAfterHandInAsync()
    {
        var rewardItem = CreateItem(1, 1);
        var (player, quest, questState) = await this.CreatePlayerWithFullInventoryAsync(rewardItem).ConfigureAwait(false);

        await new QuestCompletionAction().CompleteQuestAsync(player, quest.Group, quest.Number).ConfigureAwait(false);

        Assert.That(questState.ActiveQuest, Is.Null);
        Assert.That(player.Inventory!.GetItem(FirstInventorySlot)?.Definition, Is.SameAs(rewardItem.Definition));
        Assert.That(player.Inventory!.Items.Count(i => i.Definition == this._handInDefinition), Is.EqualTo(InventoryCells - 1));
        Mock.Get(player.ViewPlugIns.GetPlugIn<IQuestCompletionResponsePlugIn>()!).Verify(p => p.QuestCompletedAsync(quest), Times.Once);
    }

    /// <summary>
    /// Tests that two requirements of the same item, one of any level and one of level 3, hand in two different items:
    /// the item +3 for the requirement of its level, another one for the requirement of any level.
    /// </summary>
    [Test]
    public async ValueTask RequirementsOfTheSameItemHandInDifferentItemsAsync()
    {
        var (player, quest, questState) = await this.CreatePlayerWithFullInventoryAsync(CreateItem(1, 1)).ConfigureAwait(false);
        var itemOfLevel = player.Inventory!.GetItem(FirstInventorySlot)!;
        itemOfLevel.Level = 3;
        quest.RequiredItems.Add(new BasicModel.QuestItemRequirement
        {
            Item = this._handInDefinition,
            MinimumNumber = 1,
            DropItemGroup = new BasicModel.DropItemGroup { ItemLevel = 3 },
        });

        await new QuestCompletionAction().CompleteQuestAsync(player, quest.Group, quest.Number).ConfigureAwait(false);

        Assert.That(questState.ActiveQuest, Is.Null);
        Assert.That(player.Inventory.Items, Does.Not.Contain(itemOfLevel));
        Assert.That(player.Inventory.Items.Count(i => i.Definition == this._handInDefinition), Is.EqualTo(InventoryCells - 2));
    }

    /// <summary>
    /// Tests that a requirement without item isn't met, without an exception: the quest isn't completed and nothing
    /// is taken.
    /// </summary>
    [Test]
    public async ValueTask RequirementWithoutItemIsntMetAsync()
    {
        var (player, quest, questState) = await this.CreatePlayerWithFullInventoryAsync(CreateItem(1, 1)).ConfigureAwait(false);
        quest.RequiredItems.Add(new BasicModel.QuestItemRequirement { MinimumNumber = 1 });

        await new QuestCompletionAction().CompleteQuestAsync(player, quest.Group, quest.Number).ConfigureAwait(false);

        Assert.That(questState.ActiveQuest, Is.SameAs(quest));
        Assert.That(player.Inventory!.Items.Count(i => i.Definition == this._handInDefinition), Is.EqualTo(InventoryCells));
        Mock.Get(player.ViewPlugIns.GetPlugIn<IQuestCompletionResponsePlugIn>()!).Verify(p => p.QuestCompletedAsync(quest), Times.Never);
    }

    /// <summary>
    /// Tests that two requirements can't count the same item: with one item +3, two requirements of one item +3 each
    /// aren't met, and nothing is taken.
    /// </summary>
    [Test]
    public async ValueTask OneItemDoesntMeetTwoRequirementsAsync()
    {
        var (player, quest, questState) = await this.CreatePlayerWithFullInventoryAsync(CreateItem(1, 1)).ConfigureAwait(false);
        player.Inventory!.GetItem(FirstInventorySlot)!.Level = 3;
        quest.RequiredItems.Clear();
        for (var i = 0; i < 2; i++)
        {
            quest.RequiredItems.Add(new BasicModel.QuestItemRequirement
            {
                Item = this._handInDefinition,
                MinimumNumber = 1,
                DropItemGroup = new BasicModel.DropItemGroup { ItemLevel = 3 },
            });
        }

        await new QuestCompletionAction().CompleteQuestAsync(player, quest.Group, quest.Number).ConfigureAwait(false);

        Assert.That(questState.ActiveQuest, Is.SameAs(quest));
        Assert.That(player.Inventory.Items.Count(), Is.EqualTo(InventoryCells));
    }

    /// <summary>
    /// Tests that an equipped item isn't handed in: with the only required item in the hand, the quest isn't
    /// completed, and the item stays.
    /// </summary>
    [Test]
    public async ValueTask EquippedItemIsntHandedInAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(GameContextTestHelper.CreateGameContext()).ConfigureAwait(false);
        // Equipping applies the item's set groups, which only the basic model initializes.
        var weapon = new BasicModel.ItemDefinition { Width = 1, Height = 1, Durability = 1 };
        var equippedItem = new BasicModel.Item { Id = Guid.NewGuid(), Definition = weapon, Durability = 1 };
        await player.Inventory!.AddItemAsync(InventoryConstants.LeftHandSlot, equippedItem).ConfigureAwait(false);
        var quest = new BasicModel.QuestDefinition { Group = 5, Number = 7, StartingNumber = 6 };
        quest.RequiredItems.Add(new BasicModel.QuestItemRequirement { Item = weapon, MinimumNumber = 1 });
        var questState = new BasicModel.CharacterQuestState { Group = quest.Group, ActiveQuest = quest };
        Mock.Get(player.SelectedCharacter!).Setup(c => c.QuestStates).Returns(new List<CharacterQuestState> { questState });

        await new QuestCompletionAction().CompleteQuestAsync(player, quest.Group, quest.Number).ConfigureAwait(false);

        Assert.That(questState.ActiveQuest, Is.SameAs(quest));
        Assert.That(player.Inventory!.Items, Does.Contain(equippedItem));
    }

    /// <summary>
    /// Tests that a quest with an item reward without item isn't completed, and that nothing is taken: adding the
    /// reward would fail after the items were taken.
    /// </summary>
    [Test]
    public async ValueTask QuestWithItemRewardWithoutItemIsntCompletedAsync()
    {
        var (player, quest, questState) = await this.CreatePlayerWithFullInventoryAsync(CreateItem(1, 1)).ConfigureAwait(false);
        quest.Rewards.Single().ItemReward = null;

        await new QuestCompletionAction().CompleteQuestAsync(player, quest.Group, quest.Number).ConfigureAwait(false);

        Assert.That(questState.ActiveQuest, Is.SameAs(quest));
        Assert.That(questState.LastFinishedQuest, Is.Null);
        Assert.That(player.Inventory!.Items.Count(), Is.EqualTo(InventoryCells));
    }

    /// <summary>
    /// Tests that a quest isn't completed while its item reward doesn't fit, and that nothing is taken.
    /// </summary>
    [Test]
    public async ValueTask QuestIsRefusedWhenRewardDoesntFitAsync()
    {
        var (player, quest, questState) = await this.CreatePlayerWithFullInventoryAsync(CreateItem(2, 2)).ConfigureAwait(false);

        await new QuestCompletionAction().CompleteQuestAsync(player, quest.Group, quest.Number).ConfigureAwait(false);

        Assert.That(questState.ActiveQuest, Is.SameAs(quest));
        Assert.That(questState.LastFinishedQuest, Is.Null);
        Assert.That(player.Inventory!.Items.Count(), Is.EqualTo(InventoryCells));
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IQuestCompletionResponsePlugIn>()!);
        view.Verify(p => p.QuestCompletionRefusedAsync(quest, QuestCompletionRefusal.InventoryFull), Times.Once);
        view.Verify(p => p.QuestCompletedAsync(It.IsAny<QuestDefinition>()), Times.Never);
    }

    /// <summary>
    /// Tests that a quest isn't completed while its money reward would exceed the money limit.
    /// </summary>
    [Test]
    public async ValueTask QuestIsRefusedWhenMoneyExceedsLimitAsync()
    {
        var (player, quest, questState) = await this.CreatePlayerWithFullInventoryAsync(CreateItem(1, 1)).ConfigureAwait(false);
        var moneyReward = new BasicModel.QuestReward { RewardType = QuestRewardType.Money, Value = 1000 };
        quest.Rewards.Add(moneyReward);
        player.GameContext.Configuration.MaximumInventoryMoney = 5000;
        player.Money = 4500;

        await new QuestCompletionAction().CompleteQuestAsync(player, quest.Group, quest.Number).ConfigureAwait(false);

        Assert.That(questState.ActiveQuest, Is.SameAs(quest));
        Assert.That(questState.LastFinishedQuest, Is.Null);
        Assert.That(player.Money, Is.EqualTo(4500));
        Mock.Get(player.ViewPlugIns.GetPlugIn<IQuestCompletionResponsePlugIn>()!).Verify(p => p.QuestCompletionRefusedAsync(quest, QuestCompletionRefusal.ZenLimitReached), Times.Once);
    }

    private static Item CreateItem(byte width, byte height, byte slot = 0)
    {
        // The items are compared by their identifier.
        return new BasicModel.Item
        {
            Id = Guid.NewGuid(),
            Definition = new ItemDefinition { Width = width, Height = height, Durability = 1 },
            ItemSlot = slot,
            Durability = 1,
        };
    }

    private static ItemStorage CreateItemStorage(params Item[] items)
    {
        var storage = new Mock<ItemStorage>();
        storage.Setup(s => s.Items).Returns(items.ToList());
        return storage.Object;
    }

    /// <summary>
    /// Creates a player with a full inventory of items which are handed in, and an active quest which takes one of them
    /// and rewards the given item.
    /// </summary>
    private async ValueTask<(Player Player, QuestDefinition Quest, CharacterQuestState QuestState)> CreatePlayerWithFullInventoryAsync(Item rewardItem)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(GameContextTestHelper.CreateGameContext()).ConfigureAwait(false);
        for (var slot = FirstInventorySlot; slot < FirstInventorySlot + InventoryCells; slot++)
        {
            var item = new BasicModel.Item { Id = Guid.NewGuid(), Definition = this._handInDefinition, Durability = 1 };
            await player.Inventory!.AddItemAsync(slot, item).ConfigureAwait(false);
        }

        var quest = new BasicModel.QuestDefinition { Group = 5, Number = 7, StartingNumber = 6 };
        quest.RequiredItems.Add(new BasicModel.QuestItemRequirement { Item = this._handInDefinition, MinimumNumber = 1 });
        quest.Rewards.Add(new BasicModel.QuestReward { RewardType = QuestRewardType.Item, ItemReward = rewardItem });

        var questState = new BasicModel.CharacterQuestState { Group = quest.Group, ActiveQuest = quest };
        Mock.Get(player.SelectedCharacter!).Setup(c => c.QuestStates).Returns(new List<CharacterQuestState> { questState });
        return (player, quest, questState);
    }

    /// <summary>
    /// A storage without own slots, whose extensions are a main storage and an extension, like the inventory.
    /// </summary>
    private sealed class StorageWithExtension : Storage
    {
        public StorageWithExtension(Storage main, IStorage extension)
            : base(0, main.ItemStorage)
        {
            this.Extensions = [main, extension];
        }
    }
}
