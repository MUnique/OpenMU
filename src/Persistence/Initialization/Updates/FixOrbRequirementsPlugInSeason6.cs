// <copyright file="FixOrbRequirementsPlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Fixes the energy, strength and agility requirements of the orbs, crystals and scrolls of season 6.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixOrbRequirementsPlugInBase_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixOrbRequirementsPlugInBase_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("A6447A99-5811-4E52-8E1F-2B82FF1EA4B8")]
public class FixOrbRequirementsPlugInSeason6 : FixOrbRequirementsPlugInBase
{
    private static readonly IReadOnlyList<(byte Number, int Energy, int Strength, int Agility)> Season6Corrections =
    [
        .. FairyElfOrbs,
        (16, 0, 596, 0), // Orb of Fire Slash
        (18, 0, 0, 646), // Orb of Ice Arrow
        (21, 79, 0, 0), // Scroll of Fire Burst
        (22, 153, 0, 0), // Scroll of Summon
        (23, 102, 0, 0), // Scroll of Critical Damage
        (24, 126, 0, 0), // Scroll of Electric Spark
        (35, 150, 0, 0), // Scroll of Fire Scream
        (46, 168, 0, 0), // Crystal of Recovery
        (48, 84, 0, 0), // Scroll of Chaotic Diseier
    ];

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    private protected override IReadOnlyList<(byte Number, int Energy, int Strength, int Agility)> Corrections => Season6Corrections;
}
