// <copyright file="CastleSiegeParticipantTracker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Tracks Castle Siege participation and awards the configured post-battle rewards.
/// </summary>
public static class CastleSiegeParticipantTracker
{
    /// <summary>
    /// Records participation for players on the Castle Siege map.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="utcNow">The current UTC time.</param>
    /// <returns>A task that represents the asynchronous tracking operation.</returns>
    public static async ValueTask TrackAsync(CastleSiegeContext context, DateTime utcNow)
    {
        if (context.CurrentState != CastleSiegeState.Start
            || context.Configuration.CastleSiegeMapDefinition is not { } mapDefinition)
        {
            return;
        }

        foreach (var player in await context.GameContext.GetPlayersAsync().ConfigureAwait(false))
        {
            if (player.CurrentMap?.Definition.Number != mapDefinition.Number)
            {
                continue;
            }

            UpdateParticipant(context, player, utcNow, true, false);
        }
    }

    /// <summary>
    /// Awards eligible participants and increases the winning alliance's guild scores.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns>A task that represents the asynchronous reward operation.</returns>
    public static async ValueTask AwardRewardsAsync(CastleSiegeContext context)
    {
        var eligibleParticipants = context.ParticipantTracking.Values
            .Where(participant => participant.ParticipationTime.TotalSeconds >= context.Configuration.ParticipantRewardMinSeconds)
            .ToList();
        var onlinePlayers = (await context.GameContext.GetPlayersAsync().ConfigureAwait(false))
            .Where(player => player.SelectedCharacter is not null)
            .GroupBy(player => player.SelectedCharacter!.Id)
            .ToDictionary(group => group.Key, group => group.First());

        if (context.Configuration.RewardItemDefinition is { } rewardDefinition)
        {
            foreach (var participant in eligibleParticipants)
            {
                if (onlinePlayers.TryGetValue(participant.CharacterId, out var player)
                    && await CastleSiegeRewardDelivery
                        .TryAddToInventoryAsync(player, rewardDefinition)
                        .ConfigureAwait(false))
                {
                    continue;
                }

                await CastleSiegeRewardDelivery
                    .QueueAsync(context.GameContext, participant.CharacterId, rewardDefinition)
                    .ConfigureAwait(false);
            }
        }

        if (context.GameContext is not IGameServerContext gameServerContext
            || GetWinningGuild(context) is not { } ownerGuild)
        {
            return;
        }

        foreach (var guild in context.FinalGuildList.Values.Where(guild => guild.Side == ownerGuild.Side))
        {
            var score = guild.PersistentGuildId == ownerGuild.PersistentGuildId
                ? context.Configuration.GuildScoreCastleSiege
                : context.Configuration.GuildScoreCastleSiegeMembers;
            if (score <= 0)
            {
                continue;
            }

            var runtimeGuildId = await gameServerContext.GuildServer
                .GetGuildIdAsync(guild.PersistentGuildId)
                .ConfigureAwait(false);
            if (runtimeGuildId == 0)
            {
                continue;
            }

            await gameServerContext.GuildServer.IncreaseGuildScoreAsync(runtimeGuildId, score).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Starts tracking a player who entered the Castle Siege map during the battle.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="player">The player.</param>
    /// <param name="utcNow">The current UTC time.</param>
    internal static void StartTracking(CastleSiegeContext context, Player player, DateTime utcNow)
    {
        if (context.CurrentState == CastleSiegeState.Start)
        {
            UpdateParticipant(context, player, utcNow, true, false);
        }
    }

    /// <summary>
    /// Completes the current tracking interval when a player leaves the Castle Siege map.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="player">The player.</param>
    /// <param name="utcNow">The current UTC time.</param>
    internal static void StopTracking(CastleSiegeContext context, Player player, DateTime utcNow)
    {
        if (context.CurrentState == CastleSiegeState.Start)
        {
            UpdateParticipant(context, player, utcNow, false, true);
        }
    }

    private static void UpdateParticipant(
        CastleSiegeContext context,
        Player player,
        DateTime utcNow,
        bool isTracking,
        bool allowPlayerOutsideSiegeMap)
    {
        if ((!allowPlayerOutsideSiegeMap
             && player.CurrentMap?.Definition.Number != context.Configuration.CastleSiegeMapDefinition?.Number)
            || player.SelectedCharacter is not { } character
            || player.GuildStatus is not { } guildStatus
            || context.GetPlayerJoinSide(player) == CastleSiegeJoinSide.None)
        {
            return;
        }

        context.ParticipantTracking.AddOrUpdate(
            character.Id,
            _ => new CastleSiegeParticipant
            {
                CharacterId = character.Id,
                CharacterName = character.Name,
                GuildId = guildStatus.GuildId,
                LastUpdateUtc = utcNow,
                IsTracking = isTracking,
            },
            (_, participant) =>
            {
                var participationTime = participant.ParticipationTime;
                if (participant.IsTracking && utcNow > participant.LastUpdateUtc)
                {
                    participationTime += utcNow - participant.LastUpdateUtc;
                }

                return new CastleSiegeParticipant
                {
                    CharacterId = character.Id,
                    CharacterName = character.Name,
                    GuildId = guildStatus.GuildId,
                    ParticipationTime = participationTime,
                    LastUpdateUtc = utcNow,
                    IsTracking = isTracking,
                };
            });
    }

    private static CastleSiegeGuildParticipant? GetWinningGuild(CastleSiegeContext context)
    {
        if (context.MiddleOwnerGuildId is { } runtimeGuildId)
        {
            return context.FinalGuildList.GetValueOrDefault(runtimeGuildId);
        }

        return context.SiegeData.OwnerGuildId is { } persistentGuildId
            ? context.FinalGuildList.Values.FirstOrDefault(guild => guild.PersistentGuildId == persistentGuildId)
            : null;
    }
}
