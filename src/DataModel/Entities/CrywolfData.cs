// <copyright file="CrywolfData.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Persistent state of the crywolf event, stored as a single row, like the original game does it.
/// It keeps the result of the last event until the next one.
/// </summary>
[AggregateRoot]
public class CrywolfData
{
    /// <summary>
    /// Gets or sets the unique identifier of this record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the crywolf fortress is occupied by Balgass,
    /// because the last event failed. Otherwise, it's in peace.
    /// </summary>
    public bool IsOccupied { get; set; }

    /// <summary>
    /// Gets or sets the time (UTC) when the last battle ended, or <see langword="null"/> if there was no battle yet.
    /// </summary>
    public DateTime? LastBattleEnd { get; set; }
}
