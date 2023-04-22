// <copyright file="MiniGameOpeningStateRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for mini game opening state request packets.
/// </summary>
[PlugIn(nameof(MiniGameOpeningStateRequestHandlerPlugIn), "Handler for mini game opening state request packets.")]
[Guid("15BAE3E6-4654-425A-8809-FA53A4C54D09")]
internal class MiniGameOpeningStateRequestHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <summary>
    /// The game action which contains the logic to respond to the state request.
    /// </summary>
    private readonly MiniGameOpeningStateRequestAction _requestAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => MiniGameOpeningStateRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < MiniGameOpeningStateRequest.Length
            || player.SelectedCharacter?.CharacterClass is null)
        {
            return;
        }

        MiniGameOpeningStateRequest request = packet;
        var eventType = request.EventType;
        await this._requestAction.HandleRequestAsync(player, Convert(eventType), request.EventLevel).ConfigureAwait(false);
    }

    private static MiniGameType Convert(Network.Packets.MiniGameType type)
    {
        return type switch
        {
            Network.Packets.MiniGameType.ChaosCastle => MiniGameType.ChaosCastle,
            Network.Packets.MiniGameType.DevilSquare => MiniGameType.DevilSquare,
            Network.Packets.MiniGameType.BloodCastle => MiniGameType.BloodCastle,
            Network.Packets.MiniGameType.IllusionTemple => MiniGameType.IllusionTemple,
            Network.Packets.MiniGameType.Doppelganger => MiniGameType.Doppelganger,
            _ => throw new ArgumentOutOfRangeException($"Unknown mini game type {type}", nameof(type)),
        };
    }
}