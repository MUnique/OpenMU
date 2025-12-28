// <copyright file="ChatViewPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using System.Text;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Chat plugin for 0.97 clients which expect latin1 encoded strings.
/// </summary>
[PlugIn(nameof(ChatViewPlugIn097), "Chat plugin for 0.97 clients with latin1 encoding.")]
[Guid("2C1A5E6D-8D77-4DA7-A4F3-98B35347F2A2")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class ChatViewPlugIn097 : IChatViewPlugIn
{
    private static readonly Encoding MessageEncoding = Encoding.Latin1;
    private const int MaxMessageLength = 241;
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatViewPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ChatViewPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ChatMessageAsync(string message, string sender, ChatMessageType type)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var messageLength = MessageEncoding.GetByteCount(message);
        if (messageLength > MaxMessageLength)
        {
            var trimmedLength = MessageEncoding.GetCharacterCountOfMaxByteCount(message, MaxMessageLength);
            message = message.Substring(0, trimmedLength);
            messageLength = MessageEncoding.GetByteCount(message);
        }

        var senderBytes = MessageEncoding.GetBytes(sender);
        var senderLength = Math.Min(senderBytes.Length, 10);

        int WritePacket()
        {
            var packetLength = messageLength + 1 + 13;
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = (byte)(type == ChatMessageType.Whisper ? 0x02 : 0x00);
            span.Slice(3, 10).Clear();
            senderBytes.AsSpan(0, senderLength).CopyTo(span.Slice(3, senderLength));
            span.Slice(13).Clear();
            MessageEncoding.GetBytes(message, span.Slice(13, messageLength));
            return packetLength;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }
}
