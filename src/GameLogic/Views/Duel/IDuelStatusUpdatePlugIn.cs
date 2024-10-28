// <copyright file="IDuelStatusUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about the status of the available duel rooms.
/// </summary>
public interface IDuelStatusUpdatePlugIn : IViewPlugIn
{

    /// <summary>
    /// SHows the status of the duel rooms.
    /// </summary>
    /// <param name="duelRooms">The duel rooms.</param>
    ValueTask UpdateStatusAsync(DuelRoom?[] duelRooms);
}