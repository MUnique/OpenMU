// <copyright file="CastleSiegeStatueIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Intelligence;

using System.Threading;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;

/// <summary>
/// Regenerates the health of an upgraded Guardian Statue.
/// </summary>
public sealed class CastleSiegeStatueIntelligence : CastleSiegeNpcIntelligenceBase, IDisposable
{
    private static readonly TimeSpan RegenerationInterval = TimeSpan.FromSeconds(5);
    private Timer? _timer;

    /// <inheritdoc />
    public override void Start()
    {
        this._timer ??= new Timer(_ => this.Regenerate(), null, RegenerationInterval, RegenerationInterval);
    }

    /// <inheritdoc />
    public override void Pause()
    {
        this._timer?.Dispose();
        this._timer = null;
    }

    /// <summary>
    /// Executes one regeneration tick.
    /// </summary>
    /// <returns>The restored hit points.</returns>
    public int Regenerate()
    {
        return this.Npc is CastleSiegeStatue statue
            ? statue.Regenerate()
            : 0;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Pause();
    }
}
