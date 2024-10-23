// <copyright file="LowerRefineStoneConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the Lower Refine Stone which increases the item option of <see cref="ItemOptionTypes.HarmonyOption"/>.
/// </summary>
[Guid("71380E37-7AA9-447A-8A83-D08B676E55E1")]
[PlugIn(nameof(LowerRefineStoneConsumeHandlerPlugIn), "Plugin which handles the lower refine stone consumption.")]
public class LowerRefineStoneConsumeHandlerPlugIn : RefineStoneUpgradeConsumeHandlerPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LowerRefineStoneConsumeHandlerPlugIn" /> class.
    /// </summary>
    public LowerRefineStoneConsumeHandlerPlugIn()
        : base(new ItemUpgradeConfiguration(ItemOptionTypes.HarmonyOption, false, true, 0.2, ItemFailResult.SetOptionToBaseLevel))
    {
    }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.LowerRefineStone;
}