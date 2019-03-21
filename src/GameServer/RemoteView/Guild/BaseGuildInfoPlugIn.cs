﻿// <copyright file="BaseGuildInfoPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Abstract base class for a <see cref="IShowGuildInfoPlugIn" /> which allows to cache serialized guild infos.
    /// </summary>
    /// <typeparam name="T">The type of the actual <see cref="IShowGuildInfoPlugIn"/>. Required, so there is one cache per type.</typeparam>
    // ReSharper disable once UnusedTypeParameter we just use it to get type specific static fields.
    public abstract class BaseGuildInfoPlugIn<T>
        where T : IShowGuildInfoPlugIn
    {
        /// <summary>
        /// The cache for already serialized guilds. This data doesn't change, but is requested often.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType That's what we want
        private static readonly ConcurrentDictionary<uint, byte[]> Cache = new ConcurrentDictionary<uint, byte[]>();

        // ReSharper disable once StaticMemberInGenericType That's what we want
        private static readonly HashSet<IGameServerContext> AppendedGuildDeletedSenders = new HashSet<IGameServerContext>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGuildInfoPlugIn{T}"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        protected BaseGuildInfoPlugIn(RemotePlayer player)
        {
            this.Player = player;
            lock (AppendedGuildDeletedSenders)
            {
                if (AppendedGuildDeletedSenders.Add(player.GameServerContext))
                {
                    // to make sure we just add one event handler
                    this.Player.GameServerContext.GuildDeleted += OnGuildDeleted;
                }
            }
        }

        /// <summary>
        /// Gets the player.
        /// </summary>
        /// <value>
        /// The player.
        /// </value>
        protected RemotePlayer Player { get; }

        /// <summary>
        /// Serializes the specified guild.
        /// </summary>
        /// <param name="guild">The guild.</param>
        /// <param name="guildId">The guild identifier.</param>
        /// <returns>The serialized guild data packet.</returns>
        protected abstract byte[] Serialize(Guild guild, uint guildId);

        /// <summary>
        /// Returns the Guild Info Data of a Guild. It will either
        /// take the data out of the Cache, or get it from the database and serializes it.
        /// </summary>
        /// <param name="guildId">The id of the guild.</param>
        /// <returns>
        /// The data of the guild.
        /// </returns>
        protected byte[] GetGuildData(uint guildId)
        {
            if (Cache.TryGetValue(guildId, out var guildInfo))
            {
                return guildInfo;
            }

            var guild = this.Player.GameServerContext.GuildServer.GetGuild(guildId);
            var data = this.Serialize(guild, guildId);
            Cache.TryAdd(guildId, data);
            return data;
        }

        private static void OnGuildDeleted(object sender, GuildDeletedEventArgs args)
        {
            Cache.TryRemove(args.GuildId, out _);
        }
    }
}