// <copyright file="FixJeweleryPetsDamageCalcsPlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes jewelery items and pet options related to damage.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("BF56A0E2-D7B3-4456-81F7-249440489607")]
public class FixJeweleryPetsDamageCalcsPlugIn095D : FixJeweleryPetsDamageCalcsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixJeweleryPetsDamageCalcs095d;
}