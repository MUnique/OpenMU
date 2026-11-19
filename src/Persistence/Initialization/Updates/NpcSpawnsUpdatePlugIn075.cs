// <copyright file="NpcSpawnsUpdatePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Version075.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Brings the NPC spawns of 0.75 to the Season 6 data: removes the two Potion Girls of Noria, the Potion Girl and
/// the Safety Guardian of Lost Tower, the Safety Guardians of Arena and Atlans and the Crossbow Guard at 114/125 in
/// Lorencia, and moves the Soccer Ball and the Arena Guard of Arena to their Season 6 positions.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.NpcSpawnsUpdatePlugIn075_Name), Description = nameof(PlugInResources.NpcSpawnsUpdatePlugIn075_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("CF786704-BF4E-4995-8252-78C0B3729638")]
public class NpcSpawnsUpdatePlugIn075 : NpcSpawnsUpdatePlugInBase
{
    /// <summary>
    /// The NPC spawns which 0.75 doesn't have.
    /// </summary>
    internal static readonly IReadOnlyList<(byte MapNumber, short SpawnNumber, short NpcNumber)> Removed075Spawns =
    [
        (Lorencia.Number, 9, 247), // Crossbow Guard
        (Noria.Number, 1, 253), // Potion Girl
        (Noria.Number, 2, 253), // Potion Girl
        (LostTower.Number, 1, 253), // Potion Girl
        (LostTower.Number, 2, 240), // Safety Guardian
        (Arena.Number, 1, 240), // Safety Guardian
        (Atlans.Number, 1, 240), // Safety Guardian
    ];

    /// <summary>
    /// The NPC spawns of 0.75 which get the Season 6 position and direction.
    /// </summary>
    internal static readonly IReadOnlyList<NpcSpawnCorrection> Corrections075 =
    [
        new(Arena.Number, 2, 200, 63, 160, Direction.SouthWest), // Soccer Ball
        new(Arena.Number, 3, 239, 72, 164, Direction.SouthWest), // Arena Guard
    ];

    /// <inheritdoc />
    public override string Name => PlugInResources.NpcSpawnsUpdatePlugIn075_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.NpcSpawnsUpdatePlugIn075_Description;

    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 18, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    internal override IReadOnlyList<(byte MapNumber, short SpawnNumber, short NpcNumber)> RemovedSpawns => Removed075Spawns;

    /// <inheritdoc />
    internal override IReadOnlyList<NpcSpawnCorrection> Corrections => Corrections075;
}
