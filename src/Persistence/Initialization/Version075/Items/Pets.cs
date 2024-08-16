// <copyright file="Pets.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

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
        this.CreatePet(0, "Guardian Angel", 23, (Stats.DamageReceiveDecrement, 0.8f, AggregateType.Multiplicate), (Stats.MaximumHealth, 50f, AggregateType.AddRaw));
        this.CreatePet(1, "Imp", 28, (Stats.AttackDamageIncrease, 1.3f, AggregateType.Multiplicate));
        this.CreatePet(2, "Horn of Uniria", 25);
    }

    private ItemDefinition CreatePet(byte number, string name, int dropLevelAndLevelRequirement, params (AttributeDefinition, float, AggregateType)[] basePowerUps)
    {
        var pet = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(pet);
        pet.Group = 13;
        pet.Number = number;
        pet.Width = 1;
        pet.Height = 1;
        pet.Name = name;
        pet.DropLevel = (byte)System.Math.Min(255, dropLevelAndLevelRequirement);
        pet.DropsFromMonsters = true;
        pet.Durability = 255;
        pet.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(8));
        this.GameConfiguration.DetermineCharacterClasses(true, true, true).ForEach(pet.QualifiedCharacters.Add);

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

        pet.SetGuid(pet.Group, pet.Number);
        return pet;
    }
}