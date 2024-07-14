// -----------------------------------------------------------------------
// <copyright file="UpgradeItemLevelJewelConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Base class for consume handlers which upgrade the item level by consuming a jewel.
/// </summary>
/// <typeparam name="TConfig">The type of the configuration.</typeparam>
public abstract class UpgradeItemLevelJewelConsumeHandlerPlugIn<TConfig>
    : ItemModifyConsumeHandlerPlugIn, ISupportCustomConfiguration<TConfig>, ISupportDefaultCustomConfiguration
    where TConfig : UpgradeItemLevelConfiguration
{
    private readonly IRandomizer _randomizer;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpgradeItemLevelJewelConsumeHandlerPlugIn{TConfig}"/> class.
    /// </summary>
    protected UpgradeItemLevelJewelConsumeHandlerPlugIn()
        : this(Rand.GetRandomizer())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpgradeItemLevelJewelConsumeHandlerPlugIn{TConfig}"/> class.
    /// </summary>
    /// <param name="randomizer">The randomizer.</param>
    protected UpgradeItemLevelJewelConsumeHandlerPlugIn(IRandomizer randomizer)
    {
        this._randomizer = randomizer;
    }

    /// <inheritdoc/>
    public TConfig? Configuration { get; set; }

    /// <inheritdoc />
    public abstract object CreateDefaultConfig();

    /// <inheritdoc/>
    protected override bool ModifyItem(Item item, IContext persistenceContext)
    {
        if (!item.CanLevelBeUpgraded())
        {
            return false;
        }

        this.Configuration ??= (TConfig)this.CreateDefaultConfig();

        if (item.Level > this.Configuration.MaximumLevel)
        {
            return false;
        }

        int percent = this.Configuration.SuccessRatePercentage;
        if (ItemHasLuck(item))
        {
            percent += this.Configuration.SuccessRateBonusWithLuckPercentage;
        }

        if (this._randomizer.NextRandomBool(percent))
        {
            item.Level++;
            item.Durability = item.GetMaximumDurabilityOfOnePiece();
            return true; // true doesn't mean that it was successful, just that the consumption happened.
        }

        if (item.Level >= this.Configuration.ResetToLevel0WhenFailMinLevel)
        {
            item.Level = 0;
        }
        else
        {
            item.Level = (byte)Math.Max(item.Level - 1, 0);
        }

        return true;
    }

    private static bool ItemHasLuck(Item item)
    {
        return item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Luck);
    }
}