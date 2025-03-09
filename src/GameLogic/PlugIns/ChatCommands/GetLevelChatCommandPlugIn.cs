// <copyright file="GetLevelChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin to get a character's level.
/// </summary>
[Guid("B8E35F57-2ED4-4BAD-9F95-9C88E1B92B1C")]
[PlugIn("Get level command", "Gets level of a player.")]
[ChatCommandHelp(Command, "Gets level of a player. Usage: /getlevel <optional:character>", null)]
public class GetLevelChatCommandPlugIn : ChatCommandPlugInBase<GetLevelChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/getlevel";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;
    private const string CharacterNotFoundMessage = "Character '{0}' not found.";
    private const string LevelGetMessage = "Level of '{0}': {1}.";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        var targetPlayer = player;
        if (arguments?.CharacterName != null)
        {
            targetPlayer = player.GameContext.GetPlayerByCharacterName(arguments.CharacterName);
            if (targetPlayer?.SelectedCharacter == null ||
                !targetPlayer.SelectedCharacter.Name.Equals(arguments.CharacterName, StringComparison.OrdinalIgnoreCase))
            {
                await this.ShowMessageToAsync(player, string.Format(CharacterNotFoundMessage, arguments.CharacterName)).ConfigureAwait(false);
                return;
            }
        }

        if (targetPlayer?.SelectedCharacter == null)
        {
            return;
        }

        await this.ShowMessageToAsync(player, string.Format(LevelGetMessage, targetPlayer.SelectedCharacter.Name, targetPlayer.Attributes![Stats.Level])).ConfigureAwait(false);
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