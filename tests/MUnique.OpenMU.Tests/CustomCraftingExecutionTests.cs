// <copyright file="CustomCraftingExecutionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>Tests that a recipe configured like one from the custom crafting editor executes in game logic.</summary>
[TestFixture]
public sealed class CustomCraftingExecutionTests
{
    /// <summary>A configured recipe consumes its required item and creates the configured result item.</summary>
    [Test]
    public async ValueTask CustomRecipeCreatesConfiguredResultInTheCraftingStorageAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var ingredientDefinition = new Persistence.BasicModel.ItemDefinition { Id = Guid.NewGuid(), Width = 1, Height = 1, Durability = 1 };
        var resultDefinition = new Persistence.BasicModel.ItemDefinition { Id = Guid.NewGuid(), Width = 1, Height = 1, Durability = 120 };
        var requirement = new Persistence.BasicModel.ItemCraftingRequiredItem
        {
            MinimumAmount = 1,
            MaximumAmount = 1,
            MinimumItemLevel = 0,
            MaximumItemLevel = 15,
        };
        requirement.PossibleItems.Add(ingredientDefinition);
        var result = new Persistence.BasicModel.ItemCraftingResultItem
        {
            ItemDefinition = resultDefinition,
            RandomMinimumLevel = 4,
            RandomMaximumLevel = 4,
            Durability = 80,
        };
        var settings = new Persistence.BasicModel.SimpleCraftingSettings
        {
            SuccessPercent = 100,
            MaximumSuccessPercent = 100,
        };
        settings.RequiredItems.Add(requirement);
        settings.ResultItems.Add(result);
        var crafting = new ItemCrafting
        {
            Number = 1,
            ItemCraftingHandlerClassName = typeof(SimpleItemCraftingHandler).FullName!,
            SimpleCraftingSettings = settings,
        };
        var npcDefinition = new Mock<MonsterDefinition>();
        npcDefinition.SetupAllProperties();
        npcDefinition.Setup(definition => definition.ItemCraftings).Returns(new List<ItemCrafting> { crafting });
        npcDefinition.Object.NpcWindow = NpcWindow.ChaosMachine;
        var npc = new NonPlayerCharacter(null!, npcDefinition.Object, null!);
        await new TalkNpcAction().TalkToNpcAsync(player, npc).ConfigureAwait(false);

        var ingredient = player.PersistenceContext.CreateNew<Item>();
        ingredient.Definition = ingredientDefinition;
        ingredient.Durability = 1;
        await player.TemporaryStorage!.AddItemAsync(ingredient).ConfigureAwait(false);

        var (mixResult, craftedItem) = await new SimpleItemCraftingHandler(settings).DoMixAsync(player, 0).ConfigureAwait(false);

