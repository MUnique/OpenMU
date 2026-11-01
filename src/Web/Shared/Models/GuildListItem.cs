// <copyright file="GuildListItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Models;

/// <summary>
/// Data transfer object for a guild in the admin panel guild list and detail pages.
/// </summary>
public class GuildListItem
{
    /// <summary>
    /// Gets or sets the persistent identifier of the guild.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the guild.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the score of the guild.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the notice of the guild.
    /// </summary>
    public string? Notice { get; set; }

    /// <summary>
    /// Gets or sets the number of members of the guild.
    /// </summary>
    public int MemberCount { get; set; }

    /// <summary>
    /// Gets or sets the persistent identifier of the alliance master guild, if this guild is part of an alliance.
    /// This is set to <see cref="Id"/> itself when this guild is the alliance master, so that both member
    /// guilds and the master guild can link to the same alliance page.
    /// </summary>
    public Guid? AllianceGuildId { get; set; }

    /// <summary>
    /// Gets or sets the name of the alliance master guild, if this guild is part of an alliance.
    /// This is set to <see cref="Name"/> itself when this guild is the alliance master.
    /// </summary>
    public string? AllianceName { get; set; }

    /// <summary>
    /// Gets or sets the raw 8x8 16-color logo bitmap data of the guild.
    /// </summary>
    public byte[]? Logo { get; set; }
}
