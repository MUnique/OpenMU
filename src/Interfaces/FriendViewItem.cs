// <copyright file="FriendViewItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    using System;

    /// <summary>
    /// A friend view item.
    /// </summary>
    public class FriendViewItem
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
        /// Gets or sets the name of the character.
        /// </summary>
        public string CharacterName { get; set; }

        /// <summary>
        /// Gets or sets the name of the friend.
        /// </summary>
        public string FriendName { get; set; }

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
