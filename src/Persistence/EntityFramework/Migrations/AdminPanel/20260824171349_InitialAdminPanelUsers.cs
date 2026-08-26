using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations.AdminPanel
{
    /// <inheritdoc />
    public partial class InitialAdminPanelUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "admin");

            migrationBuilder.CreateTable(
                name: "AdminUser",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedLoginName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    SecurityStamp = table.Column<string>(type: "text", nullable: false),
                    Roles = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsTwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    ProtectedAuthenticatorKey = table.Column<string>(type: "text", nullable: true),
                    RecoveryCodeHashes = table.Column<string>(type: "text", nullable: true),
                    LastAcceptedTotpStep = table.Column<long>(type: "bigint", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUser", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminUser_NormalizedLoginName",
                schema: "admin",
                table: "AdminUser",
                column: "NormalizedLoginName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminUser",
                schema: "admin");
        }
    }
}
