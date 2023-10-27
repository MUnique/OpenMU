// <copyright file="MasterSkillDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The definition of a master skill. One skill can have 0-n master skill definitions,
/// for example one skill can be used for different character classes at different Rank-Levels at different Roots.
/// </summary>
[Cloneable]
public partial class MasterSkillDefinition
{
    /// <summary>
    /// Gets or sets the root.
    /// </summary>
    [Required]
    public virtual MasterSkillRoot? Root { get; set; }

    /// <summary>
    /// Gets or sets a collection with the required skills.
    /// Just one skill (of at least level 10) of this list is required
    /// to meet the requirements when learning this skill.
    /// </summary>
    public virtual ICollection<Skill> RequiredMasterSkills { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the rank.
    /// The rang determines on which level the skill is located.
    /// A skill at a higher rank can be learned, if there is at least
    /// one skill of the same tree root at the direct rank below,
    /// with a level same or greater than 10.
    /// </summary>
    public byte Rank { get; set; }

    /// <summary>
    /// Gets or sets the maximum level.
    /// </summary>
    /// <remarks>
    /// Usually it's 20, but for some skills it's 10.
    /// </remarks>
    public byte MaximumLevel { get; set; }

    /// <summary>
    /// Gets or sets the minimum level which is required until the skill gets active.
    /// It's also the number of master points which are initially required to learn the skill.
    /// </summary>
    public byte MinimumLevel { get; set; }

    /// <summary>
    /// Gets or sets the formula to calculate the effective value, depending on the level of the master skill.
    /// </summary>
    /// <remarks>
    /// We use the syntax of MathParser.org.
    /// To use the level in the formula, use the argument "level".
    /// </remarks>
    public string ValueFormula { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the formula to calculate the visible value, depending on the level of the master skill.
    /// </summary>
    /// <remarks>
    /// We use the syntax of MathParser.org.
    /// To use the level in the formula, use the argument "level".
    /// </remarks>
    public string DisplayValueFormula { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the target attribute of a passive skill boost.
    /// </summary>
    public virtual AttributeDefinition? TargetAttribute { get; set; }

    /// <summary>
    /// Gets or sets the type of how the calculated value is aggregated to the <see cref="TargetAttribute"/>.
    /// </summary>
    public AggregateType Aggregation { get; set; }

    /// <summary>
    /// Gets or sets the replaced skill. If this skill is defined, this master skill replaces it in the skill list.
    /// The attack damage is also inherited and increased by the damage AND value of the master skill.
    /// </summary>
    public virtual Skill? ReplacedSkill { get; set; }
}