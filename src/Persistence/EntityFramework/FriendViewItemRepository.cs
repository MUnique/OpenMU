// <copyright file="FriendViewItemRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Context for instances of <see cref="FriendViewItem"/>.
    /// </summary>
    /// <remarks>
    /// This class makes use of custom sql statements, because we want to load the character names from the character table and not duplicate it in the <see cref="FriendViewItem"/> table.
    /// </remarks>
    internal class FriendViewItemRepository : IFriendViewItemRepository<FriendViewItem>
    {
        private readonly IRepositoryManager repositoryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendViewItemRepository"/> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        public FriendViewItemRepository(IRepositoryManager repositoryManager)
        {
            this.repositoryManager = repositoryManager;
        }

        /// <inheritdoc/>
        public void Delete(string characterName, string friendName)
        {
            var context = this.GetCurrentEntityFrameworkContext();
            var items = context.Context.Set<FriendViewItem>().Where(item => item.FriendName == friendName && item.CharacterName == characterName);
            context.Context.RemoveRange(items);
        }

        /// <inheritdoc/>
        public IEnumerable<FriendViewItem> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FriendViewItem GetByFriend(string characterName, string friendName)
        {
            var context = this.GetCurrentEntityFrameworkContext();
            return context.Context.Set<FriendViewItem>().FirstOrDefault(item => item.FriendName == friendName && item.CharacterName == characterName);
        }

        /// <inheritdoc/>
        public FriendViewItem GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<FriendViewItem> GetFriends(Guid characterId)
        {
            var context = this.GetCurrentEntityFrameworkContext();
            return context.Context.Set<FriendViewItem>().Where(item => item.CharacterId == characterId).AsNoTracking().ToList();
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetOpenFriendRequesterNames(Guid characterId)
        {
            var context = this.GetCurrentEntityFrameworkContext();
            return context.Context.Set<FriendViewItem>().Where(item => item.CharacterId == characterId).AsNoTracking().Select(friend => friend.FriendName).ToList();
        }

        /// <inheritdoc/>
        public FriendViewItem CreateNewFriendViewItem(string characterName, string friendName)
        {
            var item = this.repositoryManager.CreateNew<FriendViewItem>();
            using (var context = new EntityDataContext())
            {
                item.CharacterId = context.Set<Character>().Where(character => character.Name == characterName).Select(character => character.Id).FirstOrDefault();
                item.FriendId = context.Set<Character>().Where(character => character.Name == friendName).Select(character => character.Id).FirstOrDefault();
            }

            // The names are available after commit and are not allowed to be set before.
            return item;
        }

        /// <inheritdoc/>
        object IRepository.GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool Delete(object obj)
        {
            throw new NotImplementedException();
        }

        private EntityFrameworkContext GetCurrentEntityFrameworkContext()
        {
            var context = this.repositoryManager.GetCurrentContext() as EntityFrameworkContext;
            if (context == null)
            {
                throw new InvalidOperationException("There is no context in current use.");
            }

            return context;
        }
    }
}
