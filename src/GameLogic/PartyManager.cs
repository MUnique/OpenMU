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
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, Party> _partyByCharacterName = new(StringComparer.OrdinalIgnoreCase);
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
        if (!this._partyByCharacterName.TryGetValue(member.Name, out var party))
        {
            return;
        }

        // Find the offline snapshot that was created when the member disconnected.
        var snapshot = party.PartyList.FirstOrDefault(m => m.Name == member.Name && !m.IsConnected);
        if (snapshot is null)
        {
            // Already replaced (e.g., duplicate reconnect event) — nothing to do.
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
}