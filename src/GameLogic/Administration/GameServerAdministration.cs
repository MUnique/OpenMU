// <copyright file="GameServerAdministration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Administration;

using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

/// <summary>Projects and changes live state without accessing another persistence context.</summary>
/// <param name="context">The owning game context.</param>
public sealed partial class GameServerAdministration(IGameContext context) : IGameServerAdministration
{
    /// <inheritdoc />
    public async ValueTask<LivePlayerPage> GetPlayersAsync(ushort? mapNumber, uint? guildId, int offset, int limit, CancellationToken cancellationToken)
    {
        if (offset < 0 || limit is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "Use offset >= 0 and limit between 1 and 100.");
        }

        var snapshot = await this.GetPlayerSnapshotAsync(cancellationToken).ConfigureAwait(false);
        var matches = snapshot.Where(p => (mapNumber is null || p.Map == mapNumber) && (guildId is null || p.GuildId == guildId)).ToList();
        return new(matches.OrderBy(p => p.Character, StringComparer.OrdinalIgnoreCase).Skip(offset).Take(limit).ToArray(), matches.Count);
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<LivePlayerInfo>> GetPlayerSnapshotAsync(CancellationToken cancellationToken)
    {
        var snapshot = new List<LivePlayerInfo>();
        foreach (var player in await context.GetPlayersAsync().ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (ReadPlayer(player) is { } info)
            {
                snapshot.Add(info);
            }
        }

        return snapshot;
    }

    /// <inheritdoc />
    public async ValueTask<LivePlayerInfo?> GetPlayerAsync(string characterName, CancellationToken cancellationToken)
        => context.GetPlayerByCharacterName(characterName) is { } player
            ? await this.ReadPlayerAsync(player, cancellationToken).ConfigureAwait(false) : null;

    /// <inheritdoc />
    public ValueTask<GameServerAdminResult> TeleportAsync(string characterName, Guid characterId, ushort mapNumber, byte x, byte y, CancellationToken cancellationToken)
        => this.WithPlayerAsync(characterName, characterId, p => GameMasterActions.TeleportAsync(p, mapNumber, x, y), cancellationToken);

    /// <inheritdoc />
    public ValueTask<GameServerAdminResult> GiveItemAsync(string characterName, Guid characterId, ItemSpecification item, CancellationToken cancellationToken)
        => this.WithPlayerAsync(characterName, characterId, p => GameMasterActions.GiveItemAsync(p, item), cancellationToken);

    /// <inheritdoc />
    public ValueTask<GameServerAdminResult> SetLevelAsync(string characterName, Guid characterId, int level, bool master, CancellationToken cancellationToken)
        => this.WithPlayerAsync(characterName, characterId, p => GameMasterActions.SetLevelAsync(p, level, master), cancellationToken);

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<LiveEventInfo>> GetEventsAsync(CancellationToken cancellationToken)
    {
        var events = new List<LiveEventInfo>();
        foreach (var definition in context.Configuration.MiniGameDefinitions.Take(100))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (context.PlugInManager.GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(definition.Type) is { } starter)
            {
                var startsIn = await starter.GetDurationUntilNextStartAsync(context, definition).ConfigureAwait(false);
                if (!starter.IsEventActive(context) && starter.IsEnabledOn(context) && starter is IScheduledPlugIn scheduled)
                {
                    var now = DateTime.UtcNow;
                    startsIn = scheduled.GetNextStartUtc(now, context.ServerTimeZone) - now;
                }

                events.Add(new(definition.Type.ToString(), definition.Name, starter.IsEnabledOn(context), starter.IsEventActive(context), startsIn, GameMasterEventActions.GetParticipantCount(context, definition.Type)));
            }
        }

        return events;
    }

    /// <inheritdoc />
    public ValueTask<GameServerAdminResult> StartEventAsync(string eventType, bool restart, LiveEventInfo expectedEvent, CancellationToken cancellationToken)
        => Enum.TryParse<MiniGameType>(eventType, false, out var type) && Enum.IsDefined(type)
            ? GameMasterEventActions.StartAsync(context, type, restart, cancellationToken, expectedEvent)
            : ValueTask.FromResult(new GameServerAdminResult(false, "Unknown event type. Use events_schedule to find its invariant name."));

    private static bool IsAvailable(Player player) => player is not OfflinePlayer && player.IsConnected
        && player.PlayerState.CurrentState == PlayerState.EnteredWorld && player.SelectedCharacter is not null && player.Account is not null;

    private static LivePlayerInfo? ReadPlayer(Player player)
    {
        if (!IsAvailable(player) || player.SelectedCharacter is not { } character || player.Account is not { } account)
        {
            return null;
        }

        // List projections are best-effort scalar snapshots. Saves must not stall population reads.
        var position = player.Position;
        var attributes = player.Attributes;
        var info = new LivePlayerInfo(character.Id, account.LoginName, character.Name, player.CurrentMap?.MapId, position.X, position.Y, (int)(attributes?[Stats.Level] ?? 0), (int)(attributes?[Stats.MasterLevel] ?? 0), player.GuildStatus?.GuildId, character.CharacterStatus == CharacterStatus.GameMaster || account.State is not (AccountState.Normal or AccountState.Banned or AccountState.TemporarilyBanned));
        return ReferenceEquals(player.SelectedCharacter, character) && ReferenceEquals(player.Account, account) && IsAvailable(player) ? info : null;
    }

    private async ValueTask<LivePlayerInfo?> ReadPlayerAsync(Player player, CancellationToken cancellationToken)
        => await player.RunPersistenceExclusiveAsync<LivePlayerInfo?>(() => ValueTask.FromResult(ReadPlayer(player)), cancellationToken).ConfigureAwait(false);

    private async ValueTask<GameServerAdminResult> WithPlayerAsync(string name, Guid id, Func<Player, ValueTask<GameServerAdminResult>> action, CancellationToken cancellationToken)
    {
        if (context.GetPlayerByCharacterName(name) is not { } player)
        {
            return new(false, "The character is no longer online on this server. Refresh players_list; no change was made.");
        }

        // Recheck identity/state only after acquiring the same lock as packet handlers and saves.
        return await player.RunPersistenceExclusiveAsync(
            async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!IsAvailable(player) || player.SelectedCharacter is not { } character || character.Id != id
                    || !string.Equals(character.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return new GameServerAdminResult(false, "The live character changed. Refresh players_list and request a new preview.");
                }

                return await action(player).ConfigureAwait(false);
            },
            cancellationToken).ConfigureAwait(false);
    }
}
