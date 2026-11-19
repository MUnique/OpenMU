// <copyright file="MonsterResistancesUpdatePlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Converts the elemental resistances of the monsters of version 0.95d to whole numbers and corrects them.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.MonsterResistancesUpdatePlugInBase_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.MonsterResistancesUpdatePlugInBase_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("C55E852E-6996-4B29-ABFF-F2D0B144BF3F")]
public class MonsterResistancesUpdatePlugIn095D : MonsterResistancesUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}
