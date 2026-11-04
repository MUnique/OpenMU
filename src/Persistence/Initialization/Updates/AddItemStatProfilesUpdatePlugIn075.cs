// <copyright file="AddItemStatProfilesUpdatePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the item stat profiles to version 0.75 configurations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddItemStatProfilesUpdatePlugIn_Name), Description = nameof(PlugInResources.AddItemStatProfilesUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3275A3CF-5B41-4B8D-97C8-0100AA1057AB")]
public class AddItemStatProfilesUpdatePlugIn075 : AddItemStatProfilesUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}
