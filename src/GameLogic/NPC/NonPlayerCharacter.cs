// <copyright file="NonPlayerCharacter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// The implementation of a non-player-character (Monster) which can not be attacked or attack.
    /// </summary>
    public class NonPlayerCharacter : IObservable, IRotateable, ILocateable, IHasBucketInformation, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonPlayerCharacter"/> class.
        /// </summary>
        /// <param name="spawnInfo">The spawn information.</param>
        /// <param name="stats">The stats.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="map">The map on which this instance will spawn.</param>
        public NonPlayerCharacter(MonsterSpawnArea spawnInfo, MonsterDefinition stats, ushort id, GameMap map)
        {
            this.Id = id;
            this.SpawnArea = spawnInfo;
            this.Definition = stats;
            this.CurrentMap = map;
        }

        /// <inheritdoc/>
        public ushort Id { get; }

        /// <summary>
        /// Gets or sets the stats of this instance.
        /// </summary>
        public MonsterDefinition Definition { get; set; }

        /// <inheritdoc/>
        public byte X { get; set; }

        /// <inheritdoc/>
        public byte Y { get; set; }

        /// <inheritdoc/>
        public Direction Rotation { get; set; }

        /// <inheritdoc/>
        public ISet<IWorldObserver> Observers { get; } = new HashSet<IWorldObserver>();

        /// <summary>
        /// Gets the lock for <see cref="Observers"/>.
        /// </summary>
        public ReaderWriterLockSlim ObserverLock { get; } = new ReaderWriterLockSlim();

        /// <inheritdoc/>
        public GameMap CurrentMap { get; }

        /// <summary>
        /// Gets or sets the spawn area of this instance.
        /// </summary>
        public MonsterSpawnArea SpawnArea { get; protected set; }

        /// <inheritdoc/>
        public Bucket<ILocateable> CurrentBucket { get; set; }

        /// <summary>
        /// Respawns this instance.
        /// </summary>
        public virtual void Respawn()
        {
            var spawnPoint = this.GetNewSpawnPoint(this.SpawnArea);
            var newx = spawnPoint.X;
            var newy = spawnPoint.Y;
            if (this.SpawnArea.Quantity > 1)
            {
                while (!(!this.CurrentMap.Terrain.SafezoneMap[newx, newy] && this.CurrentMap.Terrain.WalkMap[newx, newy]))
                {
                    spawnPoint = this.GetNewSpawnPoint(this.SpawnArea);
                    newx = spawnPoint.X;
                    newy = spawnPoint.Y;
                }
            }

            this.X = newx;
            this.Y = newy;
            this.Rotation = GetSpawnDirection(this.SpawnArea.Direction);

            this.CurrentMap.Add(this);
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
        public override bool Equals(object obj)
        {
            var otherObj = obj as NonPlayerCharacter;
            return this.Id == otherObj?.Id;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.Id;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Definition.Designation} - Id: {this.Id} - Position: {this.X}/{this.Y}";
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
        /// <param name="dispose"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "<ObserverLock>k__BackingField", Justification = "Can't access backing field.")]
        protected virtual void Dispose(bool dispose)
        {
            this.ObserverLock.Dispose();
        }

        /// <summary>
        /// Moves the instance to the specified position.
        /// </summary>
        /// <param name="newx">The new x coordinate.</param>
        /// <param name="newy">The new y coordinate.</param>
        /// <param name="type">The type of moving.</param>
        protected virtual void Move(byte newx, byte newy, MoveType type)
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

        private Point GetNewSpawnPoint(MonsterSpawnArea spawnArea)
        {
            var x = Rand.NextInt(spawnArea.X1, spawnArea.X2 + 1);
            var y = Rand.NextInt(spawnArea.Y1, spawnArea.Y2 + 1);
            return new Point((byte)x, (byte)y);
        }
    }
}
