// <copyright file="FriendServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.FriendServer;

using System.Collections.Immutable;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The friend server which manages the friend list with chat and letter system.
/// </summary>
public class FriendServer : IFriendServer
{
    private readonly IPersistenceContextProvider _persistenceContextProvider;
    private readonly IFriendNotifier _friendNotifier;
    private readonly IChatServer _chatServer;

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendServer" /> class.
    /// </summary>
    /// <param name="friendNotifier">The friend notifier.</param>
    /// <param name="chatServer">The chat server.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    public FriendServer(IFriendNotifier friendNotifier, IChatServer chatServer, IPersistenceContextProvider persistenceContextProvider)
    {
        this._friendNotifier = friendNotifier;
        this._chatServer = chatServer;
        this._persistenceContextProvider = persistenceContextProvider;
        this.OnlineFriends = new Dictionary<string, OnlineFriend>();
    }

    /// <summary>
    /// Gets the server id which represents being offline.
    /// </summary>
    public static int OfflineServerId { get; } = (int)SpecialServerId.Offline;

    /// <summary>
    /// Gets the server id which represents being invisible (=offline to other players).
    /// </summary>
    public static int InvisibleServerId { get; } = (int)SpecialServerId.Invisible;

    /// <summary>
    /// Gets the online friends dictionary. The key is the name of the character of the corresponding OnlineFriend object.
    /// </summary>
    protected IDictionary<string, OnlineFriend> OnlineFriends { get; }

