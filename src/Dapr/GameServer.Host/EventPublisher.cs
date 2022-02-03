using Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

namespace MUnique.OpenMU.GameServer.Host;

public class EventPublisher : IEventPublisher
{
    private const string PubSubName = "pubsub";
    private readonly DaprClient _daprClient;
    private readonly ILogger<GameServerStatePublisher> _logger;

    public EventPublisher(DaprClient daprClient, ILogger<GameServerStatePublisher> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public void PlayerEnteredGame(byte serverId, Guid characterId, string characterName)
    {
        try
        {
            this._daprClient.PublishEventAsync(PubSubName, nameof(IEventPublisher.PlayerEnteredGame), new PlayerOnlineStateArguments(characterId, characterName, serverId));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing a guild message");
        }
    }

    public void PlayerLeftGame(byte serverId, Guid characterId, string characterName, uint guildId = 0)
    {
        try
        {
            this._daprClient.PublishEventAsync(PubSubName, nameof(IEventPublisher.PlayerLeftGame), new PlayerOnlineStateArguments(characterId, characterName, serverId, guildId));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing a guild message");
        }
    }

    public void GuildMessage(uint guildId, string sender, string message)
    {
        try
        {
            this._daprClient.PublishEventAsync(PubSubName, nameof(IGameServer.GuildChatMessage), new GuildMessageArguments(guildId, sender, message));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing a guild message");
        }
    }

    public void AllianceMessage(uint guildId, string sender, string message)
    {
        try
        {
            this._daprClient.PublishEventAsync(PubSubName, nameof(IGameServer.AllianceChatMessage), new GuildMessageArguments(guildId, sender, message));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing an alliance message");
        }
    }
}