// <copyright file="LargeComplexPotionConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for large complex potions.
/// </summary>
[Guid("F5A8C0C4-7960-4815-83C2-F57339CD6FE2")]
[PlugIn(nameof(LargeComplexPotionConsumeHandlerPlugIn), "Plugin which handles the large complex potion consumption.")]
public class LargeComplexPotionConsumeHandlerPlugIn : ComplexPotionConsumeHandlerPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LargeComplexPotionConsumeHandlerPlugIn"/> class.
    /// </summary>
    public LargeComplexPotionConsumeHandlerPlugIn()
        : base(new LargeHealthPotionConsumeHandlerPlugIn(), new LargeShieldPotionConsumeHandlerPlugIn())
    {
    }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.LargeComplexPotion;
}