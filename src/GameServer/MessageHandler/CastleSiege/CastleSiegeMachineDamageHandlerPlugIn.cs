// <copyright file="CastleSiegeMachineDamageHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consumes the legacy machine-impact callback sent by the game client.
/// Damage is calculated server-side by <see cref="CastleSiegeMachineUseAction"/> and is never trusted to this callback.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeMachineDamageHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeMachineDamageHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("5912EA3B-FB4E-4C12-B8B0-C24550408E3A")]
[BelongsToGroup(CastleSiegeMachineGroupHandlerPlugIn.GroupKey)]
internal sealed class CastleSiegeMachineDamageHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => WeaponExplosionRequest.SubCode;

    /// <inheritdoc />
    public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < WeaponExplosionRequest.Length)
        {
            return ValueTask.CompletedTask;
        }

        _ = new WeaponExplosionRequest(packet);
        return ValueTask.CompletedTask;
    }
}
