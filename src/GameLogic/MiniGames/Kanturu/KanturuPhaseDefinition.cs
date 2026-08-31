// <copyright file="KanturuPhaseDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// The definition of one phase of the Kanturu event. The <see cref="KanturuContext"/> runs the
/// <see cref="KanturuEventDefinition.Phases"/> one after another.
/// </summary>
public class KanturuPhaseDefinition
{
    /// <summary>
    /// Gets or sets the name of the phase. It's only used for logging.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the kind of the phase, which decides how it's executed.
    /// </summary>
    public KanturuPhaseKind Kind { get; set; }

    /// <summary>
    /// Gets or sets the main state which is sent to the clients when the phase starts.
    /// </summary>
    public KanturuState State { get; set; }

    /// <summary>
    /// Gets or sets the detail state which is sent to the clients when the phase starts.
    /// </summary>
    public byte DetailState { get; set; }

    /// <summary>
    /// Gets or sets the time limit which is shown in the client HUD when the phase starts.
    /// </summary>
    public TimeSpan? TimeLimit { get; set; }

    /// <summary>
    /// Gets or sets the number of the spawn wave which is started with the phase. It refers to
    /// the <see cref="MUnique.OpenMU.DataModel.Configuration.MonsterSpawnArea.WaveNumber"/> of
    /// the spawn areas of the event map.
    /// </summary>
    public byte? SpawnWaveNumber { get; set; }

    /// <summary>
    /// Gets or sets the delay between sending the <see cref="State"/> to the clients and
    /// starting the <see cref="SpawnWaveNumber"/>, e.g. to give the clients time to play an
    /// intro animation before the monsters appear.
    /// </summary>
    public TimeSpan StartDelay { get; set; }

    /// <summary>
    /// Gets or sets the number of monsters which need to be killed to finish the phase.
    /// It's also the initial value of the monster counter in the client HUD.
    /// </summary>
    public int KillTarget { get; set; }

    /// <summary>
    /// Gets or sets the numbers of the monsters which count towards the <see cref="KillTarget"/>.
    /// </summary>
    public IList<short> CountedMonsterNumbers { get; set; } = new List<short>();

    /// <summary>
    /// Gets or sets the duration after which the phase is finished, regardless of the
    /// <see cref="KillTarget"/>. If it's not set, the phase runs until the kill target is reached.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Gets or sets the key of the localized message which is shown when the phase starts.
    /// </summary>
    public string? StartMessageKey { get; set; }

    /// <summary>
    /// Gets or sets the key of the localized message which is shown when the phase is finished.
    /// </summary>
    public string? CompletedMessageKey { get; set; }

    /// <summary>
    /// Gets or sets the standby duration after the phase, during which the HUD is hidden and
    /// the wide area attacks of Maya are paused.
    /// </summary>
    public TimeSpan StandbyDuration { get; set; }

    /// <summary>
    /// Gets or sets the definition of the transition, if this is a
    /// <see cref="KanturuPhaseKind.Transition"/> phase.
    /// </summary>
    public KanturuTransitionDefinition? Transition { get; set; }

    /// <summary>
    /// Gets or sets the definition of the boss fight, if this is a
    /// <see cref="KanturuPhaseKind.Nightmare"/> phase.
    /// </summary>
    public KanturuNightmareDefinition? Nightmare { get; set; }
}
