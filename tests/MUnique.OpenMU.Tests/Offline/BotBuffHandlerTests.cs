// <copyright file="BotBuffHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using NUnit.Framework;

/// <summary>
/// Tests for <see cref="BotBuffHandler"/>.
/// </summary>
[TestFixture]
public class BotBuffHandlerTests
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
    /// Checks that <see cref="BotBuffHandler.IsBuffNpc"/> identifies buff NPCs correctly.
    /// </summary>
    [Test]
    public void IsBuffNpcIdentifiesBuffNpcCorrectly()
    {
        var nonBuffNpc = new MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition { NpcWindow = NpcWindow.NpcDialog };
        Assert.That(BotBuffHandler.IsBuffNpc(nonBuffNpc), Is.False);

        var merchantNpc = new MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition { NpcWindow = NpcWindow.Merchant };
        Assert.That(BotBuffHandler.IsBuffNpc(merchantNpc), Is.False);

        var buffNpc = new MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition { NpcWindow = NpcWindow.NpcDialog };
        buffNpc.Buffs.Add(new MUnique.OpenMU.Persistence.BasicModel.Buff());
        Assert.That(BotBuffHandler.IsBuffNpc(buffNpc), Is.True);
    }

    /// <summary>
    /// Tests that <see cref="BotBuffHandler.TryGetApplicableBuff"/> returns null when configuration has no buff NPCs.
    /// </summary>
    [Test]
    public async ValueTask TryGetApplicableBuffReturnsNullWhenNoBuffNpcConfiguredAsync()
    {
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
        var buff = BotBuffHandler.TryGetApplicableBuff(player);
        Assert.That(buff, Is.Null);
    }

    /// <summary>
    /// Tests that <see cref="BotBuffHandler.FindBuffNpcPosition"/> returns null when no live NPC exists on the map.
    /// </summary>
    [Test]
    public async ValueTask FindBuffNpcPositionReturnsNullWhenNoNpcOnMapAsync()
    {
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
        if (player.CurrentMap is { } map)
        {
            var pos = BotBuffHandler.FindBuffNpcPosition(player, map);
            Assert.That(pos, Is.Null);
        }
    }

    /// <summary>
    /// Tests that <see cref="BotBuffHandler.HasEffect"/> returns false when the player has no active effects.
    /// </summary>
    [Test]
    public async ValueTask HasEffectReturnsFalseWhenPlayerHasNoEffectsAsync()
    {
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
        var effectDef = new MagicEffectDefinition { Number = 1 };

        Assert.That(BotBuffHandler.HasEffect(player, effectDef), Is.False);
    }

    /// <summary>
    /// Tests that <see cref="BotBuffHandler.HasEffect"/> returns true when the player has the specified effect active.
    /// </summary>
    [Test]
    public async ValueTask HasEffectReturnsTrueWhenEffectIsActiveAsync()
    {
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
        var effectDef = new MagicEffectDefinition { Number = 1 };
        var effect = new MagicEffect(TimeSpan.FromMinutes(10), effectDef);
        await player.MagicEffectList.AddEffectAsync(effect).ConfigureAwait(false);

        Assert.That(BotBuffHandler.HasEffect(player, effectDef), Is.True);
    }

    /// <summary>
    /// Tests that <see cref="BotBuffHandler.HasEffect"/> returns false when the player has a different active effect.
    /// </summary>
    [Test]
    public async ValueTask HasEffectReturnsFalseWhenDifferentEffectIsActiveAsync()
    {
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
        var effectDef1 = new MagicEffectDefinition { Number = 1 };
        var effectDef2 = new MagicEffectDefinition { Number = 2 };
        var effect = new MagicEffect(TimeSpan.FromMinutes(10), effectDef1);
        await player.MagicEffectList.AddEffectAsync(effect).ConfigureAwait(false);

        Assert.That(BotBuffHandler.HasEffect(player, effectDef2), Is.False);
    }
}
