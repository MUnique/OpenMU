using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddCashShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CashShopConfigurationId",
                schema: "config",
                table: "GameConfiguration",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoblinPoints",
                schema: "data",
                table: "Account",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WCoinC",
                schema: "data",
                table: "Account",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WCoinP",
                schema: "data",
                table: "Account",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CashShopCoinGrant",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CoinType = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    GrantedBy = table.Column<string>(type: "text", nullable: true),
                    Reference = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashShopCoinGrant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashShopCoinGrant_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "data",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashShopConfiguration",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScriptSaleZone = table.Column<short>(type: "smallint", nullable: false),
                    ScriptYear = table.Column<short>(type: "smallint", nullable: false),
                    ScriptYearId = table.Column<short>(type: "smallint", nullable: false),
                    BannerSaleZone = table.Column<short>(type: "smallint", nullable: false),
                    BannerYear = table.Column<short>(type: "smallint", nullable: false),
                    BannerYearId = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashShopConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashShopStorageItem",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductSequence = table.Column<int>(type: "integer", nullable: false),
                    PriceSequence = table.Column<int>(type: "integer", nullable: false),
                    IsGift = table.Column<bool>(type: "boolean", nullable: false),
                    GiftSenderName = table.Column<string>(type: "text", nullable: true),
                    GiftMessage = table.Column<string>(type: "text", nullable: true),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashShopStorageItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashShopStorageItem_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "data",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashShopPackage",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CashShopConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    PackageSequence = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    CoinType = table.Column<int>(type: "integer", nullable: false),
                    IsForSale = table.Column<bool>(type: "boolean", nullable: false),
                    IsGiftable = table.Column<bool>(type: "boolean", nullable: false),
                    IsBundle = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashShopPackage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashShopPackage_CashShopConfiguration_CashShopConfiguration~",
                        column: x => x.CashShopConfigurationId,
                        principalSchema: "config",
                        principalTable: "CashShopConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashShopProduct",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CashShopPackageId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductSequence = table.Column<int>(type: "integer", nullable: false),
                    PriceSequence = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    ItemLevel = table.Column<byte>(type: "smallint", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashShopProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashShopProduct_CashShopPackage_CashShopPackageId",
                        column: x => x.CashShopPackageId,
                        principalSchema: "config",
                        principalTable: "CashShopPackage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CashShopProduct_ItemDefinition_ItemDefinitionId",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameConfiguration_CashShopConfigurationId",
                schema: "config",
                table: "GameConfiguration",
                column: "CashShopConfigurationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashShopCoinGrant_AccountId_AppliedAt",
                schema: "data",
                table: "CashShopCoinGrant",
                columns: new[] { "AccountId", "AppliedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CashShopCoinGrant_Reference",
                schema: "data",
                table: "CashShopCoinGrant",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashShopPackage_CashShopConfigurationId",
                schema: "config",
                table: "CashShopPackage",
                column: "CashShopConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_CashShopProduct_CashShopPackageId",
                schema: "config",
                table: "CashShopProduct",
                column: "CashShopPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_CashShopProduct_ItemDefinitionId",
                schema: "config",
                table: "CashShopProduct",
                column: "ItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CashShopStorageItem_AccountId",
                schema: "data",
                table: "CashShopStorageItem",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameConfiguration_CashShopConfiguration_CashShopConfigurati~",
                schema: "config",
                table: "GameConfiguration",
                column: "CashShopConfigurationId",
                principalSchema: "config",
                principalTable: "CashShopConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameConfiguration_CashShopConfiguration_CashShopConfigurati~",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropTable(
                name: "CashShopCoinGrant",
                schema: "data");

            migrationBuilder.DropTable(
                name: "CashShopProduct",
                schema: "config");

            migrationBuilder.DropTable(
                name: "CashShopStorageItem",
                schema: "data");

            migrationBuilder.DropTable(
                name: "CashShopPackage",
                schema: "config");

            migrationBuilder.DropTable(
                name: "CashShopConfiguration",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_GameConfiguration_CashShopConfigurationId",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropColumn(
                name: "CashShopConfigurationId",
                schema: "config",
                table: "GameConfiguration");

            migrationBuilder.DropColumn(
                name: "GoblinPoints",
                schema: "data",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "WCoinC",
                schema: "data",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "WCoinP",
                schema: "data",
                table: "Account");
        }
    }
}
