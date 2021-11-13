// <copyright file="TestInitializationWithEfCore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// The main program class.
/// </summary>
[TestFixture]
internal class TestInitializationWithEfCore
{
    /// <summary>
    /// Tests the data initialization using the entity framework core.
    /// </summary>
    [Test]
    [Ignore("This is not a real test which should run automatically.")]
    public void SetupDatabaseAndTestLoadingData()
    {
        var manager = new PersistenceContextProvider(new NullLoggerFactory());
        manager.ReCreateDatabase();
        this.TestDataInitialization(new PersistenceContextProvider(new NullLoggerFactory()));
    }

    /// <summary>
    /// Tests the data initialization using the in-memory persistence.
    /// </summary>
    [Test]
    public void TestDataInitializationInMemory()
    {
        this.TestDataInitialization(new InMemoryPersistenceContextProvider());
    }

    /// <summary>
    /// Tests the data initialization using the in-memory persistence.
    /// </summary>
    [Test]
    public void TestSeason6Data()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        dataInitialization.CreateInitialData(1, false);
    }

    /// <summary>
    /// Tests the data initialization using the in-memory persistence.
    /// </summary>
    [Test]
    public void Test075Data()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new Version075.DataInitialization(contextProvider, new NullLoggerFactory());
        dataInitialization.CreateInitialData(1, true);
    }

    /// <summary>
    /// Tests the data initialization using the in-memory persistence.
    /// </summary>
    [Test]
    public void Test095dData()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new Version095d.DataInitialization(contextProvider, new NullLoggerFactory());
        dataInitialization.CreateInitialData(1, true);
    }

    private void TestDataInitialization(IPersistenceContextProvider contextProvider)
    {
        var initialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        initialization.CreateInitialData(3, true);

        // Loading game configuration
        using var context = contextProvider.CreateNewConfigurationContext();
        var gameConfiguraton = context.Get<DataModel.Configuration.GameConfiguration>().FirstOrDefault();
        Assert.That(gameConfiguraton, Is.Not.Null);

        // Testing loading of an account
        using var accountContext = contextProvider.CreateNewPlayerContext(gameConfiguraton!);
        var account1 = accountContext.GetAccountByLoginName("test1", "test1");
        Assert.That(account1, Is.Not.Null);
        Assert.That(account1!.LoginName, Is.EqualTo("test1"));
    }
}