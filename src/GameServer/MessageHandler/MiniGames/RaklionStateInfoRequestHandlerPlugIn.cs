// <copyright file="RaklionStateInfoRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Raklion;
using MUnique.OpenMU.GameServer.Properties;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for 0xD1/0x10 — RaklionStateInfoRequest.
/// The server responds with the state of the raklion event.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.RaklionStateInfoRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.RaklionStateInfoRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("8F4D2A63-7C19-4B5E-A0D8-3E6C1B9F7A24")]
[BelongsToGroup(KanturuGroupHandlerPlugIn.GroupKey)]
internal class RaklionStateInfoRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => RaklionStateInfoRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < RaklionStateInfoRequest.Length
            || RaklionPlugIn.GetContext(player.GameContext) is not { } context)
        {
            return;
        }

        await player.InvokeViewPlugInAsync<IRaklionEventViewPlugIn>(p => p.ShowStateInfoAsync(context.State, context.SelupanState, context.CanEnterHatchery, context.RemainingTime)).ConfigureAwait(false);
    }
}
