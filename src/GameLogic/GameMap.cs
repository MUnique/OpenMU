﻿// <copyright file="GameMap.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The game map which contains instances of players, npcs, drops, and more.
/// </summary>
public class GameMap
{
    private readonly IDictionary<ushort, ILocateable> _objectsInMap = new ConcurrentDictionary<ushort, ILocateable>();

    private readonly IAreaOfInterestManager _areaOfInterestManager;

    private readonly IdGenerator _objectIdGenerator;

    private readonly IdGenerator _dropIdGenerator;

    private readonly ExitGate? _safezoneSpawnGate;

    private int _playerCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameMap" /> class.
    /// </summary>
    /// <param name="mapDefinition">The map definition.</param>
    /// <param name="itemDropDuration">Duration of the item drop.</param>
    /// <param name="chunkSize">Size of the chunk.</param>
    public GameMap(GameMapDefinition mapDefinition, int itemDropDuration, byte chunkSize)
    {
        this.Definition = mapDefinition;
        this.ItemDropDuration = itemDropDuration;
        this.Terrain = new GameMapTerrain(this.Definition);

        this._areaOfInterestManager = new BucketAreaOfInterestManager(chunkSize);
        this._objectIdGenerator = new IdGenerator(ViewExtensions.ConstantPlayerId + 1, 0x7FFF);
        this._dropIdGenerator = new IdGenerator(0, ViewExtensions.ConstantPlayerId - 1);

        this._safezoneSpawnGate = this.Definition.GetSafezoneGate(this.Terrain);
    }

    /// <summary>
    /// Occurs when an object was added to the map.
    /// </summary>
    public event EventHandler<(GameMap Map, ILocateable Object)>? ObjectAdded;

    /// <summary>
    /// Occurs when an object was removed from the map.
    /// </summary>
    public event EventHandler<(GameMap Map, ILocateable Object)>? ObjectRemoved;

    /// <summary>
    /// Gets the map identifier.
    /// </summary>
    public ushort MapId => this.Definition.Number.ToUnsigned();

    /// <summary>
    /// Gets the terrain of the map.
    /// </summary>
    public GameMapTerrain Terrain { get; }

    /// <summary>
    /// Gets the safe zone spawn gate.
    /// </summary>
    public ExitGate? SafeZoneSpawnGate => this._safezoneSpawnGate;

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
    public ILocateable? GetObject(ushort id)
    {
        this._objectsInMap.TryGetValue(id, out var result);
        return result;
    }

    /// <summary>
    /// Gets the attackables in range of the specified coordinates.
    /// </summary>
    /// <param name="point">The coordinates.</param>
    /// <param name="range">The range.</param>
    /// <returns>The attackables in range of the specified coordinate.</returns>
    public IEnumerable<IAttackable> GetAttackablesInRange(Point point, int range)
    {
        return this._areaOfInterestManager.GetInRange(point, range).OfType<IAttackable>().ToList();
    }

    /// <summary>
    /// Gets the drop by id.
    /// </summary>
    /// <param name="dropId">The drop identifier.</param>
    /// <returns>The dropped item.</returns>
    public ILocateable? GetDrop(ushort dropId)
    {
        this._objectsInMap.TryGetValue(dropId, out var item);
        return item;
    }

    /// <summary>
    /// Removes the locateable from the map.
    /// </summary>
    /// <param name="locateable">The locateable.</param>
    public void Remove(ILocateable locateable)
    {
        this._areaOfInterestManager.RemoveObject(locateable);
        if (this._objectsInMap.Remove(locateable.Id) && locateable.Id != 0)
        {
            if (locateable is DroppedItem
                || locateable is DroppedMoney)
            {
                this._dropIdGenerator.GiveBack(locateable.Id);
            }
            else
            {
                this._objectIdGenerator.GiveBack(locateable.Id);
            }

            if (locateable is Player player)
            {
                player.Id = 0;
                Interlocked.Decrement(ref this._playerCount);
            }

            this.ObjectRemoved?.Invoke(this, (this, locateable));
        }
    }

    /// <summary>
    /// Adds the locateable to the map.
    /// </summary>
    /// <param name="locateable">The locateable object.</param>
    public void Add(ILocateable locateable)
    {
        switch (locateable)
        {
            case DroppedItem droppedItem:
                droppedItem.Id = (ushort)this._dropIdGenerator.GenerateId();
                break;
            case DroppedMoney droppedMoney:
                droppedMoney.Id = (ushort)this._dropIdGenerator.GenerateId();
                break;
            case Player player:
                player.Id = (ushort)this._objectIdGenerator.GenerateId();
                Interlocked.Increment(ref this._playerCount);
                break;
            case NonPlayerCharacter npc:
                npc.Id = (ushort)this._objectIdGenerator.GenerateId();
                break;
            case ISupportIdUpdate idUpdate:
                idUpdate.Id = (ushort)this._objectIdGenerator.GenerateId();
                break;
            default:
                throw new ArgumentException($"Adding an object of type {locateable.GetType()} is not supported.");
        }

        this._objectsInMap.Add(locateable.Id, locateable);
        this._areaOfInterestManager.AddObject(locateable);
        this.ObjectAdded?.Invoke(this, (this, locateable));
    }

    /// <summary>
    /// Moves the locatable on the map.
    /// </summary>
    /// <param name="locatable">The monster.</param>
    /// <param name="target">The new coordinates.</param>
    /// <param name="moveLock">The move lock.</param>
    /// <param name="moveType">Type of the move.</param>
    public void Move(ILocateable locatable, Point target, object moveLock, MoveType moveType)
    {
        this._areaOfInterestManager.MoveObject(locatable, target, moveLock, moveType);
    }

    /// <summary>
    /// Respawns the specified locateable.
    /// </summary>
    /// <param name="locateable">The locateable.</param>
    public void Respawn(ILocateable locateable)
    {
        this._areaOfInterestManager.RemoveObject(locateable);
        this._areaOfInterestManager.AddObject(locateable);
    }

    /// <summary>
    /// Clears event NPCs.
    /// </summary>
    public void ClearEventSpawnedNpcs()
    {
        var eventMonsters = this._objectsInMap.Values
            .OfType<NonPlayerCharacter>()
            .Where(n => n.SpawnArea.SpawnTrigger is not SpawnTrigger.Automatic)
            .ToList();
        eventMonsters.ForEach(m =>
        {
            m.CurrentMap.Remove(m);
            m.Dispose();
        });
    }
}