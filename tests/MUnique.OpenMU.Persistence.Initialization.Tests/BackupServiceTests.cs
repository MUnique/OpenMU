// <copyright file="BackupServiceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the <see cref="BackupService"/>.
/// </summary>
[TestFixture]
public class BackupServiceTests
{
    /// <summary>
    /// Tests if the data of an exported backup can be restored again.
    /// </summary>
    [Test]
    public async Task ExportAndRestoreRoundTripAsync()
    {
        var sourceProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(sourceProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var sourceAdminUsers = new InMemoryAdminUserRepository();
        await sourceAdminUsers.AddAsync(new AdminUser
        {
            Id = Guid.NewGuid(),
            LoginName = "Admin",
            NormalizedLoginName = "ADMIN",
            PasswordHash = "hash",
            Roles = AdminRoles.Administrator,
        }).ConfigureAwait(false);

        using var backupStream = new MemoryStream();
        await new BackupService(sourceProvider, sourceAdminUsers).CreateBackupAsync(backupStream).ConfigureAwait(false);
        Assert.That(backupStream.Length, Is.GreaterThan(0));

        backupStream.Position = 0;
        var targetProvider = new InMemoryPersistenceContextProvider();
        var targetAdminUsers = new InMemoryAdminUserRepository();
        var targetBackupService = new BackupService(targetProvider, targetAdminUsers);
        Assert.That(targetBackupService.ContainsRestorableData(backupStream), Is.True);
        Assert.That(backupStream.Position, Is.Zero, "The stream position should be restored after the check.");
        await targetBackupService.RestoreBackupAsync(backupStream).ConfigureAwait(false);

        await AssertSameCountAsync<GameConfiguration>(sourceProvider, targetProvider).ConfigureAwait(false);
        await AssertSameCountAsync<GameServerDefinition>(sourceProvider, targetProvider).ConfigureAwait(false);
        await AssertSameCountAsync<ChatServerDefinition>(sourceProvider, targetProvider).ConfigureAwait(false);
        await AssertSameCountAsync<ConnectServerDefinition>(sourceProvider, targetProvider).ConfigureAwait(false);
        await AssertSameCountAsync<Account>(sourceProvider, targetProvider).ConfigureAwait(false);

        using var sourceContext = sourceProvider.CreateNewContext();
        using var targetContext = targetProvider.CreateNewContext();
        var sourceConfig = (await sourceContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var targetConfig = (await targetContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();

        Assert.Multiple(() =>
        {
            Assert.That(GetId(targetConfig), Is.EqualTo(GetId(sourceConfig)));
            Assert.That(targetConfig.ExperienceRate, Is.EqualTo(sourceConfig.ExperienceRate));
            Assert.That(targetConfig.Maps, Has.Count.EqualTo(sourceConfig.Maps.Count));
            Assert.That(targetConfig.Items, Has.Count.EqualTo(sourceConfig.Items.Count));
            Assert.That(targetConfig.CharacterClasses, Has.Count.EqualTo(sourceConfig.CharacterClasses.Count));
            Assert.That(targetConfig.Attributes, Has.Count.EqualTo(sourceConfig.Attributes.Count));
            Assert.That(targetConfig.Monsters, Has.Count.EqualTo(sourceConfig.Monsters.Count));
            Assert.That(
                targetConfig.ItemSlotTypes.Sum(s => s.ItemSlots.Count),
                Is.EqualTo(sourceConfig.ItemSlotTypes.Sum(s => s.ItemSlots.Count)),
                "The item slots (a collection of value types) were not restored.");
        });

        var sourceAccount = (await sourceContext.GetAsync<Account>().ConfigureAwait(false)).OrderBy(a => a.LoginName).First();
        var targetAccount = (await targetContext.GetAsync<Account>().ConfigureAwait(false)).OrderBy(a => a.LoginName).First();
        Assert.Multiple(() =>
        {
            Assert.That(targetAccount.LoginName, Is.EqualTo(sourceAccount.LoginName));
            Assert.That(targetAccount.PasswordHash, Is.EqualTo(sourceAccount.PasswordHash));
            Assert.That(targetAccount.Characters, Has.Count.EqualTo(sourceAccount.Characters.Count));
        });

        var restoredAdminUsers = await targetAdminUsers.GetAllAsync().ConfigureAwait(false);
        Assert.That(restoredAdminUsers, Has.Count.EqualTo(1));
        Assert.That(restoredAdminUsers[0].LoginName, Is.EqualTo("Admin"));

        var sourceCharacter = sourceAccount.Characters.OrderBy(c => c.Name).First();
        var targetCharacter = targetAccount.Characters.OrderBy(c => c.Name).First();
        Assert.Multiple(() =>
        {
            Assert.That(targetCharacter.Name, Is.EqualTo(sourceCharacter.Name));
            Assert.That(GetId(targetCharacter.CharacterClass!), Is.EqualTo(GetId(sourceCharacter.CharacterClass!)));
            Assert.That(targetCharacter.Inventory?.Items, Has.Count.EqualTo(sourceCharacter.Inventory?.Items.Count));
            Assert.That(targetCharacter.Attributes, Has.Count.EqualTo(sourceCharacter.Attributes.Count));

            // References between the different backup files must point to the same restored instances.
            Assert.That(
                targetCharacter.CharacterClass,
                Is.SameAs(targetConfig.CharacterClasses.First(c => GetId(c) == GetId(targetCharacter.CharacterClass!))));
        });
    }

    /// <summary>
    /// Tests if all types which hold data are covered by the backup.
    /// It's easy to forget to add a new type of the data model to the backup process,
    /// so this test compares the objects of all types before and after a round trip.
    /// </summary>
    [Test]
    public async Task AllTypesOfTheDataModelAreCoveredAsync()
    {
        var sourceProvider = new InMemoryPersistenceContextProvider();
        await new VersionSeasonSix.DataInitialization(sourceProvider, new NullLoggerFactory())
            .CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var backupStream = new MemoryStream();
        var adminUsers = new InMemoryAdminUserRepository();
        await new BackupService(sourceProvider, adminUsers).CreateBackupAsync(backupStream).ConfigureAwait(false);

        backupStream.Position = 0;
        var targetProvider = new InMemoryPersistenceContextProvider();
        await new BackupService(targetProvider, new InMemoryAdminUserRepository()).RestoreBackupAsync(backupStream).ConfigureAwait(false);

        var expected = await GetObjectCountsAsync(sourceProvider).ConfigureAwait(false);
        var actual = await GetObjectCountsAsync(targetProvider).ConfigureAwait(false);

        // We don't compare the exact numbers here: the data initialization leaves some objects behind
        // which are not referenced by anything (e.g. items without an item storage). They can't be
        // reached from the exported root objects, so they are not part of the backup.
        // A type which is not covered at all doesn't have any object after the restore.
        var missingTypes = expected
            .Where(pair => !actual.ContainsKey(pair.Key))
            .Select(pair => $"{pair.Key} ({pair.Value} objects)")
            .ToList();

        Assert.That(
            missingTypes,
            Is.Empty,
            "These types are not covered by the backup. If you added a type to the data model, add it to the BackupService, too.");
    }

    /// <summary>
    /// Tests if a file which is no backup archive is detected as such, so that the database isn't dropped for nothing.
    /// </summary>
    [Test]
    public void ContainsRestorableDataReturnsFalseForOtherFiles()
    {
        var backupService = new BackupService(new InMemoryPersistenceContextProvider(), new InMemoryAdminUserRepository());
        using var noZipStream = new MemoryStream("This is not a zip archive."u8.ToArray());

        Assert.That(backupService.ContainsRestorableData(noZipStream), Is.False);
        Assert.That(noZipStream.Position, Is.Zero);
    }

    /// <summary>
    /// Counts the objects of every type of the data model which is known to the persistence.
    /// </summary>
    /// <param name="contextProvider">The context provider.</param>
    /// <returns>The number of objects, by the name of their type.</returns>
    private static async Task<Dictionary<string, int>> GetObjectCountsAsync(IPersistenceContextProvider contextProvider)
    {
        using var context = contextProvider.CreateNewContext();
        var result = new Dictionary<string, int>();
        var dataModelTypes = typeof(GameConfiguration).Assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false, IsPublic: true }
                           && type.Namespace?.StartsWith("MUnique.OpenMU.DataModel", StringComparison.Ordinal) is true)
            .OrderBy(type => type.FullName, StringComparer.Ordinal);

        foreach (var type in dataModelTypes)
        {
            try
            {
                var objects = await context.GetAsync(type).ConfigureAwait(false);
                var count = objects.Cast<object>().Count();
                if (count > 0)
                {
                    result.Add(type.FullName!, count);
                }
            }
            catch
            {
                // Not every type has a repository - these can't hold data on their own.
            }
        }

        return result;
    }

    private static Guid GetId(object obj) => ((IIdentifiable)obj).Id;

    private static async Task AssertSameCountAsync<T>(IPersistenceContextProvider source, IPersistenceContextProvider target)
        where T : class
    {
        using var sourceContext = source.CreateNewContext();
        using var targetContext = target.CreateNewContext();
        var sourceCount = (await sourceContext.GetAsync<T>().ConfigureAwait(false)).Count();
        var targetCount = (await targetContext.GetAsync<T>().ConfigureAwait(false)).Count();
        Assert.That(targetCount, Is.EqualTo(sourceCount), $"Unexpected number of restored {typeof(T).Name} objects.");
    }
}
