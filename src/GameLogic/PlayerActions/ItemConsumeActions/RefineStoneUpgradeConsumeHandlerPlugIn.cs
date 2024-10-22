// -----------------------------------------------------------------------
// <copyright file="RefineStoneUpgradeConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Consume handler for a Refine Stone which increases the item option of <see cref="ItemOptionTypes.HarmonyOption"/>.
/// </summary>
public abstract class RefineStoneUpgradeConsumeHandlerPlugIn : ItemUpgradeConsumeHandlerPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefineStoneUpgradeConsumeHandlerPlugIn"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    private protected RefineStoneUpgradeConsumeHandlerPlugIn(ItemUpgradeConfiguration configuration)
        : base(configuration)
    {
    }

    /// <inheritdoc/>
    protected override bool TryUpgradeItemOption(Item item)
    {
        var harmonyOption = item.ItemOptions.First(o => o.ItemOption?.OptionType == this.Configuration.OptionType);
        var levelOptions = harmonyOption.ItemOption?.LevelDependentOptions;

        if (levelOptions?.FirstOrDefault()?.PowerUpDefinition?.TargetAttribute == Stats.MinimumPhysBaseDmg)
        { // The difference betwen the max and min dmg of a weapon must be at least 1
            var weaponMinDmg = item.Definition!.BasePowerUpAttributes.First(bpu => bpu.TargetAttribute == Stats.MinimumPhysBaseDmgByWeapon).BaseValue;
            var weaponMaxDmg = item.Definition!.BasePowerUpAttributes.First(bpu => bpu.TargetAttribute == Stats.MaximumPhysBaseDmgByWeapon).BaseValue;
            var nextMinDmgBoost = levelOptions.FirstOrDefault(o => o.Level == harmonyOption.Level + 1)?.PowerUpDefinition?.Boost?.ConstantValue.Value;
            if (nextMinDmgBoost is float boost && weaponMaxDmg - (weaponMinDmg + boost) < 1)
            {
                return false;
            }
        }

        return base.TryUpgradeItemOption(item);
    }
}