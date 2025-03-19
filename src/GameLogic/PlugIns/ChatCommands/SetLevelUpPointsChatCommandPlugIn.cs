// <copyright file="SetLevelUpPointsChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Globalization;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which sets a character's level-up points.
/// </summary>
[Guid("50EF670A-DF7A-4FEE-8E42-7C7A18A68941")]
[PlugIn("Set level up points command", "Sets level up points of a player. Usage: /setleveluppoints (points) (optional:character)")]
[ChatCommandHelp(Command, "Sets level up points of a player. Usage: /setleveluppoints (points) (optional:character)", null)]
public class SetLevelUpPointsChatCommandPlugIn : ChatCommandPlugInBase<SetLevelUpPointsChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/setleveluppoints";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;
    private const string CharacterNotFoundMessage = "Character '{0}' not found.";
    private const string InvalidLevelUpPointsMessage = "Invalid level-up points - must be bigger or equal to 0.";
    private const string LevelUpPointsSetMessage = "Level-up points set to {0}.";

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

        if (arguments is null || arguments.LevelUpPoints < 0)
        {
            await this.ShowMessageToAsync(player, InvalidLevelUpPointsMessage).ConfigureAwait(false);
            return;
        }

        targetPlayer.SelectedCharacter.LevelUpPoints = checked(arguments.LevelUpPoints);
        await targetPlayer.InvokeViewPlugInAsync<IUpdateLevelPlugIn>(p => p.UpdateLevelAsync()).ConfigureAwait(false);
        await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, LevelUpPointsSetMessage, arguments.LevelUpPoints)).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the Set Level-Up Points chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the level-up points to set.
        /// </summary>
        public int LevelUpPoints { get; set; }

        /// <summary>
        /// Gets or sets the character name to set level-up points for.
        /// </summary>
        public string? CharacterName { get; set; }
    }
}