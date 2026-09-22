// <copyright file="MiniGamePlayerRegistryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.Persistence.InMemory;

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

        var result = await registry.TryEnterAsync(CreatePlayer(), _ => ValueTask.FromResult(true)).ConfigureAwait(false);

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

        var result = await registry.TryEnterAsync(CreatePlayer(), _ => ValueTask.FromResult(true)).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(EnterResult.NotOpen));
    }

    /// <summary>
    /// Tests that entering fails when the game is full with other players.
    /// </summary>
    [Test]
    public async Task EnterWhenFullIsRejectedAsync()
    {
        var registry = new MiniGamePlayerRegistry(this.CreateDefinition());
        Assert.That(await registry.TryEnterAsync(CreatePlayer(), _ => ValueTask.FromResult(true)).ConfigureAwait(false), Is.EqualTo(EnterResult.Success));

        var result = await registry.TryEnterAsync(CreatePlayer(), _ => ValueTask.FromResult(true)).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(EnterResult.Full));
    }

    private static Player CreatePlayer()
    {
        var contextMock = new Mock<IGameContext>();
        contextMock.SetupGet(c => c.LoggerFactory).Returns(NullLoggerFactory.Instance);
        contextMock.SetupGet(c => c.Configuration).Returns(new GameConfiguration());
        contextMock.SetupGet(c => c.PersistenceContextProvider).Returns(new InMemoryPersistenceContextProvider());
        return new Player(contextMock.Object);
    }

    private MiniGameDefinition CreateDefinition()
    {
        return new MiniGameDefinition { MaximumPlayerCount = 1 };
    }
}
