// <copyright file="FixWeaponRisePercentagePlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes weapons (staff) rise percentage.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("5B63534D-E5DF-46B1-992D-C1637B197EE1")]
public class FixWeaponRisePercentagePlugIn075 : FixWeaponRisePercentagePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixWeaponRisePercentage075;
}