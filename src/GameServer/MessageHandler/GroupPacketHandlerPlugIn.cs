// <copyright file="GroupPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Reflection;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// An abstract base class for a packet handler which has sub-packet handlers, implemented as
    /// <see cref="ISubPacketHandlerPlugIn"/>.
    /// </summary>
    internal abstract class GroupPacketHandlerPlugIn : PacketHandlerPlugInContainer<ISubPacketHandlerPlugIn>, IPacketHandlerPlugIn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupPacketHandlerPlugIn"/> class.
        /// </summary>
        /// <param name="clientVersionProvider">The client version provider.</param>
        /// <param name="manager">The manager.</param>
        protected GroupPacketHandlerPlugIn(IClientVersionProvider clientVersionProvider, PlugInManager manager)
            : base(clientVersionProvider, manager)
        {
        }

        /// <inheritdoc />
        public abstract byte Key { get; }

        /// <inheritdoc />
        public abstract bool IsEncryptionExpected { get; }

        /// <inheritdoc />
        public void HandlePacket(Player player, Span<byte> packet)
        {
            var subTypeIndex = packet[0] % 2 == 1 ? 3 : 4;
            var subPacketType = packet[subTypeIndex];
            this.HandlePacket(player, packet, this[subPacketType]);
        }

        /// <inheritdoc/>
        protected override void BeforeActivatePlugInType(Type plugInType)
        {
            if (this.PlugInBelongsToThisGroup(plugInType))
            {
                base.BeforeActivatePlugInType(plugInType);
            }
        }

        private bool PlugInBelongsToThisGroup(Type plugInType)
        {
            var belongsToAttribute = plugInType.GetCustomAttribute<BelongsToGroupAttribute>();
            if (belongsToAttribute == null)
            {
                return false;
            }

            return belongsToAttribute.GroupKey == this.Key;
        }
    }
}