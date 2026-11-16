// <copyright file="FieldMonsterSpawnsUpdatePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Brings the automatic monster spawns of the field maps of version 0.75 to the Season 6 data.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FieldMonsterSpawnsUpdatePlugInBase_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FieldMonsterSpawnsUpdatePlugInBase_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("FA5F8867-A3E3-48AA-9453-9D756049AECE")]
public class FieldMonsterSpawnsUpdatePlugIn075 : FieldMonsterSpawnsUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}
