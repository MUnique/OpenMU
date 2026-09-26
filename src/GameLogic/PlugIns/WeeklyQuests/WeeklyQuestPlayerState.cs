// <copyright file="WeeklyQuestPlayerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

using MUnique.OpenMU.Persistence.WeeklyQuests;
using Nito.AsyncEx;

/// <summary>
/// The quest progress of a player in the current periods, which is kept in memory
/// while the player is in the game.
/// </summary>
internal sealed class WeeklyQuestPlayerState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WeeklyQuestPlayerState"/> class.
    /// </summary>
    /// <param name="owner">The plugin which tracks the progress.</param>
    /// <param name="characterId">The identifier of the character.</param>
    /// <param name="accountId">The identifier of the account of the character.</param>
    public WeeklyQuestPlayerState(WeeklyQuestsPlugIn owner, Guid characterId, Guid? accountId)
    {
        this.Owner = owner;
        this.CharacterId = characterId;
        this.AccountId = accountId;
    }

    /// <summary>
    /// Gets the plugin which tracks the progress.
    /// </summary>
    public WeeklyQuestsPlugIn Owner { get; }

    /// <summary>
    /// Gets the lock which has to be held while accessing the state.
    /// </summary>
    public AsyncLock Lock { get; } = new();

    /// <summary>
    /// Gets the identifier of the character.
    /// </summary>
    public Guid CharacterId { get; }

    /// <summary>
    /// Gets the identifier of the account of the character.
    /// </summary>
    public Guid? AccountId { get; }

    /// <summary>
    /// Gets or sets the ids of the quests whose rewards another character of the account already received in the current periods.
    /// </summary>
    public HashSet<string> RewardedByOtherCharacters { get; set; } = new();

    /// <summary>
    /// Gets the last time (UTC) per victim character when a player kill counted for a quest.
    /// It's only kept in memory, so it starts over after leaving the game.
    /// </summary>
    public Dictionary<(string QuestId, Guid VictimId), DateTime> LastCountedVictimKills { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the progress of the <see cref="Periods"/> has been loaded.
    /// </summary>
    public bool IsLoaded { get; set; }

    /// <summary>
    /// Gets or sets the starts of the periods of the loaded progress.
    /// </summary>
    public QuestPeriodStarts Periods { get; set; }

    /// <summary>
    /// Gets or sets the progress per quest id.
    /// </summary>
    public Dictionary<string, WeeklyQuestProgress> Progress { get; set; } = new();

    /// <summary>
    /// Gets the ids of the quests whose progress has not been saved yet.
    /// </summary>
    public HashSet<string> DirtyQuestIds { get; } = new();

    /// <summary>
    /// Gets the ids of the quests whose progress has not been sent to the client yet.
    /// </summary>
    public HashSet<string> PendingUpdateIds { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether sending the <see cref="PendingUpdateIds"/> has been scheduled.
    /// </summary>
    public bool IsUpdateScheduled { get; set; }

    /// <summary>
    /// Gets or sets the number of the map which the player entered last, so that a respawn on the same map doesn't count as entering it.
    /// </summary>
    public short? LastMapNumber { get; set; }

    /// <summary>
    /// Gets or sets the cached available quests. See <see cref="AvailableQuestsKey"/>.
    /// </summary>
    public List<WeeklyQuestDefinition>? AvailableQuests { get; set; }

    /// <summary>
    /// Gets or sets the cached available quests as set, for fast lookups.
    /// </summary>
    public HashSet<WeeklyQuestDefinition>? AvailableQuestSet { get; set; }

    /// <summary>
    /// Gets or sets the data of the character and configuration for which the <see cref="AvailableQuests"/> were determined.
    /// </summary>
    public (WeeklyQuestIndex Index, int ClassNumber, int Level, int Resets) AvailableQuestsKey { get; set; }

    /// <summary>
    /// Forgets the cached <see cref="AvailableQuests"/>, e.g. after a quest has been completed which is the prerequisite of another.
    /// </summary>
    public void InvalidateAvailableQuests()
    {
        this.AvailableQuests = null;
        this.AvailableQuestSet = null;
    }

    /// <summary>
    /// Gets the progress of the quest, and creates it if it doesn't exist yet.
    /// </summary>
    /// <param name="questId">The quest identifier.</param>
    /// <param name="period">The period of the quest.</param>
    /// <returns>The progress.</returns>
    public WeeklyQuestProgress GetOrCreateProgress(string questId, QuestPeriod period)
    {
        if (!this.Progress.TryGetValue(questId, out var progress))
        {
            progress = new WeeklyQuestProgress
            {
                CharacterId = this.CharacterId,
                AccountId = this.AccountId,
                PeriodStart = this.Periods.Get(period),
                QuestId = questId,
            };
            this.Progress.Add(questId, progress);
        }

        return progress;
    }
}
