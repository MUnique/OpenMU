// <copyright file="WeeklyQuestObjectiveType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

/// <summary>
/// The type of the objective of a <see cref="WeeklyQuestDefinition"/>.
/// </summary>
public enum WeeklyQuestObjectiveType
{
    /// <summary>
    /// Kill a specific monster.
    /// </summary>
    [Display(Name = "Matar monstruo")]
    KillMonster,

    /// <summary>
    /// Kill any monster.
    /// </summary>
    [Display(Name = "Matar cualquier monstruo")]
    KillAnyMonster,

    /// <summary>
    /// Gain character levels.
    /// </summary>
    [Display(Name = "Subir niveles")]
    GainLevels,

    /// <summary>
    /// Gain master levels.
    /// </summary>
    [Display(Name = "Subir niveles master")]
    GainMasterLevels,

    /// <summary>
    /// Do character resets.
    /// </summary>
    [Display(Name = "Hacer resets")]
    GainResets,

    /// <summary>
    /// Stay in a mini game (e.g. Blood Castle) until its end.
    /// </summary>
    [Display(Name = "Completar evento")]
    CompleteMiniGame,

    /// <summary>
    /// Kill other players.
    /// </summary>
    [Display(Name = "Matar jugadores")]
    KillPlayer,

    /// <summary>
    /// Pick up a specific item which was dropped by a monster or an event.
    /// </summary>
    [Display(Name = "Juntar items")]
    CollectItem,

    /// <summary>
    /// Talk to a specific NPC.
    /// </summary>
    [Display(Name = "Hablar con NPC")]
    TalkToNpc,

    /// <summary>
    /// Enter a map.
    /// </summary>
    [Display(Name = "Entrar a un mapa")]
    EnterMap,
}
