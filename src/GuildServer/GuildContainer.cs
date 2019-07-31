// -----------------------------------------------------------------------
// <copyright file="GuildContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GuildServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.Interfaces;
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
        /// <param name="id">The short identifier.</param>
        /// <param name="databaseContext">The database context.</param>
        internal GuildContainer(DataModel.Entities.Guild guild, uint id, IGuildServerContext databaseContext)
        {
            this.DatabaseContext = databaseContext;
            this.Guild = guild;
            this.Id = id;
            this.Members = new SortedList<Guid, GuildListEntry>();
            foreach (var member in this.Guild.Members)
            {
                // The player names are loaded separately, if required.
                this.Members.Add(member.Id, new GuildListEntry { PlayerName = null, ServerId = GuildServer.OfflineServerId, PlayerPosition = member.Status });
            }
        }

        /// <summary>
        /// Gets the database context of the guild.
        /// </summary>
        /// <remarks>Each guild holds its own database context.</remarks>
        public IGuildServerContext DatabaseContext { get; }

        /// <summary>
        /// Gets the guild.
        /// </summary>
        public DataModel.Entities.Guild Guild { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// Gets the members.
        /// </summary>
        public IDictionary<Guid, GuildListEntry> Members { get; }

        /// <summary>
        /// Sets the server identifier of the member.
        /// </summary>
        /// <param name="memberId">Id of the member.</param>
        /// <param name="serverId">The server identifier.</param>
        public void SetServerId(Guid memberId, byte serverId)
        {
            if (this.Members.TryGetValue(memberId, out GuildListEntry listEntry))
            {
                listEntry.ServerId = serverId;
            }
        }

        /// <summary>
        /// Loads the member names from the database.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public void LoadMemberNames(IPersistenceContextProvider persistenceContextProvider)
        {
            var memberNames = this.DatabaseContext.GetMemberNames(this.Guild.Id);
            if (memberNames != null)
            {
                foreach (var member in this.Members.Where(m => m.Value.PlayerName == null))
                {
                    if (memberNames.TryGetValue(member.Key, out string name))
                    {
                        member.Value.PlayerName = name;
                    }
                }
            }
        }
    }
}
