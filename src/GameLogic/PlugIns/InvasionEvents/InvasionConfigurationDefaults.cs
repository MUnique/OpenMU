// <copyright file="InvasionConfigurationDefaults.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Provides ready-to-use default configurations for the built-in invasion types.
/// </summary>
internal static class InvasionConfigurationDefaults
{
    /// <summary>
    /// Gets the default configuration for the Golden Invasion event.
    /// </summary>
    public static PeriodicInvasionConfiguration Golden => new()
    {
        TaskDuration = TimeSpan.FromMinutes(30),
        PreStartMessageDelay = TimeSpan.FromSeconds(3),
        StartMessage = "[{mapName}] Golden invasion!",
        EndMessage = "[{mapName}] Golden invasion has ended.",
        Timetable = PeriodicTaskConfiguration.GenerateTimeSequence(TimeSpan.FromHours(4)).ToList(),
        Mobs =
        [
            new(InvasionMonsters.GoldenBudgeDragon, 20, [InvasionMaps.Lorencia], SpawnMapStrategy.RandomMap),
            new(InvasionMonsters.GoldenGoblin, 20, [InvasionMaps.Noria], SpawnMapStrategy.RandomMap),
            new(InvasionMonsters.GoldenSoldier, 20, [InvasionMaps.Devias], SpawnMapStrategy.RandomMap),
            new(InvasionMonsters.GoldenTitan, 10, [InvasionMaps.Devias], SpawnMapStrategy.RandomMap),
            new(InvasionMonsters.GoldenVepar, 20, [InvasionMaps.Atlans], SpawnMapStrategy.RandomMap),
            new(InvasionMonsters.GoldenLizardKing, 10, [InvasionMaps.Atlans], SpawnMapStrategy.RandomMap),
            new(InvasionMonsters.GoldenWheel, 20, [InvasionMaps.Tarkan], SpawnMapStrategy.RandomMap),
            new(InvasionMonsters.GoldenTantallos, 10, [InvasionMaps.Tarkan], SpawnMapStrategy.RandomMap),
            new(InvasionMonsters.GoldenDragon, 10, [InvasionMaps.Lorencia, InvasionMaps.Noria, InvasionMaps.Devias, InvasionMaps.Atlans, InvasionMaps.Tarkan], SpawnMapStrategy.RandomMap),
        ],
    };

    /// <summary>
    /// Gets the default configuration for the Red Dragon Invasion event.
    /// </summary>
    public static PeriodicInvasionConfiguration RedDragon => new()
    {
        TaskDuration = TimeSpan.FromMinutes(30),
        PreStartMessageDelay = TimeSpan.FromSeconds(3),
        StartMessage = "[{mapName}] Red Dragon invasion!",
        EndMessage = "[{mapName}] Red Dragon invasion has ended.",
        Timetable = PeriodicTaskConfiguration.GenerateTimeSequence(TimeSpan.FromHours(6), new TimeOnly(2, 0)).ToList(),
        Mobs =
        [
            new(InvasionMonsters.RedDragon, 5, [InvasionMaps.Lorencia, InvasionMaps.Noria, InvasionMaps.Devias], SpawnMapStrategy.RandomMap),
        ],
    };
}