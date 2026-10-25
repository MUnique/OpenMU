// <copyright file="PingHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for the ping packet, which the client sends periodically.
/// It reports that the client is alive, so that lost connections can be detected.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.PingHandlerPlugIn_Name), Description = nameof(PlugInResources.PingHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("5EC0FCA3-726B-45CA-A696-BB94FF7E7497")]
internal class PingHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => Ping.Code;

    /// <inheritdoc/>
    public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        player.ReportAlive();
        return ValueTask.CompletedTask;
    }
}
