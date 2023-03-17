// <copyright file="GuildTestBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GuildServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Base class for guild related tests.
/// </summary>
public class GuildTestBase
{
    protected const string GuildName = "Foobar";

    /// <summary>
    /// Gets or sets the first game server.
    /// </summary>
    protected Mock<IGameServer> GameServer0 { get; set; } = null!;

    /// <summary>
    /// Gets or sets the second game server.
    /// </summary>
    protected Mock<IGameServer> GameServer1 { get; set; } = null!;

    /// <summary>
    /// Gets or sets the repository provider.
    /// </summary>
    protected IPersistenceContextProvider PersistenceContextProvider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the game servers.
    /// </summary>
    protected IDictionary<int, IGameServer> GameServers { get; set; } = null!;

    /// <summary>
    /// Gets or sets the guild server.
    /// </summary>
    protected IGuildServer GuildServer { get; set; } = null!;

    /// <summary>
    /// Gets or sets the guild master.
    /// </summary>
    protected Character GuildMaster { get; set; } = null!;

    /// <summary>
    /// Setups the test objects.
    /// </summary>
    [SetUp]
    public virtual async ValueTask SetupAsync()
    {
        this.GameServer0 = new Mock<IGameServer>();
        this.GameServer1 = new Mock<IGameServer>();
        this.PersistenceContextProvider = new InMemoryPersistenceContextProvider();

        this.GuildMaster = this.GetGuildMaster();

        this.SetupGameServer(this.GameServer0);
        this.SetupGameServer(this.GameServer1);

        this.GameServers = new Dictionary<int, IGameServer> { { 0, this.GameServer0.Object }, { 1, this.GameServer1.Object } };
        this.GuildServer = new OpenMU.GuildServer.GuildServer(new GuildChangeToGameServerPublisher(this.GameServers), this.PersistenceContextProvider, new NullLogger<GuildServer>());
        await this.GuildServer.CreateGuildAsync(GuildName, this.GuildMaster.Name, this.GuildMaster.Id, new byte[16], 0).ConfigureAwait(false);
        var guildId = await this.GuildServer.GetGuildIdByNameAsync(GuildName).ConfigureAwait(false);
        await this.GuildServer.GuildMemberLeftGameAsync(guildId, this.GuildMaster.Id, 0).ConfigureAwait(false);
    }

    /// <summary>
    /// Sets up the game server.
    /// </summary>
    /// <param name="gameServer">The game server.</param>
    protected virtual void SetupGameServer(Mock<IGameServer> gameServer)
    {
        // can be overwritten.
    }

    private Character GetGuildMaster()
    {
        var context = this.PersistenceContextProvider.CreateNewContext();
        var master = context.CreateNew<Character>();
        master.Name = "GuildMaster";
        return master;
    }
}