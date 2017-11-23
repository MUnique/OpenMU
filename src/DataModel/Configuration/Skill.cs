// <copyright file="Skill.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    using System.Collections.Generic;

    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Defines the damage type of a skill.
    /// </summary>
    public enum DamageType
    {
        /// <summary>
        /// No damage type.
        /// </summary>
        None = -1,

        /// <summary>
        /// The physical damage type.
        /// </summary>
        Physical = 0,

        /// <summary>
        /// The wizardry damage type.
        /// </summary>
        Wizardry = 1,

        /// <summary>
        /// The curse damage type.
        /// </summary>
        Curse = 2,

        /// <summary>
        /// The summoned monster damage type.
        /// </summary>
        SummonedMonster = 3,

        /// <summary>
        /// All damage types.
        /// </summary>
        All = 4
    }

    /// <summary>
    /// The skill types.
    /// </summary>
    public enum SkillType
    {
        /// <summary>
        /// The skill hit its target directly.
        /// </summary>
        DirectHit = 0,

        /// <summary>
        /// The castle siege special skill.
        /// </summary>
        CastleSiegeSpecial = 1,

        /// <summary>
        /// Same as <see cref="DirectHit"/> but only applyable during castle siege event.
        /// </summary>
        CastleSiegeSkill = 2,

        /// <summary>
        /// Area skill damage, which automatically hit all targets in its target area. No declaration of hits by the client.
        /// </summary>
        AreaSkillAutomaticHits = 3,

        /// <summary>
        /// Area skill damage, but the hits have to be declared by the client.
        /// </summary>
        AreaSkillExplicitHits = 4,

        /// <summary>
        /// The buff skill type. Applies magic effects on players.
        /// </summary>
        Buff = 5,

        /// <summary>
        /// The passive boost skill type. Applies boosts to the player who has learned this skill, without the need to be casted.
        /// </summary>
        PassiveBoost = 6,

        /// <summary>
        /// Other skill type.
        /// </summary>
        Other = 7
    }

    /// <summary>
    /// Defines a skill.
    /// </summary>
    public class Skill
    {
        /// <summary>
        /// Gets or sets the skill identifier.
        /// </summary>
        /// <remarks>
        /// The client is referencing skills by this id.
        /// </remarks>
        public short SkillID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the requirements to execute the skill.
        /// </summary>
        public virtual ICollection<AttributeRequirement> Requirements { get; protected set; }

        /// <summary>
        /// Gets or sets the attributes which values will be consumed by executing this skill.
        /// </summary>
        public virtual ICollection<AttributeRequirement> ConsumeRequirements { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum range between executer of the skill and the target object.
        /// </summary>
        public short Range { get; set; }

        /// <summary>
        /// Gets or sets the type of the damage.
        /// </summary>
        public DamageType DamageType { get; set; }

        /// <summary>
        /// Gets or sets the type of the skill.
        /// </summary>
        public SkillType SkillType { get; set; }

        /// <summary>
        /// Gets or sets the magic effect definition. It will be applied for buff skills.
        /// </summary>
        public virtual MagicEffectDefinition MagicEffectDef { get; set; }

        /// <summary>
        /// Gets or sets the character classes which are qualified to learn and use this skill.
        /// </summary>
        public virtual ICollection<CharacterClass> QualifiedCharacters { get; protected set; }

        /// <summary>
        /// Gets or sets the master definitions. Only relevant for master skills.
        /// </summary>
        public virtual ICollection<MasterSkillDefinition> MasterDefinitions { get; protected set; }

        /// <summary>
        /// Gets or sets the passive power ups, depending on the skill level. Only relevant for skills of type <see cref="SkillType.PassiveBoost"/>.
        /// </summary>
        public virtual IDictionary<int, PowerUpDefinition> PassivePowerUps { get; protected set; }

        /// <summary>
        /// Gets or sets the level dependent attack damage. Only relevant for attack skills.
        /// </summary>
        public virtual ICollection<LevelDependentDamage> AttackDamage { get; protected set; }
    }
}
