// <copyright file="PartyCommand.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The online command which shows the count of connected players.
    /// </summary>
    [Guid("89A17085-491F-4C04-BA77-04F0AC44EBB7")]
    [PlugIn("Party request command", "Sending a player party request.")]
    [ChatCommandHelp(Command, null, MinimumStatus)]
    public class PartyCommand : IChatCommandPlugIn
    {
        private const string Command = "/party";

        private const CharacterStatus MinimumStatus = CharacterStatus.Normal;

        /// <inheritdoc/>
        public CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

        /// <inheritdoc />
        public string Key => Command;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            try
            {
                // logic here
            }
            catch (ArgumentException e)
            {
                player.ShowMessage(e.Message);
            }
        }

    }
}