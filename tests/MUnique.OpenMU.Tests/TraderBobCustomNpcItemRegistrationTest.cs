// <copyright file="TraderBobCustomNpcItemRegistrationTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Items.ItemRegistration;
using MUnique.OpenMU.GameLogic.PlugIns.ItemRegistration;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// A dummy view plugin just for testing so the strategy can call something.
/// </summary>
public interface ITraderBobResultPlugIn : IViewPlugIn
{
    ValueTask RegistrationResultAsync(bool success);
}

/// <summary>
/// A custom strategy to test the generic implementation.
/// </summary>
[Guid("98765432-1234-1234-1234-123456789012")]
public class TraderBobRegistrationStrategy : BaseItemRegistrationStrategy
{
    public override short NpcNumber => 500;

    // No specific stats needed for Trader Bob
    public override AttributeDefinition? TargetStat => null;
    public override AttributeDefinition? TargetTotalStat => null;

    public override ValueTask OpenDialogAsync(Player player) => ValueTask.CompletedTask;

    protected override async ValueTask OnRegistrationCompletedAsync(Player player)
    {
        await player.InvokeViewPlugInAsync<ITraderBobResultPlugIn>(p => p.RegistrationResultAsync(true)).ConfigureAwait(false);
    }

    protected override async ValueTask OnMissingItemAsync(Player player)
    {
        await player.InvokeViewPlugInAsync<ITraderBobResultPlugIn>(p => p.RegistrationResultAsync(false)).ConfigureAwait(false);
    }
}

/// <summary>
/// Tests the custom item registration logic.
/// </summary>
[TestFixture]
public class TraderBobCustomNpcItemRegistrationTest
{
    private ItemRegistrationAction _action = null!;

    [SetUp]
    public void Setup()
    {
        this._action = new ItemRegistrationAction();
    }

    [Test]
    public async Task RegisterItem_WithCustomNpc_Success()
    {
        // ARRANGE
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
        var initialMoney = player.Money;

        // Setup Item Registration Feature
        var config = new ItemRegistrationConfiguration
        {
            Rules = new List<NpcItemRegistrationRule>
            {
                new NpcItemRegistrationRule
                {
                    NpcNumber = 500, // Trader Bob
                    AcceptedItemGroup = 14,
                    AcceptedItemNumber = 30, // E.g. Apple
                    RequiredItemsCount = 1,
                    RewardZen = 1000,
                },
            },
        };

        player.GameContext.FeaturePlugIns.AddPlugIn(
            new ItemRegistrationFeaturePlugIn { Configuration = config }, true);

        // Register custom strategy
        player.GameContext.PlugInManager.RegisterPlugIn<IItemRegistrationStrategy, TraderBobRegistrationStrategy>();

        // Setup Mock NPC
        var npcMock = new Mock<NonPlayerCharacter>(new MonsterSpawnArea(), new MonsterDefinition { Number = 500 }, player.CurrentMap!);
        player.OpenedNpc = npcMock.Object;

        // Add Apple to inventory
        var appleDef = new ItemDefinition { Group = 14, Number = 30, Width = 1, Height = 1 };
        var apple = new Item { Definition = appleDef };
        await player.Inventory!.AddItemAsync(10, apple).ConfigureAwait(false);

        // ACT
        var success = await this._action.RegisterAsync(player).ConfigureAwait(false);

        // ASSERT
        Assert.That(success, Is.True, "Action should succeed.");
        Assert.That(player.Inventory.Items.Count(), Is.EqualTo(0), "Apple should be consumed.");
        Assert.That(player.Money, Is.EqualTo(initialMoney + 1000), "Player should receive 1000 Zen.");
    }
}
