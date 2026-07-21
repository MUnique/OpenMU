// <copyright file="BotMiniGameHandlerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Tests the entry decisions of <see cref="BotMiniGameHandler"/> - which party bots may follow
/// their human leader into a mini game event, and who is taken along at all. The actual entry
/// goes through the regular <see cref="MUnique.OpenMU.GameLogic.MiniGames.MiniGameContext"/>.
/// </summary>
[TestFixture]
public class BotMiniGameHandlerTest
{
    /// <summary>
    /// The event's character level bracket is enforced in both directions.
    /// </summary>
    /// <param name="level">The bot's character level.</param>
    /// <param name="expected">Whether the bot qualifies.</param>
    [TestCase(200, false)]
    [TestCase(281, true)]
    [TestCase(330, true)]
    [TestCase(331, false)]
    public async ValueTask EnforcesLevelBracketAsync(int level, bool expected)
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext).ConfigureAwait(false);
        bot.Attributes![Stats.Level] = level;
        var definition = new MiniGameDefinition { MinimumCharacterLevel = 281, MaximumCharacterLevel = 330 };

        var eligible = BotMiniGameHandler.IsEligible(bot, definition, out var reason);

        Assert.That(eligible, Is.EqualTo(expected));
        Assert.That(reason, expected ? Is.Empty : Is.Not.Empty);
    }

    /// <summary>
    /// The special characters (Magic Gladiator, Dark Lord, Rage Fighter, Summoner) enter in their own
    /// level bracket, exactly like they do for a player: a qualified Magic Gladiator must not be judged
    /// - and kicked out of its leader's party - by the bracket of the regular classes.
    /// </summary>
    /// <param name="level">The bot's character level.</param>
    /// <param name="expected">Whether the bot qualifies.</param>
    [TestCase(200, false)]
    [TestCase(221, true)]
    [TestCase(280, true)]
    [TestCase(281, false)]
    public async ValueTask EnforcesSpecialCharacterLevelBracketAsync(int level, bool expected)
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext).ConfigureAwait(false);
        bot.Attributes![Stats.Level] = level;

        // That is what makes a character "special" for the entry rules (see CharacterExtensions).
        bot.SelectedCharacter!.CharacterClass!.LevelWarpRequirementReductionPercent = 50;
        var definition = new MiniGameDefinition
        {
            MinimumCharacterLevel = 281,
            MaximumCharacterLevel = 330,
            MinimumSpecialCharacterLevel = 221,
            MaximumSpecialCharacterLevel = 280,
        };

        var eligible = BotMiniGameHandler.IsEligible(bot, definition, out _);

        Assert.That(eligible, Is.EqualTo(expected));
    }

    /// <summary>
    /// An event for master classes only is not entered before the bot's master evolution.
    /// </summary>
    /// <param name="isMasterClass">Whether the bot evolved into its master class.</param>
    /// <param name="expected">Whether the bot qualifies.</param>
    [TestCase(false, false)]
    [TestCase(true, true)]
    public async ValueTask EnforcesMasterClassRequirementAsync(bool isMasterClass, bool expected)
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext).ConfigureAwait(false);
        bot.Attributes![Stats.Level] = 400;
        bot.SelectedCharacter!.CharacterClass!.IsMasterClass = isMasterClass;
        var definition = new MiniGameDefinition { MinimumCharacterLevel = 0, MaximumCharacterLevel = 400, RequiresMasterClass = true };

        var eligible = BotMiniGameHandler.IsEligible(bot, definition, out _);

        Assert.That(eligible, Is.EqualTo(expected));
    }

    /// <summary>
    /// A player killer bot (should never happen, but the rule is mirrored from the player entry)
    /// cannot enter events which disallow player killers.
    /// </summary>
    /// <param name="killersAllowed">Whether the event allows player killers.</param>
    /// <param name="expected">Whether the bot qualifies.</param>
    [TestCase(false, false)]
    [TestCase(true, true)]
    public async ValueTask EnforcesPlayerKillerRestrictionAsync(bool killersAllowed, bool expected)
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await CreateBotAsync(gameContext).ConfigureAwait(false);
        bot.Attributes![Stats.Level] = 100;
        bot.SelectedCharacter!.State = HeroState.PlayerKiller1stStage;
        var definition = new MiniGameDefinition { MinimumCharacterLevel = 0, MaximumCharacterLevel = 400, ArePlayerKillersAllowedToEnter = killersAllowed };

        var eligible = BotMiniGameHandler.IsEligible(bot, definition, out _);

        Assert.That(eligible, Is.EqualTo(expected));
    }

    /// <summary>
    /// Only the bots of the party whose MASTER enters are taken along - and only the bots, not the
    /// human members.
    /// </summary>
    [Test]
    public async ValueTask SnapshotTakesOnlyBotsOfTheEnteringMasterAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var leader = await CreateHumanAsync(gameContext, "Leader").ConfigureAwait(false);
        var member = await CreateHumanAsync(gameContext, "Member").ConfigureAwait(false);
        var bot1 = await CreateBotAsync(gameContext, "BotOne").ConfigureAwait(false);
        var bot2 = await CreateBotAsync(gameContext, "BotTwo").ConfigureAwait(false);

        var party = gameContext.PartyManager.CreateParty();
        await party.AddAsync(leader).ConfigureAwait(false);
        await party.AddAsync(member).ConfigureAwait(false);
        await party.AddAsync(bot1).ConfigureAwait(false);
        await party.AddAsync(bot2).ConfigureAwait(false);

        var fromLeader = BotMiniGameHandler.SnapshotPartyBots(leader);
        var fromMember = BotMiniGameHandler.SnapshotPartyBots(member);

        Assert.That(fromLeader, Is.EquivalentTo(new[] { bot1, bot2 }));
        Assert.That(fromMember, Is.Empty);
    }

    /// <summary>
    /// A player without a party (or a bot, however it would get here) takes nobody along.
    /// </summary>
    [Test]
    public async ValueTask SnapshotIsEmptyWithoutPartyAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var solo = await CreateHumanAsync(gameContext, "Solo").ConfigureAwait(false);
        var bot = await CreateBotAsync(gameContext, "Bot").ConfigureAwait(false);

        Assert.That(BotMiniGameHandler.SnapshotPartyBots(solo), Is.Empty);
        Assert.That(BotMiniGameHandler.SnapshotPartyBots(bot), Is.Empty);
    }

    private static async ValueTask<OfflinePlayer> CreateBotAsync(IGameContext gameContext, string name = "Bot")
    {
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        bot.SelectedCharacter!.Name = name;
        bot.IsAlive = true;
        bot.Account!.IsBot = true;
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
