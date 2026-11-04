// <copyright file="AppearanceData.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The appearance data of a character.
/// </summary>
public class AppearanceData : IAppearanceData
{
    /// <summary>
    /// Occurs when the appearance of the player changed.
    /// </summary>
    /// <remarks>
    /// This never happens in this implementation.
    /// </remarks>
    public event EventHandler? AppearanceChanged;

    /// <summary>
    /// Gets or sets the character class.
    /// </summary>
    public virtual CharacterClass? CharacterClass { get; set; }

    /// <summary>
    /// Gets or sets the character status.
    /// </summary>
    public CharacterStatus CharacterStatus { get; set; }

    /// <inheritdoc />
    public CharacterPose Pose { get; set; }

    /// <inheritdoc />
    public bool FullAncientSetEquipped { get; set; }

    /// <summary>
    /// Gets or sets the equipped items.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemAppearance> EquippedItems { get; protected set; } = null!;

    /// <inheritdoc />
    IEnumerable<ItemAppearance> IAppearanceData.EquippedItems => this.EquippedItems;
}