// <copyright file="HigherRefineStoneConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the Higher Refine Stone which increases the item option of <see cref="ItemOptionTypes.HarmonyOption"/>.
/// </summary>
[Guid("A9F58DF6-06DB-4187-B386-9F00382333EE")]
[PlugIn(nameof(HigherRefineStoneConsumeHandlerPlugIn), "Plugin which handles the higher refine stone consumption.")]
public class HigherRefineStoneConsumeHandlerPlugIn : RefineStoneUpgradeConsumeHandlerPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HigherRefineStoneConsumeHandlerPlugIn" /> class.
    /// </summary>
    public HigherRefineStoneConsumeHandlerPlugIn()
        : base(new ItemUpgradeConfiguration(ItemOptionTypes.HarmonyOption, false, true, 0.8, ItemFailResult.SetOptionToBaseLevel))
    {
    }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.HigherRefineStone;
}