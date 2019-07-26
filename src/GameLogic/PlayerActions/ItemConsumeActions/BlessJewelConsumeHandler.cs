// -----------------------------------------------------------------------
// <copyright file="BlessJewelConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Consume handler for upgrading items up to level 6 using the Jewel of Bless.
    /// </summary>
    public class BlessJewelConsumeHandler : ItemModifyConsumeHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlessJewelConsumeHandler"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public BlessJewelConsumeHandler(IPersistenceContextProvider persistenceContextProvider)
            : base(persistenceContextProvider)
        {
        }

        /// <inheritdoc/>
        protected override bool ModifyItem(Item item, IContext persistenceContext)
        {
            if (!item.CanLevelBeUpgraded())
            {
                return false;
            }

            byte level = item.Level;
            if (level > 5)
            {
                // Webzen's server lacks of such a check... ;)
                return false;
            }

            level++;
            item.Level = level;
            return true;
        }
    }
}
