// <copyright file="PacketHandlerPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.PlugIns;
    using Unity;
    using Unity.Lifetime;

    /// <summary>
    /// A plugin container which provides the effective packet handler plugins for the specified version.
    /// Base class for different kind of packet handler interfaces.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler interface.</typeparam>
    public class PacketHandlerPlugInContainer<THandler> : StrategyPlugInProvider<byte, THandler>
        where THandler : class, IPacketHandlerPlugInBase
    {
        /// <summary>
        /// The logger of this class.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Since packet handler plugins are not holding any player state, they can be kept as singleton instances to save some memory.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, THandler> HandlerCache = new ConcurrentDictionary<Type, THandler>();

        private readonly IClientVersionProvider clientVersionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketHandlerPlugInContainer{THandler}"/> class.
        /// </summary>
        /// <param name="clientVersionProvider">The client version provider.</param>
        /// <param name="manager">The manager.</param>
        public PacketHandlerPlugInContainer(IClientVersionProvider clientVersionProvider, PlugInManager manager)
            : base(manager)
        {
            this.clientVersionProvider = clientVersionProvider;
            this.clientVersionProvider.ClientVersionChanged += this.OnClientVersionChanged;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            foreach (var plugInType in this.Manager.GetKnownPlugInsOf<THandler>().Where(this.Manager.IsPlugInActive))
            {
                this.BeforeActivatePlugInType(plugInType);
            }
        }

        /// <summary>
        /// Handles the incoming data packet by selecting the correct plugin.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="packet">The packet.</param>
        public void HandlePacket(RemotePlayer player, in Span<byte> packet)
        {
            var typeIndex = packet[0] % 2 == 1 ? 2 : 3;
            var packetType = packet[typeIndex];
            var handler = this[packetType];
            this.HandlePacket(player, packet, handler);
        }

        /// <summary>
        /// Handles the incoming data packet by using the specified handler plugin.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="packet">The packet.</param>
        /// <param name="handler">The handler.</param>
        protected void HandlePacket(Player player, in Span<byte> packet, THandler handler)
        {
            if (handler == null)
            {
                // unknown packet
                return;
            }

            if (handler.IsEncryptionExpected && (packet[0] < 0xC3))
            {
                Log.Warn($"Packet was not encrypted and will not be handled: {packet.AsString()}");
                return;
            }

            handler.HandlePacket(player, packet);
        }

        /// <inheritdoc/>
        protected override void BeforeActivatePlugInType(Type plugInType)
        {
            base.BeforeActivatePlugInType(plugInType);

            var knownPlugIn = this.FindKnownPlugin(plugInType);
            if (knownPlugIn == null && this.clientVersionProvider.ClientVersion.IsPlugInSuitable(plugInType))
            {
                var plugIn = this.CreatePlugIn(plugInType);
                this.AddPlugIn(plugIn, true);
            }
        }

        /// <inheritdoc />
        protected override void ActivatePlugIn(THandler plugIn)
        {
            var newPlugInIsEffective = false;
            if (this.TryGetPlugIn(plugIn.Key, out var currentlyActivePlugIn))
            {
                if (currentlyActivePlugIn == plugIn)
                {
                    return;
                }

                if (currentlyActivePlugIn.IsPreferedTo(plugIn))
                {
                    return;
                }

                newPlugInIsEffective = true;
            }

            base.ActivatePlugIn(plugIn);
            if (newPlugInIsEffective)
            {
                this.SetEffectivePlugin(plugIn);
            }
        }

        /// <inheritdoc />
        protected override void DeactivatePlugIn(THandler plugIn)
        {
            var isEffectivePlugIn = this.TryGetPlugIn(plugIn.Key, out var currentlyActivePlugIn) && currentlyActivePlugIn == plugIn;
            base.DeactivatePlugIn(plugIn);

            if (!isEffectivePlugIn)
            {
                return;
            }

            // find available replacement if the plugin was effective before
            var replacement = this.ActivePlugIns
                .Where(p => p.Key == plugIn.Key)
                .OrderByDescending(p => p.GetType().GetCustomAttribute(typeof(MinimumClientAttribute)))
                .FirstOrDefault();
            if (replacement != null)
            {
                this.SetEffectivePlugin(replacement);
            }
        }

        private THandler CreatePlugIn(Type plugInType)
        {
            if (!HandlerCache.TryGetValue(plugInType, out var plugIn))
            {
                if (plugInType.GetConstructors().Any(c => !c.GetParameters().Any()))
                {
                    plugIn = Activator.CreateInstance(plugInType) as THandler;
                    HandlerCache.TryAdd(plugInType, plugIn);
                }
                else
                {
                    using (var unityContainer = new UnityContainer())
                    {
                        unityContainer.RegisterInstance(typeof(PlugInManager), this.Manager, new ExternallyControlledLifetimeManager());
                        unityContainer.RegisterInstance(typeof(IClientVersionProvider), this.clientVersionProvider, new ExternallyControlledLifetimeManager());
                        plugIn = unityContainer.Resolve(plugInType) as THandler;
                        if (plugIn is GroupPacketHandlerPlugIn groupPacketHandler)
                        {
                            groupPacketHandler.Initialize();
                        }
                    }
                }
            }

            return plugIn;
        }

        private void OnClientVersionChanged(object sender, EventArgs e)
        {
            Log.Warn("Client version changed");

            foreach (var knownPlugIn in this.KnownPlugIns)
            {
                if (!this.clientVersionProvider.ClientVersion.IsPlugInSuitable(knownPlugIn.GetType()))
                {
                    this.DeactivatePlugIn(knownPlugIn);
                    this.RemovePlugIn(knownPlugIn);
                }
            }

            this.Initialize();
        }
    }
}