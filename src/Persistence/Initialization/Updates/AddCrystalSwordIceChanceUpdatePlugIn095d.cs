// <copyright file="AddCrystalSwordIceChanceUpdatePlugIn095d.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the Crystal Sword ice chance to existing version 0.95d configurations.
/// </summary>
[PlugIn]
[Display(Name = "Crystal Sword Ice Chance", Description = "Adds the Crystal Sword ice proc to existing configurations.")]
[Guid("D70AF6D6-A5D1-41B0-B922-A2AE8F6D3125")]
public class AddCrystalSwordIceChanceUpdatePlugIn095D : AddCrystalSwordIceChanceUpdatePlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => Version095d.DataInitialization.Id;
}
