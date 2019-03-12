// <copyright file="ItemConsumeAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views;
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
                var consumeHandler = this.CreateConsumeHandler(gameContext, item.ConsumeHandlerClass);
                this.consumeHandlers.Add(item, consumeHandler);
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
                player.ViewPlugIns.GetPlugIn<IPlayerView>()?.RequestedItemConsumptionFailed();
                return;
            }

            if (!this.consumeHandlers.TryGetValue(item.Definition, out var consumeHandler))
            {
                player.ViewPlugIns.GetPlugIn<IPlayerView>()?.RequestedItemConsumptionFailed();
                return;
            }

            var targetItem = player.Inventory.GetItem(inventoryTargetSlot);

            if (player.GameContext.PlugInManager.GetPlugInPoint<IItemConsumingPlugIn>() is IItemConsumingPlugIn plugInPoint)
            {
                var eventArgs = new CancelEventArgs();
                plugInPoint.ItemConsuming(player, item, targetItem, eventArgs);
                if (eventArgs.Cancel)
                {
                    return;
                }
            }

            if (!consumeHandler.ConsumeItem(player, item, targetItem))
            {
                player.ViewPlugIns.GetPlugIn<IPlayerView>()?.RequestedItemConsumptionFailed();
                return;
            }

            if (item.Durability == 0)
            {
                player.Inventory.RemoveItem(item);
                player.ViewPlugIns.GetPlugIn<IInventoryView>()?.ItemConsumed(inventorySlot, true);
            }
            else
            {
                player.ViewPlugIns.GetPlugIn<IInventoryView>()?.ItemDurabilityChanged(item, true);
            }

            player.GameContext.PlugInManager.GetPlugInPoint<IItemConsumedPlugIn>()?.ItemConsumed(player, item, targetItem);
            if (item.Durability == 0)
            {
                player.GameContext.PlugInManager.GetPlugInPoint<IItemDestroyedPlugIn>()?.ItemDestroyed(item);
            }
        }

        private IItemConsumeHandler CreateConsumeHandler(IGameContext gameContext, string handlerTypeName)
        {
            var handlerType = Type.GetType(handlerTypeName);
            if (handlerType == null)
            {
                throw new ArgumentException($"The consume handler {handlerTypeName} wasn't found.", nameof(handlerTypeName));
            }

            var constructors = handlerType.GetConstructors();
            foreach (var ctor in constructors)
            {
                var parameters = ctor.GetParameters();
                if (!parameters.Any())
                {
                    return ctor.Invoke(new object[] { }) as IItemConsumeHandler
                           ?? throw new ArgumentException($"The consume handler {handlerTypeName} isn't implementing {nameof(IItemConsumeHandler)}.", nameof(handlerTypeName));
                }

                if (parameters.Length == 1 && parameters[0].ParameterType == typeof(IPersistenceContextProvider))
                {
                    return ctor.Invoke(new object[] { gameContext.PersistenceContextProvider }) as IItemConsumeHandler
                           ?? throw new ArgumentException($"The consume handler {handlerTypeName} isn't implementing {nameof(IItemConsumeHandler)}.", nameof(handlerTypeName));
                }
            }

            throw new ArgumentException($"The consume handler {handlerTypeName} has no suitable constructor. One of '{handlerTypeName}()' or '{handlerTypeName}({nameof(IPersistenceContextProvider)}) is required.", nameof(handlerTypeName));
        }
    }
}