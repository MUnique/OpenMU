// <copyright file="NonPlayerCharacter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using Nito.AsyncEx;

/// <summary>
/// The implementation of a non-player-character (Monster) which can not be attacked or attack.
/// </summary>
public class NonPlayerCharacter : AsyncDisposable, IObservable, IRotatable, ILocateable, IHasBucketInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NonPlayerCharacter"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The stats.</param>
    /// <param name="map">The map on which this instance will spawn.</param>
    public NonPlayerCharacter(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map)
    {
        this.SpawnArea = spawnInfo;
        this.Definition = stats;
        this.CurrentMap = map;
    }

    /// <inheritdoc/>
    public ushort Id { get; set; }

    /// <summary>
    /// Gets or sets the stats of this instance.
    /// </summary>
    public MonsterDefinition Definition { get; set; }

    /// <inheritdoc/>
    public virtual Point Position { get; set; }

    /// <inheritdoc/>
    public Direction Rotation { get; set; }

    /// <inheritdoc/>
    public ISet<IWorldObserver> Observers { get; } = new HashSet<IWorldObserver>();

    /// <summary>
    /// Gets the lock for <see cref="Observers"/>.
    /// </summary>
    public AsyncReaderWriterLock ObserverLock { get; } = new();

    /// <inheritdoc/>
    public GameMap CurrentMap { get; }

    /// <summary>
    /// Gets or sets the spawn area of this instance.
    /// </summary>
    public MonsterSpawnArea SpawnArea { get; protected set; }

    /// <summary>
    /// Gets or sets the index of the spawn within the <see cref="SpawnArea"/>.
    /// </summary>
    public int SpawnIndex { get; set; }

    /// <inheritdoc/>
    public Bucket<ILocateable>? NewBucket { get; set; }

    /// <inheritdoc/>
    public Bucket<ILocateable>? OldBucket { get; set; }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public virtual void Initialize()
    {
        const int maxRetryForAreas = 100;
        var maxRetry = this.SpawnArea.IsPoint() ? 1 : maxRetryForAreas;

        Point? spawnPoint = null;

        for (var retry = 0; retry < maxRetry && spawnPoint is null; retry++)
        {
            spawnPoint = this.GetNewSpawnPoint(this.SpawnArea);
            if (spawnPoint is not null)
            {
                break;
            }
        }

        if (spawnPoint == null)
        {
            throw new InvalidOperationException("No valid spawn point found. Spawn area might not contain valid points.");
        }

        this.Position = spawnPoint.Value;
        this.Rotation = GetSpawnDirection(this.SpawnArea.Direction);
    }

    /// <summary>
    /// Called when this instance spawned on the map.
    /// </summary>
    public virtual void OnSpawn()
    {
        // can be overwritten
    }

    /// <inheritdoc/>
    public async ValueTask AddObserverAsync(IWorldObserver observer)
    {
        using var writerLock = await this.ObserverLock.WriterLockAsync();
        this.Observers.Add(observer);
        if (this.Observers.Count == 1)
        {
            this.OnFirstObserverAdded();
        }

        this.OnObserverAdded();
    }

    /// <inheritdoc/>
    public async ValueTask RemoveObserverAsync(IWorldObserver observer)
    {
        using var writerLock = await this.ObserverLock.WriterLockAsync();
        this.Observers.Remove(observer);
        if (this.Observers.Count == 0)
        {
            this.OnLastObserverRemoved();
        }

        this.OnObserverRemoved();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Definition.Designation} - Id: {this.Id} - Position: {this.Position}";
    }

    /// <summary>
    /// Called when an observer has been added.
    /// </summary>
    protected virtual void OnObserverAdded()
    {
        // can be overwritten.
    }

    /// <summary>
    /// Called when an observer has been removed.
    /// </summary>
    protected virtual void OnObserverRemoved()
    {
        // can be overwritten.
    }

    /// <summary>
    /// Called when the first observer has been added.
    /// </summary>
    protected virtual void OnFirstObserverAdded()
    {
        // can be overwritten.
    }

    /// <summary>
    /// Called when the last observer has been removed.
    /// </summary>
    protected virtual void OnLastObserverRemoved()
    {
        // can be overwritten.
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        await this.CurrentMap.RemoveAsync(this).ConfigureAwait(false);
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    /// <summary>
    /// Moves the instance to the specified position.
    /// </summary>
    /// <param name="target">The new coordinates.</param>
    /// <param name="type">The type of moving.</param>
    protected virtual ValueTask MoveAsync(Point target, MoveType type)
    {
        throw new NotSupportedException("NPCs can't be moved");
    }

    /// <summary>
    /// Gets the spawn direction.
    /// </summary>
    /// <param name="configuredDirection">The configured direction.</param>
    /// <returns>The spawn direction.</returns>
    private static Direction GetSpawnDirection(Direction configuredDirection)
    {
        if (configuredDirection == Direction.Undefined)
        {
            return (Direction)Rand.NextInt(1, 9);
        }

        return configuredDirection;
    }

    private Point? GetNewSpawnPoint(MonsterSpawnArea spawnArea)
    {
        var x = Rand.NextInt(spawnArea.X1, spawnArea.X2 + 1);
        var y = Rand.NextInt(spawnArea.Y1, spawnArea.Y2 + 1);
        var point = new Point((byte)x, (byte)y);
        if (this.IsValidSpawnPoint(point))
        {
            return point;
        }

        return null;
    }

    private bool IsValidSpawnPoint(Point spawnPoint)
    {
        var isSafezoneAllowed = this.Definition.ObjectKind != NpcObjectKind.Monster && this.Definition.ObjectKind != NpcObjectKind.Trap;
        var isInSafezone = this.CurrentMap.Terrain.SafezoneMap[spawnPoint.X, spawnPoint.Y];
        var npcCanWalk = this.Definition.ObjectKind == NpcObjectKind.Monster || this.Definition.ObjectKind == NpcObjectKind.Guard;
        var isWalkable = this.CurrentMap.Terrain.WalkMap[spawnPoint.X, spawnPoint.Y];
        return (isSafezoneAllowed || !isInSafezone) && (!npcCanWalk || isWalkable);
    }
}