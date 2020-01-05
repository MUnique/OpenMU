// <copyright file="FriendServerInMemoryContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence.BasicModel;
    using Friend = MUnique.OpenMU.Interfaces.Friend;

    /// <summary>
    /// In-memory context implementation for <see cref="IFriendServerContext"/>.
    /// </summary>
    public class FriendServerInMemoryContext : InMemoryContext, IFriendServerContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FriendServerInMemoryContext"/> class.
        /// </summary>
        /// <param name="manager">The manager which holds the memory repositories.</param>
        public FriendServerInMemoryContext(InMemoryRepositoryManager manager)
            : base(manager)
        {
        }

        /// <inheritdoc/>
        public Friend CreateNewFriend(string characterName, string friendName)
        {
            var friend = this.CreateNew<MUnique.OpenMU.Interfaces.Friend>();
            friend.FriendId = this.Manager.GetRepository<Character>().GetAll().FirstOrDefault(character => character.Name == friendName)?.Id ?? Guid.Empty;
            friend.CharacterId = this.Manager.GetRepository<Character>().GetAll().FirstOrDefault(character => character.Name == characterName)?.Id ?? Guid.Empty;
            return friend;
        }

        /// <inheritdoc/>
        public Friend GetFriendByNames(string characterName, string friendName)
        {
            var friendId = this.Manager.GetRepository<Character>().GetAll().FirstOrDefault(character => character.Name == friendName)?.Id;
            var characterId = this.Manager.GetRepository<Character>().GetAll().FirstOrDefault(character => character.Name == characterName)?.Id;

            return this.Manager.GetRepository<Friend>().GetAll().FirstOrDefault(f => f.FriendId == friendId && f.CharacterId == characterId);
        }

        /// <inheritdoc/>
        public IEnumerable<FriendViewItem> GetFriends(Guid characterId)
        {
            var characters = this.Manager.GetRepository<Character>().GetAll();
            return this.Manager.GetRepository<Friend>().GetAll().Where(f => f.CharacterId == characterId)
                .Select(f => new FriendViewItem
                    {
                        Accepted = f.Accepted,
                        CharacterId = f.CharacterId,
                        CharacterName = characters.FirstOrDefault(c => c.Id == f.CharacterId)?.Name,
                        FriendId = f.FriendId,
                        FriendName = characters.FirstOrDefault(c => c.Id == f.FriendId)?.Name,
                        Id = f.Id,
                        RequestOpen = f.RequestOpen,
                    });
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFriendNames(Guid characterId)
        {
            var characters = this.Manager.GetRepository<Character>().GetAll();
            return this.Manager.GetRepository<Friend>().GetAll().Where(f => f.CharacterId == characterId)
                .Select(f => characters.FirstOrDefault(c => c.Id == f.FriendId)?.Name);
        }

        /// <inheritdoc/>
        public void Delete(string characterName, string friendName)
        {
            var item = this.GetFriendByNames(characterName, friendName);
            this.Delete(item);
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetOpenFriendRequesterNames(Guid characterId)
        {
            var characters = this.Manager.GetRepository<Character>().GetAll();
            return this.Manager.GetRepository<Friend>().GetAll().Where(f => f.FriendId == characterId && f.RequestOpen)
                .Select(f => characters.FirstOrDefault(c => c.Id == f.CharacterId)?.Name);
        }
    }
}