// <copyright file="DuelConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Configuration for the duel feature.
/// </summary>
[Cloneable]
public partial class DuelConfiguration
{
    /// <summary>
    /// Gets or sets the maximum score at which the duel will end with a winner.
    /// </summary>
    public int MaximumScore {get; set; }

    /// <summary>
    /// Gets or sets the entrance fee for a duel.
    /// </summary>
    public int EntranceFee { get; set; }

    /// <summary>
    /// Gets or sets the minimum character level to start a duel.
    /// </summary>
    public int MinimumCharacterLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum spectators per duel room.
    /// </summary>
    public int MaximumSpectatorsPerDuelRoom { get; set; }

    /// <summary>
    /// Gets or sets the exit gate to which all players are ported after the duel has ended.
    /// If not set, players will be ported to the safezone.
    /// </summary>
    public virtual ExitGate? Exit { get; set; }

    /// <summary>
    /// Gets or sets the available duel areas.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<DuelArea> DuelAreas { get; protected set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Duel Configuration {this.DuelAreas.Count} Arenas, Exit: {this.Exit}";
    }
}