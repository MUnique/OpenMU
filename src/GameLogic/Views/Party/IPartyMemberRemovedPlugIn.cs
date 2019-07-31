// <copyright file="IPartyMemberRemovedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Party
{
    /// <summary>
    /// Interface of a view whose implementation informs about a removed party player.
    /// </summary>
    public interface IPartyMemberRemovedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Delete Member from Party.
        /// </summary>
        /// <param name="index">index of the player.</param>
        void PartyMemberRemoved(byte index);
    }
}