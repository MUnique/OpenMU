// <copyright file="SystemConfigurationAddedPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This updates adds the new <see cref="SystemConfiguration"/> with default settings
/// for Version 0.75.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("F1151FDE-14F7-4945-AEE9-57DAB6449CFF")]
public class SystemConfigurationAddedPlugIn075 : SystemConfigurationAddedPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.SystemConfigurationAdded075;
}