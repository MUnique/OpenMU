// <copyright file="AddStatChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the command to add stat points.
/// </summary>
[Guid("042EC5C6-27C8-4E00-A48B-C5458EDEA0BC")]
[PlugIn("Add Stat chat command", "Handles the chat command '/add (ene|agi|vit|str|cmd) (amount)'. Adds the specified amount of stat points to the specified attribute of the character.")]
[ChatCommandHelp(Command, "Adds the specified amount of stat points to the specified attribute of the character.", typeof(Arguments), MinimumStatus)]
public class AddStatChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/add";

    private const CharacterStatus MinimumStatus = CharacterStatus.Normal;

    private readonly IncreaseStatsAction _action = new();

    /// <inheritdoc />
    public virtual string Key => Command;

    /// <inheritdoc />
    public virtual CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    public virtual async ValueTask HandleCommandAsync(Player player, string command)
    {
        try
        {
            if (player.SelectedCharacter is null)
            {
                return;
            }

            var arguments = command.ParseArguments<Arguments>();
            var attribute = this.GetAttribute(player, arguments.StatType);
            var selectedCharacter = player.SelectedCharacter;

            if (!selectedCharacter.CanIncreaseStats(arguments.Amount))
            {
                return;
            }

            if (player.CurrentMiniGame is not null)
            {
                await player.ShowMessageAsync("Adding multiple points is not allowed when playing a mini game.").ConfigureAwait(false);
                return;
            }

            await this._action.IncreaseStatsAsync(player, attribute, arguments.Amount).ConfigureAwait(false);
        }
        catch (ArgumentException e)
        {
            await player.ShowMessageAsync(e.Message).ConfigureAwait(false);
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

        public ushort Amount { get; set; }
    }
}