// <copyright file="MediumComplexPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Consume handler for medium complex potions.
/// </summary>
public class MediumComplexPotionConsumeHandler : ComplexPotionConsumeHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediumComplexPotionConsumeHandler"/> class.
    /// </summary>
    public MediumComplexPotionConsumeHandler()
        : base(new MiddleHealthPotionConsumeHandler(), new MiddleShieldPotionConsumeHandler())
    {
    }
}