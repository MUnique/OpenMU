// <copyright file="AddHitShockSettingsPlugIn095d.cs" company="MUnique">
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
[Guid("528462F0-E79C-4D89-94FC-A087BBA70E6F")]
public class AddHitShockSettingsPlugIn095D : AddHitShockSettingsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}
