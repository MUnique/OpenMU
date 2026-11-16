// <copyright file="FieldMonsterSpawnsUpdatePlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Brings the automatic monster spawns of the field maps of version 0.95d to the Season 6 data.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FieldMonsterSpawnsUpdatePlugInBase_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FieldMonsterSpawnsUpdatePlugInBase_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("F849A946-B5D7-4590-B398-07EDFCBE891F")]
public class FieldMonsterSpawnsUpdatePlugIn095D : FieldMonsterSpawnsUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    internal override IReadOnlyList<FieldMonsterSpawnCorrection> SpawnCorrections { get; } = [.. CommonSpawnCorrections, IcarusSpawnCorrection];
}
