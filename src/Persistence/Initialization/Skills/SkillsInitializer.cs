// <copyright file="SkillsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

    /// <summary>
    /// Initialization logic for <see cref="Skill"/>s.
    /// </summary>
    internal class SkillsInitializer : InitializerBase
    {
        private const string Formula1204 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 10"; // 17
        private const string Formula61408 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85 * 6"; // 12
        private const string Formula51173 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85 * 5"; // 13
        private const string Formula181 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 1.5"; // 7
        private const string FormulaRecoveryIncrease181 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 1.5 * 0.01"; // 7
        private const string Formula120 = "1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)"; // 1
        private const string FormulaRecoveryIncrease120 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) / 100"; // 1
        private const string FormulaIncreaseMultiplicator120 = "(101 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) / 100"; // 1
        private const string Formula6020 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 50"; // 16
        private const string Formula502 = "(0.8 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 5";
        private const string Formula632 = "(0,85 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 6"; // 3
        private const string Formula883 = "(0.9 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 8"; // 4
        private const string Formula10235 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85"; // 9
        private const string Formula81877 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85 * 8"; // 14
        private const string Formula1154 = "(0.95 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 10"; // 5
        private const string Formula803 = "(0.8 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 8"; // 10
        private const string Formula1 = "1 * level";
        private const string Formula1WhenComplete = "if(level < 10; 0; 1)";
        private const string Formula722 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 6"; // 18
        private const string Formula4319 = "52 / (1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6))))"; // 6
        private const string Formula914 = "11 / (1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12))"; // 11
        private const string Formula3822 = "40 / (1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) + 5"; // 20
        private const string Formula25587 = "(1 + ( ( ( ( ( ( (level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85 * 2.5"; // 29
        private const string Formula30704 = "(1 + ( ( ( ( ( ((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85 * 3"; // 33
        private const string Formula3371 = "(1 + ( ( ( ( ( ( (level - 30) ^ 3) + 25000) / 499) / 6) ) ) ) * 28"; // 35
        private const string Formula20469 = "(1 + ( ( ( ( ( ( (level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12) ) * 85 * 2"; // 30
        private const string Formula1806 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 6)))) * 15"; // 15
        private const string Formula32751 = "(1 + ( ( ( ( ( ( (level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 85 * 3.2"; // 31
        private const string Formula5418 = "(1 + ( ( ( ( ( ( (level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12)) * 45"; // 34

        private static readonly IDictionary<SkillNumber, MagicEffectNumber> EffectsOfSkills = new Dictionary<SkillNumber, MagicEffectNumber>
        {
            { SkillNumber.SwellLife, MagicEffectNumber.GreaterFortitude },
            { SkillNumber.SoulBarrier, MagicEffectNumber.SoulBarrier },
            { SkillNumber.Defense, MagicEffectNumber.ShieldSkill },
            { SkillNumber.GreaterDefense, MagicEffectNumber.GreaterDefense },
            { SkillNumber.GreaterDamage, MagicEffectNumber.GreaterDamage },
            { SkillNumber.Heal, MagicEffectNumber.Heal },
            { SkillNumber.Recovery, MagicEffectNumber.ShieldRecover },
        };

        private IDictionary<byte, MasterSkillRoot> masterSkillRoots;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillsInitializer"/> class.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public SkillsInitializer(IContext context, GameConfiguration gameConfiguration)
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
            this.CreateSkill(SkillNumber.Poison, "Poison", 0, 12, 42, 0, 6, 140, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.Meteorite, "Meteorite", 0, 21, 12, 0, 6, 104, 0, 4, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0);
            this.CreateSkill(SkillNumber.Lightning, "Lightning", 0, 17, 15, 0, 6, 72, 0, 2, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.FireBall, "Fire Ball", 0, 8, 3, 0, 6, 40, 0, 3, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0);
            this.CreateSkill(SkillNumber.Flame, "Flame", 0, 25, 50, 0, 6, 160, 0, 3, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.Teleport, "Teleport", 0, 0, 30, 0, 6, 88, 0, -1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.Ice, "Ice", 0, 10, 38, 0, 6, 120, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0);
            this.CreateSkill(SkillNumber.Twister, "Twister", 0, 35, 60, 0, 6, 180, 0, 5, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.EvilSpirit, "Evil Spirit", 0, 45, 90, 0, 6, 220, 0, -1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.Hellfire, "Hellfire", 0, 120, 160, 0, 0, 260, 0, 3, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.PowerWave, "Power Wave", 0, 14, 5, 0, 6, 56, 0, -1, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0);
            this.CreateSkill(SkillNumber.AquaBeam, "Aqua Beam", 0, 80, 140, 0, 6, 345, 0, 6, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.Cometfall, "Cometfall", 0, 70, 150, 0, 3, 436, 0, 2, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.Inferno, "Inferno", 0, 100, 200, 0, 0, 578, 0, 3, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.TeleportAlly, "Teleport Ally", 0, 0, 90, 25, 6, 644, 0, -1, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SoulBarrier, "Soul Barrier", 0, 0, 70, 22, 6, 408, 0, -1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.Buff, SkillTarget.Explicit, 0, SkillTargetRestriction.Party);
            this.CreateSkill(SkillNumber.EnergyBall, "Energy Ball", 0, 3, 1, 0, 6, 0, 0, -1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.Defense, "Defense", 0, 0, 30, 0, 0, 0, 0, -1, -1, 0, 0, 0, 1, 0, 1, 1, 0, 0, SkillType.Buff, SkillTarget.Explicit, 0, SkillTargetRestriction.Self);
            this.CreateSkill(SkillNumber.FallingSlash, "Falling Slash", 0, 0, 9, 0, 3, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Lunge, "Lunge", 0, 0, 9, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Uppercut, "Uppercut", 0, 0, 8, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Cyclone, "Cyclone", 0, 0, 9, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Slash, "Slash", 0, 0, 10, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.TripleShot, "Triple Shot", 0, 0, 5, 0, 6, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.Heal, "Heal", 0, 0, 20, 0, 6, 52, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, SkillType.Regeneration, targetRestriction: SkillTargetRestriction.Player);
            this.CreateSkill(SkillNumber.GreaterDefense, "Greater Defense", 0, 0, 30, 0, 6, 72, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, SkillType.Buff, targetRestriction: SkillTargetRestriction.Player);
            this.CreateSkill(SkillNumber.GreaterDamage, "Greater Damage", 0, 0, 40, 0, 6, 92, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, SkillType.Buff, targetRestriction: SkillTargetRestriction.Player);
            this.CreateSkill(SkillNumber.SummonGoblin, "Summon Goblin", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonStoneGolem, "Summon Stone Golem", 0, 0, 70, 0, 0, 170, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonAssassin, "Summon Assassin", 0, 0, 110, 0, 0, 190, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonEliteYeti, "Summon Elite Yeti", 0, 0, 160, 0, 0, 230, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonDarkKnight, "Summon Dark Knight", 0, 0, 200, 0, 0, 250, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonBali, "Summon Bali", 0, 0, 250, 0, 0, 260, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonSoldier, "Summon Soldier", 0, 0, 350, 0, 0, 280, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.Decay, "Decay", 0, 95, 110, 7, 6, 953, 0, 1, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.IceStorm, "Ice Storm", 0, 80, 100, 5, 6, 849, 0, 0, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.Nova, "Nova", 100, 0, 180, 45, 6, 1052, 0, 3, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.TwistingSlash, "Twisting Slash", 0, 0, 10, 10, 2, 0, 0, 5, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.RagefulBlow, "Rageful Blow", 170, 60, 25, 20, 3, 0, 0, 4, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.DeathStab, "Death Stab", 160, 70, 15, 12, 2, 0, 0, 5, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, SkillType.DirectHit, SkillTarget.ExplicitWithImplicitInRange, 1);
            this.CreateSkill(SkillNumber.CrescentMoonSlash, "Crescent Moon Slash", 0, 90, 22, 15, 4, 0, 0, -1, -1, 0, 0, 0, 1, 0, 0, 0, 0, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Lance, "Lance", 0, 90, 150, 10, 6, 0, 0, -1, -1, 0, 0, 1, 0, 0, 0, 0, 1, 0);
            this.CreateSkill(SkillNumber.Starfall, "Starfall", 0, 120, 20, 15, 8, 0, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Impale, "Impale", 28, 15, 8, 0, 3, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.SwellLife, "Swell Life", 120, 0, 22, 24, 0, 0, 0, -1, -1, 0, 0, 0, 1, 0, 0, 0, 0, 0, SkillType.Buff, SkillTarget.ImplicitParty);
            this.CreateSkill(SkillNumber.FireBreath, "Fire Breath", 110, 30, 9, 0, 3, 0, 0, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.FlameofEvil, "Flame of Evil (Monster)", 60, 120, 160, 0, 0, 100, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.IceArrow, "Ice Arrow", 0, 105, 10, 12, 8, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Penetration, "Penetration", 130, 70, 7, 9, 6, 0, 0, 5, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.FireSlash, "Fire Slash", 0, 80, 15, 20, 2, 0, 0, 1, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.PowerSlash, "Power Slash", 0, 0, 15, 0, 5, 100, 0, -1, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.SpiralSlash, "Spiral Slash", 0, 75, 20, 15, 5, 0, 0, -1, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.Force, "Force", 0, 10, 10, 0, 4, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);
            this.CreateSkill(SkillNumber.FireBurst, "Fire Burst", 0, 100, 25, 0, 6, 79, 0, -1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, SkillType.DirectHit, SkillTarget.ExplicitWithImplicitInRange, 1);
            this.CreateSkill(SkillNumber.Earthshake, "Earthshake", 0, 150, 0, 50, 10, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.Summon, "Summon", 0, 0, 70, 30, 0, 153, 400, -1, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.IncreaseCriticalDamage, "Increase Critical Damage", 0, 0, 50, 50, 0, 102, 300, -1, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, SkillType.Buff);
            this.CreateSkill(SkillNumber.ElectricSpike, "Electric Spike", 0, 250, 0, 100, 10, 126, 340, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.ForceWave, "Force Wave", 0, 50, 10, 0, 4, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0);
            this.CreateSkill(SkillNumber.Stun, "Stun", 0, 0, 70, 50, 2, 0, 0, -1, -1, 1, 4, 1, 1, 1, 1, 1, 1, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.CancelStun, "Cancel Stun", 0, 0, 25, 30, 0, 0, 0, -1, -1, 1, 2, 1, 1, 1, 1, 1, 1, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SwellMana, "Swell Mana", 0, 0, 35, 30, 0, 0, 0, -1, -1, 1, 4, 1, 1, 1, 1, 1, 1, 0);
            this.CreateSkill(SkillNumber.Invisibility, "Invisibility", 0, 0, 80, 60, 0, 0, 0, -1, -1, 1, 5, 1, 1, 1, 1, 1, 1, 0);
            this.CreateSkill(SkillNumber.CancelInvisibility, "Cancel Invisibility", 0, 0, 40, 30, 0, 0, 0, -1, -1, 1, 2, 1, 1, 1, 1, 1, 1, 0);
            this.CreateSkill(SkillNumber.AbolishMagic, "Abolish Magic", 0, 0, 90, 70, 0, 0, 0, -1, -1, 1, 8, 0, 0, 0, 0, 1, 0, 0);
            this.CreateSkill(SkillNumber.ManaRays, "Mana Rays", 0, 85, 130, 7, 6, 0, 0, -1, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.FireBlast, "Fire Blast", 0, 150, 30, 10, 6, 0, 0, -1, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0);
            this.CreateSkill(SkillNumber.PlasmaStorm, "Plasma Storm", 110, 60, 50, 20, 6, 0, 0, -1, 0, 0, 0, 2, 2, 2, 1, 1, 2, 1, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.InfinityArrow, "Infinity Arrow", 220, 0, 50, 10, 6, 0, 0, -1, -1, 0, 0, 0, 0, 2, 0, 0, 0, 0, SkillType.Buff);
            this.CreateSkill(SkillNumber.FireScream, "Fire Scream", 0, 130, 45, 10, 6, 70, 150, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.Explosion79, "Explosion", 0, 0, 0, 0, 2, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0);
            this.CreateSkill(SkillNumber.SummonMonster, "Summon Monster", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MagicAttackImmunity, "Magic Attack Immunity", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.PhysicalAttackImmunity, "Physical Attack Immunity", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.PotionofBless, "Potion of Bless", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.PotionofSoul, "Potion of Soul", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SpellofProtection, "Spell of Protection", 0, 0, 30, 0, 0, 0, 0, 0, 0, 0, 5, 1, 1, 1, 1, 1, 1, 0);
            this.CreateSkill(SkillNumber.SpellofRestriction, "Spell of Restriction", 0, 0, 30, 0, 3, 0, 0, 0, 0, 0, 5, 1, 1, 1, 1, 1, 1, 0);
            this.CreateSkill(SkillNumber.SpellofPursuit, "Spell of Pursuit", 0, 0, 30, 0, 0, 0, 0, 0, 0, 0, 10, 1, 1, 1, 1, 1, 1, 0);
            this.CreateSkill(SkillNumber.ShieldBurn, "Shield-Burn", 0, 0, 30, 0, 3, 0, 0, 0, 0, 0, 5, 1, 1, 1, 1, 1, 1, 0);
            this.CreateSkill(SkillNumber.DrainLife, "Drain Life", 0, 35, 50, 0, 6, 150, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0);
            this.CreateSkill(SkillNumber.ChainLightning, "Chain Lightning", 0, 70, 85, 0, 6, 245, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0);
            this.CreateSkill(SkillNumber.DamageReflection, "Damage Reflection", 0, 0, 40, 10, 5, 375, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0);
            this.CreateSkill(SkillNumber.Berserker, "Berserker", 0, 0, 100, 50, 5, 620, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0);
            this.CreateSkill(SkillNumber.Sleep, "Sleep", 0, 0, 20, 3, 6, 180, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0);
            this.CreateSkill(SkillNumber.Weakness, "Weakness", 0, 0, 50, 15, 6, 663, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0);
            this.CreateSkill(SkillNumber.Innovation, "Innovation", 0, 0, 70, 15, 6, 912, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0);
            this.CreateSkill(SkillNumber.Explosion223, "Explosion", 0, 40, 90, 5, 6, 100, 0, 3, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0);
            this.CreateSkill(SkillNumber.Requiem, "Requiem", 0, 65, 110, 10, 6, 99, 0, 5, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0);
            this.CreateSkill(SkillNumber.Pollution, "Pollution", 0, 80, 120, 15, 6, 115, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0);
            this.CreateSkill(SkillNumber.LightningShock, "Lightning Shock", 0, 95, 115, 7, 6, 823, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0);
            this.CreateSkill(SkillNumber.StrikeofDestruction, "Strike of Destruction", 100, 110, 30, 24, 5, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.ExpansionofWizardry, "Expansion of Wizardry", 220, 0, 200, 50, 6, 118, 0, -1, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Recovery, "Recovery", 100, 0, 40, 10, 6, 37, 0, -1, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, SkillType.Regeneration, targetRestriction: SkillTargetRestriction.Player);
            this.CreateSkill(SkillNumber.MultiShot, "Multi-Shot", 100, 40, 10, 7, 6, 0, 0, -1, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.FlameStrike, "Flame Strike", 100, 140, 20, 25, 3, 0, 0, 3, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.GiganticStorm, "Gigantic Storm", 220, 110, 120, 10, 6, 118, 0, 5, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.ChaoticDiseier, "Chaotic Diseier", 100, 190, 50, 15, 6, 16, 0, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.DoppelgangerSelfExplosion, "Doppelganger Self Explosion", 100, 140, 20, 25, 3, 0, 0, 3, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.KillingBlow, "Killing Blow", 0, 0, 9, 0, 2, 0, 0, 4, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            this.CreateSkill(SkillNumber.BeastUppercut, "Beast Uppercut", 0, 0, 9, 0, 2, 0, 0, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            this.CreateSkill(SkillNumber.ChainDrive, "Chain Drive", 150, 0, 15, 20, 4, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            this.CreateSkill(SkillNumber.DarkSide, "Dark Side", 180, 0, 70, 0, 4, 0, 0, 5, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            this.CreateSkill(SkillNumber.DragonRoar, "Dragon Roar", 150, 0, 50, 30, 3, 0, 0, 4, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            this.CreateSkill(SkillNumber.DragonSlasher, "Dragon Slasher", 200, 0, 100, 100, 4, 0, 0, 5, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            this.CreateSkill(SkillNumber.IgnoreDefense, "Ignore Defense", 120, 0, 50, 10, 3, 404, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            this.CreateSkill(SkillNumber.IncreaseHealth, "Increase Health", 80, 0, 50, 10, 7, 132, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            this.CreateSkill(SkillNumber.IncreaseBlock, "Increase Block", 50, 0, 50, 10, 7, 80, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            this.CreateSkill(SkillNumber.Charge, "Charge", 0, 90, 20, 15, 4, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            this.CreateSkill(SkillNumber.PhoenixShot, "Phoenix Shot", 0, 0, 30, 0, 4, 0, 0, 4, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1);

            // Master skills:
            // Common:
            this.CreateSkill(SkillNumber.DurabilityReduction1, "Durability Reduction (1)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.PvPDefenceRateInc, "PvP Defence Rate Inc", 0, 12, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.MaximumSDincrease, "Maximum SD increase", 0, 13, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.AutomaticManaRecInc, "Automatic Mana Rec Inc", 0, 7, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.PoisonResistanceInc, "Poison Resistance Inc", 0, 1, 0, 0, 0, 0, 0, 1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.DurabilityReduction2, "Durability Reduction (2)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.SDRecoverySpeedInc, "SD Recovery Speed Inc", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.AutomaticHPRecInc, "Automatic HP Rec Inc", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.LightningResistanceInc, "Lightning Resistance Inc", 0, 1, 0, 0, 0, 0, 0, 2, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.DefenseIncrease, "Defense Increase", 0, 16, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.AutomaticAGRecInc, "Automatic AG Rec Inc", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.IceResistanceIncrease, "Ice Resistance Increase", 0, 1, 0, 0, 0, 0, 0, 0, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.DurabilityReduction3, "Durability Reduction (3)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.DefenseSuccessRateInc, "Defense Success Rate Inc", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.CastInvincibility, "Cast Invincibility", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.ArmorSetBonusInc, "Armor Set Bonus Inc", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.Vengeance, "Vengeance", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.EnergyIncrease, "Energy Increase", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.StaminaIncrease, "Stamina Increase", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.AgilityIncrease, "Agility Increase", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.StrengthIncrease, "Strength Increase", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.MaximumLifeIncrease, "Maximum Life Increase", 0, 9, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.ManaReduction, "Mana Reduction", 0, 18, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.MonsterAttackSDInc, "Monster Attack SD Inc", 0, 11, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.MonsterAttackLifeInc, "Monster Attack Life Inc", 0, 6, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.SwellLifeProficiency, "Swell Life Proficiency", 120, 7, 26, 28, 0, 0, 0, -1, -1, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MinimumAttackPowerInc, "Minimum Attack Power Inc", 0, 22, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 3, 0, 0);
            this.CreateSkill(SkillNumber.MonsterAttackManaInc, "Monster Attack Mana Inc", 0, 6, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.SwellLifeMastery, "Swell Life Mastery", 120, 7, 28, 30, 0, 0, 0, -1, -1, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MaximumAttackPowerInc, "Maximum Attack Power Inc", 0, 3, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 3, 0, 0);
            this.CreateSkill(SkillNumber.Inccritdamagerate, "Inc crit damage rate", 0, 7, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.RestoresallMana, "Restores all Mana", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.RestoresallHP, "Restores all HP", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.Incexcdamagerate, "Inc exc damage rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.Incdoubledamagerate, "Inc double damage rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.IncchanceofignoreDef, "Inc chance of ignore Def", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.RestoresallSD, "Restores all SD", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.Inctripledamagerate, "Inc triple damage rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.PvPAttackRate, "PvP Attack Rate", 0, 14, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);

            // Blade Master:
            this.CreateSkill(SkillNumber.WingofStormAbsPowUp, "Wing of Storm Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.WingofStormDefPowUp, "Wing of Storm Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.IronDefense, "Iron Defense", 0, 1, 0, 0, 0, 0, 0, -1, -1, 4, 0, 3, 3, 3, 3, 3, 3, 3);
            this.CreateSkill(SkillNumber.WingofStormAttPowUp, "Wing of Storm Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.AttackSuccRateInc, "Attack Succ Rate Inc", 0, 13, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.CycloneStrengthener, "Cyclone Strengthener", 0, 22, 9, 0, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SlashStrengthener, "Slash Strengthener", 0, 3, 10, 0, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.FallingSlashStreng, "Falling Slash Streng", 0, 3, 9, 0, 3, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.LungeStrengthener, "Lunge Strengthener", 0, 3, 9, 0, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.TwistingSlashStreng, "Twisting Slash Streng", 0, 3, 10, 10, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.RagefulBlowStreng, "Rageful Blow Streng", 170, 22, 25, 22, 3, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.TwistingSlashMastery, "Twisting Slash Mastery", 0, 1, 22, 20, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.RagefulBlowMastery, "Rageful Blow Mastery", 170, 1, 50, 30, 3, 0, 0, 4, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.WeaponMasteryBladeMaster, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.DeathStabStrengthener, "Death Stab Strengthener", 160, 22, 15, 13, 2, 0, 0, 5, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.StrikeofDestrStr, "Strike of Destr Str", 100, 22, 30, 24, 5, 0, 0, 0, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MaximumManaIncrease, "Maximum Mana Increase", 0, 9, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.DeathStabProficiency, "Death Stab Proficiency", 160, 7, 30, 26, 2, 0, 0, 5, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.StrikeofDestrProf, "Strike of Destr Prof", 100, 7, 30, 24, 5, 0, 0, 0, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MaximumAGIncrease, "Maximum AG Increase", 0, 8, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0);
            this.CreateSkill(SkillNumber.DeathStabMastery, "Death Stab Mastery", 160, 7, 30, 26, 2, 0, 0, 5, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.StrikeofDestrMast, "Strike of Destr Mast", 100, 1, 30, 24, 5, 0, 0, 0, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.BloodStorm, "Blood Storm", 0, 25, 87, 29, 3, 0, 0, -1, 0, 4, 0, 0, 3, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.ComboStrengthener, "Combo Strengthener", 0, 7, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.BloodStormStrengthener, "Blood Storm Strengthener", 0, 22, 87, 29, 3, 0, 0, -1, 0, 4, 0, 0, 3, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.TwoHandedSwordStrengthener, "Two-handed Sword Stren", 0, 4, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.OneHandedSwordStrengthener, "One-handed Sword Stren", 0, 22, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.MaceStrengthener, "Mace Strengthener", 0, 3, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SpearStrengthener, "Spear Strengthener", 0, 3, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.TwoHandedSwordMaster, "Two-handed Sword Mast", 0, 5, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.OneHandedSwordMaster, "One-handed Sword Mast", 0, 23, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.MaceMastery, "Mace Mastery", 0, 1, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SpearMastery, "Spear Mastery", 0, 1, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SwellLifeStrengt, "Swell Life Strengt", 120, 7, 24, 26, 0, 0, 0, -1, -1, 4, 0, 0, 3, 0, 0, 0, 0, 0);

            // Grand Master:
            this.CreateSkill(SkillNumber.EternalWingsAbsPowUp, "Eternal Wings Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.EternalWingsDefPowUp, "Eternal Wings Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.EternalWingsAttPowUp, "Eternal Wings Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.FlameStrengthener, "Flame Strengthener", 35, 3, 55, 0, 6, 100, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.LightningStrengthener, "Lightning Strengthener", 13, 3, 20, 0, 6, 100, 0, 2, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.ExpansionofWizStreng, "Expansion of Wiz Streng", 220, 1, 220, 55, 6, 118, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.InfernoStrengthener, "Inferno Strengthener", 88, 22, 220, 0, 0, 200, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.BlastStrengthener, "Blast Strengthener", 80, 22, 165, 0, 3, 150, 0, 2, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.ExpansionofWizMas, "Expansion of Wiz Mas", 220, 1, 220, 55, 6, 118, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.PoisonStrengthener, "Poison Strengthener", 30, 3, 46, 0, 6, 100, 0, 1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.EvilSpiritStreng, "Evil Spirit Streng", 50, 22, 108, 0, 6, 100, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MagicMasteryGrandMaster, "Magic Mastery", 50, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.DecayStrengthener, "Decay Strengthener", 96, 22, 120, 10, 6, 243, 0, 1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.HellfireStrengthener, "Hellfire Strengthener", 60, 3, 176, 0, 0, 100, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.IceStrengthener, "Ice Strengthener", 25, 3, 42, 0, 6, 100, 0, 0, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MeteorStrengthener, "Meteor Strengthener", 21, 4, 13, 0, 6, 100, 0, 4, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.IceStormStrengthener, "Ice Storm Strengthener", 93, 22, 110, 5, 6, 223, 0, 0, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.NovaStrengthener, "Nova Strengthener", 100, 22, 198, 49, 6, 258, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.IceStormMastery, "Ice Storm Mastery", 93, 1, 110, 5, 6, 223, 0, 0, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MeteorMastery, "Meteor Mastery", 21, 1, 14, 0, 6, 100, 0, 4, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.NovaCastStrengthener, "Nova Cast Strengthener", 100, 22, 198, 49, 6, 258, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.OneHandedStaffStrengthener, "One-handed Staff Stren", 0, 22, 0, 0, 0, 0, 0, -1, 1, 3, 0, 3, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.TwoHandedStaffStrengthener, "Two-handed Staff Stren", 0, 4, 0, 0, 0, 0, 0, -1, 1, 3, 0, 3, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.ShieldStrengthenerGrandMaster, "Shield Strengthener", 0, 10, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.OneHandedStaffMaster, "One-handed Staff Mast", 0, 23, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.TwoHandedStaffMaster, "Two-handed Staff Mast", 0, 5, 0, 0, 0, 0, 0, -1, 1, 3, 0, 3, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.ShieldMasteryGrandMaster, "Shield Mastery", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SoulBarrierStrength, "Soul Barrier Strength", 77, 7, 77, 24, 6, 126, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SoulBarrierProficie, "Soul Barrier Proficie", 77, 10, 84, 26, 6, 126, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MinimumWizardryInc, "Minimum Wizardry Inc", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.SoulBarrierMastery, "Soul Barrier Mastery", 77, 7, 92, 28, 6, 126, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MaximumWizardryInc, "Maximum Wizardry Inc", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 3, 0, 0, 0);

            // High Elf:
            this.CreateSkill(SkillNumber.IllusionWingsAbsPowUp, "Illusion Wings Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.IllusionWingsDefPowUp, "Illusion Wings Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MultiShotStreng, "Multi-Shot Streng", 100, 22, 11, 7, 6, 0, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.IllusionWingsAttPowUp, "Illusion Wings Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.HealStrengthener, "Heal Strengthener", 8, 22, 22, 0, 6, 100, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.TripleShotStrengthener, "Triple Shot Strengthener", 0, 22, 5, 0, 6, 0, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SummonedMonsterStr1, "Summoned Monster Str (1)", 0, 16, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.PenetrationStrengthener, "Penetration Strengthener", 130, 22, 10, 11, 6, 0, 0, 5, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.DefenseIncreaseStr, "Defense Increase Str", 13, 22, 33, 0, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.TripleShotMastery, "Triple Shot Mastery", 0, 0, 9, 0, 6, 0, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SummonedMonsterStr2, "Summoned Monster Str (2)", 0, 16, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.AttackIncreaseStr, "Attack Increase Str", 18, 22, 44, 0, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.WeaponMasteryHighElf, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.AttackIncreaseMastery, "Attack Increase Mastery", 18, 22, 48, 0, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.DefenseIncreaseMastery, "Defense Increase Mastery", 13, 22, 36, 0, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.IceArrowStrengthener, "Ice Arrow Strengthener", 0, 22, 15, 18, 8, 0, 0, 0, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Cure, "Cure", 0, 0, 72, 10, 6, 0, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.PartyHealing, "Party Healing", 0, 0, 66, 12, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.PoisonArrow, "Poison Arrow", 0, 27, 22, 27, 6, 0, 0, 1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SummonedMonsterStr3, "Summoned Monster Str (3)", 0, 16, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.PartyHealingStr, "Party Healing Str", 0, 22, 72, 13, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Bless, "Bless", 0, 0, 108, 18, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MultiShotMastery, "Multi-Shot Mastery", 100, 1, 12, 8, 6, 0, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SummonSatyros, "Summon Satyros", 0, 0, 525, 52, 0, 280, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.BlessStrengthener, "Bless Strengthener", 0, 10, 118, 20, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.PoisonArrowStr, "Poison Arrow Str", 0, 22, 24, 29, 6, 0, 0, 1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.BowStrengthener, "Bow Strengthener", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.CrossbowStrengthener, "Crossbow Strengthener", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.ShieldStrengthenerHighElf, "Shield Strengthener", 0, 10, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.BowMastery, "Bow Mastery", 0, 23, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.CrossbowMastery, "Crossbow Mastery", 0, 5, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.ShieldMasteryHighElf, "Shield Mastery", 0, 15, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.InfinityArrowStr, "Infinity Arrow Str", 220, 1, 55, 11, 6, 0, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MinimumAttPowerInc, "Minimum Att Power Inc", 0, 22, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 0, 3, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MaximumAttPowerInc, "Maximum Att Power Inc", 0, 3, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 0, 3, 0, 0, 0, 0);

            // Dimension Master (Summoner):
            this.CreateSkill(SkillNumber.DimensionWingsAbsPowUp, "DimensionWings Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.DimensionWingsDefPowUp, "DimensionWings Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.DimensionWingsAttPowUp, "DimensionWings Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.FireTomeStrengthener, "Fire Tome Strengthener", 0, 3, 0, 0, 0, 0, 0, 3, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.WindTomeStrengthener, "Wind Tome Strengthener", 0, 3, 0, 0, 0, 0, 0, 5, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.LightningTomeStren, "Lightning Tome Stren", 0, 3, 0, 0, 0, 0, 0, 2, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.FireTomeMastery, "Fire Tome Mastery", 0, 7, 0, 0, 0, 0, 0, 3, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.WindTomeMastery, "Wind Tome Mastery", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.LightningTomeMastery, "Lightning Tome Mastery", 0, 7, 0, 0, 0, 0, 0, 2, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.SleepStrengthener, "Sleep Strengthener", 40, 1, 30, 7, 6, 100, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.ChainLightningStr, "Chain Lightning Str", 75, 22, 103, 0, 6, 75, 0, 2, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.LightningShockStr, "Lightning Shock Str", 93, 22, 125, 10, 6, 216, 0, 2, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.MagicMasterySummoner, "Magic Mastery", 0, 22, 0, 0, 0, 0, 0, -1, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.DrainLifeStrengthener, "Drain Life Strengthener", 35, 22, 57, 0, 6, 93, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.WeaknessStrengthener, "Weakness Strengthener", 93, 3, 55, 17, 6, 173, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.InnovationStrengthener, "Innovation Strengthener", 111, 3, 77, 17, 6, 201, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.Blind, "Blind", 0, 0, 115, 25, 3, 201, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.DrainLifeMastery, "Drain Life Mastery", 35, 17, 62, 0, 6, 93, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.BlindStrengthener, "Blind Strengthener", 0, 1, 126, 27, 3, 201, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.StickStrengthener, "Stick Strengthener", 0, 22, 0, 0, 0, 0, 0, -1, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.OtherWorldTomeStreng, "Other World Tome Streng", 0, 3, 0, 0, 0, 0, 0, -1, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.StickMastery, "Stick Mastery", 0, 5, 0, 0, 0, 0, 0, -1, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.OtherWorldTomeMastery, "Other World Tome Mastery", 0, 23, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.BerserkerStrengthener, "Berserker Strengthener", 83, 7, 150, 75, 5, 181, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.BerserkerProficiency, "Berserker Proficiency", 83, 7, 165, 82, 5, 181, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0);

            // Duel Master (MG):
            this.CreateSkill(SkillNumber.MinimumWizCurseInc, "Minimum Wiz/Curse Inc", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.BerserkerMastery, "Berserker Mastery", 83, 10, 181, 90, 5, 181, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.MaximumWizCurseInc, "Maximum Wiz/Curse Inc", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0);
            this.CreateSkill(SkillNumber.WingofRuinAbsPowUp, "Wing of Ruin Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.WingofRuinDefPowUp, "Wing of Ruin Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.WingofRuinAttPowUp, "Wing of Ruin Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.CycloneStrengthenerDuelMaster, "Cyclone Strengthener", 0, 22, 9, 0, 2, 0, 0, -1, 0, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.LightningStrengthenerDuelMaster, "Lightning Strengthener", 13, 3, 20, 0, 6, 100, 0, 2, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.TwistingSlashStrengthenerDuelMaster, "Twisting Slash Stren", 0, 3, 10, 10, 2, 0, 0, -1, 0, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.PowerSlashStreng, "Power Slash Streng", 0, 3, 15, 0, 5, 100, 0, -1, -1, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.FlameStrengthenerDuelMaster, "Flame Strengthener", 35, 3, 55, 0, 6, 100, 0, 3, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.BlastStrengthenerDuelMaster, "Blast Strengthener", 80, 22, 165, 0, 3, 150, 0, 2, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.WeaponMasteryDuelMaster, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.InfernoStrengthenerDuelMaster, "Inferno Strengthener", 88, 22, 220, 0, 0, 200, 0, 3, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.EvilSpiritStrengthenerDuelMaster, "Evil Spirit Strengthen", 50, 22, 108, 0, 6, 100, 0, -1, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.MagicMasteryDuelMaster, "Magic Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.IceStrengthenerDuelMaster, "Ice Strengthener", 25, 3, 42, 0, 6, 100, 0, 0, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.BloodAttackStrengthen, "Blood Attack Strengthen", 0, 22, 15, 22, 3, 0, 0, 1, -1, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.IceMasteryDuelMaster, "Ice Mastery", 25, 1, 46, 0, 6, 100, 0, 0, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.FlameStrikeStrengthen, "Flame Strike Strengthen", 0, 22, 30, 37, 3, 0, 0, 3, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.FireSlashMastery, "Fire Slash Mastery", 0, 7, 17, 24, 3, 0, 0, 1, -1, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.FlameStrikeMastery, "Flame Strike Mastery", 0, 7, 33, 40, 3, 0, 0, 3, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.EarthPrison, "Earth Prison", 0, 26, 180, 15, 3, 127, 0, 4, 1, 4, 0, 3, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.GiganticStormStr, "Gigantic Storm Str", 220, 22, 132, 11, 6, 118, 0, 5, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0);
            this.CreateSkill(SkillNumber.EarthPrisonStr, "Earth Prison Str", 0, 22, 198, 17, 3, 127, 0, 4, 1, 4, 0, 3, 0, 0, 3, 0, 0, 0);

            // Lord Emperor (DL):
            this.CreateSkill(SkillNumber.EmperorCapeAbsPowUp, "Emperor Cape Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.EmperorCapeDefPowUp, "Emperor Cape Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.AddsCommandStat, "Adds Command Stat", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.EmperorCapeAttPowUp, "Emperor Cape Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.FireBurstStreng, "Fire Burst Streng", 74, 22, 25, 0, 6, 20, 0, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.ForceWaveStreng, "Force Wave Streng", 0, 3, 15, 0, 4, 0, 0, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.DarkHorseStreng1, "Dark Horse Streng (1)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.CriticalDMGIncPowUp, "Critical DMG Inc PowUp", 82, 3, 75, 75, 0, 25, 300, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.EarthshakeStreng, "Earthshake Streng", 0, 22, 0, 75, 10, 0, 0, 2, 0, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.WeaponMasteryLordEmperor, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.FireBurstMastery, "Fire Burst Mastery", 74, 1, 27, 0, 6, 20, 0, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.CritDMGIncPowUp2, "Crit DMG Inc PowUp (2)", 82, 10, 82, 82, 0, 25, 300, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.EarthshakeMastery, "Earthshake Mastery", 0, 1, 0, 75, 10, 0, 0, 2, 0, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.CritDMGIncPowUp3, "Crit DMG Inc PowUp (3)", 82, 7, 100, 100, 0, 25, 300, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.FireScreamStren, "Fire Scream Stren", 102, 22, 45, 11, 6, 32, 70, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.ElectricSparkStreng, "Electric Spark Streng", 92, 3, 0, 150, 10, 29, 340, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.FireScreamMastery, "Fire Scream Mastery", 102, 5, 49, 12, 6, 32, 70, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.IronDefenseLordEmperor, "Iron Defense", 0, 28, 64, 29, 0, 0, 0, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.CriticalDamageIncM, "Critical Damage Inc M", 82, 1, 110, 110, 0, 25, 300, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.ChaoticDiseierStr, "Chaotic Diseier Str", 100, 22, 75, 22, 6, 16, 0, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.IronDefenseStr, "Iron Defense Str", 0, 3, 70, 31, 0, 0, 0, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.DarkSpiritStr, "Dark Spirit Str", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.ScepterStrengthener, "Scepter Strengthener", 0, 22, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.ShieldStrengthenerLordEmperor, "Shield Strengthener", 0, 10, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.UseScepterPetStr, "Use Scepter : Pet Str", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.DarkSpiritStr2, "Dark Spirit Str (2)", 0, 7, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.ScepterMastery, "Scepter Mastery", 0, 5, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.ShieldMastery, "Shield Mastery", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.CommandAttackInc, "Command Attack Inc", 0, 20, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.DarkSpiritStr3, "Dark Spirit Str (3)", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.PetDurabilityStr, "Pet Durability Str", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.DarkSpiritStr4, "Dark Spirit Str (4)", 0, 23, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.DarkSpiritStr5, "Dark Spirit Str (5)", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);
            this.CreateSkill(SkillNumber.SpiritLord, "Spirit Lord", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0);

            // Fist Master (Rage Fighter):
            this.CreateSkill(SkillNumber.KillingBlowStrengthener, "Killing Blow Strengthener", 0, 22, 10, 0, 2, 0, 0, 4, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.BeastUppercutStrengthener, "Beast Uppercut Strengthener", 0, 22, 10, 0, 2, 0, 0, 3, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.KillingBlowMastery, "Killing Blow Mastery", 0, 1, 10, 0, 2, 0, 0, 4, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.BeastUppercutMastery, "Beast Uppercut Mastery", 0, 1, 10, 0, 2, 0, 0, 3, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.WeaponMasteryFistMaster, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.ChainDriveStrengthener, "Chain Drive Strengthener", 150, 22, 22, 22, 4, 0, 0, 0, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.DarkSideStrengthener, "Dark Side Strengthener", 180, 22, 84, 0, 4, 0, 0, 5, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.DragonRoarStrengthener, "Dragon Roar Strengthener", 150, 22, 60, 33, 3, 0, 0, 4, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.EquippedWeaponStrengthener, "Equipped Weapon Strengthener", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.DefSuccessRateIncPowUp, "Def SuccessRate IncPowUp", 50, 22, 55, 11, 7, 30, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.EquippedWeaponMastery, "Equipped Weapon Mastery", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.DefSuccessRateIncMastery, "DefSuccessRate IncMastery", 50, 22, 60, 12, 7, 30, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.StaminaIncreaseStrengthener, "Stamina Increase Strengthener", 80, 5, 55, 11, 7, 35, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.DurabilityReduction1FistMaster, "Durability Reduction (1)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreasePvPDefenseRate, "Increase PvP Defense Rate", 0, 29, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseMaximumSD, "Increase Maximum SD", 0, 33, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseManaRecoveryRate, "Increase Mana Recovery Rate", 0, 7, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreasePoisonResistance, "Increase Poison Resistance", 0, 1, 0, 0, 0, 0, 0, 1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.DurabilityReduction2FistMaster, "Durability Reduction (2)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseSDRecoveryRate, "Increase SD Recovery Rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseHPRecoveryRate, "Increase HP Recovery Rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseLightningResistance, "Increase Lightning Resistance", 0, 1, 0, 0, 0, 0, 0, 2, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreasesDefense, "Increases Defense", 0, 35, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreasesAGRecoveryRate, "Increases AG Recovery Rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseIceResistance, "Increase Ice Resistance", 0, 1, 0, 0, 0, 0, 0, 0, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.DurabilityReduction3FistMaster, "Durability Reduction(3)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseDefenseSuccessRate, "Increase Defense Success Rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.CastInvincibilityFistMaster, "Cast Invincibility", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseAttackSuccessRate, "Increase Attack Success Rate", 0, 30, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseMaximumHP, "Increase Maximum HP", 0, 34, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseMaximumMana, "Increase Maximum Mana", 0, 34, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseMaximumAG, "Increase Maximum AG", 0, 37, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreasePvPAttackRate, "Increase PvP Attack Rate", 0, 31, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.DecreaseMana, "Decrease Mana", 0, 18, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.RecoverSDfromMonsterKills, "Recover SD from Monster Kills", 0, 11, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.RecoverHPfromMonsterKills, "Recover HP from Monster Kills", 0, 6, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseMinimumAttackPower, "Increase Minimum Attack Power", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.RecoverManaMonsterKills, "Recover Mana Monster Kills", 0, 6, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseMaximumAttackPower, "Increase Maximum Attack Power", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreasesCritDamageChance, "Increases Crit Damage Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.RecoverManaFully, "Recover Mana Fully", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.RecoversHPFully, "Recovers HP Fully", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseExcDamageChance, "Increase Exc Damage Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseDoubleDamageChance, "Increase Double Damage Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseIgnoreDefChance, "Increase Ignore Def Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.RecoversSDFully, "Recovers SD Fully", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);
            this.CreateSkill(SkillNumber.IncreaseTripleDamageChance, "Increase Triple Damage Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3);

            this.InitializeEffects();
            this.MapSkillsToEffects();
            this.InitializeMasterSkillData();
        }

        private void InitializeEffects()
        {
            new SoulBarrierEffectInitializer(this.Context, this.GameConfiguration).Initialize();
            new LifeSwellEffectInitializer(this.Context, this.GameConfiguration).Initialize();
            new DefenseEffectInitializer(this.Context, this.GameConfiguration).Initialize();
            new GreaterDamageEffectInitializer(this.Context, this.GameConfiguration).Initialize();
            new GreaterDefenseEffectInitializer(this.Context, this.GameConfiguration).Initialize();
            new HealEffectInitializer(this.Context, this.GameConfiguration).Initialize();
            new ShieldRecoverEffectInitializer(this.Context, this.GameConfiguration).Initialize();
        }

        private void MapSkillsToEffects()
        {
            foreach (var effectOfSkill in EffectsOfSkills)
            {
                var skill = this.GameConfiguration.Skills.First(s => s.Number == (short)effectOfSkill.Key);
                var effect = this.GameConfiguration.MagicEffects.First(e => e.Number == (short)effectOfSkill.Value);
                skill.MagicEffectDef = effect;
            }
        }

        private void CreateSkill(SkillNumber skillId, string name, int levelRequirement, int damage, int manaConsumption, int abilityConsumption, short distance, int energyRequirement, int leadershipRequirement, int elementalModifier,
            int attackType, int useType, int count, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel, int ragefighterClassLevel,
            SkillType skillType = SkillType.DirectHit, SkillTarget skillTarget = SkillTarget.Explicit, short implicitTargetRange = 0, SkillTargetRestriction targetRestriction = SkillTargetRestriction.Undefined, bool movesToTarget = false,
            bool movesTarget = false)
        {
            var skill = this.Context.CreateNew<Skill>();
            this.GameConfiguration.Skills.Add(skill);
            skill.Number = (short)skillId;
            skill.Name = name;
            skill.MovesToTarget = movesToTarget;
            skill.MovesTarget = movesTarget;
            skill.AttackDamage = damage;

            this.CreateSkillRequirementIfNeeded(skill, Stats.Level, levelRequirement);
            this.CreateSkillRequirementIfNeeded(skill, Stats.TotalLeadership, leadershipRequirement);
            this.CreateSkillRequirementIfNeeded(skill, Stats.TotalEnergy, energyRequirement);
            this.CreateSkillConsumeRequirementIfNeeded(skill, Stats.CurrentMana, manaConsumption);
            this.CreateSkillConsumeRequirementIfNeeded(skill, Stats.CurrentAbility, abilityConsumption);

            skill.Range = distance;
            skill.DamageType = attackType == 1 ? DamageType.Wizardry : DamageType.Physical;
            skill.SkillType = skillType;
            if (useType == 3)
            {
                skill.SkillType = SkillType.PassiveBoost;
            }

            skill.ImplicitTargetRange = implicitTargetRange;
            skill.Target = skillTarget;
            skill.TargetRestriction = targetRestriction;
            var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, ragefighterClassLevel);
            foreach (var characterClass in classes)
            {
                skill.QualifiedCharacters.Add(characterClass);
            }
        }

        private void CreateSkillConsumeRequirementIfNeeded(Skill skill, AttributeDefinition attribute, int requiredValue)
        {
            if (requiredValue == 0)
            {
                return;
            }

            var requirement = this.CreateRequirement(attribute, requiredValue);
            skill.ConsumeRequirements.Add(requirement);
        }

        private void CreateSkillRequirementIfNeeded(Skill skill, AttributeDefinition attribute, int requiredValue)
        {
            if (requiredValue == 0)
            {
                return;
            }

            var requirement = this.CreateRequirement(attribute, requiredValue);
            skill.Requirements.Add(requirement);
        }

        /// <summary>
        /// Initializes the master skill data.
        /// </summary>
        private void InitializeMasterSkillData()
        {
            // Roots:
            this.masterSkillRoots = new SortedDictionary<byte, MasterSkillRoot>();
            var leftRoot = this.Context.CreateNew<MasterSkillRoot>();
            leftRoot.Name = "Left (Common Skills)";
            this.masterSkillRoots.Add(1, leftRoot);
            this.GameConfiguration.MasterSkillRoots.Add(leftRoot);
            var middleRoot = this.Context.CreateNew<MasterSkillRoot>();
            middleRoot.Name = "Middle Root";
            this.masterSkillRoots.Add(2, middleRoot);
            this.GameConfiguration.MasterSkillRoots.Add(middleRoot);
            var rightRoot = this.Context.CreateNew<MasterSkillRoot>();
            rightRoot.Name = "Right Root";
            this.masterSkillRoots.Add(3, rightRoot);
            this.GameConfiguration.MasterSkillRoots.Add(rightRoot);

            // Universal
            this.AddMasterSkillDefinition(SkillNumber.DurabilityReduction1, SkillNumber.Undefined, SkillNumber.Undefined, 1, 1, SkillNumber.Undefined, 20, Formula1204);
            this.AddPassiveMasterSkillDefinition(SkillNumber.PvPDefenceRateInc, Stats.DefenseRatePvp, AggregateType.AddRaw, Formula61408, 1, 1, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddPassiveMasterSkillDefinition(SkillNumber.MaximumSDincrease, Stats.MaximumShield, AggregateType.AddRaw, Formula51173, 2, 1, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddPassiveMasterSkillDefinition(SkillNumber.AutomaticManaRecInc, Stats.ManaRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease181, Formula181, 2, 1, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.PoisonResistanceInc, SkillNumber.Undefined, SkillNumber.Undefined, 1, 2, SkillNumber.Undefined, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.DurabilityReduction2, SkillNumber.DurabilityReduction1, SkillNumber.Undefined, 1, 3, SkillNumber.Undefined, 20, Formula1204);
            this.AddMasterSkillDefinition(SkillNumber.SDRecoverySpeedInc, SkillNumber.MaximumSDincrease, SkillNumber.Undefined, 1, 3, SkillNumber.Undefined, 20, Formula120);
            this.AddPassiveMasterSkillDefinition(SkillNumber.AutomaticHPRecInc, Stats.HealthRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 3, 1, SkillNumber.AutomaticManaRecInc, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.LightningResistanceInc, SkillNumber.PoisonResistanceInc, SkillNumber.Undefined, 1, 3, SkillNumber.Undefined, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.DefenseIncrease, SkillNumber.Undefined, SkillNumber.Undefined, 1, 4, SkillNumber.Undefined, 20, Formula6020);
            this.AddPassiveMasterSkillDefinition(SkillNumber.AutomaticAGRecInc, Stats.AbilityRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 4, 1, SkillNumber.AutomaticHPRecInc, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.IceResistanceIncrease, SkillNumber.LightningResistanceInc, SkillNumber.Undefined, 1, 4, SkillNumber.Undefined, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.DurabilityReduction3, SkillNumber.DurabilityReduction2, SkillNumber.Undefined, 1, 5, SkillNumber.Undefined, 20, Formula1204);
            this.AddPassiveMasterSkillDefinition(SkillNumber.DefenseSuccessRateInc, Stats.DefenseRatePvm, AggregateType.AddRaw, Formula120, 5, 1, SkillNumber.DefenseIncrease, SkillNumber.Undefined, 20);

            // DK
            this.AddPassiveMasterSkillDefinition(SkillNumber.AttackSuccRateInc, Stats.AttackRatePvm, AggregateType.AddRaw, Formula51173, 1, 2, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.CycloneStrengthener, SkillNumber.Cyclone, SkillNumber.Undefined, 2, 2, SkillNumber.Cyclone, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.SlashStrengthener, SkillNumber.Slash, SkillNumber.Undefined, 2, 2, SkillNumber.Slash, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.FallingSlashStreng, SkillNumber.FallingSlash, SkillNumber.Undefined, 2, 2, SkillNumber.FallingSlash, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.LungeStrengthener, SkillNumber.Lunge, SkillNumber.Undefined, 2, 2, SkillNumber.Lunge, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.TwistingSlashStreng, SkillNumber.TwistingSlash, SkillNumber.Undefined, 2, 3, SkillNumber.TwistingSlash, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.RagefulBlowStreng, SkillNumber.RagefulBlow, SkillNumber.Undefined, 2, 3, SkillNumber.RagefulBlow, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.TwistingSlashMastery, SkillNumber.TwistingSlashStreng, SkillNumber.Undefined, 2, 4, SkillNumber.TwistingSlash, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.RagefulBlowMastery, SkillNumber.RagefulBlowStreng, SkillNumber.Undefined, 2, 4, SkillNumber.RagefulBlow, 20, Formula120);
            this.AddPassiveMasterSkillDefinition(SkillNumber.MaximumLifeIncrease, Stats.MaximumHealth, AggregateType.AddRaw, Formula10235, 4, 2, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.WeaponMasteryBladeMaster, SkillNumber.Undefined, SkillNumber.Undefined, 2, 4, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.DeathStabStrengthener, SkillNumber.DeathStab, SkillNumber.Undefined, 2, 5, SkillNumber.DeathStab, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.StrikeofDestrStr, SkillNumber.StrikeofDestruction, SkillNumber.Undefined, 2, 5, SkillNumber.StrikeofDestruction, 20, Formula502);
            this.AddPassiveMasterSkillDefinition(SkillNumber.MaximumManaIncrease, Stats.MaximumMana, AggregateType.AddRaw, Formula10235, 5, 2, SkillNumber.MaximumLifeIncrease, SkillNumber.Undefined, 20);
            this.AddPassiveMasterSkillDefinition(SkillNumber.PvPAttackRate, Stats.AttackRatePvp, AggregateType.AddRaw, Formula81877, 1, 3, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.TwoHandedSwordStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula883);
            this.AddMasterSkillDefinition(SkillNumber.OneHandedSwordStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.MaceStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.SpearStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.TwoHandedSwordMaster, SkillNumber.TwoHandedSwordStrengthener, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula1154);
            this.AddPassiveMasterSkillDefinition(SkillNumber.OneHandedSwordMaster, Stats.AttackSpeed, AggregateType.AddRaw, Formula1, 3, 3, SkillNumber.OneHandedSwordStrengthener, SkillNumber.Undefined, 10);
            this.AddMasterSkillDefinition(SkillNumber.MaceMastery, SkillNumber.MaceStrengthener, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.SpearMastery, SkillNumber.SpearStrengthener, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.SwellLifeStrengt, SkillNumber.SwellLife, SkillNumber.Undefined, 3, 4, SkillNumber.SwellLife, 20, Formula181);
            this.AddMasterSkillDefinition(SkillNumber.ManaReduction, SkillNumber.Undefined, SkillNumber.Undefined, 3, 4, SkillNumber.Undefined, 20, Formula722);
            this.AddPassiveMasterSkillDefinition(SkillNumber.MonsterAttackSDInc, Stats.ShieldAfterMonsterKill, AggregateType.AddFinal, Formula914, 4, 3, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddPassiveMasterSkillDefinition(SkillNumber.MonsterAttackLifeInc, Stats.HealthAfterMonsterKill, AggregateType.AddFinal, Formula4319, 4, 3, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.SwellLifeProficiency, SkillNumber.SwellLifeStrengt, SkillNumber.Undefined, 3, 5, SkillNumber.SwellLife, 20, Formula181);
            this.AddPassiveMasterSkillDefinition(SkillNumber.MinimumAttackPowerInc, Stats.MinimumPhysBaseDmg, AggregateType.AddRaw, Formula502, 5, 3, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddPassiveMasterSkillDefinition(SkillNumber.MonsterAttackManaInc, Stats.ManaAfterMonsterKill, AggregateType.AddFinal, Formula4319, 5, 3, SkillNumber.MonsterAttackLifeInc, SkillNumber.Undefined, 20);

            // DW
            this.AddMasterSkillDefinition(SkillNumber.FlameStrengthener, SkillNumber.Flame, SkillNumber.Undefined, 2, 2, SkillNumber.Flame, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.LightningStrengthener, SkillNumber.Lightning, SkillNumber.Undefined, 2, 2, SkillNumber.Lightning, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.ExpansionofWizStreng, SkillNumber.ExpansionofWizardry, SkillNumber.Undefined, 2, 2, SkillNumber.ExpansionofWizardry, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.InfernoStrengthener, SkillNumber.Inferno, SkillNumber.FlameStrengthener, 2, 3, SkillNumber.Inferno, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.BlastStrengthener, SkillNumber.Cometfall, SkillNumber.LightningStrengthener, 2, 3, SkillNumber.Cometfall, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.ExpansionofWizMas, SkillNumber.ExpansionofWizStreng, SkillNumber.Undefined, 2, 3, SkillNumber.ExpansionofWizardry, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.PoisonStrengthener, SkillNumber.Poison, SkillNumber.Undefined, 2, 3, SkillNumber.Poison, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.EvilSpiritStreng, SkillNumber.EvilSpirit, SkillNumber.Undefined, 2, 4, SkillNumber.EvilSpirit, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.MagicMasteryGrandMaster, SkillNumber.EvilSpiritStreng, SkillNumber.Undefined, 2, 4, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.DecayStrengthener, SkillNumber.Decay, SkillNumber.PoisonStrengthener, 2, 4, SkillNumber.Decay, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.HellfireStrengthener, SkillNumber.Hellfire, SkillNumber.Undefined, 2, 5, SkillNumber.Hellfire, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.IceStrengthener, SkillNumber.Ice, SkillNumber.Undefined, 2, 5, SkillNumber.Ice, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.OneHandedStaffStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.TwoHandedStaffStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula883);
            this.AddMasterSkillDefinition(SkillNumber.ShieldStrengthenerGrandMaster, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula803);
            this.AddPassiveMasterSkillDefinition(SkillNumber.OneHandedStaffMaster, Stats.AttackSpeed, AggregateType.AddRaw, Formula1, 3, 3, SkillNumber.OneHandedStaffStrengthener, SkillNumber.Undefined, 10);
            this.AddMasterSkillDefinition(SkillNumber.TwoHandedStaffMaster, SkillNumber.TwoHandedStaffStrengthener, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula1154);
            this.AddMasterSkillDefinition(SkillNumber.ShieldMasteryGrandMaster, SkillNumber.ShieldStrengthenerGrandMaster, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula1204);
            this.AddMasterSkillDefinition(SkillNumber.SoulBarrierStrength, SkillNumber.SoulBarrier, SkillNumber.Undefined, 3, 4, SkillNumber.SoulBarrier, 20, Formula181);
            this.AddMasterSkillDefinition(SkillNumber.SoulBarrierProficie, SkillNumber.SoulBarrierStrength, SkillNumber.Undefined, 3, 5, SkillNumber.SoulBarrier, 20, Formula803);
            this.AddPassiveMasterSkillDefinition(SkillNumber.MinimumWizardryInc, Stats.MinimumWizBaseDmg, AggregateType.AddRaw, Formula502, 5, 3, SkillNumber.Undefined, SkillNumber.Undefined, 20);

            // ELF
            this.AddMasterSkillDefinition(SkillNumber.HealStrengthener, SkillNumber.Heal, SkillNumber.Undefined, 2, 2, SkillNumber.Heal, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.TripleShotStrengthener, SkillNumber.TripleShot, SkillNumber.Undefined, 2, 2, SkillNumber.TripleShot, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.SummonedMonsterStr1, SkillNumber.SummonGoblin, SkillNumber.Undefined, 2, 2, SkillNumber.SummonGoblin, 20, Formula6020);
            this.AddMasterSkillDefinition(SkillNumber.PenetrationStrengthener, SkillNumber.Penetration, SkillNumber.Undefined, 2, 3, SkillNumber.Penetration, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.DefenseIncreaseStr, SkillNumber.GreaterDefense, SkillNumber.Undefined, 2, 3, SkillNumber.GreaterDefense, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.TripleShotMastery, SkillNumber.TripleShotStrengthener, SkillNumber.Undefined, 2, 3, SkillNumber.TripleShot, 10, Formula1WhenComplete);
            this.AddMasterSkillDefinition(SkillNumber.SummonedMonsterStr2, SkillNumber.SummonedMonsterStr1, SkillNumber.Undefined, 2, 3, SkillNumber.SummonGoblin, 20, Formula6020);
            this.AddMasterSkillDefinition(SkillNumber.AttackIncreaseStr, SkillNumber.GreaterDamage, SkillNumber.Undefined, 2, 4, SkillNumber.GreaterDamage, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.WeaponMasteryHighElf, SkillNumber.Undefined, SkillNumber.Undefined, 2, 4, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.AttackIncreaseMastery, SkillNumber.AttackIncreaseStr, SkillNumber.Undefined, 2, 5, SkillNumber.GreaterDamage, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.DefenseIncreaseMastery, SkillNumber.DefenseIncreaseStr, SkillNumber.Undefined, 2, 5, SkillNumber.GreaterDefense, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.IceArrowStrengthener, SkillNumber.IceArrow, SkillNumber.Undefined, 2, 5, SkillNumber.IceArrow, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.BowStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.CrossbowStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.ShieldStrengthenerHighElf, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula803);
            this.AddPassiveMasterSkillDefinition(SkillNumber.BowMastery, Stats.AttackSpeed, AggregateType.AddRaw, Formula1, 3, 3, SkillNumber.BowStrengthener, SkillNumber.Undefined, 10);
            this.AddMasterSkillDefinition(SkillNumber.CrossbowMastery, SkillNumber.CrossbowStrengthener, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula1154);
            this.AddMasterSkillDefinition(SkillNumber.ShieldMasteryHighElf, SkillNumber.ShieldStrengthenerHighElf, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula1806);
            this.AddMasterSkillDefinition(SkillNumber.InfinityArrowStr, SkillNumber.InfinityArrow, SkillNumber.Undefined, 3, 5, SkillNumber.InfinityArrow, 20, Formula120);
            this.AddPassiveMasterSkillDefinition(SkillNumber.MinimumAttPowerInc, Stats.MinimumPhysBaseDmg, AggregateType.AddRaw, Formula502, 5, 3, SkillNumber.Undefined, SkillNumber.Undefined, 20);

            // SUM
            this.AddMasterSkillDefinition(SkillNumber.FireTomeStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 2, 2, SkillNumber.Undefined, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.WindTomeStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 2, 2, SkillNumber.Undefined, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.LightningTomeStren, SkillNumber.Undefined, SkillNumber.Undefined, 2, 2, SkillNumber.Undefined, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.FireTomeMastery, SkillNumber.FireTomeStrengthener, SkillNumber.Undefined, 2, 3, SkillNumber.Undefined, 20, Formula181);
            this.AddMasterSkillDefinition(SkillNumber.WindTomeMastery, SkillNumber.WindTomeStrengthener, SkillNumber.Undefined, 2, 3, SkillNumber.Undefined, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.LightningTomeMastery, SkillNumber.LightningTomeStren, SkillNumber.Undefined, 2, 3, SkillNumber.Undefined, 20, Formula181);
            this.AddMasterSkillDefinition(SkillNumber.SleepStrengthener, SkillNumber.Sleep, SkillNumber.Undefined, 2, 3, SkillNumber.Sleep, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.ChainLightningStr, SkillNumber.ChainLightning, SkillNumber.Undefined, 2, 4, SkillNumber.ChainLightning, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.LightningShockStr, SkillNumber.LightningShock, SkillNumber.Undefined, 2, 4, SkillNumber.LightningShock, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.MagicMasterySummoner, SkillNumber.Undefined, SkillNumber.Undefined, 2, 5, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.DrainLifeStrengthener, SkillNumber.DrainLife, SkillNumber.Undefined, 2, 5, SkillNumber.DrainLife, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.StickStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.OtherWorldTomeStreng, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.StickMastery, SkillNumber.StickStrengthener, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula1154);
            this.AddPassiveMasterSkillDefinition(SkillNumber.OtherWorldTomeMastery, Stats.AttackSpeed, AggregateType.AddRaw, Formula1, 3, 3, SkillNumber.OtherWorldTomeStreng, SkillNumber.Undefined, 10);
            this.AddMasterSkillDefinition(SkillNumber.BerserkerStrengthener, SkillNumber.Berserker, SkillNumber.Undefined, 3, 4, SkillNumber.Berserker, 20, Formula181);
            this.AddMasterSkillDefinition(SkillNumber.BerserkerProficiency, SkillNumber.BerserkerStrengthener, SkillNumber.Undefined, 3, 5, SkillNumber.Undefined, 20, Formula181);
            this.AddPassiveMasterSkillDefinition(SkillNumber.MinimumWizCurseInc, Stats.MinimumCurseBaseDmg, AggregateType.AddRaw, Formula502, 5, 3, SkillNumber.Undefined, SkillNumber.Undefined, 20);

            // MG
            this.AddMasterSkillDefinition(SkillNumber.CycloneStrengthenerDuelMaster, SkillNumber.Cyclone, SkillNumber.Undefined, 2, 2, SkillNumber.Cyclone, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.LightningStrengthenerDuelMaster, SkillNumber.Lightning, SkillNumber.Undefined, 2, 2, SkillNumber.Lightning, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.TwistingSlashStrengthenerDuelMaster, SkillNumber.TwistingSlash, SkillNumber.Undefined, 2, 2, SkillNumber.TwistingSlash, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.PowerSlashStreng, SkillNumber.PowerSlash, SkillNumber.Undefined, 2, 2, SkillNumber.PowerSlash, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.FlameStrengthenerDuelMaster, SkillNumber.Flame, SkillNumber.Undefined, 2, 3, SkillNumber.Flame, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.BlastStrengthenerDuelMaster, SkillNumber.Cometfall, SkillNumber.LightningStrengthenerDuelMaster, 2, 3, SkillNumber.Cometfall, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.WeaponMasteryDuelMaster, SkillNumber.TwistingSlashStrengthenerDuelMaster, SkillNumber.PowerSlashStreng, 2, 3, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.InfernoStrengthenerDuelMaster, SkillNumber.Inferno, SkillNumber.FlameStrengthenerDuelMaster, 2, 4, SkillNumber.Inferno, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.EvilSpiritStrengthenerDuelMaster, SkillNumber.EvilSpirit, SkillNumber.Undefined, 2, 4, SkillNumber.EvilSpirit, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.MagicMasteryDuelMaster, SkillNumber.DimensionWingsAttPowUp, SkillNumber.Undefined, 2, 4, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.IceStrengthenerDuelMaster, SkillNumber.Ice, SkillNumber.Undefined, 2, 5, SkillNumber.Ice, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.BloodAttackStrengthen, SkillNumber.FireSlash, SkillNumber.Undefined, 2, 5, SkillNumber.FireSlash, 20, Formula502);

            // DL
            this.AddMasterSkillDefinition(SkillNumber.FireBurstStreng, SkillNumber.FireBurst, SkillNumber.Undefined, 2, 2, SkillNumber.FireBurst, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.ForceWaveStreng, SkillNumber.Force, SkillNumber.Undefined, 2, 2, SkillNumber.Force, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.DarkHorseStreng1, SkillNumber.Undefined, SkillNumber.Undefined, 2, 2, SkillNumber.Undefined, 20, Formula1204);
            this.AddMasterSkillDefinition(SkillNumber.CriticalDMGIncPowUp, SkillNumber.IncreaseCriticalDamage, SkillNumber.Undefined, 2, 3, SkillNumber.IncreaseCriticalDamage, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.EarthshakeStreng, SkillNumber.Earthshake, SkillNumber.DarkHorseStreng1, 2, 3, SkillNumber.Earthshake, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.WeaponMasteryLordEmperor, SkillNumber.Undefined, SkillNumber.Undefined, 2, 3, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.FireBurstMastery, SkillNumber.FireBurstStreng, SkillNumber.Undefined, 2, 4, SkillNumber.FireBurst, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.CritDMGIncPowUp2, SkillNumber.CriticalDMGIncPowUp, SkillNumber.Undefined, 2, 4, SkillNumber.IncreaseCriticalDamage, 20, Formula803);
            this.AddMasterSkillDefinition(SkillNumber.EarthshakeMastery, SkillNumber.EarthshakeStreng, SkillNumber.Undefined, 2, 4, SkillNumber.Earthshake, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.CritDMGIncPowUp3, SkillNumber.CritDMGIncPowUp2, SkillNumber.Undefined, 2, 5, SkillNumber.IncreaseCriticalDamage, 20, Formula181);
            this.AddMasterSkillDefinition(SkillNumber.FireScreamStren, SkillNumber.FireScream, SkillNumber.Undefined, 2, 5, SkillNumber.FireScream, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.DarkSpiritStr, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.ScepterStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.ShieldStrengthenerLordEmperor, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula803);
            this.AddMasterSkillDefinition(SkillNumber.UseScepterPetStr, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula632);
            this.AddMasterSkillDefinition(SkillNumber.DarkSpiritStr2, SkillNumber.DarkSpiritStr, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula181);
            this.AddMasterSkillDefinition(SkillNumber.ScepterMastery, SkillNumber.ScepterStrengthener, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula1154);
            this.AddMasterSkillDefinition(SkillNumber.ShieldMastery, SkillNumber.ShieldStrengthenerLordEmperor, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula1204);
            this.AddMasterSkillDefinition(SkillNumber.CommandAttackInc, SkillNumber.UseScepterPetStr, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula3822);
            this.AddMasterSkillDefinition(SkillNumber.DarkSpiritStr3, SkillNumber.DarkSpiritStr2, SkillNumber.Undefined, 3, 5, SkillNumber.Undefined, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.PetDurabilityStr, SkillNumber.Undefined, SkillNumber.Undefined, 3, 5, SkillNumber.Undefined, 20, Formula1204);

            // RF
            this.AddMasterSkillDefinition(SkillNumber.DurabilityReduction1FistMaster, SkillNumber.Undefined, SkillNumber.Undefined, 1, 1, SkillNumber.Undefined, 20, Formula1204);
            this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasePvPDefenseRate, Stats.DefenseRatePvp, AggregateType.AddRaw, Formula25587, 1, 1, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseMaximumSD, Stats.MaximumShield, AggregateType.AddRaw, Formula30704, 2, 1, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseManaRecoveryRate, Stats.ManaRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease181, Formula181, 2, 1, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.IncreasePoisonResistance, SkillNumber.Undefined, SkillNumber.Undefined, 1, 2, SkillNumber.Undefined, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.DurabilityReduction2FistMaster, SkillNumber.DurabilityReduction1FistMaster, SkillNumber.Undefined, 1, 3, SkillNumber.Undefined, 20, Formula1204);
            this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseSDRecoveryRate, Stats.ShieldRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 3, 1, SkillNumber.IncreaseMaximumSD, SkillNumber.Undefined, 20);
            this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseHPRecoveryRate, Stats.HealthRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 3, 1, SkillNumber.IncreaseManaRecoveryRate, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.IncreaseLightningResistance, SkillNumber.IncreasePoisonResistance, SkillNumber.Undefined, 1, 3, SkillNumber.Undefined, 20, Formula120);
            this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasesDefense, Stats.DefenseBase, AggregateType.AddRaw, Formula3371, 4, 1, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasesAGRecoveryRate, Stats.AbilityRecoveryMultiplier, AggregateType.AddRaw, FormulaRecoveryIncrease120, Formula120, 4, 1, SkillNumber.IncreaseHPRecoveryRate, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.IncreaseIceResistance, SkillNumber.IncreaseLightningResistance, SkillNumber.Undefined, 1, 4, SkillNumber.Undefined, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.DurabilityReduction3FistMaster, SkillNumber.DurabilityReduction2FistMaster, SkillNumber.Undefined, 1, 5, SkillNumber.Undefined, 20, Formula1204);
            this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseDefenseSuccessRate, Stats.DefenseRatePvm, AggregateType.Multiplicate, FormulaIncreaseMultiplicator120, Formula120, 5, 1, SkillNumber.IncreasesDefense, SkillNumber.Undefined, 20);
            this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseAttackSuccessRate, Stats.AttackRatePvm, AggregateType.AddRaw, Formula20469, 1, 2, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.KillingBlowStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 2, 2, SkillNumber.KillingBlow, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.BeastUppercutStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 2, 2, SkillNumber.BeastUppercut, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.KillingBlowMastery, SkillNumber.KillingBlowStrengthener, SkillNumber.Undefined, 2, 3, SkillNumber.KillingBlow, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.BeastUppercutMastery, SkillNumber.BeastUppercutStrengthener, SkillNumber.Undefined, 2, 3, SkillNumber.BeastUppercut, 20, Formula120);
            this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseMaximumHP, Stats.MaximumHealth, AggregateType.AddRaw, Formula5418, 4, 2, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.WeaponMasteryFistMaster, SkillNumber.Undefined, SkillNumber.Undefined, 2, 4, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.ChainDriveStrengthener, SkillNumber.ChainDrive, SkillNumber.Undefined, 2, 5, SkillNumber.ChainDrive, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.DarkSideStrengthener, SkillNumber.DarkSide, SkillNumber.Undefined, 2, 5, SkillNumber.DarkSide, 20, Formula502);
            this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseMaximumMana, Stats.MaximumMana, AggregateType.AddRaw, Formula5418, 5, 2, SkillNumber.IncreaseMaximumHP, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.DragonRoarStrengthener, SkillNumber.DragonRoar, SkillNumber.Undefined, 2, 5, SkillNumber.DragonRoar, 20, Formula502);
            this.AddPassiveMasterSkillDefinition(SkillNumber.IncreasePvPAttackRate, Stats.AttackRatePvp, AggregateType.AddRaw, Formula32751, 1, 3, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddMasterSkillDefinition(SkillNumber.EquippedWeaponStrengthener, SkillNumber.Undefined, SkillNumber.Undefined, 3, 2, SkillNumber.Undefined, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.DefSuccessRateIncPowUp, SkillNumber.IncreaseBlock, SkillNumber.Undefined, 3, 2, SkillNumber.IncreaseBlock, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.EquippedWeaponMastery, SkillNumber.EquippedWeaponStrengthener, SkillNumber.Undefined, 3, 3, SkillNumber.Undefined, 20, Formula120);
            this.AddMasterSkillDefinition(SkillNumber.DefSuccessRateIncMastery, SkillNumber.DefSuccessRateIncPowUp, SkillNumber.Undefined, 3, 3, SkillNumber.IncreaseBlock, 20, Formula502);
            this.AddMasterSkillDefinition(SkillNumber.StaminaIncreaseStrengthener, SkillNumber.IncreaseHealth, SkillNumber.Undefined, 3, 4, SkillNumber.IncreaseHealth, 20, Formula1154);
            this.AddMasterSkillDefinition(SkillNumber.DecreaseMana, SkillNumber.Undefined, SkillNumber.Undefined, 3, 4, SkillNumber.Undefined, 20, Formula722);
            this.AddPassiveMasterSkillDefinition(SkillNumber.RecoverSDfromMonsterKills, Stats.ShieldAfterMonsterKill, AggregateType.AddFinal, Formula914, 4, 3, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddPassiveMasterSkillDefinition(SkillNumber.RecoverHPfromMonsterKills, Stats.HealthAfterMonsterKill, AggregateType.AddFinal, Formula4319, 4, 3, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddPassiveMasterSkillDefinition(SkillNumber.IncreaseMinimumAttackPower, Stats.MinimumPhysBaseDmg, AggregateType.AddRaw, Formula502, 5, 3, SkillNumber.Undefined, SkillNumber.Undefined, 20);
            this.AddPassiveMasterSkillDefinition(SkillNumber.RecoverManaMonsterKills, Stats.ManaAfterMonsterKill, AggregateType.AddFinal, Formula4319, 5, 3, SkillNumber.RecoverHPfromMonsterKills, SkillNumber.Undefined, 20);
        }

        private void AddPassiveMasterSkillDefinition(SkillNumber skillNumber, AttributeDefinition targetAttribute, AggregateType aggregateType, string valueFormula, string displayValueFormula, byte rank, byte root, SkillNumber requiredSkill1, SkillNumber requiredSkill2, byte maximumLevel)
        {
            this.AddMasterSkillDefinition(skillNumber, requiredSkill1, requiredSkill2, root, rank, SkillNumber.Undefined, maximumLevel, valueFormula, displayValueFormula, targetAttribute, aggregateType);
        }

        private void AddPassiveMasterSkillDefinition(SkillNumber skillNumber, AttributeDefinition targetAttribute, AggregateType aggregateType, string valueFormula, byte rank, byte root, SkillNumber requiredSkill1, SkillNumber requiredSkill2, byte maximumLevel)
        {
            this.AddMasterSkillDefinition(skillNumber, requiredSkill1, requiredSkill2, root, rank, SkillNumber.Undefined, maximumLevel, valueFormula, valueFormula, targetAttribute, aggregateType);
        }

        private void AddMasterSkillDefinition(SkillNumber skillNumber, SkillNumber requiredSkill1, SkillNumber requiredSkill2, byte root, byte rank, SkillNumber regularSkill, byte maximumLevel, string valueFormula)
        {
            this.AddMasterSkillDefinition(skillNumber, requiredSkill1, requiredSkill2, root, rank, regularSkill, maximumLevel, valueFormula, valueFormula, null, AggregateType.AddRaw);
        }

        private void AddMasterSkillDefinition(SkillNumber skillNumber, SkillNumber requiredSkill1, SkillNumber requiredSkill2, byte root, byte rank, SkillNumber regularSkill, byte maximumLevel, string valueFormula, string displayValueFormula, AttributeDefinition targetAttribute, AggregateType aggregateType)
        {
            var skill = this.GameConfiguration.Skills.First(s => s.Number == (short)skillNumber);
            skill.MasterDefinition = this.Context.CreateNew<MasterSkillDefinition>();
            skill.MasterDefinition.Rank = rank;
            skill.MasterDefinition.Root = this.masterSkillRoots[root];
            skill.MasterDefinition.ValueFormula = valueFormula;
            skill.MasterDefinition.DisplayValueFormula = displayValueFormula;
            skill.MasterDefinition.MaximumLevel = maximumLevel;
            skill.MasterDefinition.TargetAttribute = targetAttribute?.GetPersistent(this.GameConfiguration);
            skill.MasterDefinition.Aggregation = aggregateType;
            skill.MasterDefinition.ReplacedSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)regularSkill);
            if (requiredSkill1 != SkillNumber.Undefined)
            {
                skill.MasterDefinition.RequiredMasterSkills.Add(this.GameConfiguration.Skills.First(s => s.Number == (short)requiredSkill1));
            }

            if (requiredSkill2 != SkillNumber.Undefined)
            {
                skill.MasterDefinition.RequiredMasterSkills.Add(this.GameConfiguration.Skills.First(s => s.Number == (short)requiredSkill2));
            }

            if (maximumLevel == 10 && valueFormula == Formula1WhenComplete)
            {
                skill.MasterDefinition.MinimumLevel = maximumLevel;
            }
            else
            {
                skill.MasterDefinition.MinimumLevel = 1;
            }

            var replacedSkill = skill.MasterDefinition.ReplacedSkill;
            if (replacedSkill != null)
            {
                // Because we don't want to duplicate code from the replaced skills to the master skills, we just assign some values from the replaced skill.
                // These describe the skill behavior.
                skill.AttackDamage = replacedSkill.AttackDamage;
                skill.DamageType = replacedSkill.DamageType;
                skill.ElementalModifierTarget = replacedSkill.ElementalModifierTarget;
                skill.ImplicitTargetRange = replacedSkill.ImplicitTargetRange;
                skill.MovesTarget = replacedSkill.MovesTarget;
                skill.MovesToTarget = replacedSkill.MovesToTarget;
                skill.SkillType = replacedSkill.SkillType;
                skill.Target = replacedSkill.Target;
                skill.TargetRestriction = replacedSkill.TargetRestriction;
                skill.MagicEffectDef = replacedSkill.MagicEffectDef;
            }
        }
    }
}
