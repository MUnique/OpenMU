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
        return await context.GuildWithNameExistsAsync(guildName);
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
            await guildContainer.DatabaseContext.SaveChangesAsync();
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

        if (await context.SaveChangesAsync())
        {
            var container = this.CreateGuildContainer(guild, context);
            container.SetServerId(masterId, serverId);
            container.Members[masterId].PlayerName = masterName;
            var status = new GuildMemberStatus(container.Id, GuildPosition.GuildMaster);
            await this._changePublisher.AssignGuildToPlayerAsync(serverId, masterName, status);
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

                await guild.DatabaseContext.SaveChangesAsync();
                guild.Members.Add(characterId, new GuildListEntry { PlayerName = characterName, PlayerPosition = guildMember.Status, ServerId = serverId });
                await this._changePublisher.AssignGuildToPlayerAsync(serverId, characterName, new GuildMemberStatus(guildId, guildMember.Status));
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
                    await guild.DatabaseContext.SaveChangesAsync();
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
        var guildMember = await tempContext.GetByIdAsync<GuildMember>(characterId); // we use the same id for Character.Id and GuildMemberInfo.Id
        if (guildMember is not null)
        {
            var guildId = await this.GuildMemberEnterGameAsync(guildMember.GuildId, guildMember.Id, serverId);
            var status = new GuildMemberStatus(guildId, guildMember.Status);
            await this._changePublisher.AssignGuildToPlayerAsync(serverId, characterName, status);
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
            this.DeleteGuild(guildContainer);
            return;
        }

        guildContainer.Members.Remove(kvp.Key);

        var guildMember = guildContainer.Guild.Members.FirstOrDefault(m => m.Id == kvp.Key);
        if (guildMember != null)
        {
            await guildContainer.DatabaseContext.DeleteAsync(guildMember);
            await guildContainer.DatabaseContext.SaveChangesAsync();
        }

        await this._changePublisher.GuildPlayerKickedAsync(playerName);
    }

    /// <inheritdoc />
    public async ValueTask<GuildPosition> GetGuildPositionAsync(Guid characterId)
    {
        using var tempContext = this._persistenceContextProvider.CreateNewGuildContext();
        var guildMember = await tempContext.GetByIdAsync<GuildMember>(characterId); // we use the same id for Character.Id and GuildMemberInfo.Id
        return guildMember?.Status ?? GuildPosition.Undefined;
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
    private void DeleteGuild(GuildContainer guildContainer)
    {
        guildContainer.DatabaseContext.DeleteAsync(guildContainer.Guild);
        guildContainer.DatabaseContext.SaveChanges();
        this.RemoveGuildContainer(guildContainer);

        this._changePublisher.GuildDeletedAsync(guildContainer.Id);

        // TODO: Inform gameServers that guildwar/hostility ended
    }

    private async ValueTask<uint> GuildMemberEnterGameAsync(Guid guildId, Guid characterId, byte serverId)
    {
        var guild = await this.GetOrCreateGuildContainerAsync(guildId);
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
            var guildinfo = await context.GetByIdAsync<Guild>(guildId);
            if (guildinfo is null)
            {
                this._logger.LogWarning("GuildMemberEnter: Guild {0} not found", guildId);
                context.Dispose();
                return null;
            }

            guild = this.CreateGuildContainer(guildinfo, context);
            await guild.LoadMemberNamesAsync();
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