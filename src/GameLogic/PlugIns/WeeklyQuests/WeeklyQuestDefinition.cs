// <copyright file="WeeklyQuestDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.DataModel.Configuration.Items;

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
    /// Gets or sets a value indicating whether this quest is part of every week, when the quests rotate
    /// (<see cref="WeeklyQuestsConfiguration.QuestsPerWeek"/>). Otherwise, it's part of the drawing.
    /// </summary>
    [Display(Name = "Siempre incluida", Description = "Solo con rotación: la quest aparece todas las semanas y no entra en el sorteo.")]
    public bool AlwaysIncluded { get; set; }

    /// <summary>
    /// Gets or sets the minimum character level to make progress in this quest.
    /// </summary>
    [Display(Name = "Nivel mínimo")]
    [Range(0, int.MaxValue)]
    public int MinimumLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum character level for this quest. 0 means no limit.
    /// Characters above it don't see the quest, unless they already made progress in it.
    /// </summary>
    [Display(Name = "Nivel máximo", Description = "0 = sin límite. Arriba de este nivel la quest no se muestra, salvo que ya tenga progreso.")]
    [Range(0, int.MaxValue)]
    public int MaximumLevel { get; set; }

    /// <summary>
    /// Gets or sets the minimum reset count to make progress in this quest.
    /// </summary>
    [Display(Name = "Resets mínimos")]
    [Range(0, int.MaxValue)]
    public int MinimumResets { get; set; }

    /// <summary>
    /// Gets or sets the maximum reset count for this quest. 0 means no limit.
    /// Characters above it don't see the quest, unless they already made progress in it.
    /// </summary>
    [Display(Name = "Resets máximos", Description = "0 = sin límite. Arriba de estos resets la quest no se muestra, salvo que ya tenga progreso.")]
    [Range(0, int.MaxValue)]
    public int MaximumResets { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the rewards of this quest can be received only by one character per account and week.
    /// </summary>
    [Display(Name = "Una vez por cuenta", Description = "Si un personaje de la cuenta ya cobró el premio esta semana, los demás personajes no ven la quest.")]
    public bool OncePerAccount { get; set; }

    /// <summary>
    /// Gets or sets the character classes which can make progress in this quest.
    /// Each class includes its following <see cref="CharacterClass.NextGenerationClass"/>es,
    /// e.g. Dark Wizard includes Soul Master and Grand Master. If empty, every class qualifies.
    /// </summary>
    [Display(Name = "Clases permitidas", Description = "Opcional. Vacío = todas las clases. Cada clase incluye sus evoluciones: Dark Wizard = DW, SM y GrM; Soul Master = SM y GrM.")]
    public ICollection<CharacterClass> QualifiedCharacters { get; set; } = new List<CharacterClass>();

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
    [Display(Name = "Mapa", Description = "Opcional, para objetivos de kills y de juntar items. Vacío = cualquier mapa.")]
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
    /// Gets or sets the rewards which are given when the quest has been completed.
    /// </summary>
    [Display(Name = "Premios")]
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    public ICollection<WeeklyQuestReward> Rewards { get; set; } = new List<WeeklyQuestReward>();

    /// <summary>
    /// Determines whether a character of the specified class can make progress in this quest.
    /// </summary>
    /// <param name="characterClass">The class of the character.</param>
    /// <returns><c>true</c>, if the class is one of the <see cref="QualifiedCharacters"/> or one of their evolutions.</returns>
    public bool IsQualified(CharacterClass? characterClass)
    {
        if (this.QualifiedCharacters.Count == 0)
        {
            return true;
        }

        if (characterClass is null)
        {
            return false;
        }

        foreach (var qualified in this.QualifiedCharacters)
        {
            // The step limit guards against a misconfigured, circular chain of classes.
            var current = qualified;
            for (var steps = 0; current is not null && steps < 10; steps++)
            {
                if (current.Number == characterClass.Number)
                {
                    return true;
                }

                current = current.NextGenerationClass;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the text which describes all rewards to a player.
    /// </summary>
    /// <param name="culture">The culture of the player.</param>
    /// <returns>The rewards, separated by commas.</returns>
    public string GetRewardsText(System.Globalization.CultureInfo culture)
    {
        return string.Join(", ", this.Rewards.Select(r => r.GetDisplayText(culture)));
    }

    /// <inheritdoc />
    public override string ToString() => $"{this.Id}: {this.Name}";
}
