// <copyright file="Pets.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Items;

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
        this.CreatePet(0, 0, "Guardian Angel", 23, true, (Stats.DamageReceiveDecrement, 0.8f, AggregateType.Multiplicate), (Stats.MaximumHealth, 50f, AggregateType.AddRaw));
        this.CreatePet(1, 0, "Imp", 28, true, (Stats.AttackDamageIncrease, 1.3f, AggregateType.Multiplicate));
        this.CreatePet(2, 0, "Horn of Uniria", 25, true);

        var dinorant = this.CreatePet(3, SkillNumber.FireBreath, "Horn of Dinorant", 110, false, (Stats.DamageReceiveDecrement, 0.9f, AggregateType.Multiplicate), (Stats.AttackDamageIncrease, 1.15f, AggregateType.Multiplicate), (Stats.CanFly, 1.0f, AggregateType.AddRaw));
        this.AddDinorantOptions(dinorant);
    }

    private ItemDefinition CreatePet(byte number, SkillNumber skillNumber, string name, int dropLevelAndLevelRequirement, bool dropsFromMonsters, params (AttributeDefinition, float, AggregateType)[] basePowerUps)
    {
        var pet = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(pet);
        pet.Group = 13;
        pet.Number = number;
        pet.Width = 1;
        pet.Height = 1;
        pet.Name = name;
        pet.DropLevel = (byte)System.Math.Min(255, dropLevelAndLevelRequirement);
        pet.DropsFromMonsters = dropsFromMonsters;
        pet.Durability = 255;
        pet.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(8));
        if (skillNumber > 0)
        {
            pet.Skill = this.GameConfiguration.Skills.First(skill => skill.Number == (short)skillNumber);
        }

        this.GameConfiguration.DetermineCharacterClasses(CharacterClasses.FairyElf | CharacterClasses.DarkKnight | CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator)
            .ForEach(pet.QualifiedCharacters.Add);

        this.CreateItemRequirementIfNeeded(pet, Stats.Level, dropLevelAndLevelRequirement);

        foreach (var basePowerUp in basePowerUps)
        {
            if (basePowerUp.Item1 != null)
            {
                var powerUpDefinition = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                powerUpDefinition.TargetAttribute = basePowerUp.Item1.GetPersistent(this.GameConfiguration);
                powerUpDefinition.BaseValue = basePowerUp.Item2;
                pet.BasePowerUpAttributes.Add(powerUpDefinition);
            }
        }

        pet.SetGuid(pet.Group, pet.Number);
        return pet;
    }

    private void AddDinorantOptions(ItemDefinition dinorant)
    {
        var dinoOptionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
        this.GameConfiguration.ItemOptions.Add(dinoOptionDefinition);

        dinoOptionDefinition.Name = "Dinorant Options";
        dinoOptionDefinition.AddChance = 0.1f;
        dinoOptionDefinition.AddsRandomly = true;
        dinoOptionDefinition.MaximumOptionsPerItem = 1;

        dinoOptionDefinition.PossibleOptions.Add(this.CreateOption(ItemOptionTypes.Excellent, 1, Stats.DamageReceiveDecrement, 0.95f, AggregateType.Multiplicate));
        dinoOptionDefinition.PossibleOptions.Add(this.CreateOption(ItemOptionTypes.Excellent, 2, Stats.MaximumAbility, 50f, AggregateType.AddFinal));
        dinoOptionDefinition.PossibleOptions.Add(this.CreateOption(ItemOptionTypes.Excellent, 4, Stats.AttackSpeed, 5f, AggregateType.AddFinal));

        dinorant.PossibleItemOptions.Add(dinoOptionDefinition);
    }

    private IncreasableItemOption CreateOption(ItemOptionType optionType, int number, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == optionType);
        itemOption.Number = number;
        itemOption.PowerUpDefinition = this.CreatePowerUpDefinition(attributeDefinition, value, aggregateType);
        return itemOption;
    }
}