using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations.AdminPanel
{
    /// <inheritdoc />
    public partial class AddAdminAuditEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminAuditEntry",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PrincipalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PrincipalName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Frontend = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Operation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ArgumentsJson = table.Column<string>(type: "character varying(16384)", maxLength: 16384, nullable: false),
                    ResultJson = table.Column<string>(type: "character varying(16384)", maxLength: 16384, nullable: false),
                    Outcome = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminAuditEntry", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminAuditEntry_StartedAt_Id",
                schema: "admin",
                table: "AdminAuditEntry",
                columns: new[] { "StartedAt", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminAuditEntry",
                schema: "admin");
        }
    }
}
