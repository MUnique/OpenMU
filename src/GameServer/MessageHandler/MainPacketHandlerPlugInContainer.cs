// <copyright file="MainPacketHandlerPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin container which provides the effective packet handler plugins for the specified version.
    /// </summary>
    public class MainPacketHandlerPlugInContainer : PacketHandlerPlugInContainer<IPacketHandlerPlugIn>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPacketHandlerPlugInContainer"/> class.
        /// </summary>
        /// <param name="clientVersionProvider">The client version.</param>
        /// <param name="manager">The manager.</param>
        public MainPacketHandlerPlugInContainer(IClientVersionProvider clientVersionProvider, PlugInManager manager)
            : base(clientVersionProvider, manager)
        {
        }
    }
}