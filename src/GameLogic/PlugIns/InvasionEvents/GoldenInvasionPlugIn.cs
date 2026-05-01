// <copyright file="GoldenInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Enables the Golden Invasion feature.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GoldenInvasionPlugIn_Name), Description = nameof(PlugInResources.GoldenInvasionPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("06D18A9E-2919-4C17-9DBC-6E4F7756495C")]
public sealed class GoldenInvasionPlugIn : SimpleInvasionPlugIn
{
    private static readonly IReadOnlyList<ushort> DisplayMaps =
    [
        InvasionMaps.Lorencia,
        InvasionMaps.Noria,
        InvasionMaps.Devias,
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="GoldenInvasionPlugIn"/> class.
    /// </summary>
    public GoldenInvasionPlugIn()
        : base(MapEventType.GoldenDragonInvasion, DisplayMaps, () => InvasionConfigurationDefaults.Golden)
    {
    }
}