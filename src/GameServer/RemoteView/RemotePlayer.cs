// <copyright file="RemotePlayer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using System.Buffers;
    using log4net;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameServer.MessageHandler;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A player which is playing through a remote connection.
    /// </summary>
    public class RemotePlayer : Player
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RemotePlayer));

        private readonly byte[] packetBuffer = new byte[0xFF];

        /// <summary>
        /// Initializes a new instance of the <see cref="RemotePlayer"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        /// <param name="packetHandler">The packet handler.</param>
        /// <param name="connection">The remote connection.</param>
        public RemotePlayer(IGameServerContext gameContext, IPacketHandler packetHandler, IConnection connection)
            : base(gameContext)
        {
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
        /// Gets the currently effective appearance serializer.
        /// </summary>
        internal IAppearanceSerializer AppearanceSerializer => this.ViewPlugIns.GetPlugIn<IAppearanceSerializer>();

        /// <summary>
        /// Gets the currently effective item serializer.
        /// </summary>
        internal IItemSerializer ItemSerializer => this.ViewPlugIns.GetPlugIn<IItemSerializer>();

        /// <summary>
        /// Gets or sets the main packet handler.
        /// </summary>
        internal IPacketHandler MainPacketHandler { get; set; }

        /// <inheritdoc />
        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
        {
            return new ViewPlugInContainer(this, new ClientVersion(6, 4, ClientLanguage.English), this.GameContext.PlugInManager);
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

        /// <summary>
        /// Is getting called when a packet got received from the connection of the player.
        /// </summary>
        /// <param name="sequence">The packet.</param>
        private void PacketReceived(ReadOnlySequence<byte> sequence)
        {
            using (this.PushServerLogContext())
            using (log4net.ThreadContext.Stacks["connection"].Push(this.Connection.ToString()))
            using (log4net.ThreadContext.Stacks["account"].Push(this.GetAccountName()))
            using (log4net.ThreadContext.Stacks["character"].Push(this.GetSelectedCharacterName()))
            {
                try
                {
                    Span<byte> buffer;
                    IMemoryOwner<byte> owner = null;
                    if (sequence.Length <= this.packetBuffer.Length)
                    {
                        sequence.CopyTo(this.packetBuffer);
                        buffer = this.packetBuffer.AsSpan(0, this.packetBuffer.GetPacketSize());
                    }
                    else
                    {
                        owner = MemoryPool<byte>.Shared.Rent((int)sequence.Length);
                        buffer = owner.Memory.Slice(0, (int)sequence.Length).Span;
                    }

                    try
                    {
                        if (Logger.IsDebugEnabled)
                        {
                            Logger.DebugFormat("[C->S] {0}", buffer.ToArray().AsString());
                        }

                        this.MainPacketHandler?.HandlePacket(this, buffer);
                    }
                    finally
                    {
                        owner?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
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
