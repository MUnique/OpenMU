// <copyright file="FixJeweleryPetsDamageCalcsPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes jewelery items and pet options related to damage.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("46F50226-B0A2-4FE7-B708-AEB3F306A7C0")]
public class FixJeweleryPetsDamageCalcsPlugIn075 : FixJeweleryPetsDamageCalcsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version075.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixJeweleryPetsDamageCalcs075;
}