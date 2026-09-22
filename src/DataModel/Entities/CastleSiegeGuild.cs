// <copyright file="CastleSiegeGuild.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Stores a guild which participates in the current Castle Siege cycle.
/// </summary>
public class CastleSiegeGuild
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the persistent guild identifier.
    /// </summary>
    public Guid GuildId { get; set; }

    /// <summary>
    /// Gets or sets the denormalized guild name.
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
    /// Gets or sets a value indicating whether this guild is the alliance master.
    /// </summary>
    public bool IsAllianceMaster { get; set; }
}
