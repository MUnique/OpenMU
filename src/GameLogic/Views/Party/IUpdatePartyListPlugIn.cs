// <copyright file="IUpdatePartyListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Party
{
    /// <summary>
    /// Interface of a view whose implementation informs about an updated party list.
    /// </summary>
    public interface IUpdatePartyListPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Updates the party list.
        /// </summary>
        void UpdatePartyList();
    }
}