// <copyright file="GuildCache.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Concurrent;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// A cache for guild data.
    /// </summary>
    public class GuildCache
    {
        private readonly IGameServerContext gameServer;

        private readonly ConcurrentDictionary<ushort, Guid> shortIdToGuid;

        private readonly ConcurrentDictionary<Guid, GuildInfo> cache;

        private readonly IGuildInfoSerializer guildInfoSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildCache"/> class.
        /// </summary>
        /// <param name="gameServer">The game server.</param>
        /// <param name="serializer">The serializer.</param>
        public GuildCache(IGameServerContext gameServer, IGuildInfoSerializer serializer)
        {
            this.gameServer = gameServer;
            this.guildInfoSerializer = serializer;
            this.cache = new ConcurrentDictionary<Guid, GuildInfo>();
            this.shortIdToGuid = new ConcurrentDictionary<ushort, Guid>();
        }

        /// <summary>
        /// Declares an interface for a serializer of guilds.
        /// </summary>
        public interface IGuildInfoSerializer
        {
            /// <summary>
            /// Serializes the specified guild.
            /// </summary>
            /// <param name="guild">The guild.</param>
            /// <param name="shortId">The short identifier.</param>
            /// <returns>
            /// The serialized guild data.
            /// </returns>
            byte[] Serialize(Guild guild, ushort shortId);
        }

        /// <summary>
        /// Invalidates the cached data of the guild with the specified id.
        /// </summary>
        /// <param name="guildId">The id of the guild which data should get invalidated.</param>
        public void Invalidate(Guid guildId)
        {
            this.cache.TryRemove(guildId, out var _);
        }

        /// <summary>
        /// Registers the short identifier.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <param name="shortId">The short identifier.</param>
        public void RegisterShortId(Guid guildId, ushort shortId)
        {
            this.shortIdToGuid.AddOrUpdate(shortId, i => guildId, (i, guid) => guid);
        }

        /// <summary>
        /// Returns the Guild Info Data of a Guild. It will either
        /// take the data out of the cache, or get it from the database.
        /// </summary>
        /// <param name="shortGuildId">The id of the guild.</param>
        /// <returns>The data of the guild.</returns>
        internal byte[] GetGuildData(ushort shortGuildId)
        {
            var guildInfo = this.GetGuildInfo(shortGuildId);
            return guildInfo?.Data;
        }

        private GuildInfo GetGuildInfo(ushort shortId)
        {
            if (this.shortIdToGuid.TryGetValue(shortId, out var guildId))
            {
                return this.GetGuildInfo(guildId, shortId);
            }

            return null;
        }

        private GuildInfo GetGuildInfo(Guid guildId, ushort shortId)
        {
            if (this.cache.TryGetValue(guildId, out var guildInfo))
            {
                return guildInfo;
            }

            var guild = this.gameServer.GuildServer.GetGuild(guildId);
            var data = this.guildInfoSerializer.Serialize(guild, shortId);
            guildInfo = new GuildInfo { Data = data, AllyGuildId = guild.AllianceGuild?.Id };
            this.cache.TryAdd(guildId, guildInfo);
            return guildInfo;
        }

        private class GuildInfo
        {
            internal byte[] Data { get; set; }

            internal Guid? AllyGuildId { get; set; }
        }
    }
}
