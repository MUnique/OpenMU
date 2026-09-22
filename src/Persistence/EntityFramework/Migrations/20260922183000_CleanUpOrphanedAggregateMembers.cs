// <copyright file="20260922183000_CleanUpOrphanedAggregateMembers.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    [DbContext(typeof(EntityDataContext))]
    [Migration("20260922183000_CleanUpOrphanedAggregateMembers")]
    public partial class CleanUpOrphanedAggregateMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Removes the rows which were orphaned while EntityFrameworkContextBase.ForEachAggregate
            // did not recurse into the members of a member: deleting an account removed its characters,
            // but not their inventories - and a character's inventory is referenced BY the character,
            // so no delete cascade of the database ever reached it.
            // These are the only three references to an item storage; its items are removed through
            // the cascade of the storage itself.
            migrationBuilder.Sql(
                @"delete from data.""ItemStorage"" s
                where not exists (select 1 from data.""Character"" c where c.""InventoryId"" = s.""Id"")
                  and not exists (select 1 from data.""Account"" a where a.""VaultId"" = s.""Id"")
                  and not exists (select 1 from config.""MonsterDefinition"" m where m.""MerchantStoreId"" = s.""Id"")");

            // The same for the sender appearance of a deleted letter - a letter body is the only
            // table which references an appearance data.
            migrationBuilder.Sql(
                @"delete from data.""AppearanceData"" a
                where not exists (select 1 from data.""LetterBody"" b where b.""SenderAppearanceId"" = a.""Id"")");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nothing to do - the deleted rows were unreachable and can't be restored.
        }
    }
}
