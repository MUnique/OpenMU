// <copyright file="GensMember.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// The gens membership of a character.
/// </summary>
/// <remarks>
/// It's kept in its own table instead of the character, so that the ranking
/// can be calculated for all members at once, without loading their accounts.
/// After a character left its gens, the entry is kept with <see cref="GensType.None"/>,
/// so that the time of leaving is known.
/// </remarks>
[AggregateRoot]
public class GensMember
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the character.
    /// </summary>
    public Guid CharacterId { get; set; }

    /// <summary>
    /// Gets or sets the gens of the character.
    /// </summary>
    public GensType Gens { get; set; }

    /// <summary>
    /// Gets or sets the contribution points of the character.
    /// </summary>
    public int Contribution { get; set; }

    /// <summary>
    /// Gets or sets the rank of the character, from 1 (highest) to 14 (lowest).
    /// </summary>
    public byte Rank { get; set; }

    /// <summary>
    /// Gets or sets the position of the character in the ranking of its gens.
    /// 0, when it wasn't ranked yet.
    /// </summary>
    public int RankingPosition { get; set; }

    /// <summary>
    /// Gets or sets the time when the character joined its gens.
    /// </summary>
    public DateTime? JoinedAt { get; set; }

    /// <summary>
    /// Gets or sets the time when the character left its last gens.
    /// </summary>
    public DateTime? LeftAt { get; set; }
}
