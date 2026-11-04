// <copyright file="GensAbuse.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// The count of the recent kills of a gens member by another gens member,
/// to limit the contribution points of repeated kills of the same character.
/// </summary>
/// <remarks>
/// It's kept in its own table, so that the count doesn't start again when one of the characters
/// logs out and in again.
/// </remarks>
[AggregateRoot]
public class GensAbuse
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the killing character.
    /// </summary>
    public Guid KillerId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the killed character.
    /// </summary>
    public Guid VictimId { get; set; }

    /// <summary>
    /// Gets or sets the count of the recent kills.
    /// </summary>
    public int KillCount { get; set; }

    /// <summary>
    /// Gets or sets the time of the last kill.
    /// </summary>
    public DateTime LastKillAt { get; set; }
}
