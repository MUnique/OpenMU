using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountExternalLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountExternalLink",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: false),
                    ExternalUserId = table.Column<string>(type: "text", nullable: true),
                    ExternalUserName = table.Column<string>(type: "text", nullable: true),
                    CharacterName = table.Column<string>(type: "text", nullable: true),
                    LinkedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CodeHash = table.Column<string>(type: "text", nullable: true),
                    CodeExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountExternalLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountExternalLink_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "data",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountExternalLink_AccountId_Provider",
                schema: "data",
                table: "AccountExternalLink",
                columns: new[] { "AccountId", "Provider" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountExternalLink_CodeHash",
                schema: "data",
                table: "AccountExternalLink",
                column: "CodeHash");

            migrationBuilder.CreateIndex(
                name: "IX_AccountExternalLink_Provider_ExternalUserId",
                schema: "data",
                table: "AccountExternalLink",
                columns: new[] { "Provider", "ExternalUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountExternalLink",
                schema: "data");
        }
    }
}
