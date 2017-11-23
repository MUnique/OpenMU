// <copyright file="00000000000001_Seed.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;
    using MUnique.OpenMU.Persistence.Initialization;

    /// <summary>
    /// The seed migration which creates all the required data to start the server.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.Migrations.Migration" />
    [DbContext(typeof(EntityDataContext))]
    [Migration("00000000000001_Seed")]
    public partial class Seed : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var repositoryManager = new RepositoryManager();
            var init = new DataInitialization(repositoryManager);
            init.CreateInitialData();
        }
    }
}
