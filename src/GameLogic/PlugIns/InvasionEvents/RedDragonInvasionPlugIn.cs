// <copyright file="RedDragonInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Enables the Red Dragon Invasion feature.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.RedDragonInvasionPlugIn_Name), Description = nameof(PlugInResources.RedDragonInvasionPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("548A76CC-242C-441C-BC9D-6C22745A2D72")]
public sealed class RedDragonInvasionPlugIn : SimpleInvasionPlugIn
{
    private static readonly IReadOnlyList<ushort> DisplayMaps =
    [
        InvasionMaps.Lorencia,
        InvasionMaps.Noria,
        InvasionMaps.Devias,
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="RedDragonInvasionPlugIn"/> class.
    /// </summary>
    public RedDragonInvasionPlugIn()
        : base(MapEventType.RedDragonInvasion, DisplayMaps, () => InvasionConfigurationDefaults.RedDragon)
    {
    }
}