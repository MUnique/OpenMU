// <copyright file="GetResetsChatCommandPlugIn.cs" company="MUnique">
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
[Guid("26ACF6A9-346A-49DF-8583-EA610F6E3AEA")]
[PlugIn("Get resets command", "Gets resets of a player. Usage: /getresets (optional:character)")]
[ChatCommandHelp(Command, "Gets resets of a player. Usage: /getresets (optional:character)", null)]
public class GetResetsChatCommandPlugIn : ChatCommandPlugInBase<GetResetsChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/getresets";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;
    private const string ResetPluginDisabledMessage = "The reset system is not enabled on this server.";
    private const string CharacterNotFoundMessage = "Character '{0}' not found.";
    private const string ResetsGetMessage = "Resets of '{0}': {1}.";

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

        if (targetPlayer.SelectedCharacter is null)
        {
            return;
        }

        await this.ShowMessageToAsync(player, string.Format(CultureInfo.InvariantCulture, ResetsGetMessage, targetPlayer.SelectedCharacter.Name, targetPlayer.Attributes![Stats.Resets])).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the Get Resets chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the character name to get resets for.
        /// </summary>
        public string? CharacterName { get; set; }
    }
}