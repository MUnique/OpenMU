// <copyright file="GensRankingPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Gens;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;

/// <summary>
/// Calculates the ranking of the gens in the configured interval, and updates the ranking positions
/// and ranks of all members, including the ones which aren't online.
/// </summary>
/// <remarks>
/// All game servers of a process share this plugin, so the ranking is calculated once for all of them.
/// Each game server then updates the members which are online on it, so that their next save keeps the new values.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.GensRankingPlugIn_Name), Description = nameof(PlugInResources.GensRankingPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("D3F86B2A-9C17-4E05-A4B9-8E1C7D2F6053")]
public class GensRankingPlugIn : IPeriodicTaskPlugIn
{
    private readonly AsyncLock _lock = new();

    private readonly ConditionalWeakTable<IGameContext, StrongBox<int>> _appliedRankings = new();

    private DateTime _nextRanking = DateTime.MinValue;

    private int _rankingVersion;

    private IReadOnlyDictionary<Guid, (byte Rank, int Position)> _ranking = new Dictionary<Guid, (byte Rank, int Position)>();

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        if (GensFeaturePlugIn.GetConfiguration(gameContext) is not { } configuration)
        {
            return;
        }

        if (DateTime.UtcNow >= this._nextRanking)
        {
            using (await this._lock.LockAsync().ConfigureAwait(false))
            {
                if (DateTime.UtcNow >= this._nextRanking)
                {
                    await this.CalculateRankingAsync(gameContext, configuration).ConfigureAwait(false);
                    this._nextRanking = DateTime.UtcNow.Add(configuration.RankingInterval);
                }
            }
        }

        var appliedRanking = this._appliedRankings.GetOrCreateValue(gameContext);
        var rankingVersion = this._rankingVersion;
        if (appliedRanking.Value != rankingVersion)
        {
            appliedRanking.Value = rankingVersion;
            await UpdateOnlineMembersAsync(gameContext, this._ranking).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        this._nextRanking = DateTime.MinValue;
    }

    /// <summary>
    /// Removes the kill counts of the anti-abuse, which would start again with the next kill anyway.
    /// </summary>
    private static async ValueTask RemoveExpiredKillCountsAsync(GameContext gameContext, GensConfiguration configuration)
    {
        using var context = gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(GensAbuse), false, gameContext.Configuration);
        var expiredBefore = DateTime.UtcNow - configuration.AbuseResetTime;
        var abuses = await context.GetAsync<GensAbuse>().ConfigureAwait(false);
        var hasChanges = false;
        foreach (var abuse in abuses.Where(abuse => abuse.LastKillAt < expiredBefore).ToList())
        {
            await context.DeleteAsync(abuse).ConfigureAwait(false);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Updates the members which are online on the game server, and shows their changed rank.
    /// </summary>
    private static async ValueTask UpdateOnlineMembersAsync(GameContext gameContext, IReadOnlyDictionary<Guid, (byte Rank, int Position)> ranking)
    {
        await gameContext.ForEachPlayerAsync(async player =>
        {
            if (player.GensMember is not { Gens: not GensType.None } member
                || !ranking.TryGetValue(member.CharacterId, out var entry)
                || (member.Rank == entry.Rank && member.RankingPosition == entry.Position))
            {
                return;
            }

            await player.RunPersistenceExclusiveAsync(() =>
            {
                member.Rank = entry.Rank;
                member.RankingPosition = entry.Position;
                return ValueTask.CompletedTask;
            }).ConfigureAwait(false);
            await player.ShowChangedGensAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Calculates the ranking of each gens by the contribution points. With the same points, the member which joined first is ranked higher.
    /// </summary>
    private async ValueTask CalculateRankingAsync(GameContext gameContext, GensConfiguration configuration)
    {
        try
        {
            var ranking = new Dictionary<Guid, (byte Rank, int Position)>();
            using (var context = gameContext.PersistenceContextProvider.CreateNewTypedContext(typeof(GensMember), false, gameContext.Configuration))
            {
                var members = await context.GetAsync<GensMember>().ConfigureAwait(false);
                foreach (var gensMembers in members.Where(member => member.Gens != GensType.None).GroupBy(member => member.Gens))
                {
                    var position = 0;
                    foreach (var member in gensMembers
                                 .OrderByDescending(member => member.Contribution)
                                 .ThenBy(member => member.JoinedAt)
                                 .ThenBy(member => member.Id))
                    {
                        position++;
                        var rank = configuration.GetRank(member.Contribution, position);
                        member.RankingPosition = position;
                        member.Rank = rank;
                        ranking[member.CharacterId] = (rank, position);
                    }
                }

                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            await RemoveExpiredKillCountsAsync(gameContext, configuration).ConfigureAwait(false);

            this._ranking = ranking;
            this._rankingVersion++;
            gameContext.LoggerFactory.CreateLogger<GensRankingPlugIn>().LogInformation("Calculated the gens ranking of {count} members.", ranking.Count);
        }
        catch (Exception ex)
        {
            gameContext.LoggerFactory.CreateLogger<GensRankingPlugIn>().LogError(ex, "Error when calculating the gens ranking.");
        }
    }
}
