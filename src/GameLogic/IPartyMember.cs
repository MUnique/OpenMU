// <copyright file="IPartyMember.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    /// <summary>
    /// The interface for a party member.
    /// </summary>
    public interface IPartyMember : IWorldObserver, IObservable, IIdentifiable, ILocateable
    {
        /// <summary>
        /// Gets or sets the current party of the player.
        /// </summary>
        Party Party { get; set; }

        /// <summary>
        /// Gets or sets the last party requester.
        /// </summary>
        IPartyMember LastPartyRequester { get; set; }

        /// <summary>
        /// Gets the maximum health.
        /// </summary>
        uint MaximumHealth { get; }

        /// <summary>
        /// Gets the current health.
        /// </summary>
        uint CurrentHealth { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }
    }
}
