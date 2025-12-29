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
[PlugIn("Add Stat chat command", "Maneja el comando '/add (ene|agi|vit|str|cmd) (cantidad)'. Suma la cantidad indicada de puntos al atributo especificado del personaje.")]
[ChatCommandHelp(Command, "Suma la cantidad indicada de puntos al atributo especificado del personaje.", typeof(Arguments), MinimumStatus)]
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
                var message = player.GetLocalizedMessage("Chat_AddStat_NotAllowedDuringMiniGame", "Adding multiple points is not allowed during a mini-game.");
                await player.ShowMessageAsync(message).ConfigureAwait(false);
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
            _ => throw new ArgumentException(player.GetLocalizedMessage("Chat_AddStat_UnknownAttribute", "Unknown attribute: '{0}'.", statType)),
        };

        if (player.SelectedCharacter!.Attributes.All(sa => sa.Definition != attribute))
        {
            throw new ArgumentException(player.GetLocalizedMessage("Chat_AddStat_AttributeMissing", "The character doesn't have the attribute '{0}'.", statType));
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