        Assert.That(mixResult, Is.EqualTo(CraftingResult.Success));
        Assert.That(craftedItem, Is.Not.Null);
        Assert.That(craftedItem!.Definition, Is.SameAs(resultDefinition));
        Assert.That(craftedItem.Level, Is.EqualTo(4));
        Assert.That(craftedItem.Durability, Is.EqualTo(80));
        Assert.That(player.TemporaryStorage.Items, Does.Contain(craftedItem));
        Assert.That(player.TemporaryStorage.Items, Does.Not.Contain(ingredient));
    }

    /// <summary>Required item count and identity are enforced before a recipe can run.</summary>
    [TestCase(0, false, CraftingResult.LackingMixItems)]
    [TestCase(2, false, CraftingResult.TooManyItems)]
    [TestCase(1, true, CraftingResult.IncorrectMixItems)]
    public async ValueTask RecipeRejectsMissingExcessOrUnconfiguredItemsAsync(int itemCount, bool addExtraItem, CraftingResult expected)
    {
        var (player, handler, ingredientDefinition) = await PrepareRecipeAsync(minimumAmount: 1, maximumAmount: 1).ConfigureAwait(false);
        for (var index = 0; index < itemCount; index++)
        {
            await AddItemAsync(player, ingredientDefinition).ConfigureAwait(false);
        }

        if (addExtraItem)
        {
            await AddItemAsync(player, new Persistence.BasicModel.ItemDefinition { Id = Guid.NewGuid(), Width = 1, Height = 1, Durability = 1, Group = 14, Number = 13 }).ConfigureAwait(false);
        }

        var (result, item) = await handler.DoMixAsync(player, 0).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(expected));
        Assert.That(item, Is.Null);
    }

    /// <summary>A configured failed-mix policy keeps the ingredient when the mix chance is zero.</summary>
    [Test]
    public async ValueTask FailedRecipeAppliesConfiguredIngredientPolicyAsync()
    {
        var (player, handler, ingredientDefinition) = await PrepareRecipeAsync(successPercent: 0, failResult: MixResult.StaysAsIs).ConfigureAwait(false);
        var ingredient = await AddItemAsync(player, ingredientDefinition).ConfigureAwait(false);

        var (result, item) = await handler.DoMixAsync(player, 0).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(CraftingResult.Failed));
        Assert.That(item, Is.Null);
        Assert.That(player.TemporaryStorage!.Items, Does.Contain(ingredient));
    }

    /// <summary>Levels outside the configured requirement range do not qualify as required ingredients.</summary>
    [Test]
    public async ValueTask RecipeRejectsIngredientOutsideConfiguredLevelRangeAsync()
    {
        var (player, handler, ingredientDefinition) = await PrepareRecipeAsync(minimumLevel: 2, maximumLevel: 4).ConfigureAwait(false);
        await AddItemAsync(player, ingredientDefinition, level: 1).ConfigureAwait(false);

        var (result, item) = await handler.DoMixAsync(player, 0).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(CraftingResult.LackingMixItems));
        Assert.That(item, Is.Null);
    }

    private static async ValueTask<(Player Player, SimpleItemCraftingHandler Handler, ItemDefinition IngredientDefinition)> PrepareRecipeAsync(
        byte minimumAmount = 1,
        byte maximumAmount = 1,
        byte minimumLevel = 0,
        byte maximumLevel = 15,
        byte successPercent = 100,
        MixResult failResult = MixResult.Disappear)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var ingredientDefinition = new Persistence.BasicModel.ItemDefinition { Id = Guid.NewGuid(), Width = 1, Height = 1, Durability = 1, Group = 14, Number = 12 };
        var resultDefinition = new Persistence.BasicModel.ItemDefinition { Id = Guid.NewGuid(), Width = 1, Height = 1, Durability = 120 };
        var requirement = new Persistence.BasicModel.ItemCraftingRequiredItem
        {
            MinimumAmount = minimumAmount,
            MaximumAmount = maximumAmount,
            MinimumItemLevel = minimumLevel,
            MaximumItemLevel = maximumLevel,
            FailResult = failResult,
        };
        requirement.PossibleItems.Add(ingredientDefinition);
        var resultItem = new Persistence.BasicModel.ItemCraftingResultItem
        {
            ItemDefinition = resultDefinition,
            RandomMinimumLevel = 4,
            RandomMaximumLevel = 4,
            Durability = 80,
        };
        var settings = new Persistence.BasicModel.SimpleCraftingSettings
        {
            SuccessPercent = successPercent,
            MaximumSuccessPercent = successPercent,
        };
        settings.RequiredItems.Add(requirement);
        settings.ResultItems.Add(resultItem);
        var crafting = new ItemCrafting
        {
            Number = 1,
            ItemCraftingHandlerClassName = typeof(SimpleItemCraftingHandler).FullName!,
            SimpleCraftingSettings = settings,
        };
        var npcDefinition = new Mock<MonsterDefinition>();
        npcDefinition.SetupAllProperties();
        npcDefinition.Setup(definition => definition.ItemCraftings).Returns(new List<ItemCrafting> { crafting });
        npcDefinition.Object.NpcWindow = NpcWindow.ChaosMachine;
        await new TalkNpcAction().TalkToNpcAsync(player, new NonPlayerCharacter(null!, npcDefinition.Object, null!)).ConfigureAwait(false);
        return (player, new SimpleItemCraftingHandler(settings), ingredientDefinition);
    }

    private static async ValueTask<Item> AddItemAsync(Player player, ItemDefinition definition, byte level = 0)
    {
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = definition;
        item.Level = level;
        item.Durability = definition.Durability;
        await player.TemporaryStorage!.AddItemAsync(item).ConfigureAwait(false);
        return item;
    }
}
