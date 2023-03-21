// -----------------------------------------------------------------------
// <copyright file="MediumHealthPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for middle health potions.
/// </summary>
[Guid("2ED0A431-B562-4097-AAE4-C972074BDCBA")]
[PlugIn(nameof(MediumHealthPotionConsumeHandlerPlugIn), "Plugin which handles the medium health potion consumption.")]
public class MediumHealthPotionConsumeHandlerPlugIn : HealthPotionConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.MediumHealingPotion;

    /// <inheritdoc/>
    protected override int Multiplier => 2;
}