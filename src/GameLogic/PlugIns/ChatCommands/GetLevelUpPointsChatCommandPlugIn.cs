// <copyright file="GetLevelUpPointsChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Globalization;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin to get a character's level-up points.
/// </summary>
[Guid("E4D65354-CCD2-4960-BDCA-D4582A57BBCB")]
[PlugIn("Get level up points command", "Gets level up points of a player. Usage: /getleveluppoints (optional:character)")]
[ChatCommandHelp(Command, "Gets level up points of a player. Usage: /getleveluppoints (optional:character)", null)]
public class GetLevelUpPointsChatCommandPlugIn : ChatCommandPlugInBase<GetLevelUpPointsChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/getleveluppoints";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;
    private const string CharacterNotFoundMessage = "Character '{0}' not found.";
    private const string LevelUpPointsGetMessage = "Level-up points of '{0}': {1}.";

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

        await this.ShowMessageToAsync(player, string.Format(LevelUpPointsGetMessage, targetPlayer.SelectedCharacter.Name, targetPlayer.SelectedCharacter.LevelUpPoints)).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the Get Level-Up Points chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the character name to get level-up points for.
        /// </summary>
        public string? CharacterName { get; set; }
    }
}