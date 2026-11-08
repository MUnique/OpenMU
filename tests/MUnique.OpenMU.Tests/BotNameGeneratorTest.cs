// <copyright file="BotNameGeneratorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Moq;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Tests the <see cref="BotNameGenerator"/>: every style yields valid names without digits,
/// the styles are mixed, and a large population still gets unique names.
/// </summary>
[TestFixture]
public class BotNameGeneratorTest
{
    /// <summary>
    /// The default character name rule of the game configuration, narrowed to letters only.
    /// </summary>
    private const string ValidBotName = "^[a-zA-Z]{3,10}$";

    /// <summary>
    /// Every style produces names within the character name rules and without digits.
    /// </summary>
    [Test]
    public void EveryStyleProducesValidNames()
    {
        var generator = new BotNameGenerator();
        foreach (var style in Enum.GetValues<BotNameGenerator.NameStyle>())
        {
            for (var i = 0; i < 2000; i++)
            {
                Assert.That(generator.BuildName(style), Does.Match(ValidBotName), $"Invalid {style} name");
            }
        }
    }

    /// <summary>
    /// Once the population is crowded, the fallback names stay valid and still contain no digits.
    /// </summary>
    [Test]
    public void CrowdedAttemptsProduceValidNames()
    {
        var generator = new BotNameGenerator();
        for (var attempt = 0; attempt < 500; attempt++)
        {
            Assert.That(generator.BuildName(attempt), Does.Match(ValidBotName));
        }
    }

    /// <summary>
    /// All styles are drawn, roughly with their weights (40 / 30 / 30 %).
    /// </summary>
    [Test]
    public void StylesAreMixed()
    {
        var generator = new BotNameGenerator();
        const int draws = 10_000;
        var counts = Enumerable.Range(0, draws)
            .Select(_ => generator.PickStyle())
            .GroupBy(s => s)
            .ToDictionary(g => g.Key, g => g.Count());

        Assert.That(counts[BotNameGenerator.NameStyle.Fantasy], Is.InRange(3500, 4500));
        Assert.That(counts[BotNameGenerator.NameStyle.Compound], Is.InRange(2500, 3500));
        Assert.That(counts[BotNameGenerator.NameStyle.Handle], Is.InRange(2500, 3500));
    }

    /// <summary>
    /// The largest configurable population (1000 accounts with 5 characters) gets unique names,
    /// compared case-insensitively like the bot generator does.
    /// </summary>
    [Test]
    public async ValueTask LargePopulationGetsUniqueNamesAsync()
    {
        var context = new Mock<IPlayerContext>();
        context
            .Setup(c => c.GetAccountByCharacterNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult<Account?>(null));
        var generator = new BotNameGenerator();
        var reserved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var names = new List<string>();
        for (var i = 0; i < 5000; i++)
        {
            names.Add(await generator.GenerateUniqueAsync(context.Object, reserved).ConfigureAwait(false));
        }

        Assert.That(names.Distinct(StringComparer.OrdinalIgnoreCase).Count(), Is.EqualTo(names.Count));
        Assert.That(names, Has.All.Match(ValidBotName));
    }

