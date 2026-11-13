// <copyright file="BotRecentDeathCountTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Tests how a bot remembers who killed it (<see cref="OfflinePlayer.GetRecentDeathCount"/>) - the bot
/// chat answers a repeated kill differently ("you again...").
/// </summary>
[TestFixture]
public class BotRecentDeathCountTest
{
    /// <summary>
    /// The count grows with every kill by the same player and stays separate per killer.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask CountsKillsPerPlayerAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        bot.SetCurrentMapSilently(new GameMap(new GameMapDefinition(), TimeSpan.FromMinutes(1), 8));
        var killer = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        killer.SelectedCharacter!.Name = "Killer";
        var other = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        other.SelectedCharacter!.Name = "Other";

        Assert.That(bot.GetRecentDeathCount(killer), Is.Zero);

        bot.RegisterDeathByPlayer(killer);
        bot.RegisterDeathByPlayer(killer);

        Assert.That(bot.GetRecentDeathCount(killer), Is.EqualTo(OfflinePlayer.RepeatedKillThreshold));
        Assert.That(bot.GetRecentDeathCount(other), Is.Zero);
    }
}
