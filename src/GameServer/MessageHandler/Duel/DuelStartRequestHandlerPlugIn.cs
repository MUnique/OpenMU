// <copyright file="DuelStartRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Duel;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Duel;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for duel start request packets (new duel system).
/// </summary>
[PlugIn(nameof(DuelStartRequestHandlerPlugIn), "Handler for duel start request packets (new duel system).")]
[Guid("300D44EF-3AFD-43C3-9817-AE9B023C25CC")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
[BelongsToGroup(DuelGroupHandlerPlugIn.GroupKey)]
internal class DuelStartRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly DuelActions _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => DuelStartRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < DuelStartRequest.Length)
        {
            return;
        }

        DuelStartRequest request = packet;

        var targetId = request.PlayerId;
        var targetName = request.PlayerName;

        var target = await player.GetObservingPlayerWithIdAsync(targetId).ConfigureAwait(false);

        if (target is null)
        {
            player.Logger.LogWarning($"Player {player.Name} tried to duel player with id {targetId}, but the player was not found.");
            return;
        }

        if (target.Name != targetName)
        {
            player.Logger.LogWarning($"Player {player.Name} tried to duel player {target.Name} with id {targetId}, but the names didn't match.");
            return;
        }

        await this._action.HandleDuelRequestAsync(player, target).ConfigureAwait(false);
    }
}