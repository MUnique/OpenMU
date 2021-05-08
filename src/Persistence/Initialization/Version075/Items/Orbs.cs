// <copyright file="Orbs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Items
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.Persistence.Initialization.Skills;

    /// <summary>
    /// Initializes orb items which are used to learn skills.
    /// </summary>
    public class Orbs : InitializerBase
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
            this.CreateOrb(8, SkillNumber.Heal, 1, "Orb of Healing", 8, 0, 100, 0, 0, 800, 0, 0, 1);
            this.CreateOrb(9, SkillNumber.GreaterDefense, 1, "Orb of Greater Defense", 13, 0, 100, 0, 0, 3000, 0, 0, 1);
            this.CreateOrb(10, SkillNumber.GreaterDamage, 1, "Orb of Greater Damage", 18, 0, 100, 0, 0, 7000, 0, 0, 1);
            this.CreateOrb(11, SkillNumber.SummonGoblin, 1, "Orb of Summoning", 3, 0, 0, 0, 0, 150, 0, 0, 1);
        }

        private ItemDefinition CreateOrb(byte number, SkillNumber skillNumber, byte height, string name, byte dropLevel, int levelRequirement, int energyRequirement, int strengthRequirement, int agilityRequirement, int money, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel)
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
            orb.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.LearnablesConsumeHandler).FullName;
            if (skillNumber == SkillNumber.SummonGoblin)
            {
                orb.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.SummoningOrbConsumeHandler).FullName;
            }

            this.CreateItemRequirementIfNeeded(orb, Stats.Level, levelRequirement);
            this.CreateItemRequirementIfNeeded(orb, Stats.TotalEnergy, energyRequirement);
            this.CreateItemRequirementIfNeeded(orb, Stats.TotalStrength, strengthRequirement);
            this.CreateItemRequirementIfNeeded(orb, Stats.TotalAgility, agilityRequirement);

            orb.Value = money;
            var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel == 1, darkKnightClassLevel == 1, elfClassLevel == 1);
            foreach (var characterClass in classes)
            {
                orb.QualifiedCharacters.Add(characterClass);
            }

            return orb;
        }
    }
}
