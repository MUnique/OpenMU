// <copyright file="MapExportImportService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using System.Text.Json;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Handles the export and import of map object data,
/// converting between domain objects and JSON.
/// </summary>
public sealed class MapExportImportService
{
    /// <summary>
    /// Builds a JSON export string from the objects in the given map.
    /// </summary>
    /// <param name="map">The map whose spawns and gates are to be exported.</param>
    /// <returns>A JSON string representing all map objects.</returns>
    public string BuildExport(GameMapDefinition map)
    {
        var dto = new MapSpawnExport();

        foreach (var spawn in map.MonsterSpawns)
        {
            dto.Spawns.Add(new SpawnExport
            {
                Id = spawn.GetId(),
                X1 = spawn.X1,
                Y1 = spawn.Y1,
                X2 = spawn.X2,
                Y2 = spawn.Y2,
                Direction = spawn.Direction,
                Quantity = spawn.Quantity,
                SpawnTrigger = spawn.SpawnTrigger,
                WaveNumber = spawn.WaveNumber,
                MaximumHealthOverride = spawn.MaximumHealthOverride,
                MonsterNumber = spawn.MonsterDefinition?.Number ?? 0,
            });
        }

        foreach (var gate in map.ExitGates)
        {
            dto.ExitGates.Add(new ExitGateExport
            {
                Id = gate.GetId(),
                X1 = gate.X1,
                Y1 = gate.Y1,
                X2 = gate.X2,
                Y2 = gate.Y2,
                Direction = gate.Direction,
                IsSpawnGate = gate.IsSpawnGate,
            });
        }

        foreach (var gate in map.EnterGates)
        {
            dto.EnterGates.Add(new EnterGateExport
            {
                Id = gate.GetId(),
                X1 = gate.X1,
                Y1 = gate.Y1,
                X2 = gate.X2,
                Y2 = gate.Y2,
                LevelRequirement = gate.LevelRequirement,
                Number = gate.Number,
                TargetGateId = gate.TargetGate?.GetId(),
            });
        }

        return JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Applies a JSON import to the given map, replacing all monster spawns
    /// with those parsed from the JSON. Existing spawns are deleted via the persistence context.
    /// Gates are preserved to avoid breaking warp entries and cross-map references.
    /// </summary>
    /// <param name="map">The map whose spawns will be replaced.</param>
    /// <param name="json">The JSON string containing the replacement spawns.</param>
    /// <param name="context">The persistence context used for create and delete operations.</param>
    public async Task ApplyImportAsync(GameMapDefinition map, string json, IContext context)
    {
        MapSpawnExport? dto;
        try
        {
            dto = JsonSerializer.Deserialize<MapSpawnExport>(json);
        }
        catch (JsonException)
        {
            return;
        }

        if (dto is null || dto.FormatVersion != "1.0")
        {
            return;
        }

        foreach (var spawn in map.MonsterSpawns.ToList())
        {
            map.MonsterSpawns.Remove(spawn);
            await context.DeleteAsync(spawn).ConfigureAwait(false);
        }

        var monsters = await context.GetAsync<MonsterDefinition>().ConfigureAwait(false);
        if (monsters is null || dto.Spawns is null)
        {
            return;
        }

        foreach (var spawnDto in dto.Spawns)
        {
            var monsterDef = monsters.FirstOrDefault(m => m.Number == spawnDto.MonsterNumber);
            if (monsterDef is null)
            {
                continue;
            }

            var spawn = context.CreateNew<MonsterSpawnArea>();
            spawn.X1 = spawnDto.X1;
            spawn.Y1 = spawnDto.Y1;
            spawn.X2 = spawnDto.X2;
            spawn.Y2 = spawnDto.Y2;
            spawn.Direction = spawnDto.Direction;
            spawn.Quantity = spawnDto.Quantity;
            spawn.SpawnTrigger = spawnDto.SpawnTrigger;
            spawn.WaveNumber = spawnDto.WaveNumber;
            spawn.MaximumHealthOverride = spawnDto.MaximumHealthOverride;
            spawn.MonsterDefinition = monsterDef;
            spawn.GameMap = map;
            map.MonsterSpawns.Add(spawn);
        }
    }
}
