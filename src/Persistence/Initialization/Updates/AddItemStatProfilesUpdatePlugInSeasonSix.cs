// <copyright file="AddItemStatProfilesUpdatePlugInSeasonSix.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the item stat profiles to season 6 configurations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddItemStatProfilesUpdatePlugIn_Name), Description = nameof(PlugInResources.AddItemStatProfilesUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("0180CA47-5B5D-442F-B7B1-F30440AC8C87")]
public class AddItemStatProfilesUpdatePlugInSeasonSix : AddItemStatProfilesUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
