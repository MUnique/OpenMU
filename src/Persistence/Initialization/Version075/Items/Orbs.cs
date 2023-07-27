// <copyright file="Orbs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initializes orb items which are used to learn skills.
/// </summary>
internal class Orbs : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Orbs"/> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Orbs(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Initializes all orbs.
    /// </summary>
    public override void Initialize()
    {
        this.CreateOrb(8, SkillNumber.Heal, 1, "Orb of Healing", 8, 0, 100, 0, 0, 800, CharacterClasses.FairyElf);
        this.CreateOrb(9, SkillNumber.GreaterDefense, 1, "Orb of Greater Defense", 13, 0, 100, 0, 0, 3000, CharacterClasses.FairyElf);
        this.CreateOrb(10, SkillNumber.GreaterDamage, 1, "Orb of Greater Damage", 18, 0, 100, 0, 0, 7000, CharacterClasses.FairyElf);
        var summonOrb = this.CreateOrb(11, SkillNumber.SummonGoblin, 1, "Orb of Summoning", 3, 0, 0, 0, 0, 150, CharacterClasses.FairyElf);
        summonOrb.MaximumItemLevel = 5;
    }

    /// <summary>
    /// Creates the orb definition.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="skillNumber">The skill number.</param>
    /// <param name="height">The height.</param>
    /// <param name="name">The name.</param>
    /// <param name="dropLevel">The drop level.</param>
    /// <param name="levelRequirement">The level requirement.</param>
    /// <param name="energyRequirement">The energy requirement.</param>
    /// <param name="strengthRequirement">The strength requirement.</param>
    /// <param name="agilityRequirement">The agility requirement.</param>
    /// <param name="money">The money.</param>
    /// <param name="characterClasses">The character classes.</param>
    /// <returns>The created definition.</returns>
    protected ItemDefinition CreateOrb(byte number, SkillNumber skillNumber, byte height, string name, byte dropLevel, int levelRequirement, int energyRequirement, int strengthRequirement, int agilityRequirement, int money, CharacterClasses characterClasses)
    {
        var orb = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(orb);
        orb.Group = 12;
        orb.Number = number;
        orb.Skill = this.GameConfiguration.Skills.First(skill => skill.Number == (short)skillNumber);
        orb.Width = 1;
        orb.Height = height;
        orb.Name = name;
        orb.DropLevel = dropLevel;
        orb.DropsFromMonsters = true;
        orb.Durability = 1;

        this.CreateItemRequirementIfNeeded(orb, Stats.Level, levelRequirement);
        this.CreateItemRequirementIfNeeded(orb, Stats.TotalEnergy, energyRequirement);
        this.CreateItemRequirementIfNeeded(orb, Stats.TotalStrength, strengthRequirement);
        this.CreateItemRequirementIfNeeded(orb, Stats.TotalAgility, agilityRequirement);

        orb.Value = money;
        var classes = this.GameConfiguration.DetermineCharacterClasses(characterClasses);
        foreach (var characterClass in classes)
        {
            orb.QualifiedCharacters.Add(characterClass);
        }

        orb.SetGuid(orb.Group, orb.Number);
        return orb;
    }
}