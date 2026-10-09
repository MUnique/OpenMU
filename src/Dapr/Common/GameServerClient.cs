// <copyright file="GameServerClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using System.Threading;
using global::Dapr.Client;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// A client of a game server of another process, which publishes its calls to all game servers through dapr.
/// </summary>
/// <remarks>
/// It's created by the <see cref="ManagableServerRegistry"/> when a game server publishes its state,
/// so that e.g. the admin panel can disconnect a player or send a global message.
/// The calls aren't addressed to the dapr app id of the game server, so game servers don't need a
/// unique app id. Calls which concern a player are handled by the game server which hosts the player,
/// the other calls carry the id of the game server, and the other game servers ignore them.
/// </remarks>
public sealed class GameServerClient : ManageableServerClient, IGameServer
{
    /// <summary>
    /// The topic of the <see cref="RestartAsync"/> command.
    /// </summary>
    public const string RestartTopic = "RestartAsync";

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
    public Task RestartAsync(CancellationToken cancellationToken = default)
    {
        return this.DaprClient.PublishCommandAsync(RestartTopic, this.Id, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask GuildChatMessageAsync(uint guildId, string sender, string message)
        => this.PublishAsync(nameof(this.GuildChatMessageAsync), new GuildMessageArguments(guildId, sender, message));

    /// <inheritdoc />
    public ValueTask GuildDeletedAsync(uint guildId)
        => this.PublishAsync(nameof(this.GuildDeletedAsync), guildId);

    /// <inheritdoc />
    public ValueTask GuildPlayerKickedAsync(string playerName)
        => this.PublishAsync(nameof(this.GuildPlayerKickedAsync), playerName);

    /// <inheritdoc />
    public ValueTask AllianceChatMessageAsync(uint guildId, string sender, string message)
        => this.PublishAsync(nameof(this.AllianceChatMessageAsync), new GuildMessageArguments(guildId, sender, message));

    /// <inheritdoc />
    public ValueTask WorldChatMessageAsync(string sender, string message)
        => this.PublishAsync(nameof(this.WorldChatMessageAsync), new WorldChatMessageArguments(sender, message));

    /// <inheritdoc />
    public ValueTask SendGlobalMessageAsync(string message, MessageType messageType)
        => this.PublishCommandAsync(nameof(this.SendGlobalMessageAsync), new MessageArguments(this.Id, message, messageType));

    /// <inheritdoc />
    /// <returns><c>true</c>, when the request got published. The result of the game server which hosts the player is unknown.</returns>
    public async ValueTask<bool> DisconnectPlayerAsync(string playerName)
    {
        await this.PublishCommandAsync(nameof(this.DisconnectPlayerAsync), playerName).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    /// <returns><c>true</c>, when the request got published. The result of the game server which hosts the account is unknown.</returns>
    public async ValueTask<bool> DisconnectAccountAsync(string accountName)
    {
        await this.PublishCommandAsync(nameof(this.DisconnectAccountAsync), accountName).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    /// <returns><c>true</c>, when the request got published. The result of the game server which hosts the player is unknown.</returns>
    public async ValueTask<bool> BanPlayerAsync(string playerName)
    {
        await this.PublishCommandAsync(nameof(this.BanPlayerAsync), playerName).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public ValueTask AssignGuildToPlayerAsync(string characterName, GuildMemberStatus guildStatus)
        => this.PublishAsync(nameof(this.AssignGuildToPlayerAsync), new GuildMemberAssignArguments(characterName, guildStatus));

    /// <inheritdoc />
    public ValueTask PlayerAlreadyLoggedInAsync(byte serverId, string loginName)
        => this.PublishAsync(nameof(this.PlayerAlreadyLoggedInAsync), new PlayerLoggedInArguments(serverId, loginName));

    /// <inheritdoc />
    public ValueTask AllianceCreatedAsync(uint masterGuildId, uint memberGuildId)
        => this.PublishAsync(nameof(this.AllianceCreatedAsync), new AllianceChangedArguments(masterGuildId, memberGuildId));

    /// <inheritdoc />
    public ValueTask AllianceDisbandedAsync(uint masterGuildId, uint memberGuildId)
        => this.PublishAsync(nameof(this.AllianceDisbandedAsync), new AllianceChangedArguments(masterGuildId, memberGuildId));

    /// <inheritdoc />
    public ValueTask GuildHostilityChangedAsync(uint guildIdA, IReadOnlyList<uint> allianceGuildIdsA, uint guildIdB, IReadOnlyList<uint> allianceGuildIdsB, bool created)
        => this.PublishAsync(nameof(this.GuildHostilityChangedAsync), new GuildHostilityChangedArguments(guildIdA, allianceGuildIdsA, guildIdB, allianceGuildIdsB, created));

    /// <inheritdoc />
    public ValueTask LetterReceivedAsync(LetterHeader letter)
        => this.PublishAsync(nameof(this.LetterReceivedAsync), letter);

    /// <inheritdoc />
    public ValueTask FriendRequestAsync(string requester, string receiver)
        => this.PublishAsync(nameof(this.FriendRequestAsync), new RequestArguments(requester, receiver));

    /// <inheritdoc />
    public ValueTask FriendOnlineStateChangedAsync(string player, string friend, int serverId)
        => this.PublishAsync(nameof(this.FriendOnlineStateChangedAsync), new FriendOnlineStateChangedArguments(player, friend, serverId));

    /// <inheritdoc />
    public ValueTask ChatRoomCreatedAsync(ChatServerAuthenticationInfo playerAuthenticationInfo, string friendName)
        => this.PublishAsync(nameof(this.ChatRoomCreatedAsync), new ChatRoomCreationArguments(playerAuthenticationInfo, friendName));

    /// <inheritdoc />
    public ValueTask InitializeMessengerAsync(MessengerInitializationData initializationData)
        => this.PublishAsync(nameof(this.InitializeMessengerAsync), initializationData);

    private async ValueTask PublishAsync<TData>(string topicName, TData data)
    {
        await this.DaprClient.PublishEventAsync(DaprClientExtensions.PubSubName, topicName, data).ConfigureAwait(false);
    }

    private async ValueTask PublishCommandAsync<TData>(string topicName, TData data)
    {
        await this.DaprClient.PublishCommandAsync(topicName, data).ConfigureAwait(false);
    }
}
