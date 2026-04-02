// <copyright file="RedDragonInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables Red Dragon Invasion feature.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.RedDragonInvasionPlugIn_Name), Description = nameof(PlugInResources.RedDragonInvasionPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("548A76CC-242C-441C-BC9D-6C22745A2D72")]
public class RedDragonInvasionPlugIn : BaseInvasionPlugIn<PeriodicInvasionConfiguration>, ISupportDefaultCustomConfiguration
{
    private const ushort LorenciaId = 0;
    private const ushort DeviasId = 2;
    private const ushort NoriaId = 3;

    private static readonly IReadOnlyList<ushort> DisplayMaps = new ushort[] { LorenciaId, NoriaId, DeviasId };

    /// <summary>
    /// Initializes a new instance of the <see cref="RedDragonInvasionPlugIn"/> class.
    /// </summary>
    public RedDragonInvasionPlugIn()
        : base(MapEventType.RedDragonInvasion)
    {
    }

    /// <inheritdoc />
    protected override IReadOnlyList<ushort> EventDisplayMapIds => DisplayMaps;

    /// <inheritdoc />
    public object CreateDefaultConfig() => PeriodicInvasionConfiguration.DefaultRedDragonInvasion;
}