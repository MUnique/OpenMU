// <copyright file="FixWeaponRisePercentagePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.PlugIns;
using static MUnique.OpenMU.Persistence.Initialization.CharacterClasses.CharacterClassHelper;

/// <summary>
/// This update fixes weapons (staff, stick, book, scepter) rise percentage increase; Summoner weapons and wings wizardry/curse options; and Wing of Dimension (inc/dec), Cape of Overrule (inc/dec), Cape of Emperor (dec) damage rates..
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("58740F26-6496-4CCA-8C90-C4749E09DDB2")]
public class FixWeaponRisePercentagePlugInSeason6 : FixWeaponRisePercentagePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    private new const string PlugInName = "Fix Weapon Rise Percentage, Summoner Items Wizardry/Curse Options, and Several 3rd Level Wing Damage Rates";

    /// <summary>
    /// The plug in description.
    /// </summary>
    private new const string PlugInDescription = "This update fixes weapons (staff, stick, book, scepter) rise percentage; Summoner weapons and wings wizardry/curse options; and Wing of Dimension (inc/dec), Cape of Overrule (inc/dec), Cape of Emperor (dec) damage rates. Also includes fixes for Divine Staff of Archangel and Eternal Wing Stick.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

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
        var scepters = gameConfiguration.Items.Where(i => i.Group == (int)ItemGroups.Scepters && i.BasePowerUpAttributes.Any(pua => pua.TargetAttribute == Stats.ScepterRise));
        foreach (var scepter in scepters)
        {
            if (scepter.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.ScepterRise) is { } scepterRiseAttr)
            {
                if ((int)scepterRiseAttr.BaseValue % 2 != 0)
                {
                    scepterRiseAttr.BonusPerLevelTable = scepterOddTable;
                }

                scepterRiseAttr.BaseValue /= 2.0f;
            }
        }

        // Fix Group 5 weapons (Skull & Divine Staves, sticks, and books)
        var weaponsG5 = gameConfiguration.Items.Where(i => i.Group == 5);
        var summonerWeapons = weaponsG5.Where(i => i.PossibleItemOptions.Contains(gameConfiguration.ItemOptions.First(io => io.Name == ExcellentOptions.CurseAttackOptionsName))); // Skull Staff included at this point
        var staffEvenTable = gameConfiguration.ItemLevelBonusTables.Single(bt => bt.Name == "Staff Rise (even)");
        var staffOddTable = gameConfiguration.ItemLevelBonusTables.Single(bt => bt.Name == "Staff Rise (odd)");

        // -> fix Skull Staff
        if (weaponsG5.FirstOrDefault(e => e.Number == 0) is { } skullStaff)
        {
            skullStaff.PossibleItemOptions.Clear();
            skullStaff.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(io => io.Name == ExcellentOptions.WizardryAttackOptionsName));
            skullStaff.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(io => io.Name == HarmonyOptions.WizardryAttackOptionsName));
            skullStaff.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(io => io.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Luck)));
            skullStaff.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(io => io.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.MaximumWizBaseDmg)));

            var powerUpDefinition = context.CreateNew<ItemBasePowerUpDefinition>();
            powerUpDefinition.TargetAttribute = Stats.StaffRise.GetPersistent(gameConfiguration);
            powerUpDefinition.BaseValue = 6 / 2.0f;
            powerUpDefinition.AggregateType = AggregateType.AddRaw;
            powerUpDefinition.BonusPerLevelTable = staffEvenTable;
            skullStaff.BasePowerUpAttributes.Add(powerUpDefinition);
        }

        // -> fix Divine Staff of Archangel
        if (weaponsG5.FirstOrDefault(e => e.Number == 10) is { } divineStaff)
        {
            var basePowerUps = divineStaff.BasePowerUpAttributes;
            if (basePowerUps.FirstOrDefault(pu => pu.TargetAttribute == Stats.MinimumPhysBaseDmgByWeapon) is { } minPhysDmgAttr)
            {
                minPhysDmgAttr.BaseValue = 153;
            }

            if (basePowerUps.FirstOrDefault(pu => pu.TargetAttribute == Stats.MaximumPhysBaseDmgByWeapon) is { } maxPhysDmgAttr)
            {
                maxPhysDmgAttr.BaseValue = 165;
            }

            if (basePowerUps.FirstOrDefault(pu => pu.TargetAttribute == Stats.AttackSpeedByWeapon) is { } attackSpeedAttr)
            {
                attackSpeedAttr.BaseValue = 30;
            }

            if (basePowerUps.FirstOrDefault(pu => pu.TargetAttribute == Stats.StaffRise) is { } staffRiseAttr)
            {
                staffRiseAttr.BaseValue = 156 / 2.0f;
            }
        }

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
            stick.PossibleItemOptions.Clear();
            stick.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(io => io.Name == ExcellentOptions.WizardryAttackOptionsName));
            stick.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(io => io.Name == HarmonyOptions.WizardryAttackOptionsName));
            stick.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(io => io.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Luck)));
            stick.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(io => io.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.MaximumWizBaseDmg)));
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

        // -> fix Eternal Wing Stick
        if (weaponsG5.FirstOrDefault(e => e.Number == 20) is { } eternalStick)
        {
            eternalStick.DropLevel = 147;
            var basePowerUps = eternalStick.BasePowerUpAttributes;
            if (basePowerUps.FirstOrDefault(pu => pu.TargetAttribute == Stats.MinimumPhysBaseDmgByWeapon) is { } minPhysDmgAttr)
            {
                minPhysDmgAttr.BaseValue = 66;
            }

            if (basePowerUps.FirstOrDefault(pu => pu.TargetAttribute == Stats.MaximumPhysBaseDmgByWeapon) is { } maxPhysDmgAttr)
            {
                maxPhysDmgAttr.BaseValue = 74;
            }

            if (basePowerUps.FirstOrDefault(pu => pu.TargetAttribute == Stats.AttackSpeedByWeapon) is { } attackSpeedAttr)
            {
                attackSpeedAttr.BaseValue = 30;
            }
        }

        // -> fix books
        if (gameConfiguration.CharacterClasses.FirstOrDefault(cc => cc.Number == (int)CharacterClassNumber.Summoner) is { } summoner)
        {
            var bookRiseAttr = context.CreateNew<AttributeDefinition>(Stats.BookRise.Id, Stats.BookRise.Designation, Stats.BookRise.Description);
            gameConfiguration.Attributes.Add(bookRiseAttr);
            summoner.AttributeCombinations.Add(CreateAttributeRelationship(context, gameConfiguration, Stats.CurseAttackDamageIncrease, 1.0f / 100, Stats.BookRise));
        }

        Dictionary<int, int> booksMagicPower = new() { [21] = 46, [22] = 59, [23] = 72 };
        foreach (var book in books)
        {
            book.PossibleItemOptions.Clear();
            book.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(io => io.Name == ExcellentOptions.WizardryAttackOptionsName));
            book.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(io => io.Name == HarmonyOptions.WizardryAttackOptionsName));
            book.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(io => io.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Luck)));
            book.PossibleItemOptions.Add(gameConfiguration.ItemOptions.First(io => io.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.MaximumCurseBaseDmg)));
            if (booksMagicPower.ContainsKey(book.Number))
            {
                var powerUpDefinition = context.CreateNew<ItemBasePowerUpDefinition>();
                powerUpDefinition.TargetAttribute = Stats.BookRise.GetPersistent(gameConfiguration);
                powerUpDefinition.BaseValue = booksMagicPower[book.Number] / 2.0f;
                powerUpDefinition.AggregateType = AggregateType.AddRaw;
                powerUpDefinition.BonusPerLevelTable = booksMagicPower[book.Number] % 2 == 0 ? staffEvenTable : staffOddTable;
                book.BasePowerUpAttributes.Add(powerUpDefinition);
            }

            if (book.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.IsStickEquipped) is { } stickEquipAttr)
            {
                book.BasePowerUpAttributes.Remove(stickEquipAttr);
            }
        }

        // Fix Wings of Curse options
        var wingsOfCurse = gameConfiguration.Items.FirstOrDefault(i => i.GetId() == new Guid("00000080-000c-0029-0000-000000000000"));
        if (wingsOfCurse is not null)
        {
            wingsOfCurse.Name = "Wings of Curse";
            var wingOpts = wingsOfCurse.PossibleItemOptions.First(o => o.Name == "Wing of Curse Options");

            var wizOption = wingOpts.PossibleOptions.First();
            wizOption.PowerUpDefinition = this.CreatePowerUpDefinition(Stats.MaximumWizBaseDmg, 0, AggregateType.AddRaw, context, gameConfiguration);
            wizOption.LevelDependentOptions.Clear();
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
        }

        // Fix Wings of Despair options
        var wingsOfDespair = gameConfiguration.Items.FirstOrDefault(i => i.GetId() == new Guid("00000080-000c-002a-0000-000000000000"));
        if (wingsOfDespair is not null)
        {
            var wingOpts = wingsOfDespair.PossibleItemOptions.First(o => o.Name == "Wings of Despair Options");

            var curseOption = wingOpts.PossibleOptions.First(o => o.Number == 0);
            curseOption.PowerUpDefinition = this.CreatePowerUpDefinition(Stats.MaximumCurseBaseDmg, 0, AggregateType.AddRaw, context, gameConfiguration);
            curseOption.LevelDependentOptions.Clear();
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

            var wizOption = wingOpts.PossibleOptions.First(o => o.Number == 2);
            wizOption.PowerUpDefinition = this.CreatePowerUpDefinition(Stats.MaximumWizBaseDmg, 0, AggregateType.AddRaw, context, gameConfiguration);
            wizOption.LevelDependentOptions.Clear();
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
        }

        // Fix Wing of Dimension options
        var wingsOfDimension = gameConfiguration.Items.FirstOrDefault(i => i.GetId() == new Guid("00000080-000c-002b-0000-000000000000"));
        if (wingsOfDimension is not null)
        {
            var wingOpts = wingsOfDimension.PossibleItemOptions.First(o => o.Name == "Wing of Dimension Options");

            var wizOption = wingOpts.PossibleOptions.First(o => o.Number == 3);
            wizOption.PowerUpDefinition = this.CreatePowerUpDefinition(Stats.MaximumWizBaseDmg, 0, AggregateType.AddRaw, context, gameConfiguration);
            wizOption.LevelDependentOptions.Clear();
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

            var curseOption = wingOpts.PossibleOptions.First(o => o.Number == 2);
            curseOption.PowerUpDefinition = this.CreatePowerUpDefinition(Stats.MaximumCurseBaseDmg, 0, AggregateType.AddRaw, context, gameConfiguration);
            curseOption.LevelDependentOptions.Clear();
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
        if (capeOfEmperor is not null)
        {
            if (capeOfEmperor.BasePowerUpAttributes.FirstOrDefault(pua => pua.TargetAttribute == Stats.DamageReceiveDecrement) is { } dmgDec)
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