// <copyright file="BotPartySearchTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests the party search of a player which the bots answer (see <see cref="BotChatPlugIn"/>):
/// reading the post, reading the meeting place from the player's whisper, and choosing the bots
/// which answer.
/// </summary>
[TestFixture]
public class BotPartySearchTest
{
    private const string Keywords = "party, pt, grupa";

    private static readonly DateTime Start = new(2026, 11, 4, 20, 0, 0, DateTimeKind.Utc);

    private static readonly BotClassAlias[] Aliases =
    [
        new() { Keywords = "dk, bk, bm", ClassNumber = 4 },
        new() { Keywords = "dw, sm, gm", ClassNumber = 0 },
        new() { Keywords = "elf", ClassNumber = 8 },
    ];

    private static readonly (MiniGameType Type, string Keywords)[] EventKeywords =
    [
        (MiniGameType.BloodCastle, "bc, blood castle"),
        (MiniGameType.DevilSquare, "ds, devil square"),
        (MiniGameType.ChaosCastle, "cc, chaos castle"),
    ];

    private static readonly MiniGameDefinition[] MiniGames =
    [
        new() { Type = MiniGameType.BloodCastle, GameLevel = 7 },
        new() { Type = MiniGameType.BloodCastle, GameLevel = 8 },
        new() { Type = MiniGameType.DevilSquare, GameLevel = 5 },
        new() { Type = MiniGameType.ChaosCastle, GameLevel = 3 },
    ];

    /// <summary>
    /// A post with a party keyword and a level is a party search: a range, a minimum written as
    /// "300+", "min 300" or a single number, and optionally the wanted classes.
    /// </summary>
    /// <param name="message">The posted message.</param>
    /// <param name="minimum">The expected minimum level.</param>
    /// <param name="maximum">The expected maximum level.</param>
    /// <param name="classes">The expected class numbers, comma-separated.</param>
    [TestCase("party 300-350 bk dw", 300, 350, "4,0")]
    [TestCase("PT 350 - 300", 300, 350, "")]
    [TestCase("party 300+ elf", 300, int.MaxValue, "8")]
    [TestCase("grupa min 250", 250, int.MaxValue, "")]
    [TestCase("lf party lvl 380, need BM", 380, int.MaxValue, "4")]
    public void ReadsAPartySearch(string message, int minimum, int maximum, string classes)
    {
        var request = BotPartyRequest.TryParse(message, Keywords, Aliases, EventKeywords, MiniGames);

        Assert.That(request, Is.Not.Null);
        Assert.That(request!.MinimumLevel, Is.EqualTo(minimum));
        Assert.That(request.MaximumLevel, Is.EqualTo(maximum));
        Assert.That(string.Join(",", request.ClassNumbers), Is.EqualTo(classes));
    }

    /// <summary>
    /// A post may name an event with its level instead of a level - written together or apart, short or
    /// in full. The event's digit is no character level; a level given besides the event still counts.
    /// </summary>
    /// <param name="message">The posted message.</param>
    /// <param name="type">The expected event type.</param>
    /// <param name="gameLevel">The expected event level.</param>
    /// <param name="minimum">The expected minimum level.</param>
    /// <param name="classes">The expected class numbers, comma-separated.</param>
    [TestCase("party bc7", MiniGameType.BloodCastle, 7, 0, "")]
    [TestCase("PT BC 8 bm", MiniGameType.BloodCastle, 8, 0, "4")]
    [TestCase("grupa Blood Castle 7", MiniGameType.BloodCastle, 7, 0, "")]
    [TestCase("party ds5 elf", MiniGameType.DevilSquare, 5, 0, "8")]
    [TestCase("party cc3 350+", MiniGameType.ChaosCastle, 3, 350, "")]
    public void ReadsAnEventSearch(string message, MiniGameType type, int gameLevel, int minimum, string classes)
    {
        var request = BotPartyRequest.TryParse(message, Keywords, Aliases, EventKeywords, MiniGames);

        Assert.That(request, Is.Not.Null);
        Assert.That(request!.MiniGame, Is.Not.Null);
        Assert.That(request.MiniGame!.Type, Is.EqualTo(type));
        Assert.That(request.MiniGame.GameLevel, Is.EqualTo(gameLevel));
        Assert.That(request.MinimumLevel, Is.EqualTo(minimum));
        Assert.That(request.MaximumLevel, Is.EqualTo(int.MaxValue));
        Assert.That(string.Join(",", request.ClassNumbers), Is.EqualTo(classes));
    }

    /// <summary>
    /// A level search names no event.
    /// </summary>
    [Test]
    public void ALevelSearchNamesNoEvent()
    {
        Assert.That(BotPartyRequest.TryParse("party 300-350", Keywords, Aliases, EventKeywords, MiniGames)!.MiniGame, Is.Null);
    }

