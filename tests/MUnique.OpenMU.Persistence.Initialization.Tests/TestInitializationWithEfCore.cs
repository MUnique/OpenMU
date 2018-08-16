// <copyright file="TestInitializationWithEfCore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.Persistence.EntityFramework;
    using MUnique.OpenMU.Persistence.InMemory;
    using NUnit.Framework;

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
            var manager = new PersistenceContextProvider();
            manager.ReCreateDatabase();
            this.TestDataInitialization(new PersistenceContextProvider());
        }

        /// <summary>
        /// Tests the data initialization using the in-memory persistence.
        /// </summary>
        [Test]
        public void TestDataInitializationInMemory()
        {
            this.TestDataInitialization(new InMemoryPersistenceContextProvider());
        }

        private void TestDataInitialization(IPersistenceContextProvider contextProvider)
        {
            var initialization = new DataInitialization(contextProvider);
            initialization.CreateInitialData();

            // Loading game configuration
            using (var context = contextProvider.CreateNewConfigurationContext())
            {
                var gameConfiguraton = context.Get<DataModel.Configuration.GameConfiguration>().FirstOrDefault();
                Assert.That(gameConfiguraton, Is.Not.Null);

                // Testing loading of an account
                using (var accountContext = contextProvider.CreateNewPlayerContext(gameConfiguraton))
                {
                    var account1 = accountContext.GetAccountByLoginName("test1", "test1");
                    Assert.That(account1, Is.Not.Null);
                    Assert.That(account1.LoginName, Is.EqualTo("test1"));
                }
            }
        }
    }
}