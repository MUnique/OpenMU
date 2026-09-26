// <copyright file="WeeklyQuestPlayerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

using MUnique.OpenMU.Persistence.WeeklyQuests;
using Nito.AsyncEx;

/// <summary>
/// The weekly quest progress of a player in the current period, which is kept in memory
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
    /// Gets or sets the ids of the quests whose rewards another character of the account already received in this period.
    /// </summary>
    public HashSet<string> RewardedByOtherCharacters { get; set; } = new();

    /// <summary>
    /// Gets the last time (UTC) per victim character when a player kill counted for a quest.
    /// It's only kept in memory, so it starts over after leaving the game.
    /// </summary>
    public Dictionary<(string QuestId, Guid VictimId), DateTime> LastCountedVictimKills { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the progress of the <see cref="PeriodStartUtc"/> has been loaded.
    /// </summary>
    public bool IsLoaded { get; set; }

    /// <summary>
    /// Gets or sets the start of the period of the loaded progress.
    /// </summary>
    public DateTime PeriodStartUtc { get; set; }

    /// <summary>
    /// Gets or sets the progress per quest id.
    /// </summary>
    public Dictionary<string, WeeklyQuestProgress> Progress { get; set; } = new();

    /// <summary>
    /// Gets the ids of the quests whose progress has not been saved yet.
    /// </summary>
    public HashSet<string> DirtyQuestIds { get; } = new();

    /// <summary>
    /// Gets the progress of the quest, and creates it if it doesn't exist yet.
    /// </summary>
    /// <param name="questId">The quest identifier.</param>
    /// <returns>The progress.</returns>
    public WeeklyQuestProgress GetOrCreateProgress(string questId)
    {
        if (!this.Progress.TryGetValue(questId, out var progress))
        {
            progress = new WeeklyQuestProgress
            {
                CharacterId = this.CharacterId,
                AccountId = this.AccountId,
                PeriodStart = this.PeriodStartUtc,
                QuestId = questId,
            };
            this.Progress.Add(questId, progress);
        }

        return progress;
    }
}
