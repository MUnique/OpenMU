// <copyright file="AccountLinkPostgreSqlTest.cs" company="MUnique">
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
/// Exercises atomic link redemption against PostgreSQL using independent persistence contexts.
/// Set OPENMU_TEST_POSTGRES to a disposable test database connection string to run it.
/// Only a uniquely named table owned by this fixture is created and removed.
/// </summary>
[TestFixture]
[NonParallelizable]
public class AccountLinkPostgreSqlTest
{
    private static readonly string TableName = "AccountLinkReview_" + Guid.NewGuid().ToString("N");
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
            Assert.Ignore("Set OPENMU_TEST_POSTGRES to run the PostgreSQL link-concurrency tests.");
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
    /// Forces all redeemers to read the same pending code before allowing any of them to update it.
    /// Losing requests must keep their previous account links, including deletions rolled back by the transaction.
    /// </summary>
    [Test]
    public async Task ConcurrentRedemptionsHaveOneWinnerAndKeepLosersLinkedAsync()
    {
        const int count = 12;
        var targetAccountId = Guid.NewGuid();
        var hash = Guid.NewGuid().ToString("N");
        var now = DateTime.UtcNow;
        var accounts = Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToArray();
        await using (var context = this.CreateDbContext())
        {
            context.Add(new AccountExternalLink { Id = Guid.NewGuid(), AccountId = targetAccountId, Provider = "discord", CodeHash = hash, CodeExpiresAt = now.AddMinutes(10), CharacterName = "Hero" });
            for (var index = 0; index < count; index++)
            {
                context.Add(new AccountExternalLink { Id = Guid.NewGuid(), AccountId = accounts[index], Provider = "discord", ExternalUserId = $"user-{index}" });
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var barrier = new RedemptionBarrier(count);
        var attempts = Enumerable.Range(0, count).Select(async index =>
        {
            using var context = new PlayerContext(this.CreateDbContext(barrier), new RepositoryProvider(NullLoggerFactory.Instance, null, new ContextStack()), NullLogger<PlayerContext>.Instance);
            var result = await context.TryLinkAccountAsync("discord", hash, now, $"user-{index}", "user", CancellationToken.None).ConfigureAwait(false);
            return (Index: index, Result: result);
        }).ToArray();
        var results = await Task.WhenAll(attempts).ConfigureAwait(false);
        var winner = results.Single(result => result.Result is not null).Index;
        await using var verify = this.CreateDbContext();
        var links = await verify.Set<AccountExternalLink>().AsNoTracking().ToListAsync().ConfigureAwait(false);
        Assert.That(links.Single(link => link.AccountId == targetAccountId).ExternalUserId, Is.EqualTo($"user-{winner}"));
        Assert.That(links.Any(link => link.AccountId == accounts[winner]), Is.False);
        foreach (var loser in results.Where(result => result.Result is null))
        {
            Assert.That(links.Single(link => link.AccountId == accounts[loser.Index]).ExternalUserId, Is.EqualTo($"user-{loser.Index}"));
        }
    }

    private LinkDbContext CreateDbContext(DbCommandInterceptor? interceptor = null)
    {
        var builder = new DbContextOptionsBuilder<LinkDbContext>().UseNpgsql(this._connectionString);
        if (interceptor is not null)
        {
            builder.AddInterceptors(interceptor);
        }

        return new LinkDbContext(builder.Options);
    }

    private sealed class LinkDbContext(DbContextOptions<LinkDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var links = modelBuilder.Entity<AccountExternalLink>();
            links.ToTable(TableName, "public");
            links.HasKey(link => link.Id);
            links.HasIndex(link => new { link.AccountId, link.Provider }).IsUnique();
            links.HasIndex(link => new { link.Provider, link.ExternalUserId }).IsUnique();
        }
    }

    private sealed class RedemptionBarrier(int count) : DbCommandInterceptor
    {
        private readonly TaskCompletionSource _allRead = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _readCount;

        public override async ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            if (Interlocked.Increment(ref this._readCount) == count)
            {
                this._allRead.TrySetResult();
            }

            await this._allRead.Task.WaitAsync(TimeSpan.FromSeconds(20), cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
