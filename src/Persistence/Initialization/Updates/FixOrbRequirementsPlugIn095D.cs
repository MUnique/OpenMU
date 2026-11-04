// <copyright file="FixOrbRequirementsPlugIn095D.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Fixes the energy requirements of the orbs of version 0.95d.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixOrbRequirementsPlugInBase_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixOrbRequirementsPlugInBase_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("2E9431FA-AE99-4907-B657-DCAB4310E9B9")]
public class FixOrbRequirementsPlugIn095D : FixOrbRequirementsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    private protected override IReadOnlyList<(byte Group, byte Number, int Energy, int Strength, int Agility)> Corrections => FairyElfOrbs;
}
