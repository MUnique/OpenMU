// <copyright file="MediumComplexPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for medium complex potions.
/// </summary>
[Guid("D4ED0E2E-3CAA-4B35-BA17-230E29EC324B")]
[PlugIn(nameof(MediumComplexPotionConsumeHandlerPlugIn), "Plugin which handles the medium complex potion consumption.")]
public class MediumComplexPotionConsumeHandlerPlugIn : ComplexPotionConsumeHandlerPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediumComplexPotionConsumeHandlerPlugIn"/> class.
    /// </summary>
    public MediumComplexPotionConsumeHandlerPlugIn()
        : base(new MediumHealthPotionConsumeHandlerPlugIn(), new MediumShieldPotionConsumeHandlerPlugIn())
    {
    }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.MediumComplexPotion;
}