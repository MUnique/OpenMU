// <copyright file="SmallComplexPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for small complex potions.
/// </summary>
[Guid("9424D511-0AF7-4FC2-BD11-24799368D651")]
[PlugIn(nameof(SmallComplexPotionConsumeHandlerPlugIn), "Plugin which handles the small complex potion consumption.")]
public class SmallComplexPotionConsumeHandlerPlugIn : ComplexPotionConsumeHandlerPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SmallComplexPotionConsumeHandlerPlugIn"/> class.
    /// </summary>
    public SmallComplexPotionConsumeHandlerPlugIn()
        : base(new SmallHealthPotionConsumeHandlerPlugIn(), new SmallShieldPotionConsumeHandlerPlugIn())
    {
    }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.SmallComplexPotion;
}