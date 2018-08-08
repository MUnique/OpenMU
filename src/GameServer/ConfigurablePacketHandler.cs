// <copyright file="ConfigurablePacketHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using log4net;
    using Microsoft.Practices.Unity;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameServer.MessageHandler;
    using MUnique.OpenMU.Network;
    using Unity;

    /// <summary>
    /// A configurable packet handler, which creates its sub packet handlers based on a configuration.
    /// </summary>
    public class ConfigurablePacketHandler : IPacketHandler
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ConfigurablePacketHandler));
        private readonly IPacketHandler[] packetHandler = new IPacketHandler[0x100];
        private readonly bool[] encryptionChecks = new bool[0x100];

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurablePacketHandler"/> class.
        /// </summary>
        /// <param name="handlerConfiguration">The handler configuration.</param>
        /// <param name="container">The container.</param>
        public ConfigurablePacketHandler(PacketHandlerConfiguration handlerConfiguration, IUnityContainer container)
        {
            foreach (var config in handlerConfiguration.SubPacketHandlers)
            {
                this.CreateHandler(container, config);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurablePacketHandler"/> class.
        /// </summary>
        protected ConfigurablePacketHandler()
        {
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            var index = packet[0] % 2 == 1 ? 2 : 3;
            var packetType = packet[index];
            if (this.encryptionChecks[packetType] && (packet[0] < 0xC3))
            {
                this.log.Warn($"Packet was not encrypted and will not be handled: {packet.AsString()}");
                return;
            }

            var handler = this.GetPacketHandler(packetType);
            handler?.HandlePacket(player, packet);
        }

        /// <summary>
        /// Gets the packet handler of a specific packet type.
        /// </summary>
        /// <remarks>
        /// Internal for tests.
        /// </remarks>
        /// <param name="packetType">Type of the packet.</param>
        /// <returns>The registered packet handler of a specific packet type.</returns>
        internal IPacketHandler GetPacketHandler(byte packetType)
        {
            return this.packetHandler[packetType];
        }

        /// <summary>
        /// Creates the a new handler, based on the configuration.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="handlerConfiguration">The handler configuration.</param>
        protected void CreateHandler(IUnityContainer container, PacketHandlerConfiguration handlerConfiguration)
        {
            IPacketHandler handler;
            this.encryptionChecks[handlerConfiguration.PacketIdentifier] = handlerConfiguration.NeedsToBeEncrypted;
            if (handlerConfiguration.SubPacketHandlers != null && handlerConfiguration.SubPacketHandlers.Count > 0)
            {
                handler = new ConfigurablePacketHandler(handlerConfiguration, container);
            }
            else
            {
                var handlerType = Type.GetType(handlerConfiguration.PacketHandlerClassName);
                var obj = container.Resolve(handlerType, string.Empty);
                handler = obj as IPacketHandler;
            }

            this.packetHandler[handlerConfiguration.PacketIdentifier] = handler;
        }
    }
}
