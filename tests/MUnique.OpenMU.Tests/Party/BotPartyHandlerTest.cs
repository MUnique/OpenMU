// <copyright file="BotPartyHandlerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions.Party;

/// <summary>
/// Tests <see cref="BotPartyHandler"/> - how a server-side bot answers party invitations from
/// players and how long it stays in a party.
/// </summary>
[TestFixture]
public class BotPartyHandlerTest
{
    /// <summary>
    /// The happy path: an eligible invitation is scheduled and, once processed, forms a party with
    /// the inviter as its master.
    /// </summary>
    [Test]
    public async ValueTask AcceptsInviteAndFormsPartyAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);
        var requester = await CreateHumanAsync(gameContext, "Human").ConfigureAwait(false);

        var scheduled = await BotPartyHandler.TryScheduleAcceptAsync(bot, requester, TimeSpan.Zero).ConfigureAwait(false);
        Assert.That(scheduled, Is.True);
        Assert.That(bot.PendingPartyInvite, Is.Not.Null);
        Assert.That(bot.LastPartyRequester, Is.SameAs(requester));

        await BotPartyHandler.ProcessAsync(bot).ConfigureAwait(false);

        Assert.That(bot.Party, Is.Not.Null);
        Assert.That(bot.Party!.PartyMaster, Is.SameAs(requester));
        Assert.That(requester.Party, Is.SameAs(bot.Party));
        Assert.That(bot.PendingPartyInvite, Is.Null);
        Assert.That(bot.LastPartyRequester, Is.Null);
    }

    /// <summary>
    /// There is no level gate (matching OpenMU's own party action): a bot accepts an inviter of any
    /// level, since it is the player who invites and the bot stays with the party once it joined. The
    /// inviter here is a maxed veteran (character level 400 plus the master level cap of 200, i.e. the
    /// ceiling of the Season 6 seed) inviting a low-level bot - the widest gap a stock server produces.
    /// </summary>
    [Test]
    public async ValueTask AcceptsInviteRegardlessOfLevelGapAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);
        var requester = await CreateHumanAsync(gameContext, "Human").ConfigureAwait(false);
        requester.Attributes![Stats.Level] = 400;
        requester.Attributes![Stats.MasterLevel] = 200;

        var scheduled = await BotPartyHandler.TryScheduleAcceptAsync(bot, requester, TimeSpan.Zero).ConfigureAwait(false);

        Assert.That(scheduled, Is.True);
        Assert.That(bot.PendingPartyInvite, Is.Not.Null);
        Assert.That(bot.LastPartyRequester, Is.SameAs(requester));
    }

    /// <summary>
    /// A bot on a shopping errand declines the invitation, like a busy player would.
    /// </summary>
    [Test]
    public async ValueTask RejectsWhileOnShoppingTripAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);
        bot.IsOnShoppingTrip = true;
        var requester = await CreateHumanAsync(gameContext, "Human").ConfigureAwait(false);

        var scheduled = await BotPartyHandler.TryScheduleAcceptAsync(bot, requester, TimeSpan.Zero).ConfigureAwait(false);

        Assert.That(scheduled, Is.False);
    }

    /// <summary>
    /// Only server-side bot accounts answer; a regular offline session of a human account does not.
    /// </summary>
    [Test]
    public async ValueTask RejectsForNonBotAccountAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext, "Bot", isBot: false).ConfigureAwait(false);
        var requester = await CreateHumanAsync(gameContext, "Human").ConfigureAwait(false);

        var scheduled = await BotPartyHandler.TryScheduleAcceptAsync(bot, requester, TimeSpan.Zero).ConfigureAwait(false);

        Assert.That(scheduled, Is.False);
    }

    /// <summary>
    /// The invitation is re-validated when the delay passed: an inviter who joined another party as a
    /// plain member in the meantime cannot take the bot in anymore.
    /// </summary>
    [Test]
    public async ValueTask CancelsWhenRequesterJoinedAnotherPartyAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);
        var requester = await CreateHumanAsync(gameContext, "Human").ConfigureAwait(false);
        var thirdPlayer = await CreateHumanAsync(gameContext, "Third").ConfigureAwait(false);

        var scheduled = await BotPartyHandler.TryScheduleAcceptAsync(bot, requester, TimeSpan.Zero).ConfigureAwait(false);
        Assert.That(scheduled, Is.True);

        // Meanwhile the inviter joins another party as a plain member (the third player is master).
        var otherParty = gameContext.PartyManager.CreateParty();
        await otherParty.AddAsync(thirdPlayer).ConfigureAwait(false);
        await otherParty.AddAsync(requester).ConfigureAwait(false);

        await BotPartyHandler.ProcessAsync(bot).ConfigureAwait(false);

        Assert.That(bot.Party, Is.Null);
        Assert.That(bot.PendingPartyInvite, Is.Null);
        Assert.That(bot.LastPartyRequester, Is.Null);
        Assert.That(otherParty.PartyList, Does.Not.Contain(bot));
    }

    /// <summary>
    /// The bot stays in a party with a human instead of leaving on its own: a player who groups a
    /// bot keeps a companion for the whole session, and the bot only leaves on the engine's own
    /// terms (party disbands, kicked, or it cannot legally follow the leader anymore).
    /// </summary>
    [Test]
    public async ValueTask StaysInPartyWithHumanAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);
        var requester = await CreateHumanAsync(gameContext, "Human").ConfigureAwait(false);
        var party = gameContext.PartyManager.CreateParty();
        await party.AddAsync(requester).ConfigureAwait(false);
        await party.AddAsync(bot).ConfigureAwait(false);

        await BotPartyHandler.ProcessAsync(bot).ConfigureAwait(false);

        Assert.That(bot.Party, Is.SameAs(party));
        Assert.That(requester.Party, Is.SameAs(party));
    }

    /// <summary>
    /// Bot-only parties are managed by the hourly re-formation (see <see cref="BotManager"/>), and a
    /// bot never leaves such a party on its own - it stays with its group.
    /// </summary>
    [Test]
    public async ValueTask StaysInBotOnlyPartyAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);
        var otherBot = await CreateBotAsync(gameContext, "OtherBot").ConfigureAwait(false);
        var party = gameContext.PartyManager.CreateParty();
        await party.AddAsync(otherBot).ConfigureAwait(false);
        await party.AddAsync(bot).ConfigureAwait(false);

        await BotPartyHandler.ProcessAsync(bot).ConfigureAwait(false);

        Assert.That(bot.Party, Is.SameAs(party));
    }

    /// <summary>
    /// When the player who grouped the bot disconnects, the engine swaps them for an offline snapshot
    /// to keep the slot reserved. The bot must not leave immediately - it waits out the grace period,
    /// so a quick reconnect (or the player starting off-level) does not strand or churn the party.
    /// </summary>
    [Test]
    public async ValueTask StaysInPartyDuringGracePeriodAfterHumanDisconnectsAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);
        var requester = await CreateHumanAsync(gameContext, "Human").ConfigureAwait(false);
        var party = gameContext.PartyManager.CreateParty();
        await party.AddAsync(requester).ConfigureAwait(false);
        await party.AddAsync(bot).ConfigureAwait(false);

        // The human disconnects: the engine keeps the slot by replacing them with an offline snapshot.
        await party.LeaveTemporarilyAsync(requester).ConfigureAwait(false);

        await BotPartyHandler.ProcessAsync(bot).ConfigureAwait(false);

        Assert.That(bot.Party, Is.SameAs(party));
    }

    /// <summary>
    /// Once the human who grouped the bot has been gone for longer than the grace period - and is
    /// neither reconnected nor off-leveling - the bot leaves the dead party so it returns to the
    /// party-less pool the hourly re-formation draws from, instead of being stranded forever.
    /// </summary>
    [Test]
    public async ValueTask LeavesPartyAfterGracePeriodWhenHumanStaysGoneAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);
        var requester = await CreateHumanAsync(gameContext, "Human").ConfigureAwait(false);
        var party = gameContext.PartyManager.CreateParty();
        await party.AddAsync(requester).ConfigureAwait(false);
        await party.AddAsync(bot).ConfigureAwait(false);

        await party.LeaveTemporarilyAsync(requester).ConfigureAwait(false);

        // Simulate the human having been gone past the grace period.
        var pastGracePeriod = DateTime.UtcNow + TimeSpan.FromMinutes(11);

        await BotPartyHandler.ProcessAsync(bot, pastGracePeriod).ConfigureAwait(false);

        Assert.That(bot.Party, Is.Null);
    }

    /// <summary>
    /// Mastership moves off the disconnected human when a live member leaves the party properly
    /// (<see cref="Party"/> hands the master slot to the first remaining member), so a dead party can
    /// end up with a live bot as master while a human's snapshot is still seated in it. The bot must
    /// still leave once the grace period elapses - keying off the master's type instead of whether any
    /// human snapshot remains would let this shape through.
    /// </summary>
    [Test]
    public async ValueTask LeavesPartyWhenMasterIsBotButHumanSnapshotRemainsAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var master = await CreateHumanAsync(gameContext, "Master").ConfigureAwait(false);
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);
        var other = await CreateHumanAsync(gameContext, "Other").ConfigureAwait(false);
        var party = gameContext.PartyManager.CreateParty();
        await party.AddAsync(master).ConfigureAwait(false);
        await party.AddAsync(bot).ConfigureAwait(false);
        await party.AddAsync(other).ConfigureAwait(false);

        // The master leaves properly: mastership moves to the first remaining member, the bot.
        await party.KickMySelfAsync(master).ConfigureAwait(false);
        Assert.That(party.PartyMaster, Is.SameAs(bot));

        // The remaining human then disconnects, leaving a snapshot behind a live bot master.
        await party.LeaveTemporarilyAsync(other).ConfigureAwait(false);

        var pastGracePeriod = DateTime.UtcNow + TimeSpan.FromMinutes(11);

        await BotPartyHandler.ProcessAsync(bot, pastGracePeriod).ConfigureAwait(false);

        Assert.That(bot.Party, Is.Null);
    }

    /// <summary>
    /// The full wiring: a party request through the regular request action reaches the bot via the
    /// <see cref="GameLogic.MuHelper.PartyRequestHandler"/> criteria and schedules the delayed answer.
    /// </summary>
    [Test]
    public async ValueTask PartyRequestActionSchedulesInviteForBotAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);
        var requester = await CreateHumanAsync(gameContext, "Human").ConfigureAwait(false);
        requester.Observers.Add(bot);

        var action = new PartyRequestAction();
        await action.HandlePartyRequestAsync(requester, bot).ConfigureAwait(false);

        Assert.That(bot.PendingPartyInvite, Is.Not.Null);
        Assert.That(bot.PendingPartyInvite!.Requester, Is.SameAs(requester));
        Assert.That(bot.LastPartyRequester, Is.SameAs(requester));
    }

    private static async ValueTask<OfflinePlayer> CreateBotAsync(IGameContext gameContext, string name, bool isBot = true)
    {
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        bot.SelectedCharacter!.Name = name;
        bot.IsAlive = true;
        bot.Account!.IsBot = isBot;
        bot.MuHelperSettings = new BotMuHelperSettings();
        return bot;
    }

    private static async ValueTask<Player> CreateHumanAsync(IGameContext gameContext, string name)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        player.SelectedCharacter!.Name = name;
        player.IsAlive = true;
        return player;
    }
}
