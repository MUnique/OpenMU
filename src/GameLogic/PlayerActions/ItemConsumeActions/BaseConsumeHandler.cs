﻿// -----------------------------------------------------------------------
// <copyright file="BaseConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Base class of an item consumption handler.
/// </summary>
public class BaseConsumeHandler : IItemConsumeHandler
{
    /// <inheritdoc/>
    public virtual bool ConsumeItem(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (!this.CheckPreconditions(player, item))
        {
            return false;
        }

        this.ConsumeSourceItem(player, item);

        return true;
    }

    /// <summary>
    /// Consumes the source item.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="item">The item.</param>
    protected void ConsumeSourceItem(Player player, Item item)
    {
        if (item.Durability > 0)
        {
            item.Durability -= 1;
        }

        if (item.Durability == 0)
        {
            player.Inventory?.RemoveItem(item);
            player.PersistenceContext.Delete(item);
        }
    }

    /// <summary>
    /// Checks the preconditions.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="item">The item.</param>
    /// <returns><c>True</c>, if preconditions are met.</returns>
    protected bool CheckPreconditions(Player player, Item item)
    {
        if (player.PlayerState.CurrentState != PlayerState.EnteredWorld
            || item.Durability == 0)
        {
            return false;
        }

        return true;
    }
}