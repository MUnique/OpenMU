// <copyright file="DiscordIntegrationViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Discord;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;
using DiscordIntegrationInfo = MUnique.OpenMU.GameLogic.Discord.DiscordIntegrationInfo;

/// <summary>
/// The default implementation of the <see cref="IDiscordIntegrationViewPlugIn"/> which sends the Discord
/// integration messages to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DiscordIntegrationViewPlugIn_Name), Description = nameof(PlugInResources.DiscordIntegrationViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("D1CFC066-B31F-4FE9-8435-4AE4AEB92B5A")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class DiscordIntegrationViewPlugIn : IDiscordIntegrationViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Set when the client requested the integration info. Only then it understands the external chat messages;
    /// other clients get them as normal chat messages.
    /// </summary>
    private bool _isSupportedByClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordIntegrationViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DiscordIntegrationViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowDiscordIntegrationInfoAsync(DiscordIntegrationInfo info)
    {
        this._isSupportedByClient = true;
        var configuration = info.Configuration;
        await this._player.Connection.SendDiscordIntegrationInfoAsync(
            info.LinkedUserName is not null,
            info.IsGuildChatBridged,
            info.IsAllianceChatBridged,
            info.IsWorldChatBridged,
            configuration?.RichPresenceApplicationId ?? string.Empty,
            configuration?.RichPresenceLargeImageKey ?? string.Empty,
            configuration?.RichPresenceSmallImageKey ?? string.Empty,
            configuration?.InviteUrl ?? string.Empty,
            info.LinkedUserName ?? string.Empty).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask ShowDiscordLinkCodeAsync(string? code, TimeSpan validity)
    {
        var result = code is null ? DiscordLinkCode.DiscordLinkCodeResult.NotAvailable : DiscordLinkCode.DiscordLinkCodeResult.Success;
        var validMinutes = (byte)Math.Clamp(validity.TotalMinutes, 0, byte.MaxValue);
        await this._player.Connection.SendDiscordLinkCodeAsync(result, code ?? string.Empty, validMinutes).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> TryShowExternalChatMessageAsync(ExternalChatScope scope, string senderName, string message)
    {
        if (!this._isSupportedByClient)
        {
            return false;
        }

        await this._player.Connection.SendExternalChatMessageAsync(
            ExternalChatMessage.ExternalChatSource.Discord,
            Convert(scope),
            senderName,
            message).ConfigureAwait(false);
        return true;
    }

    private static ExternalChatMessage.ExternalChatScope Convert(ExternalChatScope scope) => scope switch
    {
        ExternalChatScope.Guild => ExternalChatMessage.ExternalChatScope.Guild,
        ExternalChatScope.Alliance => ExternalChatMessage.ExternalChatScope.Alliance,
        _ => ExternalChatMessage.ExternalChatScope.World,
    };
}
