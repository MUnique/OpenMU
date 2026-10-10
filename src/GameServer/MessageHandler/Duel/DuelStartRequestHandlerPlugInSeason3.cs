// <copyright file="DuelStartRequestHandlerPlugInSeason3.cs" company="MUnique">
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
/// Handler for duel start request packets of the clients before Season 4, which don't use a sub code.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DuelStartRequestHandlerPlugInSeason3_Name), Description = nameof(PlugInResources.DuelStartRequestHandlerPlugInSeason3_Description), ResourceType = typeof(PlugInResources))]
[Guid("29B82173-7FD3-4BF7-B61C-9FA79232F5D8")]
[MaximumClient(3, 255, ClientLanguage.Invariant)]
internal class DuelStartRequestHandlerPlugInSeason3 : IPacketHandlerPlugIn
{
    private readonly DuelActions _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => DuelStartRequestSeason3.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < DuelStartRequestSeason3.Length)
        {
            return;
        }

        DuelStartRequestSeason3 request = packet;
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
