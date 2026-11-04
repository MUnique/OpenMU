// <copyright file="BotSelfHealingTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Tests how a bot reacts to its AI failing: the engine's attribute system is not thread-safe, and a
/// lost race can corrupt a character's attribute graph for good - from then on every tick throws, the
/// bot stops playing and floods the log. It asks for a restart, which rebuilds the graph and heals it.
/// </summary>
[TestFixture]
public class BotSelfHealingTest
{
    /// <summary>
    /// A tick failing now and then is simply skipped, like before - no restart.
    /// </summary>
    [Test]
    public void SingleFailuresDoNotRestartTheBot()
    {
        var bot = new BotPlayer(GameContextTestHelper.CreateGameContext());

        for (var i = 0; i < 100; i++)
        {
            bot.OnAiTickFailed();
            bot.OnAiTickSucceeded();
        }

        Assert.That(bot.AwaitsFaultRestart, Is.False);
    }

    /// <summary>
    /// A bot whose ticks keep failing is broken and asks the maintenance pass to restart it.
    /// </summary>
    [Test]
    public void PersistentFailuresRestartTheBot()
    {
        var bot = new BotPlayer(GameContextTestHelper.CreateGameContext());

        for (var i = 0; i < 20; i++)
        {
            bot.OnAiTickFailed();
        }

        Assert.That(bot.AwaitsFaultRestart, Is.True);
    }
}
