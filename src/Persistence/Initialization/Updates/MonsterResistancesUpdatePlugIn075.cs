// <copyright file="MonsterResistancesUpdatePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Converts the elemental resistances of the monsters of version 0.75 to whole numbers and corrects them.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.MonsterResistancesUpdatePlugInBase_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.MonsterResistancesUpdatePlugInBase_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("C634A06F-1FCE-4FFB-B185-8A6540A709E8")]
public class MonsterResistancesUpdatePlugIn075 : MonsterResistancesUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}
