// <copyright file="WeeklyQuestsConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The configuration of the <see cref="WeeklyQuestsPlugIn"/>.
/// </summary>
public class WeeklyQuestsConfiguration
{
    /// <summary>
    /// Gets or sets the day of the week on which a new weekly period starts.
    /// </summary>
    [Display(Name = "Día de reinicio", Description = "Día de la semana en que se reinicia el progreso de todas las quests semanales.")]
    public DayOfWeek ResetDay { get; set; } = DayOfWeek.Monday;

    /// <summary>
    /// Gets or sets the time of the <see cref="ResetDay"/>, in the time zone of the server, on which a new weekly period starts.
    /// </summary>
    [Display(Name = "Hora de reinicio", Description = "Hora del día de reinicio, en la zona horaria del servidor.")]
    public TimeOnly ResetTime { get; set; } = TimeOnly.MinValue;

    /// <summary>
    /// Gets or sets the time of the day, in the time zone of the server, on which a new daily period starts.
    /// </summary>
    [Display(Name = "Hora de reinicio diario", Description = "Hora del día en que se reinician las quests diarias, en la zona horaria del servidor.")]
    public TimeOnly DailyResetTime { get; set; } = TimeOnly.MinValue;

    /// <summary>
    /// Gets or sets the number of quests which are drawn each week from the active weekly quests.
    /// The quests which are <see cref="WeeklyQuestDefinition.AlwaysIncluded"/> are added to them.
    /// 0 means that all active quests are available every week.
    /// </summary>
    [Display(Name = "Quests por semana", Description = "Solo quests con reinicio semanal. 0 = sin rotación, todas las activas. Si es mayor, cada semana se sortean esa cantidad entre las activas (más las \"Siempre incluidas\"). El sorteo es igual para todos; una quest exclusiva de una clase que sale sorteada deja a las demás clases con menos quests esa semana.")]
    [Range(0, int.MaxValue)]
    public int QuestsPerWeek { get; set; }

    /// <summary>
    /// Gets or sets the number of quests which are drawn each day from the active daily quests.
    /// 0 means that all active daily quests are available every day.
    /// </summary>
    [Display(Name = "Quests por día", Description = "Solo quests con reinicio diario. 0 = sin rotación, todas las activas. Si es mayor, cada día se sortean esa cantidad (más las \"Siempre incluidas\").")]
    [Range(0, int.MaxValue)]
    public int QuestsPerDay { get; set; }

    /// <summary>
    /// Gets or sets the number of weeks for which the progress of past daily and weekly periods is kept in the database.
    /// 0 means that it's kept forever. The progress of the quests which are done once is always kept.
    /// </summary>
    [Display(Name = "Semanas de historial", Description = "El progreso de días y semanas anteriores se borra de la base después de estas semanas. 0 = nunca. Las quests de una sola vez nunca se borran.")]
    [Range(0, int.MaxValue)]
    public int HistoryWeeks { get; set; } = 8;

    /// <summary>
    /// Gets or sets a value indicating whether the monster kills count for the party members nearby, like the experience.
    /// </summary>
    [Display(Name = "Kills compartidos en party", Description = "Los kills de monstruos cuentan para los miembros de la party que estén cerca, igual que la experiencia.")]
    public bool ShareKillsWithParty { get; set; } = true;

    /// <summary>
    /// Gets or sets the bonus which is given when a character completed all of its quests of the week.
    /// Its <see cref="WeeklyQuestDefinition.Name"/>, <see cref="WeeklyQuestDefinition.Description"/> and
    /// <see cref="WeeklyQuestDefinition.Rewards"/> are used. Without rewards, there is no bonus.
    /// </summary>
    [Display(Name = "Bonus por completar todas", Description = "Premio extra al completar todas las quests con reinicio semanal (las diarias y las de historia no cuentan). Se usan Nombre, Descripción y Premios; el resto de los campos se ignora. Sin premios = sin bonus.")]
    [MemberOfAggregate]
    public WeeklyQuestDefinition? AllCompletedBonus { get; set; }

    /// <summary>
    /// Gets or sets the message which is shown when the progress of a quest reached a milestone (25%, 50%, 75%).
    /// Placeholders: {0} = quest name, {1} = current count, {2} = required count.
    /// </summary>
    [Display(Name = "Mensaje de progreso", Description = "{0} = nombre de la quest, {1} = progreso actual, {2} = objetivo.")]
    public LocalizedString ProgressMessage { get; set; } = "[Quest] {0}: {1}/{2}";

    /// <summary>
    /// Gets or sets the message which is shown when a step of a quest with several objectives has been completed.
    /// Placeholders: {0} = quest name, {1} = text of the next step.
    /// </summary>
    [Display(Name = "Mensaje de paso completado", Description = "{0} = nombre de la quest, {1} = el siguiente paso.")]
    public LocalizedString StepCompletedMessage { get; set; } = "[Quest] {0} - siguiente: {1}";

