// <copyright file="DoppelgangerEnterRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameServer.MessageHandler.MuHelper;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for the <see cref="DoppelgangerEnterRequest"/>, which is sent by the
/// entrance window of the NPC Lugard.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DoppelgangerEnterRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.DoppelgangerEnterRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("B21E6F93-4C7A-4D18-8E5B-9A30C2D7F615")]
[BelongsToGroup(MuHelperGroupHandler.GroupKey)]
internal class DoppelgangerEnterRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly EnterDoppelgangerAction _enterAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => DoppelgangerEnterRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < DoppelgangerEnterRequest.Length
            || player.SelectedCharacter?.CharacterClass is null
            || player.OpenedNpc?.Definition.NpcWindow != NpcWindow.LugardDoppelgangerEntry)
        {
            return;
        }

        DoppelgangerEnterRequest request = packet;
        await this._enterAction.TryEnterAsync(player, request.TicketItemSlot).ConfigureAwait(false);

        if (player.CurrentMiniGame is not null)
        {
            // The client closes the window when it changes the map, so the player isn't at Lugard anymore.
            player.OpenedNpc = null;
        }
    }
}
