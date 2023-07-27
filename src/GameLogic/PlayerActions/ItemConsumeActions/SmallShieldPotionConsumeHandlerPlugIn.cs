// -----------------------------------------------------------------------
// <copyright file="SmallShieldPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for small shield potions.
/// </summary>
[Guid("C403C8D7-9143-42BC-9894-CA285303E17A")]
[PlugIn(nameof(SmallShieldPotionConsumeHandlerPlugIn), "Plugin which handles the small shield potion consumption.")]
public class SmallShieldPotionConsumeHandlerPlugIn : ShieldPotionConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.SmallShieldPotion;

    /// <inheritdoc />
    protected override double RecoverPercentage => 20;
}