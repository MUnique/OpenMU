// <copyright file="GetLevelChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin to get a character's level.
/// </summary>
[Guid("9D5C8FFE-EC32-48AC-8B6F-BB361AD184E5")]
[PlugIn]
[Display(Name = "Get level command", Description = "Gets level of a player. Usage: /getlevel (optional:character)")]
[ChatCommandHelp(Command, "Gets level of a player. Usage: /getlevel (optional:character)", null)]
public class GetLevelChatCommandPlugIn : ChatCommandPlugInBase<GetLevelChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/getlevel";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;

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
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CharacterNotFound), characterName).ConfigureAwait(false);
                return;
            }
        }

        if (targetPlayer.SelectedCharacter is null)
        {
            return;
        }

        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.LevelInformation), targetPlayer.SelectedCharacter.Name, targetPlayer.Attributes![Stats.Level]).ConfigureAwait(false);
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