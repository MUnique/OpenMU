// <copyright file="PartyManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Views.Party;

/// <summary>
/// Manages party creation and tracks character-to-party membership for reconnection.
/// </summary>
public sealed class PartyManager : IPartyManager
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, Party> _partyByCharacterName = new();
    private readonly ILogger<Party> _logger;
    private readonly byte _maxPartySize;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartyManager"/> class.
    /// </summary>
    /// <param name="maxPartySize">The maximum number of members per party.</param>
    /// <param name="partyLogger">The logger for party instances.</param>
    public PartyManager(byte maxPartySize, ILogger<Party> partyLogger)
    {
        this._maxPartySize = maxPartySize;
        this._logger = partyLogger;
    }

    /// <inheritdoc />
    public Party CreateParty()
    {
        return new Party(this, this._maxPartySize, this._logger);
    }

    /// <inheritdoc />
    public void TrackPartyMembership(string characterName, Party party)
    {
        this._partyByCharacterName[characterName] = party;
    }

    /// <inheritdoc />
    public void RemovePartyMembership(string characterName)
    {
        this._partyByCharacterName.TryRemove(characterName, out _);
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
        if (!this._partyByCharacterName.TryGetValue(member.Name, out var party))
        {
            this._logger.LogDebug("Party not found for {CharacterName}", member.Name);
            return;
        }

        var offlineMember = party.PartyList.FirstOrDefault(m =>
            m.Name == member.Name && !m.IsConnected);

        if (offlineMember is null)
        {
            this._logger.LogDebug("Ignoring replace party member for online character {CharacterName}", member.Name);
            return;
        }

        await party.ReplaceMemberAsync(offlineMember, member).ConfigureAwait(false);
        await member.InvokeViewPlugInAsync<IUpdatePartyListPlugIn>(p => p.UpdatePartyListAsync()).ConfigureAwait(false);
        await member.InvokeViewPlugInAsync<IPartyHealthViewPlugIn>(p => p.UpdatePartyHealthAsync()).ConfigureAwait(false);
    }
}
