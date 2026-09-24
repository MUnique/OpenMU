// <copyright file="AddRaklionEventUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adapts the monster spawns of the raklion hatchery to the raklion event.
/// </summary>
/// <remarks>
/// The spider eggs, Selupan and the monsters summoned by Selupan were spawned automatically.
/// Now they're spawned by the raklion event, depending on its state.
/// </remarks>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("2B7E9C41-5D86-4A13-B0F2-9C4A7E1D3B65")]
public class AddRaklionEventUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Add Raklion event";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "This update adapts the monster spawns of the raklion hatchery to the raklion event.";

    private const short SelupanNumber = 459;
    private const short CoolutinNumber = 457;
    private const short FirstSpiderEggNumber = 460;
    private const short LastSpiderEggNumber = 462;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddRaklionEvent;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 24, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var hatchery = gameConfiguration.Maps.FirstOrDefault(map => map.Number == RaklionBoss.Number);
        if (hatchery is null)
        {
            return ValueTask.CompletedTask;
        }

        foreach (var spawn in hatchery.MonsterSpawns.Where(spawn => spawn.SpawnTrigger == SpawnTrigger.Automatic))
        {
            if (GetWaveNumber(spawn.MonsterDefinition?.Number) is { } waveNumber)
            {
                spawn.SpawnTrigger = SpawnTrigger.OnceAtWaveStart;
                spawn.WaveNumber = waveNumber;
            }
        }

        return ValueTask.CompletedTask;
    }

    private static byte? GetWaveNumber(short? monsterNumber)
    {
        return monsterNumber switch
        {
            SelupanNumber => RaklionBoss.SelupanWaveNumber,
            CoolutinNumber => RaklionBoss.SummonWaveNumber,
            >= FirstSpiderEggNumber and <= LastSpiderEggNumber => RaklionBoss.SpiderEggWaveNumber,
            _ => null,
        };
    }
}
