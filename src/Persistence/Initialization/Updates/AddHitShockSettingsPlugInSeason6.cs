// <copyright file="AddHitShockSettingsPlugInSeason6.cs" company="MUnique">
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
[Guid("59DECC97-60A3-4130-B0DB-1858F8159D6A")]
public class AddHitShockSettingsPlugInSeason6 : AddHitShockSettingsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
