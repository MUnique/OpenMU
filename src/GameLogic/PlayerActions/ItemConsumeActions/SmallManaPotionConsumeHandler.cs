// -----------------------------------------------------------------------
// <copyright file="SmallManaPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for small health potions.
/// </summary>
[Guid("A55849BC-7BD7-4444-A35A-F1AC1D48F179")]
[PlugIn(nameof(SmallManaPotionConsumeHandler), "Plugin which handles the small mana potion consumption.")]
public class SmallManaPotionConsumeHandler : ManaPotionConsumeHandler
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.SmallManaPotion;

    /// <inheritdoc/>
    protected override int Multiplier => 1;
}