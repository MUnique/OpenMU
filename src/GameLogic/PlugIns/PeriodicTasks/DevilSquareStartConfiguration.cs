// <copyright file="DevilSquareStartConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// The devil square start configuration.
/// </summary>
public class DevilSquareStartConfiguration : MiniGameStartConfiguration
{
    /// <summary>
    /// Gets the default configuration for devil square.
    /// </summary>
    public static DevilSquareStartConfiguration Default =>
        new()
        {
            PreStartMessageDelay = TimeSpan.Zero,
            EntranceOpenedMessage = "La entrada de Devil Square está abierta y cierra en {0} minuto(s).",
            EntranceClosedMessage = "La entrada de Devil Square se cerró.",
            TaskDuration = TimeSpan.FromMinutes(25),
            Timetable = PeriodicTaskConfiguration.GenerateTimeSequence(TimeSpan.FromMinutes(240)).ToList(),
        };
}