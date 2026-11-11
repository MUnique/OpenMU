// <copyright file="FieldMonsterSpawnsUpdatePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Removes the duplicate monster spawns of Dungeon and Lost Tower of version 0.75.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FieldMonsterSpawnsUpdatePlugInBase_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FieldMonsterSpawnsUpdatePlugInBase_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("FA5F8867-A3E3-48AA-9453-9D756049AECE")]
public class FieldMonsterSpawnsUpdatePlugIn075 : FieldMonsterSpawnsUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}
