// <copyright file="HarmonyJewelConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// The consume handler for the harmony jewel.
    /// </summary>
    public class HarmonyJewelConsumeHandler : ItemUpgradeConsumeHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HarmonyJewelConsumeHandler" /> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public HarmonyJewelConsumeHandler(IPersistenceContextProvider persistenceContextProvider)
            : base(persistenceContextProvider, new ItemUpgradeConfiguration { AddsOption = true, IncreasesOption = false, OptionType = ItemOptionTypes.HarmonyOption, SuccessChance = 0.5 })
        {
        }
    }
}
