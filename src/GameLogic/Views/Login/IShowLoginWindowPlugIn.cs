// <copyright file="IShowLoginWindowPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Login
{
    /// <summary>
    /// Interface of a view whose implementation informs about the available login dialog.
    /// </summary>
    public interface IShowLoginWindowPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the login window.
        /// </summary>
        void ShowLoginWindow();
    }
}