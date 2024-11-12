// <copyright file="GuildWarChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles guild war requests.
/// </summary>
[Guid("12A6E159-0D5E-44DE-8CF8-012A7278D42C")]
[PlugIn("Guild war chat command", "Handles the chat command '/war <guild>'. Sends a request to the guild master of the requested guild.")]
[ChatCommandHelp(Command, "Sends a guild war request to the master of the requested guild.", typeof(GuildWarChatCommandArgs), CharacterStatus.Normal)]
public class GuildWarChatCommandPlugIn : ChatCommandPlugInBase<GuildWarChatCommandArgs>
{
    private const string Command = "/war";

    private readonly GuildWarRequestAction _action = new();

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player guildMaster, GuildWarChatCommandArgs arguments)
    {
        await this._action.RequestWarAsync(guildMaster, arguments.GuildName).ConfigureAwait(false);
    }
}