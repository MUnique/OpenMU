// -----------------------------------------------------------------------
// <copyright file="LargeManaPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for gib mana potions.
/// </summary>
[Guid("21CB28A4-BE9A-421C-9C7C-6F2E0FC9D614")]
[PlugIn(nameof(LargeManaPotionConsumeHandler), "Plugin which handles the large mana potion consumption.")]
public class LargeManaPotionConsumeHandler : ManaPotionConsumeHandler
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.LargeManaPotion;

    /// <inheritdoc/>
    protected override int Multiplier => 3;
}