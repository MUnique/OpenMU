// <copyright file="GameMap.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using log4net;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// The game map which contains instances of players, npcs, drops, and more.
    /// </summary>
    public class GameMap
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameMap));

        private readonly IDictionary<ushort, ILocateable> objectsInMap = new Dictionary<ushort, ILocateable>();

        private readonly IAreaOfInterestManager areaOfInterestManager;

        private readonly ConcurrentBag<ushort> freeDropIds;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameMap"/> class.
        /// </summary>
        /// <param name="mapDefinition">The map definition.</param>
        /// <param name="itemDropDuration">Duration of the item drop.</param>
        /// <param name="chunkSize">Size of the chunk.</param>
        /// <param name="itemIdStart">The item identifier start.</param>
        public GameMap(GameMapDefinition mapDefinition, int itemDropDuration, byte chunkSize, ushort itemIdStart)
        {
            this.Definition = mapDefinition;
            this.ItemDropDuration = itemDropDuration;
            Log.DebugFormat("Creating GameMap {0}", this.Definition);
            this.Terrain = new GameMapTerrain(this.Definition);

            this.areaOfInterestManager = new BucketAreaOfInterestManager(chunkSize);
            this.freeDropIds = new ConcurrentBag<ushort>(Enumerable.Range(itemIdStart, (ushort)(short.MaxValue - itemIdStart)).Select(id => (ushort)id));
        }

        /// <summary>
        /// Gets the map identifier.
        /// </summary>
        public ushort MapId => this.Definition.Number.ToUnsigned();

        /// <summary>
        /// Gets the terrain of the map.
        /// </summary>
        public GameMapTerrain Terrain { get; }

        /// <summary>
        /// Gets the time in seconds of how long drops are laying on the ground until they are disappearing.
        /// </summary>
        public int ItemDropDuration { get; }

        /// <summary>
        /// Gets the definition of the map.
        /// </summary>
        public GameMapDefinition Definition { get; }

        /// <summary>
        /// Gets the object with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The object with the specified identifier.</returns>
        public ILocateable GetObject(ushort id)
        {
            this.objectsInMap.TryGetValue(id, out ILocateable result);
            return result;
        }

        /// <summary>
        /// Gets the attackables in range of the specified coordinate.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="range">The range.</param>
        /// <returns>The attackables in range of the specified coordinate.</returns>
        public IEnumerable<IAttackable> GetAttackablesInRange(int x, int y, int range)
        {
            return this.areaOfInterestManager.GetInRange(x, y, range, RangeType.Quadratic).OfType<IAttackable>().ToList();
        }

        /// <summary>
        /// Gets the drop by id.
        /// </summary>
        /// <param name="dropId">The drop identifier.</param>
        /// <returns>The dropped item.</returns>
        public DroppedItem GetDrop(ushort dropId)
        {
            this.objectsInMap.TryGetValue(dropId, out ILocateable item);
            return item as DroppedItem;
        }

        /// <summary>
        /// Removes the locateable from the map.
        /// </summary>
        /// <param name="locateable">The locateable.</param>
        public void Remove(ILocateable locateable)
        {
            if (this.objectsInMap.Remove(locateable.Id) && locateable.Id != 0 && locateable is DroppedItem)
            {
                this.freeDropIds.Add(locateable.Id);
            }

            this.areaOfInterestManager.RemoveObject(locateable);
        }

        /// <summary>
        /// Adds the locateable to the map.
        /// </summary>
        /// <param name="locateable">The player.</param>
        public void Add(ILocateable locateable)
        {
            if (locateable is DroppedItem droppedItem)
            {
                if (this.freeDropIds.TryTake(out ushort dropId))
                {
                    droppedItem.Id = dropId;
                    Log.DebugFormat("{0}: AddDrop {1}", this.Definition, droppedItem.Id);
                }
                else
                {
                    Log.Warn("No free drop id available");
                    return;
                }
            }

            this.objectsInMap.Add(locateable.Id, locateable);
            this.areaOfInterestManager.AddObject(locateable);
        }

        /// <summary>
        /// Moves the locateable on the map.
        /// </summary>
        /// <param name="locateable">The monster.</param>
        /// <param name="newX">The new x coordinate.</param>
        /// <param name="newY">The new y coordinate.</param>
        /// <param name="moveLock">The move lock.</param>
        /// <param name="moveType">Type of the move.</param>
        public void Move(ILocateable locateable, byte newX, byte newY, object moveLock, MoveType moveType)
        {
            this.areaOfInterestManager.MoveObject(locateable, newX, newY, moveLock, moveType);
        }
    }
}
