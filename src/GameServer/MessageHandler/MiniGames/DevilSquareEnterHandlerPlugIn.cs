// <copyright file="DevilSquareEnterHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for devil square enter request packets.
/// </summary>
[PlugIn(nameof(DevilSquareEnterHandlerPlugIn), "Handler for devil square enter packets.")]
[Guid("550FFF1B-E31C-44BA-8CC9-100D5649CC87")]
internal class DevilSquareEnterHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <summary>
    /// The game action which contains the logic to enter the mini game.
    /// </summary>
    private readonly EnterMiniGameAction _enterAction = new ();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => DevilSquareEnterRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < DevilSquareEnterRequest.Length
            || player.SelectedCharacter?.CharacterClass is null)
        {
            return;
        }

        DevilSquareEnterRequest request = packet;
        var actualLevel = request.SquareLevel + 1;
        var ticketIndex = request.TicketItemInventoryIndex - InventoryConstants.EquippableSlotsCount;
        await this._enterAction.TryEnterMiniGameAsync(player, MiniGameType.DevilSquare, actualLevel, (byte)ticketIndex).ConfigureAwait(false);
    }
}