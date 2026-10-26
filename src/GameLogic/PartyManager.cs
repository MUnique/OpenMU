// <copyright file="PartyManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Views.Party;

/// <summary>
/// Manages party creation and tracks character-to-party membership for member reconnection.
/// </summary>
public sealed class PartyManager : IPartyManager
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, Party> _partyByCharacterName = new(StringComparer.Ordinal);
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
    public Party CreateParty() => new(this, this._maxPartySize, this._partyLogger);

    /// <inheritdoc />
    public async ValueTask OnMemberReconnectedAsync(IPartyMember member)
    {
        var characterId = (member as Player)?.SelectedCharacter?.Id;
        Party? party = null;
        IPartyMember? snapshot = null;

        // Find the offline snapshot that was created when the member disconnected.
        if (this._partyByCharacterName.TryGetValue(member.Name, out var partyByName))
        {
            party = partyByName;
            snapshot = party.PartyList.FirstOrDefault(m => m.Name == member.Name && !m.IsConnected && IsSameCharacter(m, characterId));
        }

        // The character may have been renamed while it was offline (e.g. by the admin panel):
        // then the snapshot still carries the old name, so look for it by the character id.
        if (snapshot is null && characterId is { } id && id != Guid.Empty)
        {
            foreach (var candidate in this._partyByCharacterName.Values.Distinct())
            {
                snapshot = candidate.PartyList.FirstOrDefault(m => !m.IsConnected && m is OfflinePartyMember offline && offline.CharacterId == id);
                if (snapshot is not null)
                {
                    party = candidate;
                    break;
                }
            }
        }

        if (party is null || snapshot is null)
        {
            // Already replaced (e.g., duplicate reconnect event) or no party - nothing to do.
            return;
        }

        await party.ReplaceMemberAsync(snapshot, member).ConfigureAwait(false);

        // Send the full party state to the rejoined player since they missed updates while offline.
        await member.InvokeViewPlugInAsync<IUpdatePartyListPlugIn>(p => p.UpdatePartyListAsync()).ConfigureAwait(false);
        await member.InvokeViewPlugInAsync<IPartyHealthViewPlugIn>(p => p.UpdatePartyHealthAsync()).ConfigureAwait(false);
    }

    /// <inheritdoc />
    void IPartyManager.TrackMembership(string characterName, Party party)
        => this._partyByCharacterName[characterName] = party;

    /// <inheritdoc />
    void IPartyManager.UntrackMembership(string characterName)
        => this._partyByCharacterName.TryRemove(characterName, out _);

    /// <summary>
    /// A snapshot with a known character id only belongs to that character, so another character
    /// which got the old name later does not take the reserved slot.
    /// </summary>
    private static bool IsSameCharacter(IPartyMember member, Guid? characterId)
        => member is not OfflinePartyMember offline
           || characterId is null
           || offline.CharacterId == Guid.Empty
           || offline.CharacterId == characterId;
}