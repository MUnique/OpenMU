// <copyright file="WeeklyQuestsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.WeeklyQuests;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tracks the progress of the weekly quests and hands out their rewards.
/// The quests are configured in the custom configuration of this plugin.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.WeeklyQuestsPlugIn_Name), Description = nameof(PlugInResources.WeeklyQuestsPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3F2C8E61-9B7A-4D25-8E1F-6A4C0D9B7E52")]
public class WeeklyQuestsPlugIn :
    IPlayerStateChangedPlugIn,
    IAttackableGotKilledPlugIn,
    ICharacterLevelUpPlugIn,
    ICharacterMasterLevelUpPlugIn,
    ICharacterResetPlugIn,
    IMiniGameEndedPlugIn,
    IPeriodicTaskPlugIn,
    ISupportCustomConfiguration<WeeklyQuestsConfiguration>,
    ISupportDefaultCustomConfiguration
{
    /// <summary>
    /// The interval in which the progress of the players is saved.
    /// Completed quests and players who leave the game are saved immediately.
    /// </summary>
    private static readonly TimeSpan SaveInterval = TimeSpan.FromSeconds(60);

    /// <summary>
    /// The repository which is used when no other is registered, e.g. in the demo mode.
    /// It's shared, so that all game servers of the process see the same progress.
    /// </summary>
    private static readonly InMemoryWeeklyQuestProgressRepository FallbackRepository = new();

    private static readonly ConditionalWeakTable<Player, WeeklyQuestPlayerState> States = new();

    private static readonly int[] Milestones = [25, 50, 75];

    private readonly IWeeklyQuestProgressRepository? _repository;

    private DateTime _nextSaveUtc = DateTime.UtcNow + SaveInterval;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeeklyQuestsPlugIn"/> class,
    /// which uses the repository of the <see cref="WeeklyQuestProgressRepositoryRegistry"/>.
    /// </summary>
    public WeeklyQuestsPlugIn()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeeklyQuestsPlugIn"/> class.
    /// </summary>
    /// <remarks>
    /// It's internal on purpose: the plugin manager must only see the parameterless constructor.
    /// </remarks>
    /// <param name="repository">The repository of the progress.</param>
    internal WeeklyQuestsPlugIn(IWeeklyQuestProgressRepository repository)
    {
        this._repository = repository;
    }

    /// <summary>
    /// Gets the repository. It's resolved on each use, because the host may set it after the plugin has been created.
    /// </summary>
    private IWeeklyQuestProgressRepository Repository => this._repository ?? WeeklyQuestProgressRepositoryRegistry.Current ?? FallbackRepository;

    /// <inheritdoc />
    public WeeklyQuestsConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public object CreateDefaultConfig() => WeeklyQuestsConfiguration.Default;

    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        try
        {
            if (currentState.IsDisconnectedOrFinished() || currentState == PlayerState.CharacterSelection)
            {
                if (States.TryGetValue(player, out var leavingState))
                {
                    States.Remove(player);
                    using var l = await leavingState.Lock.LockAsync().ConfigureAwait(false);
                    await this.SaveAsync(player, leavingState).ConfigureAwait(false);
                }

                return;
            }

            if (previousState != PlayerState.CharacterSelection || currentState != PlayerState.EnteredWorld)
            {
                return;
            }

            if (this.GetOrCreateState(player) is not { } state)
            {
                return;
            }

            using (await state.Lock.LockAsync().ConfigureAwait(false))
            {
                if (await this.EnsureLoadedAsync(player, state).ConfigureAwait(false))
                {
                    await this.RewardPendingAsync(player, state, true).ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Unexpected error handling the weekly quests at a player state change.");
        }
    }

    /// <inheritdoc />
    public async ValueTask AttackableGotKilledAsync(IAttackable killed, IAttacker? killer)
    {
        var player = killer as Player ?? (killer as Monster)?.SummonedBy;
        if (player is null)
        {
            return;
        }

        if (killed is Monster monster)
        {
            if (monster.SummonedBy is not null)
            {
                return;
            }

            var mapNumber = monster.CurrentMap?.Definition.Number;
            bool IsOnMap(WeeklyQuestDefinition quest) => quest.Map is null || quest.Map.Number == mapNumber;

            await this.AddProgressAsync(player, WeeklyQuestObjectiveType.KillAnyMonster, 1, IsOnMap).ConfigureAwait(false);
            await this.AddProgressAsync(
                player,
                WeeklyQuestObjectiveType.KillMonster,
                1,
                quest => IsOnMap(quest) && quest.Monster is not null && quest.Monster.Number == monster.Definition.Number).ConfigureAwait(false);
        }
        else if (killed is Player victim && victim != player)
        {
            await this.AddProgressAsync(
                player,
                WeeklyQuestObjectiveType.KillPlayer,
                1,
                quest => victim.Level >= quest.MinimumVictimLevel).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public void CharacterLeveledUp(Player player)
    {
        // This plugin point is synchronous, so we handle it in the background.
        _ = Task.Run(async () =>
        {
            try
            {
                await this.AddProgressAsync(player, WeeklyQuestObjectiveType.GainLevels, 1).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogError(ex, "Unexpected error when adding weekly quest progress for a level up.");
            }
        });
    }

    /// <inheritdoc />
    public ValueTask CharacterMasterLeveledUpAsync(Player player)
    {
        return this.AddProgressAsync(player, WeeklyQuestObjectiveType.GainMasterLevels, 1);
    }

    /// <inheritdoc />
    public ValueTask CharacterResetAsync(Player player, int resetCount)
    {
        return this.AddProgressAsync(player, WeeklyQuestObjectiveType.GainResets, 1);
    }

    /// <inheritdoc />
    public async ValueTask MiniGameEndedAsync(MiniGameContext miniGame, ICollection<Player> finishers)
    {
        var type = miniGame.Definition.Type;
        foreach (var player in finishers)
        {
            await this.AddProgressAsync(
                player,
                WeeklyQuestObjectiveType.CompleteMiniGame,
                1,
                quest => quest.MiniGameType == MiniGameType.Undefined || quest.MiniGameType == type).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        if (DateTime.UtcNow < this._nextSaveUtc)
        {
            return;
        }

        this._nextSaveUtc = DateTime.UtcNow + SaveInterval;
        foreach (var player in await gameContext.GetPlayersAsync().ConfigureAwait(false))
        {
            if (!States.TryGetValue(player, out var state))
            {
                continue;
            }

            try
            {
                using var l = await state.Lock.LockAsync().ConfigureAwait(false);
                await this.SaveAsync(player, state).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogError(ex, "Unexpected error when saving the weekly quest progress periodically.");
            }
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        this._nextSaveUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the overview of the active quests and the progress of the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The overview; <c>null</c>, if the progress is not available.</returns>
    public async ValueTask<WeeklyQuestOverview?> GetOverviewAsync(Player player)
    {
        if (this.Configuration is not { } configuration || this.GetOrCreateState(player) is not { } state)
        {
            return null;
        }

        using var l = await state.Lock.LockAsync().ConfigureAwait(false);
        if (!await this.EnsureLoadedAsync(player, state).ConfigureAwait(false))
        {
            return null;
        }

        // The player may have freed some inventory space in the meantime.
        await this.RewardPendingAsync(player, state, false).ConfigureAwait(false);

        return CreateOverview(configuration, state, player);
    }

    /// <summary>
    /// Sends the active quests and the progress of the player to the client, so that it can show them in a window.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask SendListAsync(Player player)
    {
        if (await this.GetOverviewAsync(player).ConfigureAwait(false) is { } overview)
        {
            await player.InvokeViewPlugInAsync<IWeeklyQuestListViewPlugIn>(p => p.ShowWeeklyQuestsAsync(overview)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets the plugin which tracks the weekly quests of the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The plugin, if the player is tracked by one.</returns>
    public static WeeklyQuestsPlugIn? GetTrackingPlugIn(Player player)
    {
        return States.TryGetValue(player, out var state) ? state.Owner : null;
    }

    private static IEnumerable<WeeklyQuestDefinition> GetActiveQuests(WeeklyQuestsConfiguration configuration)
    {
        return configuration.Quests.Where(q => q.IsActive && !string.IsNullOrWhiteSpace(q.Id));
    }

    private static WeeklyQuestOverviewEntry CreateEntry(WeeklyQuestDefinition quest, WeeklyQuestPlayerState state)
    {
        state.Progress.TryGetValue(quest.Id, out var progress);
        return new WeeklyQuestOverviewEntry(quest, progress?.Count ?? 0, progress?.CompletedAt is not null, progress?.RewardedAt is not null);
    }

    private static WeeklyQuestOverview CreateOverview(WeeklyQuestsConfiguration configuration, WeeklyQuestPlayerState state, Player player)
    {
        var entries = GetActiveQuests(configuration).Select(quest => CreateEntry(quest, state)).ToList();
        return new WeeklyQuestOverview(entries, GetNextResetUtc(state, player));
    }

    private static DateTime GetNextResetUtc(WeeklyQuestPlayerState state, Player player)
    {
        return WeeklyPeriod.GetNextPeriodStartUtc(state.PeriodStartUtc, player.GameContext.ServerTimeZone);
    }

    private static string Format(LocalizedString template, Player player, params object?[] args)
    {
        var text = template.GetTranslation(player.Culture) ?? string.Empty;
        try
        {
            return string.Format(text, args);
        }
        catch (FormatException)
        {
            // A misconfigured message shouldn't break the quest.
            return text;
        }
    }

    private static ValueTask ShowGoldenMessageAsync(Player player, string message)
    {
        return player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.GoldenCenter));
    }

    private WeeklyQuestPlayerState? GetOrCreateState(Player player)
    {
        if (States.TryGetValue(player, out var state))
        {
            return state;
        }

        if (player.SelectedCharacter is not { } character
            || player.PlayerState.CurrentState.IsDisconnectedOrFinished()
            || player.PlayerState.CurrentState == PlayerState.CharacterSelection)
        {
            return null;
        }

        return States.GetValue(player, _ => new WeeklyQuestPlayerState(this, character.GetId()));
    }

    private async ValueTask AddProgressAsync(Player player, WeeklyQuestObjectiveType objectiveType, int amount, Func<WeeklyQuestDefinition, bool>? filter = null)
    {
        if (this.Configuration is not { } configuration)
        {
            return;
        }

        var level = player.Level;
        var resets = (int)(player.Attributes?[Stats.Resets] ?? 0);
        var quests = GetActiveQuests(configuration)
            .Where(q => q.ObjectiveType == objectiveType
                        && level >= q.MinimumLevel
                        && resets >= q.MinimumResets
                        && (filter is null || filter(q)))
            .ToList();
        if (quests.Count == 0 || this.GetOrCreateState(player) is not { } state)
        {
            return;
        }

        try
        {
            using var l = await state.Lock.LockAsync().ConfigureAwait(false);
            var previousPeriodStart = state.IsLoaded ? state.PeriodStartUtc : (DateTime?)null;
            if (!await this.EnsureLoadedAsync(player, state).ConfigureAwait(false))
            {
                return;
            }

            var hasCompleted = false;
            var changedQuests = new List<WeeklyQuestDefinition>();
            foreach (var quest in quests)
            {
                var progress = state.GetOrCreateProgress(quest.Id);
                if (progress.CompletedAt is not null)
                {
                    continue;
                }

                changedQuests.Add(quest);
                var previousCount = progress.Count;
                progress.Count = (int)Math.Min((long)previousCount + amount, quest.RequiredCount);
                state.DirtyQuestIds.Add(quest.Id);

                if (progress.Count >= quest.RequiredCount)
                {
                    progress.CompletedAt = DateTime.UtcNow;
                    hasCompleted = true;
                    await this.TryRewardAsync(player, quest, progress, true).ConfigureAwait(false);
                }
                else if (Milestones.Any(m => previousCount * 100 / quest.RequiredCount < m && progress.Count * 100 / quest.RequiredCount >= m))
                {
                    await player.ShowBlueMessageAsync(Format(configuration.ProgressMessage, player, quest.Name, progress.Count, quest.RequiredCount)).ConfigureAwait(false);
                }
            }

            // A completion is saved immediately, so that it can't get lost.
            if (hasCompleted)
            {
                await this.SaveAsync(player, state).ConfigureAwait(false);
            }

            var isNewPeriod = previousPeriodStart is not null && previousPeriodStart != state.PeriodStartUtc;
            await this.SendChangesAsync(player, state, configuration, changedQuests, isNewPeriod).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Unexpected error when adding weekly quest progress of type {objectiveType}.", objectiveType);
        }
    }

    private async ValueTask SendChangesAsync(Player player, WeeklyQuestPlayerState state, WeeklyQuestsConfiguration configuration, List<WeeklyQuestDefinition> changedQuests, bool isNewPeriod)
    {
        // After a reset, all quests of the client are outdated, not only the changed ones.
        if (isNewPeriod)
        {
            var overview = CreateOverview(configuration, state, player);
            await player.InvokeViewPlugInAsync<IWeeklyQuestListViewPlugIn>(p => p.ShowWeeklyQuestsAsync(overview)).ConfigureAwait(false);
            return;
        }

        var nextResetUtc = GetNextResetUtc(state, player);
        foreach (var quest in changedQuests)
        {
            var entry = CreateEntry(quest, state);
            await player.InvokeViewPlugInAsync<IWeeklyQuestListViewPlugIn>(p => p.UpdateWeeklyQuestAsync(entry, nextResetUtc)).ConfigureAwait(false);
        }
    }

    private async ValueTask<bool> EnsureLoadedAsync(Player player, WeeklyQuestPlayerState state)
    {
        var periodStart = WeeklyPeriod.GetPeriodStartUtc(
            DateTime.UtcNow,
            this.Configuration?.ResetDay ?? DayOfWeek.Monday,
            this.Configuration?.ResetTime ?? TimeOnly.MinValue,
            player.GameContext.ServerTimeZone);
        if (state.IsLoaded && state.PeriodStartUtc == periodStart)
        {
            return true;
        }

        try
        {
            if (state.IsLoaded)
            {
                // A new week began while the player is online. The progress of the last week is saved first.
                await this.SaveAsync(player, state).ConfigureAwait(false);
                state.IsLoaded = false;
            }

            var loaded = await this.Repository.LoadAsync(state.CharacterId, periodStart).ConfigureAwait(false);
            state.Progress = loaded.ToDictionary(p => p.QuestId);
            state.PeriodStartUtc = periodStart;
            state.DirtyQuestIds.Clear();
            state.IsLoaded = true;
            return true;
        }
        catch (Exception ex)
        {
            player.Logger.LogWarning(ex, "Couldn't load the weekly quest progress of character {characterId}.", state.CharacterId);
            return false;
        }
    }

    private async ValueTask SaveAsync(Player player, WeeklyQuestPlayerState state)
    {
        if (state.DirtyQuestIds.Count == 0)
        {
            return;
        }

        var entries = state.DirtyQuestIds
            .Where(state.Progress.ContainsKey)
            .Select(id => state.Progress[id])
            .ToList();
        try
        {
            await this.Repository.SaveAsync(entries).ConfigureAwait(false);
            state.DirtyQuestIds.Clear();
        }
        catch (Exception ex)
        {
            // The entries stay dirty, so they're saved at the next attempt.
            player.Logger.LogError(ex, "Couldn't save the weekly quest progress of character {characterId}.", state.CharacterId);
        }
    }

    private async ValueTask RewardPendingAsync(Player player, WeeklyQuestPlayerState state, bool showPendingMessage)
    {
        if (this.Configuration is not { } configuration)
        {
            return;
        }

        var rewarded = false;
        foreach (var progress in state.Progress.Values.Where(p => p.CompletedAt is not null && p.RewardedAt is null).ToList())
        {
            if (configuration.Quests.FirstOrDefault(q => q.Id == progress.QuestId) is { } quest)
            {
                rewarded |= await this.TryRewardAsync(player, quest, progress, showPendingMessage).ConfigureAwait(false);
            }
        }

        if (rewarded)
        {
            await this.SaveAsync(player, state).ConfigureAwait(false);
        }
    }

    private async ValueTask<bool> TryRewardAsync(Player player, WeeklyQuestDefinition quest, WeeklyQuestProgress progress, bool showPendingMessage)
    {
        var configuration = this.Configuration!;
        if (!await WeeklyQuestRewarder.TryGiveRewardsAsync(player, quest).ConfigureAwait(false))
        {
            if (showPendingMessage)
            {
                await player.ShowBlueMessageAsync(Format(configuration.RewardPendingMessage, player, quest.Name)).ConfigureAwait(false);
            }

            return false;
        }

        progress.RewardedAt = DateTime.UtcNow;
        if (States.TryGetValue(player, out var state))
        {
            state.DirtyQuestIds.Add(progress.QuestId);
        }

        await ShowGoldenMessageAsync(player, Format(configuration.CompletedMessage, player, quest.Name)).ConfigureAwait(false);
        player.Logger.LogInformation("Character {characterId} completed the weekly quest {quest}.", progress.CharacterId, quest.Id);
        return true;
    }
}
