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
        /// <inheritdoc/>
        public void Delete(string characterName, string friendName)
        {
            using (var context = new FriendContext())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM \"FriendViewItem\" WHERE \"FriendName\" = :p0 AND \"CharacterName\" = :p1", friendName, characterName);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FriendViewItem> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FriendViewItem GetByFriend(string characterName, string friendName)
        {
            using (var context = new FriendContext())
            {
                return context.Set<FriendViewItem>().FromSql(
                    "SELECT V.\"Id\", V.\"CharacterId\", V.\"FriendId\", V.\"Accepted\", V.\"RequestOpen\", \"Character\".\"Name\" AS \"CharacterName\", Friend.\"Name\" AS \"FriendName\" FROM data.\"FriendViewItem\" V, data.\"Character\", data.\"Character\" Friend " +
                    "  WHERE V.\"CharacterId\" = \"Character\".\"Id\" " +
                    "  AND V.\"FriendId\" = Friend.\"Id\" " +
                    "  AND \"Character\".\"Name\" = :p0 " +
                    "  AND Friend.\"Name\" = :p1",
                    characterName,
                    friendName)
                    .FirstOrDefault();
            }
        }

        /// <inheritdoc/>
        public FriendViewItem GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<FriendViewItem> GetFriends(Guid characterId)
        {
            using (var context = new FriendContext())
            {
                return context.Set<FriendViewItem>().FromSql(
                    "SELECT V.\"Id\", V.\"CharacterId\", V.\"FriendId\", V.\"Accepted\", V.\"RequestOpen\", \"Character\".\"Name\" AS \"CharacterName\", Friend.\"Name\" AS \"FriendName\" FROM data.\"FriendViewItem\" V, data.\"Character\", data.\"Character\" Friend " +
                    "  WHERE V.\"CharacterId\" = \"Character\".\"Id\" " +
                    "  AND V.\"FriendId\" = Friend.\"Id\" " +
                    "  AND \"Character\".\"Id\" = :p0 " +
                    "  AND Friend.\"Name\" = :p1",
                    characterId).AsNoTracking().ToList();
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetOpenFriendRequesterNames(Guid characterId)
        {
            using (var context = new FriendContext())
            {
                return context.Set<FriendViewItem>().FromSql(
                    "SELECT V.\"Id\", V.\"CharacterId\", V.\"FriendId\", V.\"Accepted\", V.\"RequestOpen\", \"Character\".\"Name\" AS \"CharacterName\", Friend.\"Name\" AS \"FriendName\" FROM \"FriendViewItem\" V, \"Character\", \"Character\" Friend " +
                    "  WHERE V.\"CharacterId\" = \"Character\".\"Id\" " +
                    "  AND V.\"FriendId\" = Friend.\"Id\" " +
                    "  AND V.\"RequestOpen\" = :p1 " +
                    "  AND V.\"CharacterId\" = :p0 ",
                    characterId,
                    true).AsNoTracking().Select(friend => friend.FriendName).ToList();
            }
        }

        /// <inheritdoc/>
        public FriendViewItem CreateNewFriendViewItem(string characterName, string friendName)
        {
            var item = new FriendViewItem
            {
                CharacterName = characterName,
                FriendName = friendName
            };

            using (var context = new EntityDataContext())
            {
                item.CharacterId = context.Set<Character>().Where(character => character.Name == characterName).Select(character => character.Id).FirstOrDefault();
                item.FriendId = context.Set<Character>().Where(character => character.Name == friendName).Select(character => character.Id).FirstOrDefault();
            }

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
    }
}
