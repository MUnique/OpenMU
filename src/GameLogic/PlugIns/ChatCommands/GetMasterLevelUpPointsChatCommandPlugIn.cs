// <copyright file="GetMasterLevelUpPointsChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Globalization;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin to get a character's master level-up points.
/// </summary>
[Guid("8ACCF267-F5F3-4003-B4C3-536ACCB5181D")]
[PlugIn("Get master level up points command", "Obtiene los puntos de nivel maestro de un jugador. Uso: /getmasterleveluppoints (opcional:personaje)")]
[ChatCommandHelp(Command, "Obtiene los puntos de nivel maestro de un jugador. Uso: /getmasterleveluppoints (opcional:personaje)", null)]
public class GetMasterLevelUpPointsChatCommandPlugIn : ChatCommandPlugInBase<GetMasterLevelUpPointsChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/getmasterleveluppoints";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;
    private const string CharacterNotFoundMessage = "Personaje '{0}' no encontrado.";
    private const string MasterLevelUpPointsGetMessage = "Puntos de nivel maestro de '{0}': {1}.";

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

        await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, MasterLevelUpPointsGetMessage, targetPlayer.SelectedCharacter.Name, targetPlayer.SelectedCharacter.MasterLevelUpPoints)).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the Get Master Level-Up Points chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the character name to get master level-up points for.
        /// </summary>
        public string? CharacterName { get; set; }
    }
}