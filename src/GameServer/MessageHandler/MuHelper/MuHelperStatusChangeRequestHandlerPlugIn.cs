// <copyright file="MuHelperStatusChangeRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MuHelper;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.PlayerActions.MuHelper;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for MU Helper use requests.
/// </summary>
[PlugIn(nameof(MuHelperStatusChangeRequestHandlerPlugIn), "Handler for MU Helper status change requests.")]
[Guid("91B5040E-44B6-41FC-A0AB-A881770B2A16")]
[BelongsToGroup(MuHelperGroupHandler.GroupKey)]
public class MuHelperStatusChangeRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly ChangeMuHelperStateAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => MuHelperStatusChangeRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        MuHelperStatusChangeRequest message = packet;
        var status = message.PauseStatus ? MuHelperStatus.Disabled : MuHelperStatus.Enabled;
        await this._action.ChangeHelperStateAsync(player, status).ConfigureAwait(false);
    }
}