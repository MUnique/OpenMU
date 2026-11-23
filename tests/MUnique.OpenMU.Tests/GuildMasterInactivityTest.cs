// <copyright file="GuildMasterInactivityTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Tests for the handling of inactive guild masters, and for the tracking of the last seen date of accounts.
/// </summary>
public class GuildMasterInactivityTest : GuildTestBase
{
    private static readonly TimeSpan Limit = TimeSpan.FromDays(7);

    /// <summary>
    /// Tests that the leadership of a guild with an inactive master goes to an active member, and that the old master becomes a normal member.
    /// </summary>
    [Test]
    public async ValueTask InactiveMasterLosesLeadershipToActiveMemberAsync()
    {
        this.SetLastSeen(this.GuildMaster, DaysAgo(20));
        var heir = this.CreateCharacter("Heir", DaysAgo(1));
        await this.AddMembersAsync(GuildName, (heir, GuildPosition.NormalMember)).ConfigureAwait(false);

        var results = await this.GuildServer.RemoveInactiveGuildMastersAsync(Limit, 10).ConfigureAwait(false);

        Assert.Multiple(async () =>
        {
            Assert.That(results, Has.Count.EqualTo(1));
            Assert.That(results[0].GuildName, Is.EqualTo(GuildName));
            Assert.That(results[0].OldMasterName, Is.EqualTo(this.GuildMaster.Name));
            Assert.That(results[0].NewMasterName, Is.EqualTo("Heir"));
            Assert.That(results[0].IsDissolved, Is.False);
            Assert.That(results[0].Inactivity.TotalDays, Is.EqualTo(20).Within(0.1));
            Assert.That(await this.GuildServer.GetGuildPositionAsync(heir.Id).ConfigureAwait(false), Is.EqualTo(GuildPosition.GuildMaster));
            Assert.That(await this.GuildServer.GetGuildPositionAsync(this.GuildMaster.Id).ConfigureAwait(false), Is.EqualTo(GuildPosition.NormalMember));
        });
    }

    /// <summary>
    /// Tests that an assistant master is preferred over a battle master, and a battle master over normal members,
    /// and that members who are inactive, too, never inherit.
    /// </summary>
    [Test]
    public async ValueTask HeirIsTheMostSeniorActiveMemberAsync()
    {
        this.SetLastSeen(this.GuildMaster, DaysAgo(20));
        var normal = this.CreateCharacter("Normal", DaysAgo(0.1));
        var battle = this.CreateCharacter("Battle", DaysAgo(2));
        var inactiveAssistant = this.CreateCharacter("Assist", DaysAgo(9));
        await this.AddMembersAsync(
            GuildName,
            (normal, GuildPosition.NormalMember),
            (battle, GuildPosition.BattleMaster),
            (inactiveAssistant, GuildPosition.AssistantMaster)).ConfigureAwait(false);

        var results = await this.GuildServer.RemoveInactiveGuildMastersAsync(Limit, 10).ConfigureAwait(false);

        Assert.That(results.Single().NewMasterName, Is.EqualTo("Battle"));
    }

    /// <summary>
    /// Tests that a guild without any active member is dissolved.
    /// </summary>
    [Test]
    public async ValueTask GuildWithoutActiveMembersIsDissolvedAsync()
    {
        this.SetLastSeen(this.GuildMaster, DaysAgo(20));
        await this.AddMembersAsync(GuildName, (this.CreateCharacter("Away", DaysAgo(10)), GuildPosition.NormalMember)).ConfigureAwait(false);

        var results = await this.GuildServer.RemoveInactiveGuildMastersAsync(Limit, 10).ConfigureAwait(false);

        Assert.Multiple(async () =>
        {
            Assert.That(results.Single().IsDissolved, Is.True);
            Assert.That(await this.GuildServer.GuildExistsAsync(GuildName).ConfigureAwait(false), Is.False);
        });
    }

    /// <summary>
    /// Tests that a guild master which is online is active, even when the account wasn't seen for a long time,
    /// e.g. because the player stays logged in.
    /// </summary>
    [Test]
    public async ValueTask OnlineMasterIsActiveAsync()
    {
        this.SetLastSeen(this.GuildMaster, DaysAgo(20));
        await this.GuildServer.PlayerEnteredGameAsync(this.GuildMaster.Id, this.GuildMaster.Name, 1).ConfigureAwait(false);

        var results = await this.GuildServer.RemoveInactiveGuildMastersAsync(Limit, 10).ConfigureAwait(false);

        Assert.That(results, Is.Empty);
        Assert.That(await this.GuildServer.GetGuildPositionAsync(this.GuildMaster.Id).ConfigureAwait(false), Is.EqualTo(GuildPosition.GuildMaster));
    }

    /// <summary>
    /// Tests that a guild is not dissolved while one of its members is online: the online member inherits instead.
    /// </summary>
    [Test]
    public async ValueTask GuildWithOnlineMemberIsNeverDissolvedAsync()
    {
        this.SetLastSeen(this.GuildMaster, DaysAgo(20));
        var online = this.CreateCharacter("Online", DaysAgo(30));
        await this.AddMembersAsync(GuildName, (online, GuildPosition.NormalMember)).ConfigureAwait(false);
        await this.GuildServer.PlayerEnteredGameAsync(online.Id, online.Name, 1).ConfigureAwait(false);

        var results = await this.GuildServer.RemoveInactiveGuildMastersAsync(Limit, 10).ConfigureAwait(false);

        Assert.That(results.Single().NewMasterName, Is.EqualTo("Online"));
        Assert.That(await this.GuildServer.GuildExistsAsync(GuildName).ConfigureAwait(false), Is.True);
    }

