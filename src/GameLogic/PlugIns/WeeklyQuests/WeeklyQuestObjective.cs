// <copyright file="WeeklyQuestObjective.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// An objective (step) of a <see cref="WeeklyQuestDefinition"/>, e.g. "Kill 50 Skeletons".
/// </summary>
public class WeeklyQuestObjective
{
    /// <summary>
    /// Gets or sets the text which is shown in the checklist of the quest.
    /// If it's empty, a text is generated from the objective.
    /// </summary>
    [Display(Name = "Texto", Description = "Opcional. Lo que ve el jugador en la lista de pasos, p. ej. \"Mata 50 Skeletons\". Vacío = se genera solo.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the objective.
    /// </summary>
    [Display(Name = "Tipo de objetivo")]
    public WeeklyQuestObjectiveType ObjectiveType { get; set; }

    /// <summary>
    /// Gets or sets the required count to complete the objective.
    /// </summary>
    [Display(Name = "Cantidad requerida")]
    [Range(1, int.MaxValue)]
    public int RequiredCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets the monster which has to be killed, for <see cref="WeeklyQuestObjectiveType.KillMonster"/>,
    /// or the NPC to talk to, for <see cref="WeeklyQuestObjectiveType.TalkToNpc"/>.
    /// </summary>
    [Display(Name = "Monstruo / NPC", Description = "Para \"Matar monstruo\" y \"Hablar con NPC\".")]
    public virtual MonsterDefinition? Monster { get; set; }

    /// <summary>
    /// Gets or sets the map on which the objective has to be done. If empty, every map counts.
    /// For <see cref="WeeklyQuestObjectiveType.EnterMap"/>, it's the map to enter.
    /// </summary>
    [Display(Name = "Mapa", Description = "Obligatorio para \"Entrar a un mapa\". Para los demás es opcional: vacío = cualquier mapa.")]
    public virtual GameMapDefinition? Map { get; set; }

    /// <summary>
    /// Gets or sets the item which has to be picked up, for <see cref="WeeklyQuestObjectiveType.CollectItem"/>.
    /// </summary>
    [Display(Name = "Item", Description = "Solo para \"Juntar items\". Solo cuentan los items que dropean monstruos o eventos, no los que tira un jugador.")]
    public virtual ItemDefinition? Item { get; set; }

    /// <summary>
    /// Gets or sets the minimum level of the picked up item, for <see cref="WeeklyQuestObjectiveType.CollectItem"/>.
    /// </summary>
    [Display(Name = "Nivel mínimo del item", Description = "Solo para \"Juntar items\".")]
    [Range(0, 15)]
    public int MinimumItemLevel { get; set; }

    /// <summary>
    /// Gets or sets the type of the mini game, for <see cref="WeeklyQuestObjectiveType.CompleteMiniGame"/>.
    /// <see cref="DataModel.Configuration.MiniGameType.Undefined"/> means any mini game.
    /// </summary>
    [Display(Name = "Evento", Description = "Solo para \"Completar evento\". Undefined = cualquier evento.")]
    public MiniGameType MiniGameType { get; set; }

    /// <summary>
    /// Gets or sets the minimum level of a killed player, for <see cref="WeeklyQuestObjectiveType.KillPlayer"/>.
    /// </summary>
    [Display(Name = "Nivel mínimo de la víctima", Description = "Solo para \"Matar jugadores\". Evita farmear con personajes bajos.")]
    [Range(0, int.MaxValue)]
    public int MinimumVictimLevel { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether kills of players with the same IP address don't count, for <see cref="WeeklyQuestObjectiveType.KillPlayer"/>.
    /// </summary>
    [Display(Name = "Ignorar misma IP", Description = "Solo para \"Matar jugadores\". No cuentan las víctimas conectadas desde la misma IP.")]
    public bool IgnoreSameIp { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether kills of players of the same guild don't count, for <see cref="WeeklyQuestObjectiveType.KillPlayer"/>.
    /// </summary>
    [Display(Name = "Ignorar misma guild", Description = "Solo para \"Matar jugadores\".")]
    public bool IgnoreSameGuild { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether kills of players of the same party don't count, for <see cref="WeeklyQuestObjectiveType.KillPlayer"/>.
    /// </summary>
    [Display(Name = "Ignorar misma party", Description = "Solo para \"Matar jugadores\".")]
    public bool IgnoreSameParty { get; set; } = true;

    /// <summary>
    /// Gets or sets the minutes until the same victim counts again, for <see cref="WeeklyQuestObjectiveType.KillPlayer"/>. 0 means no cooldown.
    /// </summary>
    [Display(Name = "Cooldown por víctima (minutos)", Description = "Solo para \"Matar jugadores\". 0 = sin cooldown. Matar otra vez a la misma víctima no cuenta hasta que pase este tiempo.")]
    [Range(0, int.MaxValue)]
    public int VictimCooldownMinutes { get; set; }

    /// <summary>
    /// Gets the text which describes the objective to a player.
    /// </summary>
    /// <param name="culture">The culture of the player.</param>
    /// <returns>The configured <see cref="Description"/>, or a generated text like "Matá 50 Skeleton".</returns>
    public string GetDisplayText(CultureInfo culture)
    {
        if (!string.IsNullOrWhiteSpace(this.Description))
        {
            return this.Description;
        }

        var count = this.RequiredCount.ToString("N0", culture);
        var monster = this.Monster?.Designation.GetTranslation(culture) ?? "?";
        var map = this.Map?.Name.GetTranslation(culture);
        var onMap = map is null ? string.Empty : $" en {map}";
        return this.ObjectiveType switch
        {
            WeeklyQuestObjectiveType.KillMonster => $"Matá {count} {monster}{onMap}",
            WeeklyQuestObjectiveType.KillAnyMonster => $"Matá {count} monstruos{onMap}",
            WeeklyQuestObjectiveType.GainLevels => $"Subí {count} niveles",
            WeeklyQuestObjectiveType.GainMasterLevels => $"Subí {count} niveles master",
            WeeklyQuestObjectiveType.GainResets => $"Hacé {count} resets",
            WeeklyQuestObjectiveType.CompleteMiniGame => this.MiniGameType == MiniGameType.Undefined
                ? $"Terminá {count} eventos"
                : $"Terminá {count} {this.MiniGameType}",
            WeeklyQuestObjectiveType.KillPlayer => $"Derrotá {count} jugadores{onMap}",
            WeeklyQuestObjectiveType.CollectItem => $"Juntá {count} {this.Item?.Name.GetTranslation(culture) ?? "?"}{(this.MinimumItemLevel > 0 ? $" +{this.MinimumItemLevel}" : string.Empty)}{onMap}",
            WeeklyQuestObjectiveType.TalkToNpc => $"Hablá con {monster}{onMap}",
            WeeklyQuestObjectiveType.EnterMap => $"Entrá a {map ?? "?"}",
            _ => $"{this.ObjectiveType} {count}",
        };
    }

    /// <summary>
    /// Determines whether a monster kill on the specified map counts for this objective.
    /// </summary>
    /// <param name="mapNumber">The number of the map of the kill.</param>
    /// <returns><c>true</c>, if the map matches.</returns>
    public bool IsOnMap(short? mapNumber) => this.Map is null || this.Map.Number == mapNumber;
}
