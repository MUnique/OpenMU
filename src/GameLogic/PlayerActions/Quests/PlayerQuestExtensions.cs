// <copyright file="PlayerQuestExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Quests;

using MUnique.OpenMU.DataModel.Configuration.Quests;

/// <summary>
/// Quest related extension methods.
/// </summary>
public static class PlayerQuestExtensions
{
    /// <summary>
    /// Gets the state of the quest.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="group">The group of the quest.</param>
    /// <param name="number">The number of the quest.</param>
    /// <returns>The quest state, if found; Otherwise, <c>null</c>.</returns>
    public static CharacterQuestState? GetQuestState(this Player player, short group, short number)
    {
        return player.SelectedCharacter?.QuestStates.FirstOrDefault(state => state.Group == group && (state.ActiveQuest?.Number == number || state.ActiveQuest?.StartingNumber == number));
    }

    /// <summary>
    /// Gets the state of the quest of a group.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="group">The group of the quest.</param>
    /// <returns>The quest state, if found; Otherwise, <c>null</c>.</returns>
    public static CharacterQuestState? GetQuestState(this Player player, short group)
    {
        return player.SelectedCharacter?.QuestStates.FirstOrDefault(state => state.Group == group);
    }

    /// <summary>
    /// Gets the quest definition for the specified group and number depending on the players character class.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="group">The group.</param>
    /// <param name="number">The number.</param>
    /// <returns>The quest definition for the specified group and number depending on the players character class.</returns>
    public static QuestDefinition? GetQuest(this Player player, short group, short number)
    {
        var possibleQuestsOfGroup = player.OpenedNpc?.Definition.Quests
                .Where(q => q.Group == group)
                .Where(q => q.QualifiedCharacter is null || Equals(q.QualifiedCharacter, player.SelectedCharacter?.CharacterClass))
                .Where(q => !q.EvolvesIntoInactiveClass(player))
                .OrderBy(q => q.Number);

        return possibleQuestsOfGroup?.FirstOrDefault(q => q.StartingNumber == number || q.Number == number);
    }

    /// <summary>
    /// Gets the available quests of the currently opened NPC depending on the players character class.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The available quests of the currently opened NPC depending on the players character class.</returns>
    public static IEnumerable<QuestDefinition> GetAvailableQuestsOfOpenedNpc(this Player player)
    {
        return player.OpenedNpc?.Definition.Quests
                   .Where(q => q.QualifiedCharacter is null
                               || Equals(q.QualifiedCharacter, player.SelectedCharacter?.CharacterClass))
                   .Where(q => q.MinimumCharacterLevel <= player.Level
                               && (q.MaximumCharacterLevel == default || q.MaximumCharacterLevel >= player.Level))
                   .Where(q => !q.EvolvesIntoInactiveClass(player))
               ?? Enumerable.Empty<QuestDefinition>();
    }

    /// <summary>
    /// Determines whether the quest rewards the player with an evolution into the next
    /// generation class, which is inactive (<see cref="CharacterClass.IsActive"/>).
    /// </summary>
    /// <param name="quest">The quest.</param>
    /// <param name="player">The player.</param>
    /// <returns><c>true</c>, if the quest evolves the character of the player into an inactive class; Otherwise, <c>false</c>.</returns>
    public static bool EvolvesIntoInactiveClass(this QuestDefinition quest, Player player)
    {
        return player.SelectedCharacter?.CharacterClass?.NextGenerationClass is { IsActive: false }
               && quest.Rewards.Any(reward => reward.RewardType is QuestRewardType.CharacterEvolutionFirstToSecond or QuestRewardType.CharacterEvolutionSecondToThird);
    }
}