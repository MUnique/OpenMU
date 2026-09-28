// <copyright file="DoppelgangerMonsterSpawn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Monsters which spawn once at the start of the path of the doppelganger event at a specific game time.
/// These monsters always attack players in their range.
/// </summary>
public class DoppelgangerMonsterSpawn
{
    /// <summary>
    /// Gets or sets the elapsed game time at which the monsters spawn.
    /// </summary>
    public TimeSpan SpawnTime { get; set; }

    /// <summary>
    /// Gets or sets the monsters which spawn.
    /// </summary>
    public IList<MonsterDefinition> Monsters { get; set; } = new List<MonsterDefinition>();
}
