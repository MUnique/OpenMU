// <copyright file="GensRewardRequestHandlerPlugIn.cs" company="MUnique">
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
/// Handler for the request of the gens ranking reward.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GensRewardRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.GensRewardRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("9B3D7F52-E6A1-4C08-B74F-1A5C8E2D9036")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
[BelongsToGroup(GensGroupHandlerPlugIn.GroupKey)]
internal class GensRewardRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly GensActions _actions = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => GensRewardRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < GensRewardRequest.Length)
        {
            return;
        }

        GensRewardRequest request = packet;
        await this._actions.RequestRewardAsync(player, request.GensType.ToGensType()).ConfigureAwait(false);
    }
}