    /// <summary>
    /// A name taken in the database is never handed out.
    /// </summary>
    [Test]
    public async ValueTask SkipsNamesTakenInDatabaseAsync()
    {
        var taken = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var context = new Mock<IPlayerContext>();
        context
            .Setup(c => c.GetAccountByCharacterNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((name, _) =>
            {
                // Every second candidate is "taken" by a player.
                var isTaken = taken.Count % 2 == 0;
                if (isTaken)
                {
                    taken.Add(name);
                }

                return ValueTask.FromResult(isTaken ? new Account() : null);
            });
        var generator = new BotNameGenerator();

        var name = await generator.GenerateUniqueAsync(context.Object, new HashSet<string>()).ConfigureAwait(false);

        Assert.That(taken, Does.Not.Contain(name));
    }

    /// <summary>
    /// The decorations of a handle don't make another name: they all have the same root.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="root">The expected root.</param>
    [TestCase("Hex", "hex")]
    [TestCase("Hexx", "hex")]
    [TestCase("xHexx", "hex")]
    [TestCase("Boltt", "bolt")]
    [TestCase("Boltz", "bolt")]
    [TestCase("iBlaze", "blaze")]
    [TestCase("xBlazex", "blaze")]
    [TestCase("MrKira", "kira")]
    [TestCase("TheHugo", "hugo")]
    [TestCase("LilLuna", "luna")]
    [TestCase("Sakuraa", "sakura")]
    [TestCase("Ithwyn", "ithwyn")]
    [TestCase("Xanoth", "xanoth")]
    public void RootIgnoresTheDecorations(string name, string root)
    {
        Assert.That(BotNameGenerator.GetRoot(name), Is.EqualTo(root));
    }

    /// <summary>
    /// A name which looks nearly the same as one handed out before is rejected - the pairs which were
    /// found in a population of 350 bots.
    /// </summary>
    /// <param name="existing">The name handed out before.</param>
    /// <param name="candidate">The new name candidate.</param>
    /// <returns>The task.</returns>
    [TestCase("Hex", "Hexx")]
    [TestCase("Blaze", "xBlazex")]
    [TestCase("Bolt", "Boltz")]
    [TestCase("Raven", "Ravenn")]
    [TestCase("Goromir", "Noromir")]
    [TestCase("Karelgar", "Varelgar")]
    [TestCase("Brodan", "Broaedan")]
    [TestCase("Loraeth", "Loroeth")]
    [TestCase("Rex", "Vex")]
    [TestCase("Rexx", "xHexx")]
    [TestCase("Fenelis", "Fenelaelis")]
    [TestCase("DeadFang", "MadFang")]
    [TestCase("ShadowAxe", "ShadowBane")]
    [TestCase("StarBorn", "StormBorn")]
    [TestCase("Akira", "Sakura")]
    public async ValueTask NearlyTheSameNameIsRejectedAsync(string existing, string candidate)
    {
        var generator = await CreateGeneratorWithAsync(existing).ConfigureAwait(false);

        Assert.That(generator.IsTooSimilar(candidate), Is.True);
    }

    /// <summary>
    /// Different words stay allowed: short handles two letters apart, and compound names without a shared word.
    /// </summary>
    /// <param name="existing">The name handed out before.</param>
    /// <param name="candidate">The new name candidate.</param>
    /// <returns>The task.</returns>
    [TestCase("Hex", "Max")]
    [TestCase("Kira", "Mika")]
    [TestCase("DeadShot", "StormFang")]
    [TestCase("Brodan", "Karelgar")]
    public async ValueTask DifferentNameIsAllowedAsync(string existing, string candidate)
    {
        var generator = await CreateGeneratorWithAsync(existing).ConfigureAwait(false);

        Assert.That(generator.IsTooSimilar(candidate), Is.False);
    }

    /// <summary>
    /// A population like the one of a lab server (70 accounts with 5 characters) has no two names with
    /// the same root, none one letter apart, none of five or more letters two letters apart, and no
    /// word used by two compound names.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask PopulationHasNoNearlyTheSameNamesAsync()
    {
        var generator = new BotNameGenerator();
        var reserved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var names = new List<string>();
        for (var i = 0; i < 350; i++)
        {
            names.Add(await generator.GenerateUniqueAsync(CreateEmptyContext(), reserved).ConfigureAwait(false));
        }

        Assert.That(names.Select(BotNameGenerator.GetRoot).Distinct().Count(), Is.EqualTo(names.Count));
        var nearlyTheSame = names
            .SelectMany((a, i) => names.Skip(i + 1).Select(b => (a, b)))
            .Where(p => Distance(p.a, p.b) <= (Math.Min(p.a.Length, p.b.Length) >= 5 ? 2 : 1))
            .ToList();
        Assert.That(nearlyTheSame, Is.Empty);
        var compoundWords = names
            .Where(n => n.Skip(1).Any(char.IsUpper))
            .SelectMany(n => new[] { n[..n.IndexOf(n.Skip(1).First(char.IsUpper), 1)], n[n.IndexOf(n.Skip(1).First(char.IsUpper), 1)..] })
            .ToList();
        Assert.That(compoundWords.Distinct(StringComparer.OrdinalIgnoreCase).Count(), Is.EqualTo(compoundWords.Count));
    }

    private static ValueTask<BotNameGenerator> CreateGeneratorWithAsync(string existing)
    {
        // Only the given name is indexed: handing out a random name too could make the candidate similar to it.
        var generator = new BotNameGenerator();
        generator.IndexNames([existing]);
        return ValueTask.FromResult(generator);
    }

    private static IPlayerContext CreateEmptyContext()
    {
        var context = new Mock<IPlayerContext>();
        context
            .Setup(c => c.GetAccountByCharacterNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult<Account?>(null));
        return context.Object;
    }

    private static int Distance(string a, string b)
    {
        a = a.ToLowerInvariant();
        b = b.ToLowerInvariant();
        var previous = Enumerable.Range(0, b.Length + 1).ToArray();
        for (var i = 1; i <= a.Length; i++)
        {
            var current = new int[b.Length + 1];
            current[0] = i;
            for (var j = 1; j <= b.Length; j++)
            {
                current[j] = Math.Min(Math.Min(previous[j] + 1, current[j - 1] + 1), previous[j - 1] + (a[i - 1] == b[j - 1] ? 0 : 1));
            }

            previous = current;
        }

        return previous[b.Length];
    }
}
