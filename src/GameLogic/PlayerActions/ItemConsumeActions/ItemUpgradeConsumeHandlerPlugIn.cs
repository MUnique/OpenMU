// -----------------------------------------------------------------------
// <copyright file="ItemUpgradeConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence;

/// <summary>
/// An item consume handler which upgrades the target item.
/// </summary>
public abstract class ItemUpgradeConsumeHandlerPlugIn : ItemModifyConsumeHandlerPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemUpgradeConsumeHandlerPlugIn"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    internal ItemUpgradeConsumeHandlerPlugIn(ItemUpgradeConfiguration configuration)
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
        /// Sets the option to the base level.
        /// </summary>
        SetOptionToBaseLevel,

        /// <summary>
        /// Removes the option.
        /// </summary>
        RemoveOption,
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

    /// <summary>
    /// Checks if an item can have the configured option.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>Flag indicating whether the item can have the option.</returns>
    protected virtual bool ItemCanHaveOption(Item item)
    {
        return item.Definition?.PossibleItemOptions.Any(o => o.PossibleOptions.Any(p => p.OptionType == this.Configuration.OptionType)) ?? false;
    }

    /// <summary>
    /// Tries to upgrade the item option.
    /// </summary>
    /// <param name="item">The item to upgrade.</param>
    /// <returns>Flag indicating whether the item option was upgraded.</returns>
    protected virtual bool TryUpgradeItemOption(Item item)
    {
        if (!this.Configuration.IncreasesOption)
        {
            return false;
        }

        var itemOption = item.ItemOptions.First(o => o.ItemOption?.OptionType == this.Configuration.OptionType);
        var increasableOption = itemOption.ItemOption;
        var higherOptionPossible = increasableOption?.LevelDependentOptions.Any(o => o.Level > itemOption.Level && o.RequiredItemLevel <= item.Level) ?? false;
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
            case ItemFailResult.RemoveOption:
                item.ItemOptions.Remove(itemOption);
                break;
            case ItemFailResult.SetOptionToBaseLevel:
                itemOption.Level = itemOption.ItemOption?.LevelDependentOptions.Min(ldo => ldo.Level) ?? itemOption.Level;
                break;
            default:
                // do nothing
                break;
        }
    }

    private bool TryAddItemOption(Item item, IContext persistenceContext)
    {
        if (!this.Configuration.AddsOption || item.Definition is null)
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
            if (this.Configuration.OptionType == ItemOptionTypes.HarmonyOption)
            {
                // Str and agi reduction options are not always applicable, and so should be removed from the pool
                if (!item.Definition.Requirements.Any(r => r.Attribute == Stats.TotalStrengthRequirementValue)
                    && possibleOptions.FirstOrDefault(po => po.LevelDependentOptions
                        .Any(ldo => ldo.PowerUpDefinition?.TargetAttribute == Stats.RequiredStrengthReduction)) is { } strReductOpt)
                {
                    possibleOptions.Remove(strReductOpt);
                }

                if (!item.Definition.Requirements.Any(r => r.Attribute == Stats.TotalAgilityRequirementValue)
                    && possibleOptions.FirstOrDefault(po => po.LevelDependentOptions
                        .Any(ldo => ldo.PowerUpDefinition?.TargetAttribute == Stats.RequiredAgilityReduction)) is { } agiReductOpt)
                {
                    possibleOptions.Remove(agiReductOpt);
                }

                optionLink.ItemOption = possibleOptions.SelectWeightedRandom(possibleOptions.Select(po => (int)po.Weight));
                optionLink.Level = optionLink.ItemOption?.LevelDependentOptions.Select(ldo => ldo.Level).Min() ?? 0;
            }
            else
            {
                // ItemOptionTypes.Option
                optionLink.ItemOption = possibleOptions.SelectRandom();
                optionLink.Level = 1;
            }

            item.ItemOptions.Add(optionLink);
        }

        return true;
    }

    private bool ItemHasOptionAlready(Item item)
    {
        return item.ItemOptions.Any(o => o.ItemOption?.OptionType == this.Configuration.OptionType);
    }

    /// <summary>
    /// The upgrade configuration.
    /// </summary>
    internal class ItemUpgradeConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemUpgradeConfiguration"/> class.
        /// </summary>
        /// <param name="optionType">Type of the option.</param>
        /// <param name="addsOption">if set to <c>true</c> [adds option].</param>
        /// <param name="increasesOption">if set to <c>true</c> [increases option].</param>
        /// <param name="successChance">The success chance.</param>
        /// <param name="failResult">The fail result.</param>
        public ItemUpgradeConfiguration(ItemOptionType optionType, bool addsOption, bool increasesOption, double successChance, ItemFailResult failResult)
        {
            this.OptionType = optionType;
            this.AddsOption = addsOption;
            this.IncreasesOption = increasesOption;
            this.SuccessChance = successChance;
            this.FailResult = failResult;
        }

        /// <summary>
        /// Gets the type of the option.
        /// </summary>
        public ItemOptionType OptionType { get; }

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
        public ItemOptionType? BoostOptionType { get; set; }

        /// <summary>
        /// Gets or sets the success chance boost if the target item has the option of type specified in <see cref="BoostOptionType"/>.
        /// </summary>
        public double SuccessChanceBoost { get; set; }
    }
}