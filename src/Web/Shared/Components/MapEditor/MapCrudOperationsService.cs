// <copyright file="MapCrudOperationsService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Provides create, delete, and duplicate operations for map objects,
/// coordinating between <see cref="MapObjectFactory"/> and <see cref="MapEditorHistory"/>.
/// </summary>
public sealed class MapCrudOperationsService
{
    private readonly MapObjectFactory _factory;
    private readonly MapEditorHistory _history;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapCrudOperationsService"/> class.
    /// </summary>
    /// <param name="factory">The factory used to create and duplicate objects.</param>
    /// <param name="history">The history used to record changes for undo support.</param>
    public MapCrudOperationsService(MapObjectFactory factory, MapEditorHistory history)
    {
        this._factory = factory;
        this._history = history;
    }

    /// <summary>
    /// Creates a new <see cref="MonsterSpawnArea"/> on the specified map and records it in history.
    /// </summary>
    /// <param name="map">The map to add the spawn area to.</param>
    /// <returns>The newly created spawn area.</returns>
    public MonsterSpawnArea CreateSpawnArea(GameMapDefinition map)
    {
        var obj = this._factory.CreateSpawnArea(map);
        this._history.RecordCreation(map, obj);
        return obj;
    }

    /// <summary>
    /// Creates a new <see cref="EnterGate"/> on the specified map and records it in history.
    /// </summary>
    /// <param name="map">The map to add the enter gate to.</param>
    /// <returns>The newly created enter gate.</returns>
    public EnterGate CreateEnterGate(GameMapDefinition map)
    {
        var obj = this._factory.CreateEnterGate(map);
        this._history.RecordCreation(map, obj);
        return obj;
    }

    /// <summary>
    /// Creates a new <see cref="ExitGate"/> on the specified map and records it in history.
    /// </summary>
    /// <param name="map">The map to add the exit gate to.</param>
    /// <returns>The newly created exit gate.</returns>
    public ExitGate CreateExitGate(GameMapDefinition map)
    {
        var obj = this._factory.CreateExitGate(map);
        this._history.RecordCreation(map, obj);
        return obj;
    }

    /// <summary>
    /// Removes the specified object from its parent map collection and records the deletion in history.
    /// The removed object is added to the pending deletions list for later persistence.
    /// </summary>
    /// <param name="map">The map containing the object.</param>
    /// <param name="target">The object to remove.</param>
    /// <param name="pendingDeletions">The list of objects awaiting persistent deletion.</param>
    public void RemoveFromMap(GameMapDefinition map, object target, List<object> pendingDeletions)
    {
        this._history.RecordDeletion(map, target);

        switch (target)
        {
            case MonsterSpawnArea spawnArea:
                map.MonsterSpawns.Remove(spawnArea);
                pendingDeletions.Add(spawnArea);
                break;
            case EnterGate enterGate:
                map.EnterGates.Remove(enterGate);
                pendingDeletions.Add(enterGate);
                break;
            case ExitGate exitGate:
                map.ExitGates.Remove(exitGate);
                pendingDeletions.Add(exitGate);
                break;
            default:
                // Not supported.
                break;
        }
    }

    /// <summary>
    /// Duplicates the specified object, records the creation in history, and returns the duplicate.
    /// </summary>
    /// <param name="target">The object to duplicate.</param>
    /// <param name="map">The map to which the duplicate belongs.</param>
    /// <returns>The duplicated object, or <see langword="null"/> if duplication is not supported for the type.</returns>
    public object? DuplicateObject(object target, GameMapDefinition map)
    {
        var duplicate = this._factory.Duplicate(target, map);
        if (duplicate is not null)
        {
            this._history.RecordCreation(map, duplicate);
        }

        return duplicate;
    }
}
