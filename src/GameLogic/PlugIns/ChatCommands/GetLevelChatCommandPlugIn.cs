// <copyright file="GetLevelChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Globalization;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin to get a character's level.
/// </summary>
[Guid("9D5C8FFE-EC32-48AC-8B6F-BB361AD184E5")]
[PlugIn("Get level command", "Obtiene el nivel de un jugador. Uso: /getlevel (opcional:personaje)")]
[ChatCommandHelp(Command, "Obtiene el nivel de un jugador. Uso: /getlevel (opcional:personaje)", null)]
public class GetLevelChatCommandPlugIn : ChatCommandPlugInBase<GetLevelChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/getlevel";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;
    private const string CharacterNotFoundMessage = "Personaje '{0}' no encontrado.";
    private const string LevelGetMessage = "Nivel de '{0}': {1}.";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        var targetPlayer = player;
        if (arguments?.CharacterName is { } characterName)
        {
            targetPlayer = player.GameContext.GetPlayerByCharacterName(characterName);
            if (targetPlayer?.SelectedCharacter is null ||
                !targetPlayer.SelectedCharacter.Name.Equals(characterName, StringComparison.OrdinalIgnoreCase))
            {
                await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, CharacterNotFoundMessage, characterName)).ConfigureAwait(false);
                return;
            }
        }

        if (targetPlayer.SelectedCharacter is null)
        {
            return;
        }

        await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, LevelGetMessage, targetPlayer.SelectedCharacter.Name, targetPlayer.Attributes![Stats.Level])).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the Get Level chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the character name to get level for.
        /// </summary>
        public string? CharacterName { get; set; }
    }
}