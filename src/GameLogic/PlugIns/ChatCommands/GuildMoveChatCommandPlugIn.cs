// <copyright file="GuildMoveChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles gm move commands.
/// </summary>
[Guid("9163C3EA-6722-4E55-A109-20C163C05266")]
[PlugIn("Guild move chat command", "Handles the chat command '/guildmove <guild> <map> <x?> <y?>'. Move the character from a guild to a specified map and coordinates.")]
[ChatCommandHelp(Command, "Move the character from a guild to a specified map and coordinates.", typeof(GuildMoveChatCommandArgs), CharacterStatus.GameMaster)]
public class GuildMoveChatCommandPlugIn : ChatCommandPlugInBase<GuildMoveChatCommandArgs>
{
    private const string Command = "/guildmove";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, GuildMoveChatCommandArgs arguments)
    {
        var guildId = await this.GetGuildIdByNameAsync(gameMaster, arguments.GuildName!).ConfigureAwait(false);

        if (gameMaster.GameContext is not IGameServerContext gameServerContext)
        {
            return;
        }

        var exitGate = await this.GetExitGateAsync(gameMaster, arguments.MapIdOrName!, arguments.Coordinates).ConfigureAwait(false);
        await gameServerContext.ForEachGuildPlayerAsync(guildId, async guildPlayer =>
        {
            await guildPlayer.WarpToAsync(exitGate).ConfigureAwait(false);

            if (!guildPlayer.Name.Equals(gameMaster.Name))
            {
                var targetMessage = guildPlayer.GetLocalizedMessage("Chat_Move_PlayerMoved", "You have been moved by the game master.");
                await this.ShowMessageToAsync(guildPlayer, targetMessage).ConfigureAwait(false);
                var senderMessage = gameMaster.GetLocalizedMessage(
                    "Chat_Move_TargetMoved",
                    "[{0}] {1} has been moved to {2} at {3}, {4}",
                    this.Key,
                    guildPlayer.Name,
                    exitGate!.Map!.Name,
                    guildPlayer.Position.X,
                    guildPlayer.Position.Y);
                await this.ShowMessageToAsync(gameMaster, senderMessage).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }
}