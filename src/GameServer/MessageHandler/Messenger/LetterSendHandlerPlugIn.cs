// <copyright file="LetterSendHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
using MUnique.OpenMU.GameLogic.Properties;
using MUnique.OpenMU.GameLogic.Views.Messenger;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;
using PlugInResources = MUnique.OpenMU.GameServer.Properties.PlugInResources;

/// <summary>
/// Handler for letter send packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.LetterSendHandlerPlugIn_Name), Description = nameof(PlugInResources.LetterSendHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("6d10d34d-bd20-4dcf-99eb-569d38ef1c1b")]
internal class LetterSendHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly LetterSendAction _sendAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => LetterSendRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        LetterSendRequest message = packet;
        if (packet.Length < 83)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.LetterInvalid)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<ILetterSendResultPlugIn>(p => p.LetterSendResultAsync(LetterSendSuccess.TryAgain, message.LetterId)).ConfigureAwait(false);
            return;
        }

        await this._sendAction.SendLetterAsync(player, message.Receiver, message.Message, message.Title, message.Rotation, message.Animation, message.LetterId).ConfigureAwait(false);
    }
}