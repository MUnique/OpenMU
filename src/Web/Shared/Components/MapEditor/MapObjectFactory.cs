// <copyright file="MapObjectFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Handles creation and duplication of map objects within a <see cref="GameMapDefinition"/>,
/// delegating persistence to an <see cref="IContext"/>.
/// </summary>
public sealed class MapObjectFactory
{
    private const byte DefaultOffset = 5;

    private readonly IContext _persistenceContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapObjectFactory"/> class.
    /// </summary>
    /// <param name="persistenceContext">The persistence context used to create new model objects.</param>
    public MapObjectFactory(IContext persistenceContext)
    {
        this._persistenceContext = persistenceContext;
    }

    /// <summary>
    /// Creates a new <see cref="MonsterSpawnArea"/> with default bounds and adds it to the map.
    /// </summary>
    /// <param name="map">The map to add the spawn area to.</param>
    /// <returns>The newly created spawn area.</returns>
    public MonsterSpawnArea CreateSpawnArea(GameMapDefinition map)
    {
        var area = this._persistenceContext.CreateNew<MonsterSpawnArea>();
        area.GameMap = map;
        area.X1 = 100;
        area.Y1 = 100;
        area.X2 = 200;
        area.Y2 = 200;
        area.Quantity = 1;
        map.MonsterSpawns.Add(area);
        return area;
    }

    /// <summary>
    /// Creates a new <see cref="EnterGate"/> with default bounds and adds it to the map.
    /// </summary>
    /// <param name="map">The map to add the enter gate to.</param>
    /// <returns>The newly created enter gate.</returns>
    public EnterGate CreateEnterGate(GameMapDefinition map)
    {
        var gate = this._persistenceContext.CreateNew<EnterGate>();
        gate.X1 = 120;
        gate.Y1 = 120;
        gate.X2 = 140;
        gate.Y2 = 140;
        map.EnterGates.Add(gate);
        return gate;
    }

    /// <summary>
    /// Creates a new <see cref="ExitGate"/> with default bounds and adds it to the map.
    /// </summary>
    /// <param name="map">The map to add the exit gate to.</param>
    /// <returns>The newly created exit gate.</returns>
    public ExitGate CreateExitGate(GameMapDefinition map)
    {
        var gate = this._persistenceContext.CreateNew<ExitGate>();
        gate.Map = map;
        gate.X1 = 120;
        gate.Y1 = 120;
        gate.X2 = 140;
        gate.Y2 = 140;
        map.ExitGates.Add(gate);
        return gate;
    }

    /// <summary>
    /// Duplicates the given map object with a positional offset and adds the copy to the map.
    /// </summary>
    /// <param name="source">The source object to duplicate.</param>
    /// <param name="map">The map to add the duplicated object to.</param>
    /// <returns>The duplicated object, or <see langword="null"/> if the type is not supported.</returns>
    public object? Duplicate(object source, GameMapDefinition map) => source switch
    {
        MonsterSpawnArea s => this.DuplicateSpawn(s, map),
        EnterGate g => this.DuplicateEnterGate(g, map),
        ExitGate g => this.DuplicateExitGate(g, map),
        _ => null,
    };

    /// <summary>
    /// Applies a positional offset to a coordinate pair, clamping to map bounds
    /// while preserving the original width and height.
    /// </summary>
    private static void ApplyOffset(byte x1, byte y1, byte x2, byte y2, out byte outX1, out byte outY1, out byte outX2, out byte outY2)
    {
        outX1 = (byte)Math.Min(x1 + DefaultOffset, byte.MaxValue);
        outY1 = (byte)Math.Min(y1 + DefaultOffset, byte.MaxValue);

        var actualXOffset = outX1 - x1;
        var actualYOffset = outY1 - y1;

        outX2 = (byte)Math.Min(x2 + actualXOffset, byte.MaxValue);
        outY2 = (byte)Math.Min(y2 + actualYOffset, byte.MaxValue);
    }

    /// <summary>
    /// Duplicates a <see cref="MonsterSpawnArea"/> with a positional offset.
    /// </summary>
    /// <param name="original">The original spawn area.</param>
    /// <param name="map">The map to add the duplicate to.</param>
    /// <returns>The duplicated spawn area.</returns>
    private MonsterSpawnArea DuplicateSpawn(MonsterSpawnArea original, GameMapDefinition map)
    {
        ApplyOffset(original.X1, original.Y1, original.X2, original.Y2, out var x1, out var y1, out var x2, out var y2);
        var spawn = this._persistenceContext.CreateNew<MonsterSpawnArea>();
        spawn.GameMap = map;
        spawn.MonsterDefinition = original.MonsterDefinition;
        spawn.X1 = x1;
        spawn.Y1 = y1;
        spawn.X2 = x2;
        spawn.Y2 = y2;
        spawn.Direction = original.Direction;
        spawn.Quantity = original.Quantity;
        spawn.SpawnTrigger = original.SpawnTrigger;
        spawn.WaveNumber = original.WaveNumber;
        spawn.MaximumHealthOverride = original.MaximumHealthOverride;
        map.MonsterSpawns.Add(spawn);
        return spawn;
    }

    /// <summary>
    /// Duplicates an <see cref="EnterGate"/> with a positional offset.
    /// </summary>
    /// <param name="original">The original enter gate.</param>
    /// <param name="map">The map to add the duplicate to.</param>
    /// <returns>The duplicated enter gate.</returns>
    private EnterGate DuplicateEnterGate(EnterGate original, GameMapDefinition map)
    {
        ApplyOffset(original.X1, original.Y1, original.X2, original.Y2, out var outX1, out var outY1, out var outX2, out var outY2);
        var gate = this._persistenceContext.CreateNew<EnterGate>();
        gate.X1 = outX1;
        gate.Y1 = outY1;
        gate.X2 = outX2;
        gate.Y2 = outY2;
        gate.TargetGate = original.TargetGate;
        gate.LevelRequirement = original.LevelRequirement;
        gate.Number = original.Number;
        map.EnterGates.Add(gate);
        return gate;
    }

    /// <summary>
    /// Duplicates an <see cref="ExitGate"/> with a positional offset.
    /// </summary>
    /// <param name="original">The original exit gate.</param>
    /// <param name="map">The map to add the duplicate to.</param>
    /// <returns>The duplicated exit gate.</returns>
    private ExitGate DuplicateExitGate(ExitGate original, GameMapDefinition map)
    {
        ApplyOffset(original.X1, original.Y1, original.X2, original.Y2, out var outX1, out var outY1, out var outX2, out var outY2);
        var gate = this._persistenceContext.CreateNew<ExitGate>();
        gate.Map = map;
        gate.X1 = outX1;
        gate.Y1 = outY1;
        gate.X2 = outX2;
        gate.Y2 = outY2;
        gate.Direction = original.Direction;
        gate.IsSpawnGate = original.IsSpawnGate;
        map.ExitGates.Add(gate);
        return gate;
    }
}