// <copyright file="ITeleportPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view whose implementation informs about a teleport action of the current player.
/// </summary>
public interface ITeleportPlugIn : IViewPlugIn
{
    /// <summary>
    /// Will be called when the player teleported or failed to teleport to another location of the same map.
    /// </summary>
    ValueTask ShowTeleportedAsync();
}