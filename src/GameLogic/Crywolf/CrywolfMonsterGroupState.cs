// <copyright file="CrywolfMonsterGroupState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

using System.Collections.Concurrent;
using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// The state of a group of the army of Balgass during an event.
/// </summary>
internal sealed class CrywolfMonsterGroupState
{
    private readonly ConcurrentDictionary<MonsterSpawnArea, Monster> _monsters = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CrywolfMonsterGroupState"/> class.
    /// </summary>
    /// <param name="definition">The definition of the group.</param>
    /// <param name="spawnAreas">The spawn areas of the members of the group.</param>
    public CrywolfMonsterGroupState(CrywolfMonsterGroup definition, IReadOnlyList<MonsterSpawnArea> spawnAreas)
    {
        this.Definition = definition;
        this.SpawnAreas = spawnAreas;
    }

    /// <summary>
    /// Gets the definition of the group.
    /// </summary>
    public CrywolfMonsterGroup Definition { get; }

    /// <summary>
    /// Gets the spawn areas of the members of the group.
    /// </summary>
    public IReadOnlyList<MonsterSpawnArea> SpawnAreas { get; }

    /// <summary>
    /// Gets the leader of the group.
    /// </summary>
    public Monster? Leader { get; private set; }

    /// <summary>
    /// Sets the current monster of a spawn area.
    /// </summary>
    /// <param name="spawnArea">The spawn area.</param>
    /// <param name="monster">The monster.</param>
    /// <param name="isLeader">If set to <c>true</c>, the monster is the leader of the group.</param>
    public void SetMonster(MonsterSpawnArea spawnArea, Monster monster, bool isLeader)
    {
        this._monsters[spawnArea] = monster;
        if (isLeader)
        {
            this.Leader = monster;
        }
    }

    /// <summary>
    /// Gets the spawn areas of the dead members, except the leader.
    /// </summary>
    /// <returns>The spawn areas of the dead members.</returns>
    public IEnumerable<MonsterSpawnArea> GetDeadMembers()
    {
        return this._monsters
            .Where(entry => !entry.Value.IsAlive && entry.Value != this.Leader)
            .Select(entry => entry.Key)
            .ToList();
    }
}