    /// <inheritdoc/>
    public ValueTask ForwardLetterAsync(LetterHeader letter)
    {
        return this._friendNotifier.LetterReceivedAsync(letter);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> FriendRequestAsync(string playerName, string friendName)
    {
        var saveSuccess = true;
        bool friendIsNew;
        using (var context = this._persistenceContextProvider.CreateNewFriendServerContext())
        {
            var friend = await context.GetFriendByNamesAsync(playerName, friendName);
            friendIsNew = friend is null;
            if (friendIsNew)
            {
                friend = await context.CreateNewFriendAsync(playerName, friendName);
                friend.Accepted = false;
                friend.RequestOpen = true;
                saveSuccess = await context.SaveChangesAsync();
            }
        }

        if (saveSuccess && this.OnlineFriends.TryGetValue(friendName, out var onlineFriend))
        {
            // Friend is online, so we directly send him a request.
            await this._friendNotifier.FriendRequestAsync(playerName, friendName, onlineFriend.ServerId);
        }

        return friendIsNew && saveSuccess;
    }

    /// <inheritdoc/>
    public async ValueTask DeleteFriendAsync(string playerName, string friendName)
    {
        if (this.OnlineFriends.TryGetValue(playerName, out var player) && this.OnlineFriends.TryGetValue(friendName, out var friend))
        {
            player.RemoveSubscriber(friend);
        }

        using var context = this._persistenceContextProvider.CreateNewFriendServerContext();
        await context.DeleteAsync(playerName, friendName);
        await context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async ValueTask FriendResponseAsync(string characterName, string friendName, bool accepted)
    {
        using var context = this._persistenceContextProvider.CreateNewFriendServerContext();
#pragma warning disable S2234 // The parameters are passed correctly
        var requester = await context.GetFriendByNamesAsync(friendName, characterName);
#pragma warning restore S2234
        if (requester is null)
        {
            return;
        }

        requester.RequestOpen = false;
        requester.Accepted = accepted;

        if (accepted)
        {
            var responder = await context.GetFriendByNamesAsync(characterName, friendName) ?? await context.CreateNewFriendAsync(characterName, friendName);
            responder.RequestOpen = false;
            responder.Accepted = true;
            await context.SaveChangesAsync();
            this.AddSubscriptions(friendName, characterName);
        }
        else
        {
            await context.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async ValueTask CreateChatRoomAsync(string playerName, string friendName)
    {
        if (!this.OnlineFriends.TryGetValue(playerName, out var player))
        {
            return;
        }

        if (!this.OnlineFriends.TryGetValue(friendName, out var friend))
        {
            return;
        }

        if (!friend.HasSubscriber(player))
        {
            return;
        }

        // TODO: Remove direct dependency to the chat server.
        //       Instead of calling the chat server directly here, we could publish a request
        //       to create the chat room to an pub/sub-system. An available chat server could then
        //       process the request and notify the corresponding game servers.
        var roomId = await this._chatServer.CreateChatRoomAsync();
        if (await this._chatServer.RegisterClientAsync(roomId, playerName) is { } authenticationInfoPlayer)
        {
            await this._friendNotifier.ChatRoomCreatedAsync(player.ServerId, authenticationInfoPlayer, friendName);
        }

        if (await this._chatServer.RegisterClientAsync(roomId, friendName) is { } authenticationInfoFriend)
        {
            await this._friendNotifier.ChatRoomCreatedAsync(friend.ServerId, authenticationInfoFriend, playerName);
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> InviteFriendToChatRoomAsync(string playerName, string friendName, ushort roomId)
    {
        if (!this.OnlineFriends.TryGetValue(playerName, out var player))
        {
            return false;
        }

        if (!this.OnlineFriends.TryGetValue(friendName, out var friend))
        {
            return false;
        }

        if (!friend.HasSubscriber(player))
        {
            return false;
        }

        if (friend.IsInvisibleOrOffline)
        {
            return false;
        }

        var authenticationInfoFriend = await this._chatServer.RegisterClientAsync(roomId, friendName);
        if (authenticationInfoFriend is not null)
        {
            await this._friendNotifier.ChatRoomCreatedAsync(friend.ServerId, authenticationInfoFriend, playerName);
            return true;
        }

        return false;
    }

    /// <remarks>Note, that the ServerId is not filled by this implementation. The player will receive it separately when the subscription is created.</remarks>
    /// <inheritdoc/>
    public async ValueTask PlayerEnteredGameAsync(byte serverId, Guid characterId, string characterName)
    {
        using var context = this._persistenceContextProvider.CreateNewFriendServerContext();
        var friends = await context.GetFriendNamesAsync(characterId);
        var requesters = await context.GetOpenFriendRequesterNamesAsync(characterId);
        var initializationData = new MessengerInitializationData(
            characterName,
            friends.ToImmutableList(),
            requesters.ToImmutableList());

        await this._friendNotifier.InitializeMessengerAsync(serverId, initializationData);

        await this.SetOnlineStateAsync(characterId, characterName, serverId, context);
    }

    /// <inheritdoc/>
    public async ValueTask PlayerLeftGameAsync(Guid characterId, string characterName)
    {
        await this.SetOnlineStateAsync(characterId, characterName, OfflineServerId, null);
    }

    /// <inheritdoc/>
    public async ValueTask SetPlayerVisibilityStateAsync(byte serverId, Guid characterId, string characterName, bool isVisible)
    {
        await this.SetOnlineStateAsync(characterId, characterName, isVisible ? serverId : InvisibleServerId, null);
    }

    private async ValueTask SetOnlineStateAsync(Guid characterId, string characterName, int serverId, IFriendServerContext? usedContext)
    {
        if (!this.OnlineFriends.TryGetValue(characterName, out var observer))
        {
            if (serverId == InvisibleServerId || serverId == OfflineServerId)
            {
                return;
            }

            observer = new OnlineFriend(this._friendNotifier, characterName)
            {
                ServerId = serverId,
            };
            this.OnlineFriends.Add(characterName, observer);
            IFriendServerContext? newContext = null;
            var context = usedContext ?? (newContext = this._persistenceContextProvider.CreateNewFriendServerContext());

            try
            {
                var friends = await context.GetFriendsAsync(characterId);
                this.AddSubscriptions(observer, friends);
            }
            finally
            {
                newContext?.Dispose();
            }
        }

        observer.ChangeServer(serverId == InvisibleServerId ? OfflineServerId : serverId);

        if (serverId == OfflineServerId)
        {
            this.OnlineFriends.Remove(observer.PlayerName);
            observer.OnCompleted();
        }
    }

    private void AddSubscriptions(OnlineFriend observer, IEnumerable<FriendViewItem> friends)
    {
        foreach (var friend in friends)
        {
            if (!friend.Accepted || friend.RequestOpen)
            {
                continue;
            }

            if (this.OnlineFriends.TryGetValue(friend.FriendName, out var onlineFriend))
            {
                observer.AddSubscription(onlineFriend.Subscribe(observer));
                onlineFriend.AddSubscription(observer.Subscribe(onlineFriend));

                observer.OnNext(onlineFriend);
            }
        }
    }

    private void AddSubscriptions(string requester, string responder)
    {
        if (!this.OnlineFriends.TryGetValue(responder, out var responderFriend))
        {
            return;
        }

        if (this.OnlineFriends.TryGetValue(requester, out var requesterFriend))
        {
            responderFriend.AddSubscription(requesterFriend.Subscribe(responderFriend));
            requesterFriend.AddSubscription(responderFriend.Subscribe(requesterFriend));

            // inform both about their online server ids:
            responderFriend.OnNext(requesterFriend);
            requesterFriend.OnNext(responderFriend);
        }
    }
}