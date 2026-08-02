// <copyright file="CastleSiegeData.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Persistent state of the castle siege, stored as a single row across siege cycles.
/// </summary>
[AggregateRoot]
public class CastleSiegeData
{
    /// <summary>
    /// Gets or sets the unique identifier of this record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the persistent identifier of the guild that currently owns the castle.
    /// <see langword="null"/> when no guild owns the castle.
    /// </summary>
    public Guid? OwnerGuildId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether any guild currently occupies the castle.
    /// </summary>
    public bool IsOccupied { get; set; }

    /// <summary>
    /// Gets or sets the Chaos Machine tax rate applied by the castle owner (0–3).
    /// </summary>
    public byte TaxChaos { get; set; }

    /// <summary>
    /// Gets or sets the personal store tax rate applied by the castle owner (0–3).
    /// </summary>
    public byte TaxStore { get; set; }

    /// <summary>
    /// Gets or sets the entry fee (in Zen) for the castle owner's hunt zone (0–300000).
    /// </summary>
    public int TaxHunt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the hunt zone (Land of Trials) is currently open to the public.
    /// </summary>
    public bool IsHuntZoneEnabled { get; set; }

    /// <summary>
    /// Gets or sets the accumulated tribute money collected from the hunt zone and taxes.
    /// </summary>
    public long TributeMoney { get; set; }

    /// <summary>
    /// Gets or sets the persisted states of all castle NPCs.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CastleSiegeNpcState> NpcStates { get; protected set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        return this.IsOccupied
            ? $"Castle owned by guild {this.OwnerGuildId}"
            : "Castle unoccupied";
    }
}
