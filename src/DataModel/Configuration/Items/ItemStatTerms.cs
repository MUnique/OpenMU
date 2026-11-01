// <copyright file="ItemStatTerms.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The terms of one attribute in an <see cref="ItemStatProfile"/>: its level bonus table, and what an excellent and an
/// ancient item get more. A part the profile doesn't write is taken from the profile named "default".
/// </summary>
/// <remarks>
/// With d being the item's drop level plus <see cref="GameConfiguration.AncientDropLevelBonus"/>:
/// <list type="bullet">
///   <item>excellent: <c>base * ExcellentTimes / drop level + ExcellentAdd + drop level / ExcellentDropLevelDivisor</c>,</item>
///   <item>ancient: <c>value * AncientTimes / d + AncientAdd + d / AncientDropLevelDivisor</c>, where the value is the base value
///   and the excellent part, and with <see cref="AncientAfterLevelBonus"/> also the level bonus.</item>
/// </list>
/// The parts are integers, computed from left to right; a divisor of 0 leaves its part out.
/// </remarks>
[Cloneable]
public partial class ItemStatTerms
{
    /// <summary>
    /// Gets or sets the attribute of the item's base power-ups which these terms are for: the excellent and the ancient
    /// part are computed from the item's base value of it and added to it, and the level bonus table is the one of its
    /// power-ups. The excellent and the ancient part of the minimum physical damage also apply to the maximum physical
    /// damage, as the item formulas compute both from the minimum damage.
    /// </summary>
    public virtual AttributeDefinition? Attribute { get; set; }

    /// <summary>
    /// Gets or sets whether the profile writes the <see cref="Table"/>.
    /// </summary>
    /// <remarks>
    /// With <see cref="ItemStatTermState.Values"/> and no <see cref="Table"/>, the profile names a table which this configuration doesn't have.
    /// </remarks>
    public ItemStatTermState TableState { get; set; }

    /// <summary>
    /// Gets or sets the level bonus table: what each item level adds to the stat.
    /// </summary>
    public virtual ItemLevelBonusTable? Table { get; set; }

    /// <summary>
    /// Gets or sets whether the profile writes the excellent part.
    /// </summary>
    public ItemStatTermState ExcellentState { get; set; }

    /// <summary>
    /// Gets or sets the factor of the base value in the excellent part.
    /// </summary>
    public int ExcellentTimes { get; set; }

    /// <summary>
    /// Gets or sets what the excellent part adds.
    /// </summary>
    public int ExcellentAdd { get; set; }

    /// <summary>
    /// Gets or sets the divisor of the drop level in the excellent part. 0 leaves this part out.
    /// </summary>
    public int ExcellentDropLevelDivisor { get; set; }

    /// <summary>
    /// Gets or sets whether the profile writes the ancient part.
    /// </summary>
    public ItemStatTermState AncientState { get; set; }

    /// <summary>
    /// Gets or sets the factor of the value in the ancient part.
    /// </summary>
    public int AncientTimes { get; set; }

    /// <summary>
    /// Gets or sets what the ancient part adds.
    /// </summary>
    public int AncientAdd { get; set; }

    /// <summary>
    /// Gets or sets the divisor of the ancient drop level in the ancient part. 0 leaves this part out.
    /// </summary>
    public int AncientDropLevelDivisor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the value of the ancient part includes the level bonus.
    /// </summary>
    public bool AncientAfterLevelBonus { get; set; }

    /// <summary>
    /// Gets or sets what each item level adds after the magic power is halved. Only for the rises of the magic power;
    /// <c>null</c> means it isn't written by the profile.
    /// </summary>
    public int? PerLevelAfterHalving { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return this.Attribute?.ToString() ?? string.Empty;
    }
}
