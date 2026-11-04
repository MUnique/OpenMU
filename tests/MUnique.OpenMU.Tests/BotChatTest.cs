// <copyright file="BotChatTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Bots;
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
    /// The default configuration comes with 50 to 100 conversations for every place, each with a
    /// Polish translation of both texts.
    /// </summary>
    [Test]
    public void DefaultConversationsAreManyAndTranslated()
    {
        var conversations = BotChatConfiguration.CreateDefault().Conversations;
        var polish = CultureInfo.GetCultureInfo("pl");

        Assert.That(conversations, Has.Count.InRange(50, 100));
        Assert.That(conversations.Select(c => c.Where).Distinct().Count(), Is.EqualTo(Enum.GetValues<BotConversationPlace>().Length));
        Assert.That(conversations.All(c => c.Opener.GetOwnTranslation(polish) is not null && c.Reply.GetOwnTranslation(polish) is not null), Is.True);
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

        var picked = BotChatComposer.PickLine(lines, count => count - 1);
        var silent = BotChatComposer.PickLine(new List<BotChatLine> { new() { Text = string.Empty } }, _ => 0);

        Assert.That(picked?.Text.ValueInNeutralLanguage, Is.EqualTo("{level} finally"));
        Assert.That(silent, Is.Null);
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
