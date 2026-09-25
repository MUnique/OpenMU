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
    public WeeklyQuestPlayerState(WeeklyQuestsPlugIn owner, Guid characterId)
    {
        this.Owner = owner;
        this.CharacterId = characterId;
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
                PeriodStart = this.PeriodStartUtc,
                QuestId = questId,
            };
            this.Progress.Add(questId, progress);
        }

        return progress;
    }
}
