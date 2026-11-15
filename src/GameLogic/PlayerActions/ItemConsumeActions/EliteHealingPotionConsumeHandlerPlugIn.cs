// <copyright file="EliteHealingPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the elite healing potion, which recovers the whole health immediately.
/// </summary>
[Guid("B880C615-8463-453A-B940-43629B241C53")]
[PlugIn]
[Display(Name = nameof(PlugInResources.EliteHealingPotionConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.EliteHealingPotionConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class EliteHealingPotionConsumeHandlerPlugIn : ElitePotionConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.EliteHealingPotion;

    /// <inheritdoc />
    protected override AttributeDefinition CurrentAttribute => Stats.CurrentHealth;

    /// <inheritdoc />
    protected override AttributeDefinition MaximumAttribute => Stats.MaximumHealth;

    /// <inheritdoc />
    protected override double RecoverPercentage => 100;
}
