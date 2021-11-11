﻿// <copyright file="IEventStateProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Provides a flag, if the event is running.
/// </summary>
/// <remarks>
/// It has effect on re-spawning monsters, when the <see cref="MonsterSpawnArea.SpawnTrigger"/> is <see cref="SpawnTrigger.AutomaticDuringEvent"/>.
/// </remarks>
public interface IEventStateProvider
{
    /// <summary>
    /// Gets a value indicating whether the event is currently running.
    /// </summary>
    bool IsEventRunning { get; }

    /// <summary>
    /// Determines, if a spawn wave is currently active.
    /// </summary>
    /// <param name="waveNumber">The number of the wave.</param>
    /// <returns><see langword="true"/>, when the spawn wave is active; Otherwise, <see langword="false"/>.</returns>
    bool IsSpawnWaveActive(byte waveNumber);
}