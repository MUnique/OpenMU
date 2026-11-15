// <copyright file="EliteShieldPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the elite shield (SD) potion, which recovers 65 percent of the shield immediately.
/// </summary>
[Guid("3A04C1CE-611C-48BC-95B7-E8F22947EF09")]
[PlugIn]
[Display(Name = nameof(PlugInResources.EliteShieldPotionConsumeHandlerPlugIn_Name), Description = nameof(PlugInResources.EliteShieldPotionConsumeHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class EliteShieldPotionConsumeHandlerPlugIn : ElitePotionConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.EliteShieldPotion;

    /// <inheritdoc />
    protected override AttributeDefinition CurrentAttribute => Stats.CurrentShield;

    /// <inheritdoc />
    protected override AttributeDefinition MaximumAttribute => Stats.MaximumShield;

    /// <inheritdoc />
    protected override double RecoverPercentage => 65;
}