    /// <summary>
    /// Tests that an active guild master keeps the position.
    /// </summary>
    [Test]
    public async ValueTask ActiveMasterKeepsLeadershipAsync()
    {
        this.SetLastSeen(this.GuildMaster, DaysAgo(6.9));
        await this.AddMembersAsync(GuildName, (this.CreateCharacter("Member", DaysAgo(0)), GuildPosition.NormalMember)).ConfigureAwait(false);

        var results = await this.GuildServer.RemoveInactiveGuildMastersAsync(Limit, 10).ConfigureAwait(false);

        Assert.That(results, Is.Empty);
        Assert.That(await this.GuildServer.GetGuildPositionAsync(this.GuildMaster.Id).ConfigureAwait(false), Is.EqualTo(GuildPosition.GuildMaster));
    }

    /// <summary>
    /// Tests that a limit of zero disables the handling, and that a guild is handled only up to the maximum number of guilds,
    /// beginning with the longest inactivity.
    /// </summary>
    [Test]
    public async ValueTask LimitsAreRespectedAsync()
    {
        this.SetLastSeen(this.GuildMaster, DaysAgo(10));
        var otherMaster = this.CreateCharacter("OtherMaster", DaysAgo(40));
        await this.GuildServer.CreateGuildAsync("Other", otherMaster.Name, otherMaster.Id, new byte[16], 0).ConfigureAwait(false);
        await this.GuildServer.GuildMemberLeftGameAsync(await this.GuildServer.GetGuildIdByNameAsync("Other").ConfigureAwait(false), otherMaster.Id, 0).ConfigureAwait(false);

        var disabled = await this.GuildServer.RemoveInactiveGuildMastersAsync(TimeSpan.Zero, 10).ConfigureAwait(false);
        var results = await this.GuildServer.RemoveInactiveGuildMastersAsync(Limit, 1).ConfigureAwait(false);

        Assert.Multiple(async () =>
        {
            Assert.That(disabled, Is.Empty);
            Assert.That(results.Select(r => r.GuildName), Is.EqualTo(new[] { "Other" }));
            Assert.That(await this.GuildServer.GuildExistsAsync("Other").ConfigureAwait(false), Is.False);
            Assert.That(await this.GuildServer.GuildExistsAsync(GuildName).ConfigureAwait(false), Is.True);
        });
    }

    /// <summary>
    /// Tests that the account is marked as seen when a character enters the world, and when the player leaves the game.
    /// </summary>
    [Test]
    public async ValueTask AccountIsMarkedAsSeenOnEnterAndLeaveAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var character = player.SelectedCharacter!;
        player.Account!.LastSeenDate = DaysAgo(30);

        await player.SetSelectedCharacterAsync(null).ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(character).ConfigureAwait(false);
        var seenOnEnter = player.Account.LastSeenDate;

        player.Account.LastSeenDate = DaysAgo(30);
        await player.RemoveFromGameAsync().ConfigureAwait(false);
        var seenOnLeave = player.Account.LastSeenDate;

        Assert.Multiple(() =>
        {
            Assert.That(DateTime.UtcNow - seenOnEnter, Is.LessThan(TimeSpan.FromMinutes(1)));
            Assert.That(DateTime.UtcNow - seenOnLeave, Is.LessThan(TimeSpan.FromMinutes(1)));
        });
    }

    private static DateTime DaysAgo(double days) => DateTime.UtcNow.AddDays(-days);

    private void SetLastSeen(Character character, DateTime lastSeen)
    {
        var account = this.PersistenceContextProvider.CreateNewContext().CreateNew<Account>();
        account.Characters.Add(character);
        account.LastSeenDate = lastSeen;
    }

    private Character CreateCharacter(string name, DateTime lastSeen)
    {
        var character = this.PersistenceContextProvider.CreateNewContext().CreateNew<Character>();
        character.Name = name;
        this.SetLastSeen(character, lastSeen);
        return character;
    }

    private async ValueTask AddMembersAsync(string guildName, params (Character Character, GuildPosition Position)[] members)
    {
        var persistentId = (await this.GuildServer.GetPersistentGuildIdByNameAsync(guildName).ConfigureAwait(false))!.Value;
        var guildId = await this.GuildServer.GetGuildIdAsync(persistentId).ConfigureAwait(false);
        foreach (var (character, position) in members)
        {
            await this.GuildServer.CreateGuildMemberAsync(guildId, character.Id, character.Name, position, 1).ConfigureAwait(false);
        }

        // The guild gets unloaded when its last member left, so this has to be done after all members were created.
        foreach (var (character, _) in members)
        {
            await this.GuildServer.GuildMemberLeftGameAsync(guildId, character.Id, 1).ConfigureAwait(false);
        }
    }
}
