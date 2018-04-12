// <copyright file="GuildCache.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Concurrent;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// A cache for guild data.
    /// </summary>
    public class GuildCache
    {
        private readonly IGameServerContext gameServer;

        private readonly ConcurrentDictionary<uint, byte[]> cache;

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
            this.cache = new ConcurrentDictionary<uint, byte[]>();
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
            /// <param name="guildId">The guild identifier.</param>
            /// <returns>
            /// The serialized guild data.
            /// </returns>
            byte[] Serialize(Guild guild, uint guildId);
        }

        /// <summary>
        /// Invalidates the cached data of the guild with the specified id.
        /// </summary>
        /// <param name="guildId">The id of the guild which data should get invalidated.</param>
        public void Invalidate(uint guildId)
        {
            this.cache.TryRemove(guildId, out var _);
        }

        /// <summary>
        /// Returns the Guild Info Data of a Guild. It will either
        /// take the data out of the cache, or get it from the database.
        /// </summary>
        /// <param name="guildId">The id of the guild.</param>
        /// <returns>The data of the guild.</returns>
        internal byte[] GetGuildData(uint guildId)
        {
            var guildInfo = this.GetGuildInfo(guildId);
            return guildInfo;
        }

        private byte[] GetGuildInfo(uint guildId)
        {
            if (this.cache.TryGetValue(guildId, out var guildInfo))
            {
                return guildInfo;
            }

            var guild = this.gameServer.GuildServer.GetGuild(guildId);
            var data = this.guildInfoSerializer.Serialize(guild, guildId);
            this.cache.TryAdd(guildId, data);
            return data;
        }
    }
}
