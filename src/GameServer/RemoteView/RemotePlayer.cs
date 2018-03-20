// <copyright file="RemotePlayer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using log4net;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameServer.MessageHandler;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// A player which is playing though a remote connection.
    /// </summary>
    public class RemotePlayer : Player
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RemotePlayer));

        /// <summary>
        /// Initializes a new instance of the <see cref="RemotePlayer"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        /// <param name="packetHandler">The packet handler.</param>
        /// <param name="connection">The remote connection.</param>
        public RemotePlayer(IGameServerContext gameContext, IPacketHandler packetHandler, IConnection connection)
            : base(gameContext, null)
        {
            this.PlayerView = new RemoteView(connection, this, gameContext, new AppearanceSerializer());
            this.Connection = connection;
            this.MainPacketHandler = packetHandler;
            this.Connection.PacketReceived += (sender, packet) => this.PacketReceived(packet);
            this.Connection.Disconnected += (sender, packet) => this.Disconnect();
        }

        /// <summary>
        /// Gets the game server context.
        /// </summary>
        public IGameServerContext GameServerContext => this.GameContext as IGameServerContext;

        /// <summary>
        /// Gets the connection.
        /// </summary>
        internal IConnection Connection { get; private set; }

        /// <summary>
        /// Gets or sets the main packet handler.
        /// </summary>
        internal IPacketHandler MainPacketHandler { get; set; }

        /// <summary>
        /// Is getting called when a packet got received from the connection of the player.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public void PacketReceived(byte[] buffer)
        {
            using (this.PushServerLogContext())
            using (log4net.ThreadContext.Stacks["connection"].Push(this.Connection.ToString()))
            using (log4net.ThreadContext.Stacks["account"].Push(this.GetAccountName()))
            using (log4net.ThreadContext.Stacks["character"].Push(this.GetSelectedCharacterName()))
            {
                try
                {
                    if (Logger.IsDebugEnabled)
                    {
                       Logger.DebugFormat("[C->S] {0}", buffer.AsString());
                    }

                    if (this.MainPacketHandler != null)
                    {
                        this.MainPacketHandler.HandlePacket(this, buffer);
                    }

                    if (buffer[0] == 0xC1 && buffer[1] == 0xB8 && buffer[2] == 1) ////Experimental
                    {
                        this.ClientReadyAfterMapChange();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        /// <inheritdoc/>
        protected override void InternalDisconnect()
        {
            base.InternalDisconnect();
            if (this.Connection != null && this.Connection.Connected)
            {
                this.Connection.Disconnect();
                this.Connection.Dispose();
                this.Connection = null;
            }
        }

        private IDisposable PushServerLogContext()
        {
            if (log4net.ThreadContext.Stacks["gameserver"].Count > 0)
            {
                return null;
            }

            return log4net.ThreadContext.Stacks["gameserver"].Push(this.GameServerContext.Id.ToString());
        }

        private string GetAccountName()
        {
            var account = this.Account;
            if (account != null)
            {
                return account.LoginName;
            }

            return string.Empty;
        }

        private string GetSelectedCharacterName()
        {
            var character = this.SelectedCharacter;
            if (character != null)
            {
                return character.Name;
            }

            return string.Empty;
        }
    }
}
