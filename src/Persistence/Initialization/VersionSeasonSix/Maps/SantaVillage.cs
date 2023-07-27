// <copyright file="SantaVillage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the Santa Village map.
/// </summary>
internal class SantaVillage : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 62;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Santa Village";



    /// <summary>
    /// Initializes a new instance of the <see cref="SantaVillage"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public SantaVillage(IContext context, GameConfiguration gameConfiguration)
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
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(1, this.NpcDictionary[467], 202, 041, Direction.SouthWest); // Snowman
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[468], 222, 024, Direction.SouthWest); // Little Santa Yellow
        yield return this.CreateMonsterSpawn(3, this.NpcDictionary[469], 202, 033, Direction.SouthWest); // Little Santa Green
        yield return this.CreateMonsterSpawn(4, this.NpcDictionary[470], 192, 024, Direction.SouthWest); // Little Santa Red
        yield return this.CreateMonsterSpawn(5, this.NpcDictionary[471], 207, 009, Direction.SouthWest); // Little Santa Blue
        yield return this.CreateMonsterSpawn(6, this.NpcDictionary[472], 225, 011, Direction.SouthWest); // Little Santa White
        yield return this.CreateMonsterSpawn(7, this.NpcDictionary[473], 232, 013, Direction.SouthWest); // Little Santa Black
        yield return this.CreateMonsterSpawn(8, this.NpcDictionary[474], 216, 019, Direction.SouthWest); // Little Santa Orange
        yield return this.CreateMonsterSpawn(9, this.NpcDictionary[475], 193, 027, Direction.SouthWest); // Little Santa Pink
    }
}