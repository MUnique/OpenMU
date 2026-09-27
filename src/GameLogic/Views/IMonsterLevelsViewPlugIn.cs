// <copyright file="IMonsterLevelsViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose client shows the level of the monsters, as the server configures them.
/// </summary>
public interface IMonsterLevelsViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Sends the level of all monsters. The client keeps them for the rest of the session.
    /// </summary>
    ValueTask ShowMonsterLevelsAsync();
}
