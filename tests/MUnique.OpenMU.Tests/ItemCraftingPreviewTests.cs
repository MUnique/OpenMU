// <copyright file="ItemCraftingPreviewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Tests for the preview of the success rate and price of a crafting, which is sent to the client
/// when the items of the crafting dialog change.
/// </summary>
[TestFixture]
public class ItemCraftingPreviewTests
{
    private const byte CraftingNumber = 42;

    private const byte SuccessRate = 40;

    private const int Price = 1000;

    /// <summary>
    /// Tests that moving the required items into the chaos machine shows the matching crafting
    /// with its success rate and price.
    /// </summary>
    [Test]
    public async ValueTask MovingTheRequiredItemIntoTheMachineShowsThePreviewAsync()
    {
        var (player, crafting, ingredient) = await PrepareAsync().ConfigureAwait(false);

        await new MoveItemAction().MoveItemAsync(player, ingredient.ItemSlot, Storages.Inventory, 0, Storages.ChaosMachine).ConfigureAwait(false);

        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowItemCraftingPreviewPlugIn>()!)
            .Verify(p => p.ShowCraftingPreviewAsync(crafting, SuccessRate, Price), Times.Once);
    }

    /// <summary>
    /// Tests that moving the required item out of the chaos machine again shows that no crafting matches.
    /// </summary>
    [Test]
    public async ValueTask MovingTheRequiredItemOutOfTheMachineShowsNoCraftingAsync()
    {
        var (player, _, ingredient) = await PrepareAsync().ConfigureAwait(false);
        var moveAction = new MoveItemAction();
        var inventorySlot = ingredient.ItemSlot;
        await moveAction.MoveItemAsync(player, inventorySlot, Storages.Inventory, 0, Storages.ChaosMachine).ConfigureAwait(false);

        await moveAction.MoveItemAsync(player, 0, Storages.ChaosMachine, inventorySlot, Storages.Inventory).ConfigureAwait(false);

        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowItemCraftingPreviewPlugIn>()!)
            .Verify(p => p.ShowCraftingPreviewAsync(null, 0, 0), Times.Once);
    }

    /// <summary>
    /// Tests that the crafting bonus of the player is included in the success rate of the preview.
    /// </summary>
    [Test]
    public async ValueTask PreviewIncludesTheCraftingBonusAsync()
    {
        var (player, crafting, ingredient) = await PrepareAsync().ConfigureAwait(false);
        player.Attributes!.AddElement(new SimpleElement(5, AggregateType.AddRaw), Stats.CraftingBonusPercentage);

        await new MoveItemAction().MoveItemAsync(player, ingredient.ItemSlot, Storages.Inventory, 0, Storages.ChaosMachine).ConfigureAwait(false);

        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowItemCraftingPreviewPlugIn>()!)
            .Verify(p => p.ShowCraftingPreviewAsync(crafting, (byte)(SuccessRate + 5), Price), Times.Once);
    }

    private static NonPlayerCharacter CreateCraftingNpc(ItemCrafting crafting)
    {
        var definition = new Mock<MonsterDefinition>();
        definition.SetupAllProperties();
        definition.Setup(d => d.ItemCraftings).Returns(new List<ItemCrafting> { crafting });
        definition.Object.NpcWindow = NpcWindow.ChaosMachine;
        return new NonPlayerCharacter(null!, definition.Object, null!);
    }

    private static Item CreateItem()
    {
        var item = new Mock<Item>();
        item.SetupAllProperties();
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        item.Object.Definition = new ItemDefinition { Width = 1, Height = 1, Durability = 1 };
        item.Object.Durability = 1;
        return item.Object;
    }

    private static async ValueTask<(Player Player, ItemCrafting Crafting, Item Ingredient)> PrepareAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var ingredient = CreateItem();
        Assert.That(await player.Inventory!.AddItemAsync(ingredient).ConfigureAwait(false), Is.True);
        var crafting = new ItemCrafting
        {
            Number = CraftingNumber,
            ItemCraftingHandlerClassName = typeof(OneItemCraftingHandler).AssemblyQualifiedName!,
        };

        await new TalkNpcAction().TalkToNpcAsync(player, CreateCraftingNpc(crafting)).ConfigureAwait(false);
        return (player, crafting, ingredient);
    }

    /// <summary>
    /// A crafting which requires any item in the crafting dialog.
    /// </summary>
    private sealed class OneItemCraftingHandler : BaseItemCraftingHandler
    {
        /// <inheritdoc />
        public override CraftingResult? TryGetRequiredItems(Player player, out IList<CraftingRequiredItemLink> items, out byte successRateByItems)
        {
            var itemsInDialog = player.TemporaryStorage?.Items.ToList() ?? [];
            items = itemsInDialog.Count > 0
                ? [new CraftingRequiredItemLink(itemsInDialog, new ItemCraftingRequiredItem())]
                : [];
            successRateByItems = SuccessRate;
            return items.Count > 0 ? null : CraftingResult.IncorrectMixItems;
        }

        /// <inheritdoc />
        protected override int GetPrice(byte successRate, IList<CraftingRequiredItemLink> requiredItems)
        {
            // The price must be based on the success rate of the items, not on the one including the crafting bonus.
            return successRate == SuccessRate ? Price * requiredItems.Sum(link => link.Items.Count()) : 0;
        }

        /// <inheritdoc />
        protected override ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot, byte successRate)
        {
            Assert.Fail($"A preview must not craft, but {player.SelectedCharacter?.Name} crafted {requiredItems.Count} items into socket slot {socketSlot} with a success rate of {successRate} %.");
            return ValueTask.FromResult(new List<Item>());
        }
    }
}
