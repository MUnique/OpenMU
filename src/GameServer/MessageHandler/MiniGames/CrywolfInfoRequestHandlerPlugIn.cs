// <copyright file="CrywolfInfoRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Crywolf;
using MUnique.OpenMU.GameServer.Properties;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for 0xBD/0x00 — CrywolfInfoRequest, which the client sends when it entered the crywolf map.
/// The server responds with the state of the crywolf event.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CrywolfInfoRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.CrywolfInfoRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("A8C3F517-2B6D-4E90-9F4A-1D7E3C5B8A26")]
[BelongsToGroup(CrywolfGroupHandlerPlugIn.GroupKey)]
internal class CrywolfInfoRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CrywolfInfoRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < CrywolfInfoRequest.Length
            || CrywolfPlugIn.GetContext(player.GameContext) is not { } context)
        {
            return;
        }

        await context.ShowCurrentStateAsync(player).ConfigureAwait(false);
    }
}
