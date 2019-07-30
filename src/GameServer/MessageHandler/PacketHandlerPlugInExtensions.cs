// <copyright file="PacketHandlerPlugInExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System.Reflection;

    /// <summary>
    /// Extension methods for <see cref="IPacketHandlerPlugIn"/>s.
    /// </summary>
    public static class PacketHandlerPlugInExtensions
    {
        /// <summary>
        /// Determines whether this packet handler is preferred over the specified other packet handler.
        /// </summary>
        /// <param name="packetHandler">The packet handler.</param>
        /// <param name="otherPacketHandlerPlugIn">The other packet handler plug in.</param>
        /// <returns>
        ///   <c>true</c> if this packet handler is preferred over the specified other packet handler; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPreferedTo(this IPacketHandlerPlugInBase packetHandler, IPacketHandlerPlugInBase otherPacketHandlerPlugIn)
        {
            var currentPlugInClient = packetHandler.GetType().GetCustomAttribute<MinimumClientAttribute>()?.Client ?? default;
            var activatedPluginClient = otherPacketHandlerPlugIn.GetType().GetCustomAttribute<MinimumClientAttribute>()?.Client ?? default;
            return currentPlugInClient.CompareTo(activatedPluginClient) > 0;
        }
    }
}
