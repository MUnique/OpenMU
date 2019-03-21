// <copyright file="IShowLoginResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Login
{
    /// <summary>
    /// Interface of a view whose implementation informs about the result of a login request.
    /// </summary>
    public interface IShowLoginResultPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the login result.
        /// </summary>
        /// <param name="loginResult">The login result.</param>
        void ShowLoginResult(LoginResult loginResult);
    }
}