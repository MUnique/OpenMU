// <copyright file="GuildBattleSoccerChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles guild battle soccer requests.
/// </summary>
[Guid("A456F032-CE7D-4EA5-8EB2-96C2B04C70D1")]
[PlugIn("Guild battle soccer chat command", "Handles the chat command '/battlesoccer <guildname>'. Sends a request to the guild master of the requested guild.")]
[ChatCommandHelp(Command, "Sends a battle soccer request to the guild master of the requested guild.", typeof(GuildWarChatCommandArgs), CharacterStatus.Normal)]
public class GuildBattleSoccerChatCommandPlugIn : ChatCommandPlugInBase<GuildWarChatCommandArgs>
{
    private const string Command = "/battlesoccer";

    private readonly GuildWarRequestAction _action = new();

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player guildMaster, GuildWarChatCommandArgs arguments)
    {
        await this._action.RequestBattleSoccerAsync(guildMaster, arguments.GuildName).ConfigureAwait(false);
    }
}