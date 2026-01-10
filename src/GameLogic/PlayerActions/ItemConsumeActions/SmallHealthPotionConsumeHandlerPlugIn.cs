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
[PlugIn]
[Display(Name = nameof(PlugInResources.SmallHealthPotionConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.SmallHealthPotionConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class SmallHealthPotionConsumeHandlerPlugIn : HealthPotionConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.SmallHealingPotion;

    /// <inheritdoc/>
    protected override int Multiplier => 1;
}