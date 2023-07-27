// <copyright file="SpawnFixesUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The chaos castle update plugin.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("15FB42DE-A032-49B5-98B8-4CF34744B3A6")]
public class SpawnFixesUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Spawn Points Fixes";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This updates the spawn points of NPCs. 1) Crywolf-Statues are fixed. 2) Wandering merchants and Marlon. 3) Spawn trigger in chaos castle 1";
    private const short MarlonNumber = 229;
    private const short Wandering1Number = 248;
    private const short Wandering2Number = 250;
    private const short ZyroNumber = 568;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.SpawnFixesUpdate;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2023, 04, 03, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
#pragma warning disable CS1998
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
#pragma warning restore CS1998
    {
        var wanderingSpawns = gameConfiguration.Maps.SelectMany(m => m.MonsterSpawns.Where(s => s.MonsterDefinition?.Number is MarlonNumber or Wandering1Number or Wandering2Number or ZyroNumber));
        foreach (var spawn in wanderingSpawns)
        {
            spawn.SpawnTrigger = SpawnTrigger.Wandering;
        }

        UpdateChaosCastle1(gameConfiguration);
        UpdateCrywolf(gameConfiguration);
        UpdateAtlans(context, gameConfiguration);
        UpdateNoria(context, gameConfiguration);
        UpdateDevias(context, gameConfiguration);
        UpdateLorencia(context, gameConfiguration);
    }

    private static void UpdateAtlans(IContext context, GameConfiguration gameConfiguration)
    {
        var atlans = gameConfiguration.Maps.First(m => m.Number == Atlans.Number);
        var marlonAtlansSpawn = context.CreateNew<MonsterSpawnArea>();
        atlans.MonsterSpawns.Add(marlonAtlansSpawn);
        marlonAtlansSpawn.SetGuid(10);
        marlonAtlansSpawn.GameMap = atlans;
        marlonAtlansSpawn.MonsterDefinition = gameConfiguration.Monsters.First(m => m.Number == MarlonNumber);
        marlonAtlansSpawn.SpawnTrigger = SpawnTrigger.Wandering;
        marlonAtlansSpawn.Direction = Direction.SouthEast;
        marlonAtlansSpawn.X1 = 17;
        marlonAtlansSpawn.X2 = 17;
        marlonAtlansSpawn.Y1 = 35;
        marlonAtlansSpawn.Y2 = 35;
    }

    private static void UpdateNoria(IContext context, GameConfiguration gameConfiguration)
    {
        var noria = gameConfiguration.Maps.First(m => m.Number == Noria.Number);
        var marlonSpawn = context.CreateNew<MonsterSpawnArea>();
        noria.MonsterSpawns.Add(marlonSpawn);
        marlonSpawn.SetGuid(14);
        marlonSpawn.GameMap = noria;
        marlonSpawn.MonsterDefinition = gameConfiguration.Monsters.First(m => m.Number == MarlonNumber);
        marlonSpawn.SpawnTrigger = SpawnTrigger.Wandering;
        marlonSpawn.Direction = Direction.SouthEast;
        marlonSpawn.X1 = 169;
        marlonSpawn.X2 = 169;
        marlonSpawn.Y1 = 88;
        marlonSpawn.Y2 = 88;
    }

    private static void UpdateDevias(IContext context, GameConfiguration gameConfiguration)
    {
        var devias = gameConfiguration.Maps.First(m => m.Number == Devias.Number);
        var zyro = context.CreateNew<MonsterSpawnArea>();
        devias.MonsterSpawns.Add(zyro);
        zyro.SetGuid(42);
        zyro.GameMap = devias;
        zyro.MonsterDefinition = gameConfiguration.Monsters.First(m => m.Number == ZyroNumber);
        zyro.SpawnTrigger = SpawnTrigger.Wandering;
        zyro.Direction = Direction.South;
        zyro.X1 = 225;
        zyro.X2 = 225;
        zyro.Y1 = 52;
        zyro.Y2 = 52;

        var marlon = devias.MonsterSpawns.First(s => s.MonsterDefinition?.Number == MarlonNumber);
        marlon.SpawnTrigger = SpawnTrigger.Wandering;
        marlon.X1 = 197;
        marlon.X2 = 197;
        marlon.Y1 = 48;
        marlon.Y2 = 48;
    }

    private static void UpdateLorencia(IContext context, GameConfiguration gameConfiguration)
    {
        var lorencia = gameConfiguration.Maps.First(m => m.Number == Lorencia.Number);
        var zyro = context.CreateNew<MonsterSpawnArea>();
        lorencia.MonsterSpawns.Add(zyro);
        zyro.SetGuid(26);
        zyro.GameMap = lorencia;
        zyro.MonsterDefinition = gameConfiguration.Monsters.First(m => m.Number == ZyroNumber);
        zyro.SpawnTrigger = SpawnTrigger.Wandering;
        zyro.Direction = Direction.South;
        zyro.X1 = 131;
        zyro.X2 = 131;
        zyro.Y1 = 139;
        zyro.Y2 = 139;
    }

    private static void UpdateChaosCastle1(GameConfiguration gameConfiguration)
    {
        var chaosCastle = gameConfiguration.Maps.First(m => m.Number == ChaosCastle1.Number);
        foreach (var spawn in chaosCastle.MonsterSpawns)
        {
            spawn.SpawnTrigger = SpawnTrigger.ManuallyForEvent;
        }
    }

    private static void UpdateCrywolf(GameConfiguration gameConfiguration)
    {
        var chaosCastle = gameConfiguration.Maps.First(m => m.Number == CrywolfFortress.Number);
        foreach (var spawn in chaosCastle.MonsterSpawns.Where(s => s.MonsterDefinition?.Number is >= 204 and <= 209))
        {
            spawn.SpawnTrigger = SpawnTrigger.OnceAtEventStart;
        }
    }
}