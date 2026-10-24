// <copyright file="MiniGameLeaderboardServiceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Leaderboards;

using System.Threading;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.DataModel.Statistics;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Tests for the <see cref="MiniGameLeaderboardService"/>.
/// </summary>
[TestFixture]
public class MiniGameLeaderboardServiceTests
{
    private InMemoryPersistenceContextProvider _persistenceContextProvider = null!;
    private Persistence.BasicModel.GameConfiguration _gameConfiguration = null!;
    private MiniGameLeaderboardService _service = null!;

    /// <summary>
    /// Sets up a game configuration with two levels of Blood Castle and one Devil Square, of which only Blood Castle saves its ranking.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._persistenceContextProvider = new InMemoryPersistenceContextProvider();
        this._gameConfiguration = new Persistence.BasicModel.GameConfiguration();
        this.AddMiniGame(MiniGameType.BloodCastle, saveRanking: true);
        this.AddMiniGame(MiniGameType.BloodCastle, saveRanking: true);
        this.AddMiniGame(MiniGameType.DevilSquare, saveRanking: false);

        var characterClass = new Persistence.BasicModel.CharacterClass { Name = "Dark Knight" };
        this._gameConfiguration.CharacterClasses.Add(characterClass);

        var gameConfigurationSource = new Mock<IDataSource<GameConfiguration>>();
        gameConfigurationSource
            .Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(this._gameConfiguration);
        this._service = new MiniGameLeaderboardService(this._persistenceContextProvider, gameConfigurationSource.Object);
    }

    /// <summary>
    /// Only the mini game types which save their ranking statistics have a leaderboard.
    /// </summary>
    [Test]
    public async Task OnlyMiniGamesWhichSaveTheirRankingAreListedAsync()
    {
        var types = await this._service.GetMiniGameTypesAsync().ConfigureAwait(false);

        Assert.That(types, Is.EqualTo(new[] { MiniGameType.BloodCastle }));
    }

    /// <summary>
    /// The leaderboard sums up the scores of all levels of a mini game type, ranks the characters and shows their class.
    /// </summary>
    [Test]
    public async Task LeaderboardCombinesAllLevelsAsync()
    {
        var bloodCastles = this._gameConfiguration.MiniGameDefinitions.Where(d => d.Type == MiniGameType.BloodCastle).ToList();
        using (var context = this._persistenceContextProvider.CreateNewPlayerContext(this._gameConfiguration))
        {
            var account = context.CreateNew<Account>();
            account.LoginName = "player";
            var knight = context.CreateNew<Character>();
            knight.Name = "Knight";
            knight.CharacterClass = this._gameConfiguration.CharacterClasses.First();
            account.Characters.Add(knight);
            var newbie = context.CreateNew<Character>();
            newbie.Name = "Newbie";
            account.Characters.Add(newbie);

            AddEntry(context, knight, bloodCastles[0], 100);
            AddEntry(context, knight, bloodCastles[1], 250);
            AddEntry(context, newbie, bloodCastles[0], 200);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var leaderboard = await this._service.GetLeaderboardAsync(MiniGameType.BloodCastle, null).ConfigureAwait(false);

        Assert.That(leaderboard, Is.EqualTo(new[]
        {
            new MiniGameLeaderboardEntry(1, "Knight", "Dark Knight", 350, 250, 2),
            new MiniGameLeaderboardEntry(2, "Newbie", string.Empty, 200, 200, 1),
        }));
    }

    /// <summary>
    /// A mini game type without definitions has an empty leaderboard.
    /// </summary>
    [Test]
    public async Task UnknownMiniGameTypeHasEmptyLeaderboardAsync()
    {
        var leaderboard = await this._service.GetLeaderboardAsync(MiniGameType.ChaosCastle, null).ConfigureAwait(false);

        Assert.That(leaderboard, Is.Empty);
    }

    private static void AddEntry(IContext context, Character character, MiniGameDefinition miniGame, int score)
    {
        var entry = context.CreateNew<MiniGameRankingEntry>();
        entry.Character = character;
        entry.MiniGame = miniGame;
        entry.Score = score;
        entry.Timestamp = DateTime.UtcNow;
        entry.GameInstanceId = Guid.NewGuid();
    }

    private void AddMiniGame(MiniGameType type, bool saveRanking)
    {
        this._gameConfiguration.MiniGameDefinitions.Add(new Persistence.BasicModel.MiniGameDefinition
        {
            Id = Guid.NewGuid(),
            Type = type,
            SaveRankingStatistics = saveRanking,
        });
    }
}
