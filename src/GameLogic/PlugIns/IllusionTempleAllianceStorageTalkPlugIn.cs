// <copyright file="IllusionTempleAllianceStorageTalkPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles talking to the Alliance Item Storage in the illusion temple event, to which the allied
/// forces carry the sacred relic to score a point.
/// </summary>
[Guid("7F4C09B5-28EA-4D63-8C1F-6B0A95E3D472")]
[PlugIn]
[Display(Name = nameof(PlugInResources.IllusionTempleAllianceStorageTalkPlugIn_Name), Description = nameof(PlugInResources.IllusionTempleAllianceStorageTalkPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class IllusionTempleAllianceStorageTalkPlugIn : IllusionTempleTeamStorageTalkPlugInBase
{
    /// <summary>
    /// The number of the Alliance Item Storage NPC.
    /// </summary>
    internal const short AllianceItemStorageNumber = 383;

    /// <inheritdoc />
    public override short DefaultNpcNumber => AllianceItemStorageNumber;
}
