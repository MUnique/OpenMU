// <copyright file="IBotWingsEarnedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugins which are called when a server-side bot put on the new wings it earned at a level
/// milestone (see <see cref="BotWingHandler"/>).
/// </summary>
[Guid("A1E5B0F4-E58D-4D07-A01E-3F2AF82267C0")]
[PlugInPoint("Bot earned wings", "Plugins which are called when a server-side bot put on the new wings it earned at a level milestone.")]
public interface IBotWingsEarnedPlugIn
{
    /// <summary>
    /// Is called when the bot put on its new wings.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="wings">The new wings, already in the wing slot.</param>
    void BotEarnedWings(Player bot, Item wings);
}
