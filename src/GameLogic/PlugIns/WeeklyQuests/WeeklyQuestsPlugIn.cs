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
/// Tracks the progress of the quests (story chapters, daily, weekly, class and zone quests) and hands out their rewards.
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
    INpcTalkStartedPlugIn,
    IPlayerTalkToNpcPlugIn,
    IObjectAddedToMapPlugIn,
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
    /// The interval in which the progress of past periods is deleted, see <see cref="WeeklyQuestsConfiguration.HistoryWeeks"/>.
    /// </summary>
    private static readonly TimeSpan PurgeInterval = TimeSpan.FromHours(24);

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
    /// The first purge happens a while after the start, so that it doesn't slow the start down.
    /// </summary>
    private DateTime _nextPurgeUtc = DateTime.UtcNow + TimeSpan.FromMinutes(10);

    private volatile WeeklyQuestIndex? _index;

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

    /// <inheritdoc />
    public WeeklyQuestsConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets or sets the delay after which progress updates are sent to the client. The updates of this time
    /// are sent together, instead of one message per kill. Completed steps and quests are sent immediately.
    /// </summary>
    internal TimeSpan UpdateDelay { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Gets the repository. It's resolved on each use, because the host may set it after the plugin has been created.
    /// </summary>
    private IWeeklyQuestProgressRepository Repository => this._repository ?? WeeklyQuestProgressRepositoryRegistry.Current ?? FallbackRepository;

    /// <inheritdoc />
    public object CreateDefaultConfig() => WeeklyQuestsConfiguration.Default;

    /// <summary>
    /// Gets the plugin which tracks the quests of the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The plugin, if the player is tracked by one.</returns>
    public static WeeklyQuestsPlugIn? GetTrackingPlugIn(Player player)
    {
        return States.TryGetValue(player, out var state) ? state.Owner : null;
    }

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
            player.Logger.LogError(ex, "Unexpected error handling the quests at a player state change.");
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
            if (monster.SummonedBy is not null || this.GetIndex() is not { } index
                || (index.Get(WeeklyQuestObjectiveType.KillAnyMonster).Count == 0 && index.Get(WeeklyQuestObjectiveType.KillMonster).Count == 0))
            {
                return;
            }

            var mapNumber = monster.CurrentMap?.Definition.Number;
            var monsterNumber = monster.Definition.Number;
            foreach (var receiver in await this.GetKillReceiversAsync(player).ConfigureAwait(false))
            {
                await this.AddProgressAsync(receiver, WeeklyQuestObjectiveType.KillAnyMonster, 1, (_, objective, _) => objective.IsOnMap(mapNumber)).ConfigureAwait(false);
                await this.AddProgressAsync(
                    receiver,
                    WeeklyQuestObjectiveType.KillMonster,
                    1,
                    (_, objective, _) => objective.IsOnMap(mapNumber) && objective.Monster is not null && objective.Monster.Number == monsterNumber).ConfigureAwait(false);
            }
        }
        else if (killed is Player victim && victim != player)
        {
            await this.AddProgressAsync(
                player,
                WeeklyQuestObjectiveType.KillPlayer,
                1,
                (quest, objective, state) => CountsAsPlayerKill(quest, objective, state, player, victim)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public void CharacterLeveledUp(Player player)
    {
        if (this.GetIndex()?.Get(WeeklyQuestObjectiveType.GainLevels).Count is not > 0)
        {
            return;
        }

        // This plugin point is synchronous, so we handle it in the background.
        _ = Task.Run(async () =>
        {
            try
            {
                await this.AddProgressAsync(player, WeeklyQuestObjectiveType.GainLevels, 1).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogError(ex, "Unexpected error when adding quest progress for a level up.");
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
                (_, objective, _) => objective.MiniGameType == MiniGameType.Undefined || objective.MiniGameType == type).ConfigureAwait(false);
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
            (_, objective, _) => objective.IsOnMap(mapNumber)
                                 && objective.Item is { } questItem
                                 && questItem.Group == definition.Group
                                 && questItem.Number == definition.Number
                                 && item.Level >= objective.MinimumItemLevel);
    }

    /// <inheritdoc />
    public ValueTask NpcTalkStartedAsync(Player player, NonPlayerCharacter npc)
    {
        var npcNumber = npc.Definition.Number;
        var mapNumber = npc.CurrentMap?.Definition.Number;
        return this.AddProgressAsync(
            player,
            WeeklyQuestObjectiveType.TalkToNpc,
            1,
            (_, objective, _) => objective.Monster is not null && objective.Monster.Number == npcNumber && objective.IsOnMap(mapNumber));
    }

    /// <inheritdoc />
    public ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        // This is only called for NPCs without a window. An NPC which only exists for a quest would show
        // "talking is not implemented", so we mark it as handled. The progress is counted by NpcTalkStartedAsync.
        // It has to happen synchronously, because the caller doesn't await this method.
        if (player.CurrentMiniGame is null
            && this.GetIndex() is { } index
            && index.Get(WeeklyQuestObjectiveType.TalkToNpc).Any(o => o.Quest.IsActive && o.Objective.Monster?.Number == npc.Definition.Number))
        {
            eventArgs.HasBeenHandled = true;
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ObjectAddedToMapAsync(GameMap map, ILocateable addedObject)
    {
        // This is called for every object, including each monster respawn, so it has to return fast.
        if (addedObject is not Player player
            || this.GetIndex()?.Get(WeeklyQuestObjectiveType.EnterMap).Count is not > 0
            || this.GetOrCreateState(player) is not { } state)
        {
            return ValueTask.CompletedTask;
        }

        // A respawn on the same map isn't entering it.
        var mapNumber = map.Definition.Number;
        if (state.LastMapNumber == mapNumber)
        {
            return ValueTask.CompletedTask;
        }

        state.LastMapNumber = mapNumber;
        return this.AddProgressAsync(
            player,
            WeeklyQuestObjectiveType.EnterMap,
            1,
            (_, objective, _) => objective.Map is not null && objective.Map.Number == mapNumber);
    }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var now = DateTime.UtcNow;
        if (now >= this._nextPurgeUtc)
        {
            this._nextPurgeUtc = now + PurgeInterval;
            await this.PurgeExpiredProgressAsync(gameContext).ConfigureAwait(false);
        }

        if (now < this._nextSaveUtc)
        {
            return;
        }

        this._nextSaveUtc = now + SaveInterval;
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
                player.Logger.LogError(ex, "Unexpected error when saving the quest progress periodically.");
            }
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        this._nextSaveUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the overview of the available quests and the progress of the player.
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

        return this.CreateOverview(configuration, state, player);
    }

    /// <summary>
    /// Sends the available quests and the progress of the player to the client, so that it can show them in a window.
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

    private static bool CountsAsPlayerKill(WeeklyQuestDefinition quest, WeeklyQuestObjective objective, WeeklyQuestPlayerState state, Player killer, Player victim)
    {
        if (victim.Level < objective.MinimumVictimLevel || !objective.IsOnMap(killer.CurrentMap?.Definition.Number))
        {
            return false;
        }

        if (objective.IgnoreSameIp
            && (killer as IHasIpAddress)?.IpAddress is { Length: > 0 } killerIp
            && killerIp == (victim as IHasIpAddress)?.IpAddress)
        {
            return false;
        }

        if (objective.IgnoreSameGuild
            && killer.GuildStatus is { } killerGuild
            && killerGuild.GuildId == victim.GuildStatus?.GuildId)
        {
            return false;
        }

        if (objective.IgnoreSameParty && killer.Party is not null && killer.Party == victim.Party)
        {
            return false;
        }

        if (objective.VictimCooldownMinutes > 0 && victim.SelectedCharacter is { } victimCharacter)
        {
            var key = (quest.Id, victimCharacter.GetId());
            var now = DateTime.UtcNow;
            if (state.LastCountedVictimKills.TryGetValue(key, out var lastCounted)
                && now - lastCounted < TimeSpan.FromMinutes(objective.VictimCooldownMinutes))
            {
                return false;
            }

            state.LastCountedVictimKills[key] = now;
        }

        return true;
    }

    private static bool AreObjectivesDone(IReadOnlyList<WeeklyQuestObjective> objectives, WeeklyQuestProgress? progress, int count)
    {
        for (var i = 0; i < count; i++)
        {
            if ((progress?.GetCount(i) ?? 0) < Math.Max(1, objectives[i].RequiredCount))
            {
                return false;
            }
        }

        return true;
    }

    private static WeeklyQuestObjective? GetNextObjective(IReadOnlyList<WeeklyQuestObjective> objectives, WeeklyQuestProgress progress)
    {
        for (var i = 0; i < objectives.Count; i++)
        {
            if (progress.GetCount(i) < Math.Max(1, objectives[i].RequiredCount))
            {
                return objectives[i];
            }
        }

        return null;
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

    private static (DateTime Weekly, DateTime Daily) GetNextResetsUtc(WeeklyQuestPlayerState state, Player player)
    {
        var timeZone = player.GameContext.ServerTimeZone;
        return (WeeklyPeriod.GetNextPeriodStartUtc(state.Periods.Weekly, timeZone), WeeklyPeriod.GetNextDailyPeriodStartUtc(state.Periods.Daily, timeZone));
    }

    /// <summary>
    /// Gets the index of the objectives of the current configuration, and builds it again when the configuration changed.
    /// </summary>
    private WeeklyQuestIndex? GetIndex()
    {
        if (this.Configuration is not { } configuration)
        {
            return null;
        }

        var index = this._index;
        if (index is null || index.IsOutdated(configuration))
        {
            index = new WeeklyQuestIndex(configuration);
            this._index = index;
        }

        return index;
    }

    /// <summary>
    /// Gets the available quests of the player. They're cached, because they're needed for every kill,
    /// and only change with the level, resets, class, the period or when a quest has been completed.
    /// </summary>
    private HashSet<WeeklyQuestDefinition> GetAvailableQuestSet(WeeklyQuestsConfiguration configuration, WeeklyQuestIndex index, WeeklyQuestPlayerState state, WeeklyQuestCharacterInfo character)
    {
        this.GetAvailableQuests(configuration, index, state, character);
        return state.AvailableQuestSet!;
    }

    private List<WeeklyQuestDefinition> GetAvailableQuests(WeeklyQuestsConfiguration configuration, WeeklyQuestIndex index, WeeklyQuestPlayerState state, WeeklyQuestCharacterInfo character)
    {
        var key = (index, character.Class?.Number ?? -1, character.Level, character.Resets);
        if (state.AvailableQuests is { } cached && state.AvailableQuestSet is not null && state.AvailableQuestsKey == key)
        {
            return cached;
        }

        var quests = WeeklyQuestSelector.GetAvailableQuests(configuration, state.Periods, character, state.Progress, state.RewardedByOtherCharacters);
        state.AvailableQuests = quests;
        state.AvailableQuestSet = quests.ToHashSet();
        state.AvailableQuestsKey = key;
        return quests;
    }

    private WeeklyQuestOverview CreateOverview(WeeklyQuestsConfiguration configuration, WeeklyQuestPlayerState state, Player player)
    {
        var index = this.GetIndex()!;
        var quests = this.GetAvailableQuests(configuration, index, state, GetCharacterInfo(player));
        var entries = WeeklyQuestSelector.CreateEntries(configuration, quests, state.Progress);
        var (nextReset, nextDailyReset) = GetNextResetsUtc(state, player);
        return new WeeklyQuestOverview(entries, nextReset, nextDailyReset);
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

    private async ValueTask AddProgressAsync(Player player, WeeklyQuestObjectiveType objectiveType, int amount, Func<WeeklyQuestDefinition, WeeklyQuestObjective, WeeklyQuestPlayerState, bool>? filter = null)
    {
        if (this.Configuration is not { } configuration || this.GetIndex() is not { } index)
        {
            return;
        }

        // A quick check before the state is loaded, as most events don't concern any quest.
        var candidates = index.Get(objectiveType);
        if (candidates.Count == 0 || this.GetOrCreateState(player) is not { } state)
        {
            return;
        }

        try
        {
            using var l = await state.Lock.LockAsync().ConfigureAwait(false);
            var previousPeriods = state.IsLoaded ? state.Periods : (QuestPeriodStarts?)null;
            if (!await this.EnsureLoadedAsync(player, state).ConfigureAwait(false))
            {
                return;
            }

            var character = GetCharacterInfo(player);
            var available = this.GetAvailableQuestSet(configuration, index, state, character);
            var hasCompleted = false;
            var sendImmediately = false;
            var changedQuests = new List<WeeklyQuestDefinition>();
            foreach (var (quest, objectiveIndex, objective) in candidates)
            {
                if (!available.Contains(quest) || !WeeklyQuestSelector.MeetsMinimumRequirements(quest, character))
                {
                    continue;
                }

                state.Progress.TryGetValue(quest.Id, out var progress);
                var required = Math.Max(1, objective.RequiredCount);
                var previousCount = progress?.GetCount(objectiveIndex) ?? 0;
                if (progress?.CompletedAt is not null || previousCount >= required)
                {
                    continue;
                }

                var objectives = quest.GetObjectives();
                if (quest.SequentialObjectives
                    && (!AreObjectivesDone(objectives, progress, objectiveIndex) || changedQuests.Contains(quest)))
                {
                    // Only the current step counts, and one event makes progress in only one step.
                    continue;
                }

                if (filter is not null && !filter(quest, objective, state))
                {
                    continue;
                }

                progress ??= state.GetOrCreateProgress(quest.Id, quest.Period);
                var count = (int)Math.Min((long)previousCount + amount, required);
                progress.SetCount(objectiveIndex, count);
                state.DirtyQuestIds.Add(quest.Id);
                if (!changedQuests.Contains(quest))
                {
                    changedQuests.Add(quest);
                }

                if (count >= required)
                {
                    sendImmediately = true;
                    if (AreObjectivesDone(objectives, progress, objectives.Count))
                    {
                        progress.CompletedAt = DateTime.UtcNow;
                        hasCompleted = true;

                        // The completed quest may be the prerequisite of another one.
                        state.InvalidateAvailableQuests();
                        await this.TryRewardAsync(player, quest, progress, true).ConfigureAwait(false);
                    }
                    else if (GetNextObjective(objectives, progress) is { } next)
                    {
                        await ShowGoldenMessageAsync(player, Format(configuration.StepCompletedMessage, player, quest.Name, next.GetDisplayText(player.Culture))).ConfigureAwait(false);
                    }
                }
                else if (Milestones.Any(m => previousCount * 100 / required < m && count * 100 / required >= m))
                {
                    await player.ShowBlueMessageAsync(Format(configuration.ProgressMessage, player, quest.Name, count, required)).ConfigureAwait(false);
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

            var isNewPeriod = previousPeriods is not null && previousPeriods != state.Periods;
            await this.SendChangesAsync(player, state, configuration, changedQuests, isNewPeriod, sendImmediately).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Unexpected error when adding quest progress of type {objectiveType}.", objectiveType);
        }
    }

    /// <summary>
    /// Updates the progress of the <see cref="WeeklyQuestsConfiguration.AllCompletedBonus"/>, and rewards it when all weekly quests are completed.
    /// </summary>
    /// <returns>The quest of the bonus, if its progress changed.</returns>
    private async ValueTask<WeeklyQuestDefinition?> UpdateAllCompletedBonusAsync(Player player, WeeklyQuestPlayerState state, WeeklyQuestsConfiguration configuration)
    {
        if (!WeeklyQuestSelector.HasAllCompletedBonus(configuration))
        {
            return null;
        }

        var quests = this.GetAvailableQuests(configuration, this.GetIndex()!, state, GetCharacterInfo(player))
            .Where(q => q.Period == QuestPeriod.Weekly)
            .ToList();
        if (quests.Count == 0 || WeeklyQuestSelector.CreateAllCompletedBonusQuest(configuration, quests.Count) is not { } bonusQuest)
        {
            return null;
        }

        var completedCount = quests.Count(q => state.Progress.TryGetValue(q.Id, out var p) && p.CompletedAt is not null);
        var progress = state.GetOrCreateProgress(bonusQuest.Id, QuestPeriod.Weekly);
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

    private async ValueTask SendChangesAsync(Player player, WeeklyQuestPlayerState state, WeeklyQuestsConfiguration configuration, List<WeeklyQuestDefinition> changedQuests, bool isNewPeriod, bool sendImmediately)
    {
        // After a reset, all quests of the client are outdated, not only the changed ones.
        if (isNewPeriod)
        {
            state.PendingUpdateIds.Clear();
            var overview = this.CreateOverview(configuration, state, player);
            await player.InvokeViewPlugInAsync<IWeeklyQuestListViewPlugIn>(p => p.ShowWeeklyQuestsAsync(overview)).ConfigureAwait(false);
            return;
        }

        if (changedQuests.Count == 0)
        {
            return;
        }

        foreach (var quest in changedQuests)
        {
            state.PendingUpdateIds.Add(quest.Id);
        }

        if (sendImmediately)
        {
            await this.SendPendingUpdatesAsync(player, state, configuration).ConfigureAwait(false);
            return;
        }

        // Many kills in a short time are sent as one update per quest, instead of one message per kill.
        if (!state.IsUpdateScheduled)
        {
            state.IsUpdateScheduled = true;
            _ = this.SendPendingUpdatesLaterAsync(player, state);
        }
    }

    private async Task SendPendingUpdatesLaterAsync(Player player, WeeklyQuestPlayerState state)
    {
        try
        {
            await Task.Delay(this.UpdateDelay).ConfigureAwait(false);
            using var l = await state.Lock.LockAsync().ConfigureAwait(false);
            state.IsUpdateScheduled = false;
            if (this.Configuration is { } configuration
                && States.TryGetValue(player, out var currentState)
                && currentState == state)
            {
                await this.SendPendingUpdatesAsync(player, state, configuration).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Unexpected error when sending the quest progress.");
        }
    }

    private async ValueTask SendPendingUpdatesAsync(Player player, WeeklyQuestPlayerState state, WeeklyQuestsConfiguration configuration)
    {
        if (state.PendingUpdateIds.Count == 0 || this.GetIndex() is not { } index)
        {
            return;
        }

        var entries = new List<WeeklyQuestOverviewEntry>(state.PendingUpdateIds.Count);
        foreach (var questId in state.PendingUpdateIds)
        {
            var quest = questId == WeeklyQuestSelector.AllCompletedBonusId
                ? WeeklyQuestSelector.CreateAllCompletedBonusQuest(configuration, WeeklyQuestSelector.CountBonusQuests(this.GetAvailableQuests(configuration, index, state, GetCharacterInfo(player))))
                : index.FindQuest(questId);
            if (quest is not null)
            {
                entries.Add(WeeklyQuestSelector.CreateEntry(quest, state.Progress));
            }
        }

        state.PendingUpdateIds.Clear();
        var (nextReset, nextDailyReset) = GetNextResetsUtc(state, player);
        foreach (var entry in entries)
        {
            await player.InvokeViewPlugInAsync<IWeeklyQuestListViewPlugIn>(p => p.UpdateWeeklyQuestAsync(entry, nextReset, nextDailyReset)).ConfigureAwait(false);
        }
    }

    private async ValueTask<bool> EnsureLoadedAsync(Player player, WeeklyQuestPlayerState state)
    {
        if (this.Configuration is not { } configuration)
        {
            return false;
        }

        var periods = WeeklyPeriod.GetPeriodStarts(DateTime.UtcNow, configuration, player.GameContext.ServerTimeZone);
        if (state.IsLoaded && state.Periods == periods)
        {
            return true;
        }

        try
        {
            if (state.IsLoaded)
            {
                // A new day or week began while the player is online. The progress of the last period is saved first.
                await this.SaveAsync(player, state).ConfigureAwait(false);
                state.IsLoaded = false;
            }

            // One query for the progress of all periods: the current day, week and the quests which are done once.
            var loaded = await this.Repository.LoadAsync(state.CharacterId, periods.All).ConfigureAwait(false);
            state.RewardedByOtherCharacters = await this.LoadRewardedByOtherCharactersAsync(state, periods).ConfigureAwait(false);
            state.Progress = this.SelectCurrentProgress(loaded, periods);
            state.Periods = periods;
            state.DirtyQuestIds.Clear();
            state.InvalidateAvailableQuests();
            state.IsLoaded = true;
            return true;
        }
        catch (Exception ex)
        {
            player.Logger.LogWarning(ex, "Couldn't load the quest progress of character {characterId}.", state.CharacterId);
            return false;
        }
    }

    /// <summary>
    /// Selects the progress entries which belong to the current period of their quest.
    /// </summary>
    /// <remarks>
    /// When the period of a quest was changed in the configuration, there may be entries of the same quest
    /// in several of the loaded periods. Only the one of the configured period counts.
    /// </remarks>
    private Dictionary<string, WeeklyQuestProgress> SelectCurrentProgress(IEnumerable<WeeklyQuestProgress> loaded, QuestPeriodStarts periods)
    {
        var index = this.GetIndex();
        var result = new Dictionary<string, WeeklyQuestProgress>();
        foreach (var progress in loaded)
        {
            var period = progress.QuestId == WeeklyQuestSelector.AllCompletedBonusId
                ? QuestPeriod.Weekly
                : index?.FindQuest(progress.QuestId)?.Period;
            if (period is { } p && progress.PeriodStart == periods.Get(p))
            {
                result[progress.QuestId] = progress;
            }
        }

        return result;
    }

    private async ValueTask<HashSet<string>> LoadRewardedByOtherCharactersAsync(WeeklyQuestPlayerState state, QuestPeriodStarts periods)
    {
        // Only one character of an account can be in the game at the same time, so loading it once per period is enough.
        if (state.AccountId is not { } accountId
            || this.Configuration?.Quests.Any(q => q.OncePerAccount) != true)
        {
            return new HashSet<string>();
        }

        var index = this.GetIndex();
        var rewarded = await this.Repository.LoadRewardedByAccountAsync(accountId, periods.All).ConfigureAwait(false);
        return rewarded
            .Where(p => p.CharacterId != state.CharacterId
                        && index?.FindQuest(p.QuestId) is { } quest
                        && p.PeriodStart == periods.Get(quest.Period))
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
            player.Logger.LogError(ex, "Couldn't save the quest progress of character {characterId}.", state.CharacterId);
        }
    }

    private async ValueTask PurgeExpiredProgressAsync(GameContext gameContext)
    {
        if (this.Configuration is not { HistoryWeeks: > 0 } configuration)
        {
            return;
        }

        try
        {
            var olderThan = DateTime.UtcNow.AddDays(-7 * configuration.HistoryWeeks);
            var deleted = await this.Repository.DeleteExpiredAsync(olderThan, WeeklyPeriod.OncePeriodStartUtc).ConfigureAwait(false);
            if (deleted > 0)
            {
                gameContext.LoggerFactory.CreateLogger<WeeklyQuestsPlugIn>().LogInformation("Deleted {count} quest progress entries of periods before {olderThan}.", deleted, olderThan);
            }
        }
        catch (Exception ex)
        {
            gameContext.LoggerFactory.CreateLogger<WeeklyQuestsPlugIn>().LogWarning(ex, "Couldn't delete the quest progress of past periods.");
        }
    }

    private async ValueTask RewardPendingAsync(Player player, WeeklyQuestPlayerState state, bool showPendingMessage)
    {
        if (this.Configuration is not { } configuration)
        {
            return;
        }

        var index = this.GetIndex();
        var rewarded = false;
        foreach (var progress in state.Progress.Values.Where(p => p.CompletedAt is not null && p.RewardedAt is null).ToList())
        {
            var quest = progress.QuestId == WeeklyQuestSelector.AllCompletedBonusId
                ? WeeklyQuestSelector.CreateAllCompletedBonusQuest(configuration, progress.Count)
                : index?.FindQuest(progress.QuestId);
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
        player.Logger.LogInformation("Character {characterId} completed the quest {quest}.", progress.CharacterId, quest.Id);
        return true;
    }
}
