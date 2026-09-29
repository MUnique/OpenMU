// <copyright file="RaklionHatcheryGateTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.ComponentModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Raklion;

/// <summary>
/// Tests the entrance of the hatchery of the raklion event through its gate.
/// </summary>
[TestFixture]
public class RaklionHatcheryGateTest
{
    /// <summary>
    /// Tests that the hatchery can't be entered through a gate while it's closed,
    /// like in the original game, where the player stays where it is.
    /// </summary>
    [Test]
    public async Task ClosedHatcheryCantBeEnteredAsync()
    {
        var (context, player, definition) = await CreateAsync().ConfigureAwait(false);
        Assume.That(context.CanEnterHatchery, Is.False, "The event starts closed, until its first tick.");

        var hatchery = new GameMapDefinition { Number = definition.HatcheryMapNumber };

        Assert.That(await context.CanEnterThroughGateAsync(player, hatchery).ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that other maps can still be entered while the hatchery is closed.
    /// </summary>
    [Test]
    public async Task OtherMapsCanBeEnteredAsync()
    {
        var (context, player, _) = await CreateAsync().ConfigureAwait(false);
        var raklion = new GameMapDefinition { Number = 57 };

        Assert.That(await context.CanEnterThroughGateAsync(player, raklion).ConfigureAwait(false), Is.True);
    }

    /// <summary>
    /// Tests that the plugin doesn't deny any gate while the raklion event isn't running on the game context.
    /// </summary>
    [Test]
    public async Task GatesAreNotDeniedWithoutRunningEventAsync()
    {
        var (_, player, definition) = await CreateAsync().ConfigureAwait(false);
        var plugIn = new RaklionPlugIn();
        var eventArgs = new CancelEventArgs();
        var hatcheryGate = new ExitGate { Map = new GameMapDefinition { Number = definition.HatcheryMapNumber } };

        await plugIn.WarpGateEnteringAsync(player, hatcheryGate, eventArgs).ConfigureAwait(false);

        Assert.That(eventArgs.Cancel, Is.False);
    }

    private static async ValueTask<(RaklionContext Context, Player Player, RaklionEventDefinition Definition)> CreateAsync()
    {
        var gameContext = (GameContext)GameContextTestHelper.CreateGameContext();
        var definition = new RaklionEventDefinition();
        var context = new RaklionContext(gameContext, definition);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        return (context, player, definition);
    }
}
