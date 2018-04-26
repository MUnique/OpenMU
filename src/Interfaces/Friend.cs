// <copyright file="Friend.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    using System;

    /// <summary>
    /// The friend class used by the <see cref="IFriendServer"/>.
    /// </summary>
    public class Friend
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the id of the character.
        /// </summary>
        public Guid CharacterId { get; set; }

        /// <summary>
        /// Gets or sets the id of the friend character.
        /// </summary>
        public Guid FriendId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the friend request got accepted.
        /// </summary>
        public bool Accepted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this request is open.
        /// </summary>
        public bool RequestOpen { get; set; }
    }
}