// <copyright file="PartyManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using Microsoft.Extensions.Caching.Memory;

/// <summary>
/// Default implementation of <see cref="IPartyManager"/> using <see cref="IMemoryCache"/> for persistence.
/// </summary>
public sealed class PartyManager : IPartyManager, IDisposable
{
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly ILogger<Party> _partyLogger;
    private readonly byte _maxPartySize;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartyManager"/> class.
    /// </summary>
    /// <param name="maxPartySize">The maximum number of members per party.</param>
    /// <param name="partyLogger">The logger for party instances.</param>
    public PartyManager(byte maxPartySize, ILogger<Party> partyLogger)
    {
        this._maxPartySize = maxPartySize;
        this._partyLogger = partyLogger;
    }

    /// <inheritdoc />
    public Party CreateParty()
    {
        return new Party(this, this._maxPartySize, this._partyLogger);
    }

    /// <inheritdoc />
    public void SaveParty(Guid characterId, Party party)
    {
        this._cache.Set(characterId, party, TimeSpan.FromHours(24));
    }

    /// <inheritdoc />
    public void RemoveParty(Guid characterId)
    {
        this._cache.Remove(characterId);
    }

    /// <inheritdoc />
    public async ValueTask OnMemberDisconnectedAsync(IPartyMember member)
    {
        if (member.Party is not { } party)
        {
            return;
        }

        var snapshot = new OfflinePartyMember(member);
        await party.ReplaceMemberAsync(member, snapshot).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask OnMemberReconnectedAsync(IPartyMember member)
    {
        if (!this._cache.TryGetValue(member.CharacterId, out Party? party) || party is null)
        {
            return;
        }

        var offlineMember = party.PartyList.FirstOrDefault(m => m.CharacterId == member.CharacterId && !m.IsConnected);
        if (offlineMember is null)
        {
            return;
        }

        await party.ReplaceMemberAsync(offlineMember, member).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._cache.Dispose();
    }

}