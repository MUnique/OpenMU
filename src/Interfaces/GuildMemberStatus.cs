// <copyright file="GuildMemberStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    /// <summary>
    /// The guild member status of a guild member.
    /// </summary>
    public class GuildMemberStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuildMemberStatus"/> class.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <param name="position">The position.</param>
        public GuildMemberStatus(uint guildId, GuildPosition position)
        {
            this.GuildId = guildId;
            this.Position = position;
        }

        /// <summary>
        /// Gets the guild identifier.
        /// </summary>
        /// <value>
        /// The guild identifier.
        /// </value>
        public uint GuildId { get; }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public GuildPosition Position { get; }
    }
}