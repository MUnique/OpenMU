// -----------------------------------------------------------------------
// <copyright file="LargeShieldPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for large shield potions.
/// </summary>
[Guid("683C4BF1-8794-41B0-9742-B17B73A12BFE")]
[PlugIn(nameof(LargeShieldPotionConsumeHandlerPlugIn), "Plugin which handles the large shield potion consumption.")]
public class LargeShieldPotionConsumeHandlerPlugIn : ShieldPotionConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.LargeShieldPotion;

    /// <inheritdoc />
    protected override double RecoverPercentage => 100;
}