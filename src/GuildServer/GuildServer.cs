// <copyright file="GuildServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GuildServer;

using System.Collections.Concurrent;
using System.Collections.Immutable;
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

    /// <summary>
    /// The maximum number of guilds allowed in a single alliance.
    /// </summary>
    public const int MaxAllianceSize = 5;

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
    public async ValueTask<bool> CreateAllianceAsync(uint masterGuildId, uint targetGuildId)
    {
        if (!this._guildDictionary.TryGetValue(masterGuildId, out var masterContainer)
            || !this._guildDictionary.TryGetValue(targetGuildId, out var targetContainer))
        {
            return false;
        }

        // Target must not already be in an alliance
        if (targetContainer.Guild.AllianceGuild is not null)
        {
            return false;
        }

        // Check max alliance size
        var currentAllianceSize = this._guildDictionary.Values
            .Count(g => IsInSameAlliance(g.Guild, masterContainer.Guild));
        if (currentAllianceSize >= MaxAllianceSize)
        {
            return false;
        }

        try
        {
            // Set the master guild as alliance master (self-reference)
            if (masterContainer.Guild.AllianceGuild is null)
            {
                masterContainer.Guild.AllianceGuild = masterContainer.Guild;
                await masterContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
            }

            // In the target guild's context, load the master guild and set the relationship
            var masterGuildInTargetContext = await targetContainer.DatabaseContext
                .GetByIdAsync<Guild>(masterContainer.Guild.Id).ConfigureAwait(false);
            if (masterGuildInTargetContext is null)
            {
                return false;
            }

            targetContainer.Guild.AllianceGuild = masterGuildInTargetContext;
            await targetContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when creating an alliance.");
            return false;
        }

        await this._changePublisher.AllianceCreatedAsync(masterGuildId, targetGuildId).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async ValueTask<bool> RemoveAllianceGuildAsync(uint masterGuildId, uint targetGuildId)
    {
        if (!this._guildDictionary.TryGetValue(masterGuildId, out var masterContainer)
            || !this._guildDictionary.TryGetValue(targetGuildId, out var targetContainer))
        {
            return false;
        }

        // Only the alliance master can remove members
        if (!IsAllianceMaster(masterContainer.Guild))
        {
            return false;
        }

        // Verify target is actually in the master's alliance
        if (!IsInSameAlliance(targetContainer.Guild, masterContainer.Guild))
        {
            return false;
        }

        try
        {
            targetContainer.Guild.AllianceGuild = null;
            await targetContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when removing a guild from alliance.");
            return false;
        }

        await this._changePublisher.AllianceGuildRemovedAsync(masterGuildId, targetGuildId).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async ValueTask<bool> DisbandAllianceAsync(uint masterGuildId)
    {
        if (!this._guildDictionary.TryGetValue(masterGuildId, out var masterContainer))
        {
            return false;
        }

        if (!IsAllianceMaster(masterContainer.Guild))
        {
            return false;
        }

        try
        {
            // Clear AllianceGuild for all members in the alliance (including master)
            var allianceMembers = this._guildDictionary.Values
                .Where(g => IsInSameAlliance(g.Guild, masterContainer.Guild))
                .ToList();

            foreach (var member in allianceMembers)
            {
                member.Guild.AllianceGuild = null;
                await member.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when disbanding alliance.");
            return false;
        }

        await this._changePublisher.AllianceDisbandedAsync(masterGuildId).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async ValueTask<IImmutableList<AllianceGuildEntry>> GetAllianceGuildsAsync(uint guildId)
    {
        if (!this._guildDictionary.TryGetValue(guildId, out var guildContainer))
        {
            return ImmutableList<AllianceGuildEntry>.Empty;
        }

        if (guildContainer.Guild.AllianceGuild is null)
        {
            return ImmutableList<AllianceGuildEntry>.Empty;
        }

        var masterGuid = GetAllianceMasterGuid(guildContainer.Guild);
        if (masterGuid == Guid.Empty)
        {
            return ImmutableList<AllianceGuildEntry>.Empty;
        }

        return this._guildDictionary.Values
            .Where(g => GetAllianceMasterGuid(g.Guild) == masterGuid)
            .Select(g => new AllianceGuildEntry(g.Id, g.Guild.Name ?? string.Empty, g.Guild.Members.Count, g.Guild.Logo))
            .ToImmutableList();
    }

    /// <inheritdoc />
    public ValueTask<bool> IsAllianceMasterAsync(uint guildId)
    {
        if (!this._guildDictionary.TryGetValue(guildId, out var guildContainer))
        {
            return ValueTask.FromResult(false);
        }

        return ValueTask.FromResult(IsAllianceMaster(guildContainer.Guild));
    }

    /// <inheritdoc />
    public ValueTask<uint> GetAllianceMasterIdAsync(uint guildId)
    {
        if (!this._guildDictionary.TryGetValue(guildId, out var guildContainer)
            || guildContainer.Guild.AllianceGuild is null)
        {
            return ValueTask.FromResult(0u);
        }

        var masterGuid = GetAllianceMasterGuid(guildContainer.Guild);
        if (masterGuid == Guid.Empty)
        {
            return ValueTask.FromResult(0u);
        }

        // Find the uint ID of the alliance master
        var masterEntry = this._guildDictionary.FirstOrDefault(kvp => kvp.Value.Guild.Id == masterGuid);
        if (masterEntry.Value is null)
        {
            return ValueTask.FromResult(0u);
        }

        return ValueTask.FromResult(masterEntry.Key);
    }

    /// <inheritdoc />
    public async ValueTask<bool> SetHostilityAsync(uint guildId, uint targetGuildId, bool create)
    {
        if (!this._guildDictionary.TryGetValue(guildId, out var guildContainer))
        {
            return false;
        }

        try
        {
            if (create)
            {
                if (!this._guildDictionary.TryGetValue(targetGuildId, out var targetContainer))
                {
                    return false;
                }

                // Load target guild in requester's context to set the hostility FK correctly
                var targetGuildInRequesterContext = await guildContainer.DatabaseContext
                    .GetByIdAsync<Guild>(targetContainer.Guild.Id).ConfigureAwait(false);
                if (targetGuildInRequesterContext is null)
                {
                    return false;
                }

                guildContainer.Guild.Hostility = targetGuildInRequesterContext;
            }
            else
            {
                guildContainer.Guild.Hostility = null;
            }

            await guildContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when setting hostility.");
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public ValueTask<GuildRelationship> GetGuildRelationshipAsync(uint guild1Id, uint guild2Id)
    {
        if (!this._guildDictionary.TryGetValue(guild1Id, out var guild1Container)
            || !this._guildDictionary.TryGetValue(guild2Id, out var guild2Container))
        {
            return ValueTask.FromResult(GuildRelationship.None);
        }

        var guild1 = guild1Container.Guild;
        var guild2 = guild2Container.Guild;

        // Check if both guilds are in the same alliance
        if (guild1.AllianceGuild is not null && guild2.AllianceGuild is not null)
        {
            var master1Guid = GetAllianceMasterGuid(guild1);
            var master2Guid = GetAllianceMasterGuid(guild2);
            if (master1Guid != Guid.Empty && master1Guid == master2Guid)
            {
                return ValueTask.FromResult(GuildRelationship.Union);
            }
        }

        // Check hostility (transitively through alliances)
        if (HasHostility(guild1, guild2) || HasHostility(guild2, guild1))
        {
            return ValueTask.FromResult(GuildRelationship.Rival);
        }

        return ValueTask.FromResult(GuildRelationship.None);
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
        // If this guild is alliance master, disband the alliance first
        if (IsAllianceMaster(guildContainer.Guild))
        {
            await this.DisbandAllianceAsync(guildContainer.Id).ConfigureAwait(false);
        }
        else if (guildContainer.Guild.AllianceGuild is not null)
        {
            // Remove from alliance
            guildContainer.Guild.AllianceGuild = null;
            await guildContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        await guildContainer.DatabaseContext.DeleteAsync(guildContainer.Guild).ConfigureAwait(false);
        await guildContainer.DatabaseContext.SaveChangesAsync().ConfigureAwait(false);
        this.RemoveGuildContainer(guildContainer);

        await this._changePublisher.GuildDeletedAsync(guildContainer.Id).ConfigureAwait(false);

        // TODO: Inform gameServers that guildwar/hostility ended
    }

    private static bool IsAllianceMaster(Guild guild)
    {
        if (guild.AllianceGuild is null)
        {
            return false;
        }

        if (ReferenceEquals(guild.AllianceGuild, guild))
        {
            return true;
        }

        return guild.AllianceGuild is Guild allianceGuild && allianceGuild.Id == guild.Id;
    }

    private static Guid GetAllianceMasterGuid(Guild guild)
    {
        if (guild.AllianceGuild is not Guild masterGuild)
        {
            return Guid.Empty;
        }

        return masterGuild.Id;
    }

    private static bool IsInSameAlliance(Guild guild, Guild masterGuild)
    {
        if (guild.AllianceGuild is null)
        {
            return false;
        }

        var masterGuid = GetAllianceMasterGuid(masterGuild);
        if (masterGuid == Guid.Empty)
        {
            // masterGuild must itself be the master - compare by reference or ID
            return (guild.AllianceGuild is Guild allianceGuildOfMember
                    && allianceGuildOfMember.Id == masterGuild.Id)
                   || ReferenceEquals(guild.AllianceGuild, masterGuild);
        }

        return GetAllianceMasterGuid(guild) == masterGuid;
    }

    private bool HasHostility(Guild aggressor, Guild target)
    {
        if (aggressor.Hostility is null)
        {
            return false;
        }

        // Direct hostility
        if (aggressor.Hostility is Guild hostileGuild && hostileGuild.Id == target.Id)
        {
            return true;
        }

        // Transitive: if target is in an alliance, check if aggressor is hostile to any of the target's alliance members
        var targetMasterGuid = GetAllianceMasterGuid(target);
        if (targetMasterGuid == Guid.Empty)
        {
            return false;
        }

        if (aggressor.Hostility is not Guild hostileGuild2)
        {
            return false;
        }

        // Check if aggressor's hostility target is in the same alliance as target
        return this._guildDictionary.Values
            .Any(g => GetAllianceMasterGuid(g.Guild) == targetMasterGuid
                      && g.Guild.Id == hostileGuild2.Id);
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