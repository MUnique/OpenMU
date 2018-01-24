// <copyright file="00000000000004_AddItemStorePrice.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <summary>
    /// Adds the column for <see cref="DataModel.Entities.Item.StorePrice"/>.
    /// </summary>
    [DbContext(typeof(EntityDataContext))]
    [Migration("00000000000004_AddItemStorePrice")]
    public partial class AddItemStorePrice : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int?>("StorePrice", "Item", schema: "data", nullable: true);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("StorePrice", "Item", "data");
        }
    }
}
