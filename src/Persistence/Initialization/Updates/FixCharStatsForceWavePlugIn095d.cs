// <copyright file="FixCharStatsForceWavePlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes agility to defense multiplier (DW) and base energy (MG) stats.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("14DFF317-B4E6-424A-A8D1-6D1D5195E970")]
public class FixCharStatsForceWavePlugIn095D : FixCharStatsForceWavePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal new const string PlugInName = "Fix DW and MG Char Stats";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal new const string PlugInDescription = "This update fixes agility to defense multiplier (DW) and base energy (MG) stats.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixCharStatsForceWave095d;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
        this.UpdateMagicGladiatorClassesStats(gameConfiguration);
    }
}