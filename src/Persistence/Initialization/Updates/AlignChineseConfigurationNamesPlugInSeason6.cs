// <copyright file="AlignChineseConfigurationNamesPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Aligns Chinese configuration names for Season6 configurations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AlignChineseConfigurationNames_Name), Description = nameof(PlugInResources.AlignChineseConfigurationNames_Description), ResourceType = typeof(PlugInResources))]
[Guid("77967be7-1f43-4162-8faf-6a2cb83de0fa")]
public class AlignChineseConfigurationNamesPlugInSeason6 : AlignChineseConfigurationNamesPlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AlignChineseConfigurationNamesSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
