// <copyright file="GameServerClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System.Threading;
using global::Dapr.Client;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// A client of a game server of another process, which calls it through dapr.
/// </summary>
/// <remarks>
/// It's created by the <see cref="ManagableServerRegistry"/> when a game server publishes its state,
/// so that e.g. the admin panel can disconnect a player or send a global message.
/// </remarks>
public sealed class GameServerClient : ManageableServerClient, IGameServer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerClient"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="serverData">The server data.</param>
    public GameServerClient(DaprClient daprClient, ServerStateData serverData)
        : base(daprClient, serverData)
    {
    }

    /// <summary>
    /// Restarts the game server process, e.g. to apply a changed configuration.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// The process ends, so it has to be started again by its host, e.g. by the restart policy of its container.
    /// </remarks>
    public async Task RestartAsync(CancellationToken cancellationToken = default)
    {
        await this.DaprClient.InvokeMethodAsync(this.TargetAppId, "RestartAsync", cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ValueTask GuildChatMessageAsync(uint guildId, string sender, string message)
        => this.InvokeAsync(nameof(this.GuildChatMessageAsync), new GuildMessageArguments(guildId, sender, message));

    /// <inheritdoc />
    public ValueTask GuildDeletedAsync(uint guildId)
        => this.InvokeAsync(nameof(this.GuildDeletedAsync), guildId);

    /// <inheritdoc />
    public ValueTask GuildPlayerKickedAsync(string playerName)
        => this.InvokeAsync(nameof(this.GuildPlayerKickedAsync), playerName);

    /// <inheritdoc />
    public ValueTask AllianceChatMessageAsync(uint guildId, string sender, string message)
        => this.InvokeAsync(nameof(this.AllianceChatMessageAsync), new GuildMessageArguments(guildId, sender, message));

    /// <inheritdoc />
    public ValueTask SendGlobalMessageAsync(string message, MessageType messageType)
        => this.InvokeAsync(nameof(this.SendGlobalMessageAsync), new MessageArguments(message, messageType));

    /// <inheritdoc />
    public ValueTask<bool> DisconnectPlayerAsync(string playerName)
        => this.InvokeAsync<string, bool>(nameof(this.DisconnectPlayerAsync), playerName);

    /// <inheritdoc />
    public ValueTask<bool> DisconnectAccountAsync(string accountName)
        => this.InvokeAsync<string, bool>(nameof(this.DisconnectAccountAsync), accountName);

    /// <inheritdoc />
    public ValueTask<bool> BanPlayerAsync(string playerName)
        => this.InvokeAsync<string, bool>(nameof(this.BanPlayerAsync), playerName);

    /// <inheritdoc />
    public ValueTask AssignGuildToPlayerAsync(string characterName, GuildMemberStatus guildStatus)
        => this.InvokeAsync(nameof(this.AssignGuildToPlayerAsync), new GuildMemberAssignArguments(characterName, guildStatus));

    /// <inheritdoc />
    public ValueTask PlayerAlreadyLoggedInAsync(byte serverId, string loginName)
        => this.InvokeAsync(nameof(this.PlayerAlreadyLoggedInAsync), new PlayerLoggedInArguments(serverId, loginName));

    /// <inheritdoc />
    public ValueTask AllianceCreatedAsync(uint masterGuildId, uint memberGuildId)
        => this.InvokeAsync(nameof(this.AllianceCreatedAsync), new AllianceChangedArguments(masterGuildId, memberGuildId));

    /// <inheritdoc />
    public ValueTask AllianceDisbandedAsync(uint masterGuildId, uint memberGuildId)
        => this.InvokeAsync(nameof(this.AllianceDisbandedAsync), new AllianceChangedArguments(masterGuildId, memberGuildId));

    /// <inheritdoc />
    public ValueTask GuildHostilityChangedAsync(uint guildIdA, IReadOnlyList<uint> allianceGuildIdsA, uint guildIdB, IReadOnlyList<uint> allianceGuildIdsB, bool created)
        => this.InvokeAsync(nameof(this.GuildHostilityChangedAsync), new GuildHostilityChangedArguments(guildIdA, allianceGuildIdsA, guildIdB, allianceGuildIdsB, created));

    /// <inheritdoc />
    public ValueTask LetterReceivedAsync(LetterHeader letter)
        => this.InvokeAsync(nameof(this.LetterReceivedAsync), letter);

    /// <inheritdoc />
    public ValueTask FriendRequestAsync(string requester, string receiver)
        => this.InvokeAsync(nameof(this.FriendRequestAsync), new RequestArguments(requester, receiver));

    /// <inheritdoc />
    public ValueTask FriendOnlineStateChangedAsync(string player, string friend, int serverId)
        => this.InvokeAsync(nameof(this.FriendOnlineStateChangedAsync), new FriendOnlineStateChangedArguments(player, friend, serverId));

    /// <inheritdoc />
    public ValueTask ChatRoomCreatedAsync(ChatServerAuthenticationInfo playerAuthenticationInfo, string friendName)
        => this.InvokeAsync(nameof(this.ChatRoomCreatedAsync), new ChatRoomCreationArguments(playerAuthenticationInfo, friendName));

    /// <inheritdoc />
    public ValueTask InitializeMessengerAsync(MessengerInitializationData initializationData)
        => this.InvokeAsync(nameof(this.InitializeMessengerAsync), initializationData);

    private async ValueTask InvokeAsync<TRequest>(string methodName, TRequest data)
    {
        await this.DaprClient.InvokeMethodAsync(this.TargetAppId, methodName, data).ConfigureAwait(false);
    }

    private async ValueTask<TResponse> InvokeAsync<TRequest, TResponse>(string methodName, TRequest data)
    {
        return await this.DaprClient.InvokeMethodAsync<TRequest, TResponse>(this.TargetAppId, methodName, data).ConfigureAwait(false);
    }
}
