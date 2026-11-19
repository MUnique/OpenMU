// <copyright file="LoginTicketPostgreSqlTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Data.Common;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Exercises the atomic login ticket issuance against PostgreSQL using independent persistence contexts.
/// Set OPENMU_TEST_POSTGRES to a disposable test database connection string to run it.
/// Only a uniquely named table owned by this fixture is created and removed.
/// </summary>
[TestFixture]
[NonParallelizable]
public class LoginTicketPostgreSqlTest
{
    private static readonly string TableName = "LoginTicketReview_" + Guid.NewGuid().ToString("N");
    private string _connectionString = string.Empty;

    /// <summary>
    /// Creates the isolated table when a test database is configured.
    /// </summary>
    [OneTimeSetUp]
    public async Task SetUpAsync()
    {
        this._connectionString = Environment.GetEnvironmentVariable("OPENMU_TEST_POSTGRES") ?? string.Empty;
        if (string.IsNullOrEmpty(this._connectionString))
        {
            Assert.Ignore("Set OPENMU_TEST_POSTGRES to run the PostgreSQL login ticket tests.");
        }

        await using var context = this.CreateDbContext();
        await context.Database.ExecuteSqlRawAsync(context.Database.GenerateCreateScript()).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes only the table created by this fixture.
    /// </summary>
    [OneTimeTearDown]
    public async Task TearDownAsync()
    {
        if (!string.IsNullOrEmpty(this._connectionString))
        {
            await using var context = this.CreateDbContext();
            using var commandBuilder = new NpgsqlCommandBuilder();
            var dropTableSql = "DROP TABLE IF EXISTS " + commandBuilder.QuoteIdentifier(TableName);
            await context.Database.ExecuteSqlRawAsync(dropTableSql).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Holds every issuer's count open long enough for all of them to count before any inserts,
    /// which they only can if they don't take turns. Together, they must stay within the maximum.
    /// </summary>
    [Test]
    public async Task ConcurrentIssuersStayWithinTheLimitAsync()
    {
        const int count = 16;
        const int maximum = 5;
        var accountId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var slowCounts = new SlowCounts(TimeSpan.FromMilliseconds(200));
        var attempts = Enumerable.Range(0, count).Select(async index =>
        {
            using var context = new PlayerContext(this.CreateDbContext(slowCounts), new RepositoryProvider(NullLoggerFactory.Instance, null, new ContextStack()), NullLogger<PlayerContext>.Instance);
            return await context.TryAddLoginTicketAsync(accountId, $"hash-{index}", now, now.AddSeconds(90), maximum, CancellationToken.None).ConfigureAwait(false);
        }).ToArray();
        var results = await Task.WhenAll(attempts).ConfigureAwait(false);

        await using var verify = this.CreateDbContext();
        var stored = await verify.Set<LoginTicket>().AsNoTracking().CountAsync(ticket => ticket.AccountId == accountId).ConfigureAwait(false);
        Assert.That(results.Count(added => added), Is.EqualTo(maximum));
        Assert.That(stored, Is.EqualTo(maximum));
    }

    private TicketDbContext CreateDbContext(DbCommandInterceptor? interceptor = null)
    {
        var builder = new DbContextOptionsBuilder<TicketDbContext>().UseNpgsql(this._connectionString);
        if (interceptor is not null)
        {
            builder.AddInterceptors(interceptor);
        }

        return new TicketDbContext(builder.Options);
    }

    private sealed class TicketDbContext(DbContextOptions<TicketDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var tickets = modelBuilder.Entity<LoginTicket>();
            tickets.ToTable(TableName, "public");
            tickets.HasKey(ticket => ticket.Id);
            tickets.HasIndex(ticket => ticket.TokenHash).IsUnique();
        }
    }

    private sealed class SlowCounts(TimeSpan delay) : DbCommandInterceptor
    {
        public override async ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
