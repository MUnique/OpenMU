// <copyright file="IPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Interface for a packet handler plugin.
    /// </summary>
    [CustomPlugInContainer("Packet Handlers", "Plugin Container for packet handlers.")]
    [Guid("DB9741B7-D4D7-4AB6-8179-E770C5EE38CE")]
    public interface IPacketHandlerPlugIn : IPacketHandlerPlugInBase
    {
    }
}