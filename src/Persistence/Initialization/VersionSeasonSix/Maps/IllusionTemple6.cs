// <copyright file="IllusionTemple6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Initialization for the Illusion Temple 6.
/// </summary>
internal class IllusionTemple6 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 50;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Illusion Temple 6";

    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTemple6"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public IllusionTemple6(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Devias.Number;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        // NPCs:
        // Pool of Stone Statue (380) positions - only one of them is active at a time, randomly picked
        // from these two by the game logic. Positions confirmed against a working Season 6 Episode 3
        // server's spawn list.
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[380], 207, 047, Direction.Undefined, SpawnTrigger.ManuallyForEvent);
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[380], 134, 121, Direction.Undefined, SpawnTrigger.ManuallyForEvent);

        // Team guardians (decorative).
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[381], 139, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // MU Allies General
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[382], 194, 123, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Illusion Elder

        // Relic delivery targets - the team which carries the relic here scores a point.
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[383], 141, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Alliance Item Storage
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[384], 194, 113, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Illusion Item Storage

        // Temple 6 has no roaming "Illusion Sorc. Spirit" monsters.
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        // no monsters here
    }
}