// <copyright file="LowerRefineStoneConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Consume handler for the Lower Refine Stone which increases the item option of <see cref="ItemOptionTypes.HarmonyOption"/>.
    /// </summary>
    public class LowerRefineStoneConsumeHandler : ItemUpgradeConsumeHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LowerRefineStoneConsumeHandler" /> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        public LowerRefineStoneConsumeHandler(IRepositoryManager repositoryManager)
            : base(repositoryManager, new ItemUpgradeConfiguration { AddsOption = false, IncreasesOption = true, OptionType = ItemOptionTypes.HarmonyOption, SuccessChance = 0.2, FailResult = ItemFailResult.SetOptionToLevelOne })
        {
        }
    }
}
