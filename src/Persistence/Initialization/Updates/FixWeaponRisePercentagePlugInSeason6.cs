// <copyright file="FixWeaponRisePercentagePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes weapons (staff, stick, book, scepter) rise percentage increase; Summoner wings wizardry/curse options; and Wing of Dimension (inc/dec), Cape of Overrule (inc/dec), Cape of Emperor (dec) damage rates..
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("58740F26-6496-4CCA-8C90-C4749E09DDB2")]
public class FixWeaponRisePercentagePlugInSeason6 : FixWeaponRisePercentagePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixWeaponRisePercentageSeason6;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);

        var scepterRiseBonusTable = gameConfiguration.ItemLevelBonusTables.Single(bt => bt.Name == "Scepter Rise");

        // Modify existing table name and description
        scepterRiseBonusTable.Name = "Scepter Rise (even)";
        scepterRiseBonusTable.Description = "The scepter rise bonus per item level for even magic power scepters.";

        // Add new scepter odd increase table
        float[] scepterRiseIncreaseByLevelOdd = { 0, 2, 3, 5, 6, 8, 9, 11, 12, 14, 16, 18, 21, 25, 29, 33 };

        var scepterOddTable = context.CreateNew<ItemLevelBonusTable>();
        gameConfiguration.ItemLevelBonusTables.Add(scepterOddTable);
        scepterOddTable.Name = "Scepter Rise (odd)";
        scepterOddTable.Description = "The scepter rise bonus per item level for odd magic power scepters.";
        for (int level = 0; level < scepterRiseIncreaseByLevelOdd.Length; level++)
        {
            var value = scepterRiseIncreaseByLevelOdd[level];
            if (value != 0)
            {
                var levelBonus = context.CreateNew<LevelBonus>();
                levelBonus.Level = level;
                levelBonus.AdditionalValue = scepterRiseIncreaseByLevelOdd[level];
                scepterOddTable.BonusPerLevel.Add(levelBonus);
            }
        }

        // Fix scepters
        var scepters = gameConfiguration.Items.Where(i => i.Group == 2 && i.BasePowerUpAttributes.Any(pua => pua.TargetAttribute == Stats.ScepterRise));

        foreach (var scepter in scepters)
        {
            if (scepter.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.ScepterRise) is { } scepterRiseAttr)
            {
                if (scepterRiseAttr.BaseValue % 2 != 0)
                {
                    scepterRiseAttr.BonusPerLevelTable = scepterOddTable;
                }

                scepterRiseAttr.BaseValue /= 2;
            }
        }

        // Fix Group 5 weapons (Skull Staff, sticks and books)
        var weaponsG5 = gameConfiguration.Items.Where(i => i.Group == 5);
        var summonerWeapons = weaponsG5.Where(i => i.PossibleItemOptions.Contains(gameConfiguration.ItemOptions.Single(o => o.Name == ExcellentOptions.CurseAttackOptionsName))); // Skull Staff included
        var staffEvenTable = gameConfiguration.ItemLevelBonusTables.Single(bt => bt.Name == "Staff Rise (even)");
        var staffOddTable = gameConfiguration.ItemLevelBonusTables.Single(bt => bt.Name == "Staff Rise (odd)");

        // -> fix Skull Staff
        if (weaponsG5.FirstOrDefault(e => e.Number == 0) is { } skullStaff)
        {
            skullStaff.PossibleItemOptions.Clear();
            skullStaff.PossibleItemOptions.Add(gameConfiguration.ItemOptions.Single(o => o.Name == ExcellentOptions.WizardryAttackOptionsName));
            skullStaff.PossibleItemOptions.Add(gameConfiguration.ItemOptions.Single(o => o.Name == HarmonyOptions.WizardryAttackOptionsName));
            skullStaff.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.MaximumWizBaseDmg)));

            var powerUpDefinition = context.CreateNew<ItemBasePowerUpDefinition>();
            powerUpDefinition.TargetAttribute = Stats.StaffRise.GetPersistent(gameConfiguration);
            powerUpDefinition.BaseValue = 6 / 2.0f;
            powerUpDefinition.AggregateType = AggregateType.AddRaw;
            powerUpDefinition.BonusPerLevelTable = staffEvenTable;
            skullStaff.BasePowerUpAttributes.Add(powerUpDefinition);
        }

        // -> fix possible options for all Summoner sticks and books
        summonerWeapons.ForEach(e =>
        {
            e.PossibleItemOptions.Clear();
            e.PossibleItemOptions.Add(gameConfiguration.ItemOptions.Single(o => o.Name == ExcellentOptions.WizardryAttackOptionsName));
            e.PossibleItemOptions.Add(gameConfiguration.ItemOptions.Single(o => o.Name == HarmonyOptions.WizardryAttackOptionsName));
        });

        var rhOnlySlotType = gameConfiguration.ItemSlotTypes.First(t => !t.ItemSlots.Contains(0) && t.ItemSlots.Contains(1));
        var books = summonerWeapons.Where(e => e.ItemSlot == rhOnlySlotType);
        var sticks = summonerWeapons.Except(books);

        // -> fix sticks
        Dictionary<int, int> sticksMagicPower = new()
        {
            [14] = 34,
            [15] = 46,
            [16] = 59,
            [17] = 76,
            [18] = 92,
            [19] = 110,
            [20] = 106,
            [34] = 130,
            [36] = 146,
        };
        foreach (var stick in sticks)
        {
            stick.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.MaximumWizBaseDmg)));
            if (sticksMagicPower.ContainsKey(stick.Number))
            {
                var powerUpDefinition = context.CreateNew<ItemBasePowerUpDefinition>();
                powerUpDefinition.TargetAttribute = Stats.StaffRise.GetPersistent(gameConfiguration);
                powerUpDefinition.BaseValue = sticksMagicPower[stick.Number] / 2.0f;
                powerUpDefinition.AggregateType = AggregateType.AddRaw;
                powerUpDefinition.BonusPerLevelTable = sticksMagicPower[stick.Number] % 2 == 0 ? staffEvenTable : staffOddTable;
                stick.BasePowerUpAttributes.Add(powerUpDefinition);
            }
        }

        // -> fix books
        Dictionary<int, int> booksMagicPower = new() { [21] = 46, [22] = 59, [23] = 72 };
        foreach (var book in books)
        {
            book.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.MaximumCurseBaseDmg)));
            if (booksMagicPower.ContainsKey(book.Number))
            {
                var powerUpDefinition = context.CreateNew<ItemBasePowerUpDefinition>();
                powerUpDefinition.TargetAttribute = Stats.StaffRise.GetPersistent(gameConfiguration);
                powerUpDefinition.BaseValue = booksMagicPower[book.Number] / 2.0f;
                powerUpDefinition.AggregateType = AggregateType.AddRaw;
                powerUpDefinition.BonusPerLevelTable = booksMagicPower[book.Number] % 2 == 0 ? staffEvenTable : staffOddTable;
                book.BasePowerUpAttributes.Add(powerUpDefinition);
            }
        }

        // Fix Wings of Curse options
        var wingsOfCurse = gameConfiguration.Items.FirstOrDefault(i => i.GetId() == new Guid("00000080-000c-0029-0000-000000000000"));
        if (wingsOfCurse is not null)
        {
            wingsOfCurse.Name = "Wings of Curse";
            var wingOpts = wingsOfCurse.PossibleItemOptions.FirstOrDefault(o => o.Name == "Wing of Curse Options");
            wingOpts!.PossibleOptions.Clear();

            var itemOption = context.CreateNew<IncreasableItemOption>();
            itemOption.SetGuid(ItemOptionDefinitionNumbers.WingWizardry, Stats.MaximumWizBaseDmg.Id.ExtractFirstTwoBytes());
            itemOption.OptionType = gameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Option);
            itemOption.Number = 0;
            itemOption.PowerUpDefinition = this.CreatePowerUpDefinition(Stats.MaximumWizBaseDmg, 0, AggregateType.AddRaw, context, gameConfiguration);
            for (int level = 1; level <= 4; level++)
            {
                var optionOfLevel = context.CreateNew<ItemOptionOfLevel>();
                optionOfLevel.Level = level;
                optionOfLevel.PowerUpDefinition = this.CreatePowerUpDefinition(
                    itemOption.PowerUpDefinition.TargetAttribute!,
                    level * 4f,
                    AggregateType.AddRaw,
                    context,
                    gameConfiguration);
                itemOption.LevelDependentOptions.Add(optionOfLevel);
            }

            wingOpts?.PossibleOptions.Add(itemOption);
        }

        // Fix Wings of Despair options
        var wingsOfDespair = gameConfiguration.Items.FirstOrDefault(i => i.GetId() == new Guid("00000080-000c-002a-0000-000000000000"));
        if (wingsOfDespair is not null)
        {
            var wingOpts = wingsOfDespair.PossibleItemOptions.FirstOrDefault(o => o.Name == "Wings of Despair Options");
            wingOpts!.PossibleOptions.Clear();

            var curseOption = context.CreateNew<IncreasableItemOption>();
            curseOption.SetGuid(ItemOptionDefinitionNumbers.WingCurse, Stats.MaximumCurseBaseDmg.Id.ExtractFirstTwoBytes());
            curseOption.OptionType = gameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Option);
            curseOption.Number = 0b00;
            curseOption.PowerUpDefinition = this.CreatePowerUpDefinition(Stats.MaximumCurseBaseDmg, 0, AggregateType.AddRaw, context, gameConfiguration);
            for (int level = 1; level <= 4; level++)
            {
                var optionOfLevel = context.CreateNew<ItemOptionOfLevel>();
                optionOfLevel.Level = level;
                optionOfLevel.PowerUpDefinition = this.CreatePowerUpDefinition(
                    curseOption.PowerUpDefinition.TargetAttribute!,
                    level * 4f,
                    AggregateType.AddRaw,
                    context,
                    gameConfiguration);
                curseOption.LevelDependentOptions.Add(optionOfLevel);
            }

            wingOpts?.PossibleOptions.Add(curseOption);

            var wizOption = context.CreateNew<IncreasableItemOption>();
            wizOption.SetGuid(ItemOptionDefinitionNumbers.WingWizardry, Stats.MaximumWizBaseDmg.Id.ExtractFirstTwoBytes());
            wizOption.OptionType = gameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Option);
            wizOption.Number = 0b10;
            wizOption.PowerUpDefinition = this.CreatePowerUpDefinition(Stats.MaximumWizBaseDmg, 0, AggregateType.AddRaw, context, gameConfiguration);
            for (int level = 1; level <= 4; level++)
            {
                var optionOfLevel = context.CreateNew<ItemOptionOfLevel>();
                optionOfLevel.Level = level;
                optionOfLevel.PowerUpDefinition = this.CreatePowerUpDefinition(
                    wizOption.PowerUpDefinition.TargetAttribute!,
                    level * 4f,
                    AggregateType.AddRaw,
                    context,
                    gameConfiguration);
                wizOption.LevelDependentOptions.Add(optionOfLevel);
            }

            wingOpts?.PossibleOptions.Add(wizOption);
        }

        // Fix Wing of Dimension options
        var wingsOfDimension = gameConfiguration.Items.FirstOrDefault(i => i.GetId() == new Guid("00000080-000c-002b-0000-000000000000"));
        if (wingsOfDimension is not null)
        {
            var wingOpts = wingsOfDimension.PossibleItemOptions.FirstOrDefault(o => o.Name == "Wing of Dimension Options");
            var badOpts = wingOpts!.PossibleOptions.Where(opt => opt.PowerUpDefinition?.TargetAttribute != Stats.HealthRecoveryMultiplier);
            foreach (var badOpt in badOpts)
            {
                wingOpts.PossibleOptions.Remove(badOpt);
            }

            var wizOption = context.CreateNew<IncreasableItemOption>();
            wizOption.SetGuid(ItemOptionDefinitionNumbers.WingWizardry, Stats.MaximumWizBaseDmg.Id.ExtractFirstTwoBytes());
            wizOption.OptionType = gameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Option);
            wizOption.Number = 0b11;
            wizOption.PowerUpDefinition = this.CreatePowerUpDefinition(Stats.MaximumWizBaseDmg, 0, AggregateType.AddRaw, context, gameConfiguration);
            for (int level = 1; level <= 4; level++)
            {
                var optionOfLevel = context.CreateNew<ItemOptionOfLevel>();
                optionOfLevel.Level = level;
                optionOfLevel.PowerUpDefinition = this.CreatePowerUpDefinition(
                    wizOption.PowerUpDefinition.TargetAttribute!,
                    level * 4f,
                    AggregateType.AddRaw,
                    context,
                    gameConfiguration);
                wizOption.LevelDependentOptions.Add(optionOfLevel);
            }

            wingOpts?.PossibleOptions.Add(wizOption);

            var curseOption = context.CreateNew<IncreasableItemOption>();
            curseOption.SetGuid(ItemOptionDefinitionNumbers.WingCurse, Stats.MaximumCurseBaseDmg.Id.ExtractFirstTwoBytes());
            curseOption.OptionType = gameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Option);
            curseOption.Number = 0b10;
            curseOption.PowerUpDefinition = this.CreatePowerUpDefinition(Stats.MaximumCurseBaseDmg, 0, AggregateType.AddRaw, context, gameConfiguration);
            for (int level = 1; level <= 4; level++)
            {
                var optionOfLevel = context.CreateNew<ItemOptionOfLevel>();
                optionOfLevel.Level = level;
                optionOfLevel.PowerUpDefinition = this.CreatePowerUpDefinition(
                    curseOption.PowerUpDefinition.TargetAttribute!,
                    level * 4f,
                    AggregateType.AddRaw,
                    context,
                    gameConfiguration);
                curseOption.LevelDependentOptions.Add(optionOfLevel);
            }

            wingOpts?.PossibleOptions.Add(curseOption);

            if (wingsOfDimension.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.AttackDamageIncrease) is { } dmgInc)
            {
                dmgInc.BaseValue = 1f + (39 / 100f);
            }

            if (wingsOfDimension.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.DamageReceiveDecrement) is { } dmgDec)
            {
                dmgDec.BaseValue = 1f - (39 / 100f);
            }
        }

        // Fix Cape of Overrule dmg increase/decrease rate
        var capeOfOverrule = gameConfiguration.Items.FirstOrDefault(i => i.GetId() == new Guid("00000080-000c-0032-0000-000000000000"));
        if (capeOfOverrule is not null)
        {
            if (capeOfOverrule.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.AttackDamageIncrease) is { } dmgInc)
            {
                dmgInc.BaseValue = 1f + (39 / 100f);
            }

            if (capeOfOverrule.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.DamageReceiveDecrement) is { } dmgDec)
            {
                dmgDec.BaseValue = 1f - (39 / 100f);
            }
        }

        // Fix Cape of Emperor dmg decrease rate
        var capeOfEmperor = gameConfiguration.Items.FirstOrDefault(i => i.GetId() == new Guid("00000080-000c-0028-0000-000000000000"));
        if (capeOfOverrule is not null)
        {
            if (capeOfOverrule.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.DamageReceiveDecrement) is { } dmgDec)
            {
                dmgDec.BaseValue = 1f - (24 / 100f);
            }
        }
    }

    private PowerUpDefinition CreatePowerUpDefinition(AttributeDefinition attributeDefinition, float value, AggregateType aggregateType, IContext context, GameConfiguration gameConfiguration)
    {
        var powerUpDefinition = context.CreateNew<PowerUpDefinition>();
        powerUpDefinition.TargetAttribute = attributeDefinition.GetPersistent(gameConfiguration);
        if (value != 0)
        {
            powerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
            powerUpDefinition.Boost.ConstantValue.Value = value;
            powerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;
        }

        return powerUpDefinition;
    }
}