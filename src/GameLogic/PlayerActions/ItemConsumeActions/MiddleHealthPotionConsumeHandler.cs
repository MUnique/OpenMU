// -----------------------------------------------------------------------
// <copyright file="MiddleHealthPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Consume handler for middle health potions.
/// </summary>
public class MiddleHealthPotionConsumeHandler : HealthPotionConsumeHandler
{
    /// <inheritdoc/>
    protected override int Multiplier
    {
        get { return 2; }
    }
}