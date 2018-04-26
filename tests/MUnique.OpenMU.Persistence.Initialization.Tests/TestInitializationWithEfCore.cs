// <copyright file="TestInitializationWithEfCore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.Persistence.EntityFramework;
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
            var initialization = new DataInitialization(manager);
            initialization.CreateInitialData();

            // Loading game configuration
            using (var context = manager.CreateNewConfigurationContext())
            {
                var gameConfiguraton = context.Get<GameConfiguration>().FirstOrDefault();
                Assert.That(gameConfiguraton, Is.Not.Null);

                // Testing loading of an account
                using (var accountContext = manager.CreateNewPlayerContext(gameConfiguraton))
                {
                    var account1 = accountContext.GetAccountByLoginName("test1", "test1");
                    Assert.That(account1, Is.Not.Null);
                    Assert.That(account1.LoginName, Is.EqualTo("test1"));
                }
            }
        }
    }
}