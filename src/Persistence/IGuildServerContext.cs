// <copyright file="IGuildServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;
    using System.Collections.Generic;
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
        bool GuildWithNameExists(string name);

        /// <summary>
        /// Gets the member names of a guild.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <returns>The member names of a guild.</returns>
        /// <remarks>Since names are stored in Character.Name and not duplicated.</remarks>
        IReadOnlyDictionary<Guid, string> GetMemberNames(Guid guildId);
    }
}