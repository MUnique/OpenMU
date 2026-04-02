// <copyright file="GoldenInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables Golden Invasion feature.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GoldenInvasionPlugIn_Name), Description = nameof(PlugInResources.GoldenInvasionPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("06D18A9E-2919-4C17-9DBC-6E4F7756495C")]
public class GoldenInvasionPlugIn : BaseInvasionPlugIn<PeriodicInvasionConfiguration>, ISupportDefaultCustomConfiguration
{
    private const ushort LorenciaId = 0;
    private const ushort DeviasId = 2;
    private const ushort NoriaId = 3;

    private static readonly IReadOnlyList<ushort> DisplayMaps = new ushort[] { LorenciaId, NoriaId, DeviasId };

    /// <summary>
    /// Initializes a new instance of the <see cref="GoldenInvasionPlugIn"/> class.
    /// </summary>
    public GoldenInvasionPlugIn()
        : base(MapEventType.GoldenDragonInvasion)
    {
    }

    /// <inheritdoc />
    protected override IReadOnlyList<ushort> EventDisplayMapIds => DisplayMaps;

    /// <inheritdoc />
    public object CreateDefaultConfig() => PeriodicInvasionConfiguration.DefaultGoldenInvasion;
}