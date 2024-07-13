// <copyright file="HideChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles hide commands.
/// </summary>
[Guid("7CE1CA66-C6B1-4840-9997-EF15C49FAB49")]
[PlugIn("Hide command", "Handles the chat command '/hide'. Hides the own player from others.")]
[ChatCommandHelp(Command, "Hides the own player from others.", CharacterStatus.GameMaster)]
public class HideChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/hide";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc/>
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        await player.AddInvisibleEffectAsync().ConfigureAwait(false);
    }
}