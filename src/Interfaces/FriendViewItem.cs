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
        /// Initializes a new instance of the <see cref="FriendViewItem"/> class.
        /// </summary>
        /// <param name="characterName">Name of the character.</param>
        /// <param name="friendName">Name of the friend.</param>
        public FriendViewItem(string characterName, string friendName)
        {
            this.CharacterName = characterName;
            this.FriendName = friendName;
        }

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
