// <copyright file="ShowMessagePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Buffers.Binary;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowMessagePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ShowMessagePlugIn_Name), Description = nameof(PlugInResources.ShowMessagePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("e294f4ce-f2c6-4a92-8cd0-40d8d5afae66")]
public class ShowMessagePlugIn : IShowMessagePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowMessagePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowMessagePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowMessageAsync(string message, OpenMU.Interfaces.MessageType messageType)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        const int maxMessageLength = 241;

        if (Encoding.UTF8.GetByteCount(message) > maxMessageLength)
        {
            var rest = message;
            while (rest.Length > 0)
            {
                var partSize = Encoding.UTF8.GetCharacterCountOfMaxByteCount(rest, maxMessageLength);
                await this.ShowMessageAsync(rest.Substring(0, partSize), messageType).ConfigureAwait(false);
                rest = rest.Length > partSize ? rest.Substring(startIndex: partSize) : string.Empty;
            }

            return;
        }

        if (messageType == Interfaces.MessageType.SlideNotice)
        {
            if (this._player.ClientVersion.Season > 0)
            {
                await this.SendSlideNoticeAsync(message).ConfigureAwait(false);
                return;
            }

            // Clients before season 1 don't know the slide notice.
            messageType = Interfaces.MessageType.GoldenCenter;
        }

        const string messagePrefix = "000000000";
        var content = this._player.ClientVersion.Season > 0 ? messagePrefix + message : message;
        await this._player.Connection.SendServerMessageAsync(ConvertMessageType(messageType), content).ConfigureAwait(false);
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

    /// <summary>
    /// Sends a server message which the client scrolls across the top of the screen.
    /// </summary>
    /// <remarks>
    /// The <see cref="ServerMessage"/> packet only defines the types 0 to 2. For the types 10 to 15
    /// the client also reads the nine bytes which the other types fill with the "000000000" prefix:
    /// <code>
    /// [3] type | [4] loop count | [5] padding | [6..7] loop delay in seconds (LE)
    /// [8..11] text color 0xAABBGGRR (LE) | [12] speed x 10 (0 = default) | [13..] UTF-8 text + NUL
    /// </code>
    /// Type 14 is the bold notice band, which has priority over the level tips.
    /// </remarks>
    private async ValueTask SendSlideNoticeAsync(string message)
    {
        const byte slideNoticeType = 14;
        const int textOffset = 13;
        const uint white = 0xFFFFFFFF;

        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int WritePacket()
        {
            var length = ServerMessageRef.GetRequiredSize(message) + (textOffset - 4);
            var span = connection.Output.GetSpan(length)[..length];
            var packet = new ServerMessageRef(span);
            packet.Type = (ServerMessage.MessageType)slideNoticeType;
            span[4] = 1; // loop count
            span[5] = 0;
            BinaryPrimitives.WriteUInt16LittleEndian(span[6..], 0); // loop delay
            BinaryPrimitives.WriteUInt32LittleEndian(span[8..], white);
            span[12] = 0; // default speed
            span[textOffset..].WriteString(message, Encoding.UTF8);
            return length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }
}
