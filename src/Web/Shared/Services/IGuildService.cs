// <copyright file="IGuildService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using MUnique.OpenMU.Web.Shared.Models;

/// <summary>
/// Service for guild information in the admin panel, beyond the paged listing covered by <see cref="IDataService{T}"/>.
/// </summary>
public interface IGuildService : IDataService<GuildListItem>
{
    /// <summary>
    /// Gets the guild with the specified persistent identifier.
    /// </summary>
    /// <param name="guildId">The persistent identifier of the guild.</param>
    /// <returns>The guild, or <c>null</c> if it was not found.</returns>
    Task<GuildListItem?> GetGuildAsync(Guid guildId);

    /// <summary>
    /// Gets the members of the guild with the specified persistent identifier.
    /// </summary>
    /// <param name="guildId">The persistent identifier of the guild.</param>
    /// <returns>The members, ordered with the guild master first.</returns>
    Task<IReadOnlyList<GuildMemberViewItem>> GetGuildMembersAsync(Guid guildId);

    /// <summary>
    /// Gets the alliance of the guild with the specified persistent identifier.
    /// </summary>
    /// <param name="guildId">The persistent identifier of any guild in the alliance.</param>
    /// <returns>The alliance details, or <c>null</c> if the guild was not found.</returns>
    Task<AllianceDetails?> GetAllianceAsync(Guid guildId);
}
