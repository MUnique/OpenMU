// <copyright file="FriendServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// A context which is used by the <see cref="IFriendServer"/>.
    /// </summary>
    internal class FriendServerContext : CachingEntityFrameworkContext, IFriendServerContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FriendServerContext"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="repositoryManager">The repositoryManager.</param>
        public FriendServerContext(FriendContext context, RepositoryManager repositoryManager)
            : base(context, repositoryManager)
        {
        }

        /// <inheritdoc/>
        public Interfaces.Friend CreateNewFriend(string characterName, string friendName)
        {
            var item = this.CreateNew<Friend>();
            item.CharacterId = this.GetCharacterIdByName(characterName);
            item.FriendId = this.GetCharacterIdByName(friendName);

            return item;
        }

        /// <inheritdoc/>
        public void Delete(string characterName, string friendName)
        {
            this.Context.RemoveRange(this.FindItems(characterName, friendName));
        }

        /// <inheritdoc/>
        public Interfaces.Friend GetFriendByNames(string characterName, string friendName)
        {
            return this.FindItems(characterName, friendName).FirstOrDefault();
        }

        /// <inheritdoc/>
        public IEnumerable<FriendViewItem> GetFriends(Guid characterId)
        {
            return from friend in this.Context.Set<Friend>()
                join friendCharacter in this.Context.Set<CharacterName>() on friend.FriendId equals friendCharacter.Id
                join character in this.Context.Set<CharacterName>() on friend.CharacterId equals character.Id
                select new FriendViewItem
                {
                    Id = friend.Id,
                    CharacterId = friend.CharacterId,
                    FriendId = friend.FriendId,
                    Accepted = friend.Accepted,
                    RequestOpen = friend.RequestOpen,
                    CharacterName = character.Name,
                    FriendName = friendCharacter.Name,
                };
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFriendNames(Guid characterId)
        {
            return (from friend in this.Context.Set<Friend>()
                where friend.CharacterId == characterId
                join friendCharacter in this.Context.Set<CharacterName>() on friend.FriendId equals friendCharacter.Id
                select friendCharacter.Name).ToList();
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetOpenFriendRequesterNames(Guid characterId)
        {
            return (from friend in this.Context.Set<Friend>()
                where friend.RequestOpen == true && friend.FriendId == characterId
                join requester in this.Context.Set<CharacterName>() on friend.CharacterId equals requester.Id
                select requester.Name).ToList();
        }

        private IQueryable<Friend> FindItems(string characterName, string friendName)
        {
            return from friend in this.Context.Set<Friend>()
                join friendCharacter in this.Context.Set<CharacterName>() on friend.FriendId equals friendCharacter.Id
                join character in this.Context.Set<CharacterName>() on friend.CharacterId equals character.Id
                where friendCharacter.Name == friendName && character.Name == characterName
                select friend;
        }

        private Guid GetCharacterIdByName(string name) => this.Context.Set<CharacterName>().Where(character => character.Name == name).Select(character => character.Id).FirstOrDefault();
    }
}
