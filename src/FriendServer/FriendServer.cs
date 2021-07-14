// <copyright file="FriendServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.FriendServer
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// The friend server which manages the friend list with chat and letter system.
    /// </summary>
    public class FriendServer : IFriendServer
    {
        private readonly IPersistenceContextProvider persistenceContextProvider;

        private readonly IChatServer? chatServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendServer" /> class.
        /// </summary>
        /// <param name="gameServers">The game servers.</param>
        /// <param name="chatServer">The chat server.</param>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public FriendServer(IDictionary<int, IGameServer> gameServers, IChatServer? chatServer, IPersistenceContextProvider persistenceContextProvider)
        {
            this.chatServer = chatServer;
            this.GameServers = gameServers;
            this.persistenceContextProvider = persistenceContextProvider;
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

        /// <inheritdoc/>
        public string ChatServerIp => this.chatServer?.IpAddress ?? string.Empty;

        /// <summary>
        /// Gets the online friends dictionary. The key is the name of the character of the corresponding OnlineFriend object.
        /// </summary>
        protected IDictionary<string, OnlineFriend> OnlineFriends { get; }

        /// <summary>
        /// Gets the game servers.
        /// </summary>
        protected IDictionary<int, IGameServer> GameServers { get; }

        /// <inheritdoc/>
        public void ForwardLetter(LetterHeader letter)
        {
            foreach (var gameServer in this.GameServers.Values)
            {
                gameServer.LetterReceived(letter);
            }
        }

        /// <inheritdoc/>
        public bool FriendRequest(string playerName, string friendName)
        {
            var saveSuccess = true;
            bool friendIsNew;
            using (var context = this.persistenceContextProvider.CreateNewFriendServerContext())
            {
                var friend = context.GetFriendByNames(playerName, friendName);
                friendIsNew = friend is null;
                if (friendIsNew)
                {
                    friend = context.CreateNewFriend(playerName, friendName);
                    friend.Accepted = false;
                    friend.RequestOpen = true;
                    saveSuccess = context.SaveChanges();
                }
            }

            if (saveSuccess && this.OnlineFriends.TryGetValue(friendName, out var onlineFriend))
            {
                // Friend is online, so we directly send him a request.
                onlineFriend.GameServer.FriendRequest(playerName, friendName);
            }

            return friendIsNew && saveSuccess;
        }

        /// <inheritdoc/>
        public void DeleteFriend(string playerName, string friendName)
        {
            if (this.OnlineFriends.TryGetValue(playerName, out var player) && this.OnlineFriends.TryGetValue(friendName, out var friend))
            {
                player.RemoveSubscriber(friend);
            }

            using var context = this.persistenceContextProvider.CreateNewFriendServerContext();
            context.Delete(playerName, friendName);
            context.SaveChanges();
        }

        /// <inheritdoc/>
        public void FriendResponse(string characterName, string friendName, bool accepted)
        {
            using var context = this.persistenceContextProvider.CreateNewFriendServerContext();
#pragma warning disable S2234 // The parameters are passed correctly
            var requester = context.GetFriendByNames(friendName, characterName);
#pragma warning restore S2234
            if (requester is null)
            {
                return;
            }

            requester.RequestOpen = false;
            requester.Accepted = accepted;

            if (accepted)
            {
                var responder = context.GetFriendByNames(characterName, friendName) ?? context.CreateNewFriend(characterName, friendName);
                responder.RequestOpen = false;
                responder.Accepted = true;
                context.SaveChanges();
                this.AddSubscriptions(friendName, characterName);
            }
            else
            {
                context.SaveChanges();
            }
        }

        /// <inheritdoc/>
        public ChatServerAuthenticationInfo? CreateChatRoom(string playerName, string friendName)
        {
            if (this.chatServer is null)
            {
                return null;
            }

            if (!this.OnlineFriends.TryGetValue(playerName, out var player))
            {
                return null;
            }

            if (!this.OnlineFriends.TryGetValue(friendName, out var friend))
            {
                return null;
            }

            if (!friend.HasSubscriber(player))
            {
                return null;
            }

            if (!this.GameServers.TryGetValue(friend.ServerId, out var gameServerOfFriend))
            {
                return null;
            }

            var roomId = this.chatServer.CreateChatRoom();
            var authenticationInfoPlayer = this.chatServer.RegisterClient(roomId, playerName);
            var authenticationInfoFriend = this.chatServer.RegisterClient(roomId, friendName);
            gameServerOfFriend.ChatRoomCreated(authenticationInfoFriend, playerName);

            return authenticationInfoPlayer;
        }

        /// <inheritdoc />
        public bool InviteFriendToChatRoom(string playerName, string friendName, ushort roomId)
        {
            if (this.chatServer is null)
            {
                return false;
            }

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

            if (!this.GameServers.TryGetValue(friend.ServerId, out var gameServerOfFriend))
            {
                return false;
            }

            var authenticationInfoFriend = this.chatServer.RegisterClient(roomId, friendName);
            if (authenticationInfoFriend is not null)
            {
                gameServerOfFriend.ChatRoomCreated(authenticationInfoFriend, playerName);
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        /// <remarks>Note, that the ServerId is not filled by this implementation. The player will receive it separately when the subscription is created.</remarks>
        public IEnumerable<string> GetFriendList(Guid characterId)
        {
            using var context = this.persistenceContextProvider.CreateNewFriendServerContext();
            return context.GetFriendNames(characterId);
        }

        /// <inheritdoc/>
        public void SetOnlineState(Guid characterId, string characterName, int serverId)
        {
            if (!this.OnlineFriends.TryGetValue(characterName, out var observer))
            {
                if (!this.GameServers.TryGetValue(serverId, out var gameServer))
                {
                    return;
                }

                observer = new OnlineFriend(gameServer, characterName)
                {
                    ServerId = (byte)gameServer.Id,
                };
                this.OnlineFriends.Add(characterName, observer);

                using var context = this.persistenceContextProvider.CreateNewFriendServerContext();
                var friends = context.GetFriends(characterId);
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

            observer.ChangeServer(serverId == InvisibleServerId ? OfflineServerId : serverId);

            if (serverId == OfflineServerId)
            {
                this.OnlineFriends.Remove(observer.PlayerName);
                observer.OnCompleted();
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetOpenFriendRequests(Guid characterId)
        {
            using var context = this.persistenceContextProvider.CreateNewFriendServerContext();
            return context.GetOpenFriendRequesterNames(characterId);
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
}
