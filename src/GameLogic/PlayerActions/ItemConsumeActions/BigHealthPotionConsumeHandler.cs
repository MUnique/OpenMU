// -----------------------------------------------------------------------
// <copyright file="BigHealthPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Consume handler for big health potions.
/// </summary>
public class BigHealthPotionConsumeHandler : HealthPotionConsumeHandler
{
    /// <inheritdoc/>
    protected override int Multiplier
    {
        get { return 3; }
    }
}