// <copyright file="IllusionTempleIllusionStorageTalkPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles talking to the Illusion Item Storage in the illusion temple event, to which the illusion
/// forces carry the sacred relic to score a point.
/// </summary>
[Guid("C62B8D14-9F30-4A75-B5E8-1D47A0C396FB")]
[PlugIn]
[Display(Name = nameof(PlugInResources.IllusionTempleIllusionStorageTalkPlugIn_Name), Description = nameof(PlugInResources.IllusionTempleIllusionStorageTalkPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class IllusionTempleIllusionStorageTalkPlugIn : IllusionTempleTeamStorageTalkPlugInBase
{
    /// <summary>
    /// The number of the Illusion Item Storage NPC.
    /// </summary>
    internal const short IllusionItemStorageNumber = 384;

    /// <inheritdoc />
    public override short DefaultNpcNumber => IllusionItemStorageNumber;
}
