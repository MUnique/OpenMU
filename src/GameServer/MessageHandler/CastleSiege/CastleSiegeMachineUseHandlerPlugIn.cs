// <copyright file="CastleSiegeMachineUseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles requests to fire a Castle Siege warfare machine.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeMachineUseHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeMachineUseHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("A65E70D4-4BF9-4A03-A640-7B329FC1582D")]
[BelongsToGroup(CastleSiegeMachineGroupHandlerPlugIn.GroupKey)]
internal sealed class CastleSiegeMachineUseHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly CastleSiegeMachineUseAction _action = new();

    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => FireCatapultRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < FireCatapultRequest.Length)
        {
            return;
        }

        var request = new FireCatapultRequest(packet);
        _ = await this._action
            .UseAsync(
                player,
                CastleSiegeHandlerContext.Get(player),
                request.CatapultId,
                request.TargetAreaIndex)
            .ConfigureAwait(false);
    }
}
