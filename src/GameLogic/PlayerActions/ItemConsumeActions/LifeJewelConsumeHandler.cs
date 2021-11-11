// -----------------------------------------------------------------------
// <copyright file="LifeJewelConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

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
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    public LifeJewelConsumeHandler(IPersistenceContextProvider persistenceContextProvider)
        : base(persistenceContextProvider, new ItemUpgradeConfiguration(ItemOptionTypes.Option, true, true, 0.5, ItemFailResult.DecreaseOptionByOneOrRemove))
    {
    }
}