// <copyright file="DuelStartResponseHandlerPlugIn.cs" company="MUnique">
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
/// Handler for duel start response packets (new duel system).
/// </summary>
[PlugIn(nameof(DuelStartResponseHandlerPlugIn), "Handler for duel start response packets (new duel system).")]
[Guid("C2A7FD08-3F93-467E-AE8B-13F97B7F7888")]
[MinimumClient(4, 0, ClientLanguage.Invariant)]
[BelongsToGroup(DuelGroupHandlerPlugIn.GroupKey)]
internal class DuelStartResponseHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly DuelActions _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => DuelStartResponse.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < DuelStartResponse.Length)
        {
            return;
        }

        DuelStartResponse response = packet;
        var accepted = response.Response;
        var targetId = response.PlayerId;
        var targetName = response.PlayerName;

        var target = await player.GetObservingPlayerWithIdAsync(targetId).ConfigureAwait(false);

        if (target is null)
        {
            player.Logger.LogWarning($"Player {player.Name} sent response for duel player with id {targetId}, but the player was not found.");
            return;
        }

        if (target.Name != targetName)
        {
            player.Logger.LogWarning($"Player {player.Name} sent duel response for player {target.Name} with id {targetId}, but the names didn't match.");
            return;
        }

        await this._action.HandleDuelResponseAsync(player, target, accepted).ConfigureAwait(false);
    }
}