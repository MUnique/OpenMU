// <copyright file="DuelStopResponseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Duel;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for duel stop request packets (new duel system).
/// </summary>
[PlugIn(nameof(DuelStopResponseHandlerPlugIn), "Handler for duel stop request packets (new duel system).")]
[Guid("98787A41-4729-4DC8-A00F-B06CC7207598")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
[BelongsToGroup(DuelGroupHandlerPlugIn.GroupKey)]
internal class DuelStopResponseHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly DuelActions _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => DuelStopRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._action.HandleStopDuelRequestAsync(player).ConfigureAwait(false);
    }
}