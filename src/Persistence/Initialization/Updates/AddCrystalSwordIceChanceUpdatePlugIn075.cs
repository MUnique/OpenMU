// <copyright file="AddCrystalSwordIceChanceUpdatePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the Crystal Sword ice chance to existing version 0.75 configurations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddCrystalSwordIceChanceUpdatePlugIn_Name), Description = nameof(PlugInResources.AddCrystalSwordIceChanceUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("68982D35-C7B2-4A34-88BD-047905E3F8E2")]
public class AddCrystalSwordIceChanceUpdatePlugIn075 : AddCrystalSwordIceChanceUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}
