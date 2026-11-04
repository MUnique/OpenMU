// <copyright file="BotChatTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.Interfaces;

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
    /// Only lines of the requested situation are picked, and blank lines are ignored - a situation
    /// without a usable line stays silent.
    /// </summary>
    [Test]
    public void PicksOnlyUsableLinesOfTheSituation()
    {
        var lines = new List<BotChatLine>
        {
            new() { Situation = BotChatSituation.WhisperReply, Text = "busy" },
            new() { Situation = BotChatSituation.LevelMilestone, Text = " " },
            new() { Situation = BotChatSituation.LevelMilestone, Text = "{level} finally" },
        };

        var picked = BotChatComposer.PickLine(lines, BotChatSituation.LevelMilestone, count => count - 1);
        var silent = BotChatComposer.PickLine(lines, BotChatSituation.InvasionStarted, _ => 0);

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
        var line = new BotChatLine
        {
            Situation = BotChatSituation.InvasionStarted,
            Text = new LocalizedString("{event} in {map}!").WithTranslation(polish, "{event} na {map}!"),
        };
        var values = new Dictionary<string, Func<CultureInfo, string>>
        {
            ["event"] = culture => culture.Name == "pl" ? "Inwazja" : "Invasion",
            ["map"] = _ => "Devias",
        };

        Assert.That(BotChatComposer.Render(line, polish, values), Is.EqualTo("Inwazja na Devias!"));
        Assert.That(BotChatComposer.Render(line, CultureInfo.GetCultureInfo("de"), values), Is.EqualTo("Invasion in Devias!"));
    }

    /// <summary>
    /// A placeholder without a value (e.g. a typo in a configured line) stays visible instead of
    /// silently swallowing text.
    /// </summary>
    [Test]
    public void KeepsUnknownPlaceholders()
    {
        var line = new BotChatLine { Situation = BotChatSituation.LevelMilestone, Text = "{lvl} done" };

        Assert.That(BotChatComposer.Render(line, CultureInfo.InvariantCulture, NoValues), Is.EqualTo("{lvl} done"));
    }

    /// <summary>
    /// A server which switches the plugin on has something to say in every situation.
    /// </summary>
    [Test]
    public void DefaultConfigurationHasLinesForEverySituation()
    {
        var configuration = BotChatConfiguration.CreateDefault();

        foreach (var situation in Enum.GetValues<BotChatSituation>())
        {
            Assert.That(configuration.Lines.Any(line => line.Situation == situation), Is.True, situation.ToString());
        }
    }
}
