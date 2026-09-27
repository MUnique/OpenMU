// <copyright file="SlideNoticeChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which sends a notice to all players that scrolls across the top of their screen.
/// </summary>
[Guid("6F0B8F37-3C1D-4E5A-9B7E-2D4C8A1F5E93")]
[PlugIn]
[Display(Name = nameof(PlugInResources.SlideNoticeChatCommandPlugIn_Name), Description = nameof(PlugInResources.SlideNoticeChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(CommandKey, CharacterStatus.GameMaster)]
public class SlideNoticeChatCommandPlugIn : IChatCommandPlugIn
{
    private const string CommandKey = "/slidenotice";

    /// <inheritdoc />
    public string Key => CommandKey;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var regex = new Regex(Regex.Escape(CommandKey));
        var message = regex.Replace(command, string.Empty, 1)?.Trim();

        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        await player.GameContext.SendGlobalMessageAsync(message, Interfaces.MessageType.SlideNotice).ConfigureAwait(false);
    }
}
