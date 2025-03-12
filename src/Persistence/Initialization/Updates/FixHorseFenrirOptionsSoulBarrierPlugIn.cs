// <copyright file="FixHorseFenrirOptionsSoulBarrierPlugIn.cs" company="MUnique">
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
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds Dark Horse options, fixes Gold Fenrir options and Soul Barrier effects.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("3E362629-AAF3-40E0-BC6D-32230285FB03")]
public class FixHorseFenrirOptionsSoulBarrierPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Dark Horse and Gold Fenrir Options, and Soul Barrier Effects";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds Dark Horse options, fixes Gold Fenrir options and Soul Barrier effects.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 03, 12, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixHorseFenrirOptionsSoulBarrierPlugIn;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Update dark horse item crafting
        var petTrainerCraftings = gameConfiguration.Monsters.Single(m => m.NpcWindow == NpcWindow.PetTrainer).ItemCraftings;
        if (petTrainerCraftings.Single(c => c.Number == 13) is { } darkHorseCrafting)
        {
            darkHorseCrafting.ItemCraftingHandlerClassName = typeof(DarkHorseCrafting).FullName!;
        }

        // Add new Stats
        var totalLevel = context.CreateNew<AttributeDefinition>(Stats.TotalLevel.Id, Stats.TotalLevel.Designation, Stats.TotalLevel.Description);
        gameConfiguration.Attributes.Add(totalLevel);
        var dmgReceiveHorseDec = context.CreateNew<AttributeDefinition>(Stats.DamageReceiveHorseDecrement.Id, Stats.DamageReceiveHorseDecrement.Designation, Stats.DamageReceiveHorseDecrement.Description);
        gameConfiguration.Attributes.Add(dmgReceiveHorseDec);
        var soulBarrierReceiveDec = context.CreateNew<AttributeDefinition>(Stats.SoulBarrierReceiveDecrement.Id, Stats.SoulBarrierReceiveDecrement.Designation, Stats.SoulBarrierReceiveDecrement.Description);
        gameConfiguration.Attributes.Add(soulBarrierReceiveDec);
        var soulBarrierManaTollPerHit = context.CreateNew<AttributeDefinition>(Stats.SoulBarrierManaTollPerHit.Id, Stats.SoulBarrierManaTollPerHit.Designation, Stats.SoulBarrierManaTollPerHit.Description);
        gameConfiguration.Attributes.Add(soulBarrierManaTollPerHit);

        // Add total level relationships class attributes
        var level = Stats.Level.GetPersistent(gameConfiguration);
        var masterLevel = Stats.MasterLevel.GetPersistent(gameConfiguration);
        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            charClass.AttributeCombinations.Add(context.CreateNew<AttributeRelationship>(
                totalLevel,
                1,
                level,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw));

            charClass.AttributeCombinations.Add(context.CreateNew<AttributeRelationship>(
                totalLevel,
                1,
                masterLevel,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw));
        });

        // Add DL horse dmg receive base value and relationship to dmg receive
        var lordClasses = gameConfiguration.CharacterClasses.Where(c => c.Number == 16 || c.Number == 17);
        var damageReceiveDec = Stats.DamageReceiveDecrement.GetPersistent(gameConfiguration);

        foreach (var lordClass in lordClasses)
        {
            lordClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(
                1,
                dmgReceiveHorseDec));

            lordClass.AttributeCombinations.Add(context.CreateNew<AttributeRelationship>(
                damageReceiveDec,
                1,
                dmgReceiveHorseDec,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.Multiplicate));
        }

        // Add new dark horse option type
        var darkHorseOptionType = context.CreateNew<ItemOptionType>();
        darkHorseOptionType.Description = ItemOptionTypes.DarkHorse.Description;
        darkHorseOptionType.Id = ItemOptionTypes.DarkHorse.Id;
        darkHorseOptionType.Name = ItemOptionTypes.DarkHorse.Name;
        darkHorseOptionType.IsVisible = ItemOptionTypes.DarkHorse.IsVisible;
        gameConfiguration.ItemOptionTypes.Add(darkHorseOptionType);

        // Add dark horse options
        var horse = gameConfiguration.Items.Single(i => i.Group == 13 && i.Number == 4);
        var horseOptionDefinition = context.CreateNew<ItemOptionDefinition>();
        horseOptionDefinition.SetGuid(ItemOptionDefinitionNumbers.Horse);
        gameConfiguration.ItemOptions.Add(horseOptionDefinition);
        horseOptionDefinition.Name = "Dark Horse Options";
        horseOptionDefinition.PossibleOptions.Add(this.CreateRelatedPetOption(context, gameConfiguration, ItemOptionTypes.DarkHorse, 1, Stats.DamageReceiveHorseDecrement, AggregateType.AddRaw, ItemOptionDefinitionNumbers.Horse, -0.15f, (Stats.HorseLevel, -0.005f)));
        horseOptionDefinition.PossibleOptions.Add(this.CreateRelatedPetOption(context, gameConfiguration, ItemOptionTypes.DarkHorse, 2, Stats.DefenseBase, AggregateType.AddRaw, ItemOptionDefinitionNumbers.Horse, 5, (Stats.HorseLevel, 2), (Stats.TotalAgility, 1f / 20)));
        horse.PossibleItemOptions.Add(horseOptionDefinition);

        // Update gold fenrir options
        var fenrirOptionDef = gameConfiguration.ItemOptions.First(o => o.PossibleOptions.Any(opt => opt.OptionType == ItemOptionTypes.GoldFenrir));
        var goldFenrirOptions = fenrirOptionDef.PossibleOptions.Where(opt => opt.OptionType == ItemOptionTypes.GoldFenrir).ToList();
        foreach (var goldFenrirOpt in goldFenrirOptions)
        {
            AttributeDefinition? targetAttribute = null;
            float multiplier = 0;
            if (goldFenrirOpt.PowerUpDefinition!.TargetAttribute == Stats.MaximumHealth)
            {
                targetAttribute = Stats.MaximumHealth;
                multiplier = 0.5f;
            }
            else if (goldFenrirOpt.PowerUpDefinition.TargetAttribute == Stats.MaximumMana)
            {
                targetAttribute = Stats.MaximumMana;
                multiplier = 0.5f;
            }
            else if (goldFenrirOpt.PowerUpDefinition.TargetAttribute == Stats.MaximumPhysBaseDmg)
            {
                targetAttribute = Stats.PhysicalBaseDmg;
                multiplier = 1f / 12f;
            }
            else
            {
                targetAttribute = Stats.WizardryBaseDmg;
                multiplier = 1f / 25f;
            }

            goldFenrirOpt.PowerUpDefinition.TargetAttribute = targetAttribute.GetPersistent(gameConfiguration);
            goldFenrirOpt.PowerUpDefinition.Boost!.ConstantValue.Value = 0;
            goldFenrirOpt.PowerUpDefinition.Boost.RelatedValues.Add(
                this.CreateAttributeRelationship(context, gameConfiguration, targetAttribute, ItemOptionDefinitionNumbers.Fenrir, (Stats.TotalLevel, multiplier)));
        }

        // Update soul barrier magic effect. Soul barrier dmg decrease % = 10 + (Agility/50) + (Energy/200)
        var soulBarrierMagicEffect = gameConfiguration.MagicEffects.FirstOrDefault(me => me.Number == (short)MagicEffectNumber.SoulBarrier);
        if (soulBarrierMagicEffect is not null)
        {
            if (soulBarrierMagicEffect.Duration is { } duration)
            {
                duration.RelatedValues.First().InputOperand = 1f / 40f;
            }

            if (soulBarrierMagicEffect.PowerUpDefinitions.FirstOrDefault() is { } powerUp)
            {
                powerUp.TargetAttribute = soulBarrierReceiveDec;
                powerUp.Boost!.ConstantValue.Value = 0.1f;
                powerUp.Boost!.ConstantValue.AggregateType = AggregateType.AddRaw;

                var boostPerEnergy = powerUp.Boost.RelatedValues.First(v => v.InputOperand == 1 - (0.01f / 200f));
                boostPerEnergy.InputOperator = InputOperator.Multiply;
                boostPerEnergy.InputOperand = 1f / 20000f;

                var boostPerAgility = powerUp.Boost.RelatedValues.First(v => v.InputOperand == 1 - (0.01f / 50f));
                boostPerAgility.InputOperator = InputOperator.Multiply;
                boostPerAgility.InputOperand = 1f / 5000f;
            }

            var manaTollPerHit = context.CreateNew<PowerUpDefinition>();
            soulBarrierMagicEffect.PowerUpDefinitions.Add(manaTollPerHit);
            manaTollPerHit.TargetAttribute = soulBarrierManaTollPerHit;
            manaTollPerHit.Boost = context.CreateNew<PowerUpDefinitionValue>();

            var manaToll = context.CreateNew<AttributeRelationship>();
            manaToll.InputAttribute = Stats.MaximumMana.GetPersistent(gameConfiguration);
            manaToll.InputOperator = InputOperator.Multiply;
            manaToll.InputOperand = 0.02f; // two percent of total mana
            manaTollPerHit.Boost.RelatedValues.Add(manaToll);
        }

        // Update Soul Barrier Streng master skill definition
        var soulBarrierStreng = gameConfiguration.Skills.First(s => s.GetId() == new Guid("00000400-0193-0000-0000-000000000000"));
        soulBarrierStreng.MasterDefinition!.ValueFormula = $"{soulBarrierStreng.MasterDefinition.ValueFormula} / 100";
        soulBarrierStreng.MasterDefinition.TargetAttribute = soulBarrierReceiveDec;
        soulBarrierStreng.MasterDefinition.Aggregation = AggregateType.AddRaw;

        // Update Soul Barrier Profic master skill definition
        var soulBarrierProfic = gameConfiguration.Skills.First(s => s.GetId() == new Guid("00000400-0194-0000-0000-000000000000"));
        soulBarrierProfic.MasterDefinition!.ExtendsDuration = true;
        soulBarrierProfic.MasterDefinition.ReplacedSkill = soulBarrierStreng;
        soulBarrierProfic.AttackDamage = soulBarrierStreng.AttackDamage;
        soulBarrierProfic.DamageType = soulBarrierStreng.DamageType;
        soulBarrierProfic.ElementalModifierTarget = soulBarrierStreng.ElementalModifierTarget;
        soulBarrierProfic.ImplicitTargetRange = soulBarrierStreng.ImplicitTargetRange;
        soulBarrierProfic.MovesTarget = soulBarrierStreng.MovesTarget;
        soulBarrierProfic.MovesToTarget = soulBarrierStreng.MovesToTarget;
        soulBarrierProfic.SkillType = soulBarrierStreng.SkillType;
        soulBarrierProfic.Target = soulBarrierStreng.Target;
        soulBarrierProfic.TargetRestriction = soulBarrierStreng.TargetRestriction;
        soulBarrierProfic.MagicEffectDef = soulBarrierStreng.MagicEffectDef;
    }

    private IncreasableItemOption CreateRelatedPetOption(IContext context, GameConfiguration gameConfiguration, ItemOptionType optionType, int number, AttributeDefinition targetAttribute, AggregateType aggregateType, short optionNumber, float baseValue = 0, params (AttributeDefinition SourceAttribute, float Multiplier)[] relatedAttributes)
    {
        var itemOption = context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(optionNumber, targetAttribute.Id.ExtractFirstTwoBytes());
        itemOption.OptionType = gameConfiguration.ItemOptionTypes.First(t => t == optionType);
        itemOption.Number = number;
        itemOption.PowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        itemOption.PowerUpDefinition.TargetAttribute = targetAttribute.GetPersistent(gameConfiguration);
        itemOption.PowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        itemOption.PowerUpDefinition.Boost.ConstantValue.Value = baseValue;
        itemOption.PowerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;

        for (int i = 0; i < relatedAttributes.Length; i++)
        {
            itemOption.PowerUpDefinition.Boost.RelatedValues.Add(this.CreateAttributeRelationship(context, gameConfiguration, targetAttribute, optionNumber, relatedAttributes[i], i));
        }

        return itemOption;
    }

    private AttributeRelationship CreateAttributeRelationship(IContext context, GameConfiguration gameConfiguration, AttributeDefinition targetAttribute, short optionNumber, (AttributeDefinition SourceAttribute, float Multiplier) relatedAttribute, int i = 0)
    {
        var attributeRelationship = context.CreateNew<AttributeRelationship>();
        attributeRelationship.SetGuid(optionNumber, targetAttribute.Id.ExtractFirstTwoBytes(), (byte)i);
        attributeRelationship.InputAttribute = relatedAttribute.SourceAttribute.GetPersistent(gameConfiguration);
        attributeRelationship.InputOperator = InputOperator.Multiply;
        attributeRelationship.InputOperand = relatedAttribute.Multiplier;
        return attributeRelationship;
    }
}