// <copyright file="BackupServiceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
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

        using var backupStream = new MemoryStream();
        await new BackupService(sourceProvider).CreateBackupAsync(backupStream).ConfigureAwait(false);
        Assert.That(backupStream.Length, Is.GreaterThan(0));

        backupStream.Position = 0;
        var targetProvider = new InMemoryPersistenceContextProvider();
        var targetBackupService = new BackupService(targetProvider);
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
    /// Tests if a file which is no backup archive is detected as such, so that the database isn't dropped for nothing.
    /// </summary>
    [Test]
    public void ContainsRestorableDataReturnsFalseForOtherFiles()
    {
        var backupService = new BackupService(new InMemoryPersistenceContextProvider());
        using var noZipStream = new MemoryStream("This is not a zip archive."u8.ToArray());

        Assert.That(backupService.ContainsRestorableData(noZipStream), Is.False);
        Assert.That(noZipStream.Position, Is.Zero);
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
