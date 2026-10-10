// <copyright file="DuelStartResponseHandlerPlugInSeason3.cs" company="MUnique">
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
/// Handler for duel start response packets of the clients before Season 4, which answer the request with its own code.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DuelStartResponseHandlerPlugInSeason3_Name), Description = nameof(PlugInResources.DuelStartResponseHandlerPlugInSeason3_Description), ResourceType = typeof(PlugInResources))]
[Guid("BD067A54-811E-4194-9E8A-D5AF94E4E6A4")]
[MaximumClient(3, 255, ClientLanguage.Invariant)]
internal class DuelStartResponseHandlerPlugInSeason3 : IPacketHandlerPlugIn
{
    private readonly DuelActions _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => DuelStartResponseSeason3.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < DuelStartResponseSeason3.Length)
        {
            return;
        }

        DuelStartResponseSeason3 response = packet;
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
