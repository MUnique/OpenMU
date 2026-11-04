// <copyright file="INpcDialogClosedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Interface of a view whose implementation informs about the closed crafting dialog.
/// </summary>
public interface INpcDialogClosedPlugIn : IViewPlugIn
{
    /// <summary>
    /// Informs the view about the closed dialog.
    /// </summary>
    /// <param name="npc">The definition of the closed NPC.</param>
    ValueTask DialogClosedAsync(MonsterDefinition npc);
}