// <copyright file="PlayerSummon.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The summoned monster of a <see cref="Player"/>, which fights for it until it dies.
/// </summary>
internal sealed class PlayerSummon
{
    private readonly Player _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerSummon"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public PlayerSummon(Player player)
    {
        this._player = player;
    }

    /// <summary>
    /// Gets the summoned monster and its intelligence, if a summon exists.
    /// </summary>
    public (Monster Monster, INpcIntelligence Intelligence)? Current { get; private set; }

    /// <summary>
    /// Gets the summoned monster, if it exists and is alive.
    /// </summary>
    public Monster? AliveMonster => this.Current?.Monster is { IsAlive: true } monster ? monster : null;

    /// <summary>
    /// Creates a summoned monster for the player.
    /// </summary>
    /// <param name="definition">The definition.</param>
    /// <exception cref="InvalidOperationException">Can't add the player summon for a player which isn't spawned yet.</exception>
    public async ValueTask CreateAsync(MonsterDefinition definition)
    {
        if (this._player.CurrentMap is not { } gameMap)
        {
            throw new InvalidOperationException("Can't add a summon for a player which isn't spawned yet.");
        }

        var area = new MonsterSpawnArea
        {
            GameMap = gameMap.Definition,
            MonsterDefinition = definition,
            SpawnTrigger = SpawnTrigger.OnceAtEventStart,
            Quantity = 1,
            X1 = (byte)Math.Max(this._player.Position.X - 3, byte.MinValue),
            X2 = (byte)Math.Min(this._player.Position.X + 3, byte.MaxValue),
            Y1 = (byte)Math.Max(this._player.Position.Y - 3, byte.MinValue),
            Y2 = (byte)Math.Min(this._player.Position.Y + 3, byte.MaxValue),
        };
        var intelligence = new SummonedMonsterIntelligence(this._player);
        var monster = new Monster(area, definition, gameMap, NullDropGenerator.Instance, intelligence, this._player.GameContext.PlugInManager, this._player.GameContext.PathFinderPool);
        area.MaximumHealthOverride = (int)monster.Attributes[Stats.MaximumHealth];
        area.MaximumHealthOverride += (int)(monster.Attributes[Stats.MaximumHealth] * this._player.Attributes?[Stats.SummonedMonsterHealthIncrease] ?? 0);

        this.Current = (monster, intelligence);
        monster.Initialize();
        await gameMap.AddAsync(monster).ConfigureAwait(false);
        monster.OnSpawn();
    }

    /// <summary>
    /// Notifies this instance that the summoned monster died.
    /// </summary>
    public void OnDied()
    {
        this.Current = null;
    }

    /// <summary>
    /// Removes the summon from its map and disposes it.
    /// </summary>
    public async ValueTask RemoveAsync()
    {
        if (this.Current is { } summon)
        {
            // remove summon, if exists
            await summon.Monster.CurrentMap.RemoveAsync(summon.Monster).ConfigureAwait(false);
            summon.Monster.Dispose();
            this.OnDied();
        }
    }

    /// <summary>
    /// Removes the summon from the specified map, but keeps it, so that it can be added again
    /// after the player entered its new map.
    /// </summary>
    /// <param name="map">The map from which the summon is removed.</param>
    public async ValueTask RemoveFromMapAsync(GameMap map)
    {
        if (this.AliveMonster is { } summon)
        {
            await map.RemoveAsync(summon).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Adds the summon to the specified map, after the player entered it.
    /// </summary>
    /// <param name="map">The map to which the summon is added.</param>
    public async ValueTask AddToMapAsync(GameMap map)
    {
        if (this.AliveMonster is { } summon)
        {
            await map.AddAsync(summon).ConfigureAwait(false);
            summon.OnSpawn();
        }
    }

    /// <summary>
    /// Moves the summon to the specified gate, when the player is placed there.
    /// </summary>
    /// <param name="gate">The gate.</param>
    public void PlaceAtGate(ExitGate gate)
    {
        if (this.AliveMonster is { } summon)
        {
            summon.Position = gate.GetRandomPoint();
            summon.Rotation = gate.Direction;
        }
    }

    /// <summary>
    /// Registers a hit of the specified attacker at the summon's intelligence, so that it
    /// can defend its owner.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    public void RegisterHit(IAttacker attacker)
    {
        this.Current?.Intelligence.RegisterHit(attacker);
    }
}
