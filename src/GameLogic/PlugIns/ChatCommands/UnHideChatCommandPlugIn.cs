// <copyright file="UnHideChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles unhide commands.
/// </summary>
[Guid("0F0ADAC6-88C7-4EC0-94A2-A289173DEDA7")]
[PlugIn("Hide command", "Handles the chat command '/unhide'. Unhides the own player from others.")]
[ChatCommandHelp(Command, "Unhides the own player from others.", CharacterStatus.GameMaster)]
public class UnHideChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/unhide";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        await player.RemoveInvisibleEffectAsync().ConfigureAwait(false);
    }
}