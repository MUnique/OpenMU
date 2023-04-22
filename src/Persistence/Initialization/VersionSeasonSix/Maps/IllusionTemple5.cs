// <copyright file="IllusionTemple5.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Initialization for the Illusion Temple 5.
/// </summary>
internal class IllusionTemple5 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 49;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Illusion Temple 5";

    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTemple5"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public IllusionTemple5(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        // NPCs:
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[658], 169, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Cursed Statue
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[659], 136, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (1)
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[660], 151, 119, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (2)
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[661], 150, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (3)
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[662], 165, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (4)
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[663], 173, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (5)
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[664], 187, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (6)
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[665], 187, 051, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (7)
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[666], 203, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (8)
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[667], 133, 121, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (9)
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[668], 206, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (10)
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        // no monsters here
    }
}