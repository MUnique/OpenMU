// <copyright file="CrywolfPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.ComponentModel;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Crywolf;
using MUnique.OpenMU.GameLogic.PlugIns;
using ItemDefinition = MUnique.OpenMU.Persistence.BasicModel.ItemDefinition;

/// <summary>
/// Tests the benefits and penalties of the <see cref="CrywolfPlugIn"/>, which apply on all game servers.
/// </summary>
[TestFixture]
public class CrywolfPlugInTest
{
    /// <summary>
    /// Tests that jewels drop less often while the fortress is occupied, and that other items aren't affected.
    /// </summary>
    [Test]
    public async Task JewelsDropLessOftenWhileOccupiedAsync()
    {
        var (plugIn, gameContext) = await CreatePlugInAsync(isOccupied: true, d => d.JewelDropPenaltyPercentage = 0).ConfigureAwait(false);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);

        Assert.That(IsDropCancelled(plugIn, player, 14, 13), Is.True, "Jewel of Bless");
        Assert.That(IsDropCancelled(plugIn, player, 12, 15), Is.True, "Jewel of Chaos");
        Assert.That(IsDropCancelled(plugIn, player, 14, 15), Is.False, "Zen isn't a jewel");
    }

    /// <summary>
    /// Tests that jewels drop normally while the fortress is in peace.
    /// </summary>
    [Test]
    public async Task JewelsDropNormallyInPeaceAsync()
    {
        var (plugIn, gameContext) = await CreatePlugInAsync(isOccupied: false, d => d.JewelDropPenaltyPercentage = 0).ConfigureAwait(false);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);

        Assert.That(IsDropCancelled(plugIn, player, 14, 13), Is.False);
    }

    /// <summary>
    /// Tests that the experience rate of the players is reduced while the fortress is occupied.
    /// </summary>
    [Test]
    public async Task ExperienceIsReducedWhileOccupiedAsync()
    {
        var (plugIn, gameContext) = await CreatePlugInAsync(isOccupied: true, d => d.ExperiencePenaltyPercentage = 50, execute: false).ConfigureAwait(false);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await plugIn.PlayerStateChangedAsync(player, PlayerState.CharacterSelection, PlayerState.EnteredWorld).ConfigureAwait(false);
        var normalRate = player.Attributes![Stats.ExperienceRate];

        await plugIn.ExecuteTaskAsync(gameContext).ConfigureAwait(false);

        Assert.That(player.Attributes[Stats.ExperienceRate], Is.EqualTo(normalRate * 0.5f));
    }

    /// <summary>
    /// Tests that deactivating the plugin undoes its multipliers.
    /// </summary>
    [Test]
    public async Task DeactivationUndoesTheMultipliersAsync()
    {
        var (plugIn, gameContext) = await CreatePlugInAsync(isOccupied: true, d => d.ExperiencePenaltyPercentage = 50, execute: false).ConfigureAwait(false);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await plugIn.PlayerStateChangedAsync(player, PlayerState.CharacterSelection, PlayerState.EnteredWorld).ConfigureAwait(false);
        var normalRate = player.Attributes![Stats.ExperienceRate];
        await plugIn.ExecuteTaskAsync(gameContext).ConfigureAwait(false);
        Assert.That(player.Attributes[Stats.ExperienceRate], Is.EqualTo(normalRate * 0.5f), "precondition");

        gameContext.PlugInManager.RegisterPlugIn<IPeriodicTaskPlugIn, CrywolfPlugIn>();
        gameContext.PlugInManager.DeactivatePlugIn<CrywolfPlugIn>();

        Assert.That(player.Attributes[Stats.ExperienceRate], Is.EqualTo(normalRate));
    }

    /// <summary>
    /// Tests that players, which are already in the game when the plugin starts, get the multiplier, too.
    /// </summary>
    [Test]
    public async Task PlayersInTheGameGetTheMultiplierAsync()
    {
        var (plugIn, gameContext) = await CreatePlugInAsync(isOccupied: true, d => d.ExperiencePenaltyPercentage = 50, execute: false).ConfigureAwait(false);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);
        var normalRate = player.Attributes![Stats.ExperienceRate];

        await plugIn.ExecuteTaskAsync(gameContext).ConfigureAwait(false);

        Assert.That(player.Attributes[Stats.ExperienceRate], Is.EqualTo(normalRate * 0.5f));
    }

    private static bool IsDropCancelled(CrywolfPlugIn plugIn, Player killer, byte group, short number)
    {
        var item = new MUnique.OpenMU.Persistence.BasicModel.Item { Definition = new ItemDefinition { Group = group, Number = number } };
        var eventArgs = new CancelEventArgs();
        plugIn.ItemDropping(null!, killer, item, eventArgs);
        return eventArgs.Cancel;
    }

    private static async ValueTask<(CrywolfPlugIn PlugIn, GameContext GameContext)> CreatePlugInAsync(bool isOccupied, Action<CrywolfEventDefinition> configure, bool execute = true)
    {
        var gameContext = (GameContext)GameContextTestHelper.CreateGameContext();
        using (var persistenceContext = gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(CrywolfData), false, gameContext.Configuration))
        {
            var data = persistenceContext.CreateNew<CrywolfData>();
            data.IsOccupied = isOccupied;
            data.LastBattleEnd = DateTime.UtcNow;
            await persistenceContext.SaveChangesAsync().ConfigureAwait(false);
        }

        // The map of the event doesn't exist in the test configuration. The occupation state applies anyway.
        var definition = new CrywolfEventDefinition { MapNumber = 999, IsPenaltyActive = true };
        configure(definition);
        var plugIn = new CrywolfPlugIn { Configuration = definition };
        if (execute)
        {
            await plugIn.ExecuteTaskAsync(gameContext).ConfigureAwait(false);
        }

        return (plugIn, gameContext);
    }
}
