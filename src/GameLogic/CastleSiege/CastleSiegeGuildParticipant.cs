// <copyright file="CastleSiegeGuildParticipant.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Holds a guild's runtime participation data for the current Castle Siege cycle.
/// </summary>
public class CastleSiegeGuildParticipant
{
    /// <summary>
    /// Gets or sets the runtime guild identifier.
    /// </summary>
    public uint GuildId { get; set; }

    /// <summary>
    /// Gets or sets the persistent guild identifier.
    /// </summary>
    public Guid PersistentGuildId { get; set; }

    /// <summary>
    /// Gets or sets the guild name.
    /// </summary>
    public string GuildName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the side assigned to the guild.
    /// </summary>
    public CastleSiegeJoinSide Side { get; set; }

    /// <summary>
    /// Gets or sets the selection score of the guild.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the guild is the master guild of its alliance.
    /// </summary>
    public bool IsAllianceMaster { get; set; }
}
