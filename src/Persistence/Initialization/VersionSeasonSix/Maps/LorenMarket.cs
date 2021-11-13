// <copyright file="LorenMarket.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the LorenMarket map.
/// </summary>
internal class LorenMarket : BaseMapInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LorenMarket"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public LorenMarket(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 79;

    /// <inheritdoc/>
    protected override string MapName => "LorenMarket";

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        // yield return this.CreateMonsterSpawn(NpcDictionary[240], 1, Direction.SouthWest, SpawnTrigger.Automatic, 045, 045, 075, 075); // Safety Guardian
        // yield return this.CreateMonsterSpawn(NpcDictionary[240], 1, Direction.SouthWest, SpawnTrigger.Automatic, 077, 077, 045, 045); // Safety Guardian
        // yield return this.CreateMonsterSpawn(NpcDictionary[385], 1, Direction.SouthWest, SpawnTrigger.Automatic, 014, 014, 067, 067); // Mirage
        // yield return this.CreateMonsterSpawn(NpcDictionary[540], 1, Direction.SouthEast, SpawnTrigger.Automatic, 086, 086, 129, 129); // Lugard
        yield return this.CreateMonsterSpawn(this.NpcDictionary[545], 113, 116, Direction.SouthEast); // Christine the General Goods Merchant
        yield return this.CreateMonsterSpawn(this.NpcDictionary[546], 131, 109, Direction.SouthEast); // Jeweler Raul
        yield return this.CreateMonsterSpawn(this.NpcDictionary[547], 123, 140, Direction.SouthEast); // Market Union Member Julia
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        // no monsters here
    }
}