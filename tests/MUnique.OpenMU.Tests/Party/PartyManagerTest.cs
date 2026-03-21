// <copyright file="PartyManagerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for <see cref="PartyManager"/>.
/// </summary>
[TestFixture]
public class PartyManagerTest
{
    private const byte MaxPartySize = 5;

    private PartyManager _partyManager = null!;

    /// <summary>
    /// Sets up a fresh <see cref="PartyManager"/> before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._partyManager = new PartyManager(MaxPartySize, new NullLogger<Party>());
    }

    /// <summary>
    /// Tests that <see cref="PartyManager.CreateParty"/> returns a party with the configured max size.
    /// </summary>
    [Test]
    public void CreateParty_RespectsMaxPartySize()
    {
        var party = this._partyManager.CreateParty();
        Assert.That(party.MaxPartySize, Is.EqualTo(MaxPartySize));
    }

    /// <summary>
    /// Tests that a member added to the party is present in the party list.
    /// </summary>
    [Test]
    public async ValueTask CreateParty_AddedMemberIsInPartyList()
    {
        var member = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        var party = this._partyManager.CreateParty();

        await party.AddAsync(member).ConfigureAwait(false);

        Assert.That(party.PartyList, Contains.Item(member));
    }

    /// <summary>
    /// Tests that <see cref="PartyManager.OnMemberDisconnectedAsync"/> replaces the live member
    /// with an <see cref="OfflinePartyMember"/> snapshot at the same index.
    /// </summary>
    [Test]
    public async ValueTask OnMemberDisconnected_ReplacesWithOfflineSnapshot()
    {
        var (party, member1, _) = await this.CreatePartyWithTwoMembersAsync().ConfigureAwait(false);

        await this._partyManager.OnMemberDisconnectedAsync(member1).ConfigureAwait(false);

        Assert.That(party.PartyList[0], Is.InstanceOf<OfflinePartyMember>());
        Assert.That(party.PartyList[0].CharacterId, Is.EqualTo(member1.CharacterId));
    }

    /// <summary>
    /// Tests that the offline snapshot preserves the disconnected member's name.
    /// </summary>
    [Test]
    public async ValueTask OnMemberDisconnected_SnapshotPreservesName()
    {
        var (party, member1, _) = await this.CreatePartyWithTwoMembersAsync().ConfigureAwait(false);

        await this._partyManager.OnMemberDisconnectedAsync(member1).ConfigureAwait(false);

        Assert.That(party.PartyList[0].Name, Is.EqualTo(member1.Name));
    }

    /// <summary>
    /// Tests that <see cref="PartyManager.OnMemberReconnectedAsync"/> swaps the offline snapshot
    /// back to the live member.
    /// </summary>
    [Test]
    public async ValueTask OnMemberReconnected_RestoresLiveMember()
    {
        var (party, member1, _) = await this.CreatePartyWithTwoMembersAsync().ConfigureAwait(false);

        await this._partyManager.OnMemberDisconnectedAsync(member1).ConfigureAwait(false);
        await this._partyManager.OnMemberReconnectedAsync(member1).ConfigureAwait(false);

        Assert.That(party.PartyList[0], Is.SameAs(member1));
    }

    /// <summary>
    /// Tests that after reconnect the live member's Party reference is restored.
    /// </summary>
    [Test]
    public async ValueTask OnMemberReconnected_RestoresPartyReference()
    {
        var (party, member1, _) = await this.CreatePartyWithTwoMembersAsync().ConfigureAwait(false);

        await this._partyManager.OnMemberDisconnectedAsync(member1).ConfigureAwait(false);
        await this._partyManager.OnMemberReconnectedAsync(member1).ConfigureAwait(false);

        Assert.That(member1.Party, Is.SameAs(party));
    }

    /// <summary>
    /// Tests that <see cref="PartyManager.OnMemberReconnectedAsync"/> does nothing
    /// when the member has no cached party.
    /// </summary>
    [Test]
    public async ValueTask OnMemberReconnected_DoesNothingWhenNoCachedParty()
    {
        var member = await this.CreatePartyMemberAsync().ConfigureAwait(false);

        await this._partyManager.OnMemberReconnectedAsync(member).ConfigureAwait(false);

        Assert.That(member.Party, Is.Null);
    }

    /// <summary>
    /// Tests that when a member is kicked the party list no longer contains them.
    /// </summary>
    [Test]
    public async ValueTask KickedMember_IsRemovedFromPartyList()
    {
        var (party, member1, member2) = await this.CreatePartyWithTwoMembersAsync().ConfigureAwait(false);

        await party.KickPlayerAsync((byte)party.PartyList.IndexOf(member2)).ConfigureAwait(false);

        Assert.That(party.PartyList, Is.Not.Contains(member2));
        Assert.That(member2.Party, Is.Null);
    }

    private async ValueTask<(Party Party, Player Member1, Player Member2)> CreatePartyWithTwoMembersAsync()
    {
        var member1 = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        var member2 = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        var party = this._partyManager.CreateParty();
        await party.AddAsync(member1).ConfigureAwait(false);
        await party.AddAsync(member2).ConfigureAwait(false);
        return (party, member1, member2);
    }

    private async ValueTask<Player> CreatePartyMemberAsync()
    {
        var result = await PlayerTestHelper.CreatePlayerAsync(GameContextTestHelper.CreateGameContext()).ConfigureAwait(false);
        await result.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        return result;
    }
}