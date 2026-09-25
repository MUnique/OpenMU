// <copyright file="WeeklyQuestDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// The definition of a weekly quest.
/// </summary>
public class WeeklyQuestDefinition
{
    /// <summary>
    /// Gets or sets the identifier of the quest. The progress of the characters is stored by it.
    /// </summary>
    [Display(Name = "Id", Description = "Identificador único y estable, p. ej. \"bc-3\". El progreso se guarda con este valor.")]
    [Required]
    [StringLength(64)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name which is shown to the player.
    /// </summary>
    [Display(Name = "Nombre")]
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description which is shown to the player.
    /// </summary>
    [Display(Name = "Descripción")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this quest is active.
    /// </summary>
    [Display(Name = "Activa")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum character level to make progress in this quest.
    /// </summary>
    [Display(Name = "Nivel mínimo")]
    [Range(0, int.MaxValue)]
    public int MinimumLevel { get; set; }

    /// <summary>
    /// Gets or sets the minimum reset count to make progress in this quest.
    /// </summary>
    [Display(Name = "Resets mínimos")]
    [Range(0, int.MaxValue)]
    public int MinimumResets { get; set; }

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
    /// Gets or sets the monster which has to be killed, for <see cref="WeeklyQuestObjectiveType.KillMonster"/>.
    /// </summary>
    [Display(Name = "Monstruo", Description = "Solo para \"Matar monstruo\".")]
    public virtual MonsterDefinition? Monster { get; set; }

    /// <summary>
    /// Gets or sets the map on which the kills have to happen. If empty, kills on every map count.
    /// </summary>
    [Display(Name = "Mapa", Description = "Opcional, para objetivos de kills. Vacío = cualquier mapa.")]
    public virtual GameMapDefinition? Map { get; set; }

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
    /// Gets or sets the rewards which are given when the quest has been completed.
    /// </summary>
    [Display(Name = "Premios")]
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    public ICollection<WeeklyQuestReward> Rewards { get; set; } = new List<WeeklyQuestReward>();

    /// <inheritdoc />
    public override string ToString() => $"{this.Id}: {this.Name}";
}
