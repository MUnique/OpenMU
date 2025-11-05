// <copyright file="ChatMessageBaseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Chat;
using MUnique.OpenMU.Network.Packets.ClientToServer;

/// <summary>
/// Base class for a chat message handler.
/// </summary>
internal abstract class ChatMessageBaseHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public abstract byte Key { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is handling whisper messages.
    /// </summary>
    protected abstract bool IsWhisper { get; }

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (player.GameContext is not IGameServerContext gameServerContext)
        {
            return;
        }

        var messageAction = new ChatMessageAction(gameServerContext.EventPublisher);
        WhisperMessage message = packet;
        await messageAction.ChatMessageAsync(player, message.ReceiverName, message.Message, this.IsWhisper).ConfigureAwait(false);
    }
}