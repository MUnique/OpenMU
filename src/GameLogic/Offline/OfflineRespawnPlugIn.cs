// <copyright file="OfflineRespawnPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using System.Threading.Tasks;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Simulates a respawn response for an offline character.
/// </summary>
internal sealed class OfflineRespawnPlugIn : IRespawnAfterDeathPlugIn
{
    /// <inheritdoc/>
    public ValueTask RespawnAsync() => ValueTask.CompletedTask;
}
