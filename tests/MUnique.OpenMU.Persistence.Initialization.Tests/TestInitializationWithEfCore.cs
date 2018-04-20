// <copyright file="TestInitializationWithEfCore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
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
            var manager = new RepositoryManager();
            manager.RegisterRepositories();
            manager.ReCreateDatabase();
            var initialization = new DataInitialization(manager);
            initialization.CreateInitialData();

            // Loading game configuration
            using (var context = manager.CreateNewConfigurationContext())
            using (manager.UseContext(context))
            {
                var gameConfiguraton = manager.GetRepository<DataModel.Configuration.GameConfiguration>().GetAll().FirstOrDefault();
                Assert.That(gameConfiguraton, Is.Not.Null);

                // Testing loading of an account
                using (var accountContext = manager.CreateNewAccountContext(gameConfiguraton))
                using (manager.UseContext(accountContext))
                {
                    var account1 = manager.GetRepository<DataModel.Entities.Account, IAccountRepository>().GetAccountByLoginName("test1", "test1");
                    Assert.That(account1, Is.Not.Null);
                    Assert.That(account1.LoginName, Is.EqualTo("test1"));
                }
            }
        }
    }
}