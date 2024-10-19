// <copyright file="FixWingsDmgRatesUpdatePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes the wings damage absorption and increase bonus level tables values for a <see cref="CombinedElement"/> (sum) calculation, instead of a compound calculation
/// for version 075.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("3821267A-9C37-40E5-B023-BAB1A8E4DAB7")]
public class FixWingsDmgRatesUpdatePlugIn075 : FixWingsDmgRatesUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixWingsDmgRatesPlugIn075;
}