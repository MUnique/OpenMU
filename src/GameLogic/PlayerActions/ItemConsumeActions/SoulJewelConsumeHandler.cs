// -----------------------------------------------------------------------
// <copyright file="SoulJewelConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Consume handler for the Jewel of Soul which increases the item level by one until the level of 9 with a chance of 50%.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions.ItemModifyConsumeHandler" />
    public class SoulJewelConsumeHandler : ItemModifyConsumeHandler
    {
        private readonly IRandomizer randomizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoulJewelConsumeHandler"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public SoulJewelConsumeHandler(IPersistenceContextProvider persistenceContextProvider)
            : this(persistenceContextProvider, Rand.GetRandomizer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoulJewelConsumeHandler"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="randomizer">The randomizer.</param>
        public SoulJewelConsumeHandler(IPersistenceContextProvider persistenceContextProvider, IRandomizer randomizer)
            : base(persistenceContextProvider)
        {
            this.randomizer = randomizer;
        }

        /// <inheritdoc/>
        protected override bool ModifyItem(Item item, IContext persistenceContext)
        {
            if (!item.CanLevelBeUpgraded())
            {
                return false;
            }

            if (item.Level > 8)
            {
                return false;
            }

            int percent = 50;
            if (ItemHasLuck(item))
            {
                percent += 25;
            }

            if (this.randomizer.NextRandomBool(percent))
            {
                item.Level++;
                return true; // true doesn't mean that it was successful, just that the consumption happened.
            }

            if (item.Level > 6)
            {
                item.Level = 0;
            }
            else
            {
                item.Level = (byte)Math.Max(item.Level - 1, 0);
            }

            return true;
        }

        private static bool ItemHasLuck(Item item)
        {
            return item.ItemOptions.Any(o => o.ItemOption.OptionType == ItemOptionTypes.Luck);
        }
    }
}
