// <copyright file="FieldMonsterSpawnsUpdatePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Brings the automatic monster spawns of the field maps of Season 6 to the Season 6 data.
/// </summary>
/// <remarks>
/// In addition to the changes of all versions (see <see cref="FieldMonsterSpawnsUpdatePlugInBase"/>), Noria
/// gets the spiders, Elvenland gets the layout of smaller areas and single spots as well, and the field maps
/// which only Season 6 has get their corrections.
/// The update is optional, because it overwrites the spawns of the field monsters of Devias, Noria and Elvenland,
/// which a server owner may have customized. Spawns of other monsters, NPCs and spawns with another trigger are kept.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FieldMonsterSpawnsUpdatePlugInSeason6_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FieldMonsterSpawnsUpdatePlugInSeason6_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("5B20A974-D2AA-4B49-ADF6-2B5F97D15828")]
public class FieldMonsterSpawnsUpdatePlugInSeason6 : FieldMonsterSpawnsUpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string SeasonSixPlugInName = "Season 6 monster spawns of the field maps";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string SeasonSixPlugInDescription = "Brings the monster spawns of the field maps to the Season 6 data: Devias, Noria and Elvenland get the layout of smaller areas and single spots instead of quarters of the map, with the spiders of Noria and Elvenland. Missing spawns of Dungeon, Lost Tower, Icarus, Kalima 3, 6 and 7, Land of Trials, Aida, Crywolf Fortress, Kanturu Relics and Barracks of Balgass are added and surplus ones removed, spawns with wrong coordinates in Crywolf Fortress, Barracks of Balgass, Swamp of Calmness, Vulcanus and Karutan corrected. Also removes the duplicate monster spawns of Dungeon (254) and Lost Tower (2). Customized spawns of the field monsters of Devias, Noria and Elvenland are overwritten.";

    /// <inheritdoc />
    public override string Name => SeasonSixPlugInName;

    /// <inheritdoc />
    public override string Description => SeasonSixPlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    internal override IReadOnlyList<FieldMonsterSpawnCorrection> SpawnCorrections { get; } =
    [
        .. CommonSpawnCorrections,
        IcarusSpawnCorrection,
        new(
            Kalima3.Number,
            [
                new(159, 188, 35, 35, 87, 87, 1), // Schriker 3
            ],
            []),
        new(
            Kalima6.Number,
            [
                new(161, 274, 32, 32, 77, 77, 1), // Schriker 6
                new(162, 274, 41, 41, 78, 78, 1), // Schriker 6
                new(163, 274, 37, 37, 67, 67, 1), // Schriker 6
                new(164, 274, 46, 46, 86, 86, 1), // Schriker 6
                new(165, 274, 23, 23, 85, 85, 1), // Schriker 6
                new(166, 274, 23, 23, 70, 70, 1), // Schriker 6
            ],
            []),
        new(
            LandOfTrials.Number,
            [
                new(601, 293, 192, 192, 127, 127, 1), // Poison Golem
                new(602, 293, 180, 180, 117, 117, 1), // Poison Golem
                new(603, 293, 191, 191, 112, 112, 1), // Poison Golem
            ],
            []),
        new(
            Aida.Number,
            [
                new(919, 305, 94, 94, 158, 158, 1), // Blue Golem
                new(920, 306, 126, 126, 156, 156, 1), // Death Rider
                new(921, 550, 104, 104, 203, 203, 1), // Bloody Death Rider
                new(922, 550, 99, 99, 203, 203, 1), // Bloody Death Rider
                new(923, 550, 75, 75, 228, 228, 1), // Bloody Death Rider
            ],
            []),
        new(
            CrywolfFortress.Number,
            [
                new(408, 313, 88, 88, 144, 144, 1), // Werewolf
                new(816, 313, 204, 204, 130, 130, 1), // Werewolf
            ],
            []),
        new(
            Kalima7.Number,
            [
                new(161, 337, 32, 32, 77, 77, 1), // Schriker 7
                new(162, 337, 41, 41, 78, 78, 1), // Schriker 7
                new(163, 337, 37, 37, 67, 67, 1), // Schriker 7
                new(164, 337, 46, 46, 86, 86, 1), // Schriker 7
                new(165, 337, 23, 23, 85, 85, 1), // Schriker 7
                new(166, 337, 23, 23, 70, 70, 1), // Schriker 7
            ],
            []),
        new(
            KanturuRelics.Number,
            [
                new(612, 106, 141, 141, 155, 155, 1), // Laser Trap
            ],
            []),
        new(
            BarracksOfBalgass.Number,
            [
                new(301, 411, 126, 126, 164, 164, 1), // Soram Trainee Soldier
                new(309, 409, 71, 71, 101, 101, 1), // Balram Trainee Soldier
            ],
            [307, 308]),
        new(
            SwampOfCalmness.Number,
            [
                new(414, 443, 39, 39, 20, 20, 1), // Sapi Tres
            ],
            []),
        new(
            Vulcanus.Number,
            [
                new(607, 490, 174, 174, 68, 68, 1), // Ruthless Lava Giant
            ],
            []),
        new(
            Karutan1.Number,
            [
                new(146, 570, 204, 204, 104, 104, 1), // Bone Scorpion
                new(209, 573, 57, 57, 178, 178, 1), // Crypta
            ],
            []),
        new(
            Karutan2.Number,
            [
                new(408, 576, 190, 190, 38, 38, 1), // Narcondra
            ],
            []),
    ];

    /// <inheritdoc />
    internal override IReadOnlyList<FieldMonsterSpawn> NoriaMonsterSpawnLayout => Noria.MonsterSpawnLayout;

    /// <inheritdoc />
    protected override void ApplyVersionSpecificChanges(IContext context, GameConfiguration gameConfiguration)
    {
        ApplyLayout(context, gameConfiguration, Elvenland.Number, Elvenland.MonsterSpawnLayout);
    }
}
