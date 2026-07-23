// <copyright file="GoldenArcherRegistrationAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using System.Linq;
using System.Threading.Tasks;


/// <summary>
/// Action to register an event chip (or rena) at the Golden Archer.
/// </summary>
public class GoldenArcherRegistrationAction
{
    /// <summary>
    /// Registers the event chip/rena.
    /// </summary>
    /// <param name="player">The player who is registering.</param>
    public async ValueTask RegisterAsync(Player player)
    {
        // Check if user talks with the NPC - golder archer
        if (player.OpenedNpc == null)
        {
            return;
        }

        if (player.OpenedNpc.Definition.Number != 236)
        {
            return;
        }

        // Find the first matching item (Rena) in the entire inventory
        var item = player.Inventory?.Items.FirstOrDefault(i =>
            i.Definition?.Group == 14 && i.Definition?.Number == 21 // Rena
        );

        if (item == null)
        {
            await player.ShowBlueMessageAsync("You don't have any Renas in your inventory.").ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<MUnique.OpenMU.GameLogic.Views.NPC.IGoldenArcherRegistrationResultPlugIn>(
                p => p.RegistrationResultAsync()).ConfigureAwait(false);
            return;
        }

        await player.Inventory!.RemoveItemAsync(item).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<MUnique.OpenMU.GameLogic.Views.Inventory.IItemRemovedPlugIn>(
            p => p.RemoveItemAsync(item.ItemSlot)).ConfigureAwait(false);

        // Increase the number of registered renas in the database
        player.SelectedCharacter!.RegisteredRenas++;
        player.SelectedCharacter!.TotalRegisteredRenas++;

        var config = player.GameContext.Configuration.GoldenArcherConfiguration;
        if (config == null)
        {
            await player.InvokeViewPlugInAsync<MUnique.OpenMU.GameLogic.Views.IShowMessagePlugIn>(
                p => p.ShowMessageAsync("Golden Archer configuration is missing.", MUnique.OpenMU.Interfaces.MessageType.BlueNormal)).ConfigureAwait(false);
            return;
        }

        int requiredRenas = config.RequiredRenas;
        if (requiredRenas <= 0)
        {
            requiredRenas = 1;
        }

        // Check if the required amount is reached
        if (player.SelectedCharacter!.RegisteredRenas >= requiredRenas)
        {
            // Reset the counter
            player.SelectedCharacter.RegisteredRenas -= requiredRenas;

            // Reward: Give Zen from configuration
            int zenReward = config.RewardZen;
            if (zenReward > 0)
            {
                player.TryAddMoney(zenReward);
                await player.InvokeViewPlugInAsync<MUnique.OpenMU.GameLogic.Views.Inventory.IUpdateMoneyPlugIn>(
                    p => p.UpdateMoneyAsync()).ConfigureAwait(false);
            }

            // Reward: Drop an item from the configured Reward Items list
            if (config.RewardItems != null && config.RewardItems.Any() && Rand.NextRandomBool(config.ItemDropChance / 100.0))
            {
                var dropGroup = new RewardDropItemGroup(config.RewardItems);

                var dropGenerator = player.GameContext.DropGenerator;
                var (droppedItem, money, _) = dropGenerator.GenerateItemDrop(new[] { dropGroup });

                if (droppedItem != null)
                {
                    // Find a free spot at a distance of 1-2 squares from the player
                    var dropCoordinates = player.CurrentMap!.Terrain.GetRandomCoordinate(player.Position, 2);

                    // Create the dropped item object on the ground
                    var dropped = new MUnique.OpenMU.GameLogic.DroppedItem(droppedItem, dropCoordinates, player.CurrentMap, player);

                    // Add it to the map to appear visually
                    await player.CurrentMap.AddAsync(dropped).ConfigureAwait(false);
                }

                if (money > 0)
                {
                    player.TryAddMoney((int)money);
                    await player.InvokeViewPlugInAsync<MUnique.OpenMU.GameLogic.Views.Inventory.IUpdateMoneyPlugIn>(
                        p => p.UpdateMoneyAsync()).ConfigureAwait(false);
                }
                await player.ShowBlueMessageAsync($"Total Renas registered all-time: {player.SelectedCharacter?.TotalRegisteredRenas}").ConfigureAwait(false);
            }
        }

        // Send a response back to the client to update the count in the window
        await player.InvokeViewPlugInAsync<MUnique.OpenMU.GameLogic.Views.NPC.IGoldenArcherRegistrationResultPlugIn>(
            p => p.RegistrationResultAsync()).ConfigureAwait(false);
    }

    private class RewardDropItemGroup : MUnique.OpenMU.DataModel.Configuration.DropItemGroup
    {
        public RewardDropItemGroup(System.Collections.Generic.ICollection<MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition> items)
        {
            this.Chance = 1.0;
            this.Description = "Golden Archer Reward";
            this.PossibleItems = items;
        }
    }
}
