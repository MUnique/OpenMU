// <copyright file="FriendServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.FriendServer
{
    using System;
    using System.Collections.Generic;

    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// The friend server which manages the friend list with chat and letter system.
    /// </summary>
    public class FriendServer : IFriendServer
    {
        private readonly IRepositoryManager repositoryManager;

        private readonly IChatServer chatServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendServer" /> class.
        /// </summary>
        /// <param name="gameServers">The game servers.</param>
        /// <param name="chatServer">The chat server.</param>
        /// <param name="repositoryManager">The repository manager.</param>
        public FriendServer(IDictionary<int, IGameServer> gameServers, IChatServer chatServer, IRepositoryManager repositoryManager)
        {
            this.chatServer = chatServer;
            this.GameServers = gameServers;
            this.repositoryManager = repositoryManager;
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
            var friendRepository = this.repositoryManager.GetRepository<FriendViewItem, IFriendViewItemRepository<FriendViewItem>>();
            var friend = friendRepository.GetByFriend(playerName, friendName);
            var friendIsNew = friend == null;
            var saveSuccess = true;
            if (friendIsNew)
            {
                using (var context = this.repositoryManager.UseTemporaryContext())
                {
                    friend = this.GetNewFriendViewItem(playerName, friendName);
                    friend.Accepted = false;
                    friend.RequestOpen = true;
                    saveSuccess = context.SaveChanges();
                }
            }

            if (saveSuccess)
            {
                if (this.OnlineFriends.TryGetValue(friendName, out var onlineFriend))
                {
                    // Friend is online, so we directly send him a request.
                    onlineFriend.GameServer.FriendRequest(friend.CharacterName, friend.FriendName);
                }
            }

            return friendIsNew && saveSuccess;
        }

        /// <inheritdoc/>
        public void DeleteFriend(string playerName, string friendName)
        {
            if (this.OnlineFriends.TryGetValue(playerName, out var player))
            {
                if (this.OnlineFriends.TryGetValue(friendName, out var friend))
                {
                    player.RemoveSubscriber(friend);
                }
            }

            var friendRepository = this.repositoryManager.GetRepository<FriendViewItem, IFriendViewItemRepository<FriendViewItem>>();
            friendRepository.Delete(playerName, friendName);
        }

        /// <inheritdoc/>
        public void FriendResponse(string characterName, string friendName, bool accepted)
        {
            using (var context = this.repositoryManager.UseTemporaryContext())
            {
                var friendRepository = this.repositoryManager.GetRepository<FriendViewItem, IFriendViewItemRepository<FriendViewItem>>();
                #pragma warning disable S2234 // They parameters are passed correctly
                var requester = friendRepository.GetByFriend(friendName, characterName);
                #pragma warning restore S2234
                if (requester == null)
                {
                    return;
                }

                requester.RequestOpen = false;
                requester.Accepted = accepted;

                if (accepted)
                {
                    var responder = friendRepository.GetByFriend(characterName, friendName) ?? this.GetNewFriendViewItem(characterName, friendName);
                    responder.RequestOpen = false;
                    responder.Accepted = true;
                    this.AddSubscriptions(requester, responder);
                }

                context.SaveChanges();
            }
        }

        /// <inheritdoc/>
        public ChatServerAuthenticationInfo CreateChatRoom(string playerName, string friendName)
        {
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

        /// <inheritdoc/>
        /// <remarks>Note, that the ServerId is not filled by this implementation. The player will receive it separately when the subscription is created.</remarks>
        public IEnumerable<FriendViewItem> GetFriendList(Guid characterId)
        {
            var friendRepository = this.repositoryManager.GetRepository<FriendViewItem, IFriendViewItemRepository<FriendViewItem>>();
            return friendRepository.GetFriends(characterId);
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

                observer = new OnlineFriend(gameServer)
                {
                    PlayerName = characterName,
                    ServerId = (byte)gameServer.Id
                };
                this.OnlineFriends.Add(characterName, observer);
                var friendRepository = this.repositoryManager.GetRepository<FriendViewItem, IFriendViewItemRepository<FriendViewItem>>();
                var friendlist = friendRepository.GetFriends(characterId);
                foreach (var friend in friendlist)
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
        public string GetChatserverIP()
        {
            return this.chatServer.GetIPAddress();
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetOpenFriendRequests(Guid characterId)
        {
            var friendRepository = this.repositoryManager.GetRepository<FriendViewItem, IFriendViewItemRepository<FriendViewItem>>();
            return friendRepository.GetOpenFriendRequesterNames(characterId);
        }

        private void AddSubscriptions(FriendViewItem requester, FriendViewItem responder)
        {
            if (!this.OnlineFriends.TryGetValue(responder.CharacterName, out var responderFriend))
            {
                return;
            }

            if (this.OnlineFriends.TryGetValue(requester.CharacterName, out var requesterFriend))
            {
                responderFriend.AddSubscription(requesterFriend.Subscribe(responderFriend));
                requesterFriend.AddSubscription(responderFriend.Subscribe(requesterFriend));

                // inform both about their online server ids:
                responderFriend.OnNext(requesterFriend);
                requesterFriend.OnNext(responderFriend);
            }
        }

        private FriendViewItem GetNewFriendViewItem(string characterName, string friendName)
        {
            var item = this.repositoryManager.CreateNew<FriendViewItem>();
            item.CharacterName = characterName;
            item.FriendName = friendName;
            return item;
        }
    }
}
