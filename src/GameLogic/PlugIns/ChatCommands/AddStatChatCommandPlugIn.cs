// <copyright file="AddStatChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.PlayerActions.Character;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A chat command plugin which handles the command to add stat points.
    /// </summary>
    [Guid("042EC5C6-27C8-4E00-A48B-C5458EDEA0BC")]
    [PlugIn("Add Stat chat command", "Handles the chat command '/add (ene|agi|vit|str|cmd) (amount)'. Adds the specified amount of stat points to the specified attribute of the character.")]
    [ChatCommandHelp(Command, typeof(Arguments), MinimumStatus)]
    public class AddStatChatCommandPlugIn : IChatCommandPlugIn
    {
        private const string Command = "/add";

        private const CharacterStatus MinimumStatus = CharacterStatus.Normal;

        private readonly IncreaseStatsAction action = new ();

        /// <inheritdoc />
        public string Key => Command;

        /// <inheritdoc />
        public CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

        /// <inheritdoc />
        public void HandleCommand(Player player, string command)
        {
            try
            {
                if (player.SelectedCharacter is null)
                {
                    return;
                }

                var arguments = command.ParseArguments<Arguments>();
                var attribute = this.GetAttribute(player, arguments.StatType);
                for (var i = 0; i < arguments.Amount; i++)
                {
                    if (player.SelectedCharacter?.LevelUpPoints <= 0)
                    {
                        player.ShowMessage("Cancelled adding points. No more points available.");
                        break;
                    }

                    this.action.IncreaseStats(player, attribute);
                }
            }
            catch (ArgumentException e)
            {
                player.ShowMessage(e.Message);
            }
        }

        private AttributeDefinition GetAttribute(Player player, string? statType)
        {
            var attribute = statType switch
            {
                "str" => Stats.BaseStrength,
                "agi" => Stats.BaseAgility,
                "vit" => Stats.BaseVitality,
                "ene" => Stats.BaseEnergy,
                "cmd" => Stats.BaseLeadership,
                _ => throw new ArgumentException($"Unknown stat: '{statType}'."),
            };

            if (player.SelectedCharacter!.Attributes.All(sa => sa.Definition != attribute))
            {
                throw new ArgumentException($"The character has no stat attribute '{statType}'.");
            }

            return attribute;
        }

        private class Arguments : ArgumentsBase
        {
            [ValidValues("str", "agi", "vit", "ene", "cmd")]
            public string? StatType { get; set; }

            public int Amount { get; set; }
        }
    }
}