// <copyright file="Skills.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initialization logic for <see cref="Skill"/>s.
    /// </summary>
    internal class Skills : InitializerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Skills"/> class.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Skills(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
        {
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <remarks>
        /// Regex: (?m)^\s*(\d+)\s+\"(.+?)\"\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(-*\d+)\s(-*\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)\s*$
        /// Replace by: this.CreateSkill($1, "$2", $3, $4, $5, $6, $7, $9, $10, $11, $12, $13, $15, $19, $20, $21, $22, $23, $24, $25, $26, $27, $28);
        /// </remarks>
        public override void Initialize()
        {
            this.CreateSkill(1, "Poison", 30, 12, 42, 0, 6, 100, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(2, "Meteorite", 21, 21, 12, 0, 6, 100, 0, 4, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(3, "Lightning", 13, 17, 15, 0, 6, 100, 0, 2, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(4, "Fire Ball", 5, 8, 3, 0, 6, 100, 0, 3, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(5, "Flame", 35, 25, 50, 0, 6, 100, 0, 3, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(6, "Teleport", 17, 0, 30, 0, 6, 100, 0, -1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(7, "Ice", 25, 10, 38, 0, 6, 100, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(8, "Twister", 40, 35, 60, 0, 6, 100, 0, 5, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(9, "Evil Spirit", 50, 45, 90, 0, 6, 100, 0, -1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(10, "Hellfire", 60, 120, 160, 0, 0, 100, 0, 3, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(11, "Power Wave", 9, 14, 5, 0, 6, 100, 0, -1, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(12, "Aqua Beam", 74, 80, 140, 0, 6, 110, 0, 6, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(13, "Cometfall", 80, 70, 150, 0, 3, 150, 0, 2, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(14, "Inferno", 88, 100, 200, 0, 0, 200, 0, 3, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(15, "Teleport Ally", 83, 0, 90, 25, 6, 188, 0, -1, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(16, "Soul Barrier", 77, 0, 70, 22, 6, 126, 0, -1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.Buff);
            this.CreateSkill(17, "Energy Ball", 2, 3, 1, 0, 6, 0, 0, -1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(18, "Defense", 0, 0, 30, 0, 0, 0, 0, -1, -1, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, SkillType.Buff);
            this.CreateSkill(19, "Falling Slash", 0, 0, 9, 0, 3, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 0);
            this.CreateSkill(20, "Lunge", 0, 0, 9, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(21, "Uppercut", 0, 0, 8, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(22, "Cyclone", 0, 0, 9, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(23, "Slash", 0, 0, 10, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(24, "Triple Shot", 0, 0, 5, 0, 6, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(26, "Heal", 8, 0, 20, 0, 6, 100, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Buff);
            this.CreateSkill(27, "Greater Defense", 13, 0, 30, 0, 6, 100, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Buff);
            this.CreateSkill(28, "Greater Damage", 18, 0, 40, 0, 6, 100, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Buff);
            this.CreateSkill(30, "Summon Goblin", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(31, "Summon Stone Golem", 0, 0, 70, 0, 0, 170, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(32, "Summon Assassin", 0, 0, 110, 0, 0, 190, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(33, "Summon Elite Yeti", 0, 0, 160, 0, 0, 230, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(34, "Summon Dark Knight", 0, 0, 200, 0, 0, 250, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(35, "Summon Bali", 0, 0, 250, 0, 0, 260, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(36, "Summon Soldier", 0, 0, 350, 0, 0, 280, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(38, "Decay", 96, 95, 110, 7, 6, 243, 0, 1, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(39, "Ice Storm", 93, 80, 100, 5, 6, 223, 0, 0, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(40, "Nova", 100, 0, 180, 45, 6, 258, 0, 3, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(41, "Twisting Slash", 0, 0, 10, 10, 2, 0, 0, 5, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(42, "Rageful Blow", 170, 60, 25, 20, 3, 0, 0, 4, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(43, "Death Stab", 160, 70, 15, 12, 2, 0, 0, 5, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(44, "Crescent Moon Slash", 0, 90, 22, 15, 4, 0, 0, -1, -1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(45, "Lance", 0, 90, 150, 10, 6, 0, 0, -1, -1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(46, "Starfall", 0, 120, 20, 15, 8, 0, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(47, "Impale", 28, 15, 8, 0, 3, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(48, "Swell Life", 120, 0, 22, 24, 0, 0, 0, -1, -1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.Buff);
            this.CreateSkill(49, "Fire Breath", 110, 30, 9, 0, 3, 0, 0, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(50, "Flame of Evil (Monster)", 60, 120, 160, 0, 0, 100, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(51, "Ice Arrow", 0, 105, 10, 12, 8, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(52, "Penetration", 130, 70, 7, 9, 6, 0, 0, 5, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(55, "Fire Slash", 0, 80, 15, 20, 2, 0, 0, 1, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(56, "Power Slash", 0, 0, 15, 0, 5, 100, 0, -1, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(57, "Spiral Slash", 0, 75, 20, 15, 5, 0, 0, -1, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(60, "Force", 0, 10, 10, 0, 4, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(61, "Fire Burst", 74, 100, 25, 0, 6, 20, 0, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(62, "Earthshake", 0, 150, 0, 50, 10, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(63, "Summon", 98, 0, 70, 30, 0, 34, 400, -1, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(64, "Increase Critical Damage", 82, 0, 50, 50, 0, 25, 300, -1, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, SkillType.Buff);
            this.CreateSkill(65, "Electric Spike", 92, 250, 0, 100, 10, 29, 340, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(66, "Force Wave", 0, 50, 10, 0, 4, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(67, "Stun", 0, 0, 70, 50, 2, 0, 0, -1, -1, 1, 4, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(68, "Cancel Stun", 0, 0, 25, 30, 0, 0, 0, -1, -1, 1, 2, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(69, "Swell Mana", 0, 0, 35, 30, 0, 0, 0, -1, -1, 1, 4, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(70, "Invisibility", 0, 0, 80, 60, 0, 0, 0, -1, -1, 1, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(71, "Cancel Invisibility", 0, 0, 40, 30, 0, 0, 0, -1, -1, 1, 2, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(72, "Abolish Magic", 0, 0, 90, 70, 0, 0, 0, -1, -1, 1, 8, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(73, "Mana Rays", 0, 85, 130, 7, 6, 0, 0, -1, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(74, "Fire Blast", 0, 150, 30, 10, 6, 0, 0, -1, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(76, "Plasma Storm", 110, 60, 50, 20, 6, 0, 0, -1, 0, 0, 0, 2, 2, 2, 1, 1, 2, 1, 0, 0, 0);
            this.CreateSkill(77, "Infinity Arrow", 220, 0, 50, 10, 6, 0, 0, -1, -1, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, SkillType.Buff);
            this.CreateSkill(78, "Fire Scream", 102, 130, 45, 10, 6, 32, 70, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(79, "Explosion", 0, 0, 0, 0, 2, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(200, "Summon Monster", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(201, "Magic Attack Immunity", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(202, "Physical Attack Immunity", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(203, "Potion of Bless", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(204, "Potion of Soul", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(210, "Spell of Protection", 0, 0, 30, 0, 0, 0, 0, 0, 0, 0, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(211, "Spell of Restriction", 0, 0, 30, 0, 3, 0, 0, 0, 0, 0, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(212, "Spell of Pursuit", 0, 0, 30, 0, 0, 0, 0, 0, 0, 0, 10, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(213, "Shied-Burn", 0, 0, 30, 0, 3, 0, 0, 0, 0, 0, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(214, "Drain Life", 35, 35, 50, 0, 6, 93, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(215, "Chain Lightning", 75, 70, 85, 0, 6, 75, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(217, "Damage Reflection", 80, 0, 40, 10, 5, 111, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(218, "Berserker", 83, 0, 100, 50, 5, 181, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(219, "Sleep", 40, 0, 20, 3, 6, 100, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(221, "Weakness", 93, 0, 50, 15, 6, 173, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(222, "Innovation", 111, 0, 70, 15, 6, 201, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(223, "Explosion", 50, 40, 90, 5, 6, 100, 0, 3, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(224, "Requiem", 75, 65, 110, 10, 6, 99, 0, 5, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(225, "Pollution", 85, 80, 120, 15, 6, 115, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(230, "Lightning Shock", 93, 95, 115, 7, 6, 216, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(232, "Strike of Destruction", 100, 110, 30, 24, 5, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(233, "Expansion of Wizardry", 220, 0, 200, 50, 6, 118, 0, -1, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(234, "Recovery", 100, 0, 40, 10, 6, 37, 0, -1, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(235, "Multi-Shot", 100, 40, 10, 7, 6, 0, 0, -1, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(236, "Flame Strike", 100, 140, 20, 25, 3, 0, 0, 3, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(237, "Gigantic Storm", 220, 110, 120, 10, 6, 118, 0, 5, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(238, "Chaotic Diseier", 100, 190, 50, 15, 6, 16, 0, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(239, "Doppelganger Self Explosion", 100, 140, 20, 25, 3, 0, 0, 3, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(260, "Killing Blow", 0, 0, 9, 0, 2, 0, 0, 4, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(261, "Beast Uppercut", 0, 0, 9, 0, 2, 0, 0, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(262, "Chain Drive", 150, 0, 15, 20, 4, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(263, "Dark Side", 180, 0, 70, 0, 4, 0, 0, 5, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(264, "Dragon Roar", 150, 0, 50, 30, 3, 0, 0, 4, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(265, "Dragon Slasher", 200, 0, 100, 100, 4, 0, 0, 5, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(266, "Ignore Defense", 120, 0, 50, 10, 3, 80, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(267, "Increase Health", 80, 0, 50, 10, 7, 35, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(268, "Increase Block", 50, 0, 50, 10, 7, 30, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(269, "Charge", 0, 90, 20, 15, 4, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(270, "Phoenix Shot", 0, 0, 30, 0, 4, 0, 0, 4, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(300, "Durability Reduction (1)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 1, 1, 1);
            this.CreateSkill(301, "PvP Defence Rate Inc", 0, 12, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 1, 2, 1);
            this.CreateSkill(302, "Maximum SD increase", 0, 13, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 2, 3, 1);
            this.CreateSkill(303, "Automatic Mana Rec Inc", 0, 7, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 2, 4, 1);
            this.CreateSkill(304, "Poison Resistance Inc", 0, 1, 0, 0, 0, 0, 0, 1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 2, 5, 1);
            this.CreateSkill(305, "Durability Reduction (2)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 3, 6, 1);
            this.CreateSkill(306, "SD Recovery Speed Inc", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 3, 7, 1);
            this.CreateSkill(307, "Automatic HP Rec Inc", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 3, 8, 1);
            this.CreateSkill(308, "Lightning Resistance Inc", 0, 1, 0, 0, 0, 0, 0, 2, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 3, 9, 1);
            this.CreateSkill(309, "Defense Increase", 0, 16, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 10, 1);
            this.CreateSkill(310, "Automatic AG Rec Inc", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 11, 1);
            this.CreateSkill(311, "Ice Resistance Increase", 0, 1, 0, 0, 0, 0, 0, 0, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 12, 1);
            this.CreateSkill(312, "Durability Reduction (3)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 5, 13, 1);
            this.CreateSkill(313, "Defense Success Rate Inc", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 5, 14, 1);
            this.CreateSkill(314, "Cast Invincibility", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 6, 15, 1);
            this.CreateSkill(315, "Armor Set Bonus Inc", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 6, 16, 1);
            this.CreateSkill(316, "Vengeance", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 6, 17, 1);
            this.CreateSkill(317, "Energy Increase", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 18, 1);
            this.CreateSkill(318, "Stamina Increase", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 19, 1);
            this.CreateSkill(319, "Agility Increase", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 20, 1);
            this.CreateSkill(320, "Strength Increase", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 21, 1);
            this.CreateSkill(321, "Wing of Storm Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 3, 0, 0, 0, 0, 0, 8, 22, 1);
            this.CreateSkill(322, "Wing of Storm Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 3, 0, 0, 0, 0, 0, 8, 23, 1);
            this.CreateSkill(323, "Iron Defense", 0, 1, 0, 0, 0, 0, 0, -1, -1, 4, 0, 3, 3, 3, 3, 3, 3, 3, 6, 15, 1);
            this.CreateSkill(324, "Wing of Storm Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 3, 0, 0, 0, 0, 0, 9, 35, 1);
            this.CreateSkill(325, "Attack Succ Rate Inc", 0, 13, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 1, 36, 1);
            this.CreateSkill(326, "Cyclone Strengthener", 0, 22, 9, 0, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 2, 37, 1);
            this.CreateSkill(327, "Slash Strengthener", 0, 3, 10, 0, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 2, 38, 1);
            this.CreateSkill(328, "Falling Slash Streng", 0, 3, 9, 0, 3, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 2, 39, 1);
            this.CreateSkill(329, "Lunge Strengthener", 0, 3, 9, 0, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 2, 40, 1);
            this.CreateSkill(330, "Twisting Slash Streng", 0, 3, 10, 10, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 3, 41, 1);
            this.CreateSkill(331, "Rageful Blow Streng", 170, 22, 25, 22, 3, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 3, 42, 1);
            this.CreateSkill(332, "Twisting Slash Mastery", 0, 1, 22, 20, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 4, 41, 1);
            this.CreateSkill(333, "Rageful Blow Mastery", 170, 1, 50, 30, 3, 0, 0, 4, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 4, 42, 1);
            this.CreateSkill(334, "Maximum Life Increase", 0, 9, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 43, 1);
            this.CreateSkill(335, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 3, 0, 0, 0, 0, 0, 4, 44, 1);
            this.CreateSkill(336, "Slash Strengthener", 160, 22, 15, 13, 2, 0, 0, 5, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 5, 45, 1);
            this.CreateSkill(337, "Strike of Destr Str", 100, 22, 30, 24, 5, 0, 0, 0, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 5, 46, 1);
            this.CreateSkill(338, "Maximum Mana Increase", 0, 9, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 5, 47, 1);
            this.CreateSkill(339, "Death Stab Proficiency", 160, 7, 30, 26, 2, 0, 0, 5, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 6, 45, 1);
            this.CreateSkill(340, "Strike of Destr Prof", 100, 7, 30, 24, 5, 0, 0, 0, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 6, 46, 1);
            this.CreateSkill(341, "Maximum AG Increase", 0, 8, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 6, 48, 1);
            this.CreateSkill(342, "Death Stab Mastery", 160, 7, 30, 26, 2, 0, 0, 5, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 7, 45, 1);
            this.CreateSkill(343, "Strike of Destr Mast", 100, 1, 30, 24, 5, 0, 0, 0, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 7, 46, 1);
            this.CreateSkill(344, "Blood Storm", 0, 25, 87, 29, 3, 0, 0, -1, 0, 4, 0, 0, 3, 0, 3, 0, 0, 0, 8, 49, 10);
            this.CreateSkill(345, "Combo Strengthener", 0, 7, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 8, 50, 1);
            this.CreateSkill(346, "Blood Storm Strengthener", 0, 22, 87, 29, 3, 0, 0, -1, 0, 4, 0, 0, 3, 0, 3, 0, 0, 0, 9, 49, 1);
            this.CreateSkill(347, "PvP Attack Rate", 0, 14, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 1, 51, 1);
            this.CreateSkill(348, "Two-handed Sword Stren", 0, 4, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 0, 0, 0, 2, 52, 1);
            this.CreateSkill(349, "One-handed Sword Stren", 0, 22, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 0, 0, 0, 2, 53, 1);
            this.CreateSkill(350, "Mace Strengthener", 0, 3, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 2, 54, 1);
            this.CreateSkill(351, "Spear Strengthener", 0, 3, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 2, 55, 1);
            this.CreateSkill(352, "Two-handed Sword Mast", 0, 5, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 0, 0, 0, 3, 56, 1);
            this.CreateSkill(353, "One-handed Sword Mast", 0, 23, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 0, 0, 0, 3, 57, 1);
            this.CreateSkill(354, "Mace Mastery", 0, 1, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 3, 58, 1);
            this.CreateSkill(355, "Spear Mastery", 0, 1, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 3, 59, 1);
            this.CreateSkill(356, "Swell Life Strengt", 120, 7, 24, 26, 0, 0, 0, -1, -1, 4, 0, 0, 3, 0, 0, 0, 0, 0, 4, 60, 1);
            this.CreateSkill(357, "Mana Reduction", 0, 18, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 61, 1);
            this.CreateSkill(358, "Monster Attack SD Inc", 0, 11, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 62, 1);
            this.CreateSkill(359, "Monster Attack Life Inc", 0, 6, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 63, 1);
            this.CreateSkill(360, "Swell Life Proficiency", 120, 7, 26, 28, 0, 0, 0, -1, -1, 4, 0, 0, 3, 0, 0, 0, 0, 0, 5, 60, 1);
            this.CreateSkill(361, "Minimum Attack Power Inc", 0, 22, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 3, 0, 0, 5, 64, 1);
            this.CreateSkill(362, "Monster Attack Mana Inc", 0, 6, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 5, 65, 1);
            this.CreateSkill(363, "Swell Life Mastery", 120, 7, 28, 30, 0, 0, 0, -1, -1, 4, 0, 0, 3, 0, 0, 0, 0, 0, 6, 60, 1);
            this.CreateSkill(364, "Maximum Attack Power Inc", 0, 3, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 3, 0, 0, 6, 66, 1);
            this.CreateSkill(366, "Inc crit damage rate", 0, 7, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 6, 68, 1);
            this.CreateSkill(367, "Restores all Mana", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 69, 1);
            this.CreateSkill(368, "Restores all HP", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 70, 1);
            this.CreateSkill(369, "Inc exc damage rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 71, 1);
            this.CreateSkill(370, "Inc double damage rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 8, 72, 1);
            this.CreateSkill(371, "Inc chance of ignore Def", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 8, 73, 1);
            this.CreateSkill(372, "Restores all SD", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 8, 74, 1);
            this.CreateSkill(373, "Inc triple damage rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 9, 75, 1);
            this.CreateSkill(374, "Eternal Wings Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0, 8, 76, 1);
            this.CreateSkill(375, "Eternal Wings Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0, 8, 77, 1);
            this.CreateSkill(377, "Eternal Wings Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0, 9, 79, 1);
            this.CreateSkill(378, "Flame Strengthener", 35, 3, 55, 0, 6, 100, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 2, 80, 1);
            this.CreateSkill(379, "Lightning Strengthener", 13, 3, 20, 0, 6, 100, 0, 2, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 2, 81, 1);
            this.CreateSkill(380, "Expansion of Wiz Streng", 220, 1, 220, 55, 6, 118, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 2, 82, 1);
            this.CreateSkill(381, "Inferno Strengthener", 88, 22, 220, 0, 0, 200, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 3, 83, 1);
            this.CreateSkill(382, "Blast Strengthener", 80, 22, 165, 0, 3, 150, 0, 2, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 3, 84, 1);
            this.CreateSkill(383, "Expansion of Wiz Mas", 220, 1, 220, 55, 6, 118, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 3, 82, 1);
            this.CreateSkill(384, "Poison Strengthener", 30, 3, 46, 0, 6, 100, 0, 1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 3, 85, 1);
            this.CreateSkill(385, "Evil Spirit Streng", 50, 22, 108, 0, 6, 100, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 4, 86, 1);
            this.CreateSkill(386, "Magic Mastery", 50, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0, 4, 87, 1);
            this.CreateSkill(387, "Decay Strengthener", 96, 22, 120, 10, 6, 243, 0, 1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 4, 88, 1);
            this.CreateSkill(388, "Hellfire Strengthener", 60, 3, 176, 0, 0, 100, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 5, 89, 1);
            this.CreateSkill(389, "Ice Strengthener", 25, 3, 42, 0, 6, 100, 0, 0, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 5, 90, 1);
            this.CreateSkill(390, "Meteor Strengthener", 21, 4, 13, 0, 6, 100, 0, 4, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 6, 210, 1);
            this.CreateSkill(391, "Ice Storm Strengthener", 93, 22, 110, 5, 6, 223, 0, 0, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 6, 92, 1);
            this.CreateSkill(392, "Nova Strengthener", 100, 22, 198, 49, 6, 258, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 7, 93, 1);
            this.CreateSkill(393, "Ice Storm Mastery", 93, 1, 110, 5, 6, 223, 0, 0, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 7, 92, 1);
            this.CreateSkill(394, "Meteor Mastery", 21, 1, 14, 0, 6, 100, 0, 4, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 8, 210, 1);
            this.CreateSkill(395, "Nova Cast Strengthener", 100, 22, 198, 49, 6, 258, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 7, 93, 1);
            this.CreateSkill(397, "One-handed Staff Stren", 0, 22, 0, 0, 0, 0, 0, -1, 1, 3, 0, 3, 0, 0, 3, 0, 0, 0, 2, 96, 1);
            this.CreateSkill(398, "Two-handed Staff Stren", 0, 4, 0, 0, 0, 0, 0, -1, 1, 3, 0, 3, 0, 0, 3, 0, 0, 0, 2, 97, 1);
            this.CreateSkill(399, "Shield Strengthener", 0, 10, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0, 2, 98, 1);
            this.CreateSkill(400, "One-handed Staff Mast", 0, 23, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 3, 0, 0, 0, 3, 99, 1);
            this.CreateSkill(401, "Two-handed Staff Mast", 0, 5, 0, 0, 0, 0, 0, -1, 1, 3, 0, 3, 0, 0, 3, 0, 0, 0, 3, 100, 1);
            this.CreateSkill(402, "Shield Mastery", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0, 3, 101, 1);
            this.CreateSkill(403, "Soul Barrier Strength", 77, 7, 77, 24, 6, 126, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 4, 102, 1);
            this.CreateSkill(404, "Soul Barrier Proficie", 77, 10, 84, 26, 6, 126, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 5, 102, 1);
            this.CreateSkill(405, "Minimum Wizardry Inc", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 3, 0, 0, 0, 5, 103, 1);
            this.CreateSkill(406, "Soul Barrier Mastery", 77, 7, 92, 28, 6, 126, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 6, 102, 1);
            this.CreateSkill(407, "Maximum Wizardry Inc", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 3, 0, 0, 0, 6, 104, 1);
            this.CreateSkill(409, "Illusion Wings Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 8, 106, 1);
            this.CreateSkill(410, "Illusion Wings Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 8, 107, 1);
            this.CreateSkill(411, "Multi-Shot Streng", 100, 22, 11, 7, 6, 0, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 6, 211, 1);
            this.CreateSkill(412, "Illusion Wings Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 9, 109, 1);
            this.CreateSkill(413, "Heal Strengthener", 8, 22, 22, 0, 6, 100, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 2, 110, 1);
            this.CreateSkill(414, "Triple Shot Strengthener", 0, 22, 5, 0, 6, 0, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 2, 111, 1);
            this.CreateSkill(415, "Summoned Monster Str (1)", 0, 16, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 2, 112, 1);
            this.CreateSkill(416, "Penetration Strengthener", 130, 22, 10, 11, 6, 0, 0, 5, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 3, 113, 1);
            this.CreateSkill(417, "Defense Increase Str", 13, 22, 33, 0, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 3, 114, 1);
            this.CreateSkill(418, "Triple Shot Mastery", 0, 0, 9, 0, 6, 0, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 3, 111, 10);
            this.CreateSkill(419, "Summoned Monster Str (2)", 0, 16, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 3, 115, 1);
            this.CreateSkill(420, "Attack Increase Str", 18, 22, 44, 0, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 4, 116, 1);
            this.CreateSkill(421, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 4, 117, 1);
            this.CreateSkill(422, "Attack Increase Mastery", 18, 22, 48, 0, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 5, 116, 1);
            this.CreateSkill(423, "Defense Increase Mastery", 13, 22, 36, 0, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 5, 114, 1);
            this.CreateSkill(424, "Ice Arrow Strengthener", 0, 22, 15, 18, 8, 0, 0, 0, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 5, 118, 1);
            this.CreateSkill(425, "Cure", 0, 0, 72, 10, 6, 0, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 6, 119, 10);
            this.CreateSkill(426, "Party Healing", 0, 0, 66, 12, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 7, 120, 10);
            this.CreateSkill(427, "Poison Arrow", 0, 27, 22, 27, 6, 0, 0, 1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 8, 121, 10);
            this.CreateSkill(428, "Summoned Monster Str (3)", 0, 16, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 7, 122, 1);
            this.CreateSkill(429, "Party Healing Str", 0, 22, 72, 13, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 8, 120, 1);
            this.CreateSkill(430, "Bless", 0, 0, 108, 18, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 8, 123, 10);
            this.CreateSkill(431, "Multi-Shot Mastery", 100, 1, 12, 8, 6, 0, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 7, 211, 1);
            this.CreateSkill(432, "Summon Satyros", 0, 0, 525, 52, 0, 280, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 8, 125, 10);
            this.CreateSkill(433, "Bless Strengthener", 0, 10, 118, 20, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 9, 123, 1);
            this.CreateSkill(434, "Poison Arrow Str", 0, 22, 24, 29, 6, 0, 0, 1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 9, 121, 1);
            this.CreateSkill(435, "Bow Strengthener", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 2, 126, 1);
            this.CreateSkill(436, "Crossbow Strengthener", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 2, 127, 1);
            this.CreateSkill(437, "Shield Strengthener", 0, 10, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 2, 128, 1);
            this.CreateSkill(438, "Bow Mastery", 0, 23, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 3, 129, 1);
            this.CreateSkill(439, "Crossbow Mastery", 0, 5, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 3, 130, 1);
            this.CreateSkill(440, "Shield Mastery", 0, 15, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 3, 131, 1);
            this.CreateSkill(441, "Infinity Arrow Str", 220, 1, 55, 11, 6, 0, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 5, 132, 1);
            this.CreateSkill(442, "Minimum Att Power Inc", 0, 22, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 0, 3, 0, 0, 0, 0, 5, 133, 1);
            this.CreateSkill(443, "Maximum Att Power Inc", 0, 3, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 0, 3, 0, 0, 0, 0, 6, 134, 1);
            this.CreateSkill(445, "DimensionWings Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 8, 136, 1);
            this.CreateSkill(446, "DimensionWings Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 8, 137, 1);
            this.CreateSkill(447, "DimensionWings Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 9, 138, 1);
            this.CreateSkill(448, "Fire Tome Strengthener", 0, 3, 0, 0, 0, 0, 0, 3, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 2, 139, 1);
            this.CreateSkill(449, "Wind Tome Strengthener", 0, 3, 0, 0, 0, 0, 0, 5, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 2, 140, 1);
            this.CreateSkill(450, "Lightning Tome Stren", 0, 3, 0, 0, 0, 0, 0, 2, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 2, 141, 1);
            this.CreateSkill(451, "Fire Tome Mastery", 0, 7, 0, 0, 0, 0, 0, 3, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 3, 139, 1);
            this.CreateSkill(452, "Wind Tome Mastery", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 3, 140, 1);
            this.CreateSkill(453, "Lightning Tome Mastery", 0, 7, 0, 0, 0, 0, 0, 2, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 3, 141, 1);
            this.CreateSkill(454, "Sleep Strengthener", 40, 1, 30, 7, 6, 100, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 3, 142, 1);
            this.CreateSkill(455, "Chain Lightning Str", 75, 22, 103, 0, 6, 75, 0, 2, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 4, 143, 1);
            this.CreateSkill(456, "Lightning Shock Str", 93, 22, 125, 10, 6, 216, 0, 2, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 4, 144, 1);
            this.CreateSkill(457, "Magic Mastery", 0, 22, 0, 0, 0, 0, 0, -1, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 5, 145, 1);
            this.CreateSkill(458, "Drain Life Strengthener", 35, 22, 57, 0, 6, 93, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 5, 146, 1);
            this.CreateSkill(459, "Weakness Strengthener", 93, 3, 55, 17, 6, 173, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 7, 147, 1);
            this.CreateSkill(460, "Innovation Strengthener", 111, 3, 77, 17, 6, 201, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 6, 148, 1);
            this.CreateSkill(461, "Blind", 0, 0, 115, 25, 3, 201, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 8, 149, 10);
            this.CreateSkill(462, "Drain Life Mastery", 35, 17, 62, 0, 6, 93, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 7, 146, 1);
            this.CreateSkill(463, "Blind Strengthener", 0, 1, 126, 27, 3, 201, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 9, 149, 1);
            this.CreateSkill(465, "Stick Strengthener", 0, 22, 0, 0, 0, 0, 0, -1, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 2, 150, 1);
            this.CreateSkill(466, "Other World Tome Streng", 0, 3, 0, 0, 0, 0, 0, -1, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 2, 151, 1);
            this.CreateSkill(467, "Stick Mastery", 0, 5, 0, 0, 0, 0, 0, -1, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 3, 152, 1);
            this.CreateSkill(468, "Other World Tome Mastery", 0, 23, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 3, 153, 1);
            this.CreateSkill(469, "Berserker Strengthener", 83, 7, 150, 75, 5, 181, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 4, 154, 1);
            this.CreateSkill(470, "Berserker Proficiency", 83, 7, 165, 82, 5, 181, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 5, 154, 1);
            this.CreateSkill(471, "Minimum Wiz/Curse Inc", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 5, 155, 1);
            this.CreateSkill(472, "Berserker Mastery", 83, 10, 181, 90, 5, 181, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 6, 154, 1);
            this.CreateSkill(473, "Maximum Wiz/Curse Inc", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 6, 156, 1);
            this.CreateSkill(475, "Wing of Ruin Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0, 8, 158, 1);
            this.CreateSkill(476, "Wing of Ruin Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0, 8, 159, 1);
            this.CreateSkill(478, "Wing of Ruin Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0, 9, 161, 1);
            this.CreateSkill(479, "Cyclone Strengthener", 0, 22, 9, 0, 2, 0, 0, -1, 0, 4, 0, 0, 0, 0, 3, 0, 0, 0, 2, 162, 1);
            this.CreateSkill(480, "Lightning Strengthener", 13, 3, 20, 0, 6, 100, 0, 2, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 2, 163, 1);
            this.CreateSkill(481, "Twisting Slash Stren", 0, 3, 10, 10, 2, 0, 0, -1, 0, 4, 0, 0, 0, 0, 3, 0, 0, 0, 2, 164, 1);
            this.CreateSkill(482, "Power Slash Streng", 0, 3, 15, 0, 5, 100, 0, -1, -1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 2, 165, 1);
            this.CreateSkill(483, "Flame Strengthener", 35, 3, 55, 0, 6, 100, 0, 3, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 3, 166, 1);
            this.CreateSkill(484, "Blast Strengthener", 80, 22, 165, 0, 3, 150, 0, 2, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 3, 167, 1);
            this.CreateSkill(485, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0, 3, 168, 1);
            this.CreateSkill(486, "Inferno Strengthener", 88, 22, 220, 0, 0, 200, 0, 3, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 4, 169, 1);
            this.CreateSkill(487, "Evil Spirit Strengthen", 50, 22, 108, 0, 6, 100, 0, -1, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 4, 170, 1);
            this.CreateSkill(488, "Magic Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0, 4, 171, 1);
            this.CreateSkill(489, "Ice Strengthener", 25, 3, 42, 0, 6, 100, 0, 0, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 5, 172, 1);
            this.CreateSkill(490, "Blood Attack Strengthen", 0, 22, 15, 22, 3, 0, 0, 1, -1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 5, 173, 1);
            this.CreateSkill(491, "Ice Mastery", 25, 1, 46, 0, 6, 100, 0, 0, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 6, 172, 1);
            this.CreateSkill(492, "Flame Strike Strengthen", 0, 22, 30, 37, 3, 0, 0, 3, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 6, 175, 1);
            this.CreateSkill(493, "Fire Slash Mastery", 0, 7, 17, 24, 3, 0, 0, 1, -1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 7, 173, 1);
            this.CreateSkill(494, "Flame Strike Mastery", 0, 7, 33, 40, 3, 0, 0, 3, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 7, 175, 1);
            this.CreateSkill(495, "Earth Prison", 0, 26, 180, 15, 3, 127, 0, 4, 1, 4, 0, 3, 0, 0, 3, 0, 0, 0, 8, 178, 10);
            this.CreateSkill(496, "Gigantic Storm Str", 220, 22, 132, 11, 6, 118, 0, 5, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 7, 180, 1);
            this.CreateSkill(497, "Earth Prison Str", 0, 22, 198, 17, 3, 127, 0, 4, 1, 4, 0, 3, 0, 0, 3, 0, 0, 0, 9, 178, 1);
            this.CreateSkill(504, "Emperor Cape Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 8, 182, 1);
            this.CreateSkill(505, "Emperor Cape Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 8, 183, 1);
            this.CreateSkill(506, "Adds Command Stat", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 8, 184, 1);
            this.CreateSkill(507, "Emperor Cape Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 9, 185, 1);
            this.CreateSkill(508, "Fire Burst Streng", 74, 22, 25, 0, 6, 20, 0, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 2, 186, 1);
            this.CreateSkill(509, "Force Wave Streng", 0, 3, 15, 0, 4, 0, 0, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 2, 187, 1);
            this.CreateSkill(510, "Dark Horse Streng (1)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 2, 188, 1);
            this.CreateSkill(511, "Critical DMG Inc PowUp", 82, 3, 75, 75, 0, 25, 300, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 3, 189, 1);
            this.CreateSkill(512, "Earthshake Streng", 0, 22, 0, 75, 10, 0, 0, 2, 0, 4, 0, 0, 0, 0, 0, 3, 0, 0, 3, 190, 1);
            this.CreateSkill(513, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 3, 191, 1);
            this.CreateSkill(514, "Fire Burst Mastery", 74, 1, 27, 0, 6, 20, 0, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 4, 186, 1);
            this.CreateSkill(515, "Crit DMG Inc PowUp (2)", 82, 10, 82, 82, 0, 25, 300, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 4, 189, 1);
            this.CreateSkill(516, "Earthshake Mastery", 0, 1, 0, 75, 10, 0, 0, 2, 0, 4, 0, 0, 0, 0, 0, 3, 0, 0, 4, 190, 1);
            this.CreateSkill(517, "Crit DMG Inc PowUp (3)", 82, 7, 100, 100, 0, 25, 300, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 5, 189, 1);
            this.CreateSkill(518, "Fire Scream Stren", 102, 22, 45, 11, 6, 32, 70, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 5, 192, 1);
            this.CreateSkill(519, "Electric Spark Streng", 92, 3, 0, 150, 10, 29, 340, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 6, 193, 1);
            this.CreateSkill(520, "Fire Scream Mastery", 102, 5, 49, 12, 6, 32, 70, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 6, 192, 1);
            this.CreateSkill(521, "Iron Defense", 0, 28, 64, 29, 0, 0, 0, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 8, 24, 10);
            this.CreateSkill(522, "Critical Damage Inc M", 82, 1, 110, 110, 0, 25, 300, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 7, 189, 1);
            this.CreateSkill(523, "Chaotic Diseier Str", 100, 22, 75, 22, 6, 16, 0, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 7, 194, 1);
            this.CreateSkill(524, "Iron Defense Str", 0, 3, 70, 31, 0, 0, 0, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 9, 24, 1);
            this.CreateSkill(526, "Dark Spirit Str", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 2, 196, 1);
            this.CreateSkill(527, "Scepter Strengthener", 0, 22, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 0, 0, 0, 3, 0, 0, 2, 197, 1);
            this.CreateSkill(528, "Shield Strengthener", 0, 10, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 2, 198, 1);
            this.CreateSkill(529, "Use Scepter : Pet Str", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 2, 199, 1);
            this.CreateSkill(530, "Dark Spirit Str (2)", 0, 7, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 3, 200, 1);
            this.CreateSkill(531, "Scepter Mastery", 0, 5, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 3, 201, 1);
            this.CreateSkill(532, "Shield Mastery", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 3, 202, 1);
            this.CreateSkill(533, "Command Attack Inc", 0, 20, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 3, 203, 1);
            this.CreateSkill(534, "Dark Spirit Str (3)", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 5, 204, 1);
            this.CreateSkill(535, "Pet Durability Str", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 5, 205, 1);
            this.CreateSkill(536, "Dark Spirit Str (4)", 0, 23, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 6, 206, 1);
            this.CreateSkill(538, "Dark Spirit Str (5)", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 7, 208, 1);
            this.CreateSkill(539, "Spirit Lord", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 8, 209, 1);
            this.CreateSkill(551, "Killing Blow Strengthener", 0, 22, 10, 0, 2, 0, 0, 4, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 2, 210, 1);
            this.CreateSkill(552, "Beast Uppercut Strengthener", 0, 22, 10, 0, 2, 0, 0, 3, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 2, 211, 1);
            this.CreateSkill(554, "Killing Blow Mastery", 0, 1, 10, 0, 2, 0, 0, 4, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 3, 210, 1);
            this.CreateSkill(555, "Beast Uppercut Mastery", 0, 1, 10, 0, 2, 0, 0, 3, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 3, 211, 1);
            this.CreateSkill(557, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 212, 1);
            this.CreateSkill(558, "Chain Drive Strengthener", 150, 22, 22, 22, 4, 0, 0, 0, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 5, 213, 1);
            this.CreateSkill(559, "Dark Side Strengthener", 180, 22, 84, 0, 4, 0, 0, 5, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 5, 214, 1);
            this.CreateSkill(560, "Dragon Roar Strengthener", 150, 22, 60, 33, 3, 0, 0, 4, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 5, 215, 1);
            this.CreateSkill(568, "Equipped Weapon Strengthener", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 2, 216, 1);
            this.CreateSkill(569, "Def SuccessRate IncPowUp", 50, 22, 55, 11, 7, 30, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 2, 217, 1);
            this.CreateSkill(571, "Equipped Weapon Mastery", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 3, 218, 1);
            this.CreateSkill(572, "DefSuccessRate IncMastery", 50, 22, 60, 12, 7, 30, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 3, 217, 1);
            this.CreateSkill(573, "Stamina Increase Strengthener", 80, 5, 55, 11, 7, 35, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 4, 219, 1);
            this.CreateSkill(578, "Durability Reduction (1)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 1, 220, 1);
            this.CreateSkill(579, "Increase PvP Defense Rate", 0, 29, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 1, 221, 1);
            this.CreateSkill(580, "Increase Maximum SD", 0, 33, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 2, 222, 1);
            this.CreateSkill(581, "Increase Mana Recovery Rate", 0, 7, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 2, 223, 1);
            this.CreateSkill(582, "Increase Poison Resistance", 0, 1, 0, 0, 0, 0, 0, 1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 2, 224, 1);
            this.CreateSkill(583, "Durability Reduction (2)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 3, 225, 1);
            this.CreateSkill(584, "Increase SD Recovery Rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 3, 226, 1);
            this.CreateSkill(585, "Increase HP Recovery Rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 3, 227, 1);
            this.CreateSkill(586, "Increase Lightning Resistance", 0, 1, 0, 0, 0, 0, 0, 2, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 3, 228, 1);
            this.CreateSkill(587, "Increases Defense", 0, 35, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 229, 1);
            this.CreateSkill(588, "Increases AG Recovery Rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 230, 1);
            this.CreateSkill(589, "Increase Ice Resistance", 0, 1, 0, 0, 0, 0, 0, 0, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 231, 1);
            this.CreateSkill(590, "Durability Reduction(3)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 5, 232, 1);
            this.CreateSkill(591, "Increase Defense Success Rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 5, 233, 1);
            this.CreateSkill(592, "Cast Invincibility", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 6, 234, 1);
            this.CreateSkill(599, "Increase Attack Success Rate", 0, 30, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 1, 235, 1);
            this.CreateSkill(600, "Increase Maximum HP", 0, 34, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 236, 1);
            this.CreateSkill(601, "Increase Maximum Mana", 0, 34, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 5, 237, 1);
            this.CreateSkill(602, "Increase Maximum AG", 0, 37, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 6, 238, 1);
            this.CreateSkill(603, "Increase PvP Attack Rate", 0, 31, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 1, 239, 1);
            this.CreateSkill(604, "Decrease Mana", 0, 18, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 240, 1);
            this.CreateSkill(605, "Recover SD from Monster Kills", 0, 11, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 241, 1);
            this.CreateSkill(606, "Recover HP from Monster Kills", 0, 6, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 242, 1);
            this.CreateSkill(607, "Increase Minimum Attack Power", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 5, 243, 1);
            this.CreateSkill(608, "Recover Mana Monster Kills", 0, 6, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 5, 244, 1);
            this.CreateSkill(609, "Increase Maximum Attack Power", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 6, 245, 1);
            this.CreateSkill(610, "Increases Crit Damage Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 6, 246, 1);
            this.CreateSkill(611, "Recover Mana Fully", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 7, 247, 1);
            this.CreateSkill(612, "Recovers HP Fully", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 7, 248, 1);
            this.CreateSkill(613, "Increase Exc Damage Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 7, 249, 1);
            this.CreateSkill(614, "Increase Double Damage Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 8, 250, 1);
            this.CreateSkill(615, "Increase Ignore Def Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 9, 251, 1);
            this.CreateSkill(616, "Recovers SD Fully", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 8, 252, 1);
            this.CreateSkill(617, "Increase Triple Damage Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 9, 253, 1);
        }

        private void CreateSkill(short skillId, string name, int levelRequirement, int damage, int manaConsumption, int abilityConsumption, short distance, int energyRequirement, int leadershipRequirement, int elementalModifier, int attackType, int useType, int count, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel, int ragefighterClassLevel, int rank, int group, int masterp, SkillType skillType = SkillType.DirectHit)
        {
            var skill = this.Context.CreateNew<Skill>();
            this.GameConfiguration.Skills.Add(skill);
            skill.SkillID = skillId;
            skill.Name = name;
            if (levelRequirement > 0)
            {
                var requirement = this.Context.CreateNew<AttributeRequirement>();
                requirement.Attribute = Stats.Level.GetPersistent(this.GameConfiguration);
                requirement.MinimumValue = levelRequirement;
                skill.Requirements.Add(requirement);
            }

            if (leadershipRequirement > 0)
            {
                var requirement = this.Context.CreateNew<AttributeRequirement>();
                requirement.Attribute = Stats.TotalLeadership.GetPersistent(this.GameConfiguration);
                requirement.MinimumValue = leadershipRequirement;
                skill.Requirements.Add(requirement);
            }

            if (energyRequirement > 0)
            {
                var requirement = this.Context.CreateNew<AttributeRequirement>();
                requirement.Attribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
                requirement.MinimumValue = energyRequirement;
                skill.Requirements.Add(requirement);
            }

            if (damage > 0)
            {
                var levelDependentDamage = this.Context.CreateNew<LevelDependentDamage>();
                levelDependentDamage.Damage = damage;
                levelDependentDamage.Level = 0; // TODO: or is it 1?
                skill.AttackDamage.Add(levelDependentDamage);
            }

            if (manaConsumption > 0)
            {
                var requirement = this.Context.CreateNew<AttributeRequirement>();
                requirement.Attribute = Stats.CurrentMana.GetPersistent(this.GameConfiguration);
                requirement.MinimumValue = manaConsumption;
                skill.ConsumeRequirements.Add(requirement);
            }

            if (abilityConsumption > 0)
            {
                var requirement = this.Context.CreateNew<AttributeRequirement>();
                requirement.Attribute = Stats.CurrentAbility.GetPersistent(this.GameConfiguration);
                requirement.MinimumValue = manaConsumption;
                skill.ConsumeRequirements.Add(requirement);
            }

            skill.Range = distance;
            skill.DamageType = attackType == 1 ? DamageType.Wizardry : DamageType.Physical;
            skill.SkillType = skillType;
            var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, ragefighterClassLevel);
            foreach (var characterClass in classes)
            {
                skill.QualifiedCharacters.Add(characterClass);
            }

            // TODO: Master skill related stuff?
        }
    }
}
