// <copyright file="SkillsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.Persistence.Initialization.Skills;

    /// <summary>
    /// Initialization logic for <see cref="Skill"/>s.
    /// </summary>
    internal class SkillsInitializer : InitializerBase
    {
        private static readonly IDictionary<SkillNumber, MagicEffectNumber> EffectsOfSkills = new Dictionary<SkillNumber, MagicEffectNumber>
        {
            { SkillNumber.Defense, MagicEffectNumber.ShieldSkill },
            { SkillNumber.GreaterDefense, MagicEffectNumber.GreaterDefense },
            { SkillNumber.GreaterDamage, MagicEffectNumber.GreaterDamage },
            { SkillNumber.Heal, MagicEffectNumber.Heal },
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
        /// Replace by: this.CreateSkill($1, "$2", $3, $4, $5, $6, $7, $9, $10, $11, $12, $13, $15, $19, $20, $21, $22, $23, $24, $25, $26, $27, $28);.
        /// </remarks>
        public override void Initialize()
        {
            this.CreateSkill(SkillNumber.Poison, "Poison", 0, 12, 42, 0, 6, 140, 0, 1, 1, 0, 0, 1, 0, 0);
            this.CreateSkill(SkillNumber.Meteorite, "Meteorite", 0, 21, 12, 0, 6, 104, 0, 4, 1, 0, 0, 1, 0, 0);
            this.CreateSkill(SkillNumber.Lightning, "Lightning", 0, 17, 15, 0, 6, 72, 0, 2, 1, 0, 0, 1, 0, 0);
            this.CreateSkill(SkillNumber.FireBall, "Fire Ball", 0, 8, 3, 0, 6, 40, 0, 3, 1, 0, 0, 1, 0, 0);
            this.CreateSkill(SkillNumber.Flame, "Flame", 0, 25, 50, 0, 6, 160, 0, 3, 1, 0, 0, 1, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.Teleport, "Teleport", 0, 0, 30, 0, 6, 88, 0, -1, 1, 0, 0, 1, 0, 0, SkillType.Other);
            this.CreateSkill(SkillNumber.Ice, "Ice", 0, 10, 38, 0, 6, 120, 0, 0, 1, 0, 0, 1, 0, 0);
            this.CreateSkill(SkillNumber.Twister, "Twister", 0, 35, 60, 0, 6, 180, 0, 5, 1, 0, 0, 1, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.EvilSpirit, "Evil Spirit", 0, 45, 90, 0, 6, 220, 0, -1, 1, 0, 0, 1, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.Hellfire, "Hellfire", 0, 120, 160, 0, 0, 260, 0, 3, 1, 0, 0, 1, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.PowerWave, "Power Wave", 0, 14, 5, 0, 6, 56, 0, -1, 1, 0, 0, 1, 0, 0);
            this.CreateSkill(SkillNumber.AquaBeam, "Aqua Beam", 0, 80, 140, 0, 6, 345, 0, 6, 1, 0, 0, 1, 0, 0, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.EnergyBall, "Energy Ball", 0, 3, 1, 0, 6, 0, 0, -1, 1, 0, 0, 1, 0, 0);
            this.CreateSkill(SkillNumber.Defense, "Defense", 0, 0, 30, 0, 0, 0, 0, -1, -1, 0, 0, 0, 1, 0, SkillType.Buff, SkillTarget.Explicit, 0, SkillTargetRestriction.Self);
            this.CreateSkill(SkillNumber.FallingSlash, "Falling Slash", 0, 0, 9, 0, 3, 0, 0, -1, 0, 0, 0, 0, 1, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Lunge, "Lunge", 0, 0, 9, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Uppercut, "Uppercut", 0, 0, 8, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Cyclone, "Cyclone", 0, 0, 9, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.Slash, "Slash", 0, 0, 10, 0, 2, 0, 0, -1, 0, 0, 0, 0, 1, 0, movesToTarget: true, movesTarget: true);
            this.CreateSkill(SkillNumber.TripleShot, "Triple Shot", 0, 0, 5, 0, 6, 0, 0, -1, 0, 0, 0, 0, 0, 1, SkillType.AreaSkillExplicitHits);
            this.CreateSkill(SkillNumber.Heal, "Heal", 0, 0, 20, 0, 6, 52, 0, -1, 0, 0, 0, 0, 0, 1, SkillType.Regeneration, targetRestriction: SkillTargetRestriction.Player);
            this.CreateSkill(SkillNumber.GreaterDefense, "Greater Defense", 0, 0, 30, 0, 6, 72, 0, -1, -1, 0, 0, 0, 0, 1, SkillType.Buff, targetRestriction: SkillTargetRestriction.Player);
            this.CreateSkill(SkillNumber.GreaterDamage, "Greater Damage", 0, 0, 40, 0, 6, 92, 0, -1, -1, 0, 0, 0, 0, 1, SkillType.Buff, targetRestriction: SkillTargetRestriction.Player);
            this.CreateSkill(SkillNumber.SummonGoblin, "Summon Goblin", 0, 0, 40, 0, 0, 90, 0, -1, -1, 0, 0, 0, 0, 1, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonStoneGolem, "Summon Stone Golem", 0, 0, 70, 0, 0, 170, 0, -1, -1, 0, 0, 0, 0, 1, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonAssassin, "Summon Assassin", 0, 0, 110, 0, 0, 190, 0, -1, -1, 0, 0, 0, 0, 1, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonEliteYeti, "Summon Elite Yeti", 0, 0, 160, 0, 0, 230, 0, -1, -1, 0, 0, 0, 0, 1, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonDarkKnight, "Summon Dark Knight", 0, 0, 200, 0, 0, 250, 0, -1, -1, 0, 0, 0, 0, 1, SkillType.Other);
            this.CreateSkill(SkillNumber.SummonBali, "Summon Bali", 0, 0, 250, 0, 0, 260, 0, -1, -1, 0, 0, 0, 0, 1, SkillType.Other);
            this.CreateSkill(SkillNumber.FlameofEvil, "Flame of Evil (Monster)", 60, 120, 160, 0, 0, 100, 0, -1, -1, 0, 0, 0, 0, 0);

            this.InitializeEffects();
            this.MapSkillsToEffects();
        }

        private void InitializeEffects()
        {
            new DefenseEffectInitializer(this.Context, this.GameConfiguration).Initialize();
            new GreaterDamageEffectInitializer(this.Context, this.GameConfiguration).Initialize();
            new GreaterDefenseEffectInitializer(this.Context, this.GameConfiguration).Initialize();
            new HealEffectInitializer(this.Context, this.GameConfiguration).Initialize();
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
            int attackType, int useType, int count, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel,
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
            var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel == 1, darkKnightClassLevel == 1, elfClassLevel == 1);
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
    }
}
