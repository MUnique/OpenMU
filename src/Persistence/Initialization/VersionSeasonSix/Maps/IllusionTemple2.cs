// <copyright file="IllusionTemple2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Initialization for the Illusion Temple 2.
/// </summary>
internal class IllusionTemple2 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 46;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Illusion Temple 2";

    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTemple2"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public IllusionTemple2(IContext context, GameConfiguration gameConfiguration)
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

        // Roaming "Illusion Sorc. Spirit" monsters (389-391) - killing them grants skill points for the
        // event's special skills.
        for (var i = 0; i < SorcererSpiritPositions.Length; i++)
        {
            var (x, y) = SorcererSpiritPositions[i];
            yield return this.CreateMonsterSpawn((short)(120 + i), this.NpcDictionary[(short)(389 + (i % 3))], x, y, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
        }
    }

    private static readonly (byte X, byte Y)[] SorcererSpiritPositions =
    {
        (131, 93), (131, 89), (131, 85),
        (168, 123), (164, 123), (160, 123),
        (169, 48), (169, 52), (169, 56),
        (206, 85), (206, 81), (206, 77),
        (158, 85), (168, 94), (169, 75),
        (179, 85), (157, 66), (162, 66),
        (150, 78), (150, 73), (176, 103),
        (181, 103), (187, 98), (187, 92),
        (197, 57), (141, 113), (193, 61),
        (145, 109), (189, 65), (149, 105),
        (167, 87), (171, 83),
    };

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        // no monsters here
    }
}