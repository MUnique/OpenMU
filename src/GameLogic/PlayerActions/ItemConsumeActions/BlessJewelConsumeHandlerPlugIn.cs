// -----------------------------------------------------------------------
// <copyright file="BlessJewelConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for upgrading items up to level 6 using the Jewel of Bless.
/// </summary>
[Guid("E95A0292-B3B4-4E8C-AC5A-7F3DB4F01A37")]
[PlugIn(nameof(BlessJewelConsumeHandlerPlugIn), "Plugin which handles the jewel of bless consumption.")]
public class BlessJewelConsumeHandlerPlugIn : UpgradeItemLevelJewelConsumeHandlerPlugIn<BlessJewelConsumeHandlerPlugInConfiguration>
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.JewelOfBless;

    /// <inheritdoc />
    public override object CreateDefaultConfig()
    {
        return new BlessJewelConsumeHandlerPlugInConfiguration
        {
            MaximumLevel = 5,
            MinimumLevel = 0,
            SuccessRatePercentage = 100,
            SuccessRateBonusWithLuckPercentage = 0,
            ResetToLevel0WhenFailMinLevel = 0,
        };
    }

    /// <inheritdoc/>
    protected override bool ModifyItem(Item item, IContext persistenceContext)
    {
        if (this.Configuration?.RepairTargetItems.Contains(item.Definition!) is true
            && item.Durability < item.GetMaximumDurabilityOfOnePiece())
        {
            item.Durability = item.GetMaximumDurabilityOfOnePiece();
            return true;
        }

        return base.ModifyItem(item, persistenceContext);
    }
}