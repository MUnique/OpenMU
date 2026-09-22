// <copyright file="CastleSiegeGuildSelector.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Selects the guilds which participate in the next Castle Siege battle.
/// </summary>
public static class CastleSiegeGuildSelector
{
    private const int MaximumSupportedAttackingSides = 3;

    /// <summary>
    /// Calculates the Castle Siege selection score.
    /// </summary>
    /// <param name="marks">The submitted Signs of Lord.</param>
    /// <param name="memberCount">The guild member count.</param>
    /// <param name="guildMasterCombinedLevel">The guild master's normal and master levels.</param>
    /// <returns>The selection score.</returns>
    public static int CalculateScore(int marks, int memberCount, int guildMasterCombinedLevel)
    {
        var score = ((long)marks * 5) + memberCount + (guildMasterCombinedLevel / 4);
        return (int)Math.Clamp(score, 0, int.MaxValue);
    }

    /// <summary>
    /// Orders the final guild list consistently for persistence and client display.
    /// </summary>
    /// <param name="guilds">The guilds to order.</param>
    /// <returns>The ordered guilds.</returns>
    public static IEnumerable<CastleSiegeGuildParticipant> OrderFinalGuilds(
        IEnumerable<CastleSiegeGuildParticipant> guilds)
    {
        return guilds
            .OrderBy(guild => guild.Side)
            .ThenByDescending(guild => guild.IsAllianceMaster)
            .ThenBy(guild => guild.GuildName, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Selects the attacking guilds, assigns the defending guild and expands their alliances.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <returns>A task that represents the asynchronous selection operation.</returns>
    public static async ValueTask SelectGuildsAsync(CastleSiegeContext context)
    {
        context.FinalGuildList.Clear();
        if (context.GameContext is not IGameServerContext gameServerContext)
        {
            await context.SaveFinalGuildListAsync().ConfigureAwait(false);
            return;
        }

        var candidates = new List<SelectionCandidate>();
        foreach (var registration in context.RegisteredGuilds.Values)
        {
            if (registration.Marks <= 0)
            {
                continue;
            }

            var runtimeGuildId = await gameServerContext.GuildServer
                .GetGuildIdAsync(registration.GuildId)
                .ConfigureAwait(false);
            if (runtimeGuildId == 0)
            {
                continue;
            }

            var members = await gameServerContext.GuildServer
                .GetGuildListAsync(runtimeGuildId)
                .ConfigureAwait(false);
            if (members.Count == 0)
            {
                continue;
            }

            var guildMaster = members.FirstOrDefault(member => member.PlayerPosition == GuildPosition.GuildMaster);
            var combinedLevel = await GetCombinedLevelAsync(context, guildMaster?.PlayerName).ConfigureAwait(false);
            candidates.Add(new(
                runtimeGuildId,
                registration,
                CalculateScore(registration.Marks, members.Count, combinedLevel)));
        }

        if (context.SiegeData is { IsOccupied: true, OwnerGuildId: { } ownerGuildId })
        {
            await AddOwnerGuildAsync(context, gameServerContext, ownerGuildId).ConfigureAwait(false);
        }

        var maximumAttackers = Math.Clamp(
            context.Configuration.MaxAttackingGuilds,
            0,
            MaximumSupportedAttackingSides);
        var selectedAttackers = candidates
            .OrderByDescending(candidate => candidate.Score)
            .ThenBy(candidate => candidate.Registration.RegistrationOrder);

        var attackSideIndex = 0;
        foreach (var candidate in selectedAttackers)
        {
            if (attackSideIndex >= maximumAttackers)
            {
                break;
            }

            // The owner or another member of an already selected alliance must not consume an attack slot.
            if (context.FinalGuildList.ContainsKey(candidate.RuntimeGuildId))
            {
                continue;
            }

            var side = (CastleSiegeJoinSide)((byte)CastleSiegeJoinSide.Attack1 + attackSideIndex);
            if (await AddGuildAndAllianceAsync(
                    context,
                    gameServerContext,
                    candidate.RuntimeGuildId,
                    candidate.Registration.GuildId,
                    candidate.Registration.GuildName,
                    side,
                    candidate.Score)
                .ConfigureAwait(false))
            {
                attackSideIndex++;
            }
        }

        context.InitializeBattleOwner();
        await context.SaveFinalGuildListAsync().ConfigureAwait(false);
    }

    private static CastleSiegeGuildParticipant CreateParticipant(
        uint runtimeGuildId,
        Guid persistentGuildId,
        string guildName,
        CastleSiegeJoinSide side,
        int score,
        bool isAllianceMaster)
    {
        return new()
        {
            GuildId = runtimeGuildId,
            PersistentGuildId = persistentGuildId,
            GuildName = guildName,
            Side = side,
            Score = score,
            IsAllianceMaster = isAllianceMaster,
        };
    }

    private static async ValueTask AddOwnerGuildAsync(
        CastleSiegeContext context,
        IGameServerContext gameServerContext,
        Guid persistentGuildId)
    {
        var runtimeGuildId = await gameServerContext.GuildServer
            .GetGuildIdAsync(persistentGuildId)
            .ConfigureAwait(false);
        if (runtimeGuildId == 0
            || await gameServerContext.GuildServer.GetGuildAsync(runtimeGuildId).ConfigureAwait(false) is not { Name: { Length: > 0 } guildName })
        {
            return;
        }

        await AddGuildAndAllianceAsync(
                context,
                gameServerContext,
                runtimeGuildId,
                persistentGuildId,
                guildName,
                CastleSiegeJoinSide.Defense,
                0)
            .ConfigureAwait(false);
    }

    private static async ValueTask<bool> AddGuildAndAllianceAsync(
        CastleSiegeContext context,
        IGameServerContext gameServerContext,
        uint masterGuildId,
        Guid persistentMasterGuildId,
        string masterGuildName,
        CastleSiegeJoinSide side,
        int score)
    {
        var allianceGuilds = await gameServerContext.GuildServer
            .GetAllianceGuildsAsync(masterGuildId)
            .ConfigureAwait(false);
        if (!context.FinalGuildList.TryAdd(
                masterGuildId,
                CreateParticipant(
                    masterGuildId,
                    persistentMasterGuildId,
                    masterGuildName,
                    side,
                    score,
                    true)))
        {
            return false;
        }

        foreach (var allianceGuild in allianceGuilds.Where(guild => guild.Id != masterGuildId))
        {
            var persistentGuildId = await gameServerContext.GuildServer
                .GetPersistentGuildIdAsync(allianceGuild.Id)
                .ConfigureAwait(false);
            if (persistentGuildId is null)
            {
                continue;
            }

            context.FinalGuildList.TryAdd(
                allianceGuild.Id,
                CreateParticipant(
                    allianceGuild.Id,
                    persistentGuildId.Value,
                    allianceGuild.GuildName,
                    side,
                    0,
                    false));
        }

        return true;
    }

    private static async ValueTask<int> GetCombinedLevelAsync(CastleSiegeContext context, string? guildMasterName)
    {
        if (string.IsNullOrWhiteSpace(guildMasterName))
        {
            return 0;
        }

        if (context.GameContext.GetPlayerByCharacterName(guildMasterName) is { } onlineGuildMaster)
        {
            return onlineGuildMaster.Level
                   + (int)(onlineGuildMaster.Attributes?[Stats.MasterLevel] ?? 0);
        }

        using var persistenceContext = context.GameContext.PersistenceContextProvider
            .CreateNewPlayerContext(context.GameContext.Configuration);
        var account = await persistenceContext
            .GetAccountByCharacterNameAsync(guildMasterName)
            .ConfigureAwait(false);
        var character = account?.Characters.FirstOrDefault(candidate =>
            string.Equals(candidate.Name, guildMasterName, StringComparison.OrdinalIgnoreCase));
        if (character is null)
        {
            return 0;
        }

        var level = character.Attributes.FirstOrDefault(attribute => attribute.Definition == Stats.Level)?.Value ?? 0;
        var masterLevel = character.Attributes.FirstOrDefault(attribute => attribute.Definition == Stats.MasterLevel)?.Value ?? 0;
        return (int)(level + masterLevel);
    }

    private sealed record SelectionCandidate(
        uint RuntimeGuildId,
        CastleSiegeGuildRegistration Registration,
        int Score);
}
