using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

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