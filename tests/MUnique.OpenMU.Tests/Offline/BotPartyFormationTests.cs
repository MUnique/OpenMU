// <copyright file="BotPartyFormationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Tests for <see cref="BotPartyComposer"/>: one buffer per party,
/// one of each build at most, buffers never left solo when placeable.
/// </summary>
[TestFixture]
public class BotPartyFormationTests
{
    private IGameContext _gameContext = null!;

    /// <summary>
    /// Sets up a fresh game context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
    }

    /// <summary>
    /// Tests that every buffer is placed with exactly one buffer
    /// and unique build keys per party.
    /// </summary>
    [Test]
    public async ValueTask BuffersLeadAndAllArePlacedAsync()
    {
        var buffers = new[]
        {
            await this.CreateBotAsync("A", 8, 40).ConfigureAwait(false),
            await this.CreateBotAsync("C", 8, 41).ConfigureAwait(false),
        };
        var others = new[]
        {
            await this.CreateBotAsync("B", 8, 45).ConfigureAwait(false),
            await this.CreateBotAsync("D", 8, 45).ConfigureAwait(false),
            await this.CreateBotAsync("F", 8, 46).ConfigureAwait(false),
            await this.CreateBotAsync("H", 4, 45).ConfigureAwait(false),
            await this.CreateBotAsync("J", 4, 46).ConfigureAwait(false),
            await this.CreateBotAsync("L", 4, 47).ConfigureAwait(false),
        };
        var candidates = buffers.Concat(others).OrderBy(BotResetHandler.GetEffectiveLevel).ToList();

        var formation = BotPartyComposer.Compose(candidates);

        Assert.That(formation.UnplacedBuffers, Is.Empty);
        Assert.That(formation.Parties, Has.Count.EqualTo(2));
        foreach (var party in formation.Parties)
        {
            Assert.That(party.Count(BotBuild.IsSupportElf), Is.EqualTo(1));
            Assert.That(party.Select(BotBuild.GetBuildKey).Distinct().Count(), Is.EqualTo(party.Count));
        }
    }

    /// <summary>
    /// Tests that no party forms without a buffer.
    /// </summary>
    [Test]
    public async ValueTask NoBufferMeansNoPartyAsync()
    {
        var candidates = new[]
        {
            await this.CreateBotAsync("H", 4, 50).ConfigureAwait(false),
            await this.CreateBotAsync("J", 4, 51).ConfigureAwait(false),
            await this.CreateBotAsync("L", 4, 52).ConfigureAwait(false),
        };

        for (var i = 0; i < 5; i++)
        {
            var formation = BotPartyComposer.Compose(candidates);

            Assert.That(formation.Parties, Is.Empty);
            Assert.That(formation.UnplacedBuffers, Is.Empty);
        }
    }

    /// <summary>
    /// Tests that duplicate build keys never share a party.
    /// </summary>
    [Test]
    public async ValueTask DuplicateBuildKeysNeverSharePartyAsync()
    {
        var archer1 = await this.CreateBotAsync("B", 8, 45).ConfigureAwait(false);
        var archer2 = await this.CreateBotAsync("D", 8, 45).ConfigureAwait(false);
        var candidates = new OfflinePlayer[]
        {
            await this.CreateBotAsync("A", 8, 40).ConfigureAwait(false),
            await this.CreateBotAsync("H", 4, 45).ConfigureAwait(false),
            archer1,
            archer2,
        };

        for (var i = 0; i < 10; i++)
        {
            var formation = BotPartyComposer.Compose(candidates);

            foreach (var party in formation.Parties)
            {
                Assert.That(party.Count(m => ReferenceEquals(m, archer1) || ReferenceEquals(m, archer2)), Is.LessThanOrEqualTo(1));
                Assert.That(party.Count(BotBuild.IsSupportElf), Is.EqualTo(1));
            }
        }
    }

    /// <summary>
    /// Tests that party span never exceeds the level gap.
    /// </summary>
    [Test]
    public async ValueTask PartySpanNeverExceedsLevelGapAsync()
    {
        var candidates = new[]
        {
            await this.CreateBotAsync("A", 8, 30).ConfigureAwait(false),
            await this.CreateBotAsync("C", 8, 31).ConfigureAwait(false),
            await this.CreateBotAsync("H", 4, 35).ConfigureAwait(false),
            await this.CreateBotAsync("J", 4, 42).ConfigureAwait(false),
            await this.CreateBotAsync("L", 12, 47).ConfigureAwait(false),
            await this.CreateBotAsync("N", 12, 54).ConfigureAwait(false),
            await this.CreateBotAsync("P", 0, 60).ConfigureAwait(false),
        }.OrderBy(BotResetHandler.GetEffectiveLevel).ToList();

        for (var i = 0; i < 10; i++)
        {
            var formation = BotPartyComposer.Compose(candidates);

            foreach (var party in formation.Parties)
            {
                var levels = party.Select(BotResetHandler.GetEffectiveLevel).ToList();
                Assert.That(levels.Max() - levels.Min(), Is.LessThanOrEqualTo(BotPartyPolicy.MaxLevelGap));
                Assert.That(party.Count(BotBuild.IsSupportElf), Is.EqualTo(1));
            }
        }
    }

    private async ValueTask<OfflinePlayer> CreateBotAsync(string name, byte classNumber, int level)
    {
        var donor = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var bot = new OfflinePlayer(this._gameContext) { Account = donor.Account };
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        await bot.SetSelectedCharacterAsync(donor.SelectedCharacter!).ConfigureAwait(false);
        donor.SelectedCharacter!.Name = name;
        donor.SelectedCharacter.CharacterClass = new CharacterClass { Number = classNumber };
        bot.Attributes![Stats.Level] = level;
        return bot;
    }
}
