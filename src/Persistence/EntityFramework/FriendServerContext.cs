// <copyright file="FriendServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// A context which is used by the <see cref="IFriendServer"/>.
/// </summary>
internal class FriendServerContext : CachingEntityFrameworkContext, IFriendServerContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FriendServerContext" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="repositoryProvider">The repositoryManager.</param>
    /// <param name="logger">The logger.</param>
    public FriendServerContext(FriendContext context, IContextAwareRepositoryProvider repositoryProvider, ILogger<FriendServerContext> logger)
        : base(context, repositoryProvider, null, logger)
    {
    }

    /// <inheritdoc/>
    public async ValueTask<Interfaces.Friend> CreateNewFriendAsync(string characterName, string friendName)
    {
        var item = this.CreateNew<Model.Friend>();
        item.CharacterId = await this.GetCharacterIdByNameAsync(characterName).ConfigureAwait(false) ?? Guid.Empty;
        item.FriendId = await this.GetCharacterIdByNameAsync(friendName).ConfigureAwait(false) ?? Guid.Empty;

        return item;
    }

    /// <inheritdoc/>
    public async ValueTask DeleteAsync(string characterName, string friendName)
    {
        this.Context.RemoveRange(await this.FindItems(characterName, friendName).ToListAsync().ConfigureAwait(false));
    }

    /// <inheritdoc/>
    public async ValueTask<Interfaces.Friend?> GetFriendByNamesAsync(string characterName, string friendName)
    {
        return await this.FindItems(characterName, friendName).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<FriendViewItem>> GetFriendsAsync(Guid characterId)
    {
        return await (from friend in this.Context.Set<Model.Friend>()
            join friendCharacter in this.Context.Set<CharacterName>() on friend.FriendId equals friendCharacter.Id
            join character in this.Context.Set<CharacterName>() on friend.CharacterId equals character.Id
            select new FriendViewItem(character.Name, friendCharacter.Name)
            {
                Id = friend.Id,
                CharacterId = friend.CharacterId,
                FriendId = friend.FriendId,
                Accepted = friend.Accepted,
                RequestOpen = friend.RequestOpen,
            }).ToListAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<IEnumerable<string>> GetFriendNamesAsync(Guid characterId)
    {
        return await (from friend in this.Context.Set<Model.Friend>()
            where friend.CharacterId == characterId
            join friendCharacter in this.Context.Set<CharacterName>() on friend.FriendId equals friendCharacter.Id
            select friendCharacter.Name).ToListAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<string>> GetOpenFriendRequesterNamesAsync(Guid characterId)
    {
        return await (from friend in this.Context.Set<Model.Friend>()
            where friend.RequestOpen == true && friend.FriendId == characterId
            join requester in this.Context.Set<CharacterName>() on friend.CharacterId equals requester.Id
            select requester.Name).ToListAsync().ConfigureAwait(false);
    }

    private IQueryable<Model.Friend> FindItems(string characterName, string friendName)
    {
        return from friend in this.Context.Set<Model.Friend>()
            join friendCharacter in this.Context.Set<CharacterName>() on friend.FriendId equals friendCharacter.Id
            join character in this.Context.Set<CharacterName>() on friend.CharacterId equals character.Id
            where friendCharacter.Name == friendName && character.Name == characterName
            select friend;
    }

    private async ValueTask<Guid?> GetCharacterIdByNameAsync(string name) => await this.Context.Set<CharacterName>().Where(character => character.Name == name).Select(character => character.Id).FirstOrDefaultAsync().ConfigureAwait(false);
}