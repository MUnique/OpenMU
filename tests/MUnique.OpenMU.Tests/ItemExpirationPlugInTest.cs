// <copyright file="ItemExpirationPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Interfaces;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests the <see cref="ItemExpirationPlugIn"/>.
/// </summary>
[TestFixture]
public class ItemExpirationPlugInTest
{
    private const float PetHealth = 50;

    /// <summary>
    /// Tests that an equipped item which expired loses its effect and stays equipped, and that the player
    /// is told and the client gets the item again, once.
    /// </summary>
    [Test]
    public async Task EquippedItemLosesEffectWhenExpiredAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var pet = await EquipPetAsync(player, DateTime.UtcNow.AddSeconds(1)).ConfigureAwait(false);
        var healthWithPet = player.Attributes![Stats.MaximumHealth];
        var plugIn = new ItemExpirationPlugIn { Configuration = new ItemExpirationConfiguration { CheckInterval = TimeSpan.Zero } };

        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(healthWithPet), "the pet is valid");
        Assert.That(player.AppearanceData.EquippedItems.Select(appearance => appearance.ItemSlot), Does.Contain(InventoryConstants.PetSlot));

        pet.ExpiresAt = DateTime.UtcNow.AddSeconds(-1);
        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);
        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);

        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(healthWithPet - PetHealth));
        Assert.That(player.Inventory!.GetItem(InventoryConstants.PetSlot), Is.SameAs(pet), "an expired item stays");
        Assert.That(player.AppearanceData.EquippedItems.Select(appearance => appearance.ItemSlot), Does.Not.Contain(InventoryConstants.PetSlot), "other players don't see an expired pet");
        Mock.Get(player.ViewPlugIns.GetPlugIn<IItemUpgradedPlugIn>()!).Verify(v => v.ItemUpgradedAsync(pet), Times.Once);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!).Verify(v => v.ShowMessageAsync(It.Is<string>(m => m.Contains("Rented pet")), MessageType.BlueNormal), Times.Once);
    }

    /// <summary>
    /// Tests that an item which expired while the player was offline has no effect from the start, and that the
    /// player is told at the first check.
    /// </summary>
    [Test]
    public async Task ItemWhichExpiredBeforeIsReportedAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var healthWithoutPet = player.Attributes![Stats.MaximumHealth];
        var pet = await EquipPetAsync(player, DateTime.UtcNow.AddDays(-1)).ConfigureAwait(false);
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(healthWithoutPet));

        await new ItemExpirationPlugIn().ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);

        Mock.Get(player.ViewPlugIns.GetPlugIn<IItemUpgradedPlugIn>()!).Verify(v => v.ItemUpgradedAsync(pet), Times.Once);
    }

    /// <summary>
    /// Tests that the plugin doesn't check again before the check interval elapsed.
    /// </summary>
    [Test]
    public async Task NoCheckBeforeIntervalAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var pet = await EquipPetAsync(player, DateTime.UtcNow.AddDays(1)).ConfigureAwait(false);
        var plugIn = new ItemExpirationPlugIn { Configuration = new ItemExpirationConfiguration { CheckInterval = TimeSpan.FromHours(1) } };
        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);

        pet.ExpiresAt = DateTime.UtcNow.AddSeconds(-1);
        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IItemUpgradedPlugIn>()!).Verify(v => v.ItemUpgradedAsync(pet), Times.Never);

        plugIn.ForceStart();
        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IItemUpgradedPlugIn>()!).Verify(v => v.ItemUpgradedAsync(pet), Times.Once);
    }

    /// <summary>
    /// Tests that each game context has its own check interval. One plugin instance serves all game servers of a
    /// process, so the check of one game server mustn't take the check of another.
    /// </summary>
    [Test]
    public async Task EachGameContextIsCheckedAsync()
    {
        var firstPlayer = await CreatePlayerAsync().ConfigureAwait(false);
        var secondPlayer = await CreatePlayerAsync().ConfigureAwait(false);
        Assert.That(secondPlayer.GameContext, Is.Not.SameAs(firstPlayer.GameContext));
        var secondPet = await EquipPetAsync(secondPlayer, DateTime.UtcNow.AddSeconds(-1)).ConfigureAwait(false);
        var plugIn = new ItemExpirationPlugIn { Configuration = new ItemExpirationConfiguration { CheckInterval = TimeSpan.FromHours(1) } };

        await plugIn.ExecuteTaskAsync((GameContext)firstPlayer.GameContext).ConfigureAwait(false);
        await plugIn.ExecuteTaskAsync((GameContext)secondPlayer.GameContext).ConfigureAwait(false);

        Mock.Get(secondPlayer.ViewPlugIns.GetPlugIn<IItemUpgradedPlugIn>()!).Verify(v => v.ItemUpgradedAsync(secondPet), Times.Once);
    }

    /// <summary>
    /// Tests that an expired item whose handling failed while its effects were refreshed is handled again at the
    /// next check, instead of being skipped for the rest of the session.
    /// </summary>
    [Test]
    public async Task FailedHandlingIsRetriedAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var healthWithoutPet = player.Attributes![Stats.MaximumHealth];
        var pet = await EquipPetAsync(player, DateTime.UtcNow.AddDays(1)).ConfigureAwait(false);
        var failRefresh = true;
        player.Inventory!.AsInventoryStorage!.EquippedItemsChanged += _ => failRefresh ? throw new InvalidOperationException("Transient failure") : ValueTask.CompletedTask;
        var plugIn = new ItemExpirationPlugIn { Configuration = new ItemExpirationConfiguration { CheckInterval = TimeSpan.Zero } };
        var itemUpgradedPlugIn = Mock.Get(player.ViewPlugIns.GetPlugIn<IItemUpgradedPlugIn>()!);
        var messagePlugIn = Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!);

        pet.ExpiresAt = DateTime.UtcNow.AddSeconds(-1);
        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);
        itemUpgradedPlugIn.Verify(v => v.ItemUpgradedAsync(pet), Times.Never, "the handling failed");

        failRefresh = false;
        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);
        await plugIn.ExecuteTaskAsync((GameContext)player.GameContext).ConfigureAwait(false);

        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(healthWithoutPet), "the expired pet has no effect");
        itemUpgradedPlugIn.Verify(v => v.ItemUpgradedAsync(pet), Times.Once);
        messagePlugIn.Verify(v => v.ShowMessageAsync(It.Is<string>(m => m.Contains("Rented pet")), MessageType.BlueNormal), Times.Once);
    }

    private static async ValueTask<Player> CreatePlayerAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        await player.GameContext.AddPlayerAsync(player).ConfigureAwait(false);
        return player;
    }

    private static async ValueTask<Item> EquipPetAsync(Player player, DateTime expiresAt)
    {
        var slot = new BasicModel.ItemSlotType();
        slot.ItemSlots.Add(InventoryConstants.PetSlot);
        var definition = new BasicModel.ItemDefinition { Name = "Rented pet", Durability = 255, ItemSlot = slot, Width = 1, Height = 1 };
        definition.BasePowerUpAttributes.Add(new BasicModel.ItemBasePowerUpDefinition
        {
            TargetAttribute = Stats.MaximumHealth,
            BaseValue = PetHealth,
            AggregateType = AggregateType.AddRaw,
        });

        var pet = player.PersistenceContext.CreateNew<Item>();
        pet.Definition = definition;
        pet.Durability = definition.Durability;
        pet.ExpiresAt = expiresAt;
        await player.Inventory!.AddItemAsync(InventoryConstants.PetSlot, pet).ConfigureAwait(false);
        return pet;
    }
}
