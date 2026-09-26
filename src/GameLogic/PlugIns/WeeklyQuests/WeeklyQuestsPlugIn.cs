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
    IItemPickedUpPlugIn,
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

            foreach (var receiver in await this.GetKillReceiversAsync(player).ConfigureAwait(false))
            {
                await this.AddProgressAsync(receiver, WeeklyQuestObjectiveType.KillAnyMonster, 1, (quest, _) => IsOnMap(quest)).ConfigureAwait(false);
                await this.AddProgressAsync(
                    receiver,
                    WeeklyQuestObjectiveType.KillMonster,
                    1,
                    (quest, _) => IsOnMap(quest) && quest.Monster is not null && quest.Monster.Number == monster.Definition.Number).ConfigureAwait(false);
            }
        }
        else if (killed is Player victim && victim != player)
        {
            await this.AddProgressAsync(
                player,
                WeeklyQuestObjectiveType.KillPlayer,
                1,
                (quest, state) => CountsAsPlayerKill(quest, state, player, victim)).ConfigureAwait(false);
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
                (quest, _) => quest.MiniGameType == MiniGameType.Undefined || quest.MiniGameType == type).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public ValueTask ItemPickedUpAsync(Player player, Item item, bool fromPlayerInventory)
    {
        // Items which were dropped by a player don't count, otherwise the same item could be dropped and picked up again.
        if (fromPlayerInventory || item.Definition is not { } definition)
        {
            return ValueTask.CompletedTask;
        }

        var mapNumber = player.CurrentMap?.Definition.Number;
        return this.AddProgressAsync(
            player,
            WeeklyQuestObjectiveType.CollectItem,
            1,
            (quest, _) => (quest.Map is null || quest.Map.Number == mapNumber)
                          && quest.Item is { } questItem
                          && questItem.Group == definition.Group
                          && questItem.Number == definition.Number
                          && item.Level >= quest.MinimumItemLevel);
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

        // The available quests may have changed, e.g. by a level up, so the bonus may be reached now.
        await this.UpdateAllCompletedBonusAsync(player, state, configuration).ConfigureAwait(false);

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

    /// <summary>
    /// Gets the players for whom a monster kill of the killer counts.
    /// </summary>
    /// <param name="killer">The killer.</param>
    /// <returns>The killer and, if configured, its party members nearby.</returns>
    internal async ValueTask<IReadOnlyList<Player>> GetKillReceiversAsync(Player killer)
    {
        if (this.Configuration is not { ShareKillsWithParty: true } || killer.Party is not { } party)
        {
            return [killer];
        }

        // Like the experience: the party members who see the killer, i.e. who are nearby on the same map.
        using (await killer.ObserverLock.ReaderLockAsync())
        {
            return party.PartyList
                .OfType<Player>()
                .Where(p => p == killer || (p.IsAlive && killer.Observers.Contains(p)))
                .ToList();
        }
    }

    private static WeeklyQuestCharacterInfo GetCharacterInfo(Player player)
    {
        return new WeeklyQuestCharacterInfo(
            player.SelectedCharacter?.CharacterClass,
            player.Level,
            (int)(player.Attributes?[Stats.Resets] ?? 0));
    }

    private static List<WeeklyQuestDefinition> GetAvailableQuests(WeeklyQuestsConfiguration configuration, WeeklyQuestPlayerState state, WeeklyQuestCharacterInfo character)
    {
        return WeeklyQuestSelector.GetAvailableQuests(configuration, state.PeriodStartUtc, character, state.Progress, state.RewardedByOtherCharacters);
    }

    private static bool CountsAsPlayerKill(WeeklyQuestDefinition quest, WeeklyQuestPlayerState state, Player killer, Player victim)
    {
        if (victim.Level < quest.MinimumVictimLevel)
        {
            return false;
        }

        if (quest.IgnoreSameIp
            && (killer as IHasIpAddress)?.IpAddress is { Length: > 0 } killerIp
            && killerIp == (victim as IHasIpAddress)?.IpAddress)
        {
            return false;
        }

        if (quest.IgnoreSameGuild
            && killer.GuildStatus is { } killerGuild
            && killerGuild.GuildId == victim.GuildStatus?.GuildId)
        {
            return false;
        }

        if (quest.IgnoreSameParty && killer.Party is not null && killer.Party == victim.Party)
        {
            return false;
        }

        if (quest.VictimCooldownMinutes > 0 && victim.SelectedCharacter is { } victimCharacter)
        {
            var key = (quest.Id, victimCharacter.GetId());
            var now = DateTime.UtcNow;
            if (state.LastCountedVictimKills.TryGetValue(key, out var lastCounted)
                && now - lastCounted < TimeSpan.FromMinutes(quest.VictimCooldownMinutes))
            {
                return false;
            }

            state.LastCountedVictimKills[key] = now;
        }

        return true;
    }

    private static WeeklyQuestOverview CreateOverview(WeeklyQuestsConfiguration configuration, WeeklyQuestPlayerState state, Player player)
    {
        var entries = WeeklyQuestSelector.CreateEntries(configuration, state.PeriodStartUtc, GetCharacterInfo(player), state.Progress, state.RewardedByOtherCharacters);
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

        var accountId = player.Account?.GetId();
        return States.GetValue(player, _ => new WeeklyQuestPlayerState(this, character.GetId(), accountId == Guid.Empty ? null : accountId));
    }

    private async ValueTask AddProgressAsync(Player player, WeeklyQuestObjectiveType objectiveType, int amount, Func<WeeklyQuestDefinition, WeeklyQuestPlayerState, bool>? filter = null)
    {
        if (this.Configuration is not { } configuration)
        {
            return;
        }

        // A quick check before the state is loaded, as most events don't concern any quest.
        if (!configuration.Quests.Any(q => q.IsActive && q.ObjectiveType == objectiveType)
            || this.GetOrCreateState(player) is not { } state)
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

            var character = GetCharacterInfo(player);
            var quests = GetAvailableQuests(configuration, state, character)
                .Where(q => q.ObjectiveType == objectiveType
                            && WeeklyQuestSelector.MeetsMinimumRequirements(q, character)
                            && (filter is null || filter(q, state)))
                .ToList();

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

            if (hasCompleted && await this.UpdateAllCompletedBonusAsync(player, state, configuration).ConfigureAwait(false) is { } bonusQuest)
            {
                changedQuests.Add(bonusQuest);
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

    /// <summary>
    /// Updates the progress of the <see cref="WeeklyQuestsConfiguration.AllCompletedBonus"/>, and rewards it when all quests are completed.
    /// </summary>
    /// <returns>The quest of the bonus, if its progress changed.</returns>
    private async ValueTask<WeeklyQuestDefinition?> UpdateAllCompletedBonusAsync(Player player, WeeklyQuestPlayerState state, WeeklyQuestsConfiguration configuration)
    {
        var quests = GetAvailableQuests(configuration, state, GetCharacterInfo(player));
        if (quests.Count == 0 || WeeklyQuestSelector.CreateAllCompletedBonusQuest(configuration, quests.Count) is not { } bonusQuest)
        {
            return null;
        }

        var completedCount = quests.Count(q => state.Progress.TryGetValue(q.Id, out var p) && p.CompletedAt is not null);
        var progress = state.GetOrCreateProgress(bonusQuest.Id);
        if (progress.CompletedAt is not null || progress.Count == completedCount)
        {
            return null;
        }

        progress.Count = completedCount;
        state.DirtyQuestIds.Add(bonusQuest.Id);
        if (completedCount >= quests.Count)
        {
            progress.CompletedAt = DateTime.UtcNow;
            await this.TryRewardAsync(player, bonusQuest, progress, true).ConfigureAwait(false);
            await this.SaveAsync(player, state).ConfigureAwait(false);
        }

        return bonusQuest;
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
            var entry = WeeklyQuestSelector.CreateEntry(quest, state.Progress);
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
            state.RewardedByOtherCharacters = await this.LoadRewardedByOtherCharactersAsync(state, periodStart).ConfigureAwait(false);
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

    private async ValueTask<HashSet<string>> LoadRewardedByOtherCharactersAsync(WeeklyQuestPlayerState state, DateTime periodStart)
    {
        // Only one character of an account can be in the game at the same time, so loading it once per period is enough.
        if (state.AccountId is not { } accountId
            || this.Configuration?.Quests.Any(q => q.OncePerAccount) != true)
        {
            return new HashSet<string>();
        }

        var rewarded = await this.Repository.LoadRewardedByAccountAsync(accountId, periodStart).ConfigureAwait(false);
        return rewarded
            .Where(p => p.CharacterId != state.CharacterId)
            .Select(p => p.QuestId)
            .ToHashSet();
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
            var quest = progress.QuestId == WeeklyQuestSelector.AllCompletedBonusId
                ? WeeklyQuestSelector.CreateAllCompletedBonusQuest(configuration, progress.Count)
                : configuration.Quests.FirstOrDefault(q => q.Id == progress.QuestId);
            if (quest is not null)
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
