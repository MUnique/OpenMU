// <copyright file="ConfigurableMainPacketHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameServer.MessageHandler;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Interfaces;
    using Unity;
    using Unity.Lifetime;

    /// <summary>
    /// A main packet handler which can be configured.
    /// </summary>
    public class ConfigurableMainPacketHandler : ConfigurablePacketHandler, IMainPacketHandler
    {
        private readonly MainPacketHandlerConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableMainPacketHandler" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="gameServer">The game server.</param>
        public ConfigurableMainPacketHandler(MainPacketHandlerConfiguration configuration, IGameServerContext gameServer)
        {
            this.configuration = configuration;
            using (var container = new UnityContainer())
            {
                container.RegisterInstance(typeof(IGameServerContext), string.Empty, gameServer, new ExternallyControlledLifetimeManager());
                container.RegisterInstance(typeof(IGameContext), string.Empty, gameServer, new ExternallyControlledLifetimeManager());
                container.RegisterInstance(typeof(IFriendServer), string.Empty, gameServer.FriendServer, new ExternallyControlledLifetimeManager());
                foreach (var handlerConfiguration in this.configuration.PacketHandlers)
                {
                    this.CreateHandler(container, handlerConfiguration);
                }
            }
        }

        /// <inheritdoc/>
        public byte[] ClientVersion
        {
            get
            {
                return this.configuration.ClientVersion;
            }
        }
    }
}
