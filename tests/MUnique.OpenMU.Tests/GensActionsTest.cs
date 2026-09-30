// <copyright file="GensActionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Gens;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Gens;
using MUnique.OpenMU.GameLogic.Views.Gens;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Tests for the <see cref="GensActions"/>.
/// </summary>
[TestFixture]
public class GensActionsTest
{
    private const short DuprianNpcNumber = 543;
    private const short VanertNpcNumber = 544;

    private readonly GensActions _actions = new();

    /// <summary>
    /// Tests that a player joins the gens of the npc, and gets the starting contribution and the lowest rank.
    /// </summary>
    [Test]
    public async Task JoinAsync()
    {
        var player = await CreatePlayerAsync(DuprianNpcNumber).ConfigureAwait(false);

        await this._actions.JoinAsync(player, GensType.Duprian).ConfigureAwait(false);

        Assert.That(player.GensMember, Is.Not.Null);
        Assert.That(player.GensMember!.Gens, Is.EqualTo(GensType.Duprian));
        Assert.That(player.GensMember.CharacterId, Is.EqualTo(player.SelectedCharacter!.Id));
        Assert.That(player.GensMember.Contribution, Is.EqualTo(new GensConfiguration().StartingContribution));
        Assert.That(player.GensMember.Rank, Is.EqualTo(new GensConfiguration().StartingRank));
        Assert.That(player.GensMember.JoinedAt, Is.Not.Null);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IGensViewPlugIn>()!);
        view.Verify(v => v.ShowJoinResultAsync(GensJoinResult.Success, GensType.Duprian), Times.Once);
        view.Verify(v => v.ShowGensInfoAsync(), Times.Once);
    }

    /// <summary>
    /// Tests that the membership is saved, so that it's loaded again when the character enters the game.
    /// </summary>
    [Test]
    public async Task JoinedMembershipIsSavedAsync()
    {
        var player = await CreatePlayerAsync(VanertNpcNumber).ConfigureAwait(false);

        await this._actions.JoinAsync(player, GensType.Vanert).ConfigureAwait(false);

        var member = await player.PersistenceContext.GetGensMemberAsync(player.SelectedCharacter!.Id).ConfigureAwait(false);
        Assert.That(member?.Gens, Is.EqualTo(GensType.Vanert));
    }

    /// <summary>
    /// Tests that the requested gens has to be the one of the npc, to which the player talks.
    /// Otherwise, the request is ignored without a response, because the client doesn't send such a request.
    /// </summary>
    [Test]
    public async Task JoinAtNpcOfOtherGensIsIgnoredAsync()
    {
        var player = await CreatePlayerAsync(DuprianNpcNumber).ConfigureAwait(false);

        await this._actions.JoinAsync(player, GensType.Vanert).ConfigureAwait(false);
        await this._actions.JoinAsync(player, GensType.None).ConfigureAwait(false);

        Assert.That(player.GensMember, Is.Null);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IGensViewPlugIn>()!);
        view.Verify(v => v.ShowJoinResultAsync(It.IsAny<GensJoinResult>(), It.IsAny<GensType>()), Times.Never);
    }

    /// <summary>
    /// Tests that a player can't join a gens, when the gens system is deactivated.
    /// </summary>
    [Test]
    public async Task JoinWithoutGensSystemIsIgnoredAsync()
    {
        var player = await CreatePlayerAsync(DuprianNpcNumber, registerFeature: false).ConfigureAwait(false);

        await this._actions.JoinAsync(player, GensType.Duprian).ConfigureAwait(false);

        Assert.That(player.GensMember, Is.Null);
    }

    /// <summary>
    /// Tests the requirements to join a gens.
    /// </summary>
    [Test]
    public async Task JoinRequirementsAsync()
    {
        var player = await CreatePlayerAsync(DuprianNpcNumber).ConfigureAwait(false);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IGensViewPlugIn>()!);

        player.Attributes![Stats.Level] = 49;
        await this._actions.JoinAsync(player, GensType.Duprian).ConfigureAwait(false);
        view.Verify(v => v.ShowJoinResultAsync(GensJoinResult.LevelTooLow, GensType.Duprian), Times.Once);

        player.Attributes[Stats.Level] = 50;
        player.GuildStatus = new GuildMemberStatus(1, GuildPosition.NormalMember);
        await this._actions.JoinAsync(player, GensType.Duprian).ConfigureAwait(false);
        view.Verify(v => v.ShowJoinResultAsync(GensJoinResult.GuildMember, GensType.Duprian), Times.Once);

        player.GuildStatus = new GuildMemberStatus(1, GuildPosition.GuildMaster);
        await this._actions.JoinAsync(player, GensType.Duprian).ConfigureAwait(false);
        view.Verify(v => v.ShowJoinResultAsync(GensJoinResult.GuildMaster, GensType.Duprian), Times.Once);

        Assert.That(player.GensMember, Is.Null);

        player.GuildStatus = null;
        await this._actions.JoinAsync(player, GensType.Duprian).ConfigureAwait(false);
        await this._actions.JoinAsync(player, GensType.Duprian).ConfigureAwait(false);
        view.Verify(v => v.ShowJoinResultAsync(GensJoinResult.AlreadyJoined, GensType.Duprian), Times.Once);
    }

    /// <summary>
    /// Tests that a player which left a gens has to wait the configured time, until it can join again.
    /// </summary>
    [Test]
    public async Task RejoinWaitTimeAsync()
    {
        var configuration = new GensConfiguration { RejoinWaitTime = TimeSpan.FromDays(7) };
        var player = await CreatePlayerAsync(DuprianNpcNumber, configuration).ConfigureAwait(false);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IGensViewPlugIn>()!);

        await this._actions.JoinAsync(player, GensType.Duprian).ConfigureAwait(false);
        await this._actions.LeaveAsync(player).ConfigureAwait(false);
        await this._actions.JoinAsync(player, GensType.Duprian).ConfigureAwait(false);
        view.Verify(v => v.ShowJoinResultAsync(GensJoinResult.LeftRecently, GensType.Duprian), Times.Once);

        player.GensMember!.LeftAt = DateTime.UtcNow.AddDays(-8);
        await this._actions.JoinAsync(player, GensType.Duprian).ConfigureAwait(false);
        view.Verify(v => v.ShowJoinResultAsync(GensJoinResult.Success, GensType.Duprian), Times.Exactly(2));
        Assert.That(player.GensMember.Gens, Is.EqualTo(GensType.Duprian));
    }

    /// <summary>
    /// Tests that a player leaves its gens, and loses its contribution and rank.
    /// </summary>
    [Test]
    public async Task LeaveAsync()
    {
        var player = await CreatePlayerAsync(VanertNpcNumber).ConfigureAwait(false);
        await this._actions.JoinAsync(player, GensType.Vanert).ConfigureAwait(false);

        await this._actions.LeaveAsync(player).ConfigureAwait(false);

        Assert.That(player.GensMember!.Gens, Is.EqualTo(GensType.None));
        Assert.That(player.GensMember.Contribution, Is.Zero);
        Assert.That(player.GensMember.Rank, Is.Zero);
        Assert.That(player.GensMember.LeftAt, Is.Not.Null);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IGensViewPlugIn>()!);
        view.Verify(v => v.ShowLeaveResultAsync(GensLeaveResult.Success), Times.Once);
    }

    /// <summary>
    /// Tests the requirements to leave a gens.
    /// </summary>
    [Test]
    public async Task LeaveRequirementsAsync()
    {
        var player = await CreatePlayerAsync(DuprianNpcNumber).ConfigureAwait(false);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IGensViewPlugIn>()!);

        await this._actions.LeaveAsync(player).ConfigureAwait(false);
        view.Verify(v => v.ShowLeaveResultAsync(GensLeaveResult.NotJoined), Times.Once);

        await this._actions.JoinAsync(player, GensType.Duprian).ConfigureAwait(false);
        player.GuildStatus = new GuildMemberStatus(1, GuildPosition.GuildMaster);
        await this._actions.LeaveAsync(player).ConfigureAwait(false);
        view.Verify(v => v.ShowLeaveResultAsync(GensLeaveResult.GuildMaster), Times.Once);

        player.GuildStatus = null;
        player.OpenedNpc = CreateNpc(VanertNpcNumber);
        await this._actions.LeaveAsync(player).ConfigureAwait(false);
        view.Verify(v => v.ShowLeaveResultAsync(GensLeaveResult.DifferentGensNpc), Times.Once);

        Assert.That(player.GensMember!.Gens, Is.EqualTo(GensType.Duprian));
    }

    /// <summary>
    /// Tests that the request of the gens reward is always answered at a gens npc, because the client waits for it.
    /// The rewards aren't implemented yet, so a member of the gens of the npc is not eligible.
    /// </summary>
    [Test]
    public async Task RequestRewardAsync()
    {
        var player = await CreatePlayerAsync(DuprianNpcNumber).ConfigureAwait(false);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IGensViewPlugIn>()!);

        await this._actions.RequestRewardAsync(player, GensType.Duprian).ConfigureAwait(false);
        view.Verify(v => v.ShowRewardResultAsync(GensRewardResult.NotJoined), Times.Once);

        await this._actions.JoinAsync(player, GensType.Duprian).ConfigureAwait(false);
        await this._actions.RequestRewardAsync(player, GensType.Duprian).ConfigureAwait(false);
        view.Verify(v => v.ShowRewardResultAsync(GensRewardResult.NotEligible), Times.Once);

        player.OpenedNpc = CreateNpc(VanertNpcNumber);
        await this._actions.RequestRewardAsync(player, GensType.Vanert).ConfigureAwait(false);
        view.Verify(v => v.ShowRewardResultAsync(GensRewardResult.DifferentGensNpc), Times.Once);

        await this._actions.RequestRewardAsync(player, GensType.Duprian).ConfigureAwait(false);
        view.Verify(v => v.ShowRewardResultAsync(It.IsAny<GensRewardResult>()), Times.Exactly(3), "A request for the gens of another npc is ignored.");
    }

    /// <summary>
    /// Tests that the membership is loaded when the character enters the game, and removed when it leaves it.
    /// </summary>
    [Test]
    public async Task MembershipIsLoadedWhenEnteringTheGameAsync()
    {
        var player = await CreatePlayerAsync(DuprianNpcNumber).ConfigureAwait(false);
        await this._actions.JoinAsync(player, GensType.Duprian).ConfigureAwait(false);
        var plugIn = new GensMembershipPlugIn();

        await plugIn.PlayerStateChangedAsync(player, PlayerState.EnteredWorld, PlayerState.CharacterSelection).ConfigureAwait(false);
        Assert.That(player.GensMember, Is.Null);

        await plugIn.PlayerStateChangedAsync(player, PlayerState.CharacterSelection, PlayerState.EnteredWorld).ConfigureAwait(false);
        Assert.That(player.GensMember?.Gens, Is.EqualTo(GensType.Duprian));
    }

    private static async ValueTask<Player> CreatePlayerAsync(short npcNumber, GensConfiguration? configuration = null, bool registerFeature = true)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        if (registerFeature)
        {
            player.GameContext.FeaturePlugIns.AddPlugIn(new GensFeaturePlugIn { Configuration = configuration ?? new GensConfiguration() }, true);
        }

        player.SelectedCharacter!.Id = Guid.NewGuid();
        player.Attributes![Stats.Level] = 50;
        player.OpenedNpc = CreateNpc(npcNumber);
        return player;
    }

    private static NonPlayerCharacter CreateNpc(short number)
    {
        return new NonPlayerCharacter(null!, new MonsterDefinition { Number = number, NpcWindow = NpcWindow.NpcDialog }, null!);
    }
}
