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

    private readonly Stack<IUndoStep> _undoStack = new();

    /// <summary>
    /// Gets a value indicating whether there are any steps available to undo.
    /// </summary>
    public bool CanUndo => this._undoStack.Count > 0;

    /// <summary>
    /// Records the current state of a spawn area before a mutation.
    /// </summary>
    /// <param name="spawn">The spawn area to snapshot.</param>
    public void RecordSnapshot(MonsterSpawnArea spawn)
    {
        this.Push(new SpawnAreaSnapshot(spawn));
    }

    /// <summary>
    /// Records the current state of a gate before a mutation.
    /// </summary>
    /// <param name="gate">The gate to snapshot.</param>
    public void RecordSnapshot(Gate gate)
    {
        this.Push(new GateSnapshot(gate));
    }

    /// <summary>
    /// Records a creation step so it can be undone by removing the object.
    /// </summary>
    /// <param name="map">The map the object was added to.</param>
    /// <param name="obj">The object that was created.</param>
    public void RecordCreation(GameMapDefinition map, object obj)
    {
        this.Push(new CreationStep(map, obj));
    }

    /// <summary>
    /// Records a deletion step so it can be undone by re-adding the object.
    /// </summary>
    /// <param name="map">The map the object was removed from.</param>
    /// <param name="obj">The object that was deleted.</param>
    public void RecordDeletion(GameMapDefinition map, object obj)
    {
        this.Push(new DeletionStep(map, obj));
    }

    /// <summary>
    /// Undoes the most recent recorded step.
    /// </summary>
    /// <returns>The object that was affected by the undo, or <see langword="null"/>.</returns>
    public object? Undo()
    {
        if (!this._undoStack.TryPop(out var step))
        {
            return null;
        }

        return step.Undo();
    }

    /// <summary>
    /// Clears all recorded history.
    /// </summary>
    public void Clear() => this._undoStack.Clear();

    private void Push(IUndoStep step)
    {
        if (this._undoStack.Count >= MaxUndoSteps)
        {
            var items = this._undoStack.ToArray();
            this._undoStack.Clear();
            foreach (var item in items.Take(items.Length - 1).Reverse())
            {
                this._undoStack.Push(item);
            }
        }

        this._undoStack.Push(step);
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
                    break;
                case EnterGate enterGate:
                    this._map.EnterGates.Remove(enterGate);
                    break;
                case ExitGate exitGate:
                    this._map.ExitGates.Remove(exitGate);
                    break;
                default:
                    // Unsupported object type.
                    break;
            }

            return null;
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