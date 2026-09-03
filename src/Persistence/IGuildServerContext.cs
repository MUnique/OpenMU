// <copyright file="IGuildServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// A context which is used by the <see cref="IGuildServer"/>.
/// </summary>
public interface IGuildServerContext : IContext
{
    /// <summary>
    /// Returns if the guild with the specified name exists.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>True, if the guild with the specified name exists.</returns>
    ValueTask<bool> GuildWithNameExistsAsync(string name);

    /// <summary>
    /// Gets the persistent identifier of the guild with the specified name.
    /// </summary>
    /// <param name="name">The guild name.</param>
    /// <returns>The guild identifier, or <see langword="null"/> when no guild has the name.</returns>
    ValueTask<Guid?> GetPersistentGuildIdByNameAsync(string name);

    /// <summary>
    /// Gets the member names of a guild.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <returns>The member names of a guild.</returns>
    /// <remarks>Since names are stored in Character.Name and not duplicated.</remarks>
    ValueTask<IReadOnlyDictionary<Guid, string>> GetMemberNamesAsync(Guid guildId);

    /// <summary>
    /// Gets the alliances of a guild.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <returns>The ids of the alliances of a guild.</returns>
    ValueTask<IReadOnlyList<DataModel.Entities.Guild>> GetAlliancesAsync(Guid guildId);
}
