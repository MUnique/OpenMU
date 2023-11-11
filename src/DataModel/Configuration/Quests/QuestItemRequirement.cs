// <copyright file="QuestItemRequirement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Quests;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Defines the required item(s) which should be in the inventory of the character
/// when the player requests to complete the quest.
/// </summary>
[Cloneable]
public partial class QuestItemRequirement
{
    /// <summary>
    /// Gets or sets the required item.
    /// </summary>
    [Required]
    public virtual ItemDefinition? Item { get; set; }

    /// <summary>
    /// Gets or sets the drop item group which should be considered when this quest is active and this requirement applies.
    /// </summary>
    public virtual DropItemGroup? DropItemGroup { get; set; }

    /// <summary>
    /// Gets or sets the minimum number of <see cref="Item"/>s.
    /// </summary>
    public int MinimumNumber { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.MinimumNumber} x {this.Item?.Name}";
    }
}