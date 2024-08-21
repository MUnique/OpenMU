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
    /// The castle door health per castle level.
    /// </summary>
    public static readonly int[] CastleDoorHealthPerLevel = { 0, 150000, 205000, 260000, 325000, 400000, 480000, 565000, 650000 };

    /// <summary>
    /// The crystal statue health per castle level.
    /// </summary>
    public static readonly int[] CrystalStatueHealthPerLevel = { 0, 65000, 105000, 145000, 185000, 225000, 265000, 305000, 345000 };

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
    protected override byte SafezoneMapNumber => Devias.Number;

    /// <inheritdoc />
    protected override string MapName => $"Blood Castle {this.CastleLevel}";

    /// <summary>
    /// Gets the castle level.
    /// </summary>
    protected abstract int CastleLevel { get; }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        var castleDoor = this.CreateMonsterSpawn(1, this.NpcDictionary[131], 014, 075, Direction.SouthWest, SpawnTrigger.OnceAtEventStart); // Castle Gate
        castleDoor.MaximumHealthOverride = CastleDoorHealthPerLevel[this.CastleLevel];
        yield return castleDoor;
        yield return this.CreateMonsterSpawn(2, this.NpcDictionary[232], 010, 009, Direction.SouthWest, SpawnTrigger.Automatic); // Archangel
    }
}