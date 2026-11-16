// <copyright file="BotChatTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests the bot chat (<see cref="BotChatPlugIn"/>): the limits which keep a large bot population
/// from flooding the chat, and how a configured line becomes the text a player sees.
/// </summary>
[TestFixture]
public class BotChatTest
{
    private static readonly DateTime Start = new(2026, 11, 4, 20, 0, 0, DateTimeKind.Utc);

    private static readonly Dictionary<string, Func<CultureInfo, string>> NoValues = new();

    /// <summary>
    /// A <c>/post</c> reaches the whole server, so all bots of a server share one gap between posts.
    /// </summary>
    [Test]
    public void PostsKeepTheServerWideGap()
    {
        var throttle = new BotChatThrottle();
        var gap = TimeSpan.FromMinutes(3);

        Assert.That(throttle.TryReservePost(Start, gap), Is.True);
        Assert.That(throttle.TryReservePost(Start.AddMinutes(1), gap), Is.False);
        Assert.That(throttle.TryReservePost(Start.AddMinutes(3), gap), Is.True);
    }

    /// <summary>
    /// Every level of Blood Castle reports its own opening; only the first gets the announcement of
    /// the event type, the others stay silent until the reservation expires. Other occasions are
    /// independent of it.
    /// </summary>
    [Test]
    public void AnOccasionIsAnnouncedOncePerWindow()
    {
        var throttle = new BotChatThrottle();
        var window = TimeSpan.FromMinutes(5);

        Assert.That(throttle.TryReserveOccasion("mini game BloodCastle", Start, window), Is.True);
        Assert.That(throttle.TryReserveOccasion("mini game BloodCastle", Start.AddSeconds(20), window), Is.False);
        Assert.That(throttle.TryReserveOccasion("mini game DevilSquare", Start.AddSeconds(20), window), Is.True);
        Assert.That(throttle.TryReserveOccasion("mini game BloodCastle", Start.AddMinutes(5), window), Is.True);
    }

    /// <summary>
    /// Occasions are keyed by player names too (greetings, congratulations); the expired ones are
    /// forgotten from time to time instead of piling up until the server restarts.
    /// </summary>
    [Test]
    public void ForgetsExpiredOccasions()
    {
        var throttle = new BotChatThrottle();
        var window = TimeSpan.FromMinutes(1);

        for (var i = 0; i < 100; i++)
        {
            throttle.TryReserveOccasion($"greeting Player{i}", Start.AddSeconds(i), window);
        }

        Assert.That(throttle.OccasionCount, Is.EqualTo(100));
        Assert.That(throttle.TryReserveOccasion("greeting Latecomer", Start.AddMinutes(20), window), Is.True);
        Assert.That(throttle.OccasionCount, Is.EqualTo(1));
    }

    /// <summary>
    /// Small talk happens once per interval - a freshly started server waits one interval first,
    /// while the bots log in - and a forced start from the admin panel makes it due at once.
    /// </summary>
    [Test]
    public void MerchantTalkFollowsTheIntervalOrAForcedStart()
    {
        var throttle = new BotChatThrottle();
        var interval = TimeSpan.FromMinutes(10);

        Assert.That(throttle.TryStartMerchantTalk(Start, interval, 0), Is.False);
        Assert.That(throttle.TryStartMerchantTalk(Start.AddMinutes(10), interval, 0), Is.True);
        Assert.That(throttle.TryStartMerchantTalk(Start.AddMinutes(11), interval, 0), Is.False);
        Assert.That(throttle.TryStartMerchantTalk(Start.AddMinutes(11), interval, 1), Is.True);
        Assert.That(throttle.TryStartMerchantTalk(Start.AddMinutes(12), interval, 1), Is.False);
    }

    /// <summary>
    /// Conversations are drawn without repetition: each one comes again only after all of them were
    /// used.
    /// </summary>
    [Test]
    public void ConversationsDontRepeatUntilAllWereUsed()
    {
        var throttle = new BotChatThrottle();
        var conversations = Enumerable.Range(0, 5).Select(i => new BotConversation { Opener = $"opener {i}", Reply = "reply" }).ToList();

        var firstRound = Enumerable.Range(0, 5).Select(_ => throttle.DrawConversation(conversations, _ => 0)).ToList();
        var nextRound = throttle.DrawConversation(conversations, _ => 0);

        Assert.That(firstRound, Is.EquivalentTo(conversations));
        Assert.That(nextRound, Is.Not.Null);
    }

