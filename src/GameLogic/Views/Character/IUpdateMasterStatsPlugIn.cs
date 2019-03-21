// <copyright file="IUpdateMasterStatsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    /// <summary>
    /// Interface of a view whose implementation informs about the master stats of the character.
    /// </summary>
    public interface IUpdateMasterStatsPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Sends the master stats to the client.
        /// </summary>
        void SendMasterStats();
    }
}