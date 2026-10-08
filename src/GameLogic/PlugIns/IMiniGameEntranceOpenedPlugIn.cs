// <copyright file="IMiniGameEntranceOpenedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when the entrance of a mini game opened.
/// </summary>
/// <remarks>
/// Mini games with a shared map, like Blood Castle, open one game per level at the same time.
/// In this case, it's called once for each level.
/// </remarks>
[Guid("52AC710D-905B-497C-B820-1C9D022AAE3D")]
[PlugInPoint("Mini game entrance opened", "Plugins which will be executed when the entrance of a mini game opened.")]
public interface IMiniGameEntranceOpenedPlugIn
{
    /// <summary>
    /// Is called when the entrance of a mini game opened.
    /// </summary>
    /// <param name="miniGame">The mini game. Its <see cref="MiniGameContext.EnterEndsAtUtc"/> tells when the entrance closes.</param>
    ValueTask MiniGameEntranceOpenedAsync(MiniGameContext miniGame);
}
