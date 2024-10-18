// <copyright file="FixWingsDmgRatesUpdatePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes the wings damage absorption and increase bonus level tables values for a <see cref="CombinedElement"/> (sum) calculation, instead of a compound calculation
/// for season 6.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("03F49890-CB0E-40B7-A590-174BBA1962F4")]
public class FixWingsDmgRatesUpdatePlugInSeason6 : FixWingsDmgRatesUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixWingsDmgRatesPlugInSeason6;
}