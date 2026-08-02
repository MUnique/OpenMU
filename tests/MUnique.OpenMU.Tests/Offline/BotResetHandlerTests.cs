// <copyright file="BotResetHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.Resets;

/// <summary>
/// Tests for <see cref="BotResetHandler"/>.
/// </summary>
[TestFixture]
public class BotResetHandlerTests
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
    /// Without the reset feature the effective level is simply the character level.
    /// </summary>
    [Test]
    public async ValueTask EffectiveLevelWithoutResetFeatureIsPlainLevelAsync()
    {
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
        player.Attributes![Stats.Level] = 123;

        Assert.That(BotResetHandler.GetEffectiveLevel(player), Is.EqualTo(123));
    }

    /// <summary>
    /// With the reset feature every reset counts as the configured level span. Uses the same plain
    /// test context as <see cref="ResetCharacterActionTest"/>, where the added feature plugin is the
    /// effective one (the offline helper's context discovers the real, disabled-by-default plugin).
    /// </summary>
    [Test]
    public async ValueTask EffectiveLevelCountsResetsAsLevelSpansAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.FeaturePlugIns.AddPlugIn(new ResetFeaturePlugIn { Configuration = new ResetConfiguration { RequiredLevel = 400 } }, true);
        player.Attributes![Stats.Level] = 50;
        player.Attributes[Stats.Resets] = 3;

        Assert.That(BotResetHandler.GetEffectiveLevel(player), Is.EqualTo((3 * 400) + 50));
    }

    /// <summary>
    /// A due reset raises the reset count, drops the level and grants the configured points, without
    /// consuming any costs when the bot doesn't pay them.
    /// </summary>
    [Test]
    public async ValueTask TryResetPerformsResetAndSkipsCostsAsync()
    {
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
        var configuration = new ResetConfiguration
        {
            RequiredLevel = 400,
            LevelAfterReset = 10,
            RequiredMoney = 500,
            MultiplyRequiredMoneyByResetCount = false,
            PointsPerReset = 1000,
            MultiplyPointsByResetCount = false,
            ReplacePointsPerReset = true,
            ResetStats = false,
            MoveHome = false,
            LogOut = false,
        };
        this._gameContext.FeaturePlugIns.AddPlugIn(new ResetFeaturePlugIn { Configuration = configuration }, true);
        player.Attributes![Stats.Level] = 400;
        player.Money = 100; // less than the required zen - must not matter for a non-paying bot

        var performed = await BotResetHandler.TryResetAsync(player, configuration, payCosts: false).ConfigureAwait(false);

        Assert.That(performed, Is.True);
        Assert.That((int)player.Attributes[Stats.Resets], Is.EqualTo(1));
        Assert.That((int)player.Attributes[Stats.Level], Is.EqualTo(10));
        Assert.That(player.SelectedCharacter!.LevelUpPoints, Is.EqualTo(1000));
        Assert.That(player.SelectedCharacter.Experience, Is.EqualTo(0));
        Assert.That(player.Money, Is.EqualTo(100));
    }

    /// <summary>
    /// A reset is not performed below the required level or beyond the reset limit.
    /// </summary>
    [Test]
    public async ValueTask TryResetRespectsLevelAndLimitAsync()
    {
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
        var configuration = new ResetConfiguration { RequiredLevel = 400, ResetLimit = 2, MoveHome = false, LogOut = false };
        this._gameContext.FeaturePlugIns.AddPlugIn(new ResetFeaturePlugIn { Configuration = configuration }, true);

        player.Attributes![Stats.Level] = 399;
        Assert.That(await BotResetHandler.TryResetAsync(player, configuration, payCosts: false).ConfigureAwait(false), Is.False);

        player.Attributes[Stats.Level] = 400;
        player.Attributes[Stats.Resets] = 2;
        Assert.That(await BotResetHandler.TryResetAsync(player, configuration, payCosts: false).ConfigureAwait(false), Is.False);
        Assert.That((int)player.Attributes[Stats.Resets], Is.EqualTo(2));
    }
}
