// <copyright file="Scrolls.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initializer for scroll items which allow a character to learn <see cref="Skill"/>s.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Persistence.Initialization.InitializerBase" />
    public class Scrolls : InitializerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scrolls"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Scrolls(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <summary>
        /// Initializes the scroll data.
        /// </summary>
        /// <remarks>
        /// Regex: (?m)^\s*(\d+)\s+(-*\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+\"(.+?)\"\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+).*$
        /// Replace by: this.CreateScroll($1, TODO, $5, "$9", $10, $13, $14, $15, $16, $17, $18, $19, $20);
        /// </remarks>
        public override void Initialize()
        {
            this.CreateScroll(0, 1, "Scroll of Poison", 30, 0, 100, 17000, 1, 0, 0, 1, 0, 0, 0);
            this.CreateScroll(1, 2, "Scroll of Meteorite", 21, 0, 100, 11000, 1, 0, 0, 1, 0, 1, 0);
            this.CreateScroll(2, 3, "Scroll of Lighting", 13, 0, 100, 3000, 1, 0, 0, 1, 0, 0, 0);
            this.CreateScroll(3, 4, "Scroll of Fire Ball", 5, 0, 100, 300, 1, 0, 0, 1, 0, 1, 0);
            this.CreateScroll(4, 5, "Scroll of Flame", 35, 0, 100, 21000, 1, 0, 0, 1, 0, 0, 0);
            this.CreateScroll(5, 6, "Scroll of Teleport", 17, 0, 100, 5000, 1, 0, 0, 0, 0, 0, 0);
            this.CreateScroll(6, 7, "Scroll of Ice", 25, 0, 100, 14000, 1, 0, 0, 1, 0, 1, 0);
            this.CreateScroll(7, 8, "Scroll of Twister", 40, 0, 100, 25000, 1, 0, 0, 1, 0, 0, 0);
            this.CreateScroll(8, 9, "Scroll of Evil Spirit", 50, 0, 100, 35000, 1, 0, 0, 1, 0, 0, 0);
            this.CreateScroll(9, 10, "Scroll of Hellfire", 60, 0, 100, 60000, 1, 0, 0, 1, 0, 0, 0);
            this.CreateScroll(10, 11, "Scroll of Power Wave", 9, 0, 100, 1150, 1, 0, 0, 1, 0, 1, 0);
            this.CreateScroll(11, 12, "Scroll of Aqua Beam", 74, 0, 110, 100000, 1, 0, 0, 1, 0, 0, 0);
            this.CreateScroll(12, 13, "Scroll of Cometfall", 80, 0, 150, 175000, 1, 0, 0, 1, 0, 0, 0);
            this.CreateScroll(13, 14, "Scroll of Inferno", 88, 0, 200, 265000, 1, 0, 0, 1, 0, 0, 0);
            this.CreateScroll(14, 15, "Scroll of Teleport Ally", 83, 0, 188, 245000, 2, 0, 0, 0, 0, 0, 0);
            this.CreateScroll(15, 16, "Scroll of Soul Barrier", 77, 0, 126, 135000, 1, 0, 0, 0, 0, 0, 0);
            this.CreateScroll(16, 38, "Scroll of Decay", 96, 0, 243, 345000, 2, 0, 0, 0, 0, 0, 0);
            this.CreateScroll(17, 39, "Scroll of Ice Storm", 93, 0, 223, 315000, 2, 0, 0, 0, 0, 0, 0);
            this.CreateScroll(18, 40, "Scroll of Nova", 100, 0, 258, 410000, 2, 0, 0, 0, 0, 0, 0);
            this.CreateScroll(19, 215, "Chain Lightning Parchment", 75, 0, 75, 175000, 0, 0, 0, 0, 0, 1, 0);
            this.CreateScroll(20, 214, "Drain Life Parchment", 35, 0, 93, 100000, 0, 0, 0, 0, 0, 1, 0);
            this.CreateScroll(21, 230, "Lightning Shock Parchment", 93, 0, 216, 315000, 0, 0, 0, 0, 0, 1, 0);
            this.CreateScroll(22, 217, "Damage Reflection Parchment", 80, 0, 111, 245000, 0, 0, 0, 0, 0, 1, 0);
            this.CreateScroll(23, 218, "Berserker Parchment", 83, 0, 181, 265000, 0, 0, 0, 0, 0, 1, 0);
            this.CreateScroll(24, 219, "Sleep Parchment", 40, 0, 100, 135000, 0, 0, 0, 0, 0, 1, 0);
            this.CreateScroll(26, 221, "Weakness Parchment", 93, 0, 173, 410000, 0, 0, 0, 0, 0, 2, 0);
            this.CreateScroll(27, 222, "Innovation Parchment", 111, 0, 201, 450000, 0, 0, 0, 0, 0, 2, 0);
            this.CreateScroll(28, 233, "Scroll of Wizardry Enhance", 100, 220, 118, 425000, 2, 0, 0, 0, 0, 0, 0);
            this.CreateScroll(29, 237, "Scroll of Gigantic Storm", 100, 220, 118, 380000, 0, 0, 0, 1, 0, 0, 0);
            this.CreateScroll(30, 262, "Chain Drive Parchment", 80, 150, 0, 175000, 0, 0, 0, 0, 0, 0, 1);
            this.CreateScroll(31, 263, "Dark Side Parchment", 100, 180, 0, 345000, 0, 0, 0, 0, 0, 0, 1);
            this.CreateScroll(32, 264, "Dragon Roar Parchment", 90, 150, 0, 265000, 0, 0, 0, 0, 0, 0, 1);
            this.CreateScroll(33, 265, "Dragon Slasher Parchment", 100, 200, 0, 345000, 0, 0, 0, 0, 0, 0, 1);
            this.CreateScroll(34, 266, "Ignore Defense Parchment", 100, 120, 404, 345000, 0, 0, 0, 0, 0, 0, 1);
            this.CreateScroll(35, 267, "Increase Health Parchment", 90, 80, 132, 265000, 0, 0, 0, 0, 0, 0, 1);
            this.CreateScroll(36, 268, "Increase Block Parchment", 70, 50, 80, 60000, 0, 0, 0, 0, 0, 0, 1);
        }

        private void CreateScroll(byte number, int skillNumber, string name, byte dropLevel, int levelRequirement, int energyRequirement, int money, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel, int ragefighterClassLevel)
        {
            var scroll = this.Context.CreateNew<ItemDefinition>();
            this.GameConfiguration.Items.Add(scroll);
            scroll.Group = 15;
            scroll.Number = number;
            scroll.Skill = this.GameConfiguration.Skills.First(skill => skill.Number == skillNumber);
            scroll.Width = 1;
            scroll.Height = 2;
            scroll.Name = name;
            scroll.DropLevel = dropLevel;
            scroll.DropsFromMonsters = true;
            scroll.Durability = 1;
            scroll.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.LearnablesConsumeHandler).FullName;

            if (levelRequirement > 0)
            {
                var requirement = this.Context.CreateNew<AttributeRequirement>();
                requirement.Attribute = Stats.Level.GetPersistent(this.GameConfiguration);
                requirement.MinimumValue = levelRequirement;
                scroll.Requirements.Add(requirement);
            }

            if (energyRequirement > 0)
            {
                var requirement = this.Context.CreateNew<AttributeRequirement>();
                requirement.Attribute = Stats.TotalEnergyRequirementValue.GetPersistent(this.GameConfiguration);
                requirement.MinimumValue = energyRequirement;
                scroll.Requirements.Add(requirement);
            }

            scroll.Value = money;
            var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, ragefighterClassLevel);
            foreach (var characterClass in classes)
            {
                scroll.QualifiedCharacters.Add(characterClass);
            }
        }
    }
}
