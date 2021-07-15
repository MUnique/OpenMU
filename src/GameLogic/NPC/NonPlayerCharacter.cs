// <copyright file="NonPlayerCharacter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// The implementation of a non-player-character (Monster) which can not be attacked or attack.
    /// </summary>
    public class NonPlayerCharacter : IObservable, IRotatable, ILocateable, IHasBucketInformation, IDisposable
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
        public ReaderWriterLockSlim ObserverLock { get; } = new ();

        /// <inheritdoc/>
        public GameMap CurrentMap { get; }

        /// <summary>
        /// Gets or sets the spawn area of this instance.
        /// </summary>
        public MonsterSpawnArea SpawnArea { get; protected set; }

        /// <inheritdoc/>
        public Bucket<ILocateable>? NewBucket { get; set; }

        /// <inheritdoc/>
        public Bucket<ILocateable>? OldBucket { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize()
        {
            const int maxRetry = 20;
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

        /// <inheritdoc/>
        public void AddObserver(IWorldObserver observer)
        {
            this.ObserverLock.EnterWriteLock();
            try
            {
                this.Observers.Add(observer);
            }
            finally
            {
                this.ObserverLock.ExitWriteLock();
            }
        }

        /// <inheritdoc/>
        public void RemoveObserver(IWorldObserver observer)
        {
            this.ObserverLock.EnterWriteLock();
            try
            {
                this.Observers.Remove(observer);
            }
            finally
            {
                this.ObserverLock.ExitWriteLock();
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Definition.Designation} - Id: {this.Id} - Position: {this.Position}";
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="managed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "<ObserverLock>k__BackingField", Justification = "Can't access backing field.")]
        protected virtual void Dispose(bool managed)
        {
            this.ObserverLock.Dispose();
        }

        /// <summary>
        /// Moves the instance to the specified position.
        /// </summary>
        /// <param name="target">The new coordinates.</param>
        /// <param name="type">The type of moving.</param>
        protected virtual void Move(Point target, MoveType type)
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
}
