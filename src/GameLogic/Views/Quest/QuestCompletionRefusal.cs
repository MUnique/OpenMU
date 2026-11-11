// <copyright file="QuestCompletionRefusal.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Quest;

/// <summary>
/// The reason why the completion of a quest was refused, although its requirements are met.
/// </summary>
public enum QuestCompletionRefusal
{
    /// <summary>
    /// The item rewards don't fit into the inventory.
    /// </summary>
    InventoryFull,

    /// <summary>
    /// The money rewards would exceed the money limit of the inventory.
    /// </summary>
    ZenLimitReached,
}
