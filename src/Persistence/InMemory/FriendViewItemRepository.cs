// <copyright file="FriendViewItemRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence.EntityFramework;

    /// <summary>
    /// In-memory repository for <see cref="MUnique.OpenMU.Interfaces.FriendViewItem"/>s.
    /// </summary>
    public class FriendViewItemRepository : MemoryRepository<FriendViewItem>, IFriendViewItemRepository<FriendViewItem>
    {
        /// <summary>
        /// The characters enumeration of all characters of all accounts. Used to retrieve character identifiers.
        /// </summary>
        private readonly IEnumerable<Character> characters;

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendViewItemRepository"/> class.
        /// </summary>
        /// <param name="accountRepository">The account repository.</param>
        public FriendViewItemRepository(IRepository<Account> accountRepository)
        {
            this.characters = accountRepository.GetAll().SelectMany(a => a.Characters);
        }

        /// <inheritdoc/>
        public FriendViewItem CreateNewFriendViewItem(string characterName, string friendName)
        {
            var item = TypeHelper.CreateNew<FriendViewItem>();
            item.Id = Guid.NewGuid();
            item.CharacterName = characterName;
            item.FriendName = friendName;
            item.CharacterId = this.characters.FirstOrDefault(character => character.Name == characterName)?.Id ?? Guid.Empty;
            item.FriendId = this.characters.FirstOrDefault(character => character.Name == friendName)?.Id ?? Guid.Empty;
            this.Add(item.Id, item);
            return item;
        }

        /// <inheritdoc/>
        public void Delete(string characterName, string friendName)
        {
            var item = this.GetByFriend(characterName, friendName);
            if (item != null)
            {
                this.Delete(item);
            }
        }

        /// <inheritdoc/>
        public FriendViewItem GetByFriend(string characterName, string friendName)
        {
            return this.GetAll().FirstOrDefault(f => f.CharacterName == characterName && f.FriendName == friendName);
        }

        /// <inheritdoc/>
        public IEnumerable<FriendViewItem> GetFriends(Guid characterId)
        {
            return this.GetAll().Where(f => f.CharacterId == characterId);
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetOpenFriendRequesterNames(Guid characterId)
        {
            return this.GetAll().Where(f => f.CharacterId == characterId && f.RequestOpen).Select(f => f.FriendName);
        }
    }
}