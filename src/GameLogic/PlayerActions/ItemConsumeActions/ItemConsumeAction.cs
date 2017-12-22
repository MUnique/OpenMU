// <copyright file="ItemConsumeAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Action to consume an item.
    /// </summary>
    public class ItemConsumeAction
    {
        private readonly IDictionary<ItemDefinition, IItemConsumeHandler> consumeHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemConsumeAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public ItemConsumeAction(IGameContext gameContext)
        {
            this.consumeHandlers = new Dictionary<ItemDefinition, IItemConsumeHandler>();

            // find all items with configured item consume handler
            var items = gameContext.Configuration.Items.Where(def => !string.IsNullOrEmpty(def.ConsumeHandlerClass));
            foreach (var item in items)
            {
                var type = Type.GetType(item.ConsumeHandlerClass);
                if (type != null)
                {
                    var constructors = type.GetConstructors();
                    foreach (var ctor in constructors)
                    {
                        var parameters = ctor.GetParameters();
                        if (!parameters.Any())
                        {
                            if (ctor.Invoke(new object[] { }) is IItemConsumeHandler consumeHandler)
                            {
                                this.consumeHandlers.Add(item, consumeHandler);
                                break;
                            }
                        }
                        else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(IRepositoryManager))
                        {
                            if (ctor.Invoke(new object[] { gameContext.RepositoryManager }) is IItemConsumeHandler consumeHandler)
                            {
                                this.consumeHandlers.Add(item, consumeHandler);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the consume request.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="inventorySlot">The inventory slot.</param>
        /// <param name="inventoryTargetSlot">The inventory target slot.</param>
        public void HandleConsumeRequest(Player player, byte inventorySlot, byte inventoryTargetSlot)
        {
            Item item = player.Inventory.GetItem(inventorySlot);
            if (item == null)
            {
                player.PlayerView.RequestedItemConsumptionFailed();
                return;
            }

            IItemConsumeHandler consumeHandler;
            if (!this.consumeHandlers.TryGetValue(item.Definition, out consumeHandler))
            {
                player.PlayerView.RequestedItemConsumptionFailed();
                return;
            }

            if (!consumeHandler.ConsumeItem(player, inventorySlot, inventoryTargetSlot))
            {
                player.PlayerView.RequestedItemConsumptionFailed();
                return;
            }

            if (item.Durability <= 1)
            {
                player.Inventory.RemoveItem(item);
                player.PlayerView.InventoryView.ItemConsumed(inventorySlot, true);
            }
            else
            {
                player.PlayerView.InventoryView.ItemDurabilityChanged(item);
            }
        }

        /*
                public ConsumeItemHandler(IGameContext gameContext) : base(gameContext)
                {
                    // This is specified in the ItemDefinition.ConsumeHandlerClass now:
                    this.consumeHandlers = new Dictionary<ushort, IItemConsumeHandler>();
                    this.AddConsumeHandler(0, 0xE0, new AppleConsumeHandler());
                    this.AddConsumeHandler(1, 0xE0, new SmallHealthPotionConsumeHandler());
                    this.AddConsumeHandler(2, 0xE0, new MiddleHealthPotionConsumeHandler());
                    this.AddConsumeHandler(3, 0xE0, new BigHealthPotionConsumeHandler());
                    this.AddConsumeHandler(4, 0xE0, new SmallManaPotionConsumeHandler());
                    this.AddConsumeHandler(5, 0xE0, new MiddleManaPotionConsumeHandler());
                    this.AddConsumeHandler(6, 0xE0, new BigManaPotionConsumeHandler());
                    this.AddConsumeHandler(9, 0xE0, new AlcoholConsumeHandler());
                    this.AddConsumeHandler(13, 0xE0, new BlessJewelConsumeHandler());
                    this.AddConsumeHandler(14, 0xE0, new SoulJewelConsumeHandler());
                    this.AddConsumeHandler(16, 0xE0, new LifeJewelConsumeHandler());
                    this.AddConsumeHandler(35, 0xE0, new SmallShieldPotionConsumeHandler());
                    this.AddConsumeHandler(36, 0xE0, new MiddleShieldPotionConsumeHandler());
                    this.AddConsumeHandler(37, 0xE0, new LargeShieldPotionConsumeHandler());

                    var learnablesConsumeHandler = new LearnablesConsumeHandler(gameServer);
                    this.AddConsumeHandler(AllItemsOfAGroup, 0xF0, learnablesConsumeHandler);
                    this.AddOrbConsumeHandler(learnablesConsumeHandler);
                    //not implemented yet:
                    this.AddConsumeHandler(7, 0xE0, new NotImplementedItemConsumeHandler()); //Potion of Soul
                    this.AddConsumeHandler(8, 0xE0, new NotImplementedItemConsumeHandler()); //Antidote
                    this.AddConsumeHandler(10, 0xE0, new NotImplementedItemConsumeHandler()); //Town portal scroll
                    this.AddConsumeHandler(20, 0xE0, new NotImplementedItemConsumeHandler()); //Redemy of love
                    //Complex Potions:
                    this.AddConsumeHandler(38, 0xE0, new NotImplementedItemConsumeHandler());
                    this.AddConsumeHandler(39, 0xE0, new NotImplementedItemConsumeHandler());
                    this.AddConsumeHandler(40, 0xE0, new NotImplementedItemConsumeHandler());
                    //Halloween items:
                    this.AddConsumeHandler(46, 0xE0, new NotImplementedItemConsumeHandler());
                    this.AddConsumeHandler(47, 0xE0, new NotImplementedItemConsumeHandler());
                    this.AddConsumeHandler(48, 0xE0, new NotImplementedItemConsumeHandler());
                    //Cherry Blossom Items:
                    this.AddConsumeHandler(85, 0xE0, new NotImplementedItemConsumeHandler());
                    this.AddConsumeHandler(86, 0xE0, new NotImplementedItemConsumeHandler());
                    this.AddConsumeHandler(87, 0xE0, new NotImplementedItemConsumeHandler());


                    this.AddConsumeHandler(15, 0xD0, new NotImplementedItemConsumeHandler()); //Fruits
                }

                void AddOrbConsumeHandler(IItemConsumeHandler learnablesConsumeHandler)
                {
                    var itemIds = new List<byte>()
                    {
                        7, 8, 9, 10, 11, 12, 13, 14,
                        16, 17, 18, 19, 20, 21, 22, 23, 24,
                        35, 44, 45, 46, 47, 48
                    };
                    foreach (var itemId in itemIds)
                    {
                        this.AddConsumeHandler(itemId, 0xC0, learnablesConsumeHandler);
                    }
                }
        */
    }
}

