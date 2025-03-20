// <copyright file="SetMasterLevelUpPointsChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Globalization;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which sets a character's master level-up points.
/// </summary>
[Guid("69AC0B9E-1063-448E-ABD6-C5837A1E8A4B")]
[PlugIn("Set master level up points command", "Sets master level up points of a player. Usage: /setmasterleveluppoints (points) (optional:character)")]
[ChatCommandHelp(Command, "Sets master level up points of a player. Usage: /setmasterleveluppoints (points) (optional:character)", null)]
public class SetMasterLevelUpPointsChatCommandPlugIn : ChatCommandPlugInBase<SetMasterLevelUpPointsChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/setmasterleveluppoints";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;
    private const string CharacterNotFoundMessage = "Character '{0}' not found.";
    private const string InvalidMasterLevelUpPointsMessage = "Invalid master level-up points - must be bigger or equal to 0.";
    private const string MasterLevelUpPointsSetMessage = "Master level-up points set to {0}.";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        if (arguments is null)
        {
            await this.ShowMessageToAsync(player, InvalidMasterLevelUpPointsMessage).ConfigureAwait(false);
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

        if (targetPlayer?.SelectedCharacter is null)
        {
            return;
        }

        if (arguments.MasterLevelUpPoints < 0)
        {
            await this.ShowMessageToAsync(player, InvalidMasterLevelUpPointsMessage).ConfigureAwait(false);
            return;
        }

        targetPlayer.SelectedCharacter.MasterLevelUpPoints = checked(arguments.MasterLevelUpPoints);
        await targetPlayer.InvokeViewPlugInAsync<IUpdateLevelPlugIn>(p => p.UpdateMasterLevelAsync()).ConfigureAwait(false);
        await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, MasterLevelUpPointsSetMessage, arguments.MasterLevelUpPoints)).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the Set Master Level-Up Points chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the master level-up points to set.
        /// </summary>
        public int MasterLevelUpPoints { get; set; }

        /// <summary>
        /// Gets or sets the character name to set master level-up points for.
        /// </summary>
        public string? CharacterName { get; set; }
    }
}