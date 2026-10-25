// <copyright file="AddItemStatProfilesUpdatePlugIn095D.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the item stat profiles to version 0.95d configurations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddItemStatProfilesUpdatePlugIn_Name), Description = nameof(PlugInResources.AddItemStatProfilesUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("E8E872DF-96F7-4FE4-B8AE-B7D362E8D66B")]
public class AddItemStatProfilesUpdatePlugIn095D : AddItemStatProfilesUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}
