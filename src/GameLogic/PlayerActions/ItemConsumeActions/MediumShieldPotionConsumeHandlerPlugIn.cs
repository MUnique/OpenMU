// -----------------------------------------------------------------------
// <copyright file="MediumShieldPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for middle shield potions.
/// </summary>
[Guid("5205B818-68DE-4639-BA50-85CD285CDC95")]
[PlugIn(nameof(MediumShieldPotionConsumeHandlerPlugIn), "Plugin which handles the medium shield potion consumption.")]
public class MediumShieldPotionConsumeHandlerPlugIn : ShieldPotionConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.MediumShieldPotion;

    /// <inheritdoc />
    protected override double RecoverPercentage => 40;
}