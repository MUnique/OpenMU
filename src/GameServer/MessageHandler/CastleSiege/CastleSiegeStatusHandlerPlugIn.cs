// <copyright file="CastleSiegeStatusHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles Castle Siege status requests, which the client sends when the guardsman window opens.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeStatusHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeStatusHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3E5E1A6A-A064-4F3E-8E96-B52860931601")]
[BelongsToGroup(CastleSiegeGroupHandlerPlugIn.GroupKey)]
internal class CastleSiegeStatusHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly CastleSiegeStatusAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CastleSiegeStatusRequest.SubCode;

    /// <inheritdoc/>
    public ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        return this._action.ShowStatusAsync(player, CastleSiegeHandlerContext.Get(player));
    }
}
