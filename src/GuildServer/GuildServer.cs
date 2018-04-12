// <copyright file="GuildServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GuildServer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// GuildServer which is managing the guilds. All Guilds of online guild members will be cached here.
    /// The server needs to be aware when characters leave or enter the world, so a connection to a
    /// GameServer is required.
    /// </summary>
    public class GuildServer : IGuildServer
    {
        /// <summary>
        /// The offline server identifier.
        /// </summary>
        public const byte OfflineServerId = 0xFF;

        private const ushort NonGuild = 0;

        private static readonly ILog Log = LogManager.GetLogger(typeof(GuildServer));

        private readonly IDictionary<int, IGameServer> gameServers;
        private readonly IDictionary<Guid, GuildContainer> guildDictionary;
        private readonly ConcurrentQueue<ushort> freeIds;
        private readonly IRepositoryManager repositoryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildServer"/> class.
        /// </summary>
        /// <param name="gameServers">The game servers.</param>
        /// <param name="repositoryManager">The repository manager.</param>
        public GuildServer(IDictionary<int, IGameServer> gameServers, IRepositoryManager repositoryManager)
        {
            this.gameServers = gameServers;
            this.repositoryManager = repositoryManager;
            this.guildDictionary = new ConcurrentDictionary<Guid, GuildContainer>();
            this.freeIds = new ConcurrentQueue<ushort>(Enumerable.Range(1, ushort.MaxValue - 1).Select(i => (ushort)i));
        }

        /// <inheritdoc/>
        public void GuildMessage(Guid guildId, string sender, string message)
        {
            // TODO: threadsafe?
            foreach (var gameServer in this.gameServers.Values)
            {
                gameServer.GuildChatMessage(guildId, sender, message);
            }
        }

        /// <inheritdoc/>
        public void AllianceMessage(Guid guildId, string sender, string message)
        {
            // TODO: threadsafe?
            foreach (var gameServer in this.gameServers.Values)
            {
                gameServer.AllianceChatMessage(guildId, sender, message);
            }
        }

        /// <inheritdoc/>
        public bool GuildExists(string guildName)
        {
            var repository = this.repositoryManager.GetRepository<Guild, IGuildRepository>();

            return repository.GuildWithNameExists(guildName);
        }

        /// <inheritdoc/>
        public Guild GetGuild(Guid guildId)
        {
            if (this.guildDictionary.TryGetValue(guildId, out GuildContainer guild))
            {
                return guild.Guild;
            }

            return null;
        }

        /// <inheritdoc/>
        public Guild CreateGuild(string name, string masterName, Guid masterId, byte[] logo, out ushort shortGuildId, out GuildMemberInfo masterGuildMemberInfo)
        {
            shortGuildId = NonGuild;
            var context = this.repositoryManager.CreateNewContext();
            using (this.repositoryManager.UseContext(context))
            {
                var guild = this.repositoryManager.CreateNew<Guild>();
                guild.Name = name;
                guild.Master = masterName;
                guild.MasterId = masterId;
                guild.Logo = logo;

                masterGuildMemberInfo = this.repositoryManager.CreateNew<GuildMemberInfo>();
                masterGuildMemberInfo.GuildId = guild.Id;
                masterGuildMemberInfo.CharacterId = masterId;
                masterGuildMemberInfo.Name = masterName;
                masterGuildMemberInfo.Status = GuildPosition.GuildMaster;
                guild.Members.Add(masterGuildMemberInfo);

                if (context.SaveChanges())
                {
                    var container = this.CreateGuildContainer(guild, context);
                    shortGuildId = container.ShortId;
                    return guild;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public bool UpdateGuild(Guild guild)
        {
            if (this.guildDictionary.TryGetValue(guild.Id, out GuildContainer guildContainer))
            {
                using (this.repositoryManager.UseContext(guildContainer.DatabaseContext))
                {
                    if (guild != guildContainer.Guild)
                    {
                        guildContainer.Guild.Hostility = guild.Hostility;
                        guildContainer.Guild.AllianceGuild = guild.AllianceGuild;
                        guildContainer.Guild.Notice = guild.Notice;

                        // i think notice can be removed. it should move to the guildcontainer.
                        guildContainer.Guild.Score = guild.Score;
                    }

                    guildContainer.DatabaseContext.SaveChanges();
                }
            }

            return false;
        }

        /// <summary>
        /// Removes a guild from the server and the database.
        ///
        /// First we are trying to get the guild out of our dictionary.
        /// We are assuming that all guilds are in the dictionary, because
        /// we are holding all guilds of all ingame-characters in it.
        /// So this method is only called usefully from the gameserver itself,
        /// by player interaction.
        /// </summary>
        /// <param name="guildId">The id of the guild which should be deleted</param>
        /// <returns>The success.</returns>
        public bool DeleteGuild(Guid guildId)
        {
            if (!this.guildDictionary.TryGetValue(guildId, out GuildContainer guildContainer))
            {
                return false;
            }

            Guild guild = guildContainer.Guild;
            using (this.repositoryManager.UseContext(guildContainer.DatabaseContext))
            {
                var repository = this.repositoryManager.GetRepository<Guild>();
                repository.Delete(guild);
            }

            guildContainer.DatabaseContext.Dispose();

            this.guildDictionary.Remove(guildContainer.Guild.Id);
            this.freeIds.Enqueue(guildContainer.ShortId);

            foreach (var gameServer in this.gameServers.Values)
            {
                gameServer.GuildDeleted(guildContainer.Guild.Id);
            }

            // TODO: Inform gameServers that guildwar/hostility ended
            return true;
        }

        /// <inheritdoc/>
        public GuildMemberInfo CreateGuildMember(Guid guildId, Guid characterId, string characterName, GuildPosition role)
        {
            try
            {
                if (this.guildDictionary.TryGetValue(guildId, out GuildContainer guild))
                {
                    if (guild.Members.ContainsKey(characterName))
                    {
                        Log.WarnFormat("Guildmember already exists: {0}", characterName);
                        return null;
                    }

                    using (this.repositoryManager.UseContext(guild.DatabaseContext))
                    {
                        var guildMember = this.repositoryManager.CreateNew<GuildMemberInfo>();
                        guildMember.CharacterId = characterId;
                        guildMember.GuildId = guildId;
                        guildMember.Name = characterName;
                        guildMember.Status = role;

                        guild.Members.Add(guildMember.Name, guildMember);
                        guild.Guild.Members.Add(guildMember);

                        guild.DatabaseContext.SaveChanges();
                        return guildMember;
                    }
                }
            }
            catch (Exception ex)
            {
                // Rollback?
                Log.Error(ex);
            }

            return null;
        }

        /// <inheritdoc/>
        public bool UpdateGuildMember(GuildMemberInfo guildMember)
        {
            try
            {
                if (this.guildDictionary.TryGetValue(guildMember.GuildId, out GuildContainer guild))
                {
                    using (this.repositoryManager.UseContext(guild.DatabaseContext))
                    {
                        var guildMemberInfo = guild.Members[guildMember.Name];
                        guildMemberInfo.Status = guildMember.Status; // its just the status which can be changed
                        return guild.DatabaseContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool DeleteGuildMember(GuildMemberInfo guildMember)
        {
            try
            {
                var repository = this.repositoryManager.GetRepository<GuildMemberInfo>();
                if (repository.Delete(guildMember))
                {
                    if (this.guildDictionary.TryGetValue(guildMember.GuildId, out GuildContainer guild))
                    {
                        guild.Members.Remove(guildMember.Name);
                        guild.Guild.Members.Remove(guildMember);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return false;
        }

        /// <inheritdoc/>
        public ushort GuildMemberEnterGame(Guid guildId, string guildMemberName, byte serverId)
        {
            GuildContainer guild = this.GetOrCreateGuildContainer(guildId);
            if (guild != null)
            {
                guild.SetServerId(guildMemberName, serverId);
                return guild.ShortId;
            }

            return NonGuild;
        }

        /// <inheritdoc/>
        public void GuildMemberLeaveGame(Guid guildId, string guildMemberName, byte serverId)
        {
            if (this.guildDictionary.TryGetValue(guildId, out GuildContainer guild))
            {
                guild.SetServerId(guildMemberName, OfflineServerId);
                if (guild.Members.Values.All(member => member.ServerId == OfflineServerId))
                {
                    this.guildDictionary.Remove(guild.Guild.Id);
                    this.freeIds.Enqueue(guild.ShortId);
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<GuildListEntry> GetGuildList(Guid guildId)
        {
            if (!this.guildDictionary.TryGetValue(guildId, out GuildContainer guildContainer))
            {
                yield break;
            }

            foreach (var guildmember in guildContainer.Members.Values)
            {
                var entry = new GuildListEntry();
                entry.PlayerName = guildmember.Name;
                entry.PlayerPosition = guildmember.Status;
                entry.ServerId = guildmember.ServerId;
                yield return entry;
            }
        }

        /// <inheritdoc/>
        public void KickPlayer(Guid guildId, string playerName)
        {
            if (!this.guildDictionary.TryGetValue(guildId, out GuildContainer guildContainer))
            {
                return;
            }

            if (!guildContainer.Members.TryGetValue(playerName, out GuildMemberInfo guildMemberInfo))
            {
                return;
            }

            guildContainer.Members.Remove(playerName);
            if (this.gameServers.TryGetValue(guildMemberInfo.ServerId, out IGameServer gameServer))
            {
                gameServer.GuildPlayerKicked(playerName);
            }
        }

        private Guild LoadGuildInternal(Guid guildId)
        {
            var repository = this.repositoryManager.GetRepository<Guild>();
            return repository.GetById(guildId);
        }

        private GuildContainer GetOrCreateGuildContainer(Guid guildId)
        {
            if (!this.guildDictionary.TryGetValue(guildId, out GuildContainer guild))
            {
                var context = this.repositoryManager.CreateNewContext();
                using (this.repositoryManager.UseContext(context))
                {
                    var guildinfo = this.LoadGuildInternal(guildId);
                    if (guildinfo == null)
                    {
                        Log.WarnFormat("GuildMemberEnter: Guild {0} not found", guildId);
                        context.Dispose();
                        return null;
                    }

                    guild = this.CreateGuildContainer(guildinfo, context);
                }
            }

            return guild;
        }

        private GuildContainer CreateGuildContainer(Guild guild, IContext databaseContext)
        {
            if (!this.freeIds.TryDequeue(out ushort shortId))
            {
                throw new Exception("no free short id");
            }

            var guildContainer = new GuildContainer(guild, shortId, databaseContext);
            this.guildDictionary.Add(guild.Id, guildContainer);

            return guildContainer;
        }
    }
}