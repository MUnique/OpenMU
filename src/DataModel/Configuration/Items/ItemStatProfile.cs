// <copyright file="ItemStatProfile.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;

/// <summary>
/// A named set of item formulas: what an item's level, excellent options, ancient set and option level add to its
/// stats and requirements. Items refer to it by <see cref="ItemDefinition.StatProfile"/>.
/// </summary>
/// <remarks>
/// The profile named <see cref="DefaultName"/> writes every part. Every other profile writes only what differs from it;
/// what it doesn't write is taken from the default profile.
/// The names and the parts are the same as in the game client's item stat file, so that they can be exchanged with it.
/// </remarks>
[Cloneable]
public partial class ItemStatProfile
{
    /// <summary>
    /// Gets the name of the profile which writes every part, and whose parts the other profiles take when they don't write them.
    /// </summary>
    public static string DefaultName => "default";

    /// <summary>
    /// Gets or sets the name. It is unique within a configuration.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets what each item level adds to the level requirement. <c>null</c> means it isn't written by the profile.
    /// </summary>
    public int? LevelPerItemLevel { get; set; }

    /// <summary>
    /// Gets or sets what an excellent item needs more as level requirement, when its <see cref="ItemDefinition.ExcellentRaisesLevel"/> is set.
    /// <c>null</c> means it isn't written by the profile.
    /// </summary>
    public int? ExcellentLevelAdd { get; set; }

    /// <summary>
    /// Gets or sets what each option level adds to the strength requirement. <c>null</c> means it isn't written by the profile.
    /// </summary>
    public int? StrengthPerOptionLevel { get; set; }

    /// <summary>
    /// Gets or sets the terms per stat, one for each stat the profile writes.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemStatTerms> StatTerms { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the requirement formulas, one for each stat requirement the profile writes.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemRequirementFormula> RequirementFormulas { get; protected set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        return this.Name;
    }
}
