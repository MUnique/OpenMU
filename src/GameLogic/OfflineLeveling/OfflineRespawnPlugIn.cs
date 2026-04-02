// <copyright file="OfflineRespawnPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.GameLogic.Views.World;
using System.Threading.Tasks;

/// <summary>
/// Simulates a respawn response for an offline leveling character.
/// </summary>
internal sealed class OfflineRespawnPlugIn : IRespawnAfterDeathPlugIn
{
    /// <inheritdoc/>
    public ValueTask RespawnAsync() => ValueTask.CompletedTask;
}
