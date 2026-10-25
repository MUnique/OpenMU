// <copyright file="MiniGameLeaderboardEfCoreTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.DataModel.Statistics;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the mini game leaderboard query of <see cref="IPlayerContext.GetMiniGameLeaderboardAsync"/>
/// against PostgreSQL, which requires a running postgres database.
/// </summary>
[TestFixture]
internal class MiniGameLeaderboardEfCoreTests
{
    /// <summary>
    /// Saves rankings of two Blood Castle levels, Devil Square, an old game and a bot,
    /// and checks the totals, the time filter, the bot exclusion, the order and the limit.
    /// </summary>
    [Test]
    [Ignore("This is not a real test which should run automatically. It requires a database.")]
    public async Task LeaderboardIsAggregatedByTheDatabaseAsync()
    {
        // These converters are registered by the host application (see Startup/Program.cs).
        JsonConverterRegistry.ClearConverters();
        JsonConverterRegistry.RegisterConverter(new LocalizedStringJsonConverter());
        JsonConverterRegistry.RegisterConverter(new BinaryAsHexJsonConverter());

        var contextProvider = new PersistenceContextProvider(new NullLoggerFactory(), null);
        (await contextProvider.ReCreateDatabaseAsync().ConfigureAwait(false)).Dispose();

        await new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory())
            .CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var configurationContext = contextProvider.CreateNewConfigurationContext();
        var configuration = (await configurationContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var bloodCastles = configuration.MiniGameDefinitions.Where(d => d.Type == MiniGameType.BloodCastle).OrderBy(d => d.GameLevel).ToList();
        var devilSquare = configuration.MiniGameDefinitions.First(d => d.Type == MiniGameType.DevilSquare);
        var characterClass = configuration.CharacterClasses.First();
        var now = DateTime.UtcNow;

        using (var playerContext = contextProvider.CreateNewPlayerContext(configuration))
        {
            var steady = AddCharacter(playerContext, "Steady", characterClass, isBot: false);
            var lucky = AddCharacter(playerContext, "Lucky", characterClass, isBot: false);
            var oldTimer = AddCharacter(playerContext, "OldTimer", characterClass, isBot: false);
            var bot = AddCharacter(playerContext, "Bot", characterClass, isBot: true);
            await playerContext.SaveChangesAsync().ConfigureAwait(false);

            using var rankingContext = contextProvider.CreateNewTypedContext(typeof(MiniGameRankingEntry), false, configuration);
            AddEntry(rankingContext, steady, bloodCastles[0], 300, now.AddHours(-1));
            AddEntry(rankingContext, steady, bloodCastles[1], 400, now.AddHours(-2));
            AddEntry(rankingContext, lucky, bloodCastles[1], 600, now.AddHours(-3));
            AddEntry(rankingContext, lucky, devilSquare, 5000, now.AddHours(-3));
            AddEntry(rankingContext, oldTimer, bloodCastles[0], 9000, now.AddDays(-30));
            AddEntry(rankingContext, bot, bloodCastles[0], 9000, now.AddHours(-1));
            await rankingContext.SaveChangesAsync().ConfigureAwait(false);
        }

        var bloodCastleIds = bloodCastles.Select(d => d.GetId()).ToList();
        using var queryContext = contextProvider.CreateNewPlayerContext(configuration);
        var lastWeek = await queryContext.GetMiniGameLeaderboardAsync(bloodCastleIds, now.AddDays(-7), 100).ConfigureAwait(false);
        var allTime = await queryContext.GetMiniGameLeaderboardAsync(bloodCastleIds, null, 100).ConfigureAwait(false);
        var limited = await queryContext.GetMiniGameLeaderboardAsync(bloodCastleIds, null, 2).ConfigureAwait(false);

        Assert.That(lastWeek.Select(e => (e.CharacterName, e.TotalScore, e.BestScore, e.GameCount)), Is.EqualTo(new[] { ("Steady", 700L, 400, 2), ("Lucky", 600L, 600, 1) }));
        Assert.That(lastWeek[0].CharacterClassId, Is.EqualTo(characterClass.GetId()));
        Assert.That(allTime.Select(e => e.CharacterName), Is.EqualTo(new[] { "OldTimer", "Steady", "Lucky" }));
        Assert.That(limited.Select(e => e.CharacterName), Is.EqualTo(new[] { "OldTimer", "Steady" }));
    }

    private static Character AddCharacter(IContext context, string name, CharacterClass characterClass, bool isBot)
    {
        var account = context.CreateNew<Account>();
        account.LoginName = name.ToLowerInvariant();
        account.PasswordHash = string.Empty;
        account.SecurityCode = string.Empty;
        account.EMail = string.Empty;
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
