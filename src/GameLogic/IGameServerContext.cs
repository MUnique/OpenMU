// <copyright file="IGameServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// The context of a game server.
    /// </summary>
    public interface IGameServerContext : IGameContext
    {
        /// <summary>
        /// Occurs when a guild has been deleted.
        /// </summary>
        event EventHandler<GuildDeletedEventArgs> GuildDeleted;

        /// <summary>
        /// Gets the identifier of the server.
        /// </summary>
        byte Id { get; }

        /// <summary>
        /// Gets the guild server.
        /// </summary>
        IGuildServer GuildServer { get; }

        /// <summary>
        /// Gets the login server.
        /// </summary>
        ILoginServer LoginServer { get; }

        /// <summary>
        /// Gets the friend server.
        /// </summary>
        IFriendServer FriendServer { get; }

        /// <summary>
        /// Gets the server configuration.
        /// </summary>
        GameServerConfiguration ServerConfiguration { get; }
    }
}
