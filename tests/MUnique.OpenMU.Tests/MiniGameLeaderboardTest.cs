// <copyright file="MiniGameLeaderboardTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.DataModel.Statistics;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the <see cref="IPlayerContext.GetMiniGameLeaderboardAsync"/>.
/// </summary>
[TestFixture]
public class MiniGameLeaderboardTest
{
    /// <summary>
    /// Tests that the query of the entity framework implementation can be translated to SQL.
    /// </summary>
    [Test]
    public void EntityFrameworkQueryIsTranslatable()
    {
        using var context = new AccountContext();

        var sql = PlayerContext.CreateMiniGameLeaderboardQuery(context, new[] { Guid.NewGuid(), Guid.NewGuid() }, DateTime.UtcNow.AddDays(-7), 10).ToQueryString();

        Assert.That(sql, Does.Contain("GROUP BY"));
        Assert.That(sql, Does.Contain("ORDER BY"));
    }

    /// <summary>
    /// Tests that the characters are ordered by their total score of the requested mini games,
    /// and that other mini games, old games and bots are not counted.
    /// </summary>
    [Test]
    public async Task CharactersAreRankedByTotalScoreAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var now = DateTime.UtcNow;
        MiniGameDefinition bloodCastle1;
        MiniGameDefinition bloodCastle2;
        CharacterClass characterClass;
        using (var context = contextProvider.CreateNewPlayerContext(new GameConfiguration()))
        {
            bloodCastle1 = context.CreateNew<MiniGameDefinition>();
            bloodCastle2 = context.CreateNew<MiniGameDefinition>();
            var devilSquare = context.CreateNew<MiniGameDefinition>();
            characterClass = context.CreateNew<CharacterClass>();

            var steady = AddCharacter(context, "Steady", characterClass, isBot: false);
            var lucky = AddCharacter(context, "Lucky", characterClass, isBot: false);
            var oldTimer = AddCharacter(context, "OldTimer", characterClass, isBot: false);
            var bot = AddCharacter(context, "Bot", characterClass, isBot: true);

            AddEntry(context, steady, bloodCastle1, 300, now.AddHours(-1));
            AddEntry(context, steady, bloodCastle2, 400, now.AddHours(-2));
            AddEntry(context, lucky, bloodCastle2, 600, now.AddHours(-3));
            AddEntry(context, lucky, devilSquare, 5000, now.AddHours(-3));
            AddEntry(context, oldTimer, bloodCastle1, 9000, now.AddDays(-30));
            AddEntry(context, bot, bloodCastle1, 9000, now.AddHours(-1));
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        using var queryContext = contextProvider.CreateNewPlayerContext(new GameConfiguration());
        var bloodCastleIds = new[] { bloodCastle1.GetId(), bloodCastle2.GetId() };
        var lastWeek = await queryContext.GetMiniGameLeaderboardAsync(bloodCastleIds, now.AddDays(-7), 10).ConfigureAwait(false);
        var allTime = await queryContext.GetMiniGameLeaderboardAsync(bloodCastleIds, null, 10).ConfigureAwait(false);

        Assert.That(lastWeek.Select(entry => entry.CharacterName), Is.EqualTo(new[] { "Steady", "Lucky" }));
        Assert.That(lastWeek[0].TotalScore, Is.EqualTo(700));
        Assert.That(lastWeek[0].BestScore, Is.EqualTo(400));
        Assert.That(lastWeek[0].GameCount, Is.EqualTo(2));
        Assert.That(lastWeek[0].CharacterClassId, Is.EqualTo(characterClass.GetId()));
        Assert.That(allTime.Select(entry => entry.CharacterName), Is.EqualTo(new[] { "OldTimer", "Steady", "Lucky" }));
    }

    /// <summary>
    /// Tests that the leaderboard is limited to the requested number of characters.
    /// </summary>
    [Test]
    public async Task LeaderboardIsLimitedToCountAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        MiniGameDefinition chaosCastle;
        using (var context = contextProvider.CreateNewPlayerContext(new GameConfiguration()))
        {
            chaosCastle = context.CreateNew<MiniGameDefinition>();
            for (var i = 0; i < 5; i++)
            {
                AddEntry(context, AddCharacter(context, $"Player{i}", null, isBot: false), chaosCastle, i * 10, DateTime.UtcNow);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        using var queryContext = contextProvider.CreateNewPlayerContext(new GameConfiguration());
        var leaderboard = await queryContext.GetMiniGameLeaderboardAsync(new[] { chaosCastle.GetId() }, null, 2).ConfigureAwait(false);

        Assert.That(leaderboard.Select(entry => entry.CharacterName), Is.EqualTo(new[] { "Player4", "Player3" }));
    }

    private static Character AddCharacter(IContext context, string name, CharacterClass? characterClass, bool isBot)
    {
        var account = context.CreateNew<Account>();
        account.LoginName = Guid.NewGuid().ToString("N")[..10];
        account.IsBot = isBot;
        var character = context.CreateNew<Character>();
        character.Name = name;
        character.CharacterClass = characterClass;
        account.Characters.Add(character);
        return character;
    }

    private static void AddEntry(IContext context, Character character, MiniGameDefinition miniGame, int score, DateTime timestamp)
    {
        var entry = context.CreateNew<MiniGameRankingEntry>();
        entry.Character = character;
        entry.MiniGame = miniGame;
        entry.Score = score;
        entry.Timestamp = timestamp;
        entry.GameInstanceId = Guid.NewGuid();
    }
}
