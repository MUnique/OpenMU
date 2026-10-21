// <copyright file="GensJoinRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Gens;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Gens;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for the request to join a gens.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GensJoinRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.GensJoinRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("7A1C5E93-2B64-4D8F-A0E7-C3B9F5D21A46")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
[BelongsToGroup(GensGroupHandlerPlugIn.GroupKey)]
internal class GensJoinRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly GensActions _actions = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => GensJoinRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < GensJoinRequest.Length)
        {
            return;
        }

        GensJoinRequest request = packet;
        await this._actions.JoinAsync(player, request.GensType.ToGensType()).ConfigureAwait(false);
    }
}
