// <copyright file="LetterDeleteHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for letter delete packets.
/// </summary>
[PlugIn("LetterDeleteHandlerPlugIn", "Handler for letter delete packets.")]
[Guid("3334483b-2de2-47ff-8d74-7407d3ddf15f")]
internal class LetterDeleteHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly LetterDeleteAction _deleteAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => LetterDeleteRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Span[3] != 0)
        {
            player.Logger.LogWarning("Player {0} Unknown Letter Delete Request: {1}", player.SelectedCharacter?.Name, packet.Span.AsString());
            return;
        }

        LetterDeleteRequest message = packet;
        var letterIndex = message.LetterIndex;
        if (letterIndex < player.SelectedCharacter?.Letters.Count)
        {
            var letter = player.SelectedCharacter!.Letters[letterIndex];
            await this._deleteAction.DeleteLetterAsync(player, letter).ConfigureAwait(false);
        }
    }
}