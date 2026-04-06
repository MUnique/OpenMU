// <copyright file="MapEditorHistory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Records and restores snapshots of map object state to support undo operations.
/// </summary>
public sealed class MapEditorHistory
{
    private const int MaxUndoSteps = 50;

    private readonly List<IUndoStep> _undoList = [];

    /// <summary>
    /// Gets a value indicating whether there are any steps available to undo.
    /// </summary>
    public bool CanUndo => this._undoList.Count > 0;

    /// <summary>
    /// Records the current state of a spawn area before a mutation.
    /// </summary>
    /// <param name="spawn">The spawn area to snapshot.</param>
    public void RecordSnapshot(MonsterSpawnArea spawn)
    {
        this.Add(new SpawnAreaSnapshot(spawn));
    }

    /// <summary>
    /// Records the current state of a gate before a mutation.
    /// </summary>
    /// <param name="gate">The gate to snapshot.</param>
    public void RecordSnapshot(Gate gate)
    {
        this.Add(new GateSnapshot(gate));
    }

    /// <summary>
    /// Records a creation step so it can be undone by removing the object.
    /// </summary>
    /// <param name="map">The map the object was added to.</param>
    /// <param name="obj">The object that was created.</param>
    public void RecordCreation(GameMapDefinition map, object obj)
    {
        this.Add(new CreationStep(map, obj));
    }

    /// <summary>
    /// Records a deletion step so it can be undone by re-adding the object.
    /// </summary>
    /// <param name="map">The map the object was removed from.</param>
    /// <param name="obj">The object that was deleted.</param>
    public void RecordDeletion(GameMapDefinition map, object obj)
    {
        this.Add(new DeletionStep(map, obj));
    }

    /// <summary>
    /// Undoes the most recent recorded step.
    /// </summary>
    /// <returns>The object that was affected by the undo, or <see langword="null"/>.</returns>
    public object? Undo()
    {
        if (this._undoList.Count == 0)
        {
            return null;
        }

        var index = this._undoList.Count - 1;
        var step = this._undoList[index];
        this._undoList.RemoveAt(index);

        return step.Undo();
    }

    /// <summary>
    /// Clears all recorded history.
    /// </summary>
    public void Clear() => this._undoList.Clear();

    private void Add(IUndoStep step)
    {
        if (this._undoList.Count >= MaxUndoSteps)
        {
            this._undoList.RemoveAt(0);
        }

        this._undoList.Add(step);
    }

    private interface IUndoStep
    {
        object? Undo();
    }

    private sealed class SpawnAreaSnapshot : IUndoStep
    {
        private readonly MonsterSpawnArea _spawn;
        private readonly byte _x1;
        private readonly byte _y1;
        private readonly byte _x2;
        private readonly byte _y2;
        private readonly Direction _direction;

        public SpawnAreaSnapshot(MonsterSpawnArea spawn)
        {
            this._spawn = spawn;
            this._x1 = spawn.X1;
            this._y1 = spawn.Y1;
            this._x2 = spawn.X2;
            this._y2 = spawn.Y2;
            this._direction = spawn.Direction;
        }

        public object? Undo()
        {
            this._spawn.X1 = this._x1;
            this._spawn.Y1 = this._y1;
            this._spawn.X2 = this._x2;
            this._spawn.Y2 = this._y2;
            this._spawn.Direction = this._direction;
            return this._spawn;
        }
    }

    private sealed class GateSnapshot : IUndoStep
    {
        private readonly Gate _gate;
        private readonly byte _x1;
        private readonly byte _y1;
        private readonly byte _x2;
        private readonly byte _y2;

        public GateSnapshot(Gate gate)
        {
            this._gate = gate;
            this._x1 = gate.X1;
            this._y1 = gate.Y1;
            this._x2 = gate.X2;
            this._y2 = gate.Y2;
        }

        public object? Undo()
        {
            this._gate.X1 = this._x1;
            this._gate.Y1 = this._y1;
            this._gate.X2 = this._x2;
            this._gate.Y2 = this._y2;
            return this._gate;
        }
    }

    private sealed class CreationStep : IUndoStep
    {
        private readonly GameMapDefinition _map;
        private readonly object _obj;

        public CreationStep(GameMapDefinition map, object obj)
        {
            this._map = map;
            this._obj = obj;
        }

        public object? Undo()
        {
            switch (this._obj)
            {
                case MonsterSpawnArea spawn:
                    this._map.MonsterSpawns.Remove(spawn);
                    return spawn;
                case EnterGate enterGate:
                    this._map.EnterGates.Remove(enterGate);
                    return enterGate;
                case ExitGate exitGate:
                    this._map.ExitGates.Remove(exitGate);
                    return exitGate;
                default:
                    // Unsupported object type.
                    return null;
            }
        }
    }

    private sealed class DeletionStep : IUndoStep
    {
        private readonly GameMapDefinition _map;
        private readonly object _obj;

        public DeletionStep(GameMapDefinition map, object obj)
        {
            this._map = map;
            this._obj = obj;
        }

        public object? Undo()
        {
            switch (this._obj)
            {
                case MonsterSpawnArea spawn:
                    this._map.MonsterSpawns.Add(spawn);
                    break;
                case EnterGate enterGate:
                    this._map.EnterGates.Add(enterGate);
                    break;
                case ExitGate exitGate:
                    this._map.ExitGates.Add(exitGate);
                    break;
                default:
                    // Unsupported object type.
                    break;
            }

            return this._obj;
        }
    }
}
