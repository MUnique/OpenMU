// <copyright file="GameEventPublisherPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.GameEvents;

using System.Reflection;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Publishes events of the game as <see cref="GameEvent"/>s to the whole server, through <see cref="IEventPublisher.GameEventAsync"/>.
/// This allows external systems, like a Discord integration, to react on them.
/// </summary>
/// <remarks>
/// Global notices are published when a game master sends them by chat, with the <c>!</c> prefix.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.GameEventPublisherPlugIn_Name), Description = nameof(PlugInResources.GameEventPublisherPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("B951E351-B258-49E7-9783-9C4B8ADC7B19")]
public class GameEventPublisherPlugIn
    : IMiniGameEntranceOpenedPlugIn,
    IMiniGameStartedPlugIn,
    IMiniGameEndedPlugIn,
    IInvasionStartedPlugIn,
    IInvasionEndedPlugIn,
    ICastleSiegeStateChangedPlugIn,
    IMonsterItemDroppedPlugIn,
    IChatMessageSentPlugIn,
    IAttackableGotKilledPlugIn,
    ICharacterLevelUpPlugIn,
    ICharacterMasterLevelUpPlugIn,
    ISupportCustomConfiguration<GameEventPublisherConfiguration>,
    ISupportDefaultCustomConfiguration,
    IDisabledByDefault
{
    /// <inheritdoc />
    public GameEventPublisherConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new GameEventPublisherConfiguration();

    /// <inheritdoc />
    public ValueTask MiniGameEntranceOpenedAsync(MiniGameContext miniGame)
    {
        if (this.Configuration?.PublishMiniGameEvents is not true
            || miniGame.GameContext is not IGameServerContext context)
        {
            return ValueTask.CompletedTask;
        }

        var definition = miniGame.Definition;
        return context.EventPublisher.GameEventAsync(new MiniGameEntranceOpenedEvent(
            context.Id,
            DateTime.UtcNow,
            definition.Type.ToString(),
            definition.Name,
            definition.GameLevel,
            miniGame.EnterEndsAtUtc));
    }

    /// <inheritdoc />
    public ValueTask MiniGameStartedAsync(MiniGameContext miniGame, IReadOnlyCollection<Player> players)
    {
        if (this.Configuration?.PublishMiniGameEvents is not true
            || miniGame.GameContext is not IGameServerContext context)
        {
            return ValueTask.CompletedTask;
        }

        var definition = miniGame.Definition;
        return context.EventPublisher.GameEventAsync(new MiniGameStartedEvent(
            context.Id,
            DateTime.UtcNow,
            definition.Type.ToString(),
            definition.Name,
            definition.GameLevel,
            players.Count));
    }

    /// <inheritdoc />
    public ValueTask MiniGameEndedAsync(MiniGameContext miniGame, Player? winner, IReadOnlyCollection<Player> finishers)
    {
        if (this.Configuration?.PublishMiniGameEvents is not true
            || miniGame.GameContext is not IGameServerContext context)
        {
            return ValueTask.CompletedTask;
        }

        var definition = miniGame.Definition;
        return context.EventPublisher.GameEventAsync(new MiniGameEndedEvent(
            context.Id,
            DateTime.UtcNow,
            definition.Type.ToString(),
            definition.Name,
            definition.GameLevel,
            winner?.Name,
            finishers.Select(player => player.Name).ToList()));
    }

    /// <inheritdoc />
    public ValueTask InvasionStartedAsync(IGameContext gameContext, IPeriodicTaskPlugIn invasion, IReadOnlyCollection<GameMapDefinition> maps)
    {
        if (this.Configuration?.PublishInvasionEvents is not true
            || gameContext is not IGameServerContext context)
        {
            return ValueTask.CompletedTask;
        }

        var (id, name) = GetInvasionIdentity(invasion);
        return context.EventPublisher.GameEventAsync(new InvasionStartedEvent(context.Id, DateTime.UtcNow, id, name, GetMapNames(maps)));
    }

    /// <inheritdoc />
    public ValueTask InvasionEndedAsync(IGameContext gameContext, IPeriodicTaskPlugIn invasion, IReadOnlyCollection<GameMapDefinition> maps)
    {
        if (this.Configuration?.PublishInvasionEvents is not true
            || gameContext is not IGameServerContext context)
        {
            return ValueTask.CompletedTask;
        }

        var (id, name) = GetInvasionIdentity(invasion);
        return context.EventPublisher.GameEventAsync(new InvasionEndedEvent(context.Id, DateTime.UtcNow, id, name, GetMapNames(maps)));
    }

    /// <inheritdoc />
    public ValueTask CastleSiegeStateChangedAsync(IGameContext gameContext, CastleSiegeContext castleSiege, CastleSiegeState previousState)
    {
        if (this.Configuration?.PublishCastleSiegeEvents is not true
            || gameContext is not IGameServerContext context)
        {
            return ValueTask.CompletedTask;
        }

        return context.EventPublisher.GameEventAsync(new CastleSiegeStateChangedEvent(
            context.Id,
            DateTime.UtcNow,
            previousState.ToString(),
            castleSiege.CurrentState.ToString(),
            castleSiege.StateEndTimeUtc,
            castleSiege.SiegeData?.OwnerGuildId));
    }

    /// <inheritdoc />
    public ValueTask MonsterItemDroppedAsync(AttackableNpcBase monster, Player killer, DroppedItem droppedItem)
    {
        // This is called for every dropped item, so the cheap checks come first.
        if (this.Configuration is not { } configuration
            || !(configuration.PublishExcellentItemDrops || configuration.PublishAncientItemDrops)
            || killer.GameContext is not IGameServerContext context)
        {
            return ValueTask.CompletedTask;
        }

        var item = droppedItem.Item;
        var isExcellent = item.IsExcellent();
        var isAncient = item.IsAncient();
        if (!((isExcellent && configuration.PublishExcellentItemDrops) || (isAncient && configuration.PublishAncientItemDrops)))
        {
            return ValueTask.CompletedTask;
        }

        return context.EventPublisher.GameEventAsync(new MonsterItemDroppedEvent(
            context.Id,
            DateTime.UtcNow,
            killer.Name,
            monster.Definition.Designation,
            monster.CurrentMap.Definition.Name,
            item.Definition?.Name ?? string.Empty,
            item.Level,
            isExcellent,
            isAncient));
    }

    /// <inheritdoc />
    public ValueTask ChatMessageSentAsync(Player sender, string message, ChatMessageType messageType, Player? receiver)
    {
        if (sender.GameContext is not IGameServerContext context)
        {
            return ValueTask.CompletedTask;
        }

        if (messageType == ChatMessageType.GlobalNotification)
        {
            return this.Configuration?.PublishGlobalNotices is true
                ? context.EventPublisher.GameEventAsync(new GlobalNoticeEvent(context.Id, DateTime.UtcNow, sender.Name, message.TrimStart('!')))
                : ValueTask.CompletedTask;
        }

        GameChatChannel? channel = messageType switch
        {
            ChatMessageType.Guild => GameChatChannel.Guild,
            ChatMessageType.Alliance => GameChatChannel.Alliance,
            ChatMessageType.World => GameChatChannel.World,
            _ => null,
        };
        if (channel is null || this.Configuration?.PublishChatMessages is not true)
        {
            return ValueTask.CompletedTask;
        }

        // The guild and alliance messages still contain their prefix, @ or @@.
        return context.EventPublisher.GameEventAsync(new ChatMessageEvent(
            context.Id,
            DateTime.UtcNow,
            channel.Value,
            sender.GuildStatus?.GuildId ?? 0,
            sender.Name,
            channel == GameChatChannel.World ? message : message.TrimStart('@')));
    }

    /// <inheritdoc />
    public ValueTask AttackableGotKilledAsync(IAttackable killed, IAttacker? killer)
    {
        if (killed is not Monster monster
            || this.Configuration is not { PublishBossKills: true } configuration
            || !configuration.BossMonsterNumbers.Contains(monster.Definition.Number))
        {
            return ValueTask.CompletedTask;
        }

        // A summoned monster kills on behalf of its owner.
        var player = killer as Player ?? (killer as Monster)?.SummonedBy;
        if (player?.GameContext is not IGameServerContext context)
        {
            return ValueTask.CompletedTask;
        }

        return context.EventPublisher.GameEventAsync(new BossKilledEvent(
            context.Id,
            DateTime.UtcNow,
            player.Name,
            monster.Definition.Designation,
            monster.CurrentMap.Definition.Name));
    }

    /// <inheritdoc />
    public void CharacterLeveledUp(Player player)
    {
        if (this.Configuration is not { PublishLevelMilestones: true } configuration
            || player.Attributes is not { } attributes)
        {
            return;
        }

        var level = (int)attributes[Stats.Level];
        if (configuration.LevelMilestones.Contains(level))
        {
            // The plugin point is synchronous, so the event is published in the background.
            _ = PublishMilestoneAsync(player, level, false);
        }
    }

    /// <inheritdoc />
    public ValueTask CharacterMasterLeveledUpAsync(Player player)
    {
        if (this.Configuration is not { PublishLevelMilestones: true } configuration
            || player.Attributes is not { } attributes)
        {
            return ValueTask.CompletedTask;
        }

        var level = (int)attributes[Stats.MasterLevel];
        return configuration.MasterLevelMilestones.Contains(level)
            ? new ValueTask(PublishMilestoneAsync(player, level, true))
            : ValueTask.CompletedTask;
    }

    private static async Task PublishMilestoneAsync(Player player, int level, bool isMasterLevel)
    {
        if (player.GameContext is not IGameServerContext context
            || player.SelectedCharacter is not { } character)
        {
            return;
        }

        try
        {
            await context.EventPublisher.GameEventAsync(new CharacterLevelMilestoneEvent(
                context.Id,
                DateTime.UtcNow,
                character.Name,
                (string?)character.CharacterClass?.Name ?? string.Empty,
                level,
                isMasterLevel)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Error when publishing the level milestone of {character}", character.Name);
        }
    }

    private static (Guid Id, string Name) GetInvasionIdentity(IPeriodicTaskPlugIn invasion)
    {
        var type = invasion.GetType();
        var name = type.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? type.Name;
        return (type.GUID, name);
    }

    private static List<string> GetMapNames(IEnumerable<GameMapDefinition> maps)
    {
        return maps.Select(map => (string)map.Name).ToList();
    }
}
