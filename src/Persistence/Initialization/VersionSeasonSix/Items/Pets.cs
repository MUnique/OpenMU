// <copyright file="Pets.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.Network;

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initializer for pets.
/// </summary>
public class Pets : InitializerBase
{
    private const string PetExperienceFormula = "level * level * level * 100 * (level + 10)";

    /// <summary>
    /// Initializes a new instance of the <see cref="Pets"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Pets(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreatePet(0, 0, 1, 1, "Guardian Angel", 23, true, true, (Stats.DamageReceiveDecrement, 0.8f, AggregateType.Multiplicate), (Stats.MaximumHealth, 50f, AggregateType.AddRaw));
        this.CreatePet(1, 0, 1, 1, "Imp", 28, true, true, (Stats.AttackDamageIncrease, 1.3f, AggregateType.Multiplicate));
        this.CreatePet(2, 0, 1, 1, "Horn of Uniria", 25, true, true);
        var dinorant = this.CreatePet(3, SkillNumber.FireBreath, 1, 1, "Horn of Dinorant", 110, false, true, (Stats.DamageReceiveDecrement, 0.9f, AggregateType.Multiplicate), (Stats.AttackDamageIncrease, 1.15f, AggregateType.Multiplicate), (Stats.CanFly, 1.0f, AggregateType.AddRaw));
        this.AddDinorantOptions(dinorant);

        var darkHorse = this.CreatePet(4, SkillNumber.Earthshake, 1, 1, "Dark Horse", 218, false, false, (Stats.IsHorseEquipped, 1, AggregateType.AddRaw));
        this.GameConfiguration.DetermineCharacterClasses(CharacterClasses.AllLords).ForEach(darkHorse.QualifiedCharacters.Add);
        darkHorse.PetExperienceFormula = PetExperienceFormula;
        darkHorse.MaximumItemLevel = 50;

