// <copyright file="DropMoneyCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles reset command.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.IChatCommandPlugIn" />
    [Guid("90B35404-AADE-4F22-B5D2-4CD59B8BB4C3")]
    [PlugIn("Reset chat command", "Handles the chat command '/reset'.")]
    [ChatCommandHelp(Command, null)]
    public class DropMoneyCommandPlugIn : IChatCommandPlugIn
    {
        private const string Command = "/money";

        /// <inheritdoc />
        public string Key => Command;

        /// <inheritdoc />
        public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            var money = command.Split(' ').Skip(1).FirstOrDefault();
            if (money == null)
            {
                return;
            }

            var pos = new Point(player.SelectedCharacter.PositionX, player.SelectedCharacter.PositionY);
            var droppedItem = new DroppedMoney(uint.Parse(money), pos, player.CurrentMap, player, new[] { player });
            player.CurrentMap.Add(droppedItem);
        }
    }
}
