// <copyright file="InventoryStorage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;
    using static OpenMU.GameLogic.InventoryConstants;

    /// <summary>
    /// The storage of an inventory of a player, which also contains equippable slots. This class also manages the powerups which get created by equipped items.
    /// </summary>
    public class InventoryStorage : Storage
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(InventoryStorage));

        private readonly IGameContext gameContext;

        private readonly Player player;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryStorage" /> class.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="context">The game context.</param>
        public InventoryStorage(Player player, IGameContext context)
            : base(
                FirstEquippableItemSlotIndex,
                LastEquippableItemSlotIndex,
                GetInventorySize(player),
                new ItemStorageAdapter(player.SelectedCharacter.Inventory, FirstEquippableItemSlotIndex, GetInventorySize(player)))
        {
            this.player = player;
            this.EquippedItemsChanged += (sender, eventArgs) => this.UpdateItemsOnChange(eventArgs.Item);
            this.gameContext = context;
            this.InitializePowerUps();
        }

        /// <inheritdoc />
        /// <remarks>Additionally we need to make a temporary item persistent with the context of the player.</remarks>
        public override bool AddItem(byte slot, Item item)
        {
            Item convertedItem = null;
            if (item is TemporaryItem temporaryItem)
            {
                convertedItem = temporaryItem.MakePersistent(this.player.PersistenceContext);
            }

            var success = base.AddItem(slot, item);
            if (!success && convertedItem != null)
            {
                this.player.PersistenceContext.Delete(convertedItem);
            }

            return success;
        }

        private void UpdateItemsOnChange(Item item)
        {
            this.player.OnAppearanceChanged();
            this.player.ForEachObservingPlayer(p => p.PlayerView.AppearanceChanged(this.player), false); // in my tests it was not needed to send the appearance to the own players client.
            if (this.player.Attributes.ItemPowerUps.TryGetValue(item, out IReadOnlyList<PowerUpWrapper> itemPowerUps))
            {
                this.player.Attributes.ItemPowerUps.Remove(item);
                foreach (var powerUp in itemPowerUps)
                {
                    powerUp.Dispose();
                }
            }

            if (item.ItemSetGroups != null && item.ItemSetGroups.Any())
            {
                this.UpdateSetPowerUps();
            }

            var itemAdded = this.EquippedItems.Contains(item);
            if (itemAdded)
            {
                var factory = this.gameContext.ItemPowerUpFactory;
                this.player.Attributes.ItemPowerUps.Add(item, factory.GetPowerUps(item, this.player.Attributes).ToList());
            }
        }

        private void InitializePowerUps()
        {
            foreach (var powerUp in this.player.Attributes.ItemPowerUps.Values.SelectMany(p => p).ToList())
            {
                powerUp.Dispose();
            }

            var factory = this.gameContext?.ItemPowerUpFactory;
            if (factory != null)
            {
                foreach (var item in this.EquippedItems)
                {
                    this.player.Attributes.ItemPowerUps.Add(item, factory.GetPowerUps(item, this.player.Attributes).ToList());
                }

                this.UpdateSetPowerUps();
            }
            else
            {
                Log.Error("item power up factory not available.");
            }
        }

        private void UpdateSetPowerUps()
        {
            if (this.player.Attributes.ItemSetPowerUps != null)
            {
                foreach (var powerUp in this.player.Attributes.ItemSetPowerUps)
                {
                    powerUp.Dispose();
                }
            }

            var factory = this.gameContext.ItemPowerUpFactory;
            this.player.Attributes.ItemSetPowerUps = factory.GetSetPowerUps(this.EquippedItems, this.player.Attributes).ToList();
        }
    }
}
