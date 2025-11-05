// <copyright file="GuildServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Immutable;

namespace MUnique.OpenMU.GuildServer;

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using Guild = MUnique.OpenMU.DataModel.Entities.Guild;
using GuildMember = MUnique.OpenMU.DataModel.Entities.GuildMember;

/// <summary>
/// GuildServer which is managing the guilds. All Guilds of online guild members will be cached here.
/// The server needs to be aware when characters leave or enter the world, so a connection to a
/// GameServer is required.
/// </summary>
public class GuildServer : IGuildServer
{
    /// <summary>
    /// The offline server identifier.
    /// </summary>
    public static readonly byte OfflineServerId = 0xFF;

    private readonly ILogger<GuildServer> _logger;

    private readonly IDictionary<uint, GuildContainer> _guildDictionary;
    private readonly IDictionary<Guid, uint> _guildIdMapping;
    private readonly IdGenerator _idGenerator;
    private readonly IGuildChangePublisher _changePublisher;
    private readonly IPersistenceContextProvider _persistenceContextProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildServer" /> class.
    /// </summary>
    /// <param name="changePublisher">The change publisher.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="logger">The logger.</param>
    public GuildServer(IGuildChangePublisher changePublisher, IPersistenceContextProvider persistenceContextProvider, ILogger<GuildServer> logger)
    {
        this._changePublisher = changePublisher;
        this._persistenceContextProvider = persistenceContextProvider;
        this._logger = logger;
        this._guildDictionary = new ConcurrentDictionary<uint, GuildContainer>();
        this._idGenerator = new IdGenerator(1, int.MaxValue)
        {
            ReUseSetting = IdGenerator.ReUsePolicy.ReUseWhenExceeded,
        };
        this._guildIdMapping = new ConcurrentDictionary<Guid, uint>();
    }

