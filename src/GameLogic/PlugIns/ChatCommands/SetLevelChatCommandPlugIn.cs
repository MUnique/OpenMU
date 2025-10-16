// <copyright file="SetLevelChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which sets a character's level.
/// </summary>
[Guid("4BE779C9-E6B6-47F2-BC23-2E71D82A6C1D")]
[PlugIn("Set level command", "Establece el nivel de un jugador. Uso: /setlevel (nivel) (opcional:personaje)")]
[ChatCommandHelp(Command, "Establece el nivel de un jugador. Uso: /setlevel (nivel) (opcional:personaje)", null)]
public class SetLevelChatCommandPlugIn : ChatCommandPlugInBase<SetLevelChatCommandPlugIn.Arguments>, IDisabledByDefault
{
    private const string Command = "/setlevel";
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
                var message = player.GetLocalizedMessage("Chat_SetLevel_CharacterNotFound", "Character '{0}' not found.", characterName);
                await this.ShowMessageToAsync(player, message).ConfigureAwait(false);
                return;
            }
        }

        if (targetPlayer.SelectedCharacter is null)
        {
            return;
        }

        if (arguments is null || arguments.Level < 1 || arguments.Level > targetPlayer.GameContext.Configuration.MaximumLevel)
        {
            var message = player.GetLocalizedMessage("Chat_SetLevel_InvalidLevel", "Invalid level - must be between 1 and {0}.", targetPlayer.GameContext.Configuration.MaximumLevel);
            await this.ShowMessageToAsync(player, message).ConfigureAwait(false);
            return;
        }

        targetPlayer.Attributes![Stats.Level] = checked(arguments.Level);
        await targetPlayer.InvokeViewPlugInAsync<IUpdateLevelPlugIn>(p => p.UpdateLevelAsync()).ConfigureAwait(false);
        await targetPlayer.ForEachWorldObserverAsync<IShowEffectPlugIn>(p => p.ShowEffectAsync(targetPlayer, IShowEffectPlugIn.EffectType.LevelUp), true).ConfigureAwait(false);
        var confirmation = player.GetLocalizedMessage("Chat_SetLevel_Success", "Level set to {0}.", arguments.Level);
        await this.ShowMessageToAsync(player, confirmation).ConfigureAwait(false);
    }

    /// <summary>
    /// Arguments for the Set Level chat command.
    /// </summary>
    public class Arguments : ArgumentsBase
    {
        /// <summary>
        /// Gets or sets the level to set.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the character name to set level for.
        /// </summary>
        public string? CharacterName { get; set; }
    }
}