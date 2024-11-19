// <copyright file="FixWeaponRisePercentagePlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes weapons (staff) rise percentage.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("33259706-F3DF-4F4D-9935-3DEF7E53BF81")]
public class FixWeaponRisePercentagePlugIn095D : FixWeaponRisePercentagePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixWeaponRisePercentage095d;
}