    /// <summary>
    /// A player overhears a conversation once per interval: the first one only after an interval in
    /// the game, the next one an interval after the last.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask ConversationsFollowTheIntervalPerPlayerAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var throttle = new BotChatThrottle();
        var interval = TimeSpan.FromMinutes(5);

        Assert.That(throttle.IsConversationDue(player, Start, interval), Is.False);
        Assert.That(throttle.IsConversationDue(player, Start.AddMinutes(5), interval), Is.True);
        throttle.PostponeConversation(player, Start.AddMinutes(10));
        Assert.That(throttle.IsConversationDue(player, Start.AddMinutes(9), interval), Is.False);
        Assert.That(throttle.IsConversationDue(player, Start.AddMinutes(10), interval), Is.True);
    }

    /// <summary>
    /// The default configuration comes with 150 to 250 conversations for every place, each with a
    /// Polish translation of both texts.
    /// </summary>
    [Test]
    public void DefaultConversationsAreManyAndTranslated()
    {
        var conversations = BotChatConfiguration.CreateDefault().Conversations;
        var polish = CultureInfo.GetCultureInfo("pl");

        Assert.That(conversations, Has.Count.InRange(150, 250));
        Assert.That(conversations.Select(c => c.Where).Distinct().Count(), Is.EqualTo(Enum.GetValues<BotConversationPlace>().Length));
        Assert.That(conversations.All(c => c.Opener.GetOwnTranslation(polish) is not null && c.Reply.GetOwnTranslation(polish) is not null), Is.True);
    }

    /// <summary>
    /// A reset is noticed by the grown reset count of a character - not when the character is seen
    /// for the first time, and not when an admin lowered the count.
    /// </summary>
    [Test]
    public void NoticesAGrownResetCount()
    {
        var throttle = new BotChatThrottle();

        Assert.That(throttle.TryUpdateKnownResets("Hero", 3), Is.False);
        Assert.That(throttle.TryUpdateKnownResets("Hero", 3), Is.False);
        Assert.That(throttle.TryUpdateKnownResets("Hero", 4), Is.True);
        Assert.That(throttle.TryUpdateKnownResets("Hero", 4), Is.False);
        Assert.That(throttle.TryUpdateKnownResets("Hero", 1), Is.False);
        Assert.That(throttle.TryUpdateKnownResets("Hero", 2), Is.True);
    }

    /// <summary>
    /// A bot answers like a typing player: a short "np" comes quickly, a long sentence takes longer -
    /// but never longer than the cap, so the player doesn't give up waiting.
    /// </summary>
    [Test]
    public void TypesLongerForLongerTexts()
    {
        var reaction = TimeSpan.FromSeconds(1);
        var perCharacter = TimeSpan.FromMilliseconds(200);

        Assert.That(BotChatPlugIn.GetTypingTime(2, reaction, perCharacter), Is.EqualTo(TimeSpan.FromMilliseconds(1400)));
        Assert.That(BotChatPlugIn.GetTypingTime(30, reaction, perCharacter), Is.EqualTo(TimeSpan.FromSeconds(7)));
        Assert.That(BotChatPlugIn.GetTypingTime(500, reaction, perCharacter), Is.EqualTo(TimeSpan.FromSeconds(12)));
    }

    /// <summary>
    /// A player who chimes in right after a conversation of two bots near them is answered once; a
    /// message after the window, or far from the bots, is not taken for chiming in.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask APlayerCanChimeInOnceAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var first = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var second = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var throttle = new BotChatThrottle();
        throttle.RememberConversation(first, second, Start.AddSeconds(30), Start);

        Assert.That(throttle.TakeRecentConversation(_ => false, Start.AddSeconds(10)), Is.Null);
        Assert.That(throttle.TakeRecentConversation(bot => bot == second, Start.AddSeconds(10)), Is.Not.Null);
        Assert.That(throttle.TakeRecentConversation(_ => true, Start.AddSeconds(11)), Is.Null);

        throttle.RememberConversation(first, second, Start.AddSeconds(30), Start);
        Assert.That(throttle.TakeRecentConversation(_ => true, Start.AddSeconds(31)), Is.Null);
    }

    /// <summary>
    /// Conversations nobody chimed in to are forgotten when the bots have the next one - on a quiet
    /// server no player's message prunes them.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask ForgetsConversationsNobodyChimedInToAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var first = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var second = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var throttle = new BotChatThrottle();

        for (var minute = 0; minute < 100; minute++)
        {
            var now = Start.AddMinutes(minute);
            throttle.RememberConversation(first, second, now.AddSeconds(30), now);
        }

        Assert.That(throttle.RecentConversationCount, Is.EqualTo(1));
    }

    /// <summary>
    /// A bot remembers the players it was in a party with - and only those.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask RemembersPartyCompanionsAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var throttle = new BotChatThrottle();

        throttle.RememberFriends(bot, ["Hero", "Mage"]);

        Assert.That(throttle.IsFriend(bot, "Hero"), Is.True);
        Assert.That(throttle.IsFriend(bot, "Stranger"), Is.False);
    }

    /// <summary>
    /// Two players searching a party at the same time can't both reserve the same bot: one wins, and
    /// the bot's open offer is the winner's. After the offer expired, the bot is free again.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask APartySearchReservesTheBotOnceAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var requesters = new List<Player>();
        for (var i = 0; i < 8; i++)
        {
            requesters.Add(await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false));
        }

        var throttle = new BotChatThrottle();
        var expires = Start.AddMinutes(2);
        using var startSignal = new ManualResetEventSlim();
        var attempts = requesters
            .Select(requester => Task.Run(() =>
            {
                startSignal.Wait();
                return throttle.TryReservePartyOffer(bot, new BotPartyOffer(requester, expires), Start) ? requester : null;
            }))
            .ToList();
        startSignal.Set();
        var winners = (await Task.WhenAll(attempts).ConfigureAwait(false)).OfType<Player>().ToList();

        Assert.That(winners, Has.Count.EqualTo(1));
        Assert.That(throttle.GetOpenPartyOffer(bot, Start)?.Requester, Is.SameAs(winners[0]));
        Assert.That(throttle.TryReservePartyOffer(bot, new BotPartyOffer(requesters[0], expires.AddMinutes(2)), expires), Is.True);
    }

    /// <summary>
    /// A party line said after the typing time goes only to the party the bot meant to talk to - not to
    /// another party it joined meanwhile.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask SaysAPartyLineOnlyToThePartyItMeantAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var companion = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var party = new Party(new PartyManager(5, new NullLogger<Party>()), 5, new NullLogger<Party>());
        await party.AddAsync(bot).ConfigureAwait(false);
        await party.AddAsync(companion).ConfigureAwait(false);
        var formerParty = new Party(new PartyManager(5, new NullLogger<Party>()), 5, new NullLogger<Party>());
        var view = Mock.Get(companion.ViewPlugIns.GetPlugIn<IChatViewPlugIn>()!);

        await BotChatSpeaker.PartySayAsync(bot, formerParty, new LocalizedString("gg"), NoValues).ConfigureAwait(false);
        view.Verify(v => v.ChatMessageAsync("gg", It.IsAny<string>(), ChatMessageType.Party), Times.Never);

        await BotChatSpeaker.PartySayAsync(bot, party, new LocalizedString("gg"), NoValues).ConfigureAwait(false);
        view.Verify(v => v.ChatMessageAsync("gg", It.IsAny<string>(), ChatMessageType.Party), Times.Once);
    }

    /// <summary>
    /// The chat and whisper reply cooldowns belong to one bot; another bot may still talk, and the
    /// two cooldowns don't block each other.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask CooldownsArePerBotAndPerKindAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var first = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var second = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var throttle = new BotChatThrottle();
        var cooldown = TimeSpan.FromMinutes(2);

        Assert.That(throttle.TryReserveChat(first, Start, cooldown), Is.True);
        Assert.That(throttle.TryReserveChat(first, Start.AddMinutes(1), cooldown), Is.False);
        Assert.That(throttle.TryReserveChat(second, Start.AddMinutes(1), cooldown), Is.True);
        Assert.That(throttle.TryReserveWhisperReply(first, Start.AddMinutes(1), cooldown), Is.True);
        Assert.That(throttle.TryReserveChat(first, Start.AddMinutes(2), cooldown), Is.True);
    }

    /// <summary>
    /// Blank lines are ignored - a list without a usable line stays silent.
    /// </summary>
    [Test]
    public void PicksOnlyUsableLines()
    {
        var lines = new List<BotChatLine>
        {
            new() { Text = " " },
            new() { Text = "{level} finally" },
        };

        var picked = BotChatComposer.PickLine(lines, BotChatTimeOfDay.Day, count => count - 1);
        var silent = BotChatComposer.PickLine(new List<BotChatLine> { new() { Text = string.Empty } }, BotChatTimeOfDay.Day, _ => 0);

        Assert.That(picked?.Text.ValueInNeutralLanguage, Is.EqualTo("{level} finally"));
        Assert.That(silent, Is.Null);
    }

    /// <summary>
    /// A line of a time of day is only said then - "good morning" in the morning - while the lines
    /// for any time are always available.
    /// </summary>
    [Test]
    public void PicksLinesOfTheTimeOfDay()
    {
        var lines = new List<BotChatLine>
        {
            new() { Text = "good morning", When = BotChatTimeOfDay.Morning },
            new() { Text = "late night grind?", When = BotChatTimeOfDay.Night },
        };
        var anytime = new BotChatLine { Text = "hey" };

        Assert.That(BotChatComposer.PickLine(lines, BotChatTimeOfDay.Morning, _ => 0)?.Text.ValueInNeutralLanguage, Is.EqualTo("good morning"));
        Assert.That(BotChatComposer.PickLine(lines, BotChatTimeOfDay.Day, _ => 0), Is.Null);
        Assert.That(BotChatComposer.PickLine(lines.Append(anytime), BotChatTimeOfDay.Day, _ => 0), Is.SameAs(anytime));
    }

    /// <summary>
    /// The hours of the server's local time map to the times of day; the night wraps around midnight.
    /// </summary>
    /// <param name="hour">The hour.</param>
    /// <param name="expected">The expected time of day.</param>
    [TestCase(4, BotChatTimeOfDay.Night)]
    [TestCase(5, BotChatTimeOfDay.Morning)]
    [TestCase(11, BotChatTimeOfDay.Day)]
    [TestCase(18, BotChatTimeOfDay.Evening)]
    [TestCase(23, BotChatTimeOfDay.Night)]
    [TestCase(0, BotChatTimeOfDay.Night)]
    public void KnowsTheTimeOfDay(int hour, BotChatTimeOfDay expected)
    {
        Assert.That(BotChatPlugIn.GetTimeOfDay(hour), Is.EqualTo(expected));
    }

    /// <summary>
    /// Every receiver sees the translation of their own language, with the placeholders filled in
    /// for that language; without a translation, the English text is used.
    /// </summary>
    [Test]
    public void RendersTheLineInTheReceiversLanguage()
    {
        var polish = CultureInfo.GetCultureInfo("pl");
        var text = new LocalizedString("{event} in {map}!").WithTranslation(polish, "{event} na {map}!");
        var values = new Dictionary<string, Func<CultureInfo, string>>
        {
            ["event"] = culture => culture.Name == "pl" ? "Inwazja" : "Invasion",
            ["map"] = _ => "Devias",
        };

        Assert.That(BotChatComposer.Render(text, polish, values), Is.EqualTo("Inwazja na Devias!"));
        Assert.That(BotChatComposer.Render(text, CultureInfo.GetCultureInfo("de"), values), Is.EqualTo("Invasion in Devias!"));
    }

    /// <summary>
    /// A placeholder without a value (e.g. a typo in a configured line) stays visible instead of
    /// silently swallowing text.
    /// </summary>
    [Test]
    public void KeepsUnknownPlaceholders()
    {
        Assert.That(BotChatComposer.Render("{lvl} done", CultureInfo.InvariantCulture, NoValues), Is.EqualTo("{lvl} done"));
    }

    /// <summary>
    /// A place is written the way players write it - the map, then the coordinates - and without
    /// coordinates it's the map alone.
    /// </summary>
    [Test]
    public void FormatsALocationLikePlayersDo()
    {
        Assert.That(BotChatComposer.FormatLocation("Devias", new Point(215, 40)), Is.EqualTo("Devias 215, 40"));
        Assert.That(BotChatComposer.FormatLocation("Devias", null), Is.EqualTo("Devias"));
    }

    /// <summary>
    /// An item is named the way players name it - with its level, but without a "+0".
    /// </summary>
    [Test]
    public void FormatsAnItemLikePlayersDo()
    {
        Assert.That(BotChatComposer.FormatItem("Wings of Soul", 9), Is.EqualTo("Wings of Soul +9"));
        Assert.That(BotChatComposer.FormatItem("Wings of Elf", 0), Is.EqualTo("Wings of Elf"));
    }

    /// <summary>
    /// A player puts the same wings on again after every change of equipment; the bots comment on a
    /// pair only once per character - a new pair, or the same one upgraded, is news again.
    /// </summary>
    [Test]
    public void CommentsOnAPairOfWingsOnce()
    {
        var throttle = new BotChatThrottle();

        Assert.That(throttle.TryRememberWings("Hero", "12/4+9"), Is.True);
        Assert.That(throttle.TryRememberWings("Hero", "12/4+9"), Is.False);
        Assert.That(throttle.TryRememberWings("Hero", "12/4+10"), Is.True);
        Assert.That(throttle.TryRememberWings("Hero", "12/36+0"), Is.True);
        Assert.That(throttle.TryRememberWings("Other", "12/4+9"), Is.True);
    }

    /// <summary>
    /// A whisper is answered by the lines whose keywords match it - ignoring case and punctuation,
    /// preferring the longest match, and matching whole words only. Without a match, a reply
    /// without keywords is used.
    /// </summary>
    /// <param name="message">The whispered message.</param>
    /// <param name="expected">The expected reply.</param>
    [TestCase("How are you?!", "fine, thanks for asking")]
    [TestCase("hi, how are you", "fine, thanks for asking")]
    [TestCase("HI", "hi, what's up?")]
    [TestCase("this is cool", "busy")]
    [TestCase("asdf", "busy")]
    public void AnswersWhispersByKeywords(string message, string expected)
    {
        var replies = new List<BotChatReply>
        {
            new() { Text = "busy" },
            new() { Text = "hi, what's up?", Keywords = "hi, hello" },
            new() { Text = "fine, thanks for asking", Keywords = "how are you, how r u" },
        };

        var reply = BotChatComposer.PickWhisperReply(replies, message, _ => 0);

        Assert.That(reply?.Text.ValueInNeutralLanguage, Is.EqualTo(expected));
    }

    /// <summary>
    /// A player who asks the same twice doesn't get the very same answer twice - unless there is no
    /// other answer.
    /// </summary>
    [Test]
    public void DoesNotRepeatTheLastAnswer()
    {
        var replies = new List<BotChatReply>
        {
            new() { Text = "fine", Keywords = "how are you" },
            new() { Text = "good, you?", Keywords = "how are you" },
            new() { Text = "only one", Keywords = "level" },
        };

        var first = BotChatComposer.PickWhisperReply(replies, "how are you", _ => 0);
        var second = BotChatComposer.PickWhisperReply(replies, "how are you", _ => 0, first);
        var only = BotChatComposer.PickWhisperReply(replies, "level", _ => 0, replies[2]);

        Assert.That(second, Is.Not.SameAs(first));
        Assert.That(only, Is.SameAs(replies[2]));
    }

    /// <summary>
    /// In the party chat, a message without the bot's name is only answered when it matches a
    /// keyword - an "ok" or "let's go" gets no fallback reply.
    /// </summary>
    [Test]
    public void AnswersUnaddressedPartyMessagesOnlyByKeyword()
    {
        var replies = new List<BotChatReply>
        {
            new() { Text = "busy" },
            new() { Text = "lvl 300", Keywords = "level, lvl" },
        };

        Assert.That(BotChatComposer.PickWhisperReply(replies, "ok lets go", _ => 0, requireKeyword: true), Is.Null);
        Assert.That(BotChatComposer.PickWhisperReply(replies, "~what lvl are you?", _ => 0, requireKeyword: true)?.Text.ValueInNeutralLanguage, Is.EqualTo("lvl 300"));
        Assert.That(BotChatComposer.PickWhisperReply(replies, "ok lets go", _ => 0)?.Text.ValueInNeutralLanguage, Is.EqualTo("busy"));
    }

    /// <summary>
    /// The language of a player's message is recognized by its typical words and letters, so the bot
    /// answers in it; a message which doesn't tell leaves it open.
    /// </summary>
    /// <param name="message">The player's message.</param>
    /// <param name="expected">The expected language, or <c>null</c>.</param>
    [TestCase("jak sie masz?", "pl")]
    [TestCase("cześć!", "pl")]
    [TestCase("hi, jak się masz", "pl")]
    [TestCase("how are you?", "en")]
    [TestCase("lol", null)]
    public void RecognizesTheLanguageOfAMessage(string message, string? expected)
    {
        var languages = BotChatConfiguration.CreateDefault().MessageLanguages;

        Assert.That(BotChatComposer.DetectLanguage(message, languages), Is.EqualTo(expected));
    }

    /// <summary>
    /// A bot feels addressed when its name is in a chat message as a whole word - not when it is only
    /// a part of a longer word.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <param name="expected">Whether the message mentions the bot.</param>
    [TestCase("hi Serael!", true)]
    [TestCase("SERAEL how are you", true)]
    [TestCase("serael,where r u", true)]
    [TestCase("Seraelion is strong", false)]
    [TestCase("hello all", false)]
    public void RecognizesTheBotsName(string message, bool expected)
    {
        Assert.That(BotChatComposer.MentionsName(message, "Serael"), Is.EqualTo(expected));
    }

    /// <summary>
    /// The default configuration knows the basic small talk: a "how are you" gets a fitting answer.
    /// </summary>
    [Test]
    public void DefaultRepliesAnswerHowAreYou()
    {
        var configuration = BotChatConfiguration.CreateDefault();

        var reply = BotChatComposer.PickWhisperReply(configuration.WhisperReplies, "hey, how are you?", _ => 0);

        Assert.That(reply?.Keywords, Does.Contain("how are you"));
    }

    /// <summary>
    /// The default configuration answers a question about the bot's wings by naming them - in English
    /// and in Polish, with or without diacritics.
    /// </summary>
    /// <param name="message">The whispered message.</param>
    [TestCase("nice wings!")]
    [TestCase("what wings do you have?")]
    [TestCase("jakie masz skrzydła?")]
    [TestCase("ladne skrzydla")]
    public void DefaultRepliesNameTheWings(string message)
    {
        var configuration = BotChatConfiguration.CreateDefault();

        var reply = BotChatComposer.PickWhisperReply(configuration.WhisperReplies, message, _ => 0);

        Assert.That(reply?.Text.ValueInNeutralLanguage, Does.Contain("{wings}"));
        Assert.That(configuration.WithoutWingsLines.Select(l => l.Text.ValueInNeutralLanguage), Has.None.Contain("{wings}"));
    }

    /// <summary>
    /// The default answers of a merchant point a player who asks what it sells to its store - and leave any
    /// other whisper, like a question about its level, to the usual replies.
    /// </summary>
    /// <param name="message">The whispered message.</param>
    /// <param name="asksForGoods">Whether the message asks what the bot sells.</param>
    [TestCase("what are you selling?", true)]
    [TestCase("how much for the ring?", true)]
    [TestCase("co sprzedajesz?", true)]
    [TestCase("za ile ten miecz?", true)]
    [TestCase("masz coś ciekawego?", true)]
    [TestCase("wtb ring", true)]
    [TestCase("ile masz lvl?", false)]
    [TestCase("hi, how are you?", false)]
    public void DefaultMerchantRepliesPointToTheStore(string message, bool asksForGoods)
    {
        var configuration = BotChatConfiguration.CreateDefault();

        var reply = BotChatComposer.PickWhisperReply(configuration.MerchantWhisperReplies, message, _ => 0, requireKeyword: true);

        Assert.That(reply is not null, Is.EqualTo(asksForGoods), message);
        Assert.That(configuration.MerchantWhisperReplies.Select(r => r.Text.GetTranslation(CultureInfo.GetCultureInfo("pl"))), Has.All.Not.Empty);
    }

    /// <summary>
    /// A merchant never answers like a bot without goods: every trade keyword of the usual replies is a
    /// keyword of the merchant replies too.
    /// </summary>
    [Test]
    public void DefaultMerchantRepliesCoverTheTradeKeywords()
    {
        var configuration = BotChatConfiguration.CreateDefault();
        static IEnumerable<string> Keywords(string? list) => (list ?? string.Empty).Split(',').Select(k => k.Trim()).Where(k => k.Length > 0);
        var tradeKeywords = configuration.WhisperReplies.First(r => Keywords(r.Keywords).Contains("trade")).Keywords;

        Assert.That(Keywords(configuration.MerchantWhisperReplies.First().Keywords), Is.SupersetOf(Keywords(tradeKeywords)));
    }

    /// <summary>
    /// The default lines come with a Polish translation: a Polish player who whispers in Polish gets
    /// a fitting answer in Polish, also when typing without diacritics.
    /// </summary>
    /// <param name="message">The whispered message.</param>
    [TestCase("siema, jak się masz?")]
    [TestCase("jak sie masz")]
    public void DefaultRepliesSpeakPolish(string message)
    {
        var configuration = BotChatConfiguration.CreateDefault();

        var reply = BotChatComposer.PickWhisperReply(configuration.WhisperReplies, message, _ => 0);
        var text = reply is null ? null : BotChatComposer.Render(reply.Text, CultureInfo.GetCultureInfo("pl-PL"), NoValues);

        Assert.That(text, Is.EqualTo("dobrze, dzięki że pytasz"));
    }

    /// <summary>
    /// The default replies understand a player saying goodbye in Polish and English, with and
    /// without diacritics - "muszę kończyć" is not answered with "busy hunting".
    /// </summary>
    /// <param name="message">The whispered message.</param>
    [TestCase("musze konczyc")]
    [TestCase("Muszę kończyć, nara")]
    [TestCase("gtg, cya")]
    [TestCase("ok spadam")]
    public void DefaultRepliesUnderstandGoodbyes(string message)
    {
        var configuration = BotChatConfiguration.CreateDefault();

        var reply = BotChatComposer.PickWhisperReply(configuration.WhisperReplies, message, _ => 0);

        Assert.That(reply?.Keywords, Does.Contain("musze konczyc"));
    }

    /// <summary>
    /// The default lines come with fitting gestures: a wave with a greeting and with a goodbye, a
    /// clap with a "gz". The values are the client's emote animations.
    /// </summary>
    [Test]
    public void DefaultLinesComeWithGestures()
    {
        var configuration = BotChatConfiguration.CreateDefault();

        Assert.That(configuration.GreetingLines.All(line => line.Gesture == BotChatGesture.Greeting), Is.True);
        Assert.That(configuration.CongratulationLines.All(line => line.Gesture == BotChatGesture.Clap), Is.True);
        Assert.That(BotChatComposer.PickWhisperReply(configuration.WhisperReplies, "muszę kończyć", _ => 0)?.Gesture, Is.EqualTo(BotChatGesture.Goodbye));
        Assert.That((byte)BotChatGesture.Greeting, Is.EqualTo(131));
    }

    /// <summary>
    /// The bots call out the golden monsters of the invasions by default - the Golden Goblin is one.
    /// </summary>
    [Test]
    public void DefaultNotableMonstersAreTheGoldenOnes()
    {
        var numbers = BotChatConfiguration.CreateDefault().NotableMonsterNumbers.Split(',').Select(n => n.Trim());

        Assert.That(numbers, Does.Contain("78").And.Contain("43").And.Contain("44"));
    }

    /// <summary>
    /// A server which switches the plugin on has something to say in every situation: each list of
    /// the default configuration has lines.
    /// </summary>
    [Test]
    public void DefaultConfigurationHasLinesForEverySituation()
    {
        var configuration = BotChatConfiguration.CreateDefault();

        foreach (var property in typeof(BotChatConfiguration).GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)))
        {
            Assert.That(property.GetValue(configuration) as System.Collections.ICollection, Is.Not.Empty, property.Name);
        }
    }
}
