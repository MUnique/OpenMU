// <copyright file="BotSayChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Globalization;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command for game masters which makes a server-side bot say something in the normal chat:
/// <c>/botsay &lt;bot&gt; &lt;text&gt;</c> - for staged scenes of an event, or a bot "joining" a
/// conversation the GM wants to spark.
/// </summary>
/// <remarks>
/// The text is said as typed, to every player who sees the bot - it's not translated. Only bots can
/// be made to talk, never a player's character.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.BotSayChatCommandPlugIn_Name), Description = nameof(PlugInResources.BotSayChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("825B3883-EDD9-4638-BA85-7A2852A86997")]
[ChatCommandHelp(CommandKey, CharacterStatus.GameMaster)]
public class BotSayChatCommandPlugIn : IChatCommandPlugIn
{
    private const string CommandKey = "/botsay";

    /// <inheritdoc />
    public string Key => CommandKey;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var arguments = command.Length > CommandKey.Length ? command[CommandKey.Length..].Trim() : string.Empty;
        var separator = arguments.IndexOf(' ', StringComparison.Ordinal);
        if (separator <= 0)
        {
            return;
        }

        var botName = arguments[..separator];
        var text = arguments[(separator + 1)..].Trim();
        if (text.Length == 0)
        {
            return;
        }

        if (player.GameContext.GetPlayerByCharacterName(botName) is not OfflinePlayer bot
            || bot.Account?.IsBot != true)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CharacterNotFound), botName).ConfigureAwait(false);
            return;
        }

        await BotChatSpeaker.SayAsync(bot, new LocalizedString(text), new Dictionary<string, Func<CultureInfo, string>>()).ConfigureAwait(false);
    }
}
