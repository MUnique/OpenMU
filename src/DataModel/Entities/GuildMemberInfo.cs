// <copyright file="GuildMemberInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities
{
    using System;

    /// <summary>
    /// The position of a character in a guild.
    /// Some of them have special skills in the castle siege event.
    /// </summary>
    public enum GuildPosition : byte
    {
        /// <summary>
        /// A normal guild member.
        /// </summary>
        NormalMember,

        /// <summary>
        /// The guild master.
        /// </summary>
        GuildMaster,

        /// <summary>
        /// The battle master.
        /// </summary>
        BattleMaster,
    }

    /// <summary>
    /// Information about a guild member.
    /// </summary>
    public class GuildMemberInfo
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the guild identifier to which the member belongs.
        /// </summary>
        public Guid GuildId { get; set; }

        /// <summary>
        /// Gets or sets the character identifier.
        /// </summary>
        public Guid CharacterId { get; set; }

        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the status of the member.
        /// </summary>
        public GuildPosition Status { get; set; }

        /// <summary>
        /// Gets or sets the server identifier where the member is currently playing on.
        /// </summary>
        public byte ServerId { get; set; }
    }
}
