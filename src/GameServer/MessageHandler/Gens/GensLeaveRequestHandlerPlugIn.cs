// <copyright file="GensLeaveRequestHandlerPlugIn.cs" company="MUnique">
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
/// Handler for the request to leave the gens.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GensLeaveRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.GensLeaveRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("C58E3B17-9D42-4A6E-8F31-B2D7A4E90C65")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
[BelongsToGroup(GensGroupHandlerPlugIn.GroupKey)]
internal class GensLeaveRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly GensActions _actions = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => GensLeaveRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._actions.LeaveAsync(player).ConfigureAwait(false);
    }
}
