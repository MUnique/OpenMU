// -----------------------------------------------------------------------
// <copyright file="ItemUpgradeConsumeHandler.cs" company="MUnique">
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
    /// An item consume handler which upgrades the target item.
    /// </summary>
    public class ItemUpgradeConsumeHandler : ItemModifyConsumeHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemUpgradeConsumeHandler"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="configuration">The configuration.</param>
        internal ItemUpgradeConsumeHandler(IPersistenceContextProvider persistenceContextProvider, ItemUpgradeConfiguration configuration)
            : base(persistenceContextProvider)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Specifies what should happen with the item, if the upgrading failed randomly.
        /// </summary>
        public enum ItemFailResult
        {
            /// <summary>
            /// Nothing happens.
            /// </summary>
            None,

            /// <summary>
            /// Sets the option to level one.
            /// </summary>
            SetOptionToLevelOne,

            /// <summary>
            /// Decreases the option by one level.
            /// </summary>
            DecreaseOptionByOne,

            /// <summary>
            /// Decreases the option by one level, or removes the option if the level would reach 0.
            /// </summary>
            DecreaseOptionByOneOrRemove,
        }

        /// <summary>
        /// Gets the upgrade configuration.
        /// </summary>
        internal ItemUpgradeConfiguration Configuration { get; }

        /// <inheritdoc/>
        protected override bool ModifyItem(Item item, IContext persistenceContext)
        {
            if (!this.ItemCanHaveOption(item))
            {
                return false;
            }

            if (this.ItemHasOptionAlready(item))
            {
                return this.TryUpgradeItemOption(item);
            }

            return this.TryAddItemOption(item, persistenceContext);
        }

        private bool TryUpgradeItemOption(Item item)
        {
            if (!this.Configuration.IncreasesOption)
            {
                return false;
            }

            var itemOption = item.ItemOptions.First(o => o.ItemOption.OptionType == this.Configuration.OptionType);
            var increasableOption = itemOption.ItemOption;
            var higherOptionPossible = increasableOption?.LevelDependentOptions.Any(o => o.Level > itemOption.Level) ?? false;
            if (!higherOptionPossible)
            {
                return false;
            }

            if (Rand.NextRandomBool(this.Configuration.SuccessChance))
            {
                itemOption.Level++;
            }
            else
            {
                this.HandleFailedUpgrade(item, itemOption);
            }

            return true;
        }

        private void HandleFailedUpgrade(Item item, ItemOptionLink itemOption)
        {
            switch (this.Configuration.FailResult)
            {
                case ItemFailResult.DecreaseOptionByOne:
                    itemOption.Level = Math.Max(itemOption.Level - 1, 1);
                    break;
                case ItemFailResult.DecreaseOptionByOneOrRemove:
                    itemOption.Level -= 1;
                    if (itemOption.Level == 0)
                    {
                        item.ItemOptions.Remove(itemOption);
                    }

                    break;
                case ItemFailResult.SetOptionToLevelOne:
                    itemOption.Level = 1;
                    break;
                default:
                    // do nothing
                    break;
            }
        }

        private bool TryAddItemOption(Item item, IContext persistenceContext)
        {
            if (!this.Configuration.AddsOption)
            {
                return false;
            }

            if (Rand.NextRandomBool(this.Configuration.SuccessChance))
            {
                var possibleOptions = item.Definition.PossibleItemOptions.
                    SelectMany(o => o.PossibleOptions).
                    Where(o => o.OptionType == this.Configuration.OptionType
                               && (!o.LevelDependentOptions.Any() || o.LevelDependentOptions.Any(ldo => ldo.RequiredItemLevel <= item.Level))).ToList();
                if (!possibleOptions.Any())
                {
                    return false;
                }

                var optionLink = persistenceContext.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = possibleOptions.SelectRandom();
                optionLink.Level = 1;
                item.ItemOptions.Add(optionLink);
            }

            return true;
        }

        private bool ItemHasOptionAlready(Item item)
        {
            return item.ItemOptions.Any(o => o.ItemOption.OptionType == this.Configuration.OptionType);
        }

        private bool ItemCanHaveOption(Item item)
        {
            return item.Definition.PossibleItemOptions.Any(o => o.PossibleOptions.Any(p => p.OptionType == this.Configuration.OptionType));
        }

        /// <summary>
        /// The upgrade configuration.
        /// </summary>
        internal class ItemUpgradeConfiguration
        {
            /// <summary>
            /// Gets or sets the type of the option.
            /// </summary>
            public ItemOptionType OptionType { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the handler adds option, if the item does not already have it.
            /// </summary>
            public bool AddsOption { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the handler increases the option.
            /// </summary>
            public bool IncreasesOption { get; set; }

            /// <summary>
            /// Gets or sets the success chance between 0 and 1.
            /// </summary>
            public double SuccessChance { get; set; }

            /// <summary>
            /// Gets or sets what should happen with the item if the upgrade fails because of no success.
            /// </summary>
            public ItemFailResult FailResult { get; set; }

            /// <summary>
            /// Gets or sets the option type which can boost the success rate.
            /// </summary>
            /// <remarks>
            /// e.g. luck option which adds 25 per cent.
            /// </remarks>
            public ItemOptionType BoostOptionType { get; set; }

            /// <summary>
            /// Gets or sets the success chance boost if the target item has the option of type specified in <see cref="BoostOptionType"/>.
            /// </summary>
            public double SuccessChanceBoost { get; set; }
        }
    }
}
