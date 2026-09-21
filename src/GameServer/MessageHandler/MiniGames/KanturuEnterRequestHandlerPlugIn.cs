// <copyright file="KanturuEnterRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameServer.Properties;
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
[Display(Name = nameof(PlugInResources.KanturuEnterRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.KanturuEnterRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("E6A3C9B2-5D84-4F10-9B42-8C7A0F3D5E19")]
[BelongsToGroup(KanturuGroupHandlerPlugIn.GroupKey)]
internal class KanturuEnterRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <summary>
    /// The Kanturu event has no ticket item, so no inventory slot is sent.
    /// </summary>
    private const byte UndefinedTicketSlot = 0xFF;

    private readonly EnterMiniGameAction _enterAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => KanturuEnterRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < KanturuEnterRequest.Length
            || player.SelectedCharacter?.CharacterClass is null
            || player.OpenedNpc?.Definition.Number != KanturuGatewayPlugIn.GatewayMachineNumber)
        {
            return;
        }

        // Try to enter the Kanturu mini game.
        // On success: the player is teleported to the event map.
        // On failure: TryEnterMiniGameAsync shows a message to the player and the client
        //             NPC animation resets naturally at frame 50, so the dialog stays usable.
        await this._enterAction.TryEnterMiniGameAsync(player, MiniGameType.Kanturu, 1, UndefinedTicketSlot)
            .ConfigureAwait(false);

        if (player.CurrentMiniGame is not null)
        {
            // The client closes the dialog when it changes the map, so the player isn't at
            // the gateway NPC anymore.
            player.OpenedNpc = null;
        }
    }
}
