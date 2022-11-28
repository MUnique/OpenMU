// <copyright file="RecoverConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Consume handler which can recover attributes.
/// </summary>
public abstract class RecoverConsumeHandlerPlugIn : BaseConsumeHandlerPlugIn
{
    /// <summary>
    /// Gets the recover percent of the value of the <see cref="MaximumAttribute"/>.
    /// </summary>
    protected abstract int RecoverPercent { get; }

    /// <summary>
    /// Gets the attribute which contains the value which should get recovered.
    /// </summary>
    protected abstract AttributeDefinition CurrentAttribute { get; }

    /// <summary>
    /// Gets the attribute which contains the maximum value of the <see cref="CurrentAttribute"/>.
    /// </summary>
    protected abstract AttributeDefinition MaximumAttribute { get; }

    /// <inheritdoc/>
    public override async ValueTask<bool> ConsumeItemAsync(Player player, Item item, Item? targetItem, FruitUsage fruitUsage)
    {
        if (await base.ConsumeItemAsync(player, item, targetItem, fruitUsage).ConfigureAwait(false))
        {
            this.Recover(player);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Recovers the attributes of the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    internal void Recover(Player player)
    {
        if (player.Attributes is null)
        {
            return;
        }

        var recoverAmount = (player.Attributes[this.MaximumAttribute] * this.RecoverPercent / 100) + this.GetAdditionalRecover(player);
        player.Attributes[this.CurrentAttribute] = (uint)Math.Min(player.Attributes[this.MaximumAttribute], player.Attributes[this.CurrentAttribute] + recoverAmount);
    }

    /// <summary>
    /// Gets the additional recover value which is usually a fixed value influenced by player attributes.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The additional recover value.</returns>
    protected virtual double GetAdditionalRecover(Player player)
    {
        return 0;
    }

    /// <summary>
    /// The base class for health and mana recoverage.
    /// </summary>
    public abstract class ManaHealthConsumeHandlerPlugIn : RecoverConsumeHandlerPlugIn
    {
        /// <summary>
        /// Gets the multiplier of 50 for the additional recover.
        /// </summary>
        protected abstract int Multiplier { get; }

        /// <inheritdoc/>
        protected override int RecoverPercent => this.Multiplier * 10;

        /// <inheritdoc/>
        protected override double GetAdditionalRecover(Player player)
        {
            return Math.Max(0, 50 + (this.Multiplier * 50) - player.Attributes![Stats.Level]);
        }
    }
}