    /// <summary>
    /// An event level which doesn't exist makes no search - its digit is not taken for a character
    /// level either - and an event word must stand on its own.
    /// </summary>
    /// <param name="message">The posted message.</param>
    [TestCase("party bc9")]
    [TestCase("party bc 9")]
    [TestCase("party abc7")]
    [TestCase("bc7 anyone?")]
    public void IgnoresUnknownEvents(string message)
    {
        Assert.That(BotPartyRequest.TryParse(message, Keywords, Aliases, EventKeywords, MiniGames), Is.Null);
    }

    /// <summary>
    /// Without a party keyword, or without a level, a post calls no bot - "thx for the party" is no
    /// search.
    /// </summary>
    /// <param name="message">The posted message.</param>
    [TestCase("thx for the party guys")]
    [TestCase("selling wings 300kk")]
    public void IgnoresOtherPosts(string message)
    {
        Assert.That(BotPartyRequest.TryParse(message, Keywords, Aliases, EventKeywords, MiniGames), Is.Null);
    }

    /// <summary>
    /// The meeting place is the map named in the whisper - in any of its translations, the longest
    /// name winning - and the first two numbers after the name as the coordinates.
    /// </summary>
    /// <param name="message">The whisper.</param>
    /// <param name="expectedMap">The expected map number.</param>
    /// <param name="expectedX">The expected x coordinate, or -1 for none.</param>
    /// <param name="expectedY">The expected y coordinate, or -1 for none.</param>
    [TestCase("devias 215 40", 2, 215, 40)]
    [TestCase("Devias, 215,40 pls", 2, 215, 40)]
    [TestCase("come to lost tower", 4, -1, -1)]
    [TestCase("kalima 1 at 20 30", 24, 20, 30)]
    [TestCase("chodź: Wieża 100 120", 4, 100, 120)]
    public void ReadsTheMeetingPlace(string message, int expectedMap, int expectedX, int expectedY)
    {
        var place = BotChatComposer.FindPlace(message, CreateMaps(), []);

        Assert.That(place, Is.Not.Null);
        Assert.That(place!.Value.Map.Number, Is.EqualTo(expectedMap));
        Point? expectedPosition = expectedX < 0 ? null : new Point((byte)expectedX, (byte)expectedY);
        Assert.That(place.Value.Position, Is.EqualTo(expectedPosition));
    }

    /// <summary>
    /// A whisper without a known map names no meeting place.
    /// </summary>
    [Test]
    public void NoMapNoPlace()
    {
        Assert.That(BotChatComposer.FindPlace("somewhere 10 10", CreateMaps(), []), Is.Null);
    }

    /// <summary>
    /// A class line is followed back to its first class, so a Blade Master counts as a Dark Knight.
    /// </summary>
    [Test]
    public void FindsTheFirstClassOfALine()
    {
        var bladeMaster = new CharacterClass { Number = 7 };
        var bladeKnight = new CharacterClass { Number = 6, NextGenerationClass = bladeMaster };
        var darkKnight = new CharacterClass { Number = 4, NextGenerationClass = bladeKnight };
        var darkWizard = new CharacterClass { Number = 0 };
        var classes = new List<CharacterClass> { darkWizard, bladeMaster, bladeKnight, darkKnight };

        Assert.That(BotChatPlugIn.GetFirstClassOfLine(bladeMaster, classes), Is.SameAs(darkKnight));
        Assert.That(BotChatPlugIn.GetFirstClassOfLine(darkWizard, classes), Is.SameAs(darkWizard));
    }

    /// <summary>
    /// A search is answered by up to the wanted number of bots; the same search posted again is
    /// answered by the others.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask AnotherSearchCallsOtherBotsAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var requester = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var bots = new List<GameLogic.Player>();
        for (var i = 0; i < 7; i++)
        {
            bots.Add(await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false));
        }

        var throttle = new BotChatThrottle();

        var first = throttle.TakeNewVolunteers(requester, bots, 5, Start, _ => 0);
        var second = throttle.TakeNewVolunteers(requester, bots, 5, Start.AddMinutes(1), _ => 0);
        var afterAWhile = throttle.TakeNewVolunteers(requester, bots, 5, Start.AddHours(1), _ => 0);

        Assert.That(first, Has.Count.EqualTo(5));
        Assert.That(second, Has.Count.EqualTo(2));
        Assert.That(second.Intersect(first), Is.Empty);
        Assert.That(afterAWhile, Has.Count.EqualTo(5));
    }

    private static List<GameMapDefinition> CreateMaps() =>
    [
        new() { Number = 0, Name = "Lorencia" },
        new() { Number = 2, Name = "Devias" },
        new() { Number = 4, Name = new LocalizedString("Lost Tower").WithTranslation(CultureInfo.GetCultureInfo("pl"), "Wieża") },
        new() { Number = 5, Name = "Tower" },
        new() { Number = 24, Name = "Kalima 1" },
    ];
}
