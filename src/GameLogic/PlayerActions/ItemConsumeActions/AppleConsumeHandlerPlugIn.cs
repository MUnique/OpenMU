// -----------------------------------------------------------------------
// <copyright file="AppleConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for apples.
/// </summary>
[Guid("58518298-42BC-48D1-AB07-17A9D83A2103")]
[PlugIn(nameof(AppleConsumeHandlerPlugIn), "Plugin which handles the apple consumption.")]
public class AppleConsumeHandlerPlugIn : HealthPotionConsumeHandlerPlugIn
{
    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.Apple;

    /// <inheritdoc/>
    protected override int Multiplier => 0;
}