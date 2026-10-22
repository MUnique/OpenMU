// <copyright file="DashboardStatisticsTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Dashboard;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the statistics queries of the persistence contexts, which are shown on the dashboard.
/// </summary>
[TestFixture]
public class DashboardStatisticsTests
{
    private InMemoryPersistenceContextProvider _persistenceContextProvider = null!;

    /// <summary>
    /// Setups the test objects.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._persistenceContextProvider = new InMemoryPersistenceContextProvider();
    }

    /// <summary>
    /// The account statistics count the accounts of players, their characters, new and banned ones.
    /// </summary>
    [Test]
    public async Task AccountStatisticsAsync()
    {
        var now = DateTime.UtcNow;
        using (var context = this._persistenceContextProvider.CreateNewPlayerContext(new GameConfiguration()))
        {
            CreateAccount(context, now.AddDays(-30), AccountState.Normal, characters: 2);
            CreateAccount(context, now.AddDays(-1), AccountState.Normal, characters: 1);
            CreateAccount(context, now.AddDays(-30), AccountState.Banned, characters: 0);
            CreateAccount(context, now.AddDays(-30), AccountState.TemporarilyBanned, characters: 0);
            CreateAccount(context, now.AddDays(-1), AccountState.Normal, characters: 1).IsBot = true;
            CreateAccount(context, now.AddDays(-1), AccountState.Normal, characters: 1).IsTemplate = true;
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        using var playerContext = this._persistenceContextProvider.CreateNewPlayerContext(new GameConfiguration());
        var statistics = await playerContext.GetAccountStatisticsAsync(now.AddDays(-7)).ConfigureAwait(false);

        Assert.That(statistics, Is.EqualTo(new AccountStatistics(AccountCount: 4, NewAccountCount: 1, BannedAccountCount: 2, CharacterCount: 3)));
    }

    /// <summary>
    /// The guild statistics count the guilds and the alliances.
    /// </summary>
    [Test]
    public async Task GuildStatisticsAsync()
    {
        using (var context = this._persistenceContextProvider.CreateNewGuildContext())
        {
            var firstMaster = context.CreateNew<Guild>();
            var secondMaster = context.CreateNew<Guild>();
            context.CreateNew<Guild>().AllianceGuild = firstMaster;
            context.CreateNew<Guild>().AllianceGuild = firstMaster;
            context.CreateNew<Guild>().AllianceGuild = secondMaster;
            context.CreateNew<Guild>();
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        using var guildContext = this._persistenceContextProvider.CreateNewGuildContext();
        var statistics = await guildContext.GetGuildStatisticsAsync().ConfigureAwait(false);

        Assert.That(statistics, Is.EqualTo(new GuildStatistics(GuildCount: 6, AllianceCount: 2)));
    }

    private static Account CreateAccount(IContext context, DateTime registrationDate, AccountState state, int characters)
    {
        var account = context.CreateNew<Account>();
        account.LoginName = Guid.NewGuid().ToString("N")[..10];
        account.RegistrationDate = registrationDate;
        account.State = state;
        for (var i = 0; i < characters; i++)
        {
            account.Characters.Add(context.CreateNew<Character>());
        }

        return account;
    }
}
