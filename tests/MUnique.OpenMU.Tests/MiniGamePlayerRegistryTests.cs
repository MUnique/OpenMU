// <copyright file="MiniGamePlayerRegistryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

/// <summary>
/// Tests for the <see cref="MiniGamePlayerRegistry"/> entering policy: entering is only
/// possible while the game is open and not full, atomically with state transitions.
/// </summary>
[TestFixture]
public class MiniGamePlayerRegistryTests
{
    /// <summary>
    /// Tests that entering works while the game is open.
    /// </summary>
    [Test]
    public async Task EnterWhileOpenSucceedsAsync()
    {
        var registry = new MiniGamePlayerRegistry(this.CreateDefinition());

        var result = await registry.TryEnterAsync(null!, _ => ValueTask.FromResult(true)).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(EnterResult.Success));
    }

    /// <summary>
    /// Tests that entering fails once the entrance has been closed.
    /// </summary>
    [Test]
    public async Task EnterAfterCloseIsRejectedAsync()
    {
        var registry = new MiniGamePlayerRegistry(this.CreateDefinition());
        await registry.SetStateAsync(MiniGameState.Closed).ConfigureAwait(false);

        var result = await registry.TryEnterAsync(null!, _ => ValueTask.FromResult(true)).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(EnterResult.NotOpen));
    }

    /// <summary>
    /// Tests that entering fails when the game is full.
    /// </summary>
    [Test]
    public async Task EnterWhenFullIsRejectedAsync()
    {
        var registry = new MiniGamePlayerRegistry(this.CreateDefinition());
        Assert.That(await registry.TryEnterAsync(null!, _ => ValueTask.FromResult(true)).ConfigureAwait(false), Is.EqualTo(EnterResult.Success));

        var result = await registry.TryEnterAsync(null!, _ => ValueTask.FromResult(true)).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(EnterResult.Full));
    }

    private MiniGameDefinition CreateDefinition()
    {
        return new MiniGameDefinition { MaximumPlayerCount = 1 };
    }
}
