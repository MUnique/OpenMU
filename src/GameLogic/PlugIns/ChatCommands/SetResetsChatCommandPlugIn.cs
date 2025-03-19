// <copyright file="SetResetsChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Globalization;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Resets;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which sets a character's resets.
/// </summary>
[Guid("47A8644C-B6C5-439E-BAB0-C1A7AE72691C")]
[PlugIn("Set resets command", "Sets resets of a player. Usage: /setresets (resets) (optional:character)")]
[ChatCommandHelp(Command, "Sets resets of a player. Usage: /setresets (resets) (optional:character)", null)]
public class SetResetsChatCommandPlugIn : ChatCommandPlugInBase<SetResetsChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/setresets";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;
    private const string ResetPluginDisabledMessage = "The reset system is not enabled on this server.";
    private const string CharacterNotFoundMessage = "Character '{0}' not found.";
    private const string InvalidResetsWithLimitMessage = "Invalid resets - must be between 0 and {0}.";
    private const string InvalidResetsNoLimitMessage = "Invalid resets - must be bigger than 0.";
    private const string ResetsSetMessage = "Resets set to {0}.";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        var configuration = player.GameContext.FeaturePlugIns.GetPlugIn<ResetFeaturePlugIn>()?.Configuration;
        if (configuration is null)
        {
            await this.ShowMessageToAsync(player, ResetPluginDisabledMessage).ConfigureAwait(false);
            return;
        }

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

        if (targetPlayer?.SelectedCharacter is null)
        {
            return;
        }

        if (configuration.ResetLimit is null)
        {
            if (arguments is null || arguments.Resets < 0)
            {
                await this.ShowMessageToAsync(player, InvalidResetsNoLimitMessage).ConfigureAwait(false);
                return;
            }
        }
        else
        {
            if (arguments is null || arguments.Resets < 0 || arguments.Resets > configuration.ResetLimit)
            {
                await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, InvalidResetsWithLimitMessage, configuration.ResetLimit)).ConfigureAwait(false);
                return;
            }
        }

        targetPlayer.Attributes![Stats.Resets] = checked(arguments.Resets);
        await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, ResetsSetMessage, arguments.Resets)).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the Set Resets chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the resets to set.
        /// </summary>
        public int Resets { get; set; }

        /// <summary>
        /// Gets or sets the character name to set resets for (GM only).
        /// </summary>
        public string? CharacterName { get; set; }
    }
}