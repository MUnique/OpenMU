// <copyright file="ICastleSiegeNpc.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.NPC;

/// <summary>
/// Common contract of a configured Castle Siege NPC.
/// </summary>
public interface ICastleSiegeNpc
{
    /// <summary>
    /// Gets the runtime entry which owns this NPC.
    /// </summary>
    CastleSiegeNpcRuntime Runtime { get; }
}
