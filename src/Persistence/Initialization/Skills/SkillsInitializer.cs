// <copyright file="SkillsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initialization logic for <see cref="Skill"/>s.
    /// </summary>
    internal class SkillsInitializer : InitializerBase
    {
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
            this.CreateSkill(SkillNumber.Poison, "Poison", 30, 12, 42, 0, 6, 100, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Meteorite, "Meteorite", 21, 21, 12, 0, 6, 100, 0, 4, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Lightning, "Lightning", 13, 17, 15, 0, 6, 100, 0, 2, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.FireBall, "Fire Ball", 5, 8, 3, 0, 6, 100, 0, 3, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Flame, "Flame", 35, 25, 50, 0, 6, 100, 0, 3, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.Teleport, "Teleport", 17, 0, 30, 0, 6, 100, 0, -1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.Ice, "Ice", 25, 10, 38, 0, 6, 100, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Twister, "Twister", 40, 35, 60, 0, 6, 100, 0, 5, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.EvilSpirit, "Evil Spirit", 50, 45, 90, 0, 6, 100, 0, -1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.Hellfire, "Hellfire", 60, 120, 160, 0, 0, 100, 0, 3, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.PowerWave, "Power Wave", 9, 14, 5, 0, 6, 100, 0, -1, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.AquaBeam, "Aqua Beam", 74, 80, 140, 0, 6, 110, 0, 6, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.Cometfall, "Cometfall", 80, 70, 150, 0, 3, 150, 0, 2, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Inferno, "Inferno", 88, 100, 200, 0, 0, 200, 0, 3, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.TeleportAlly, "Teleport Ally", 83, 0, 90, 25, 6, 188, 0, -1, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SoulBarrier, "Soul Barrier", 77, 0, 70, 22, 6, 126, 0, -1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.Buff, SkillTarget.Explicit, 0, SkillTargetRestriction.Party);
            this.CreateSkill(SkillNumber.EnergyBall, "Energy Ball", 2, 3, 1, 0, 6, 0, 0, -1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Defense, "Defense", 0, 0, 30, 0, 0, 0, 0, -1, -1, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, SkillType.Buff, SkillTarget.Explicit, 0, SkillTargetRestriction.Self);
            this.CreateSkill(SkillNumber.FallingSlash, "Falling Slash", 0, 0, 9, 0, 3, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Lunge, "Lunge", 0, 0, 9, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Uppercut, "Uppercut", 0, 0, 8, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Cyclone, "Cyclone", 0, 0, 9, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Slash, "Slash", 0, 0, 10, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.TripleShot, "Triple Shot", 0, 0, 5, 0, 6, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.Heal, "Heal", 8, 0, 20, 0, 6, 100, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Regeneration, targetRestriction: SkillTargetRestriction.Player);
            this.CreateSkill(SkillNumber.GreaterDefense, "Greater Defense", 13, 0, 30, 0, 6, 100, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Buff, targetRestriction: SkillTargetRestriction.Player);
            this.CreateSkill(SkillNumber.GreaterDamage, "Greater Damage", 18, 0, 40, 0, 6, 100, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Buff, targetRestriction: SkillTargetRestriction.Player);
            this.CreateSkill(SkillNumber.SummonGoblin, "Summon Goblin", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonStoneGolem, "Summon Stone Golem", 0, 0, 70, 0, 0, 170, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonAssassin, "Summon Assassin", 0, 0, 110, 0, 0, 190, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonEliteYeti, "Summon Elite Yeti", 0, 0, 160, 0, 0, 230, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonDarkKnight, "Summon Dark Knight", 0, 0, 200, 0, 0, 250, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonBali, "Summon Bali", 0, 0, 250, 0, 0, 260, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonSoldier, "Summon Soldier", 0, 0, 350, 0, 0, 280, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.Decay, "Decay", 96, 95, 110, 7, 6, 243, 0, 1, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.IceStorm, "Ice Storm", 93, 80, 100, 5, 6, 223, 0, 0, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.Nova, "Nova", 100, 0, 180, 45, 6, 258, 0, 3, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.TwistingSlash, "Twisting Slash", 0, 0, 10, 10, 2, 0, 0, 5, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.RagefulBlow, "Rageful Blow", 170, 60, 25, 20, 3, 0, 0, 4, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.DeathStab, "Death Stab", 160, 70, 15, 12, 2, 0, 0, 5, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.DirectHit, SkillTarget.ExplicitWithImplicitInRange, 1);
            this.CreateSkill(SkillNumber.CrescentMoonSlash, "Crescent Moon Slash", 0, 90, 22, 15, 4, 0, 0, -1, -1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Lance, "Lance", 0, 90, 150, 10, 6, 0, 0, -1, -1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Starfall, "Starfall", 0, 120, 20, 15, 8, 0, 0, -1, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Impale, "Impale", 28, 15, 8, 0, 3, 0, 0, -1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SwellLife, "Swell Life", 120, 0, 22, 24, 0, 0, 0, -1, -1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, SkillType.Buff, SkillTarget.ImplicitParty);
            this.CreateSkill(SkillNumber.FireBreath, "Fire Breath", 110, 30, 9, 0, 3, 0, 0, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.FlameofEvil, "Flame of Evil (Monster)", 60, 120, 160, 0, 0, 100, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.IceArrow, "Ice Arrow", 0, 105, 10, 12, 8, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Penetration, "Penetration", 130, 70, 7, 9, 6, 0, 0, 5, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.FireSlash, "Fire Slash", 0, 80, 15, 20, 2, 0, 0, 1, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.PowerSlash, "Power Slash", 0, 0, 15, 0, 5, 100, 0, -1, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.SpiralSlash, "Spiral Slash", 0, 75, 20, 15, 5, 0, 0, -1, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Force, "Force", 0, 10, 10, 0, 4, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.FireBurst, "Fire Burst", 74, 100, 25, 0, 6, 20, 0, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, SkillType.DirectHit, SkillTarget.ExplicitWithImplicitInRange, 1);
            this.CreateSkill(SkillNumber.Earthshake, "Earthshake", 0, 150, 0, 50, 10, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.Summon, "Summon", 98, 0, 70, 30, 0, 34, 400, -1, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.IncreaseCriticalDamage, "Increase Critical Damage", 82, 0, 50, 50, 0, 25, 300, -1, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, SkillType.Buff);
            this.CreateSkill(SkillNumber.ElectricSpike, "Electric Spike", 92, 250, 0, 100, 10, 29, 340, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.ForceWave, "Force Wave", 0, 50, 10, 0, 4, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Stun, "Stun", 0, 0, 70, 50, 2, 0, 0, -1, -1, 1, 4, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.CancelStun, "Cancel Stun", 0, 0, 25, 30, 0, 0, 0, -1, -1, 1, 2, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.SwellMana, "Swell Mana", 0, 0, 35, 30, 0, 0, 0, -1, -1, 1, 4, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Invisibility, "Invisibility", 0, 0, 80, 60, 0, 0, 0, -1, -1, 1, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.CancelInvisibility, "Cancel Invisibility", 0, 0, 40, 30, 0, 0, 0, -1, -1, 1, 2, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.AbolishMagic, "Abolish Magic", 0, 0, 90, 70, 0, 0, 0, -1, -1, 1, 8, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.ManaRays, "Mana Rays", 0, 85, 130, 7, 6, 0, 0, -1, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.FireBlast, "Fire Blast", 0, 150, 30, 10, 6, 0, 0, -1, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.PlasmaStorm, "Plasma Storm", 110, 60, 50, 20, 6, 0, 0, -1, 0, 0, 0, 2, 2, 2, 1, 1, 2, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.InfinityArrow, "Infinity Arrow", 220, 0, 50, 10, 6, 0, 0, -1, -1, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, SkillType.Buff);
            this.CreateSkill(SkillNumber.FireScream, "Fire Scream", 102, 130, 45, 10, 6, 32, 70, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, SkillType.AreaSkillAutomaticHits);
            this.CreateSkill(SkillNumber.Explosion79, "Explosion", 0, 0, 0, 0, 2, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SummonMonster, "Summon Monster", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.MagicAttackImmunity, "Magic Attack Immunity", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.PhysicalAttackImmunity, "Physical Attack Immunity", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.PotionofBless, "Potion of Bless", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.PotionofSoul, "Potion of Soul", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SpellofProtection, "Spell of Protection", 0, 0, 30, 0, 0, 0, 0, 0, 0, 0, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SpellofRestriction, "Spell of Restriction", 0, 0, 30, 0, 3, 0, 0, 0, 0, 0, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.SpellofPursuit, "Spell of Pursuit", 0, 0, 30, 0, 0, 0, 0, 0, 0, 0, 10, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.ShieldBurn, "Shield-Burn", 0, 0, 30, 0, 3, 0, 0, 0, 0, 0, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.DrainLife, "Drain Life", 35, 35, 50, 0, 6, 93, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.ChainLightning, "Chain Lightning", 75, 70, 85, 0, 6, 75, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.DamageReflection, "Damage Reflection", 80, 0, 40, 10, 5, 111, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Berserker, "Berserker", 83, 0, 100, 50, 5, 181, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Sleep, "Sleep", 40, 0, 20, 3, 6, 100, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Weakness, "Weakness", 93, 0, 50, 15, 6, 173, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Innovation, "Innovation", 111, 0, 70, 15, 6, 201, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Explosion223, "Explosion", 50, 40, 90, 5, 6, 100, 0, 3, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Requiem, "Requiem", 75, 65, 110, 10, 6, 99, 0, 5, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Pollution, "Pollution", 85, 80, 120, 15, 6, 115, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.LightningShock, "Lightning Shock", 93, 95, 115, 7, 6, 216, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.StrikeofDestruction, "Strike of Destruction", 100, 110, 30, 24, 5, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.ExpansionofWizardry, "Expansion of Wizardry", 220, 0, 200, 50, 6, 118, 0, -1, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.Recovery, "Recovery", 100, 0, 40, 10, 6, 37, 0, -1, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, SkillType.Regeneration, targetRestriction: SkillTargetRestriction.Player);
            this.CreateSkill(SkillNumber.MultiShot, "Multi-Shot", 100, 40, 10, 7, 6, 0, 0, -1, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.FlameStrike, "Flame Strike", 100, 140, 20, 25, 3, 0, 0, 3, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.GiganticStorm, "Gigantic Storm", 220, 110, 120, 10, 6, 118, 0, 5, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.ChaoticDiseier, "Chaotic Diseier", 100, 190, 50, 15, 6, 16, 0, -1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.DoppelgangerSelfExplosion, "Doppelganger Self Explosion", 100, 140, 20, 25, 3, 0, 0, 3, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0);
            this.CreateSkill(SkillNumber.KillingBlow, "Killing Blow", 0, 0, 9, 0, 2, 0, 0, 4, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.BeastUppercut, "Beast Uppercut", 0, 0, 9, 0, 2, 0, 0, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.ChainDrive, "Chain Drive", 150, 0, 15, 20, 4, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.DarkSide, "Dark Side", 180, 0, 70, 0, 4, 0, 0, 5, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.DragonRoar, "Dragon Roar", 150, 0, 50, 30, 3, 0, 0, 4, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.DragonSlasher, "Dragon Slasher", 200, 0, 100, 100, 4, 0, 0, 5, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.IgnoreDefense, "Ignore Defense", 120, 0, 50, 10, 3, 80, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.IncreaseHealth, "Increase Health", 80, 0, 50, 10, 7, 35, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.IncreaseBlock, "Increase Block", 50, 0, 50, 10, 7, 30, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.Charge, "Charge", 0, 90, 20, 15, 4, 0, 0, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
            this.CreateSkill(SkillNumber.PhoenixShot, "Phoenix Shot", 0, 0, 30, 0, 4, 0, 0, 4, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);

            // Master skills:
            // Common:
            this.CreateSkill(SkillNumber.DurabilityReduction1, "Durability Reduction (1)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 1, 1, 1);
            this.CreateSkill(SkillNumber.PvPDefenceRateInc, "PvP Defence Rate Inc", 0, 12, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 1, 2, 1);
            this.CreateSkill(SkillNumber.MaximumSDincrease, "Maximum SD increase", 0, 13, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 2, 3, 1);
            this.CreateSkill(SkillNumber.AutomaticManaRecInc, "Automatic Mana Rec Inc", 0, 7, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 2, 4, 1);
            this.CreateSkill(SkillNumber.PoisonResistanceInc, "Poison Resistance Inc", 0, 1, 0, 0, 0, 0, 0, 1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 2, 5, 1);
            this.CreateSkill(SkillNumber.DurabilityReduction2, "Durability Reduction (2)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 3, 6, 1);
            this.CreateSkill(SkillNumber.SDRecoverySpeedInc, "SD Recovery Speed Inc", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 3, 7, 1);
            this.CreateSkill(SkillNumber.AutomaticHPRecInc, "Automatic HP Rec Inc", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 3, 8, 1);
            this.CreateSkill(SkillNumber.LightningResistanceInc, "Lightning Resistance Inc", 0, 1, 0, 0, 0, 0, 0, 2, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 3, 9, 1);
            this.CreateSkill(SkillNumber.DefenseIncrease, "Defense Increase", 0, 16, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 10, 1);
            this.CreateSkill(SkillNumber.AutomaticAGRecInc, "Automatic AG Rec Inc", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 11, 1);
            this.CreateSkill(SkillNumber.IceResistanceIncrease, "Ice Resistance Increase", 0, 1, 0, 0, 0, 0, 0, 0, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 12, 1);
            this.CreateSkill(SkillNumber.DurabilityReduction3, "Durability Reduction (3)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 5, 13, 1);
            this.CreateSkill(SkillNumber.DefenseSuccessRateInc, "Defense Success Rate Inc", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 5, 14, 1);
            this.CreateSkill(SkillNumber.CastInvincibility, "Cast Invincibility", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 6, 15, 1);
            this.CreateSkill(SkillNumber.ArmorSetBonusInc, "Armor Set Bonus Inc", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 6, 16, 1);
            this.CreateSkill(SkillNumber.Vengeance, "Vengeance", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 6, 17, 1);
            this.CreateSkill(SkillNumber.EnergyIncrease, "Energy Increase", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 18, 1);
            this.CreateSkill(SkillNumber.StaminaIncrease, "Stamina Increase", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 19, 1);
            this.CreateSkill(SkillNumber.AgilityIncrease, "Agility Increase", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 20, 1);
            this.CreateSkill(SkillNumber.StrengthIncrease, "Strength Increase", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 21, 1);
            this.CreateSkill(SkillNumber.MaximumLifeIncrease, "Maximum Life Increase", 0, 9, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 43, 1);
            this.CreateSkill(SkillNumber.ManaReduction, "Mana Reduction", 0, 18, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 61, 1);
            this.CreateSkill(SkillNumber.MonsterAttackSDInc, "Monster Attack SD Inc", 0, 11, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 62, 1);
            this.CreateSkill(SkillNumber.MonsterAttackLifeInc, "Monster Attack Life Inc", 0, 6, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 4, 63, 1);
            this.CreateSkill(SkillNumber.SwellLifeProficiency, "Swell Life Proficiency", 120, 7, 26, 28, 0, 0, 0, -1, -1, 4, 0, 0, 3, 0, 0, 0, 0, 0, 5, 60, 1);
            this.CreateSkill(SkillNumber.MinimumAttackPowerInc, "Minimum Attack Power Inc", 0, 22, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 3, 0, 0, 5, 64, 1);
            this.CreateSkill(SkillNumber.MonsterAttackManaInc, "Monster Attack Mana Inc", 0, 6, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 5, 65, 1);
            this.CreateSkill(SkillNumber.SwellLifeMastery, "Swell Life Mastery", 120, 7, 28, 30, 0, 0, 0, -1, -1, 4, 0, 0, 3, 0, 0, 0, 0, 0, 6, 60, 1);
            this.CreateSkill(SkillNumber.MaximumAttackPowerInc, "Maximum Attack Power Inc", 0, 3, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 3, 0, 0, 6, 66, 1);
            this.CreateSkill(SkillNumber.Inccritdamagerate, "Inc crit damage rate", 0, 7, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 6, 68, 1);
            this.CreateSkill(SkillNumber.RestoresallMana, "Restores all Mana", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 69, 1);
            this.CreateSkill(SkillNumber.RestoresallHP, "Restores all HP", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 70, 1);
            this.CreateSkill(SkillNumber.Incexcdamagerate, "Inc exc damage rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 7, 71, 1);
            this.CreateSkill(SkillNumber.Incdoubledamagerate, "Inc double damage rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 8, 72, 1);
            this.CreateSkill(SkillNumber.IncchanceofignoreDef, "Inc chance of ignore Def", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 8, 73, 1);
            this.CreateSkill(SkillNumber.RestoresallSD, "Restores all SD", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 8, 74, 1);
            this.CreateSkill(SkillNumber.Inctripledamagerate, "Inc triple damage rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 9, 75, 1);
            this.CreateSkill(SkillNumber.PvPAttackRate, "PvP Attack Rate", 0, 14, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 1, 51, 1);

            // Blade Master:
            this.CreateSkill(SkillNumber.WingofStormAbsPowUp, "Wing of Storm Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 3, 0, 0, 0, 0, 0, 8, 22, 1);
            this.CreateSkill(SkillNumber.WingofStormDefPowUp, "Wing of Storm Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 3, 0, 0, 0, 0, 0, 8, 23, 1);
            this.CreateSkill(SkillNumber.IronDefense, "Iron Defense", 0, 1, 0, 0, 0, 0, 0, -1, -1, 4, 0, 3, 3, 3, 3, 3, 3, 3, 6, 15, 1);
            this.CreateSkill(SkillNumber.WingofStormAttPowUp, "Wing of Storm Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 3, 0, 0, 0, 0, 0, 9, 35, 1);
            this.CreateSkill(SkillNumber.AttackSuccRateInc, "Attack Succ Rate Inc", 0, 13, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 1, 36, 1);
            this.CreateSkill(SkillNumber.CycloneStrengthener, "Cyclone Strengthener", 0, 22, 9, 0, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 2, 37, 1);
            this.CreateSkill(SkillNumber.SlashStrengthener, "Slash Strengthener", 0, 3, 10, 0, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 2, 38, 1);
            this.CreateSkill(SkillNumber.FallingSlashStreng, "Falling Slash Streng", 0, 3, 9, 0, 3, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 2, 39, 1);
            this.CreateSkill(SkillNumber.LungeStrengthener, "Lunge Strengthener", 0, 3, 9, 0, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 2, 40, 1);
            this.CreateSkill(SkillNumber.TwistingSlashStreng, "Twisting Slash Streng", 0, 3, 10, 10, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 3, 41, 1);
            this.CreateSkill(SkillNumber.RagefulBlowStreng, "Rageful Blow Streng", 170, 22, 25, 22, 3, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 3, 42, 1);
            this.CreateSkill(SkillNumber.TwistingSlashMastery, "Twisting Slash Mastery", 0, 1, 22, 20, 2, 0, 0, -1, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 4, 41, 1);
            this.CreateSkill(SkillNumber.RagefulBlowMastery, "Rageful Blow Mastery", 170, 1, 50, 30, 3, 0, 0, 4, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 4, 42, 1);
            this.CreateSkill(SkillNumber.WeaponMasteryBladeMaster, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 3, 0, 0, 0, 0, 0, 4, 44, 1);
            this.CreateSkill(SkillNumber.DeathStabStrengthener, "Death Stab Strengthener", 160, 22, 15, 13, 2, 0, 0, 5, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 5, 45, 1);
            this.CreateSkill(SkillNumber.StrikeofDestrStr, "Strike of Destr Str", 100, 22, 30, 24, 5, 0, 0, 0, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 5, 46, 1);
            this.CreateSkill(SkillNumber.MaximumManaIncrease, "Maximum Mana Increase", 0, 9, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 5, 47, 1);
            this.CreateSkill(SkillNumber.DeathStabProficiency, "Death Stab Proficiency", 160, 7, 30, 26, 2, 0, 0, 5, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 6, 45, 1);
            this.CreateSkill(SkillNumber.StrikeofDestrProf, "Strike of Destr Prof", 100, 7, 30, 24, 5, 0, 0, 0, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 6, 46, 1);
            this.CreateSkill(SkillNumber.MaximumAGIncrease, "Maximum AG Increase", 0, 8, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 3, 3, 3, 3, 3, 0, 6, 48, 1);
            this.CreateSkill(SkillNumber.DeathStabMastery, "Death Stab Mastery", 160, 7, 30, 26, 2, 0, 0, 5, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 7, 45, 1);
            this.CreateSkill(SkillNumber.StrikeofDestrMast, "Strike of Destr Mast", 100, 1, 30, 24, 5, 0, 0, 0, 0, 4, 0, 0, 3, 0, 0, 0, 0, 0, 7, 46, 1);
            this.CreateSkill(SkillNumber.BloodStorm, "Blood Storm", 0, 25, 87, 29, 3, 0, 0, -1, 0, 4, 0, 0, 3, 0, 3, 0, 0, 0, 8, 49, 10);
            this.CreateSkill(SkillNumber.ComboStrengthener, "Combo Strengthener", 0, 7, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 8, 50, 1);
            this.CreateSkill(SkillNumber.BloodStormStrengthener, "Blood Storm Strengthener", 0, 22, 87, 29, 3, 0, 0, -1, 0, 4, 0, 0, 3, 0, 3, 0, 0, 0, 9, 49, 1);
            this.CreateSkill(SkillNumber.TwoHandedSwordStrengthener, "Two-handed Sword Stren", 0, 4, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 0, 0, 0, 2, 52, 1);
            this.CreateSkill(SkillNumber.OneHandedSwordStrengthener, "One-handed Sword Stren", 0, 22, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 0, 0, 0, 2, 53, 1);
            this.CreateSkill(SkillNumber.MaceStrengthener, "Mace Strengthener", 0, 3, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 2, 54, 1);
            this.CreateSkill(SkillNumber.SpearStrengthener, "Spear Strengthener", 0, 3, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 2, 55, 1);
            this.CreateSkill(SkillNumber.TwoHandedSwordMaster, "Two-handed Sword Mast", 0, 5, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 0, 0, 0, 3, 56, 1);
            this.CreateSkill(SkillNumber.OneHandedSwordMaster, "One-handed Sword Mast", 0, 23, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 3, 0, 0, 0, 3, 57, 1);
            this.CreateSkill(SkillNumber.MaceMastery, "Mace Mastery", 0, 1, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 3, 58, 1);
            this.CreateSkill(SkillNumber.SpearMastery, "Spear Mastery", 0, 1, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 3, 59, 1);
            this.CreateSkill(SkillNumber.SwellLifeStrengt, "Swell Life Strengt", 120, 7, 24, 26, 0, 0, 0, -1, -1, 4, 0, 0, 3, 0, 0, 0, 0, 0, 4, 60, 1);

            // Grand Master:
            this.CreateSkill(SkillNumber.EternalWingsAbsPowUp, "Eternal Wings Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0, 8, 76, 1);
            this.CreateSkill(SkillNumber.EternalWingsDefPowUp, "Eternal Wings Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0, 8, 77, 1);
            this.CreateSkill(SkillNumber.EternalWingsAttPowUp, "Eternal Wings Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0, 9, 79, 1);
            this.CreateSkill(SkillNumber.FlameStrengthener, "Flame Strengthener", 35, 3, 55, 0, 6, 100, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 2, 80, 1);
            this.CreateSkill(SkillNumber.LightningStrengthener, "Lightning Strengthener", 13, 3, 20, 0, 6, 100, 0, 2, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 2, 81, 1);
            this.CreateSkill(SkillNumber.ExpansionofWizStreng, "Expansion of Wiz Streng", 220, 1, 220, 55, 6, 118, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 2, 82, 1);
            this.CreateSkill(SkillNumber.InfernoStrengthener, "Inferno Strengthener", 88, 22, 220, 0, 0, 200, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 3, 83, 1);
            this.CreateSkill(SkillNumber.BlastStrengthener, "Blast Strengthener", 80, 22, 165, 0, 3, 150, 0, 2, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 3, 84, 1);
            this.CreateSkill(SkillNumber.ExpansionofWizMas, "Expansion of Wiz Mas", 220, 1, 220, 55, 6, 118, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 3, 82, 1);
            this.CreateSkill(SkillNumber.PoisonStrengthener, "Poison Strengthener", 30, 3, 46, 0, 6, 100, 0, 1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 3, 85, 1);
            this.CreateSkill(SkillNumber.EvilSpiritStreng, "Evil Spirit Streng", 50, 22, 108, 0, 6, 100, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 4, 86, 1);
            this.CreateSkill(SkillNumber.MagicMasteryGrandMaster, "Magic Mastery", 50, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0, 4, 87, 1);
            this.CreateSkill(SkillNumber.DecayStrengthener, "Decay Strengthener", 96, 22, 120, 10, 6, 243, 0, 1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 4, 88, 1);
            this.CreateSkill(SkillNumber.HellfireStrengthener, "Hellfire Strengthener", 60, 3, 176, 0, 0, 100, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 5, 89, 1);
            this.CreateSkill(SkillNumber.IceStrengthener, "Ice Strengthener", 25, 3, 42, 0, 6, 100, 0, 0, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 5, 90, 1);
            this.CreateSkill(SkillNumber.MeteorStrengthener, "Meteor Strengthener", 21, 4, 13, 0, 6, 100, 0, 4, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 6, 210, 1);
            this.CreateSkill(SkillNumber.IceStormStrengthener, "Ice Storm Strengthener", 93, 22, 110, 5, 6, 223, 0, 0, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 6, 92, 1);
            this.CreateSkill(SkillNumber.NovaStrengthener, "Nova Strengthener", 100, 22, 198, 49, 6, 258, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 7, 93, 1);
            this.CreateSkill(SkillNumber.IceStormMastery, "Ice Storm Mastery", 93, 1, 110, 5, 6, 223, 0, 0, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 7, 92, 1);
            this.CreateSkill(SkillNumber.MeteorMastery, "Meteor Mastery", 21, 1, 14, 0, 6, 100, 0, 4, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 8, 210, 1);
            this.CreateSkill(SkillNumber.NovaCastStrengthener, "Nova Cast Strengthener", 100, 22, 198, 49, 6, 258, 0, 3, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 7, 93, 1);
            this.CreateSkill(SkillNumber.OneHandedStaffStrengthener, "One-handed Staff Stren", 0, 22, 0, 0, 0, 0, 0, -1, 1, 3, 0, 3, 0, 0, 3, 0, 0, 0, 2, 96, 1);
            this.CreateSkill(SkillNumber.TwoHandedStaffStrengthener, "Two-handed Staff Stren", 0, 4, 0, 0, 0, 0, 0, -1, 1, 3, 0, 3, 0, 0, 3, 0, 0, 0, 2, 97, 1);
            this.CreateSkill(SkillNumber.ShieldStrengthenerGrandMaster, "Shield Strengthener", 0, 10, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0, 2, 98, 1);
            this.CreateSkill(SkillNumber.OneHandedStaffMaster, "One-handed Staff Mast", 0, 23, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 3, 0, 0, 0, 3, 99, 1);
            this.CreateSkill(SkillNumber.TwoHandedStaffMaster, "Two-handed Staff Mast", 0, 5, 0, 0, 0, 0, 0, -1, 1, 3, 0, 3, 0, 0, 3, 0, 0, 0, 3, 100, 1);
            this.CreateSkill(SkillNumber.ShieldMasteryGrandMaster, "Shield Mastery", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 0, 0, 0, 0, 3, 101, 1);
            this.CreateSkill(SkillNumber.SoulBarrierStrength, "Soul Barrier Strength", 77, 7, 77, 24, 6, 126, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 4, 102, 1);
            this.CreateSkill(SkillNumber.SoulBarrierProficie, "Soul Barrier Proficie", 77, 10, 84, 26, 6, 126, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 5, 102, 1);
            this.CreateSkill(SkillNumber.MinimumWizardryInc, "Minimum Wizardry Inc", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 3, 0, 0, 0, 5, 103, 1);
            this.CreateSkill(SkillNumber.SoulBarrierMastery, "Soul Barrier Mastery", 77, 7, 92, 28, 6, 126, 0, -1, 1, 4, 0, 3, 0, 0, 0, 0, 0, 0, 6, 102, 1);
            this.CreateSkill(SkillNumber.MaximumWizardryInc, "Maximum Wizardry Inc", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 3, 0, 0, 3, 0, 0, 0, 6, 104, 1);

            // High Elf:
            this.CreateSkill(SkillNumber.IllusionWingsAbsPowUp, "Illusion Wings Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 8, 106, 1);
            this.CreateSkill(SkillNumber.IllusionWingsDefPowUp, "Illusion Wings Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 8, 107, 1);
            this.CreateSkill(SkillNumber.MultiShotStreng, "Multi-Shot Streng", 100, 22, 11, 7, 6, 0, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 6, 211, 1);
            this.CreateSkill(SkillNumber.IllusionWingsAttPowUp, "Illusion Wings Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 9, 109, 1);
            this.CreateSkill(SkillNumber.HealStrengthener, "Heal Strengthener", 8, 22, 22, 0, 6, 100, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 2, 110, 1);
            this.CreateSkill(SkillNumber.TripleShotStrengthener, "Triple Shot Strengthener", 0, 22, 5, 0, 6, 0, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 2, 111, 1);
            this.CreateSkill(SkillNumber.SummonedMonsterStr1, "Summoned Monster Str (1)", 0, 16, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 2, 112, 1);
            this.CreateSkill(SkillNumber.PenetrationStrengthener, "Penetration Strengthener", 130, 22, 10, 11, 6, 0, 0, 5, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 3, 113, 1);
            this.CreateSkill(SkillNumber.DefenseIncreaseStr, "Defense Increase Str", 13, 22, 33, 0, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 3, 114, 1);
            this.CreateSkill(SkillNumber.TripleShotMastery, "Triple Shot Mastery", 0, 0, 9, 0, 6, 0, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 3, 111, 10);
            this.CreateSkill(SkillNumber.SummonedMonsterStr2, "Summoned Monster Str (2)", 0, 16, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 3, 115, 1);
            this.CreateSkill(SkillNumber.AttackIncreaseStr, "Attack Increase Str", 18, 22, 44, 0, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 4, 116, 1);
            this.CreateSkill(SkillNumber.WeaponMasteryHighElf, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 4, 117, 1);
            this.CreateSkill(SkillNumber.AttackIncreaseMastery, "Attack Increase Mastery", 18, 22, 48, 0, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 5, 116, 1);
            this.CreateSkill(SkillNumber.DefenseIncreaseMastery, "Defense Increase Mastery", 13, 22, 36, 0, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 5, 114, 1);
            this.CreateSkill(SkillNumber.IceArrowStrengthener, "Ice Arrow Strengthener", 0, 22, 15, 18, 8, 0, 0, 0, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 5, 118, 1);
            this.CreateSkill(SkillNumber.Cure, "Cure", 0, 0, 72, 10, 6, 0, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 6, 119, 10);
            this.CreateSkill(SkillNumber.PartyHealing, "Party Healing", 0, 0, 66, 12, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 7, 120, 10);
            this.CreateSkill(SkillNumber.PoisonArrow, "Poison Arrow", 0, 27, 22, 27, 6, 0, 0, 1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 8, 121, 10);
            this.CreateSkill(SkillNumber.SummonedMonsterStr3, "Summoned Monster Str (3)", 0, 16, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 7, 122, 1);
            this.CreateSkill(SkillNumber.PartyHealingStr, "Party Healing Str", 0, 22, 72, 13, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 8, 120, 1);
            this.CreateSkill(SkillNumber.Bless, "Bless", 0, 0, 108, 18, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 8, 123, 10);
            this.CreateSkill(SkillNumber.MultiShotMastery, "Multi-Shot Mastery", 100, 1, 12, 8, 6, 0, 0, -1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 7, 211, 1);
            this.CreateSkill(SkillNumber.SummonSatyros, "Summon Satyros", 0, 0, 525, 52, 0, 280, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 8, 125, 10);
            this.CreateSkill(SkillNumber.BlessStrengthener, "Bless Strengthener", 0, 10, 118, 20, 6, 100, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 9, 123, 1);
            this.CreateSkill(SkillNumber.PoisonArrowStr, "Poison Arrow Str", 0, 22, 24, 29, 6, 0, 0, 1, 0, 4, 0, 0, 0, 3, 0, 0, 0, 0, 9, 121, 1);
            this.CreateSkill(SkillNumber.BowStrengthener, "Bow Strengthener", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 2, 126, 1);
            this.CreateSkill(SkillNumber.CrossbowStrengthener, "Crossbow Strengthener", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 2, 127, 1);
            this.CreateSkill(SkillNumber.ShieldStrengthenerHighElf, "Shield Strengthener", 0, 10, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 2, 128, 1);
            this.CreateSkill(SkillNumber.BowMastery, "Bow Mastery", 0, 23, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 3, 129, 1);
            this.CreateSkill(SkillNumber.CrossbowMastery, "Crossbow Mastery", 0, 5, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 3, 130, 1);
            this.CreateSkill(SkillNumber.ShieldMasteryHighElf, "Shield Mastery", 0, 15, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 3, 0, 0, 0, 0, 3, 131, 1);
            this.CreateSkill(SkillNumber.InfinityArrowStr, "Infinity Arrow Str", 220, 1, 55, 11, 6, 0, 0, -1, -1, 4, 0, 0, 0, 3, 0, 0, 0, 0, 5, 132, 1);
            this.CreateSkill(SkillNumber.MinimumAttPowerInc, "Minimum Att Power Inc", 0, 22, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 0, 3, 0, 0, 0, 0, 5, 133, 1);
            this.CreateSkill(SkillNumber.MaximumAttPowerInc, "Maximum Att Power Inc", 0, 3, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 0, 3, 0, 0, 0, 0, 6, 134, 1);

            // Dimension Master (Summoner):
            this.CreateSkill(SkillNumber.DimensionWingsAbsPowUp, "DimensionWings Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 8, 136, 1);
            this.CreateSkill(SkillNumber.DimensionWingsDefPowUp, "DimensionWings Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 8, 137, 1);
            this.CreateSkill(SkillNumber.DimensionWingsAttPowUp, "DimensionWings Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 9, 138, 1);
            this.CreateSkill(SkillNumber.FireTomeStrengthener, "Fire Tome Strengthener", 0, 3, 0, 0, 0, 0, 0, 3, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 2, 139, 1);
            this.CreateSkill(SkillNumber.WindTomeStrengthener, "Wind Tome Strengthener", 0, 3, 0, 0, 0, 0, 0, 5, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 2, 140, 1);
            this.CreateSkill(SkillNumber.LightningTomeStren, "Lightning Tome Stren", 0, 3, 0, 0, 0, 0, 0, 2, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 2, 141, 1);
            this.CreateSkill(SkillNumber.FireTomeMastery, "Fire Tome Mastery", 0, 7, 0, 0, 0, 0, 0, 3, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 3, 139, 1);
            this.CreateSkill(SkillNumber.WindTomeMastery, "Wind Tome Mastery", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 3, 140, 1);
            this.CreateSkill(SkillNumber.LightningTomeMastery, "Lightning Tome Mastery", 0, 7, 0, 0, 0, 0, 0, 2, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 3, 141, 1);
            this.CreateSkill(SkillNumber.SleepStrengthener, "Sleep Strengthener", 40, 1, 30, 7, 6, 100, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 3, 142, 1);
            this.CreateSkill(SkillNumber.ChainLightningStr, "Chain Lightning Str", 75, 22, 103, 0, 6, 75, 0, 2, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 4, 143, 1);
            this.CreateSkill(SkillNumber.LightningShockStr, "Lightning Shock Str", 93, 22, 125, 10, 6, 216, 0, 2, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 4, 144, 1);
            this.CreateSkill(SkillNumber.MagicMasterySummoner, "Magic Mastery", 0, 22, 0, 0, 0, 0, 0, -1, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 5, 145, 1);
            this.CreateSkill(SkillNumber.DrainLifeStrengthener, "Drain Life Strengthener", 35, 22, 57, 0, 6, 93, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 5, 146, 1);
            this.CreateSkill(SkillNumber.WeaknessStrengthener, "Weakness Strengthener", 93, 3, 55, 17, 6, 173, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 7, 147, 1);
            this.CreateSkill(SkillNumber.InnovationStrengthener, "Innovation Strengthener", 111, 3, 77, 17, 6, 201, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 6, 148, 1);
            this.CreateSkill(SkillNumber.Blind, "Blind", 0, 0, 115, 25, 3, 201, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 8, 149, 10);
            this.CreateSkill(SkillNumber.DrainLifeMastery, "Drain Life Mastery", 35, 17, 62, 0, 6, 93, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 7, 146, 1);
            this.CreateSkill(SkillNumber.BlindStrengthener, "Blind Strengthener", 0, 1, 126, 27, 3, 201, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 9, 149, 1);
            this.CreateSkill(SkillNumber.StickStrengthener, "Stick Strengthener", 0, 22, 0, 0, 0, 0, 0, -1, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 2, 150, 1);
            this.CreateSkill(SkillNumber.OtherWorldTomeStreng, "Other World Tome Streng", 0, 3, 0, 0, 0, 0, 0, -1, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 2, 151, 1);
            this.CreateSkill(SkillNumber.StickMastery, "Stick Mastery", 0, 5, 0, 0, 0, 0, 0, -1, 1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 3, 152, 1);
            this.CreateSkill(SkillNumber.OtherWorldTomeMastery, "Other World Tome Mastery", 0, 23, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 3, 153, 1);
            this.CreateSkill(SkillNumber.BerserkerStrengthener, "Berserker Strengthener", 83, 7, 150, 75, 5, 181, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 4, 154, 1);
            this.CreateSkill(SkillNumber.BerserkerProficiency, "Berserker Proficiency", 83, 7, 165, 82, 5, 181, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 5, 154, 1);

            // Duel Master (MG):
            this.CreateSkill(SkillNumber.MinimumWizCurseInc, "Minimum Wiz/Curse Inc", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 5, 155, 1);
            this.CreateSkill(SkillNumber.BerserkerMastery, "Berserker Mastery", 83, 10, 181, 90, 5, 181, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 3, 0, 6, 154, 1);
            this.CreateSkill(SkillNumber.MaximumWizCurseInc, "Maximum Wiz/Curse Inc", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 3, 0, 6, 156, 1);
            this.CreateSkill(SkillNumber.WingofRuinAbsPowUp, "Wing of Ruin Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0, 8, 158, 1);
            this.CreateSkill(SkillNumber.WingofRuinDefPowUp, "Wing of Ruin Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0, 8, 159, 1);
            this.CreateSkill(SkillNumber.WingofRuinAttPowUp, "Wing of Ruin Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0, 9, 161, 1);
            this.CreateSkill(SkillNumber.CycloneStrengthenerDuelMaster, "Cyclone Strengthener", 0, 22, 9, 0, 2, 0, 0, -1, 0, 4, 0, 0, 0, 0, 3, 0, 0, 0, 2, 162, 1);
            this.CreateSkill(SkillNumber.LightningStrengthenerDuelMaster, "Lightning Strengthener", 13, 3, 20, 0, 6, 100, 0, 2, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 2, 163, 1);
            this.CreateSkill(SkillNumber.TwistingSlashStrengthenerDuelMaster, "Twisting Slash Stren", 0, 3, 10, 10, 2, 0, 0, -1, 0, 4, 0, 0, 0, 0, 3, 0, 0, 0, 2, 164, 1);
            this.CreateSkill(SkillNumber.PowerSlashStreng, "Power Slash Streng", 0, 3, 15, 0, 5, 100, 0, -1, -1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 2, 165, 1);
            this.CreateSkill(SkillNumber.FlameStrengthenerDuelMaster, "Flame Strengthener", 35, 3, 55, 0, 6, 100, 0, 3, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 3, 166, 1);
            this.CreateSkill(SkillNumber.BlastStrengthenerDuelMaster, "Blast Strengthener", 80, 22, 165, 0, 3, 150, 0, 2, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 3, 167, 1);
            this.CreateSkill(SkillNumber.WeaponMasteryDuelMaster, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0, 3, 168, 1);
            this.CreateSkill(SkillNumber.InfernoStrengthenerDuelMaster, "Inferno Strengthener", 88, 22, 220, 0, 0, 200, 0, 3, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 4, 169, 1);
            this.CreateSkill(SkillNumber.EvilSpiritStrengthenerDuelMaster, "Evil Spirit Strengthen", 50, 22, 108, 0, 6, 100, 0, -1, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 4, 170, 1);
            this.CreateSkill(SkillNumber.MagicMasteryDuelMaster, "Magic Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 3, 0, 0, 0, 4, 171, 1);
            this.CreateSkill(SkillNumber.IceStrengthener, "Ice Strengthener", 25, 3, 42, 0, 6, 100, 0, 0, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 5, 172, 1);
            this.CreateSkill(SkillNumber.BloodAttackStrengthen, "Blood Attack Strengthen", 0, 22, 15, 22, 3, 0, 0, 1, -1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 5, 173, 1);
            this.CreateSkill(SkillNumber.IceMasteryDuelMaster, "Ice Mastery", 25, 1, 46, 0, 6, 100, 0, 0, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 6, 172, 1);
            this.CreateSkill(SkillNumber.FlameStrikeStrengthen, "Flame Strike Strengthen", 0, 22, 30, 37, 3, 0, 0, 3, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 6, 175, 1);
            this.CreateSkill(SkillNumber.FireSlashMastery, "Fire Slash Mastery", 0, 7, 17, 24, 3, 0, 0, 1, -1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 7, 173, 1);
            this.CreateSkill(SkillNumber.FlameStrikeMastery, "Flame Strike Mastery", 0, 7, 33, 40, 3, 0, 0, 3, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 7, 175, 1);
            this.CreateSkill(SkillNumber.EarthPrison, "Earth Prison", 0, 26, 180, 15, 3, 127, 0, 4, 1, 4, 0, 3, 0, 0, 3, 0, 0, 0, 8, 178, 10);
            this.CreateSkill(SkillNumber.GiganticStormStr, "Gigantic Storm Str", 220, 22, 132, 11, 6, 118, 0, 5, 1, 4, 0, 0, 0, 0, 3, 0, 0, 0, 7, 180, 1);
            this.CreateSkill(SkillNumber.EarthPrisonStr, "Earth Prison Str", 0, 22, 198, 17, 3, 127, 0, 4, 1, 4, 0, 3, 0, 0, 3, 0, 0, 0, 9, 178, 1);

            // Lord Emperor (DL):
            this.CreateSkill(SkillNumber.EmperorCapeAbsPowUp, "Emperor Cape Abs PowUp", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 8, 182, 1);
            this.CreateSkill(SkillNumber.EmperorCapeDefPowUp, "Emperor Cape Def PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 8, 183, 1);
            this.CreateSkill(SkillNumber.AddsCommandStat, "Adds Command Stat", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 8, 184, 1);
            this.CreateSkill(SkillNumber.EmperorCapeAttPowUp, "Emperor Cape Att PowUp", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 9, 185, 1);
            this.CreateSkill(SkillNumber.FireBurstStreng, "Fire Burst Streng", 74, 22, 25, 0, 6, 20, 0, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 2, 186, 1);
            this.CreateSkill(SkillNumber.ForceWaveStreng, "Force Wave Streng", 0, 3, 15, 0, 4, 0, 0, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 2, 187, 1);
            this.CreateSkill(SkillNumber.DarkHorseStreng1, "Dark Horse Streng (1)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 2, 188, 1);
            this.CreateSkill(SkillNumber.CriticalDMGIncPowUp, "Critical DMG Inc PowUp", 82, 3, 75, 75, 0, 25, 300, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 3, 189, 1);
            this.CreateSkill(SkillNumber.EarthshakeStreng, "Earthshake Streng", 0, 22, 0, 75, 10, 0, 0, 2, 0, 4, 0, 0, 0, 0, 0, 3, 0, 0, 3, 190, 1);
            this.CreateSkill(SkillNumber.WeaponMasteryLordEmperor, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 3, 191, 1);
            this.CreateSkill(SkillNumber.FireBurstMastery, "Fire Burst Mastery", 74, 1, 27, 0, 6, 20, 0, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 4, 186, 1);
            this.CreateSkill(SkillNumber.CritDMGIncPowUp2, "Crit DMG Inc PowUp (2)", 82, 10, 82, 82, 0, 25, 300, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 4, 189, 1);
            this.CreateSkill(SkillNumber.EarthshakeMastery, "Earthshake Mastery", 0, 1, 0, 75, 10, 0, 0, 2, 0, 4, 0, 0, 0, 0, 0, 3, 0, 0, 4, 190, 1);
            this.CreateSkill(SkillNumber.CritDMGIncPowUp3, "Crit DMG Inc PowUp (3)", 82, 7, 100, 100, 0, 25, 300, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 5, 189, 1);
            this.CreateSkill(SkillNumber.FireScreamStren, "Fire Scream Stren", 102, 22, 45, 11, 6, 32, 70, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 5, 192, 1);
            this.CreateSkill(SkillNumber.ElectricSparkStreng, "Electric Spark Streng", 92, 3, 0, 150, 10, 29, 340, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 6, 193, 1);
            this.CreateSkill(SkillNumber.FireScreamMastery, "Fire Scream Mastery", 102, 5, 49, 12, 6, 32, 70, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 6, 192, 1);
            this.CreateSkill(SkillNumber.IronDefenseLordEmperor, "Iron Defense", 0, 28, 64, 29, 0, 0, 0, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 8, 24, 10);
            this.CreateSkill(SkillNumber.CriticalDamageIncM, "Critical Damage Inc M", 82, 1, 110, 110, 0, 25, 300, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 7, 189, 1);
            this.CreateSkill(SkillNumber.ChaoticDiseierStr, "Chaotic Diseier Str", 100, 22, 75, 22, 6, 16, 0, -1, 1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 7, 194, 1);
            this.CreateSkill(SkillNumber.IronDefenseStr, "Iron Defense Str", 0, 3, 70, 31, 0, 0, 0, -1, -1, 4, 0, 0, 0, 0, 0, 3, 0, 0, 9, 24, 1);
            this.CreateSkill(SkillNumber.DarkSpiritStr, "Dark Spirit Str", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 2, 196, 1);
            this.CreateSkill(SkillNumber.ScepterStrengthener, "Scepter Strengthener", 0, 22, 0, 0, 0, 0, 0, -1, 0, 3, 0, 0, 0, 0, 0, 3, 0, 0, 2, 197, 1);
            this.CreateSkill(SkillNumber.ShieldStrengthenerLordEmperor, "Shield Strengthener", 0, 10, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 2, 198, 1);
            this.CreateSkill(SkillNumber.UseScepterPetStr, "Use Scepter : Pet Str", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 2, 199, 1);
            this.CreateSkill(SkillNumber.DarkSpiritStr2, "Dark Spirit Str (2)", 0, 7, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 3, 200, 1);
            this.CreateSkill(SkillNumber.ScepterMastery, "Scepter Mastery", 0, 5, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 3, 201, 1);
            this.CreateSkill(SkillNumber.ShieldMastery, "Shield Mastery", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 3, 202, 1);
            this.CreateSkill(SkillNumber.CommandAttackInc, "Command Attack Inc", 0, 20, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 3, 203, 1);
            this.CreateSkill(SkillNumber.DarkSpiritStr3, "Dark Spirit Str (3)", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 5, 204, 1);
            this.CreateSkill(SkillNumber.PetDurabilityStr, "Pet Durability Str", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 5, 205, 1);
            this.CreateSkill(SkillNumber.DarkSpiritStr4, "Dark Spirit Str (4)", 0, 23, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 6, 206, 1);
            this.CreateSkill(SkillNumber.DarkSpiritStr5, "Dark Spirit Str (5)", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 7, 208, 1);
            this.CreateSkill(SkillNumber.SpiritLord, "Spirit Lord", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 3, 0, 0, 8, 209, 1);

            // Fist Master (Rage Fighter):
            this.CreateSkill(SkillNumber.KillingBlowStrengthener, "Killing Blow Strengthener", 0, 22, 10, 0, 2, 0, 0, 4, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 2, 210, 1);
            this.CreateSkill(SkillNumber.BeastUppercutStrengthener, "Beast Uppercut Strengthener", 0, 22, 10, 0, 2, 0, 0, 3, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 2, 211, 1);
            this.CreateSkill(SkillNumber.KillingBlowMastery, "Killing Blow Mastery", 0, 1, 10, 0, 2, 0, 0, 4, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 3, 210, 1);
            this.CreateSkill(SkillNumber.BeastUppercutMastery, "Beast Uppercut Mastery", 0, 1, 10, 0, 2, 0, 0, 3, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 3, 211, 1);
            this.CreateSkill(SkillNumber.WeaponMasteryFistMaster, "Weapon Mastery", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 212, 1);
            this.CreateSkill(SkillNumber.ChainDriveStrengthener, "Chain Drive Strengthener", 150, 22, 22, 22, 4, 0, 0, 0, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 5, 213, 1);
            this.CreateSkill(SkillNumber.DarkSideStrengthener, "Dark Side Strengthener", 180, 22, 84, 0, 4, 0, 0, 5, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 5, 214, 1);
            this.CreateSkill(SkillNumber.DragonRoarStrengthener, "Dragon Roar Strengthener", 150, 22, 60, 33, 3, 0, 0, 4, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 5, 215, 1);
            this.CreateSkill(SkillNumber.EquippedWeaponStrengthener, "Equipped Weapon Strengthener", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 2, 216, 1);
            this.CreateSkill(SkillNumber.DefSuccessRateIncPowUp, "Def SuccessRate IncPowUp", 50, 22, 55, 11, 7, 30, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 2, 217, 1);
            this.CreateSkill(SkillNumber.EquippedWeaponMastery, "Equipped Weapon Mastery", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 3, 218, 1);
            this.CreateSkill(SkillNumber.DefSuccessRateIncMastery, "DefSuccessRate IncMastery", 50, 22, 60, 12, 7, 30, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 3, 217, 1);
            this.CreateSkill(SkillNumber.StaminaIncreaseStrengthener, "Stamina Increase Strengthener", 80, 5, 55, 11, 7, 35, 0, -1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 3, 4, 219, 1);
            this.CreateSkill(SkillNumber.DurabilityReduction1FistMaster, "Durability Reduction (1)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 1, 220, 1);
            this.CreateSkill(SkillNumber.IncreasePvPDefenseRate, "Increase PvP Defense Rate", 0, 29, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 1, 221, 1);
            this.CreateSkill(SkillNumber.IncreaseMaximumSD, "Increase Maximum SD", 0, 33, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 2, 222, 1);
            this.CreateSkill(SkillNumber.IncreaseManaRecoveryRate, "Increase Mana Recovery Rate", 0, 7, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 2, 223, 1);
            this.CreateSkill(SkillNumber.IncreasePoisonResistance, "Increase Poison Resistance", 0, 1, 0, 0, 0, 0, 0, 1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 2, 224, 1);
            this.CreateSkill(SkillNumber.DurabilityReduction2FistMaster, "Durability Reduction (2)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 3, 225, 1);
            this.CreateSkill(SkillNumber.IncreaseSDRecoveryRate, "Increase SD Recovery Rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 3, 226, 1);
            this.CreateSkill(SkillNumber.IncreaseHPRecoveryRate, "Increase HP Recovery Rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 3, 227, 1);
            this.CreateSkill(SkillNumber.IncreaseLightningResistance, "Increase Lightning Resistance", 0, 1, 0, 0, 0, 0, 0, 2, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 3, 228, 1);
            this.CreateSkill(SkillNumber.IncreasesDefense, "Increases Defense", 0, 35, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 229, 1);
            this.CreateSkill(SkillNumber.IncreasesAGRecoveryRate, "Increases AG Recovery Rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 230, 1);
            this.CreateSkill(SkillNumber.IncreaseIceResistance, "Increase Ice Resistance", 0, 1, 0, 0, 0, 0, 0, 0, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 231, 1);
            this.CreateSkill(SkillNumber.DurabilityReduction3FistMaster, "Durability Reduction(3)", 0, 17, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 5, 232, 1);
            this.CreateSkill(SkillNumber.IncreaseDefenseSuccessRate, "Increase Defense Success Rate", 0, 1, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 5, 233, 1);
            this.CreateSkill(SkillNumber.CastInvincibilityFistMaster, "Cast Invincibility", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 6, 234, 1);
            this.CreateSkill(SkillNumber.IncreaseAttackSuccessRate, "Increase Attack Success Rate", 0, 30, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 1, 235, 1);
            this.CreateSkill(SkillNumber.IncreaseMaximumHP, "Increase Maximum HP", 0, 34, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 236, 1);
            this.CreateSkill(SkillNumber.IncreaseMaximumMana, "Increase Maximum Mana", 0, 34, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 5, 237, 1);
            this.CreateSkill(SkillNumber.IncreaseMaximumAG, "Increase Maximum AG", 0, 37, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 6, 238, 1);
            this.CreateSkill(SkillNumber.IncreasePvPAttackRate, "Increase PvP Attack Rate", 0, 31, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 1, 239, 1);
            this.CreateSkill(SkillNumber.DecreaseMana, "Decrease Mana", 0, 18, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 240, 1);
            this.CreateSkill(SkillNumber.RecoverSDfromMonsterKills, "Recover SD from Monster Kills", 0, 11, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 241, 1);
            this.CreateSkill(SkillNumber.RecoverHPfromMonsterKills, "Recover HP from Monster Kills", 0, 6, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 4, 242, 1);
            this.CreateSkill(SkillNumber.IncreaseMinimumAttackPower, "Increase Minimum Attack Power", 0, 22, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 5, 243, 1);
            this.CreateSkill(SkillNumber.RecoverManaMonsterKills, "Recover Mana Monster Kills", 0, 6, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 5, 244, 1);
            this.CreateSkill(SkillNumber.IncreaseMaximumAttackPower, "Increase Maximum Attack Power", 0, 3, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 6, 245, 1);
            this.CreateSkill(SkillNumber.IncreasesCritDamageChance, "Increases Crit Damage Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 6, 246, 1);
            this.CreateSkill(SkillNumber.RecoverManaFully, "Recover Mana Fully", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 7, 247, 1);
            this.CreateSkill(SkillNumber.RecoversHPFully, "Recovers HP Fully", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 7, 248, 1);
            this.CreateSkill(SkillNumber.IncreaseExcDamageChance, "Increase Exc Damage Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 7, 249, 1);
            this.CreateSkill(SkillNumber.IncreaseDoubleDamageChance, "Increase Double Damage Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 8, 250, 1);
            this.CreateSkill(SkillNumber.IncreaseIgnoreDefChance, "Increase Ignore Def Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 9, 251, 1);
            this.CreateSkill(SkillNumber.RecoversSDFully, "Recovers SD Fully", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 8, 252, 1);
            this.CreateSkill(SkillNumber.IncreaseTripleDamageChance, "Increase Triple Damage Chance", 0, 38, 0, 0, 0, 0, 0, -1, -1, 3, 0, 0, 0, 0, 0, 0, 0, 3, 9, 253, 1);

            this.InitializeEffects();
            this.MapSkillsToEffects();
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

        private void CreateSkill(SkillNumber skillId, string name, int levelRequirement, int damage, int manaConsumption, int abilityConsumption, short distance, int energyRequirement, int leadershipRequirement, int elementalModifier, int attackType, int useType, int count, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel, int ragefighterClassLevel, int rank, int group, int masterp, SkillType skillType = SkillType.DirectHit, SkillTarget skillTarget = SkillTarget.Explicit, short implicitTargetRange = 0, SkillTargetRestriction targetRestriction = SkillTargetRestriction.Undefined, bool movesToTarget = false, bool movesTarget = false)
        {
            var skill = this.Context.CreateNew<Skill>();
            this.GameConfiguration.Skills.Add(skill);
            skill.Number = (short)skillId;
            skill.Name = name;
            skill.MovesToTarget = movesToTarget;
            skill.MovesTarget = movesTarget;
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
            skill.ImplicitTargetRange = implicitTargetRange;
            skill.Target = skillTarget;
            skill.TargetRestriction = targetRestriction;
            var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, ragefighterClassLevel);
            foreach (var characterClass in classes)
            {
                skill.QualifiedCharacters.Add(characterClass);
            }

            // TODO: Master skill related stuff?
        }
    }
}
