// <copyright file="IGuildMemberEnricher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using MUnique.OpenMU.Web.Shared.Models;

/// <summary>
/// Enriches guild members with character and account data, which lives in a separate
/// persistence context (the player context) from the guild data itself.
/// </summary>
public interface IGuildMemberEnricher
{
    /// <summary>
    /// Enriches the given guild members with character and account data, where available.
    /// Enrichment is best-effort: members whose character data cannot be resolved are left
    /// with just their name and position, so callers can always show the base guild member list.
    /// </summary>
    /// <param name="members">The guild members to enrich, in place.</param>
    Task EnrichAsync(IReadOnlyList<GuildMemberViewItem> members);
}
