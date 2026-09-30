// <copyright file="AlignChineseConfigurationNamesPlugIn095D.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Aligns Chinese configuration names for 095d configurations.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AlignChineseConfigurationNames_Name), Description = nameof(PlugInResources.AlignChineseConfigurationNames_Description), ResourceType = typeof(PlugInResources))]
[Guid("3871520b-cde6-4dee-aae5-bf15190e6816")]
public class AlignChineseConfigurationNamesPlugIn095D : AlignChineseConfigurationNamesPlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AlignChineseConfigurationNames095d;

    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}
