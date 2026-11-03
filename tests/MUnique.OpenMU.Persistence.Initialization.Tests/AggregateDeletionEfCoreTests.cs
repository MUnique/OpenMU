// <copyright file="AggregateDeletionEfCoreTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence.EntityFramework;
using ItemStorageEntity = MUnique.OpenMU.Persistence.EntityFramework.Model.ItemStorage;

/// <summary>
/// Tests the deletion of an aggregate with the entity framework core, which requires a running postgres database.
/// </summary>
/// <remarks>
/// The interesting part is what the database is left with, so this can't be tested without one.
/// </remarks>
[TestFixture]
internal class AggregateDeletionEfCoreTests
{
    /// <summary>
    /// Deletes an account which owns a vault and a character with an inventory, and checks that no
    /// item storage is left behind. The inventory is the case which used to leak: it's referenced BY
    /// the character, so no delete cascade of the database reaches it, and the traversal of the
    /// aggregate didn't go deeper than the account's own members.
    /// </summary>
    [Test]
    [Ignore("This is not a real test which should run automatically. It requires a database.")]
    public async Task DeletingAnAccountDeletesTheItemStoragesOfItsCharactersAsync()
    {
        var contextProvider = await CreateInitializedDatabaseAsync().ConfigureAwait(false);
        GameConfiguration configuration;
        using (var context = contextProvider.CreateNewContext())
        {
            configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        }

        using (var context = contextProvider.CreateNewPlayerContext(configuration))
        {
            var account = context.CreateNew<Account>();
            account.LoginName = "deleteme";
            account.PasswordHash = "hash";
            account.Vault = context.CreateNew<ItemStorage>();

            var characterClass = configuration.CharacterClasses.First(c => c is { CanGetCreated: true, HomeMap: not null });
            var character = context.CreateNew<Character>();
            character.Name = "DeleteMe";
            character.CharacterClass = characterClass;
            character.CurrentMap = characterClass.HomeMap;
            character.CreateDate = DateTime.UtcNow;
            character.KeyConfiguration = new byte[30];
            character.Inventory = context.CreateNew<ItemStorage>();
            account.Characters.Add(character);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        Assert.That(await CountItemStoragesAsync().ConfigureAwait(false), Is.EqualTo(2), "The vault and the inventory should have been created.");

        using (var context = contextProvider.CreateNewPlayerContext(configuration))
        {
            var account = await context.GetAccountByLoginNameAsync("deleteme").ConfigureAwait(false);
            Assert.That(account, Is.Not.Null);
            await context.DeleteAsync(account!).ConfigureAwait(false);

            // Must not throw: nothing may delete a row which the context has marked as deleted itself.
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        Assert.That(await CountItemStoragesAsync().ConfigureAwait(false), Is.EqualTo(0));
    }

    private static async ValueTask<PersistenceContextProvider> CreateInitializedDatabaseAsync()
    {
        await ReCreateDatabaseAsync().ConfigureAwait(false);

        var contextProvider = new PersistenceContextProvider(new NullLoggerFactory(), null);
        await new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory())
            .CreateInitialDataAsync(1, true).ConfigureAwait(false);

        return contextProvider;
    }

    private static async ValueTask ReCreateDatabaseAsync()
    {
        var contextProvider = new PersistenceContextProvider(new NullLoggerFactory(), null);
        using var update = await contextProvider.ReCreateDatabaseAsync().ConfigureAwait(false);
    }

    private static async ValueTask<int> CountItemStoragesAsync()
    {
        await using var context = new EntityDataContext();
        return await context.Set<ItemStorageEntity>().CountAsync().ConfigureAwait(false);
    }
}
