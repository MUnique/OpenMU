// <copyright file="NpcTalkPlugInExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// Extension methods for the <see cref="NpcTalkPlugInBase"/>.
/// </summary>
public static class NpcTalkPlugInExtensions
{
    /// <summary>
    /// Determines whether the NPC is the configured NPC of an active plugin of type <typeparamref name="TPlugIn"/>.
    /// </summary>
    /// <typeparam name="TPlugIn">The type of the plugin.</typeparam>
    /// <param name="npc">The NPC, e.g. the <see cref="Player.OpenedNpc"/>.</param>
    /// <param name="gameContext">The game context.</param>
    /// <returns><c>true</c>, if the NPC is the configured NPC of an active plugin of type <typeparamref name="TPlugIn"/>.</returns>
    public static bool IsNpcOf<TPlugIn>(this NonPlayerCharacter? npc, IGameContext gameContext)
        where TPlugIn : NpcTalkPlugInBase
    {
        return npc is not null
               && gameContext.PlugInManager.GetActivePlugInsOf<IPlayerTalkToNpcPlugIn>().OfType<TPlugIn>().Any(plugIn => plugIn.IsConfiguredNpc(npc));
    }
}
