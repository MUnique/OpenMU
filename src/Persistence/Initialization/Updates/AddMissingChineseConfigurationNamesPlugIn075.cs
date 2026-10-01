// <copyright file="AddMissingChineseConfigurationNamesPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds missing Chinese configuration names for 075 configurations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AddMissingChineseConfigurationNames_Name), Description = nameof(PlugInResources.AddMissingChineseConfigurationNames_Description), ResourceType = typeof(PlugInResources))]
[Guid("d4498a46-f81e-4eae-b7a9-fedbe6bf3589")]
public class AddMissingChineseConfigurationNamesPlugIn075 : AddMissingChineseConfigurationNamesPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;
}
