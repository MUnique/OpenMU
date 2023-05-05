// <copyright file="SystemConfigurationAddedPlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This updates adds the new <see cref="SystemConfiguration"/> with default settings
/// for version 0.95d.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("EC9FE71E-5C6C-456A-AC75-428EBA3FF626")]
public class SystemConfigurationAddedPlugIn095d : SystemConfigurationAddedPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.SystemConfigurationAdded095d;
}