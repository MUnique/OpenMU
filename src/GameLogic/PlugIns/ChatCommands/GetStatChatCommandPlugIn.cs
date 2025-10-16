// <copyright file="GetStatChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Globalization;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles the command to get stat points.
/// </summary>
[Guid("F8CACA47-D486-45AE-814F-C6218AD87652")]
[PlugIn("Get Stat chat command", "Obtiene puntos de atributo. Uso: /get (ene|agi|vit|str|cmd) (opcional:personaje)")]
[ChatCommandHelp(Command, "Obtiene puntos de atributo. Uso: /get (ene|agi|vit|str|cmd) (opcional:personaje)", typeof(Arguments), MinimumStatus)]
public class GetStatChatCommandPlugIn : ChatCommandPlugInBase<GetStatChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/get";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;
    private const string CharacterNotFoundMessage = "Personaje '{0}' no encontrado.";
    private const string StatGetMessage = "Atributo de '{0}': {1}.";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        try
        {
            if (arguments is null)
            {
                return;
            }

            var targetPlayer = player;
            if (arguments.CharacterName is { } characterName)
            {
                targetPlayer = player.GameContext.GetPlayerByCharacterName(characterName);
                if (targetPlayer?.SelectedCharacter is null ||
                    !targetPlayer.SelectedCharacter.Name.Equals(characterName, StringComparison.OrdinalIgnoreCase))
                {
                    await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, CharacterNotFoundMessage, characterName)).ConfigureAwait(false);
                    return;
                }
            }

            if (targetPlayer.SelectedCharacter is not { } selectedCharacter)
            {
                return;
            }

            var attribute = this.GetAttribute(selectedCharacter, arguments.StatType);
            await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, StatGetMessage, selectedCharacter.Name, targetPlayer.Attributes![attribute])).ConfigureAwait(false);
        }
        catch (ArgumentException e)
        {
            await player.ShowMessageAsync(e.Message).ConfigureAwait(false);
        }
    }

    private AttributeDefinition GetAttribute(Character selectedCharacter, string? statType)
    {
        var attribute = statType switch
        {
            "str" => Stats.BaseStrength,
            "agi" => Stats.BaseAgility,
            "vit" => Stats.BaseVitality,
            "ene" => Stats.BaseEnergy,
            "cmd" => Stats.BaseLeadership,
            _ => throw new ArgumentException($"Atributo desconocido: '{statType}'."),
        };

        if (selectedCharacter.Attributes.All(sa => sa.Definition != attribute))
        {
            throw new ArgumentException($"El personaje no tiene el atributo '{statType}'.");
        }

        return attribute;
    }

    /// <summary>
    /// Arguments for the Get Stat chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the stat type to get.
        /// </summary>
        [ValidValues("str", "agi", "vit", "ene", "cmd")]
        public string? StatType { get; set; }

        /// <summary>
        /// Gets or sets the character name to get stat for.
        /// </summary>
        public string? CharacterName { get; set; }
    }
}
