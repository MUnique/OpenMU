// <copyright file="Guild.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    /// <summary>
    /// A guild is a group of players who like to play together.
    /// </summary>
    /// <remarks>
    /// You may wonder where the Id is. The original server and this <see cref="IGuildServer"/> uses an integer id for guilds, too.
    /// I decided to manage these keys in memory and on demand, they are not persistent.
    /// I could've used an integer Id as primary key in the persistent Guild class, but don't do it by purpose:
    /// We would lose the ability to easily merge two databases (realms), if we do that. Even if we use a separate integer id column in the database,
    /// it needs to be re-assigned after a merge.
    /// </remarks>
    public class Guild
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <remarks>
        /// It's like a 16 color 8x8 pixel bitmap, therefore has a size of 32 bytes.
        /// </remarks>
        public byte[] Logo { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the guild notice which can be set by the guild master.
        /// </summary>
        /// <remarks>Visible in green color after a character entered the game.</remarks>
        public string Notice { get; set; }

        /// <summary>
        /// Gets or sets the hostile guild. Members of a hostile guild can be killed without consequences.
        /// </summary>
        public virtual Guild Hostility { get; set; }

        /// <summary>
        /// Gets or sets the parent alliance guild.
        /// </summary>
        /// <value>
        /// The alliance guild.
        /// </value>
        public virtual Guild AllianceGuild { get; set; }
    }
}
