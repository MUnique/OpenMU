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
/// players and when it leaves the party again.
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
    /// An inviter whose effective level is too far from the bot's is declined - the group would only
    /// be a power-leveling service.
    /// </summary>
    [Test]
    public async ValueTask RejectsTooLargeLevelGapAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);
        var requester = await CreateHumanAsync(gameContext, "Human").ConfigureAwait(false);
        requester.Attributes![Stats.Level] = 700;

        var scheduled = await BotPartyHandler.TryScheduleAcceptAsync(bot, requester, TimeSpan.Zero).ConfigureAwait(false);

        Assert.That(scheduled, Is.False);
        Assert.That(bot.PendingPartyInvite, Is.Null);
        Assert.That(bot.LastPartyRequester, Is.Null);
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
    /// After its rolled party time is up, the bot gets bored of the party with a human and leaves.
    /// </summary>
    [Test]
    public async ValueTask LeavesPartyWithHumanWhenBoredAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);
        var requester = await CreateHumanAsync(gameContext, "Human").ConfigureAwait(false);
        var party = gameContext.PartyManager.CreateParty();
        await party.AddAsync(requester).ConfigureAwait(false);
        await party.AddAsync(bot).ConfigureAwait(false);

        bot.PartyBoredomAtUtc = DateTime.UtcNow - TimeSpan.FromSeconds(1);
        await BotPartyHandler.ProcessAsync(bot).ConfigureAwait(false);

        Assert.That(bot.Party, Is.Null);
        Assert.That(bot.PartyBoredomAtUtc, Is.Null);
    }

    /// <summary>
    /// Bot-only parties are managed by the hourly re-formation instead - no boredom timer runs, and
    /// the bot stays with its group.
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

        bot.PartyBoredomAtUtc = DateTime.UtcNow - TimeSpan.FromSeconds(1);
        await BotPartyHandler.ProcessAsync(bot).ConfigureAwait(false);

        Assert.That(bot.Party, Is.SameAs(party));
        Assert.That(bot.PartyBoredomAtUtc, Is.Null);
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
