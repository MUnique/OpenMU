// <copyright file="FortressOfImperialGuardian2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

/// <summary>
/// Map initialization for the Empire fortress 2 event map.
/// </summary>
internal class FortressOfImperialGuardian2 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 70;

    /// <summary>
    /// Initializes a new instance of the <see cref="FortressOfImperialGuardian2"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public FortressOfImperialGuardian2(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the name of the map.
    /// </summary>
    internal static LocalizedString Name => LocalizedString.FromResource(() => MapNames.FortressOfImperialGuardian2);

    /// <inheritdoc />
    protected override byte MapNumber => Number;

    /// <inheritdoc />
    protected override LocalizedString MapName => Name;

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Devias.Number;

    /// <inheritdoc />
    protected override void CreateMonsters()
    {
        // All Monsters and NPCs are defined in the first map.
    }

    /// <inheritdoc />
    /// <remarks>
    /// The monsters are spawned by the imperial guardian event, zone by zone.
    /// </remarks>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        return ImperialGuardianSpawns.GetSpawns(Number).Select(spawn => this.CreateMonsterSpawn(
            spawn.Number,
            this.NpcDictionary[spawn.MonsterNumber],
            spawn.X,
            spawn.X,
            spawn.Y,
            spawn.Y,
            1,
            spawn.Direction,
            SpawnTrigger.OnceAtWaveStart,
            spawn.WaveNumber));
    }
}