// -----------------------------------------------------------------------
// <copyright file="SmallManaPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    /// <summary>
    /// Consume handler for small health potions.
    /// </summary>
    public class SmallManaPotionConsumeHandler : ManaPotionConsumehandler
    {
        /// <inheritdoc/>
        protected override int Multiplier
        {
            get { return 1; }
        }
    }
}
