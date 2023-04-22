// <copyright file="KanturuEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initialization for the Kanturu event map.
/// </summary>
internal class KanturuEvent : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 39;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Kanturu Event";

    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuEvent"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public KanturuEvent(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override void CreateMapAttributeRequirements()
    {
        // It's only required during the event. Events are not implemented yet - probably we need multiple GameMapDefinitions, for each state of a map.
        this.CreateRequirement(Stats.MoonstonePendantEquipped, 1);
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(1, this.GameConfiguration.Monsters.First(m => m.Number == 368), 77, 177, Direction.SouthWest); // Elpis NPC
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        var laserTrap = this.GameConfiguration.Monsters.First(m => m.Number == 106);
        yield return this.CreateMonsterSpawn(100, laserTrap, 60, 108);
        yield return this.CreateMonsterSpawn(101, laserTrap, 173, 61);
        yield return this.CreateMonsterSpawn(102, laserTrap, 173, 64);
        yield return this.CreateMonsterSpawn(103, laserTrap, 173, 67);
        yield return this.CreateMonsterSpawn(104, laserTrap, 173, 70);
        yield return this.CreateMonsterSpawn(105, laserTrap, 173, 73);
        yield return this.CreateMonsterSpawn(106, laserTrap, 173, 76);
        yield return this.CreateMonsterSpawn(107, laserTrap, 173, 79);
        yield return this.CreateMonsterSpawn(108, laserTrap, 179, 89);
        yield return this.CreateMonsterSpawn(109, laserTrap, 176, 86);
        yield return this.CreateMonsterSpawn(110, laserTrap, 173, 82);
        yield return this.CreateMonsterSpawn(111, laserTrap, 201, 94);
        yield return this.CreateMonsterSpawn(112, laserTrap, 204, 92);
        yield return this.CreateMonsterSpawn(113, laserTrap, 207, 91);
        yield return this.CreateMonsterSpawn(114, laserTrap, 210, 89);
        yield return this.CreateMonsterSpawn(115, laserTrap, 212, 88);
        yield return this.CreateMonsterSpawn(116, laserTrap, 215, 86);
        yield return this.CreateMonsterSpawn(117, laserTrap, 217, 84);
        yield return this.CreateMonsterSpawn(118, laserTrap, 218, 81);
        yield return this.CreateMonsterSpawn(119, laserTrap, 218, 78);
        yield return this.CreateMonsterSpawn(120, laserTrap, 218, 73);
        yield return this.CreateMonsterSpawn(121, laserTrap, 218, 70);
        yield return this.CreateMonsterSpawn(122, laserTrap, 218, 67);
        yield return this.CreateMonsterSpawn(123, laserTrap, 218, 64);
        yield return this.CreateMonsterSpawn(124, laserTrap, 217, 60);
        yield return this.CreateMonsterSpawn(125, laserTrap, 214, 57);
        yield return this.CreateMonsterSpawn(126, laserTrap, 211, 54);
        yield return this.CreateMonsterSpawn(127, laserTrap, 208, 54);
        yield return this.CreateMonsterSpawn(128, laserTrap, 205, 54);
        yield return this.CreateMonsterSpawn(129, laserTrap, 201, 54);
        yield return this.CreateMonsterSpawn(130, laserTrap, 198, 54);
        yield return this.CreateMonsterSpawn(131, laserTrap, 193, 54);
        yield return this.CreateMonsterSpawn(132, laserTrap, 190, 54);
        yield return this.CreateMonsterSpawn(133, laserTrap, 185, 54);
        yield return this.CreateMonsterSpawn(134, laserTrap, 182, 54);
        yield return this.CreateMonsterSpawn(135, laserTrap, 178, 56);
        yield return this.CreateMonsterSpawn(136, laserTrap, 176, 58);
        yield return this.CreateMonsterSpawn(137, laserTrap, 174, 59);
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        // no monsters to create
    }
}