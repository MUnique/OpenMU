// <copyright file="MediumEliteHealingPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the medium elite healing potion, which recovers 65 percent of the health immediately.
/// </summary>
[Guid("FEC1CEDB-8980-4EB8-A956-D230252045E9")]
[PlugIn]
[Display(Name = nameof(PlugInResources.MediumEliteHealingPotionConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.MediumEliteHealingPotionConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class MediumEliteHealingPotionConsumeHandlerPlugIn : ElitePotionConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.MediumEliteHealingPotion;

    /// <inheritdoc />
    protected override AttributeDefinition CurrentAttribute => Stats.CurrentHealth;

    /// <inheritdoc />
    protected override AttributeDefinition MaximumAttribute => Stats.MaximumHealth;

    /// <inheritdoc />
    protected override double RecoverPercentage => 65;
}
