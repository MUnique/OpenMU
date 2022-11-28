// -----------------------------------------------------------------------
// <copyright file="LargeHealthPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for big health potions.
/// </summary>
[Guid("5035BBF5-A45D-454F-9D15-4DB6F725DCFB")]
[PlugIn(nameof(LargeHealthPotionConsumeHandlerPlugIn), "Plugin which handles the large health potion consumption.")]
public class LargeHealthPotionConsumeHandlerPlugIn : HealthPotionConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.LargeHealingPotion;

    /// <inheritdoc/>
    protected override int Multiplier => 3;
}