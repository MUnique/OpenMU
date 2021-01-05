// <copyright file="HigherRefineStoneConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Consume handler for the Lower Refine Stone which increases the item option of <see cref="ItemOptionTypes.HarmonyOption"/>.
    /// </summary>
    public class HigherRefineStoneConsumeHandler : ItemUpgradeConsumeHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HigherRefineStoneConsumeHandler" /> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public HigherRefineStoneConsumeHandler(IPersistenceContextProvider persistenceContextProvider)
            : base(persistenceContextProvider, new ItemUpgradeConfiguration(ItemOptionTypes.HarmonyOption, false, true, 0.5, ItemFailResult.SetOptionToLevelOne))
        {
        }
    }
}
