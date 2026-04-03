// <copyright file="KanturuEnterRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for 0xD1/0x01 — KanturuEnterRequest.
/// The client sends this at animation frame 42 of the Gateway Machine NPC animation,
/// after the player clicked "Enter" in the INTERFACE_KANTURU2ND_ENTERNPC dialog.
/// On success the player is teleported to the event map (map 39) — no result packet needed.
/// On failure <see cref="EnterMiniGameAction"/> already shows an error to the player.
/// </summary>
[PlugIn]
[Display(Name = "Kanturu Enter Request Handler", Description = "Handles 0xD1/0x01 (KanturuEnterRequest) and teleports the player into the event.")]
[Guid("E6A3C9B2-5D84-4F10-9B42-8C7A0F3D5E19")]
[BelongsToGroup(KanturuGroupHandlerPlugIn.GroupKey)]
internal class KanturuEnterRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly EnterMiniGameAction _enterAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => KanturuEnterRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < KanturuEnterRequest.Length
            || player.SelectedCharacter?.CharacterClass is null)
        {
            return;
        }

        // Try to enter the Kanturu mini game.
        // On success: player is teleported to map 39 — the map change clears the dialog.
        // On failure: TryEnterMiniGameAsync shows a message to the player and the client
        //             NPC animation resets naturally at frame 50.
        await this._enterAction.TryEnterMiniGameAsync(player, MiniGameType.Kanturu, 1, 0xFF)
            .ConfigureAwait(false);
    }
}
