// <copyright file="CastleSiegeNpcBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.NPC;

using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// Base class for non-attackable Castle Siege NPCs which own an intelligence instance.
/// </summary>
public abstract class CastleSiegeNpcBase : NonPlayerCharacter, ICastleSiegeNpc
{
    private readonly INpcIntelligence _intelligence;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeNpcBase"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The NPC definition.</param>
    /// <param name="map">The map on which the NPC is spawned.</param>
    /// <param name="runtime">The Castle Siege runtime entry.</param>
    /// <param name="intelligence">The NPC intelligence.</param>
    protected CastleSiegeNpcBase(
        MonsterSpawnArea spawnInfo,
        MonsterDefinition stats,
        GameMap map,
        CastleSiegeNpcRuntime runtime,
        INpcIntelligence intelligence)
        : base(spawnInfo, stats, map)
    {
        this.Runtime = runtime;
        this._intelligence = intelligence;
        this._intelligence.Npc = this;
    }

    /// <inheritdoc />
    public CastleSiegeNpcRuntime Runtime { get; }

    /// <inheritdoc />
    public override void OnSpawn()
    {
        base.OnSpawn();
        this._intelligence.Start();
    }

    /// <inheritdoc />
    protected override void Dispose(bool managed)
    {
        if (managed)
        {
            this._intelligence.Pause();
            (this._intelligence as IDisposable)?.Dispose();
        }

        base.Dispose(managed);
    }
}
