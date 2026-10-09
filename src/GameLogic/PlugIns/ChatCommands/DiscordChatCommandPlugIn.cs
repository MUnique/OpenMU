// <copyright file="DiscordChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which links the account of the player to a Discord user.
/// <c>/discord link</c> creates a one-time code, which the player enters in Discord with <c>/link</c>.
/// <c>/discord unlink</c> removes the link, and <c>/discord</c> shows it.
/// <c>/discord notify letter</c> turns the direct messages about letters on or off; <c>login</c> and <c>friend</c> are the other types.
/// </summary>
[Guid("6694D50F-49B1-481D-8F65-B0D7DCCA77C3")]
[PlugIn]
[Display(Name = nameof(PlugInResources.DiscordChatCommandPlugIn_Name), Description = nameof(PlugInResources.DiscordChatCommandPlugIn_Description), ResourceType = typeof(PlugInResources))]
[ChatCommandHelp(Command, typeof(DiscordChatCommandArgs), CharacterStatus.Normal)]
public class DiscordChatCommandPlugIn : ChatCommandPlugInBase<DiscordChatCommandArgs>
{
    private const string Command = "/discord";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc/>
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, DiscordChatCommandArgs arguments)
    {
        if (player.Account is not { } account)
        {
            return;
        }

        var service = new AccountLinkService(() => player.GameContext.PersistenceContextProvider.CreateNewPlayerContext(player.GameContext.Configuration));
        var accountId = account.GetId();
        switch (arguments.Action?.ToLowerInvariant())
        {
            case DiscordChatCommandArgs.LinkAction:
                var code = await service.CreateCodeAsync(accountId, AccountLinkService.DiscordProvider, player.SelectedCharacter?.Name).ConfigureAwait(false);
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.DiscordLinkCode), code, (int)AccountLinkService.CodeValidity.TotalMinutes).ConfigureAwait(false);
                break;
            case DiscordChatCommandArgs.UnlinkAction:
                if (await service.UnlinkAccountAsync(accountId, AccountLinkService.DiscordProvider).ConfigureAwait(false) is { } userId
                    && player.GameContext is IGameServerContext gameServerContext)
                {
                    // The Discord bot removes the roles of the user.
                    await gameServerContext.EventPublisher.GameEventAsync(
                        new AccountUnlinkedEvent(gameServerContext.Id, DateTime.UtcNow, AccountLinkService.DiscordProvider, userId)).ConfigureAwait(false);
                }

                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.DiscordUnlinked)).ConfigureAwait(false);
                break;
            case DiscordChatCommandArgs.NotifyAction:
                var type = AccountNotificationKeywords.Parse(arguments.Type);
                if (await service.SetNotificationAsync(accountId, AccountLinkService.DiscordProvider, type, null).ConfigureAwait(false) is { } notifications)
                {
                    await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.DiscordNotifications), AccountNotificationKeywords.Format(notifications)).ConfigureAwait(false);
                }
                else
                {
                    await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.DiscordNotLinked)).ConfigureAwait(false);
                }

                break;
            default:
                var link = await service.GetLinkAsync(accountId, AccountLinkService.DiscordProvider).ConfigureAwait(false);
                if (link is null)
                {
                    await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.DiscordNotLinked)).ConfigureAwait(false);
                }
                else
                {
                    await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.DiscordLinked), link.ExternalUserName ?? link.ExternalUserId ?? string.Empty, link.CharacterName ?? "-").ConfigureAwait(false);
                }

                break;
        }
    }
}
