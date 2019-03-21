// <copyright file="ILogoutPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Login
{
    /// <summary>
    /// Interface of a view whose implementation informs about the log out from the game.
    /// </summary>
    public interface ILogoutPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Logouts with the specified logout type.
        /// </summary>
        /// <param name="logoutType">Type of the logout.</param>
        void Logout(LogoutType logoutType);
    }
}