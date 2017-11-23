// <copyright file="Guild.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A guild is a group of players who like to play together.
    /// </summary>
    public class Guild
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <remarks>
        /// It's like a 16 color bitmap.
        /// </remarks>
        public byte[] Logo { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the guild master identifier.
        /// </summary>
        public Guid MasterId { get; set; }

        /// <summary>
        /// Gets or sets the guild master character name.
        /// </summary>
        public string Master { get; set; }

        /// <summary>
        /// Gets or sets the guild notice which can be set by the guild master
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

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        public virtual ICollection<GuildMemberInfo> Members { get; protected set; }
    }
}
