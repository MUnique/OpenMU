// <copyright file="EliteManaPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the elite mana potion, which recovers the whole mana immediately.
/// </summary>
[Guid("F93ED39E-2D3C-47B3-97AB-F30EB72E8227")]
[PlugIn]
[Display(Name = nameof(PlugInResources.EliteManaPotionConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.EliteManaPotionConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class EliteManaPotionConsumeHandlerPlugIn : ElitePotionConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.EliteManaPotion;

    /// <inheritdoc />
    protected override AttributeDefinition CurrentAttribute => Stats.CurrentMana;

    /// <inheritdoc />
    protected override AttributeDefinition MaximumAttribute => Stats.MaximumMana;

    /// <inheritdoc />
    protected override double RecoverPercentage => 100;
}
