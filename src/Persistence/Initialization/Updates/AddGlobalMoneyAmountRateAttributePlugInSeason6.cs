// <copyright file="AddGlobalMoneyAmountRateAttributePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update moves the MoneyAmountRate attribute to global base attributes for Season 6.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("C3D4E5F6-A7B8-9012-CDEF-123456789012")]
public class AddGlobalMoneyAmountRateAttributePlugInSeason6 : AddGlobalMoneyAmountRateAttributePlugInBase
{
    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddGlobalMoneyAmountRateAttributeSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
