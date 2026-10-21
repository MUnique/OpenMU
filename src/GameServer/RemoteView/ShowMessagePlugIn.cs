// <copyright file="ShowMessagePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views;
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
    /// Sends a notice which the client scrolls through the notice band at the top of the screen.
    /// </summary>
    /// <param name="message">The message.</param>
    private ValueTask SendSlideNoticeAsync(string message)
    {
        // White text at the default speed of the client. The type and the loop count keep
        // the defaults of the packet: queued in the notice band, shown once.
        const uint white = 0xFFFFFFFF;
        const byte defaultSpeed = 0;
        return this._player.Connection.SendSlideNoticeAsync(loopDelay: 0, white, defaultSpeed, message);
    }
}
