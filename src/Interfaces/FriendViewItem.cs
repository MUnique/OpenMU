// <copyright file="FriendViewItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    /// <summary>
    /// A friend view item, which includes the character names.
    /// </summary>
    public class FriendViewItem : Friend
    {
        /// <summary>
        /// Gets or sets the name of the character.
        /// </summary>
        public string CharacterName { get; set; }

        /// <summary>
        /// Gets or sets the name of the friend.
        /// </summary>
        public string FriendName { get; set; }
    }
}
