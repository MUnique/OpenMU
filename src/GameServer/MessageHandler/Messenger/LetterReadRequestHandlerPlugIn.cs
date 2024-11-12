// <copyright file="LetterReadRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for letter read request packets.
/// </summary>
[PlugIn("LetterReadRequestHandlerPlugIn", "Handler for letter read request packets.")]
[Guid("056ffd3b-567b-4787-9d07-2c9d8a5a7175")]
internal class LetterReadRequestHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly LetterReadRequestAction _readAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => LetterReadRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        LetterReadRequest message = packet;
        await this._readAction.ReadRequestAsync(player, message.LetterIndex).ConfigureAwait(false);
    }
}