// <copyright file="DirectChatRoomRequestPublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.FriendServer;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A direct implementation of <see cref="IChatRoomRequestPublisher"/> that calls the chat server directly.
/// This is a transitional implementation that maintains existing behavior while enabling dependency injection.
/// </summary>
public class DirectChatRoomRequestPublisher : IChatRoomRequestPublisher
{
    private readonly IChatServer _chatServer;
    private readonly IFriendNotifier _friendNotifier;
    private readonly ILogger<DirectChatRoomRequestPublisher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectChatRoomRequestPublisher"/> class.
    /// </summary>
    /// <param name="chatServer">The chat server.</param>
    /// <param name="friendNotifier">The friend notifier.</param>
    /// <param name="logger">The logger.</param>
    public DirectChatRoomRequestPublisher(IChatServer chatServer, IFriendNotifier friendNotifier, ILogger<DirectChatRoomRequestPublisher> logger)
    {
        this._chatServer = chatServer ?? throw new ArgumentNullException(nameof(chatServer));
        this._friendNotifier = friendNotifier ?? throw new ArgumentNullException(nameof(friendNotifier));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async ValueTask PublishChatRoomCreationRequestAsync(string playerName, string friendName, int playerServerId, int friendServerId)
    {
        try
        {
            var roomId = await this._chatServer.CreateChatRoomAsync().ConfigureAwait(false);
            
            // Register both players in the chat room
            var playerAuthInfo = await this._chatServer.RegisterClientAsync(roomId, playerName).ConfigureAwait(false);
            if (playerAuthInfo is not null)
            {
                await this._friendNotifier.ChatRoomCreatedAsync(playerServerId, playerAuthInfo, friendName).ConfigureAwait(false);
            }

            var friendAuthInfo = await this._chatServer.RegisterClientAsync(roomId, friendName).ConfigureAwait(false);
            if (friendAuthInfo is not null)
            {
                await this._friendNotifier.ChatRoomCreatedAsync(friendServerId, friendAuthInfo, playerName).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error creating chat room for players {Player} and {Friend}", playerName, friendName);
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> PublishChatRoomInvitationRequestAsync(string playerName, string friendName, ushort roomId, int playerServerId, int friendServerId)
    {
        try
        {
            // Register the friend in the existing chat room
            var friendAuthInfo = await this._chatServer.RegisterClientAsync(roomId, friendName).ConfigureAwait(false);
            if (friendAuthInfo is not null)
            {
                await this._friendNotifier.ChatRoomCreatedAsync(friendServerId, friendAuthInfo, playerName).ConfigureAwait(false);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error inviting {Friend} to chat room {RoomId}", friendName, roomId);
            return false;
        }
    }
}