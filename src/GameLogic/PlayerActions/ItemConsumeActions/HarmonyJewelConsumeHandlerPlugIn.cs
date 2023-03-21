// <copyright file="HarmonyJewelConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The consume handler for the harmony jewel.
/// </summary>
[Guid("DAC3E5C2-FF0F-4773-AFBF-EBDC0C35336D")]
[PlugIn(nameof(HarmonyJewelConsumeHandlerPlugIn), "Plugin which handles the jewel of harmony consumption.")]
public class HarmonyJewelConsumeHandlerPlugIn : ItemUpgradeConsumeHandlerPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HarmonyJewelConsumeHandlerPlugIn" /> class.
    /// </summary>
    public HarmonyJewelConsumeHandlerPlugIn()
        : base(new ItemUpgradeConfiguration(ItemOptionTypes.HarmonyOption, true, false, 0.5, ItemFailResult.None))
    {
    }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.JewelOfHarmony;
}