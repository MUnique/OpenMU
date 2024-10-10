// -----------------------------------------------------------------------
// <copyright file="LifeJewelConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the Jewel of Life which adds and increases the item option of <see cref="ItemOptionTypes.Option"/>.
/// </summary>
[Guid("8AC6592D-D51C-47C9-B491-4778C615691D")]
[PlugIn(nameof(LifeJewelConsumeHandlerPlugIn), "Plugin which handles the jewel of life consumption.")]
public class LifeJewelConsumeHandlerPlugIn : ItemUpgradeConsumeHandlerPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LifeJewelConsumeHandlerPlugIn" /> class.
    /// </summary>
    public LifeJewelConsumeHandlerPlugIn()
        : base(new ItemUpgradeConfiguration(ItemOptionTypes.Option, true, true, 0.5, ItemFailResult.RemoveOption))
    {
    }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.JewelOfLife;
}