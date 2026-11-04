// <copyright file="ItemLevelBonusTable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Defines a table of item level related bonus values for <see cref="ItemBasePowerUpDefinition"/>s.
/// </summary>
[Cloneable]
public partial class ItemLevelBonusTable
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public LocalizedString Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public LocalizedString Description { get; set; }

    /// <summary>
    /// Gets or sets the bonus per level.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<LevelBonus> BonusPerLevel { get; protected set; } = null!;
}