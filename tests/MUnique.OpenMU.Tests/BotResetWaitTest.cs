// <copyright file="BotResetWaitTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests how a game server which doesn't generate the bot population learns that a requested reset
/// (<see cref="BotConfiguration.ResetBots"/>) is done: through the flag in the database, which the
/// generator server clears once it deleted the accounts.
/// </summary>
[TestFixture]
public class BotResetWaitTest
{
    /// <summary>
    /// While the flag is still set in the database, the reset is pending, so the server must not spawn
    /// the accounts which are about to be deleted.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask ResetIsPendingWhileTheFlagIsSetAsync()
    {
        var gameContext = CreateGameContext(new BotConfiguration { ResetBots = true });

        var isPending = await BotFeaturePlugIn.IsResetPendingAsync(gameContext, NullLogger.Instance).ConfigureAwait(false);

        Assert.That(isPending, Is.True);
    }

    /// <summary>
    /// Once the generator server cleared the flag in the database, the reset is done.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask ResetIsDoneOnceTheFlagIsClearedAsync()
    {
        var gameContext = CreateGameContext(new BotConfiguration { ResetBots = false });

        var isPending = await BotFeaturePlugIn.IsResetPendingAsync(gameContext, NullLogger.Instance).ConfigureAwait(false);

        Assert.That(isPending, Is.False);
    }

    /// <summary>
    /// Without a configuration row, nobody can clear the flag there, so the server doesn't wait for it.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask MissingConfigurationRowDoesNotBlockAsync()
    {
        var gameContext = CreateGameContext(null);

        var isPending = await BotFeaturePlugIn.IsResetPendingAsync(gameContext, NullLogger.Instance).ConfigureAwait(false);

        Assert.That(isPending, Is.False);
    }

    private static IGameContext CreateGameContext(BotConfiguration? botConfiguration)
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        if (botConfiguration is not null)
        {
            // Stored like the generator server persists it, so the check reads it back from the repository.
            using var context = gameContext.PersistenceContextProvider.CreateNewContext();
            var entity = context.CreateNew<PlugInConfiguration>();
            entity.TypeId = typeof(BotFeaturePlugIn).GUID;
            entity.IsActive = true;
            entity.SetConfiguration(botConfiguration, null);
        }

        return gameContext;
    }
}
