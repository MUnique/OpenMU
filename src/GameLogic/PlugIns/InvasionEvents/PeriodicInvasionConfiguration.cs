// <copyright file="PeriodicInvasionConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.ComponentModel.DataAnnotations;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Abstract configuration for periodic invasions.
/// </summary>
public class PeriodicInvasionConfiguration : PeriodicTaskConfiguration
{
    private const ushort LorenciaId = 0;
    private const ushort DeviasId = 2;
    private const ushort NoriaId = 3;
    private const ushort AtlansId = 7;
    private const ushort TarkanId = 8;

    private const ushort GoldenBudgeDragonId = 43;
    private const ushort GoldenGoblinId = 78;
    private const ushort GoldenSoldierId = 54;
    private const ushort GoldenTitanId = 53;
    private const ushort GoldenDragonId = 79;
    private const ushort GoldenVeparId = 81;
    private const ushort GoldenLizardKingId = 80;
    private const ushort GoldenWheelId = 83;
    private const ushort GoldenTantallosId = 82;

    private const ushort RedDragonId = 44;

    /// <summary>
    /// Initializes a new instance of the <see cref="PeriodicInvasionConfiguration"/> class.
    /// </summary>
    public PeriodicInvasionConfiguration()
    {
        this.Message = "Invasion's been started!";
    }

    /// <summary>
    /// Gets the default configuration for Golden Invasion.
    /// </summary>
    public static PeriodicInvasionConfiguration DefaultGoldenInvasion => new()
    {
        TaskDuration = TimeSpan.FromMinutes(5),
        PreStartMessageDelay = TimeSpan.FromSeconds(3),
        Message = "[{mapName}] Golden Invasion!",
        Timetable = GenerateTimeSequence(TimeSpan.FromHours(4)).ToList(),
        Mobs = new List<InvasionSpawnConfiguration>
        {
            new(GoldenBudgeDragonId, 20, new List<ushort> { LorenciaId }, false),
            new(GoldenGoblinId, 20, new List<ushort> { NoriaId }, false),
            new(GoldenSoldierId, 20, new List<ushort> { DeviasId }, false),
            new(GoldenTitanId, 10, new List<ushort> { DeviasId }, false),
            new(GoldenVeparId, 20, new List<ushort> { AtlansId }, false),
            new(GoldenLizardKingId, 10, new List<ushort> { AtlansId }, false),
            new(GoldenWheelId, 20, new List<ushort> { TarkanId }, false),
            new(GoldenTantallosId, 10, new List<ushort> { TarkanId }, false),
            new(GoldenDragonId, 10, new List<ushort> { LorenciaId, NoriaId, DeviasId, AtlansId, TarkanId }, false),
        },
    };

    /// <summary>
    /// Gets the default configuration for the Red Dragon Invasion.
    /// </summary>
    public static PeriodicInvasionConfiguration DefaultRedDragonInvasion => new()
    {
        TaskDuration = TimeSpan.FromMinutes(10),
        PreStartMessageDelay = TimeSpan.FromSeconds(3),
        Message = "[{mapName}] Red Dragon Invasion!",
        Timetable = GenerateTimeSequence(TimeSpan.FromHours(6), new TimeOnly(2, 0)).ToList(),
        Mobs = new List<InvasionSpawnConfiguration>
        {
            new(RedDragonId, 5, new List<ushort> { LorenciaId, NoriaId, DeviasId }, false),
        },
    };

    /// <summary>
    /// Gets or sets the monster spawns.
    /// </summary>
    [Display(Name = "Monster Spawns", Order = 5)]
    public IList<InvasionSpawnConfiguration> Mobs { get; set; } = new List<InvasionSpawnConfiguration>();
}
