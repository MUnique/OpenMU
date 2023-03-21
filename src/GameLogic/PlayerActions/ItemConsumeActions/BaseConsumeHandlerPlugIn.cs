// -----------------------------------------------------------------------
// <copyright file="BaseConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Base class of an item consumption handler.
/// </summary>
public abstract class BaseConsumeHandlerPlugIn : IItemConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public abstract ItemIdentifier Key { get; }

    /// <inheritdoc/>
    public virtual async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (!this.CheckPreconditions(player, item))
        {
            return false;
        }

        await this.ConsumeSourceItemAsync(player, item).ConfigureAwait(false);

        return true;
    }

    /// <summary>
    /// Consumes the source item.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="item">The item.</param>
    protected async ValueTask ConsumeSourceItemAsync(Player player, Item item)
    {
        if (item.Durability > 0)
        {
            item.Durability -= 1;
        }
    }

    /// <summary>
    /// Checks the preconditions.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="item">The item.</param>
    /// <returns><c>True</c>, if preconditions are met.</returns>
    protected virtual bool CheckPreconditions(Player player, Item item)
    {
        if (player.PlayerState.CurrentState != PlayerState.EnteredWorld
            || item.Durability == 0)
        {
            return false;
        }

        return true;
    }
}