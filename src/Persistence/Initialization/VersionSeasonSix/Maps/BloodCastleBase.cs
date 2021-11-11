// <copyright file="BloodCastleBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Initialization for the Blood Castle.
/// </summary>
internal abstract class BloodCastleBase : BaseMapInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastleBase"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    protected BloodCastleBase(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(this.NpcDictionary[131], 014, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Castle Gate
        yield return this.CreateMonsterSpawn(this.NpcDictionary[132], 014, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Statue of Saint
        yield return this.CreateMonsterSpawn(this.NpcDictionary[232], 010, 009, Direction.SouthWest, SpawnTrigger.AutomaticDuringEvent); // Archangel
    }
}