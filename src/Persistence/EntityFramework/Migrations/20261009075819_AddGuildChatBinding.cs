using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddGuildChatBinding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildChatBinding",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GuildId = table.Column<Guid>(type: "uuid", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: false),
                    ExternalServerId = table.Column<string>(type: "text", nullable: false),
                    ExternalChannelId = table.Column<string>(type: "text", nullable: false),
                    IsHosted = table.Column<bool>(type: "boolean", nullable: false),
                    BoundBy = table.Column<string>(type: "text", nullable: true),
                    BoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildChatBinding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuildChatBinding_Guild_GuildId",
                        column: x => x.GuildId,
                        principalSchema: "guild",
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildChatBinding_GuildId_Scope_Provider",
                schema: "data",
                table: "GuildChatBinding",
                columns: new[] { "GuildId", "Scope", "Provider" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuildChatBinding_Provider_ExternalChannelId",
                schema: "data",
                table: "GuildChatBinding",
                columns: new[] { "Provider", "ExternalChannelId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildChatBinding",
                schema: "data");
        }
    }
}
