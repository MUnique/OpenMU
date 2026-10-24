// <copyright file="AddCrystalSwordIceChanceUpdatePlugInSeasonSix.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the Crystal Sword ice chance to existing Season 6 configurations.
/// </summary>
[PlugIn]
[Display(Name = "Crystal Sword Ice Chance", Description = "Adds the Crystal Sword ice proc to existing configurations.")]
[Guid("85AD7C42-7BAE-42A0-8CB1-0F3CC894DF05")]
public class AddCrystalSwordIceChanceUpdatePlugInSeasonSix : AddCrystalSwordIceChanceUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;
}
