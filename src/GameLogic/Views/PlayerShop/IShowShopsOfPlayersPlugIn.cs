// <copyright file="IShowShopsOfPlayersPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.PlayerShop
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface of a view whose implementation informs about available player shops in the current scope.
    /// </summary>
    public interface IShowShopsOfPlayersPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the shops of players.
        /// </summary>
        /// <param name="playersWithShop">The players with shop.</param>
        void ShowShopsOfPlayers(ICollection<Player> playersWithShop);
    }
}