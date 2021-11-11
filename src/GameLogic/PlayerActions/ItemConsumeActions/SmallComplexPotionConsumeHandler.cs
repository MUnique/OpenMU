// <copyright file="SmallComplexPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Consume handler for small complex potions.
/// </summary>
public class SmallComplexPotionConsumeHandler : ComplexPotionConsumeHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SmallComplexPotionConsumeHandler"/> class.
    /// </summary>
    public SmallComplexPotionConsumeHandler()
        : base(new SmallHealthPotionConsumeHandler(), new SmallShieldPotionConsumeHandler())
    {
    }
}