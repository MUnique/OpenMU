// <copyright file="RedDragonInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables Red Dragon Invasion feature.
/// </summary>
[PlugIn(nameof(RedDragonInvasionPlugIn), "Handle red dragon invasion event")]
[Guid("548A76CC-242C-441C-BC9D-6C22745A2D72")]
public class RedDragonInvasionPlugIn : BaseInvasionPlugIn<PeriodicInvasionConfiguration>, ISupportDefaultCustomConfiguration
{
    private const ushort RedDragonId = 44;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedDragonInvasionPlugIn"/> class.
    /// </summary>
    public RedDragonInvasionPlugIn()
        : base(MapEventType.RedDragonInvasion, null, new[] { (RedDragonId, (ushort)5) })
    {
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => PeriodicInvasionConfiguration.DefaultRedDragonInvasion;
}