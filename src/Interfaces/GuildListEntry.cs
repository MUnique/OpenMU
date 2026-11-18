// <copyright file="GuildListEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// The guild list entry.
/// </summary>
public class GuildListEntry
{
    /// <summary>
    /// Gets or sets the name of the player.
    /// </summary>
    public string? PlayerName { get; set; }

    /// <summary>
    /// Gets or sets the server identifier on which the player is playing.
    /// </summary>
    public byte ServerId { get; set; }

    /// <summary>
    /// Gets or sets the players position in the guild.
    /// </summary>
    public GuildPosition PlayerPosition { get; set; }
}
