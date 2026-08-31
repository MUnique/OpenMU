// <copyright file="KanturuEventDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using MUnique.OpenMU.GameLogic.Properties;

/// <summary>
/// The definition of the Kanturu Refinery Tower event. It describes the whole run of the
/// event, so that it can be adapted without code changes.
/// </summary>
/// <remarks>
/// It's configured at the Kanturu start plug-in, see
/// <c>MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks.KanturuStartConfiguration</c>.
/// The values of <see cref="Default"/> describe the original season 6 event.
/// </remarks>
public class KanturuEventDefinition
{
    private const short MayaLeftHandNumber = 362;

    private const short MayaRightHandNumber = 363;

    private const short NightmareNumber = 361;

    private const short BladeHunterNumber = 354;

    private const short DreadfearNumber = 360;

    private const short TwinTaleNumber = 359;

    private const short GenociderNumber = 357;

    private const short PersonaNumber = 358;

    /// <summary>
    /// Gets the definition of the original season 6 event.
    /// </summary>
    public static KanturuEventDefinition Default => new()
    {
        IntroSpawnWaveNumber = 0,
        IntroMessageKey = nameof(PlayerMessage.KanturuMayaRises),
        IntroState = KanturuState.MayaBattle,
        IntroDetailState = (byte)KanturuMayaDetailState.Notify,
        IntroDuration = TimeSpan.FromSeconds(3),
        MayaAttackInterval = TimeSpan.FromSeconds(15),
        BarrierOpeningMessageKey = nameof(PlayerMessage.KanturuBarrierOpening),
        VictoryMessageKey = nameof(PlayerMessage.KanturuVictory),
        DefeatMessageKey = nameof(PlayerMessage.KanturuDefeat),
        TowerOfRefinementDuration = TimeSpan.FromHours(1),
        TowerClosingWarningOffset = TimeSpan.FromMinutes(5),
        TowerConqueredMessageKey = nameof(PlayerMessage.KanturuTowerConquered),
        TowerClosingWarningMessageKey = nameof(PlayerMessage.KanturuTowerClosingWarning),
        TowerClosedMessageKey = nameof(PlayerMessage.KanturuTowerClosed),

        // The whole X=73-90, Y=144-195 column is NoGround in Terrain39.att and blocks the path
        // from the Nightmare zone to the Elpis NPC area.
        BarrierAreas = [new KanturuTerrainArea { StartX = 73, StartY = 144, EndX = 90, EndY = 195 }],
        Phases =
        [
            new KanturuPhaseDefinition
            {
                Name = "Phase 1 - Monsters",
                Kind = KanturuPhaseKind.MonsterWave,
                State = KanturuState.MayaBattle,
                DetailState = (byte)KanturuMayaDetailState.Monster1,
                TimeLimit = TimeSpan.FromMinutes(10),
                SpawnWaveNumber = 1,
                KillTarget = 40,
                CountedMonsterNumbers = [BladeHunterNumber, DreadfearNumber],
                StartMessageKey = nameof(PlayerMessage.KanturuPhase1Start),
            },
            new KanturuPhaseDefinition
            {
                Name = "Phase 1 - Maya's left hand",
                Kind = KanturuPhaseKind.MonsterWave,
                State = KanturuState.MayaBattle,
                DetailState = (byte)KanturuMayaDetailState.Maya1,
                SpawnWaveNumber = 2,
                KillTarget = 1,
                CountedMonsterNumbers = [MayaLeftHandNumber],
                StartMessageKey = nameof(PlayerMessage.KanturuMayaLeftHandAppeared),
                CompletedMessageKey = nameof(PlayerMessage.KanturuPhase1Cleared),
                StandbyDuration = TimeSpan.FromMinutes(2),
            },
            new KanturuPhaseDefinition
            {
                Name = "Phase 2 - Monsters",
                Kind = KanturuPhaseKind.MonsterWave,
                State = KanturuState.MayaBattle,
                DetailState = (byte)KanturuMayaDetailState.Monster2,
                TimeLimit = TimeSpan.FromMinutes(10),
                SpawnWaveNumber = 3,
                KillTarget = 40,
                CountedMonsterNumbers = [BladeHunterNumber, DreadfearNumber],
                StartMessageKey = nameof(PlayerMessage.KanturuPhase2Start),
            },
            new KanturuPhaseDefinition
            {
                Name = "Phase 2 - Maya's right hand",
                Kind = KanturuPhaseKind.MonsterWave,
                State = KanturuState.MayaBattle,
                DetailState = (byte)KanturuMayaDetailState.Maya2,
                SpawnWaveNumber = 4,
                KillTarget = 1,
                CountedMonsterNumbers = [MayaRightHandNumber],
                StartMessageKey = nameof(PlayerMessage.KanturuMayaRightHandAppeared),
                CompletedMessageKey = nameof(PlayerMessage.KanturuPhase2Cleared),
                StandbyDuration = TimeSpan.FromMinutes(2),
            },
            new KanturuPhaseDefinition
            {
                Name = "Phase 3 - Monsters",
                Kind = KanturuPhaseKind.MonsterWave,
                State = KanturuState.MayaBattle,
                DetailState = (byte)KanturuMayaDetailState.Monster3,
                TimeLimit = TimeSpan.FromMinutes(10),
                SpawnWaveNumber = 5,
                KillTarget = 20,
                CountedMonsterNumbers = [DreadfearNumber, TwinTaleNumber],
                StartMessageKey = nameof(PlayerMessage.KanturuPhase3Start),
            },
            new KanturuPhaseDefinition
            {
                Name = "Phase 3 - Both hands of Maya",
                Kind = KanturuPhaseKind.MonsterWave,
                State = KanturuState.MayaBattle,
                DetailState = (byte)KanturuMayaDetailState.Maya3,
                SpawnWaveNumber = 6,
                KillTarget = 2,
                CountedMonsterNumbers = [MayaLeftHandNumber, MayaRightHandNumber],
                StartMessageKey = nameof(PlayerMessage.KanturuBothMayaHandsAppeared),

                // The standby is the loot window for the drops of both hands.
                CompletedMessageKey = nameof(PlayerMessage.KanturuMayaHandsFallen),
                StandbyDuration = TimeSpan.FromSeconds(10),
            },
            new KanturuPhaseDefinition
            {
                Name = "Transition to the Nightmare zone",
                Kind = KanturuPhaseKind.Transition,
                State = KanturuState.MayaBattle,
                DetailState = (byte)KanturuMayaDetailState.EndCycleMaya3,
                Transition = new KanturuTransitionDefinition
                {
                    CinematicDuration = TimeSpan.FromSeconds(10),
                    EntryPointX = 79,
                    EntryPointY = 98,
                    WarpAnimationDelay = TimeSpan.FromMilliseconds(200),
                },
            },
            new KanturuPhaseDefinition
            {
                Name = "Nightmare - Guardians",
                Kind = KanturuPhaseKind.MonsterWave,
                State = KanturuState.NightmareBattle,
                DetailState = (byte)KanturuNightmareDetailState.Idle,
                TimeLimit = TimeSpan.FromMinutes(30),
                SpawnWaveNumber = 7,
                KillTarget = 45,
                CountedMonsterNumbers = [GenociderNumber, DreadfearNumber, PersonaNumber],
                StartMessageKey = nameof(PlayerMessage.KanturuNightmareGuardiansAppeared),

                // The guardians don't have to be killed; they fight alongside the boss.
                Duration = TimeSpan.FromSeconds(3),
            },
            new KanturuPhaseDefinition
            {
                Name = "Nightmare",
                Kind = KanturuPhaseKind.Nightmare,
                State = KanturuState.NightmareBattle,
                DetailState = (byte)KanturuNightmareDetailState.NightmareIntro,
                StartDelay = TimeSpan.FromSeconds(3),
                SpawnWaveNumber = 8,
                KillTarget = 1,
                CountedMonsterNumbers = [NightmareNumber],
                StartMessageKey = nameof(PlayerMessage.KanturuNightmareAppeared),
                Nightmare = new KanturuNightmareDefinition
                {
                    MonsterNumber = NightmareNumber,
                    BattleDetailState = (byte)KanturuNightmareDetailState.Battle,

                    // The boss spawns at (78, 143) and moves within the zone of X:75-88, Y:97-143.
                    HpPhases =
                    [
                        new KanturuNightmareHpPhase
                        {
                            HealthPercentage = 75,
                            TeleportTargetX = 82,
                            TeleportTargetY = 130,
                            MessageKey = nameof(PlayerMessage.KanturuNightmareTeleport2),
                        },
                        new KanturuNightmareHpPhase
                        {
                            HealthPercentage = 50,
                            TeleportTargetX = 76,
                            TeleportTargetY = 115,
                            MessageKey = nameof(PlayerMessage.KanturuNightmareTeleport3),
                        },
                        new KanturuNightmareHpPhase
                        {
                            HealthPercentage = 25,
                            TeleportTargetX = 85,
                            TeleportTargetY = 100,
                            MessageKey = nameof(PlayerMessage.KanturuNightmareTeleport4),
                        },
                    ],
                },
            },
        ],
    };

