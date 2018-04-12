// <copyright file="GuildPosition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
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
}