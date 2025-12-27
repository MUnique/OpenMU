// <copyright file="ShowMessagePlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using System.Text;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Message plugin for 0.97 clients which expect latin1 encoded strings.
/// </summary>
[PlugIn("ShowMessagePlugIn 0.97", "Show messages for 0.97 clients with latin1 encoding.")]
[Guid("1DF2A59B-0C95-4D62-80ED-53F2C9E35397")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class ShowMessagePlugIn097 : IShowMessagePlugIn
{
    private static readonly Encoding MessageEncoding = Encoding.Latin1;
    private const int MaxMessageLength = 250;
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowMessagePlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowMessagePlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowMessageAsync(string message, OpenMU.Interfaces.MessageType messageType)
    {
        if (MessageEncoding.GetByteCount(message) > MaxMessageLength)
        {
            var rest = message;
            while (rest.Length > 0)
            {
                var partSize = MessageEncoding.GetCharacterCountOfMaxByteCount(rest, MaxMessageLength);
                await this.ShowMessageAsync(rest.Substring(0, partSize), messageType).ConfigureAwait(false);
                rest = rest.Length > partSize ? rest.Substring(partSize) : string.Empty;
            }

            return;
        }

        var content = this._player.ClientVersion.Season > 0 ? $"000000000{message}" : message;
        await this.SendMessageAsync(ConvertMessageType(messageType), content).ConfigureAwait(false);
    }

    private ValueTask SendMessageAsync(ServerMessage.MessageType type, string message)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return ValueTask.CompletedTask;
        }

        int WritePacket()
        {
            var messageLength = MessageEncoding.GetByteCount(message);
            var packetLength = messageLength + 1 + 4;
            var span = connection.Output.GetSpan(packetLength)[..packetLength];
            span[0] = 0xC1;
            span[1] = (byte)packetLength;
            span[2] = 0x0D;
            span[3] = (byte)type;

            span.Slice(4).Clear();
            MessageEncoding.GetBytes(message, span.Slice(4, messageLength));
            return packetLength;
        }

        return connection.SendAsync(WritePacket);
    }

    private static ServerMessage.MessageType ConvertMessageType(OpenMU.Interfaces.MessageType messageType)
    {
        return messageType switch
        {
            Interfaces.MessageType.BlueNormal => ServerMessage.MessageType.BlueNormal,
            Interfaces.MessageType.GoldenCenter => ServerMessage.MessageType.GoldenCenter,
            Interfaces.MessageType.GuildNotice => ServerMessage.MessageType.GuildNotice,
            _ => throw new NotImplementedException($"Case for {messageType} is not implemented."),
        };
    }
}
