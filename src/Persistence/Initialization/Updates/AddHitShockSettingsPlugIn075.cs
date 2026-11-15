// <copyright file="AddHitShockSettingsPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds hit-shock settings.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddHitShockSettingsPlugIn_Name), Description = nameof(PlugInResources.AddHitShockSettingsPlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("CC6D707F-ED95-4524-B907-F5A3F955561D")]
public class AddHitShockSettingsPlugIn075 : AddHitShockSettingsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}
