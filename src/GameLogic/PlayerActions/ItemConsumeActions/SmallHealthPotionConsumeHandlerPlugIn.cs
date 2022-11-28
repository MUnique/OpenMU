// -----------------------------------------------------------------------
// <copyright file="SmallHealthPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for small health potions.
/// </summary>
[Guid("BF28D5A4-D97E-44AA-88CB-D448B1BF7A75")]
[PlugIn(nameof(SmallHealthPotionConsumeHandlerPlugIn), "Plugin which handles the small health potion consumption.")]
public class SmallHealthPotionConsumeHandlerPlugIn : HealthPotionConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.SmallHealingPotion;

    /// <inheritdoc/>
    protected override int Multiplier => 1;
}