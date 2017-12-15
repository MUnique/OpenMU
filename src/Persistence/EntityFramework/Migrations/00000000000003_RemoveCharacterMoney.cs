// <copyright file="00000000000003_RemoveCharacterMoney.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <summary>
    /// Removes the obsolete column of Character.Money. The money of a character is already stored in Character.Inventory.Money.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.Migrations.Migration" />
    [DbContext(typeof(EntityDataContext))]
    [Migration("00000000000003_RemoveCharacterMoney")]
    public partial class RemoveCharacterMoney : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("Money", "Character", "data");
        }
    }
}
