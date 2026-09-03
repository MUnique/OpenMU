// <copyright file="CastleSiegeLifeStoneIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Intelligence;

using MUnique.OpenMU.GameLogic.CastleSiege.NPC;

/// <summary>
/// Drives the creation and healing phases of a Castle Siege Life Stone.
/// </summary>
public sealed class CastleSiegeLifeStoneIntelligence : CastleSiegeNpcIntelligenceBase
{
    /// <summary>
    /// Executes one Castle Siege task tick for the assigned Life Stone.
    /// </summary>
    /// <param name="utcNow">The current UTC time.</param>
    /// <returns>A task that represents the update operation.</returns>
    public ValueTask TickAsync(DateTime utcNow)
    {
        return this.Npc is CastleSiegeLifeStone lifeStone
            ? lifeStone.TickCoreAsync(utcNow)
            : ValueTask.CompletedTask;
    }
}