    /// <inheritdoc/>
    public async ValueTask<bool> GuildExistsAsync(string guildName)
    {
        using var context = this._persistenceContextProvider.CreateNewGuildContext();
        return await context.GuildWithNameExistsAsync(guildName).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<Interfaces.Guild?> GetGuildAsync(uint guildId)
    {
        if (this._guildDictionary.TryGetValue(guildId, out var guild))
        {
            return guild.Guild;
        }

        return null;
    }

    /// <inheritdoc/>
    public async ValueTask<uint> GetGuildIdByNameAsync(string guildName)
    {
        var guild = this._guildDictionary
            .FirstOrDefault(x => x.Value.Guild.Name!.Equals(guildName, StringComparison.OrdinalIgnoreCase));

        return guild.Key;
    }

    /// <inheritdoc />
    public async ValueTask IncreaseGuildScoreAsync(uint guildId)
    {
        if (this._guildDictionary.TryGetValue(guildId, out var guildContainer))
        {
            guildContainer.Guild.Score++;
            await guildContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask<bool> CreateGuildAsync(string name, string masterName, Guid masterId, byte[] logo, byte serverId)
    {
        var context = this._persistenceContextProvider.CreateNewGuildContext();

        var guild = context.CreateNew<Guild>();
        guild.Name = name;
        guild.Logo = logo;

        var masterGuildMemberInfo = context.CreateNew<GuildMember>(masterId);
        masterGuildMemberInfo.Status = GuildPosition.GuildMaster;
        masterGuildMemberInfo.GuildId = guild.Id;
        guild.Members.Add(masterGuildMemberInfo);

        if (await context.SaveChangesAsync().ConfigureAwait(false))
        {
            var container = this.CreateGuildContainer(guild, context);
            container.SetServerId(masterId, serverId);
            container.Members[masterId].PlayerName = masterName;
            var status = new GuildMemberStatus(container.Id, GuildPosition.GuildMaster);
            await this._changePublisher.AssignGuildToPlayerAsync(serverId, masterName, status).ConfigureAwait(false);
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public async ValueTask CreateGuildMemberAsync(uint guildId, Guid characterId, string characterName, GuildPosition role, byte serverId)
    {
        try
        {
            if (this._guildDictionary.TryGetValue(guildId, out var guild))
            {
                if (guild.Members.ContainsKey(characterId))
                {
                    this._logger.LogWarning("Guildmember already exists: {0}", characterName);
                    return;
                }

                var guildMember = guild.DatabaseContext.CreateNew<GuildMember>();
                guildMember.Id = characterId;
                guildMember.Status = role;
                guildMember.GuildId = guild.Guild.Id;
                guild.Guild.Members.Add(guildMember);

                await guild.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
                guild.Members.Add(characterId, new GuildListEntry { PlayerName = characterName, PlayerPosition = guildMember.Status, ServerId = serverId });
                await this._changePublisher.AssignGuildToPlayerAsync(serverId, characterName, new GuildMemberStatus(guildId, guildMember.Status)).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            // Rollback?
            this._logger.LogError(ex, "Error when creating a guild member.");
        }
    }

    /// <inheritdoc/>
    public async ValueTask ChangeGuildMemberPositionAsync(uint guildId, Guid characterId, GuildPosition role)
    {
        try
        {
            if (this._guildDictionary.TryGetValue(guildId, out var guild))
            {
                var guildMember = guild.Guild.Members.FirstOrDefault(m => m.Id == characterId);
                if (guildMember != null)
                {
                    guildMember.Status = role;
                    await guild.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
                    var listEntry = guild.Members[characterId];
                    listEntry.PlayerPosition = role;
                }
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when saving a changed guild member.");
        }
    }

    /// <inheritdoc />
    public async ValueTask PlayerEnteredGameAsync(Guid characterId, string characterName, byte serverId)
    {
        using var tempContext = this._persistenceContextProvider.CreateNewGuildContext();
        var guildMember = await tempContext.GetByIdAsync<GuildMember>(characterId).ConfigureAwait(false); // we use the same id for Character.Id and GuildMemberInfo.Id
        if (guildMember is not null)
        {
            var guildId = await this.GuildMemberEnterGameAsync(guildMember.GuildId, guildMember.Id, serverId).ConfigureAwait(false);
            var status = new GuildMemberStatus(guildId, guildMember.Status);
            await this._changePublisher.AssignGuildToPlayerAsync(serverId, characterName, status).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public ValueTask GuildMemberLeftGameAsync(uint guildId, Guid guildMemberId, byte serverId)
    {
        if (this._guildDictionary.TryGetValue(guildId, out var guild))
        {
            guild.SetServerId(guildMemberId, OfflineServerId);
            if (guild.Members.Values.All(member => member.ServerId == OfflineServerId))
            {
                this.RemoveGuildContainer(guild);
            }
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask<IImmutableList<GuildListEntry>> GetGuildListAsync(uint guildId)
    {
        if (!this._guildDictionary.TryGetValue(guildId, out var guildContainer))
        {
            return ImmutableList<GuildListEntry>.Empty;
        }

        return guildContainer.Members.Values.ToImmutableList();
    }

    /// <inheritdoc/>
    public async ValueTask KickMemberAsync(uint guildId, string playerName)
    {
        if (!this._guildDictionary.TryGetValue(guildId, out var guildContainer))
        {
            this._logger.LogWarning($"Guild {guildId} not found, so Player {playerName} can't be kicked.");
            return;
        }

        var kvp = guildContainer.Members.FirstOrDefault(m => m.Value.PlayerName == playerName);
        if (default(KeyValuePair<Guid, GuildListEntry>).Equals(kvp))
        {
            this._logger.LogWarning($"Guild {guildId} and Player {playerName} not found, so it can't be kicked.");
            return;
        }

        var member = kvp.Value;
        if (member.PlayerPosition == GuildPosition.GuildMaster)
        {
            await this.DeleteGuildAsync(guildContainer).ConfigureAwait(false);
            return;
        }

        guildContainer.Members.Remove(kvp.Key);

        var guildMember = guildContainer.Guild.Members.FirstOrDefault(m => m.Id == kvp.Key);
        if (guildMember != null)
        {
            await guildContainer.DatabaseContext.DeleteAsync(guildMember).ConfigureAwait(false);
            await guildContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        await this._changePublisher.GuildPlayerKickedAsync(playerName).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<GuildPosition> GetGuildPositionAsync(Guid characterId)
    {
        using var tempContext = this._persistenceContextProvider.CreateNewGuildContext();
        var guildMember = await tempContext.GetByIdAsync<GuildMember>(characterId).ConfigureAwait(false); // we use the same id for Character.Id and GuildMemberInfo.Id
        return guildMember?.Status ?? GuildPosition.Undefined;
    }

    /// <inheritdoc />
    public async ValueTask<bool> CreateAllianceAsync(uint requestingGuildId, uint targetGuildId)
    {
        if (!this._guildDictionary.TryGetValue(requestingGuildId, out var requestingContainer) ||
            !this._guildDictionary.TryGetValue(targetGuildId, out var targetContainer))
        {
            this._logger.LogWarning($"One of the guilds not found for alliance creation: {requestingGuildId}, {targetGuildId}");
            return false;
        }

        // Check if target guild is already in an alliance
        if (targetContainer.Guild.AllianceGuild is not null)
        {
            this._logger.LogWarning($"Target guild {targetGuildId} is already in an alliance");
            return false;
        }

        // Check if target guild has hostility
        if (targetContainer.Guild.Hostility is not null)
        {
            this._logger.LogWarning($"Target guild {targetGuildId} has a hostility relationship");
            return false;
        }

        // Determine the alliance master
        Guild allianceMaster;
        if (requestingContainer.Guild.AllianceGuild is null)
        {
            // Requesting guild is not in an alliance, so it becomes the master
            allianceMaster = requestingContainer.Guild;
        }
        else
        {
            // Requesting guild is already in an alliance, use its alliance master
            allianceMaster = requestingContainer.Guild.AllianceGuild;
        }

        // Check alliance size limit (1 master + max 3 members = 4 total)
        var currentAllianceSize = this._guildDictionary.Values.Count(g => g.Guild.AllianceGuild?.Id == allianceMaster.Id || g.Guild.Id == allianceMaster.Id);
        if (currentAllianceSize >= 4)
        {
            this._logger.LogWarning($"Alliance is full. Master: {allianceMaster.Name}");
            return false;
        }

        // Add target guild to alliance
        targetContainer.Guild.AllianceGuild = allianceMaster;
        await targetContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

        this._logger.LogInformation($"Guild {targetContainer.Guild.Name} joined alliance with master {allianceMaster.Name}");
        return true;
    }

    /// <inheritdoc />
    public async ValueTask<bool> RemoveFromAllianceAsync(uint guildId)
    {
        if (!this._guildDictionary.TryGetValue(guildId, out var guildContainer))
        {
            this._logger.LogWarning($"Guild {guildId} not found for alliance removal");
            return false;
        }

        if (guildContainer.Guild.AllianceGuild is null)
        {
            // Guild is not in an alliance, but check if it's a master with members
            var membersInAlliance = this._guildDictionary.Values
                .Where(g => g.Guild.AllianceGuild?.Id == guildContainer.Guild.Id)
                .ToList();

            if (membersInAlliance.Any())
            {
                // This guild is an alliance master with members, remove all members
                foreach (var member in membersInAlliance)
                {
                    member.Guild.AllianceGuild = null;
                    await member.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
                }

                this._logger.LogInformation($"Alliance master {guildContainer.Guild.Name} disbanded alliance, removing {membersInAlliance.Count} members");
            }

            return true;
        }

        // Remove guild from alliance
        guildContainer.Guild.AllianceGuild = null;
        await guildContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

        this._logger.LogInformation($"Guild {guildContainer.Guild.Name} left alliance");
        return true;
    }

    /// <inheritdoc />
    public async ValueTask<IImmutableList<uint>> GetAllianceMemberGuildIdsAsync(uint guildId)
    {
        if (!this._guildDictionary.TryGetValue(guildId, out var guildContainer))
        {
            return ImmutableList<uint>.Empty;
        }

        var allianceMasterId = guildContainer.Guild.AllianceGuild?.Id ?? guildContainer.Guild.Id;

        // Get all guilds in the same alliance (including the master)
        var allianceGuilds = this._guildDictionary
            .Where(kvp => kvp.Value.Guild.Id == allianceMasterId || kvp.Value.Guild.AllianceGuild?.Id == allianceMasterId)
            .Select(kvp => kvp.Key)
            .ToImmutableList();

        return allianceGuilds;
    }

    /// <inheritdoc />
    public async ValueTask<uint> GetAllianceMasterGuildIdAsync(uint guildId)
    {
        if (!this._guildDictionary.TryGetValue(guildId, out var guildContainer))
        {
            return 0;
        }

        if (guildContainer.Guild.AllianceGuild is not null)
        {
            // This guild is a member, find the master's short ID
            var masterId = guildContainer.Guild.AllianceGuild.Id;
            var masterContainer = this._guildDictionary.FirstOrDefault(kvp => kvp.Value.Guild.Id == masterId);
            return masterContainer.Key;
        }

        // Check if this guild is itself a master
        var hasMembers = this._guildDictionary.Values.Any(g => g.Guild.AllianceGuild?.Id == guildContainer.Guild.Id);
        return hasMembers ? guildId : 0;
    }

    /// <inheritdoc />
    public async ValueTask<bool> CreateHostilityAsync(uint requestingGuildId, uint targetGuildId)
    {
        if (!this._guildDictionary.TryGetValue(requestingGuildId, out var requestingContainer) ||
            !this._guildDictionary.TryGetValue(targetGuildId, out var targetContainer))
        {
            this._logger.LogWarning($"One of the guilds not found for hostility creation: {requestingGuildId}, {targetGuildId}");
            return false;
        }

        // Check if either guild is in an alliance (mutual exclusivity rule)
        if (requestingContainer.Guild.AllianceGuild is not null)
        {
            this._logger.LogWarning($"Requesting guild {requestingGuildId} is in an alliance and cannot have hostility");
            return false;
        }

        if (targetContainer.Guild.AllianceGuild is not null)
        {
            this._logger.LogWarning($"Target guild {targetGuildId} is in an alliance and cannot have hostility");
            return false;
        }

        // Check if hostility already exists
        if (requestingContainer.Guild.Hostility?.Id == targetContainer.Guild.Id)
        {
            return true; // Already hostile
        }

        // Create bidirectional hostility
        requestingContainer.Guild.Hostility = targetContainer.Guild;
        targetContainer.Guild.Hostility = requestingContainer.Guild;

        await requestingContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
        await targetContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

        this._logger.LogInformation($"Hostility created between {requestingContainer.Guild.Name} and {targetContainer.Guild.Name}");
        return true;
    }

    /// <inheritdoc />
    public async ValueTask<bool> RemoveHostilityAsync(uint guildId)
    {
        if (!this._guildDictionary.TryGetValue(guildId, out var guildContainer))
        {
            this._logger.LogWarning($"Guild {guildId} not found for hostility removal");
            return false;
        }

        if (guildContainer.Guild.Hostility is null)
        {
            return true; // No hostility to remove
        }

        // Find the hostile guild and remove bidirectional hostility
        var hostileGuildId = guildContainer.Guild.Hostility.Id;
        var hostileContainer = this._guildDictionary.Values.FirstOrDefault(g => g.Guild.Id == hostileGuildId);

        guildContainer.Guild.Hostility = null;
        await guildContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

        if (hostileContainer is not null)
        {
            hostileContainer.Guild.Hostility = null;
            await hostileContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        this._logger.LogInformation($"Hostility removed for guild {guildContainer.Guild.Name}");

        // Notify game servers that the hostility ended
        var shortGuildId1 = guildContainer.Id;
        var shortGuildId2 = hostileContainer?.Id ?? 0;
        if (shortGuildId2 > 0)
        {
            await this._changePublisher.GuildWarEndedAsync(shortGuildId1, shortGuildId2).ConfigureAwait(false);
        }

        return true;
    }

    /// <summary>
    /// Removes a guild from the server and the database.
    /// First we are trying to get the guild out of our dictionary.
    /// We are assuming that all guilds are in the dictionary, because
    /// we are holding all guilds of all ingame-characters in it.
    /// So this method is only called usefully from the gameserver itself,
    /// by player interaction.
    /// </summary>
    /// <param name="guildContainer">The container of the guild which should be deleted.</param>
    private async ValueTask DeleteGuildAsync(GuildContainer guildContainer)
    {
        // Check if the guild has a hostility and notify game servers before deletion
        uint? hostileGuildId = null;
        if (guildContainer.Guild.Hostility is not null)
        {
            var hostileGuild = this._guildDictionary.Values.FirstOrDefault(g => g.Guild.Id == guildContainer.Guild.Hostility.Id);
            hostileGuildId = hostileGuild?.Id;
        }

        await guildContainer.DatabaseContext.DeleteAsync(guildContainer.Guild).ConfigureAwait(false);
        await guildContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
        this.RemoveGuildContainer(guildContainer);

        await this._changePublisher.GuildDeletedAsync(guildContainer.Id).ConfigureAwait(false);

        // Notify game servers that the guild war/hostility ended (if any existed)
        if (hostileGuildId.HasValue)
        {
            await this._changePublisher.GuildWarEndedAsync(guildContainer.Id, hostileGuildId.Value).ConfigureAwait(false);
        }
    }

    private async ValueTask<uint> GuildMemberEnterGameAsync(Guid guildId, Guid characterId, byte serverId)
    {
        var guild = await this.GetOrCreateGuildContainerAsync(guildId).ConfigureAwait(false);
        if (guild is null)
        {
            throw new ArgumentException($"Guild not found. Id: {guildId}", nameof(guildId));
        }

        guild.SetServerId(characterId, serverId);
        return guild.Id;
    }

    private async ValueTask<GuildContainer?> GetOrCreateGuildContainerAsync(Guid guildId)
    {
        if (!this._guildIdMapping.TryGetValue(guildId, out var shortGuildId) || !this._guildDictionary.TryGetValue(shortGuildId, out var guild))
        {
            var context = this._persistenceContextProvider.CreateNewGuildContext();
            var guildinfo = await context.GetByIdAsync<Guild>(guildId).ConfigureAwait(false);
            if (guildinfo is null)
            {
                this._logger.LogWarning("GuildMemberEnter: Guild {0} not found", guildId);
                context.Dispose();
                return null;
            }

            guild = this.CreateGuildContainer(guildinfo, context);
            await guild.LoadMemberNamesAsync().ConfigureAwait(false);
        }

        return guild;
    }

    private GuildContainer CreateGuildContainer(Guild guild, IGuildServerContext databaseContext)
    {
        var id = (uint)this._idGenerator.GenerateId();

        var guildContainer = new GuildContainer(guild, id, databaseContext);
        this._guildDictionary.Add(id, guildContainer);
        this._guildIdMapping.Add(guild.Id, id);

        return guildContainer;
    }

    private void RemoveGuildContainer(GuildContainer guildContainer)
    {
        this._guildDictionary.Remove(guildContainer.Id);
        this._guildIdMapping.Remove(guildContainer.Guild.Id);
        this._idGenerator.GiveBack((int)guildContainer.Id);
        guildContainer.DatabaseContext.Dispose();
    }
}