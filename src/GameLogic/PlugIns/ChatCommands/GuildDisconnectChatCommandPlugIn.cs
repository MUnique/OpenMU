// <copyright file="GuildDisconnectChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles gm disconnect commands.
/// </summary>
[Guid("F23262E6-0D7C-4B9C-8CD5-7E44AF4EE469")]
[PlugIn("Guild disconnect chat command", "Handles the chat command '/guilddisconnect <guild>'. Disconnect the guild members.")]
[ChatCommandHelp(Command, "Disconnects the members of a guild.", typeof(GuildDisconnectChatCommandArgs), CharacterStatus.GameMaster)]
public class GuildDisconnectChatCommandPlugIn : ChatCommandPlugInBase<GuildDisconnectChatCommandArgs>
{
    private const string Command = "/guilddisconnect";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player gameMaster, GuildDisconnectChatCommandArgs arguments)
    {
        var guildId = await this.GetGuildIdByNameAsync(gameMaster, arguments.GuildName!).ConfigureAwait(false);
        if (gameMaster.GameContext is not IGameServerContext gameServerContext)
        {
            return;
        }

        await gameServerContext.ForEachGuildPlayerAsync(guildId, async guildPlayer =>
        {
            await guildPlayer.DisconnectAsync().ConfigureAwait(false);

            if (!guildPlayer.Name.Equals(gameMaster.Name))
            {
                await this.ShowMessageToAsync(gameMaster, $"[{this.Key}] {guildPlayer.Name} has been disconnected.").ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }
}