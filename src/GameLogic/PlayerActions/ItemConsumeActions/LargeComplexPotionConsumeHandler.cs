// <copyright file="LargeComplexPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    /// <summary>
    /// Consume handler for large complex potions.
    /// </summary>
    public class LargeComplexPotionConsumeHandler : ComplexPotionConsumeHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LargeComplexPotionConsumeHandler"/> class.
        /// </summary>
        public LargeComplexPotionConsumeHandler()
            : base(new BigHealthPotionConsumeHandler(), new BigShieldPotionConsumeHandler())
        {
        }
    }
}