// <copyright file="SkinChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments
{
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Arguments used by the <see cref="SkinChatCommandPlugIn"/>.
    /// </summary>
    public class SkinChatCommandArgs : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the skin number, which is mostly equivalent to the <see cref="MonsterDefinition.Number"/>.
        /// </summary>
        [Argument("skin", true)]
        public short SkinNumber { get; set; }
    }
}