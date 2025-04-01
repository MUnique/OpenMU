// <copyright file="FixCharStatsForceWavePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes DW agility to defense multiplier stat.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("D7CD05B7-06EE-4D9F-BAD0-65267F3A9FE8")]
public class FixCharStatsForceWavePlugIn075 : FixCharStatsForceWavePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixCharStatsForceWave075;
}