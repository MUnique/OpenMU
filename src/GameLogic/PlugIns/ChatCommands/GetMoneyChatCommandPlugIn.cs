// <copyright file="GetMoneyChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Globalization;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin to get a character's money.
/// </summary>
[Guid("207F5872-33AB-4764-B67F-95AB7C6313E3")]
[PlugIn("Get money command", "Obtiene el Zen de un jugador. Uso: /getmoney (opcional:personaje)")]
[ChatCommandHelp(Command, "Obtiene el Zen de un jugador. Uso: /getmoney (opcional:personaje)", null)]
public class GetMoneyChatCommandPlugIn : ChatCommandPlugInBase<GetMoneyChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/getmoney";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;
    private const string CharacterNotFoundMessage = "Personaje '{0}' no encontrado.";
    private const string MoneyGetMessage = "Zen de '{0}': {1}.";

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

        if (targetPlayer.SelectedCharacter?.Inventory is null)
        {
            return;
        }

        await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, MoneyGetMessage, targetPlayer.SelectedCharacter.Name, targetPlayer.Money)).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the Get Money chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the character name to get money for.
        /// </summary>
        public string? CharacterName { get; set; }
    }
}