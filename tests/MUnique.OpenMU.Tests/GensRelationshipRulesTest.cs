// <copyright file="GensRelationshipRulesTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.ComponentModel;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Gens;
using MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// Tests for the <see cref="GensRelationshipRulesPlugIn"/>.
/// </summary>
[TestFixture]
public class GensRelationshipRulesTest
{
    /// <summary>
    /// The number of the map of the test players.
    /// </summary>
    private const short MapNumber = 0;

    private readonly GensRelationshipRulesPlugIn _plugIn = new();

    /// <summary>
    /// Tests that members of different gens can't form a party, while members of the same gens and non-members can.
    /// </summary>
    [Test]
    public async Task PartyBetweenGensAsync()
    {
        var (duprian, vanert) = await CreatePlayersAsync(GensType.Duprian, GensType.Vanert).ConfigureAwait(false);

        Assert.That(await this.IsPartyDeniedAsync(duprian, vanert).ConfigureAwait(false), Is.True);

        vanert.GensMember!.Gens = GensType.Duprian;
        Assert.That(await this.IsPartyDeniedAsync(duprian, vanert).ConfigureAwait(false), Is.False);

        vanert.GensMember.Gens = GensType.None;
        Assert.That(await this.IsPartyDeniedAsync(duprian, vanert).ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that no party can be formed in the battle zone.
    /// </summary>
    [Test]
    public async Task NoPartyInBattleZoneAsync()
    {
        var (first, second) = await CreatePlayersAsync(GensType.Duprian, GensType.Duprian).ConfigureAwait(false);
        GetConfiguration(first).BattleZoneMapNumbers.Add(MapNumber);

        Assert.That(await this.IsPartyDeniedAsync(first, second).ConfigureAwait(false), Is.True);
    }

    /// <summary>
    /// Tests that a player leaves its party when it enters the battle zone.
    /// </summary>
    [Test]
    public async Task PartyIsLeftWhenEnteringTheBattleZoneAsync()
    {
        var (first, second) = await CreatePlayersAsync(GensType.Duprian, GensType.Duprian).ConfigureAwait(false);
        var party = new PartyManager(5, new NullLogger<Party>()).CreateParty();
        await party.AddAsync(first).ConfigureAwait(false);
        await party.AddAsync(second).ConfigureAwait(false);

        await this._plugIn.ObjectAddedToMapAsync(first.CurrentMap!, first).ConfigureAwait(false);
        Assert.That(first.Party, Is.Not.Null, "Outside of the battle zone, the party stays.");

        GetConfiguration(first).BattleZoneMapNumbers.Add(MapNumber);
        await this._plugIn.ObjectAddedToMapAsync(first.CurrentMap!, first).ConfigureAwait(false);
        Assert.That(first.Party, Is.Null);
    }

    /// <summary>
    /// Tests that a player can only join the guild of a guild master of the same gens.
    /// </summary>
    /// <param name="requesterGens">The gens of the requesting player.</param>
    /// <param name="guildMasterGens">The gens of the guild master.</param>
    /// <param name="expectedResult">The expected result; <c>null</c>, if the request isn't denied.</param>
    [TestCase(GensType.Duprian, GensType.Duprian, null)]
    [TestCase(GensType.Duprian, GensType.None, GuildRequestAnswerResult.GuildMasterNotInGens)]
    [TestCase(GensType.None, GensType.Duprian, GuildRequestAnswerResult.NotInGensOfGuildMaster)]
    [TestCase(GensType.Vanert, GensType.Duprian, GuildRequestAnswerResult.GuildMasterInDifferentGens)]
    public async Task GuildJoinAsync(GensType requesterGens, GensType guildMasterGens, GuildRequestAnswerResult? expectedResult)
    {
        var (requester, guildMaster) = await CreatePlayersAsync(requesterGens, guildMasterGens).ConfigureAwait(false);

        var eventArgs = new CancelEventArgs();
        await this._plugIn.GuildJoinRequestingAsync(requester, guildMaster, eventArgs).ConfigureAwait(false);

        Assert.That(eventArgs.Cancel, Is.EqualTo(expectedResult is not null));
        var view = Mock.Get(requester.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()!);
        if (expectedResult is { } result)
        {
            view.Verify(v => v.ShowGuildJoinResponseAsync(result), Times.Once);
        }
        else
        {
            view.Verify(v => v.ShowGuildJoinResponseAsync(It.IsAny<GuildRequestAnswerResult>()), Times.Never);
        }
    }

    /// <summary>
    /// Tests that only gens members can create a guild, unless it's configured otherwise.
    /// </summary>
    [Test]
    public async Task GuildCreationAsync()
    {
        var (member, nonMember) = await CreatePlayersAsync(GensType.Duprian, GensType.None).ConfigureAwait(false);

        Assert.That(await this.IsGuildCreationDeniedAsync(member).ConfigureAwait(false), Is.False);
        Assert.That(await this.IsGuildCreationDeniedAsync(nonMember).ConfigureAwait(false), Is.True);

        GetConfiguration(member).GuildRequiresGens = false;
        Assert.That(await this.IsGuildCreationDeniedAsync(nonMember).ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that only the masters of guilds of the same gens can form an alliance.
    /// </summary>
    /// <param name="requesterGens">The gens of the requesting guild master.</param>
    /// <param name="targetGens">The gens of the other guild master.</param>
    /// <param name="expectedResult">The expected result; <c>null</c>, if the request isn't denied.</param>
    [TestCase(GensType.Vanert, GensType.Vanert, null)]
    [TestCase(GensType.None, GensType.Vanert, GuildRelationshipChangeResultType.AllianceMasterNotInGens)]
    [TestCase(GensType.Vanert, GensType.None, GuildRelationshipChangeResultType.GuildMasterNotInGens)]
    [TestCase(GensType.Vanert, GensType.Duprian, GuildRelationshipChangeResultType.DifferentGens)]
    public async Task AllianceAsync(GensType requesterGens, GensType targetGens, GuildRelationshipChangeResultType? expectedResult)
    {
        var (requester, target) = await CreatePlayersAsync(requesterGens, targetGens).ConfigureAwait(false);

        var eventArgs = new CancelEventArgs();
        await this._plugIn.GuildRelationshipChangingAsync(requester, target, GuildRelationshipType.Alliance, GuildRelationshipRequestType.Join, eventArgs).ConfigureAwait(false);
        var hostilityArgs = new CancelEventArgs();
        await this._plugIn.GuildRelationshipChangingAsync(requester, target, GuildRelationshipType.Hostility, GuildRelationshipRequestType.Join, hostilityArgs).ConfigureAwait(false);

        Assert.That(eventArgs.Cancel, Is.EqualTo(expectedResult is not null));
        Assert.That(hostilityArgs.Cancel, Is.False, "The gens don't matter for hostilities.");
        if (expectedResult is { } result)
        {
            var view = Mock.Get(requester.ViewPlugIns.GetPlugIn<IGuildRelationshipChangeResultPlugIn>()!);
            view.Verify(v => v.ShowResultAsync(GuildRelationshipType.Alliance, GuildRelationshipRequestType.Join, result, It.IsAny<ushort?>()), Times.Once);
        }
    }

    private static GensConfiguration GetConfiguration(Player player)
    {
        return GensFeaturePlugIn.GetConfiguration(player.GameContext)!;
    }

    private static async ValueTask<(Player First, Player Second)> CreatePlayersAsync(GensType firstGens, GensType secondGens)
    {
        var first = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var gameContext = (GameContext)first.GameContext;
        gameContext.FeaturePlugIns.AddPlugIn(new GensFeaturePlugIn { Configuration = new GensConfiguration { BattleZoneMapNumbers = new List<short>(), GuildRequiresGens = true, AllianceRequiresSameGens = true } }, true);
        await PreparePlayerAsync(first, firstGens).ConfigureAwait(false);

        var second = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await PreparePlayerAsync(second, secondGens).ConfigureAwait(false);
        return (first, second);
    }

    private static async ValueTask PreparePlayerAsync(Player player, GensType gens)
    {
        player.SelectedCharacter!.Id = Guid.NewGuid();
        player.Attributes![Stats.Level] = 100;
        player.CurrentMap = await player.GameContext.GetMapAsync((ushort)MapNumber).ConfigureAwait(false);
        var member = player.PersistenceContext.CreateNew<GensMember>();
        member.CharacterId = player.SelectedCharacter.Id;
        member.Gens = gens;
        player.GensMember = member;
    }

    private async ValueTask<bool> IsPartyDeniedAsync(Player requester, Player target)
    {
        var eventArgs = new CancelEventArgs();
        await this._plugIn.PartyRequestingAsync(requester, target, eventArgs).ConfigureAwait(false);
        return eventArgs.Cancel;
    }

    private async ValueTask<bool> IsGuildCreationDeniedAsync(Player creator)
    {
        var eventArgs = new CancelEventArgs();
        await this._plugIn.GuildCreatingAsync(creator, eventArgs).ConfigureAwait(false);
        return eventArgs.Cancel;
    }
}