    /// <summary>
    /// Gets or sets the message which is shown when a quest has been completed and rewarded.
    /// Placeholder: {0} = quest name.
    /// </summary>
    [Display(Name = "Mensaje de quest completada", Description = "{0} = nombre de la quest.")]
    public LocalizedString CompletedMessage { get; set; } = "¡Quest completada: {0}!";

    /// <summary>
    /// Gets or sets the message which is shown when the rewards of a completed quest couldn't be handed out.
    /// Placeholder: {0} = quest name.
    /// </summary>
    [Display(Name = "Mensaje de premio pendiente", Description = "Se muestra cuando no hay lugar en el inventario o se supera el máximo de zen. {0} = nombre de la quest.")]
    public LocalizedString RewardPendingMessage { get; set; } = "[Quest] Liberá espacio en el inventario para recibir el premio de {0}.";

    /// <summary>
    /// Gets or sets the quests.
    /// </summary>
    [Display(Name = "Quests", Description = "Todas las quests: historia, diarias, semanales, de clase y de zona. El progreso se guarda por el Id, así que no lo cambies en una quest que ya está en curso.")]
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    public ICollection<WeeklyQuestDefinition> Quests { get; set; } = new List<WeeklyQuestDefinition>();

    /// <summary>
    /// Gets the default configuration, with a few example quests which are not active.
    /// </summary>
    public static WeeklyQuestsConfiguration Default => new()
    {
        Quests = new List<WeeklyQuestDefinition>
        {
            new()
            {
                Id = "kill-500",
                Name = "Cazador",
                Description = "Matá 500 monstruos.",
                IsActive = false,
                ObjectiveType = WeeklyQuestObjectiveType.KillAnyMonster,
                RequiredCount = 500,
                Rewards = new List<WeeklyQuestReward>
                {
                    new() { RewardType = WeeklyQuestRewardType.Money, Amount = 5_000_000 },
                },
            },
            new()
            {
                Id = "bc-3",
                Name = "Defensor del castillo",
                Description = "Terminá 3 Blood Castle.",
                IsActive = false,
                ObjectiveType = WeeklyQuestObjectiveType.CompleteMiniGame,
                MiniGameType = MiniGameType.BloodCastle,
                RequiredCount = 3,
                Rewards = new List<WeeklyQuestReward>
                {
                    new() { RewardType = WeeklyQuestRewardType.Experience, Amount = 1_000_000 },
                },
            },
            new()
            {
                Id = "daily-kill-100",
                Name = "Patrulla diaria",
                Description = "Matá 100 monstruos hoy.",
                IsActive = false,
                Category = QuestCategory.Daily,
                Period = QuestPeriod.Daily,
                ObjectiveType = WeeklyQuestObjectiveType.KillAnyMonster,
                RequiredCount = 100,
                Rewards = new List<WeeklyQuestReward>
                {
                    new() { RewardType = WeeklyQuestRewardType.Money, Amount = 500_000 },
                },
            },
            new()
            {
                // The NPC, the monsters, the item and the map have to be chosen in the admin panel.
                Id = "main-1",
                Name = "Capítulo I - El regreso de Kundun",
                Description = "El Guardián presiente que Kundun vuelve. Averiguá qué está pasando.",
                IsActive = false,
                Category = QuestCategory.Main,
                Period = QuestPeriod.Once,
                SequentialObjectives = true,
                Objectives = new List<WeeklyQuestObjective>
                {
                    new() { Description = "Habla con el Guardián", ObjectiveType = WeeklyQuestObjectiveType.TalkToNpc },
                    new() { Description = "Mata 50 Skeletons", ObjectiveType = WeeklyQuestObjectiveType.KillMonster, RequiredCount = 50 },
                    new() { Description = "Encuentra el Fragmento del Chaos", ObjectiveType = WeeklyQuestObjectiveType.CollectItem },
                    new() { Description = "Derrota al jefe", ObjectiveType = WeeklyQuestObjectiveType.KillMonster },
                },
                Rewards = new List<WeeklyQuestReward>
                {
                    new() { RewardType = WeeklyQuestRewardType.Experience, Amount = 5_000_000 },
                },
            },
            new()
            {
                Id = "main-2",
                Name = "Capítulo II - Las puertas de Lost Tower",
                Description = "Seguí el rastro de Kundun.",
                IsActive = false,
                Category = QuestCategory.Main,
                Period = QuestPeriod.Once,
                PrerequisiteQuestId = "main-1",
                ObjectiveType = WeeklyQuestObjectiveType.EnterMap,
            },
        },
    };
}
