// <copyright file="GetStatChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using MUnique.OpenMU.PlugIns;
using System.Runtime.InteropServices;

/// <summary>
/// A chat command plugin which handles the command to get stat points.
/// </summary>
[Guid("F8CACA47-D486-45AE-814F-C6218AD87652")]
[PlugIn]
[Display(Name = nameof(PlugInResources.GetStatChatCommandPlugIn_Name), Description = nameof(PlugInResources.GetStatChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, "Get stat points. Usage: /get (ene|agi|vit|str|cmd) (optional:character)", typeof(Arguments), MinimumStatus)]
public class GetStatChatCommandPlugIn : ChatCommandPlugInBase<GetStatChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/get";
    private const CharacterStatus MinimumStatus = CharacterStatus.GameMaster;

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, Arguments arguments)
    {
        var targetPlayer = player;
        if (arguments.CharacterName is { } characterName)
        {
            targetPlayer = player.GameContext.GetPlayerByCharacterName(characterName);
            if (targetPlayer?.SelectedCharacter is null ||
                !targetPlayer.SelectedCharacter.Name.Equals(characterName, StringComparison.OrdinalIgnoreCase))
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CharacterNotFound), characterName).ConfigureAwait(false);
                return;
            }
        }

        if (await this.TryGetAttributeAsync(targetPlayer, arguments.StatType).ConfigureAwait(false) is not { } attribute)
        {
            return;
        }

        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.StatPointInfo), targetPlayer.SelectedCharacter?.Name, targetPlayer.Attributes![attribute]).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the Get Stat chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the stat type to get.
        /// </summary>
        [ValidValues("str", "agi", "vit", "ene", "cmd")]
        public string? StatType { get; set; }

        /// <summary>
        /// Gets or sets the character name to get stat for.
        /// </summary>
        public string? CharacterName { get; set; }
    }
}