        var darkRaven = this.CreatePet(5, 0, 1, 1, "Dark Raven", 218, false, false);
        darkRaven.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(1));
        darkRaven.PetExperienceFormula = PetExperienceFormula;
        darkRaven.MaximumItemLevel = 50;
        this.GameConfiguration.DetermineCharacterClasses(CharacterClasses.AllLords).ForEach(darkRaven.QualifiedCharacters.Add);

        var fenrir = this.CreatePet(37, SkillNumber.PlasmaStorm, 2, 2, "Horn of Fenrir", 300, false, true, (Stats.CanFly, 1.0f, AggregateType.AddRaw));
        this.AddFenrirOptions(fenrir);

        this.CreatePet(64, 0, 1, 1, "Demon", 1, false, true, (Stats.AttackDamageIncrease, 1.4f, AggregateType.Multiplicate), (Stats.AttackSpeed, 10f, AggregateType.AddRaw));
        this.CreatePet(65, 0, 1, 1, "Spirit of Guardian", 1, false, true, (Stats.DamageReceiveDecrement, 0.7f, AggregateType.Multiplicate), (Stats.MaximumHealth, 50f, AggregateType.AddRaw));
        this.CreatePet(67, 0, 1, 1, "Pet Rudolf", 28, false, true);
        this.CreatePet(80, 0, 1, 1, "Pet Panda", 1, false, true, (Stats.ExperienceRate, 1.5f, AggregateType.Multiplicate), (Stats.MasterExperienceRate, 1.5f, AggregateType.Multiplicate), (Stats.DefenseBase, 50f, AggregateType.AddRaw));
        this.CreatePet(106, 0, 1, 1, "Pet Unicorn", 28, false, true, (Stats.MoneyAmountRate, 1.5f, AggregateType.Multiplicate), (Stats.DefenseBase, 50f, AggregateType.AddRaw));
        this.CreatePet(123, 0, 1, 1, "Pet Skeleton", 1, false, true, (Stats.AttackDamageIncrease, 1.2f, AggregateType.Multiplicate), (Stats.AttackSpeed, 10f, AggregateType.AddRaw), (Stats.ExperienceRate, 1.3f, AggregateType.Multiplicate));

        // Items which are required for crafting:
        this.CreateSpirit();
        this.SplinterOfArmor();
        this.BlessOfGuardian();
        this.ClawOfBeast();
        this.FragmentOfHorn();
        this.BrokenHorn();
    }

    private void SplinterOfArmor()
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Group = 13;
        item.Number = 32;
        item.Name = "Splinter of Armor";
        item.Width = 1;
        item.Height = 1;
        item.Durability = 20;
        item.SetGuid(item.Group, item.Number);
        this.GameConfiguration.Items.Add(item);
    }

    private void BlessOfGuardian()
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Group = 13;
        item.Number = 33;
        item.Name = "Bless of Guardian";
        item.Width = 1;
        item.Height = 1;
        item.Durability = 1;
        item.Durability = 20;
        item.SetGuid(item.Group, item.Number);
        this.GameConfiguration.Items.Add(item);
    }

    private void ClawOfBeast()
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Group = 13;
        item.Number = 34;
        item.Name = "Claw of Beast";
        item.Width = 1;
        item.Height = 1;
        item.Durability = 10;
        item.SetGuid(item.Group, item.Number);
        this.GameConfiguration.Items.Add(item);
    }

    private void FragmentOfHorn()
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Group = 13;
        item.Number = 35;
        item.Name = "Fragment of Horn";
        item.Width = 1;
        item.Height = 1;
        item.Durability = 1;
        item.SetGuid(item.Group, item.Number);
        this.GameConfiguration.Items.Add(item);
    }

    private void BrokenHorn()
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        item.Group = 13;
        item.Number = 36;
        item.Name = "Broken Horn";
        item.Width = 2;
        item.Height = 2;
        item.Durability = 1;
        item.SetGuid(item.Group, item.Number);
        this.GameConfiguration.Items.Add(item);
    }

    private void CreateSpirit()
    {
        // A level 0 spirit is for the Dark Horse; Level 1 is for the Dark Raven.
        var spirit = this.Context.CreateNew<ItemDefinition>();
        spirit.Group = 13;
        spirit.Number = 31;
        spirit.Name = "Spirit";
        spirit.Width = 1;
        spirit.Height = 1;
        spirit.Durability = 1;
        spirit.SetGuid(spirit.Group, spirit.Number);
        this.GameConfiguration.Items.Add(spirit);

        var horseDrop = this.Context.CreateNew<DropItemGroup>();
        horseDrop.SetGuid(NumberConversionExtensions.MakeWord(13, 31).ToSigned(), 0);
        horseDrop.ItemLevel = 0;
        horseDrop.Chance = 0.001;
        horseDrop.Description = "Dark Horse Spirit";
        horseDrop.PossibleItems.Add(spirit);
        horseDrop.MinimumMonsterLevel = 102;
        this.GameConfiguration.DropItemGroups.Add(horseDrop);
        BaseMapInitializer.RegisterDefaultDropItemGroup(horseDrop);

        var ravenDrop = this.Context.CreateNew<DropItemGroup>();
        horseDrop.SetGuid(NumberConversionExtensions.MakeWord(13, 31).ToSigned(), 1);
        ravenDrop.ItemLevel = 1;
        ravenDrop.Chance = 0.001;
        ravenDrop.Description = "Dark Raven Spirit";
        ravenDrop.PossibleItems.Add(spirit);
        ravenDrop.MinimumMonsterLevel = 96;
        this.GameConfiguration.DropItemGroups.Add(ravenDrop);
        BaseMapInitializer.RegisterDefaultDropItemGroup(ravenDrop);
    }

    private ItemDefinition CreatePet(byte number, SkillNumber skillNumber, byte width, byte height, string name, int dropLevelAndLevelRequirement, bool dropsFromMonsters, bool addAllCharacterClasses, params (AttributeDefinition, float, AggregateType)[] basePowerUps)
    {
        var pet = this.Context.CreateNew<ItemDefinition>();
        pet.SetGuid(13, number);
        this.GameConfiguration.Items.Add(pet);
        pet.Group = 13;
        pet.Number = number;
        pet.Width = width;
        pet.Height = height;
        pet.Name = name;
        pet.DropLevel = (byte)System.Math.Min(255, dropLevelAndLevelRequirement);
        pet.DropsFromMonsters = dropsFromMonsters;
        pet.Durability = 255;
        pet.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(8));
        if (skillNumber > 0)
        {
            pet.Skill = this.GameConfiguration.Skills.First(skill => skill.Number == (short)skillNumber);
        }

        if (addAllCharacterClasses)
        {
            this.GameConfiguration.DetermineCharacterClasses(CharacterClasses.All).ForEach(pet.QualifiedCharacters.Add);
        }

        this.CreateItemRequirementIfNeeded(pet, Stats.Level, dropLevelAndLevelRequirement);

        foreach (var basePowerUp in basePowerUps)
        {
            if (basePowerUp.Item1 != null)
            {
                var powerUpDefinition = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                powerUpDefinition.TargetAttribute = basePowerUp.Item1.GetPersistent(this.GameConfiguration);
                powerUpDefinition.BaseValue = basePowerUp.Item2;
                powerUpDefinition.AggregateType = basePowerUp.Item3;
                pet.BasePowerUpAttributes.Add(powerUpDefinition);
            }
        }

        return pet;
    }

    private void AddDinorantOptions(ItemDefinition dinorant)
    {
        var dinoOptionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
        dinoOptionDefinition.SetGuid(ItemOptionDefinitionNumbers.Dino);
        this.GameConfiguration.ItemOptions.Add(dinoOptionDefinition);

        dinoOptionDefinition.Name = "Dinorant Options";
        dinoOptionDefinition.AddChance = 0.1f;
        dinoOptionDefinition.AddsRandomly = true;
        dinoOptionDefinition.MaximumOptionsPerItem = 1;

        dinoOptionDefinition.PossibleOptions.Add(this.CreateOption(ItemOptionTypes.Excellent, 1, Stats.DamageReceiveDecrement, 0.95f, AggregateType.Multiplicate, ItemOptionDefinitionNumbers.Dino));
        dinoOptionDefinition.PossibleOptions.Add(this.CreateOption(ItemOptionTypes.Excellent, 2, Stats.MaximumAbility, 50f, AggregateType.AddFinal, ItemOptionDefinitionNumbers.Dino));
        dinoOptionDefinition.PossibleOptions.Add(this.CreateOption(ItemOptionTypes.Excellent, 4, Stats.AttackSpeed, 5f, AggregateType.AddFinal, ItemOptionDefinitionNumbers.Dino));

        dinorant.PossibleItemOptions.Add(dinoOptionDefinition);
    }

    /// <summary>
    /// Adds the fenrir options.
    /// </summary>
    /// <param name="fenrir">The fenrir.</param>
    /// <remarks>
    /// See <see cref="ItemOptionTypes.BlueFenrir" />, <see cref="ItemOptionTypes.BlackFenrir" />, <see cref="ItemOptionTypes.GoldFenrir" />.
    /// </remarks>
    private void AddFenrirOptions(ItemDefinition fenrir)
    {
        var fenrirOptionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
        fenrirOptionDefinition.SetGuid(ItemOptionDefinitionNumbers.Fenrir);
        this.GameConfiguration.ItemOptions.Add(fenrirOptionDefinition);

        fenrirOptionDefinition.Name = "Fenrir Options";

        fenrirOptionDefinition.PossibleOptions.Add(this.CreateOption(ItemOptionTypes.BlackFenrir, 1, Stats.AttackDamageIncrease, 1.1f, AggregateType.Multiplicate, ItemOptionDefinitionNumbers.Fenrir));
        fenrirOptionDefinition.PossibleOptions.Add(this.CreateOption(ItemOptionTypes.BlueFenrir, 2, Stats.DamageReceiveDecrement, 0.95f, AggregateType.Multiplicate, ItemOptionDefinitionNumbers.Fenrir));

        fenrirOptionDefinition.PossibleOptions.Add(this.CreateOption(ItemOptionTypes.GoldFenrir, 4, Stats.MaximumHealth, 200f, AggregateType.AddFinal, ItemOptionDefinitionNumbers.Fenrir));
        fenrirOptionDefinition.PossibleOptions.Add(this.CreateOption(ItemOptionTypes.GoldFenrir, 4, Stats.MaximumMana, 200f, AggregateType.AddFinal, ItemOptionDefinitionNumbers.Fenrir));
        fenrirOptionDefinition.PossibleOptions.Add(this.CreateOption(ItemOptionTypes.GoldFenrir, 4, Stats.MaximumPhysBaseDmg, 33f, AggregateType.AddRaw, ItemOptionDefinitionNumbers.Fenrir));
        fenrirOptionDefinition.PossibleOptions.Add(this.CreateOption(ItemOptionTypes.GoldFenrir, 4, Stats.MaximumWizBaseDmg, 16f, AggregateType.AddRaw, ItemOptionDefinitionNumbers.Fenrir));

        fenrir.PossibleItemOptions.Add(fenrirOptionDefinition);
    }

    private IncreasableItemOption CreateOption(ItemOptionType optionType, int number, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType, short optionNumber)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(optionNumber, attributeDefinition.Id.ExtractFirstTwoBytes());
        itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == optionType);
        itemOption.Number = number;
        itemOption.PowerUpDefinition = this.CreatePowerUpDefinition(attributeDefinition, value, aggregateType);
        return itemOption;
    }
}