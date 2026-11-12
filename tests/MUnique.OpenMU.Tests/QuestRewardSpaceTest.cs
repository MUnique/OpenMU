// <copyright file="QuestRewardSpaceTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
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
    /// Tests that a quest is completed when its item reward fits into the place of the handed in item.
    /// </summary>
    [Test]
    public async ValueTask QuestIsCompletedWhenRewardFitsAfterHandInAsync()
    {
        var (player, quest, questState) = await this.CreatePlayerWithFullInventoryAsync(CreateItem(1, 1)).ConfigureAwait(false);

        await new QuestCompletionAction().CompleteQuestAsync(player, quest.Group, quest.Number).ConfigureAwait(false);

        Assert.That(questState.ActiveQuest, Is.Null);
        Assert.That(player.Inventory!.Items.Count(i => i.Definition == this._handInDefinition), Is.EqualTo(InventoryCells - 1));
        Mock.Get(player.ViewPlugIns.GetPlugIn<IQuestCompletionResponsePlugIn>()!).Verify(p => p.QuestCompletedAsync(quest), Times.Once);
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
            await player.Inventory!.AddItemAsync((byte)slot, item).ConfigureAwait(false);
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
        public StorageWithExtension(Storage main, Storage extension)
            : base(0, main.ItemStorage)
        {
            this.Extensions = [main, extension];
        }
    }
}
