// <copyright file="IOpenNpcWindowPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC
{
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Interface of a view whose implementation informs about an opened npc dialog window, which was requested by the player before.
    /// </summary>
    public interface IOpenNpcWindowPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Opens the Monster window.
        /// </summary>
        /// <param name="window">The window.</param>
        void OpenNpcWindow(NpcWindow window);
    }
}