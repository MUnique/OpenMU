// <copyright file="ChaosCastleStartConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// The chaos castle start configuration.
/// </summary>
public class ChaosCastleStartConfiguration : MiniGameStartConfiguration
{
    /// <summary>
    /// Gets the default configuration for chaos castle.
    /// </summary>
    public static ChaosCastleStartConfiguration Default =>
        new()
        {
            PreStartMessageDelay = TimeSpan.Zero,
            EntranceOpenedMessage = "La entrada de Chaos Castle está abierta y cierra en {0} minuto(s).",
            EntranceClosedMessage = "La entrada de Chaos Castle se cerró.",
            TaskDuration = TimeSpan.FromMinutes(15),
            Timetable = PeriodicTaskConfiguration.GenerateTimeSequence(TimeSpan.FromMinutes(60)).ToList(),
        };
}