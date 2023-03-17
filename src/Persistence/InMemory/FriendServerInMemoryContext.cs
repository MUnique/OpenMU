// <copyright file="FriendServerInMemoryContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

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
    /// <param name="provider">The manager which holds the memory repositories.</param>
    public FriendServerInMemoryContext(InMemoryRepositoryProvider provider)
        : base(provider)
    {
    }

    /// <inheritdoc/>
    public async ValueTask<Friend> CreateNewFriendAsync(string characterName, string friendName)
    {
        var friend = this.CreateNew<MUnique.OpenMU.Interfaces.Friend>();
        friend.FriendId = (await this.Provider.GetRepository<Character>().GetAllAsync().ConfigureAwait(false)).FirstOrDefault(character => character.Name == friendName)?.Id ?? Guid.Empty;
        friend.CharacterId = (await this.Provider.GetRepository<Character>().GetAllAsync().ConfigureAwait(false)).FirstOrDefault(character => character.Name == characterName)?.Id ?? Guid.Empty;
        return friend;
    }

    /// <inheritdoc/>
    public async ValueTask<Friend?> GetFriendByNamesAsync(string characterName, string friendName)
    {
        var friendId = (await this.Provider.GetRepository<Character>().GetAllAsync().ConfigureAwait(false)).FirstOrDefault(character => character.Name == friendName)?.Id;
        var characterId = (await this.Provider.GetRepository<Character>().GetAllAsync().ConfigureAwait(false)).FirstOrDefault(character => character.Name == characterName)?.Id;

        return (await this.Provider.GetRepository<Friend>().GetAllAsync().ConfigureAwait(false)).FirstOrDefault(f => f.FriendId == friendId && f.CharacterId == characterId);
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<FriendViewItem>> GetFriendsAsync(Guid characterId)
    {
        var characters = await this.Provider.GetRepository<Character>().GetAllAsync().ConfigureAwait(false);
        return (await this.Provider.GetRepository<Friend>().GetAllAsync().ConfigureAwait(false)).Where(f => f.CharacterId == characterId)
            .Select(f => (Friend: f, CharacterName: characters.FirstOrDefault(c => c.Id == f.CharacterId)?.Name, FriendName: characters.FirstOrDefault(c => c.Id == f.FriendId)?.Name))
            .Where(f => f.CharacterName is not null && f.FriendName is not null)
            .Select(f => new FriendViewItem(f.CharacterName!, f.FriendName!)
            {
                Accepted = f!.Friend.Accepted,
                CharacterId = f.Friend.CharacterId,
                FriendId = f.Friend.FriendId,
                Id = f.Friend.Id,
                RequestOpen = f.Friend.RequestOpen,
            });
    }

    /// <inheritdoc />
    public async ValueTask<IEnumerable<string>> GetFriendNamesAsync(Guid characterId)
    {
        var characters = await this.Provider.GetRepository<Character>().GetAllAsync().ConfigureAwait(false);
        return (await this.Provider.GetRepository<Friend>().GetAllAsync().ConfigureAwait(false)).Where(f => f.CharacterId == characterId)
            .Select(f => characters.FirstOrDefault(c => c.Id == f.FriendId)?.Name!)
            .Where(name => name is not null);
    }

    /// <inheritdoc/>
    public async ValueTask DeleteAsync(string characterName, string friendName)
    {
        if (await this.GetFriendByNamesAsync(characterName, friendName).ConfigureAwait(false) is { } item)
        {
            await this.DeleteAsync(item).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<string>> GetOpenFriendRequesterNamesAsync(Guid characterId)
    {
        var characters = await this.Provider.GetRepository<Character>().GetAllAsync().ConfigureAwait(false);
        return (await this.Provider.GetRepository<Friend>().GetAllAsync().ConfigureAwait(false)).Where(f => f.FriendId == characterId && f.RequestOpen)
            .Select(f => characters.FirstOrDefault(c => c.Id == f.CharacterId)?.Name!)
            .Where(name => name is not null);
    }
}