    /// <summary>
    /// Gets or sets the number of the spawn wave which is started when the battle begins, so
    /// that Maya rises from the depths.
    /// </summary>
    public byte? IntroSpawnWaveNumber { get; set; }

    /// <summary>
    /// Gets or sets the key of the localized message which is shown when the battle begins.
    /// </summary>
    public string? IntroMessageKey { get; set; }

    /// <summary>
    /// Gets or sets the main state which is sent to the clients for the intro cinematic.
    /// </summary>
    public KanturuState IntroState { get; set; } = KanturuState.MayaBattle;

    /// <summary>
    /// Gets or sets the detail state which is sent to the clients for the intro cinematic.
    /// </summary>
    public byte IntroDetailState { get; set; } = (byte)KanturuMayaDetailState.Notify;

    /// <summary>
    /// Gets or sets how long the context waits for the intro cinematic before it starts the
    /// first phase.
    /// </summary>
    public TimeSpan IntroDuration { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Gets or sets the interval in which the wide area attack of Maya is broadcasted during
    /// the Maya phases. Set it to <see cref="TimeSpan.Zero"/> to disable it.
    /// </summary>
    /// <remarks>
    /// It's a purely visual effect; the damage is done by the attack skills of the monsters.
    /// The broadcast alternates between the storm and the stone rain animation, and it's
    /// paused during the standby time between the phases.
    /// </remarks>
    public TimeSpan MayaAttackInterval { get; set; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Gets or sets the phases of the event, in the order in which they are executed.
    /// </summary>
    public IList<KanturuPhaseDefinition> Phases { get; set; } = new List<KanturuPhaseDefinition>();

    /// <summary>
    /// Gets or sets the key of the localized message which is shown when the barrier to the
    /// Elphis area is opened.
    /// </summary>
    public string? BarrierOpeningMessageKey { get; set; }

    /// <summary>
    /// Gets or sets how long the victory cinematic takes, before the tower is opened.
    /// </summary>
    public TimeSpan VictoryCinematicDuration { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Gets or sets the areas whose <see cref="MUnique.OpenMU.DataModel.Configuration.TerrainAttributeType.NoGround"/>
    /// attribute is removed when the Nightmare boss has been defeated, so that the players can
    /// walk to the Elphis area.
    /// </summary>
    public IList<KanturuTerrainArea> BarrierAreas { get; set; } = new List<KanturuTerrainArea>();

    /// <summary>
    /// Gets or sets how long the Tower of Refinement stays open after the Nightmare boss has
    /// been defeated. Set it to <see cref="TimeSpan.Zero"/> to skip the tower phase.
    /// </summary>
    public TimeSpan TowerOfRefinementDuration { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets or sets how long before the end of the <see cref="TowerOfRefinementDuration"/> the
    /// players are warned about the closing of the tower.
    /// </summary>
    public TimeSpan TowerClosingWarningOffset { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the key of the localized message which is shown when the tower opens.
    /// </summary>
    public string? TowerConqueredMessageKey { get; set; }

    /// <summary>
    /// Gets or sets the key of the localized message which warns about the closing tower.
    /// </summary>
    public string? TowerClosingWarningMessageKey { get; set; }

    /// <summary>
    /// Gets or sets the key of the localized message which is shown when the tower closed.
    /// </summary>
    public string? TowerClosedMessageKey { get; set; }

    /// <summary>
    /// Gets or sets the key of the localized message which is shown when the event is won.
    /// </summary>
    public string? VictoryMessageKey { get; set; }

    /// <summary>
    /// Gets or sets the key of the localized message which is shown when the event is lost.
    /// </summary>
    public string? DefeatMessageKey { get; set; }
}
