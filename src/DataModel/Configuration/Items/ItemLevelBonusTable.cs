// <copyright file="ItemLevelBonusTable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines a table of item level related bonus values for <see cref="ItemBasePowerUpDefinition"/>s.
/// </summary>
[Cloneable]
public partial class ItemLevelBonusTable
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bonus per level.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<LevelBonus> BonusPerLevel { get; protected set; } = null!;
}