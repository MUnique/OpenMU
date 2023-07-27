// <copyright file="SystemConfigurationAddedPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This updates adds the new <see cref="SystemConfiguration"/> with default settings
/// for season 6.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("7231172F-51AD-4129-9003-C1ACC7E04147")]
public class SystemConfigurationAddedPlugInSeason6 : SystemConfigurationAddedPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.SystemConfigurationAddedSeason6;
}