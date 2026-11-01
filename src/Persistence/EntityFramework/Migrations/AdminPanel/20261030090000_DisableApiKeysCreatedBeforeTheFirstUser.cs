using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations.AdminPanel
{
    /// <summary>
    /// Disables the API keys which were created before the first user of the admin panel.
    /// </summary>
    /// <remarks>
    /// While the admin panel had no user, anybody who reached it could create an API key, and the
    /// key kept working after the first user existed. The admin panel doesn't create keys without
    /// a user anymore; the keys of that time are disabled once by this migration, so that an
    /// administrator enables only the ones they created. The oldest stored user stands for the
    /// first one. Without a stored user, no key is disabled here: the first user disables them
    /// when it's created in the admin panel.
    /// </remarks>
    public partial class DisableApiKeysCreatedBeforeTheFirstUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE admin."ApiKey"
                SET "IsDisabled" = TRUE
                WHERE NOT "IsDisabled"
                  AND "CreatedAt" < (SELECT MIN("CreatedAt") FROM admin."AdminUser");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The keys stay disabled: it isn't known which of them were enabled before, and an
            // administrator may have enabled the trusted ones again already.
        }
    }
}
