// <copyright file="AllianceDetails.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Models;

/// <summary>
/// Data transfer object for the admin panel alliance page.
/// </summary>
public class AllianceDetails
{
    /// <summary>
    /// Gets or sets the alliance master guild.
    /// </summary>
    public GuildListItem Master { get; set; } = new();

    /// <summary>
    /// Gets or sets the guilds which are part of the alliance, including the master guild.
    /// </summary>
    public IReadOnlyList<GuildListItem> Guilds { get; set; } = [];
}
