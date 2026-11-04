// <copyright file="ILegacyQuestStateDialogPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Quest;

/// <summary>
/// Interface of a view whose implementation informs about quest state dialog, after clicking on the quest npc.
/// </summary>
/// <remarks>
/// Sends C1A3.
/// </remarks>
public interface ILegacyQuestStateDialogPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the quest state of the character.
    /// </summary>
    ValueTask ShowAsync();
}