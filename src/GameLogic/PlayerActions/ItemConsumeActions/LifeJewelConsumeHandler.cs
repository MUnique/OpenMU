// -----------------------------------------------------------------------
// <copyright file="LifeJewelConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Consume handler for the Jewel of Life which adds and increases the item option of <see cref="ItemOptionTypes.Option"/>.
    /// </summary>
    public class LifeJewelConsumeHandler : ItemUpgradeConsumeHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifeJewelConsumeHandler" /> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        public LifeJewelConsumeHandler(IRepositoryManager repositoryManager)
            : base(repositoryManager, new ItemUpgradeConfiguration { AddsOption = true, IncreasesOption = false, FailResult = ItemFailResult.DecreaseOptionByOneOrRemove, OptionType = ItemOptionTypes.Option, SuccessChance = 0.5 })
        {
        }
    }
}
