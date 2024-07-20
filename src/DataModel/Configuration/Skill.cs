// <copyright file="Skill.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;
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
    /// The damage of the fenrir pet.
    /// </summary>
    Fenrir = 4,

    /// <summary>
    /// All damage types.
    /// </summary>
    All = 5,
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
    /// Area skill, which only hits the explicit target. No declaration of hits by the client.
    /// </summary>
    AreaSkillExplicitTarget = 5,

    /// <summary>
    /// The nova skill which hits all targets in range and applies some bonus damage.
    /// </summary>
    Nova = 6,

    /// <summary>
    /// The buff skill type. Applies magic effects on players.
    /// </summary>
    Buff = 10,

    /// <summary>
    /// The regeneration skill type. Regenerates the target attribute of the defined effect.
    /// </summary>
    Regeneration = 11,

    /// <summary>
    /// The passive boost skill type. Applies boosts to the player who has learned this skill, without the need to be casted.
    /// </summary>
    PassiveBoost = 20,

    /// <summary>
    /// The skill type for monster summoning.
    /// </summary>
    SummonMonster = 30,

    /// <summary>
    /// Other skill type.
    /// </summary>
    Other = 40,
}

/// <summary>
/// Defines how the target(s) of a skill are determined.
/// </summary>
public enum SkillTarget
{
    /// <summary>
    /// The target selection is undefined.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The skill target is stated explicitly.
    /// </summary>
    Explicit = 1,

    /// <summary>
    /// The targets are implicitly all party member which are in view range of the attacker.
    /// </summary>
    ImplicitParty = 2,

    /// <summary>
    /// The targets are implicitly all players which are in <see cref="Skill.ImplicitTargetRange"/> of the attacker.
    /// </summary>
    ImplicitPlayersInRange = 3,

    /// <summary>
    /// The targets are implicitly all non-player-characters in <see cref="Skill.ImplicitTargetRange"/> of the attacker.
    /// </summary>
    ImplicitNpcsInRange = 4,

    /// <summary>
    /// The targets are implicitly all objects in <see cref="Skill.ImplicitTargetRange"/> of the attacker.
    /// </summary>
    ImplicitAllInRange = 5,

    /// <summary>
    /// The primary target is stated explicitly, additional targets are all objects in the <see cref="Skill.ImplicitTargetRange"/> of the primary target.
    /// </summary>
    ExplicitWithImplicitInRange = 6,

    /// <summary>
    /// The skill target is only the own player implicitly.
    /// </summary>
    ImplicitPlayer = 7,
}

/// <summary>
/// Defines how a skill is restricted to specific targets.
/// </summary>
public enum SkillTargetRestriction
{
    /// <summary>
    /// Undefined restriction. Skill can be applied to all possible entities (players, NPCs, etc.).
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The skill can only be applied to the executor.
    /// </summary>
    Self = 1,

    /// <summary>
    /// The skill can only be applied to the executor or its party members.
    /// </summary>
    Party = 2,

    /// <summary>
    /// The skill can only be applied to players (and summoned monsters of a player).
    /// </summary>
    Player = 3,
}

/// <summary>
/// Defines a skill.
/// </summary>
[Cloneable]
public partial class Skill
{
    /// <summary>
    /// Gets or sets the skill number.
    /// </summary>
    /// <remarks>
    /// The client is referencing skills by this number.
    /// </remarks>
    public short Number { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the requirements to execute the skill.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<AttributeRequirement> Requirements { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the attributes which values will be consumed by executing this skill.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<AttributeRequirement> ConsumeRequirements { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the maximum range between executor of the skill and the target object.
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
    /// Gets or sets the <see cref="SkillTarget"/> which defines how the target(s) of a skill are determined.
    /// </summary>
    public SkillTarget Target { get; set; }

    /// <summary>
    /// Gets or sets the range for automatic targeting of additional target.
    /// Has only effect if greater than <c>0</c>.
    /// </summary>
    /// <remarks>
    /// Possible use cases: Additional hits for the "Fireburst" or "Deathstab" skills. They use direct targeting, but also hit nearby enemies.
    /// </remarks>
    public short ImplicitTargetRange { get; set; }

    /// <summary>
    /// Gets or sets the target restriction.
    /// </summary>
    public SkillTargetRestriction TargetRestriction { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the skill moves the attacker to the target.
    /// </summary>
    /// <remarks>Used by dark knight weapon skills, e.g. Slash.</remarks>
    public bool MovesToTarget { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the skill moves the target.
    /// </summary>
    /// <remarks>
    /// Used by dark knight weapon physical attack skills, e.g. Slash. The target gets pushed around randomly.
    /// This is not a use case for the lightning skill, since resistances play a role there.
    /// </remarks>
    public bool MovesTarget { get; set; }

    /// <summary>
    /// Gets or sets the elemental modifier target attribute.
    /// If this is set, hitting the target (successfully or not) may apply additional effects.
    /// A value of <c>1.0f</c> means, the target is immune to effects of this element.
    /// </summary>
    public virtual AttributeDefinition? ElementalModifierTarget { get; set; }

    /// <summary>
    /// Gets or sets the magic effect definition. It will be applied for buff skills.
    /// </summary>
    public virtual MagicEffectDefinition? MagicEffectDef { get; set; }

    /// <summary>
    /// Gets or sets the character classes which are qualified to learn and use this skill.
    /// </summary>
    public virtual ICollection<CharacterClass> QualifiedCharacters { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the master skill definition. Only relevant for master skills.
    /// </summary>
    [MemberOfAggregate]
    public virtual MasterSkillDefinition? MasterDefinition { get; set; }

    /// <summary>
    /// Gets or sets the attack damage. Only relevant for attack skills.
    /// </summary>
    public int AttackDamage { get; set; }
}