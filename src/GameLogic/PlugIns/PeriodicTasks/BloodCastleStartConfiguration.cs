﻿// <copyright file="BloodCastleStartConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// The blood castle start configuration.
/// </summary>
public class BloodCastleStartConfiguration : MiniGameStartConfiguration
{
    /// <summary>
    /// Gets the default configuration for blood castle.
    /// </summary>
    public static BloodCastleStartConfiguration Default =>
        new()
        {
            PreStartMessageDelay = TimeSpan.Zero,
            EntranceOpenedMessage = "La entrada de Blood Castle está abierta y cierra en {0} minuto(s).",
            EntranceClosedMessage = "La entrada de Blood Castle se cerró.",
            TaskDuration = TimeSpan.FromMinutes(20),
            Timetable = GenerateTimeSequence(TimeSpan.FromMinutes(120)).ToList(),
        };
}