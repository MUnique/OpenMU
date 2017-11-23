// -----------------------------------------------------------------------
// <copyright file="GuildContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GuildServer
{
    using System.Collections.Generic;

    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// This is a container Class which holds a Guild object, and a list of all guild members
    /// including their online-state.
    /// </summary>
    internal class GuildContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuildContainer" /> class.
        /// </summary>
        /// <param name="guild">The guild.</param>
        /// <param name="shortId">The short identifier.</param>
        /// <param name="databaseContext">The database context.</param>
        internal GuildContainer(Guild guild, ushort shortId, IContext databaseContext)
        {
            this.DatabaseContext = databaseContext;
            this.Guild = guild;
            this.ShortId = shortId;
            this.Members = new SortedList<string, GuildMemberInfo>();
            foreach (var member in this.Guild.Members)
            {
                this.Members.Add(member.Name, member);
            }
        }

        /// <summary>
        /// Gets the database context of the guild
        /// </summary>
        /// <remarks>Each guild holds its own database context.</remarks>
        public IContext DatabaseContext { get; }

        /// <summary>
        /// Gets the guild.
        /// </summary>
        public Guild Guild { get; }

        /// <summary>
        /// Gets the short identifier.
        /// </summary>
        public ushort ShortId { get; }

        /// <summary>
        /// Gets the members.
        /// </summary>
        public IDictionary<string, GuildMemberInfo> Members { get; }

        /// <summary>
        /// Sets the server identifier of the member.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="serverId">The server identifier.</param>
        public void SetServerId(string memberName, byte serverId)
        {
            if (this.Members.TryGetValue(memberName, out GuildMemberInfo guildMemberInfo))
            {
                guildMemberInfo.ServerId = serverId;
            }
        }
    }
}
