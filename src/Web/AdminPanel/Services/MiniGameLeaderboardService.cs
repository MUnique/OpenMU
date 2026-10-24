// <copyright file="MiniGameLeaderboardService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Provides the leaderboards of the mini games, like Blood Castle, Devil Square and Chaos Castle.
/// </summary>
/// <remarks>
/// The leaderboards are built from the saved <see cref="DataModel.Statistics.MiniGameRankingEntry"/>s,
/// which are only saved for mini games with <see cref="MiniGameDefinition.SaveRankingStatistics"/>.
/// </remarks>
public class MiniGameLeaderboardService
{
    /// <summary>
    /// The maximum number of characters which are shown in a leaderboard.
    /// </summary>
    internal const int MaximumEntryCount = 100;

    private readonly IPersistenceContextProvider _persistenceContextProvider;
    private readonly IDataSource<GameConfiguration> _gameConfigurationSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniGameLeaderboardService"/> class.
    /// </summary>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="gameConfigurationSource">The game configuration source.</param>
    public MiniGameLeaderboardService(IPersistenceContextProvider persistenceContextProvider, IDataSource<GameConfiguration> gameConfigurationSource)
    {
        this._persistenceContextProvider = persistenceContextProvider;
        this._gameConfigurationSource = gameConfigurationSource;
    }

    /// <summary>
    /// Gets the types of the mini games which save their ranking statistics, so that they have a leaderboard.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The types of the mini games.</returns>
    public async ValueTask<IReadOnlyList<MiniGameType>> GetMiniGameTypesAsync(CancellationToken cancellationToken = default)
    {
        var gameConfiguration = await this._gameConfigurationSource.GetOwnerAsync(Guid.Empty, cancellationToken).ConfigureAwait(false);
        return gameConfiguration.MiniGameDefinitions
            .Where(definition => definition.SaveRankingStatistics)
            .Select(definition => definition.Type)
            .Distinct()
            .Order()
            .ToList();
    }

    /// <summary>
    /// Gets the leaderboard of all levels of a mini game type.
    /// </summary>
    /// <param name="miniGameType">The type of the mini game.</param>
    /// <param name="since">The optional point in time (UTC) since which finished games are counted.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The best characters, ordered by their total score.</returns>
    public async ValueTask<IReadOnlyList<MiniGameLeaderboardEntry>> GetLeaderboardAsync(MiniGameType miniGameType, DateTime? since, CancellationToken cancellationToken = default)
    {
        var gameConfiguration = await this._gameConfigurationSource.GetOwnerAsync(Guid.Empty, cancellationToken).ConfigureAwait(false);
        var miniGameIds = gameConfiguration.MiniGameDefinitions
            .Where(definition => definition.Type == miniGameType)
            .Select(definition => definition.GetId())
            .ToList();
        if (miniGameIds.Count == 0)
        {
            return [];
        }

        using var context = this._persistenceContextProvider.CreateNewPlayerContext(gameConfiguration);
        var summaries = await context.GetMiniGameLeaderboardAsync(miniGameIds, since, MaximumEntryCount, cancellationToken).ConfigureAwait(false);
        var classNames = gameConfiguration.CharacterClasses.ToDictionary(characterClass => characterClass.GetId(), characterClass => characterClass.Name.ToString() ?? string.Empty);
        return summaries
            .Select((summary, index) => new MiniGameLeaderboardEntry(
                index + 1,
                summary.CharacterName,
                summary.CharacterClassId is { } classId && classNames.TryGetValue(classId, out var className) ? className : string.Empty,
                summary.TotalScore,
                summary.BestScore,
                summary.GameCount))
            .ToList();
    }
}
