// <copyright file="NpcSpawnsUpdatePlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Brings the NPC spawns of 0.95d to the Season 6 data, like <see cref="NpcSpawnsUpdatePlugIn075"/>,
/// and moves the Golden Archer from Lorencia to its Season 6 position in Devias.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.NpcSpawnsUpdatePlugIn095d_Name), Description = nameof(PlugInResources.NpcSpawnsUpdatePlugIn095d_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("B5403DEE-E1F6-46F2-A32A-4327EA7160FF")]
public class NpcSpawnsUpdatePlugIn095D : NpcSpawnsUpdatePlugInBase
{
    /// <inheritdoc />
    public override string Name => PlugInResources.NpcSpawnsUpdatePlugIn095d_Name;

    /// <inheritdoc />
    public override string Description => PlugInResources.NpcSpawnsUpdatePlugIn095d_Description;

    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 18, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    internal override IReadOnlyList<(byte MapNumber, short SpawnNumber, short NpcNumber)> RemovedSpawns { get; } =
    [
        .. NpcSpawnsUpdatePlugIn075.Removed075Spawns,
        (Lorencia.Number, 15, 236), // Golden Archer, which stands in Devias
    ];

    /// <inheritdoc />
    internal override IReadOnlyList<NpcSpawnCorrection> Corrections { get; } =
    [
        .. NpcSpawnsUpdatePlugIn075.Corrections075,
        new(Devias.Number, 44, 236, 226, 66, Direction.SouthWest, true), // Golden Archer
    ];
}
