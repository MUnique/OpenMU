// <copyright file="FixDamageAbsorbItemsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes the damage absorption settings for items and skills.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("280ACE93-2B96-476C-A4AF-4FDA7611D5D5")]
public class FixDamageAbsorbItemsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fixed damage absorb items/skills";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes the damage absorbtion settings for items and skills.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixDamageAbsorbItems;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 08, 16, 14, 00, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        FixSoulBarrierEffect(gameConfiguration);
        FixGuardianAngel(gameConfiguration);
        FixImp(gameConfiguration);
        FixDinorant(gameConfiguration);
        FixDemon(gameConfiguration);
        FixSpiritOfGuardian(gameConfiguration);
        FixPanda(gameConfiguration);
        FixUnicorn(gameConfiguration);
        FixSkeleton(gameConfiguration, context);

        FixHarmonyDefenseOption(gameConfiguration);

        FixWings(gameConfiguration, 12, 0);
        FixWings(gameConfiguration, 12, 1);
        FixWings(gameConfiguration, 12, 2);
        FixWings(gameConfiguration, 12, 41);

        FixWings(gameConfiguration, 12, 3);
        FixWings(gameConfiguration, 12, 4);
        FixWings(gameConfiguration, 12, 5);
        FixWings(gameConfiguration, 12, 6);
        FixWings(gameConfiguration, 12, 42);

        FixWings(gameConfiguration, 12, 49);
        FixWings(gameConfiguration, 13, 30);

        FixWings(gameConfiguration, 12, 36);
        FixWings(gameConfiguration, 12, 37);
        FixWings(gameConfiguration, 12, 38);
        FixWings(gameConfiguration, 12, 39);
        FixWings(gameConfiguration, 12, 40);
        FixWings(gameConfiguration, 12, 43);
        FixWings(gameConfiguration, 12, 50);

        FixDamageIncreaseTable(gameConfiguration, 12, 0); // First and Third Wings
        FixDamageIncreaseTable(gameConfiguration, 12, 3); // Second Wings
        FixDamageAbsorbTable(gameConfiguration); // One for all
    }

    private static void FixDamageIncreaseTable(GameConfiguration gameConfiguration, short group, short number)
    {
        var wings = gameConfiguration.Items.FirstOrDefault(item => item.Group == group && item.Number == number);
        if (wings is null)
        {
            return;
        }

        var damageIncreaseTable = wings.BasePowerUpAttributes
            .FirstOrDefault(a => a.TargetAttribute == Stats.AttackDamageIncrease)
            ?.BonusPerLevelTable;
        if (damageIncreaseTable is null)
        {
            return;
        }

        foreach (var row in damageIncreaseTable.BonusPerLevel)
        {
            if (row.AdditionalValue < 1)
            {
                row.AdditionalValue += 1f;
            }
        }
    }

    private static void FixDamageAbsorbTable(GameConfiguration gameConfiguration)
    {
        var wings = gameConfiguration.Items.FirstOrDefault(item => item.Group == 12 && item.Number == 0);
        if (wings is null)
        {
            return;
        }

        var damageDecreaseTable = wings.BasePowerUpAttributes
            .FirstOrDefault(a => a.TargetAttribute == Stats.DamageReceiveDecrement)
            ?.BonusPerLevelTable;
        if (damageDecreaseTable is null)
        {
            return;
        }

        foreach (var row in damageDecreaseTable.BonusPerLevel)
        {
            if (row.AdditionalValue <= 0)
            {
                row.AdditionalValue += 1f;
            }
        }
    }

    private static void FixHarmonyDefenseOption(GameConfiguration gameConfiguration)
    {
        var harmonyOption = gameConfiguration.ItemOptions
            .FirstOrDefault(io => io.Name == HarmonyOptions.DefenseOptionsName)
            ?.PossibleOptions.FirstOrDefault(o => o.Number == 7);
        if (harmonyOption is null)
        {
            return;
        }

        var options = harmonyOption.LevelDependentOptions.OrderBy(o => o.RequiredItemLevel).ToList();
        float[] values = [0.97f, 0.96f, 0.95f, 0.94f, 0.93f];
        for (int i = 0; i < values.Length; i++)
        {
            var option = options[i];
            var boost = option.PowerUpDefinition?.Boost;
            if (boost is null)
            {
                continue;
            }

            boost.ConstantValue.Value = values[i];
            boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
        }
    }

    private static void FixWings(GameConfiguration gameConfiguration, short group, short number)
    {
        var wings = gameConfiguration.Items.FirstOrDefault(item => item.Group == group && item.Number == number);
        if (wings is null)
        {
            return;
        }

        var damageDecrease = wings.BasePowerUpAttributes
            .FirstOrDefault(a => a.TargetAttribute == Stats.DamageReceiveDecrement && a.AggregateType != AggregateType.Multiplicate);
        if (damageDecrease is not null)
        {
            damageDecrease.BaseValue += 1f;
            damageDecrease.AggregateType = AggregateType.Multiplicate;
        }

        var damageIncrease = wings.BasePowerUpAttributes
            .FirstOrDefault(a => a.TargetAttribute == Stats.AttackDamageIncrease && a.AggregateType != AggregateType.Multiplicate);
        if (damageIncrease is not null)
        {
            damageIncrease.BaseValue += 1f;
            damageIncrease.AggregateType = AggregateType.Multiplicate;
        }
    }

    private static void FixGuardianAngel(GameConfiguration gameConfiguration)
    {
        var angel = gameConfiguration.Items.FirstOrDefault(item => item is { Group: 13, Number: 0 });
        if (angel is null)
        {
            return;
        }

        var damageDecrease = angel.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.DamageReceiveDecrement);
        if (damageDecrease is not null)
        {
            damageDecrease.BaseValue = 0.8f;
            damageDecrease.AggregateType = AggregateType.Multiplicate;
        }
    }

    private static void FixImp(GameConfiguration gameConfiguration)
    {
        var imp = gameConfiguration.Items.FirstOrDefault(item => item is { Group: 13, Number: 1 });
        if (imp is null)
        {
            return;
        }

        var damageIncrease = imp.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.AttackDamageIncrease);
        if (damageIncrease is not null)
        {
            damageIncrease.BaseValue = 1.3f;
            damageIncrease.AggregateType = AggregateType.Multiplicate;
        }
    }

    private static void FixDemon(GameConfiguration gameConfiguration)
    {
        var demon = gameConfiguration.Items.FirstOrDefault(item => item is { Group: 13, Number: 64 });
        if (demon is null)
        {
            return;
        }

        var damageIncrease = demon.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.AttackDamageIncrease);
        if (damageIncrease is not null)
        {
            damageIncrease.BaseValue = 1.4f;
            damageIncrease.AggregateType = AggregateType.Multiplicate;
        }
    }

    private static void FixSpiritOfGuardian(GameConfiguration gameConfiguration)
    {
        var spiritOfGuardian = gameConfiguration.Items.FirstOrDefault(item => item is { Group: 13, Number: 65 });
        if (spiritOfGuardian is null)
        {
            return;
        }

        var damageDecrease = spiritOfGuardian.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.DamageReceiveDecrement);
        if (damageDecrease is not null)
        {
            damageDecrease.BaseValue = 0.7f;
            damageDecrease.AggregateType = AggregateType.Multiplicate;
        }
    }

    private static void FixDinorant(GameConfiguration gameConfiguration)
    {
        var dinorant = gameConfiguration.Items.FirstOrDefault(item => item is { Group: 13, Number: 2 });
        if (dinorant is null)
        {
            return;
        }

        var damageDecrease = dinorant.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.DamageReceiveDecrement);
        if (damageDecrease is not null)
        {
            damageDecrease.BaseValue = 0.9f;
            damageDecrease.AggregateType = AggregateType.Multiplicate;
        }

        var damageIncrease = dinorant.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.AttackDamageIncrease);
        if (damageIncrease is not null)
        {
            damageIncrease.BaseValue = 1.15f;
            damageIncrease.AggregateType = AggregateType.Multiplicate;
        }
    }

    private static void FixPanda(GameConfiguration gameConfiguration)
    {
        var panda = gameConfiguration.Items.FirstOrDefault(item => item is { Group: 13, Number: 80 });
        if (panda is null)
        {
            return;
        }

        var exp = panda.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.ExperienceRate);
        if (exp is not null)
        {
            exp.BaseValue = 1.5f;
            exp.AggregateType = AggregateType.Multiplicate;
        }

        var masterExp = panda.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.MasterExperienceRate);
        if (masterExp is not null)
        {
            masterExp.BaseValue = 1.5f;
            masterExp.AggregateType = AggregateType.Multiplicate;
        }
    }

    private static void FixUnicorn(GameConfiguration gameConfiguration)
    {
        var panda = gameConfiguration.Items.FirstOrDefault(item => item is { Group: 13, Number: 106 });
        if (panda is null)
        {
            return;
        }

        var moneyRate = panda.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.MoneyAmountRate);
        if (moneyRate is not null)
        {
            moneyRate.BaseValue = 1.5f;
            moneyRate.AggregateType = AggregateType.Multiplicate;
        }
    }

    private static void FixSkeleton(GameConfiguration gameConfiguration, IContext context)
    {
        var skeleton = gameConfiguration.Items.FirstOrDefault(item => item is { Group: 13, Number: 123 });
        if (skeleton is null)
        {
            return;
        }

        var damageIncrease = skeleton.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.AttackDamageIncrease);
        if (damageIncrease is not null)
        {
            damageIncrease.BaseValue = 1.2f;
            damageIncrease.AggregateType = AggregateType.Multiplicate;
        }

        var exp = skeleton.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.ExperienceRate);
        if (exp is not null)
        {
            exp.BaseValue = 1.3f;
            exp.AggregateType = AggregateType.Multiplicate;
        }

        var masterExp = skeleton.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.MasterExperienceRate);
        if (masterExp is null)
        {
            masterExp = context.CreateNew<ItemBasePowerUpDefinition>();
            masterExp.TargetAttribute = Stats.MasterExperienceRate.GetPersistent(gameConfiguration);
            skeleton.BasePowerUpAttributes.Add(masterExp);
        }

        masterExp.BaseValue = 1.3f;
        masterExp.AggregateType = AggregateType.Multiplicate;
    }

    private static void FixSoulBarrierEffect(GameConfiguration gameConfiguration)
    {
        var soulBarrierEffect = gameConfiguration.MagicEffects.FirstOrDefault(effect => effect.Number == (short)MagicEffectNumber.SoulBarrier);
        if (soulBarrierEffect is null)
        {
            return;
        }

        var powerUp = soulBarrierEffect.PowerUpDefinitions.FirstOrDefault();
        if (powerUp is null)
        {
            return;
        }

        var boost = powerUp.Boost;
        if (boost is null)
        {
            return;
        }

        boost.ConstantValue.Value = 0.90f;
        boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        var boostPerEnergy = boost.RelatedValues.FirstOrDefault(value => value.InputAttribute == Stats.TotalEnergy);
        if (boostPerEnergy is not null)
        {
            boostPerEnergy.InputOperand = 1 - (0.01f / 200f);
            boostPerEnergy.InputOperator = InputOperator.ExponentiateByAttribute;
        }

        var boostPerAgility = boost.RelatedValues.FirstOrDefault(value => value.InputAttribute == Stats.TotalAgility);
        if (boostPerAgility is not null)
        {
            boostPerAgility.InputOperand = 1 - (0.01f / 50f);
            boostPerAgility.InputOperator = InputOperator.ExponentiateByAttribute;
        }
    }
}