// -----------------------------------------------------------------------
// <copyright file="BigManaPotionConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    /// <summary>
    /// Consume handler for gib mana potions.
    /// </summary>
    public class BigManaPotionConsumeHandler : ManaPotionConsumehandler
    {
        /// <inheritdoc/>
        protected override int Multiplier
        {
            get { return 3; }
        }
    }
}
