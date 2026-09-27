// <copyright file="WeeklyQuestIndex.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

/// <summary>
/// An index of the objectives of a <see cref="WeeklyQuestsConfiguration"/> by their type, so that an event
/// (e.g. a monster kill) only looks at the objectives which it concerns, instead of all quests.
/// </summary>
internal sealed class WeeklyQuestIndex
{
    private static readonly IReadOnlyList<IndexedObjective> NoObjectives = [];

    private readonly ICollection<WeeklyQuestDefinition> _quests;
    private readonly int _questCount;
    private readonly Dictionary<WeeklyQuestObjectiveType, List<IndexedObjective>> _objectivesByType = new();
    private readonly Dictionary<string, WeeklyQuestDefinition> _questsById = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="WeeklyQuestIndex"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public WeeklyQuestIndex(WeeklyQuestsConfiguration configuration)
    {
        this.Configuration = configuration;
        this._quests = configuration.Quests;
        this._questCount = configuration.Quests.Count;
        foreach (var quest in configuration.Quests)
        {
            if (string.IsNullOrWhiteSpace(quest.Id))
            {
                continue;
            }

            this._questsById.TryAdd(quest.Id, quest);
            var objectives = quest.GetObjectives();
            for (var i = 0; i < objectives.Count; i++)
            {
                if (!this._objectivesByType.TryGetValue(objectives[i].ObjectiveType, out var list))
                {
                    list = new List<IndexedObjective>();
                    this._objectivesByType.Add(objectives[i].ObjectiveType, list);
                }

                list.Add(new IndexedObjective(quest, i, objectives[i]));
            }
        }
    }

    /// <summary>
    /// Gets the configuration of the index.
    /// </summary>
    public WeeklyQuestsConfiguration Configuration { get; }

    /// <summary>
    /// Determines whether the index doesn't reflect the specified configuration anymore.
    /// </summary>
    /// <param name="configuration">The current configuration.</param>
    /// <returns><c>true</c>, if the index has to be built again.</returns>
    public bool IsOutdated(WeeklyQuestsConfiguration configuration)
    {
        return !ReferenceEquals(configuration, this.Configuration)
               || !ReferenceEquals(configuration.Quests, this._quests)
               || configuration.Quests.Count != this._questCount;
    }

    /// <summary>
    /// Gets the objectives of the specified type, of all quests (including the inactive ones), ordered by quest and index.
    /// </summary>
    /// <param name="type">The type of the objectives.</param>
    /// <returns>The objectives.</returns>
    public IReadOnlyList<IndexedObjective> Get(WeeklyQuestObjectiveType type)
    {
        return this._objectivesByType.TryGetValue(type, out var list) ? list : NoObjectives;
    }

    /// <summary>
    /// Finds the quest with the specified identifier.
    /// </summary>
    /// <param name="questId">The identifier of the quest.</param>
    /// <returns>The quest, if it exists.</returns>
    public WeeklyQuestDefinition? FindQuest(string questId)
    {
        return this._questsById.GetValueOrDefault(questId);
    }

    /// <summary>
    /// An objective of a quest.
    /// </summary>
    /// <param name="Quest">The quest.</param>
    /// <param name="Index">The index of the objective in <see cref="WeeklyQuestDefinition.GetObjectives"/>.</param>
    /// <param name="Objective">The objective.</param>
    internal readonly record struct IndexedObjective(WeeklyQuestDefinition Quest, int Index, WeeklyQuestObjective Objective);
}
