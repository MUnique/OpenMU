using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "data");

            migrationBuilder.EnsureSchema(
                name: "config");

            migrationBuilder.EnsureSchema(
                name: "friend");

            migrationBuilder.EnsureSchema(
                name: "guild");

            migrationBuilder.CreateTable(
                name: "ChatServerDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServerId = table.Column<byte>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MaximumConnections = table.Column<int>(type: "integer", nullable: false),
                    ClientTimeout = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ClientCleanUpInterval = table.Column<TimeSpan>(type: "interval", nullable: false),
                    RoomCleanUpInterval = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatServerDefinition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Friend",
                schema: "friend",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    FriendId = table.Column<Guid>(type: "uuid", nullable: false),
                    Accepted = table.Column<bool>(type: "boolean", nullable: false),
                    RequestOpen = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friend", x => x.Id);
                    table.UniqueConstraint("AK_Friend_CharacterId_FriendId", x => new { x.CharacterId, x.FriendId });
                });

            migrationBuilder.CreateTable(
                name: "GameClientDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Season = table.Column<byte>(type: "smallint", nullable: false),
                    Episode = table.Column<byte>(type: "smallint", nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<byte[]>(type: "bytea", nullable: true),
                    Serial = table.Column<byte[]>(type: "bytea", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameClientDefinition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameConfiguration",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaximumLevel = table.Column<short>(type: "smallint", nullable: false),
                    MaximumMasterLevel = table.Column<short>(type: "smallint", nullable: false),
                    ExperienceRate = table.Column<float>(type: "real", nullable: false),
                    MinimumMonsterLevelForMasterExperience = table.Column<byte>(type: "smallint", nullable: false),
                    InfoRange = table.Column<byte>(type: "smallint", nullable: false),
                    AreaSkillHitsPlayer = table.Column<bool>(type: "boolean", nullable: false),
                    MaximumInventoryMoney = table.Column<int>(type: "integer", nullable: false),
                    MaximumVaultMoney = table.Column<int>(type: "integer", nullable: false),
                    RecoveryInterval = table.Column<int>(type: "integer", nullable: false),
                    MaximumLetters = table.Column<int>(type: "integer", nullable: false),
                    LetterSendPrice = table.Column<int>(type: "integer", nullable: false),
                    MaximumCharactersPerAccount = table.Column<byte>(type: "smallint", nullable: false),
                    CharacterNameRegex = table.Column<string>(type: "text", nullable: true),
                    MaximumPasswordLength = table.Column<int>(type: "integer", nullable: false),
                    MaximumPartySize = table.Column<byte>(type: "smallint", nullable: false),
                    ShouldDropMoney = table.Column<bool>(type: "boolean", nullable: false),
                    DamagePerOneItemDurability = table.Column<double>(type: "double precision", nullable: false),
                    DamagePerOnePetDurability = table.Column<double>(type: "double precision", nullable: false),
                    HitsPerOneItemDurability = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameServerConfiguration",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaximumPlayers = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameServerConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guild",
                schema: "guild",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HostilityId = table.Column<Guid>(type: "uuid", nullable: true),
                    AllianceGuildId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Logo = table.Column<byte[]>(type: "bytea", nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Notice = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guild", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guild_Guild_AllianceGuildId",
                        column: x => x.AllianceGuildId,
                        principalSchema: "guild",
                        principalTable: "Guild",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Guild_Guild_HostilityId",
                        column: x => x.HostilityId,
                        principalSchema: "guild",
                        principalTable: "Guild",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemStorage",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Money = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemStorage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerUpDefinitionValue",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<float>(type: "real", nullable: false),
                    AggregateType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerUpDefinitionValue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rectangle",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    X1 = table.Column<byte>(type: "smallint", nullable: false),
                    Y1 = table.Column<byte>(type: "smallint", nullable: false),
                    X2 = table.Column<byte>(type: "smallint", nullable: false),
                    Y2 = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rectangle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimpleCraftingSettings",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Money = table.Column<int>(type: "integer", nullable: false),
                    MoneyPerFinalSuccessPercentage = table.Column<int>(type: "integer", nullable: false),
                    SuccessPercent = table.Column<byte>(type: "smallint", nullable: false),
                    MaximumSuccessPercent = table.Column<byte>(type: "smallint", nullable: false),
                    MultipleAllowed = table.Column<bool>(type: "boolean", nullable: false),
                    ResultItemSelect = table.Column<int>(type: "integer", nullable: false),
                    SuccessPercentageAdditionForLuck = table.Column<int>(type: "integer", nullable: false),
                    SuccessPercentageAdditionForExcellentItem = table.Column<int>(type: "integer", nullable: false),
                    SuccessPercentageAdditionForAncientItem = table.Column<int>(type: "integer", nullable: false),
                    SuccessPercentageAdditionForSocketItem = table.Column<int>(type: "integer", nullable: false),
                    ResultItemLuckOptionChance = table.Column<byte>(type: "smallint", nullable: false),
                    ResultItemSkillChance = table.Column<byte>(type: "smallint", nullable: false),
                    ResultItemExcellentOptionChance = table.Column<byte>(type: "smallint", nullable: false),
                    ResultItemMaxExcOptionCount = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleCraftingSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatServerEndpoint",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChatServerDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    NetworkPort = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatServerEndpoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatServerEndpoint_ChatServerDefinition_ChatServerDefinitio~",
                        column: x => x.ChatServerDefinitionId,
                        principalSchema: "config",
                        principalTable: "ChatServerDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatServerEndpoint_GameClientDefinition_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "config",
                        principalTable: "GameClientDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ConnectServerDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    ServerId = table.Column<byte>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DisconnectOnUnknownPacket = table.Column<bool>(type: "boolean", nullable: false),
                    MaximumReceiveSize = table.Column<byte>(type: "smallint", nullable: false),
                    ClientListenerPort = table.Column<int>(type: "integer", nullable: false),
                    Timeout = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CurrentPatchVersion = table.Column<byte[]>(type: "bytea", nullable: true),
                    PatchAddress = table.Column<string>(type: "text", nullable: false),
                    MaxConnectionsPerAddress = table.Column<int>(type: "integer", nullable: false),
                    CheckMaxConnectionsPerAddress = table.Column<bool>(type: "boolean", nullable: false),
                    MaxConnections = table.Column<int>(type: "integer", nullable: false),
                    ListenerBacklog = table.Column<int>(type: "integer", nullable: false),
                    MaxFtpRequests = table.Column<int>(type: "integer", nullable: false),
                    MaxIpRequests = table.Column<int>(type: "integer", nullable: false),
                    MaxServerListRequests = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectServerDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConnectServerDefinition_GameClientDefinition_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "config",
                        principalTable: "GameClientDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttributeDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Designation = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemLevelBonusTable",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemLevelBonusTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemLevelBonusTable_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemOptionDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AddsRandomly = table.Column<bool>(type: "boolean", nullable: false),
                    AddChance = table.Column<float>(type: "real", nullable: false),
                    MaximumOptionsPerItem = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOptionDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOptionDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemOptionType",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOptionType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOptionType_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemSetGroup",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AlwaysApplies = table.Column<bool>(type: "boolean", nullable: false),
                    CountDistinct = table.Column<bool>(type: "boolean", nullable: false),
                    MinimumItemCount = table.Column<int>(type: "integer", nullable: false),
                    SetLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSetGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemSetGroup_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemSlotType",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemSlots = table.Column<string>(type: "text", nullable: true),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSlotType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemSlotType_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MasterSkillRoot",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterSkillRoot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterSkillRoot_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlugInConfiguration",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CustomPlugInSource = table.Column<string>(type: "text", nullable: true),
                    ExternalAssemblyName = table.Column<string>(type: "text", nullable: true),
                    CustomConfiguration = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlugInConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlugInConfiguration_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameServerDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServerConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ServerID = table.Column<byte>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ExperienceRate = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameServerDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameServerDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameServerDefinition_GameServerConfiguration_ServerConfigur~",
                        column: x => x.ServerConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameServerConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Account",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VaultId = table.Column<Guid>(type: "uuid", nullable: true),
                    LoginName = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    SecurityCode = table.Column<string>(type: "text", nullable: false),
                    EMail = table.Column<string>(type: "text", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    TimeZone = table.Column<short>(type: "smallint", nullable: false),
                    VaultPassword = table.Column<string>(type: "text", nullable: false),
                    IsVaultExtended = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_ItemStorage_VaultId",
                        column: x => x.VaultId,
                        principalSchema: "data",
                        principalTable: "ItemStorage",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MagicEffectDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Number = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SubType = table.Column<byte>(type: "smallint", nullable: false),
                    InformObservers = table.Column<bool>(type: "boolean", nullable: false),
                    StopByDeath = table.Column<bool>(type: "boolean", nullable: false),
                    SendDuration = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicEffectDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagicEffectDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MagicEffectDefinition_PowerUpDefinitionValue_DurationId",
                        column: x => x.DurationId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinitionValue",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BattleZoneDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroundId = table.Column<Guid>(type: "uuid", nullable: true),
                    LeftGoalId = table.Column<Guid>(type: "uuid", nullable: true),
                    RightGoalId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    LeftTeamSpawnPointX = table.Column<byte>(type: "smallint", nullable: true),
                    LeftTeamSpawnPointY = table.Column<byte>(type: "smallint", nullable: false),
                    RightTeamSpawnPointX = table.Column<byte>(type: "smallint", nullable: true),
                    RightTeamSpawnPointY = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleZoneDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattleZoneDefinition_Rectangle_GroundId",
                        column: x => x.GroundId,
                        principalSchema: "config",
                        principalTable: "Rectangle",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BattleZoneDefinition_Rectangle_LeftGoalId",
                        column: x => x.LeftGoalId,
                        principalSchema: "config",
                        principalTable: "Rectangle",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BattleZoneDefinition_Rectangle_RightGoalId",
                        column: x => x.RightGoalId,
                        principalSchema: "config",
                        principalTable: "Rectangle",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemCraftingRequiredItem",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SimpleCraftingSettingsId = table.Column<Guid>(type: "uuid", nullable: true),
                    MinimumItemLevel = table.Column<byte>(type: "smallint", nullable: false),
                    MaximumItemLevel = table.Column<byte>(type: "smallint", nullable: false),
                    MinimumAmount = table.Column<byte>(type: "smallint", nullable: false),
                    MaximumAmount = table.Column<byte>(type: "smallint", nullable: false),
                    SuccessResult = table.Column<int>(type: "integer", nullable: false),
                    FailResult = table.Column<int>(type: "integer", nullable: false),
                    NpcPriceDivisor = table.Column<int>(type: "integer", nullable: false),
                    AddPercentage = table.Column<byte>(type: "smallint", nullable: false),
                    Reference = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCraftingRequiredItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemCraftingRequiredItem_SimpleCraftingSettings_SimpleCraft~",
                        column: x => x.SimpleCraftingSettingsId,
                        principalSchema: "config",
                        principalTable: "SimpleCraftingSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LevelBonus",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemLevelBonusTableId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    AdditionalValue = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelBonus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LevelBonus_ItemLevelBonusTable_ItemLevelBonusTableId",
                        column: x => x.ItemLevelBonusTableId,
                        principalSchema: "config",
                        principalTable: "ItemLevelBonusTable",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameServerEndpoint",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameServerDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    NetworkPort = table.Column<int>(type: "integer", nullable: false),
                    AlternativePublishedPort = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameServerEndpoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameServerEndpoint_GameClientDefinition_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "config",
                        principalTable: "GameClientDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameServerEndpoint_GameServerDefinition_GameServerDefinitio~",
                        column: x => x.GameServerDefinitionId,
                        principalSchema: "config",
                        principalTable: "GameServerDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PowerUpDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetAttributeId = table.Column<Guid>(type: "uuid", nullable: true),
                    BoostId = table.Column<Guid>(type: "uuid", nullable: true),
                    MagicEffectDefinitionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerUpDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerUpDefinition_AttributeDefinition_TargetAttributeId",
                        column: x => x.TargetAttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PowerUpDefinition_MagicEffectDefinition_MagicEffectDefiniti~",
                        column: x => x.MagicEffectDefinitionId,
                        principalSchema: "config",
                        principalTable: "MagicEffectDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PowerUpDefinition_PowerUpDefinitionValue_BoostId",
                        column: x => x.BoostId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinitionValue",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameMapDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SafezoneMapId = table.Column<Guid>(type: "uuid", nullable: true),
                    BattleZoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Number = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TerrainData = table.Column<byte[]>(type: "bytea", nullable: true),
                    ExpMultiplier = table.Column<double>(type: "double precision", nullable: false),
                    Discriminator = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameMapDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameMapDefinition_BattleZoneDefinition_BattleZoneId",
                        column: x => x.BattleZoneId,
                        principalSchema: "config",
                        principalTable: "BattleZoneDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameMapDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameMapDefinition_GameMapDefinition_SafezoneMapId",
                        column: x => x.SafezoneMapId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemCraftingRequiredItemItemOptionType",
                schema: "config",
                columns: table => new
                {
                    ItemCraftingRequiredItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemOptionTypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCraftingRequiredItemItemOptionType", x => new { x.ItemCraftingRequiredItemId, x.ItemOptionTypeId });
                    table.ForeignKey(
                        name: "FK_ItemCraftingRequiredItemItemOptionType_ItemCraftingRequired~",
                        column: x => x.ItemCraftingRequiredItemId,
                        principalSchema: "config",
                        principalTable: "ItemCraftingRequiredItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemCraftingRequiredItemItemOptionType_ItemOptionType_ItemO~",
                        column: x => x.ItemOptionTypeId,
                        principalSchema: "config",
                        principalTable: "ItemOptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncreasableItemOption",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OptionTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    PowerUpDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ItemOptionDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ItemSetGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    SubOptionType = table.Column<int>(type: "integer", nullable: false),
                    LevelType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncreasableItemOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncreasableItemOption_ItemOptionDefinition_ItemOptionDefini~",
                        column: x => x.ItemOptionDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemOptionDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncreasableItemOption_ItemOptionType_OptionTypeId",
                        column: x => x.OptionTypeId,
                        principalSchema: "config",
                        principalTable: "ItemOptionType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncreasableItemOption_ItemSetGroup_ItemSetGroupId",
                        column: x => x.ItemSetGroupId,
                        principalSchema: "config",
                        principalTable: "ItemSetGroup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncreasableItemOption_PowerUpDefinition_PowerUpDefinitionId",
                        column: x => x.PowerUpDefinitionId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemOptionCombinationBonus",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BonusId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    AppliesMultipleTimes = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOptionCombinationBonus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOptionCombinationBonus_GameConfiguration_GameConfigurat~",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemOptionCombinationBonus_PowerUpDefinition_BonusId",
                        column: x => x.BonusId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CharacterClass",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NextGenerationClassId = table.Column<Guid>(type: "uuid", nullable: true),
                    HomeMapId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Number = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CanGetCreated = table.Column<bool>(type: "boolean", nullable: false),
                    LevelRequirementByCreation = table.Column<short>(type: "smallint", nullable: false),
                    CreationAllowedFlag = table.Column<byte>(type: "smallint", nullable: false),
                    IsMasterClass = table.Column<bool>(type: "boolean", nullable: false),
                    LevelWarpRequirementReductionPercent = table.Column<int>(type: "integer", nullable: false),
                    FruitCalculation = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterClass_CharacterClass_NextGenerationClassId",
                        column: x => x.NextGenerationClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterClass_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterClass_GameMapDefinition_HomeMapId",
                        column: x => x.HomeMapId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExitGate",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MapId = table.Column<Guid>(type: "uuid", nullable: true),
                    X1 = table.Column<byte>(type: "smallint", nullable: false),
                    Y1 = table.Column<byte>(type: "smallint", nullable: false),
                    X2 = table.Column<byte>(type: "smallint", nullable: false),
                    Y2 = table.Column<byte>(type: "smallint", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    IsSpawnGate = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExitGate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExitGate_GameMapDefinition_MapId",
                        column: x => x.MapId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameServerConfigurationGameMapDefinition",
                schema: "config",
                columns: table => new
                {
                    GameServerConfigurationId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameMapDefinitionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameServerConfigurationGameMapDefinition", x => new { x.GameServerConfigurationId, x.GameMapDefinitionId });
                    table.ForeignKey(
                        name: "FK_GameServerConfigurationGameMapDefinition_GameMapDefinition_~",
                        column: x => x.GameMapDefinitionId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameServerConfigurationGameMapDefinition_GameServerConfigur~",
                        column: x => x.GameServerConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameServerConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemOptionOfLevel",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PowerUpDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    IncreasableItemOptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    RequiredItemLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOptionOfLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOptionOfLevel_IncreasableItemOption_IncreasableItemOpti~",
                        column: x => x.IncreasableItemOptionId,
                        principalSchema: "config",
                        principalTable: "IncreasableItemOption",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemOptionOfLevel_PowerUpDefinition_PowerUpDefinitionId",
                        column: x => x.PowerUpDefinitionId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CombinationBonusRequirement",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OptionTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ItemOptionCombinationBonusId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubOptionType = table.Column<int>(type: "integer", nullable: false),
                    MinimumCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombinationBonusRequirement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombinationBonusRequirement_ItemOptionCombinationBonus_Item~",
                        column: x => x.ItemOptionCombinationBonusId,
                        principalSchema: "config",
                        principalTable: "ItemOptionCombinationBonus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CombinationBonusRequirement_ItemOptionType_OptionTypeId",
                        column: x => x.OptionTypeId,
                        principalSchema: "config",
                        principalTable: "ItemOptionType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccountCharacterClass",
                schema: "data",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterClassId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCharacterClass", x => new { x.AccountId, x.CharacterClassId });
                    table.ForeignKey(
                        name: "FK_AccountCharacterClass_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "data",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountCharacterClass_CharacterClass_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppearanceData",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterClassId = table.Column<Guid>(type: "uuid", nullable: true),
                    Pose = table.Column<byte>(type: "smallint", nullable: false),
                    FullAncientSetEquipped = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppearanceData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppearanceData_CharacterClass_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttributeRelationship",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetAttributeId = table.Column<Guid>(type: "uuid", nullable: true),
                    InputAttributeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CharacterClassId = table.Column<Guid>(type: "uuid", nullable: true),
                    PowerUpDefinitionValueId = table.Column<Guid>(type: "uuid", nullable: true),
                    InputOperator = table.Column<int>(type: "integer", nullable: false),
                    InputOperand = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeRelationship", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeRelationship_AttributeDefinition_InputAttributeId",
                        column: x => x.InputAttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttributeRelationship_AttributeDefinition_TargetAttributeId",
                        column: x => x.TargetAttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttributeRelationship_CharacterClass_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttributeRelationship_PowerUpDefinitionValue_PowerUpDefinit~",
                        column: x => x.PowerUpDefinitionValueId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinitionValue",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Character",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentMapId = table.Column<Guid>(type: "uuid", nullable: true),
                    InventoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CharacterSlot = table.Column<byte>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Experience = table.Column<long>(type: "bigint", nullable: false),
                    MasterExperience = table.Column<long>(type: "bigint", nullable: false),
                    LevelUpPoints = table.Column<int>(type: "integer", nullable: false),
                    MasterLevelUpPoints = table.Column<int>(type: "integer", nullable: false),
                    PositionX = table.Column<byte>(type: "smallint", nullable: false),
                    PositionY = table.Column<byte>(type: "smallint", nullable: false),
                    PlayerKillCount = table.Column<int>(type: "integer", nullable: false),
                    StateRemainingSeconds = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    CharacterStatus = table.Column<int>(type: "integer", nullable: false),
                    Pose = table.Column<byte>(type: "smallint", nullable: false),
                    UsedFruitPoints = table.Column<int>(type: "integer", nullable: false),
                    UsedNegFruitPoints = table.Column<int>(type: "integer", nullable: false),
                    InventoryExtensions = table.Column<int>(type: "integer", nullable: false),
                    KeyConfiguration = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Account_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "data",
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Character_CharacterClass_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Character_GameMapDefinition_CurrentMapId",
                        column: x => x.CurrentMapId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Character_ItemStorage_InventoryId",
                        column: x => x.InventoryId,
                        principalSchema: "data",
                        principalTable: "ItemStorage",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ConstValueAttribute",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CharacterClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstValueAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConstValueAttribute_AttributeDefinition_DefinitionId",
                        column: x => x.DefinitionId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConstValueAttribute_CharacterClass_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatAttributeDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CharacterClassId = table.Column<Guid>(type: "uuid", nullable: true),
                    BaseValue = table.Column<float>(type: "real", nullable: false),
                    IncreasableByPlayer = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatAttributeDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatAttributeDefinition_AttributeDefinition_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StatAttributeDefinition_CharacterClass_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EnterGate",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetGateId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameMapDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    X1 = table.Column<byte>(type: "smallint", nullable: false),
                    Y1 = table.Column<byte>(type: "smallint", nullable: false),
                    X2 = table.Column<byte>(type: "smallint", nullable: false),
                    Y2 = table.Column<byte>(type: "smallint", nullable: false),
                    LevelRequirement = table.Column<short>(type: "smallint", nullable: false),
                    Number = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnterGate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnterGate_ExitGate_TargetGateId",
                        column: x => x.TargetGateId,
                        principalSchema: "config",
                        principalTable: "ExitGate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EnterGate_GameMapDefinition_GameMapDefinitionId",
                        column: x => x.GameMapDefinitionId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarpInfo",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GateId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Costs = table.Column<int>(type: "integer", nullable: false),
                    LevelRequirement = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarpInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarpInfo_ExitGate_GateId",
                        column: x => x.GateId,
                        principalSchema: "config",
                        principalTable: "ExitGate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WarpInfo_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GuildMember",
                schema: "guild",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GuildId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuildMember_Character_Id",
                        column: x => x.Id,
                        principalSchema: "data",
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildMember_Guild_GuildId",
                        column: x => x.GuildId,
                        principalSchema: "guild",
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LetterHeader",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderName = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "text", nullable: true),
                    LetterDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReadFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LetterHeader_Character_ReceiverId",
                        column: x => x.ReceiverId,
                        principalSchema: "data",
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatAttribute",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatAttribute_AttributeDefinition_DefinitionId",
                        column: x => x.DefinitionId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StatAttribute_Character_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "data",
                        principalTable: "Character",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LetterBody",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeaderId = table.Column<Guid>(type: "uuid", nullable: true),
                    SenderAppearanceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Rotation = table.Column<byte>(type: "smallint", nullable: false),
                    Animation = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterBody", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LetterBody_AppearanceData_SenderAppearanceId",
                        column: x => x.SenderAppearanceId,
                        principalSchema: "data",
                        principalTable: "AppearanceData",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LetterBody_LetterHeader_HeaderId",
                        column: x => x.HeaderId,
                        principalSchema: "data",
                        principalTable: "LetterHeader",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CharacterDropItemGroup",
                schema: "data",
                columns: table => new
                {
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                    DropItemGroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterDropItemGroup", x => new { x.CharacterId, x.DropItemGroupId });
                    table.ForeignKey(
                        name: "FK_CharacterDropItemGroup_Character_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "data",
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterQuestState",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LastFinishedQuestId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActiveQuestId = table.Column<Guid>(type: "uuid", nullable: true),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: true),
                    Group = table.Column<short>(type: "smallint", nullable: false),
                    ClientActionPerformed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterQuestState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterQuestState_Character_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "data",
                        principalTable: "Character",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DropItemGroup",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonsterId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Chance = table.Column<double>(type: "double precision", nullable: false),
                    MinimumMonsterLevel = table.Column<byte>(type: "smallint", nullable: true),
                    MaximumMonsterLevel = table.Column<byte>(type: "smallint", nullable: true),
                    ItemLevel = table.Column<byte>(type: "smallint", nullable: true),
                    ItemType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DropItemGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DropItemGroup_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameMapDefinitionDropItemGroup",
                schema: "config",
                columns: table => new
                {
                    GameMapDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DropItemGroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameMapDefinitionDropItemGroup", x => new { x.GameMapDefinitionId, x.DropItemGroupId });
                    table.ForeignKey(
                        name: "FK_GameMapDefinitionDropItemGroup_DropItemGroup_DropItemGroupId",
                        column: x => x.DropItemGroupId,
                        principalSchema: "config",
                        principalTable: "DropItemGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameMapDefinitionDropItemGroup_GameMapDefinition_GameMapDef~",
                        column: x => x.GameMapDefinitionId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DropItemGroupItemDefinition",
                schema: "config",
                columns: table => new
                {
                    DropItemGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DropItemGroupItemDefinition", x => new { x.DropItemGroupId, x.ItemDefinitionId });
                    table.ForeignKey(
                        name: "FK_DropItemGroupItemDefinition_DropItemGroup_DropItemGroupId",
                        column: x => x.DropItemGroupId,
                        principalSchema: "config",
                        principalTable: "DropItemGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemStorageId = table.Column<Guid>(type: "uuid", nullable: true),
                    DefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ItemSlot = table.Column<byte>(type: "smallint", nullable: false),
                    Durability = table.Column<double>(type: "double precision", nullable: false),
                    Level = table.Column<byte>(type: "smallint", nullable: false),
                    HasSkill = table.Column<bool>(type: "boolean", nullable: false),
                    SocketCount = table.Column<int>(type: "integer", nullable: false),
                    StorePrice = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_ItemStorage_ItemStorageId",
                        column: x => x.ItemStorageId,
                        principalSchema: "data",
                        principalTable: "ItemStorage",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemOptionLink",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemOptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOptionLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOptionLink_IncreasableItemOption_ItemOptionId",
                        column: x => x.ItemOptionId,
                        principalSchema: "config",
                        principalTable: "IncreasableItemOption",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemOptionLink_Item_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "data",
                        principalTable: "Item",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemAppearance",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    AppearanceDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    ItemSlot = table.Column<byte>(type: "smallint", nullable: false),
                    Level = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAppearance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemAppearance_AppearanceData_AppearanceDataId",
                        column: x => x.AppearanceDataId,
                        principalSchema: "data",
                        principalTable: "AppearanceData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemAppearanceItemOptionType",
                schema: "data",
                columns: table => new
                {
                    ItemAppearanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemOptionTypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAppearanceItemOptionType", x => new { x.ItemAppearanceId, x.ItemOptionTypeId });
                    table.ForeignKey(
                        name: "FK_ItemAppearanceItemOptionType_ItemAppearance_ItemAppearanceId",
                        column: x => x.ItemAppearanceId,
                        principalSchema: "data",
                        principalTable: "ItemAppearance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemAppearanceItemOptionType_ItemOptionType_ItemOptionTypeId",
                        column: x => x.ItemOptionTypeId,
                        principalSchema: "config",
                        principalTable: "ItemOptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemBasePowerUpDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetAttributeId = table.Column<Guid>(type: "uuid", nullable: true),
                    BonusPerLevelTableId = table.Column<Guid>(type: "uuid", nullable: true),
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    BaseValue = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemBasePowerUpDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemBasePowerUpDefinition_AttributeDefinition_TargetAttribu~",
                        column: x => x.TargetAttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemBasePowerUpDefinition_ItemLevelBonusTable_BonusPerLevel~",
                        column: x => x.BonusPerLevelTableId,
                        principalSchema: "config",
                        principalTable: "ItemLevelBonusTable",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemCrafting",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SimpleCraftingSettingsId = table.Column<Guid>(type: "uuid", nullable: true),
                    MonsterDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Number = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ItemCraftingHandlerClassName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCrafting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemCrafting_SimpleCraftingSettings_SimpleCraftingSettingsId",
                        column: x => x.SimpleCraftingSettingsId,
                        principalSchema: "config",
                        principalTable: "SimpleCraftingSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemCraftingRequiredItemItemDefinition",
                schema: "config",
                columns: table => new
                {
                    ItemCraftingRequiredItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCraftingRequiredItemItemDefinition", x => new { x.ItemCraftingRequiredItemId, x.ItemDefinitionId });
                    table.ForeignKey(
                        name: "FK_ItemCraftingRequiredItemItemDefinition_ItemCraftingRequired~",
                        column: x => x.ItemCraftingRequiredItemId,
                        principalSchema: "config",
                        principalTable: "ItemCraftingRequiredItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemCraftingResultItem",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SimpleCraftingSettingsId = table.Column<Guid>(type: "uuid", nullable: true),
                    RandomMinimumLevel = table.Column<byte>(type: "smallint", nullable: false),
                    RandomMaximumLevel = table.Column<byte>(type: "smallint", nullable: false),
                    Durability = table.Column<byte>(type: "smallint", nullable: true),
                    Reference = table.Column<byte>(type: "smallint", nullable: false),
                    AddLevel = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCraftingResultItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemCraftingResultItem_SimpleCraftingSettings_SimpleCraftin~",
                        column: x => x.SimpleCraftingSettingsId,
                        principalSchema: "config",
                        principalTable: "SimpleCraftingSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemSlotId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConsumeEffectId = table.Column<Guid>(type: "uuid", nullable: true),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Number = table.Column<short>(type: "smallint", nullable: false),
                    Width = table.Column<byte>(type: "smallint", nullable: false),
                    Height = table.Column<byte>(type: "smallint", nullable: false),
                    DropsFromMonsters = table.Column<bool>(type: "boolean", nullable: false),
                    IsAmmunition = table.Column<bool>(type: "boolean", nullable: false),
                    IsBoundToCharacter = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DropLevel = table.Column<byte>(type: "smallint", nullable: false),
                    MaximumItemLevel = table.Column<byte>(type: "smallint", nullable: false),
                    Durability = table.Column<byte>(type: "smallint", nullable: false),
                    Group = table.Column<byte>(type: "smallint", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    ConsumeHandlerClass = table.Column<string>(type: "text", nullable: true),
                    MaximumSockets = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemDefinition_ItemSlotType_ItemSlotId",
                        column: x => x.ItemSlotId,
                        principalSchema: "config",
                        principalTable: "ItemSlotType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemDefinition_MagicEffectDefinition_ConsumeEffectId",
                        column: x => x.ConsumeEffectId,
                        principalSchema: "config",
                        principalTable: "MagicEffectDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemDefinitionCharacterClass",
                schema: "config",
                columns: table => new
                {
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterClassId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDefinitionCharacterClass", x => new { x.ItemDefinitionId, x.CharacterClassId });
                    table.ForeignKey(
                        name: "FK_ItemDefinitionCharacterClass_CharacterClass_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemDefinitionCharacterClass_ItemDefinition_ItemDefinitionId",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemDefinitionItemOptionDefinition",
                schema: "config",
                columns: table => new
                {
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemOptionDefinitionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDefinitionItemOptionDefinition", x => new { x.ItemDefinitionId, x.ItemOptionDefinitionId });
                    table.ForeignKey(
                        name: "FK_ItemDefinitionItemOptionDefinition_ItemDefinition_ItemDefin~",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemDefinitionItemOptionDefinition_ItemOptionDefinition_Ite~",
                        column: x => x.ItemOptionDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemOptionDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemDefinitionItemSetGroup",
                schema: "config",
                columns: table => new
                {
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemSetGroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDefinitionItemSetGroup", x => new { x.ItemDefinitionId, x.ItemSetGroupId });
                    table.ForeignKey(
                        name: "FK_ItemDefinitionItemSetGroup_ItemDefinition_ItemDefinitionId",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemDefinitionItemSetGroup_ItemSetGroup_ItemSetGroupId",
                        column: x => x.ItemSetGroupId,
                        principalSchema: "config",
                        principalTable: "ItemSetGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemOfItemSet",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemSetGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    BonusOptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    AncientSetDiscriminator = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOfItemSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOfItemSet_IncreasableItemOption_BonusOptionId",
                        column: x => x.BonusOptionId,
                        principalSchema: "config",
                        principalTable: "IncreasableItemOption",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemOfItemSet_ItemDefinition_ItemDefinitionId",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemOfItemSet_ItemSetGroup_ItemSetGroupId",
                        column: x => x.ItemSetGroupId,
                        principalSchema: "config",
                        principalTable: "ItemSetGroup",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JewelMix",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SingleJewelId = table.Column<Guid>(type: "uuid", nullable: true),
                    MixedJewelId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Number = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JewelMix", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JewelMix_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JewelMix_ItemDefinition_MixedJewelId",
                        column: x => x.MixedJewelId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JewelMix_ItemDefinition_SingleJewelId",
                        column: x => x.SingleJewelId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MiniGameDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntranceId = table.Column<Guid>(type: "uuid", nullable: true),
                    TicketItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    GameLevel = table.Column<byte>(type: "smallint", nullable: false),
                    MapCreationPolicy = table.Column<int>(type: "integer", nullable: false),
                    EnterDuration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    GameDuration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ExitDuration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    MaximumPlayerCount = table.Column<int>(type: "integer", nullable: false),
                    SaveRankingStatistics = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresMasterClass = table.Column<bool>(type: "boolean", nullable: false),
                    MinimumCharacterLevel = table.Column<int>(type: "integer", nullable: false),
                    MaximumCharacterLevel = table.Column<int>(type: "integer", nullable: false),
                    MinimumSpecialCharacterLevel = table.Column<int>(type: "integer", nullable: false),
                    MaximumSpecialCharacterLevel = table.Column<int>(type: "integer", nullable: false),
                    TicketItemLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniGameDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniGameDefinition_ExitGate_EntranceId",
                        column: x => x.EntranceId,
                        principalSchema: "config",
                        principalTable: "ExitGate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MiniGameDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MiniGameDefinition_ItemDefinition_TicketItemId",
                        column: x => x.TicketItemId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemItemOfItemSet",
                schema: "data",
                columns: table => new
                {
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemOfItemSetId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemItemOfItemSet", x => new { x.ItemId, x.ItemOfItemSetId });
                    table.ForeignKey(
                        name: "FK_ItemItemOfItemSet_Item_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "data",
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemItemOfItemSet_ItemOfItemSet_ItemOfItemSetId",
                        column: x => x.ItemOfItemSetId,
                        principalSchema: "config",
                        principalTable: "ItemOfItemSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiniGameRankingEntry",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: true),
                    MiniGameId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniGameRankingEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniGameRankingEntry_Character_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "data",
                        principalTable: "Character",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MiniGameRankingEntry_MiniGameDefinition_MiniGameId",
                        column: x => x.MiniGameId,
                        principalSchema: "config",
                        principalTable: "MiniGameDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MiniGameSpawnWave",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MiniGameDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    WaveNumber = table.Column<byte>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniGameSpawnWave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniGameSpawnWave_MiniGameDefinition_MiniGameDefinitionId",
                        column: x => x.MiniGameDefinitionId,
                        principalSchema: "config",
                        principalTable: "MiniGameDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttributeRequirement",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameMapDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: true),
                    SkillId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    MinimumValue = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeRequirement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeRequirement_AttributeDefinition_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttributeRequirement_GameMapDefinition_GameMapDefinitionId",
                        column: x => x.GameMapDefinitionId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttributeRequirement_ItemDefinition_ItemDefinitionId",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemDropItemGroup",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonsterId = table.Column<Guid>(type: "uuid", nullable: true),
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Chance = table.Column<double>(type: "double precision", nullable: false),
                    MinimumMonsterLevel = table.Column<byte>(type: "smallint", nullable: true),
                    MaximumMonsterLevel = table.Column<byte>(type: "smallint", nullable: true),
                    ItemLevel = table.Column<byte>(type: "smallint", nullable: true),
                    ItemType = table.Column<int>(type: "integer", nullable: false),
                    SourceItemLevel = table.Column<byte>(type: "smallint", nullable: false),
                    MoneyAmount = table.Column<int>(type: "integer", nullable: false),
                    MinimumLevel = table.Column<byte>(type: "smallint", nullable: false),
                    MaximumLevel = table.Column<byte>(type: "smallint", nullable: false),
                    RequiredCharacterLevel = table.Column<short>(type: "smallint", nullable: false),
                    DropEffect = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDropItemGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemDropItemGroup_ItemDefinition_ItemDefinitionId",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemDropItemGroupItemDefinition",
                schema: "config",
                columns: table => new
                {
                    ItemDropItemGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemDefinitionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDropItemGroupItemDefinition", x => new { x.ItemDropItemGroupId, x.ItemDefinitionId });
                    table.ForeignKey(
                        name: "FK_ItemDropItemGroupItemDefinition_ItemDefinition_ItemDefiniti~",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemDropItemGroupItemDefinition_ItemDropItemGroup_ItemDropI~",
                        column: x => x.ItemDropItemGroupId,
                        principalSchema: "config",
                        principalTable: "ItemDropItemGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterSkillDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RootId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetAttributeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReplacedSkillId = table.Column<Guid>(type: "uuid", nullable: true),
                    Rank = table.Column<byte>(type: "smallint", nullable: false),
                    MaximumLevel = table.Column<byte>(type: "smallint", nullable: false),
                    MinimumLevel = table.Column<byte>(type: "smallint", nullable: false),
                    ValueFormula = table.Column<string>(type: "text", nullable: false),
                    DisplayValueFormula = table.Column<string>(type: "text", nullable: false),
                    Aggregation = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterSkillDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterSkillDefinition_AttributeDefinition_TargetAttributeId",
                        column: x => x.TargetAttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MasterSkillDefinition_MasterSkillRoot_RootId",
                        column: x => x.RootId,
                        principalSchema: "config",
                        principalTable: "MasterSkillRoot",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElementalModifierTargetId = table.Column<Guid>(type: "uuid", nullable: true),
                    MagicEffectDefId = table.Column<Guid>(type: "uuid", nullable: true),
                    MasterDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Number = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Range = table.Column<short>(type: "smallint", nullable: false),
                    DamageType = table.Column<int>(type: "integer", nullable: false),
                    SkillType = table.Column<int>(type: "integer", nullable: false),
                    Target = table.Column<int>(type: "integer", nullable: false),
                    ImplicitTargetRange = table.Column<short>(type: "smallint", nullable: false),
                    TargetRestriction = table.Column<int>(type: "integer", nullable: false),
                    MovesToTarget = table.Column<bool>(type: "boolean", nullable: false),
                    MovesTarget = table.Column<bool>(type: "boolean", nullable: false),
                    AttackDamage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skill_AttributeDefinition_ElementalModifierTargetId",
                        column: x => x.ElementalModifierTargetId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Skill_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Skill_MagicEffectDefinition_MagicEffectDefId",
                        column: x => x.MagicEffectDefId,
                        principalSchema: "config",
                        principalTable: "MagicEffectDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Skill_MasterSkillDefinition_MasterDefinitionId",
                        column: x => x.MasterDefinitionId,
                        principalSchema: "config",
                        principalTable: "MasterSkillDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MasterSkillDefinitionSkill",
                schema: "config",
                columns: table => new
                {
                    MasterSkillDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterSkillDefinitionSkill", x => new { x.MasterSkillDefinitionId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_MasterSkillDefinitionSkill_MasterSkillDefinition_MasterSkil~",
                        column: x => x.MasterSkillDefinitionId,
                        principalSchema: "config",
                        principalTable: "MasterSkillDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasterSkillDefinitionSkill_Skill_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "config",
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonsterDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttackSkillId = table.Column<Guid>(type: "uuid", nullable: true),
                    MerchantStoreId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameConfigurationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Number = table.Column<short>(type: "smallint", nullable: false),
                    Designation = table.Column<string>(type: "text", nullable: false),
                    MoveRange = table.Column<byte>(type: "smallint", nullable: false),
                    AttackRange = table.Column<byte>(type: "smallint", nullable: false),
                    ViewRange = table.Column<short>(type: "smallint", nullable: false),
                    MoveDelay = table.Column<TimeSpan>(type: "interval", nullable: false),
                    AttackDelay = table.Column<TimeSpan>(type: "interval", nullable: false),
                    RespawnDelay = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Attribute = table.Column<byte>(type: "smallint", nullable: false),
                    NumberOfMaximumItemDrops = table.Column<int>(type: "integer", nullable: false),
                    NpcWindow = table.Column<int>(type: "integer", nullable: false),
                    ObjectKind = table.Column<int>(type: "integer", nullable: false),
                    IntelligenceTypeName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MonsterDefinition_ItemStorage_MerchantStoreId",
                        column: x => x.MerchantStoreId,
                        principalSchema: "data",
                        principalTable: "ItemStorage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MonsterDefinition_Skill_AttackSkillId",
                        column: x => x.AttackSkillId,
                        principalSchema: "config",
                        principalTable: "Skill",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SkillCharacterClass",
                schema: "config",
                columns: table => new
                {
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterClassId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillCharacterClass", x => new { x.SkillId, x.CharacterClassId });
                    table.ForeignKey(
                        name: "FK_SkillCharacterClass_CharacterClass_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillCharacterClass_Skill_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "config",
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkillEntry",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: true),
                    CharacterId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillEntry_Character_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "data",
                        principalTable: "Character",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SkillEntry_Skill_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "config",
                        principalTable: "Skill",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MiniGameReward",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemRewardId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequiredKillId = table.Column<Guid>(type: "uuid", nullable: true),
                    MiniGameDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    RewardType = table.Column<int>(type: "integer", nullable: false),
                    RewardAmount = table.Column<int>(type: "integer", nullable: false),
                    RequiredSuccess = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniGameReward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniGameReward_DropItemGroup_ItemRewardId",
                        column: x => x.ItemRewardId,
                        principalSchema: "config",
                        principalTable: "DropItemGroup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MiniGameReward_MiniGameDefinition_MiniGameDefinitionId",
                        column: x => x.MiniGameDefinitionId,
                        principalSchema: "config",
                        principalTable: "MiniGameDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MiniGameReward_MonsterDefinition_RequiredKillId",
                        column: x => x.RequiredKillId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MonsterAttribute",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    MonsterDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterAttribute_AttributeDefinition_AttributeDefinitionId",
                        column: x => x.AttributeDefinitionId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MonsterAttribute_MonsterDefinition_MonsterDefinitionId",
                        column: x => x.MonsterDefinitionId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MonsterDefinitionDropItemGroup",
                schema: "config",
                columns: table => new
                {
                    MonsterDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DropItemGroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterDefinitionDropItemGroup", x => new { x.MonsterDefinitionId, x.DropItemGroupId });
                    table.ForeignKey(
                        name: "FK_MonsterDefinitionDropItemGroup_DropItemGroup_DropItemGroupId",
                        column: x => x.DropItemGroupId,
                        principalSchema: "config",
                        principalTable: "DropItemGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonsterDefinitionDropItemGroup_MonsterDefinition_MonsterDef~",
                        column: x => x.MonsterDefinitionId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonsterSpawnArea",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonsterDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    GameMapId = table.Column<Guid>(type: "uuid", nullable: true),
                    X1 = table.Column<byte>(type: "smallint", nullable: false),
                    Y1 = table.Column<byte>(type: "smallint", nullable: false),
                    X2 = table.Column<byte>(type: "smallint", nullable: false),
                    Y2 = table.Column<byte>(type: "smallint", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<short>(type: "smallint", nullable: false),
                    SpawnTrigger = table.Column<int>(type: "integer", nullable: false),
                    WaveNumber = table.Column<byte>(type: "smallint", nullable: false),
                    MaximumHealthOverride = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterSpawnArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterSpawnArea_GameMapDefinition_GameMapId",
                        column: x => x.GameMapId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MonsterSpawnArea_MonsterDefinition_MonsterDefinitionId",
                        column: x => x.MonsterDefinitionId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuestDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestGiverId = table.Column<Guid>(type: "uuid", nullable: true),
                    QualifiedCharacterId = table.Column<Guid>(type: "uuid", nullable: true),
                    MonsterDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<short>(type: "smallint", nullable: false),
                    Number = table.Column<short>(type: "smallint", nullable: false),
                    StartingNumber = table.Column<short>(type: "smallint", nullable: false),
                    RefuseNumber = table.Column<short>(type: "smallint", nullable: false),
                    Repeatable = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresClientAction = table.Column<bool>(type: "boolean", nullable: false),
                    RequiredStartMoney = table.Column<int>(type: "integer", nullable: false),
                    MinimumCharacterLevel = table.Column<int>(type: "integer", nullable: false),
                    MaximumCharacterLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestDefinition_CharacterClass_QualifiedCharacterId",
                        column: x => x.QualifiedCharacterId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestDefinition_MonsterDefinition_MonsterDefinitionId",
                        column: x => x.MonsterDefinitionId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestDefinition_MonsterDefinition_QuestGiverId",
                        column: x => x.QuestGiverId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MiniGameChangeEvent",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SpawnAreaId = table.Column<Guid>(type: "uuid", nullable: true),
                    MiniGameDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    Target = table.Column<int>(type: "integer", nullable: false),
                    MinimumTargetLevel = table.Column<short>(type: "smallint", nullable: true),
                    NumberOfKills = table.Column<short>(type: "smallint", nullable: false),
                    MultiplyKillsByPlayers = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniGameChangeEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniGameChangeEvent_MiniGameDefinition_MiniGameDefinitionId",
                        column: x => x.MiniGameDefinitionId,
                        principalSchema: "config",
                        principalTable: "MiniGameDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MiniGameChangeEvent_MonsterDefinition_TargetDefinitionId",
                        column: x => x.TargetDefinitionId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MiniGameChangeEvent_MonsterSpawnArea_SpawnAreaId",
                        column: x => x.SpawnAreaId,
                        principalSchema: "config",
                        principalTable: "MonsterSpawnArea",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuestItemRequirement",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    DropItemGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuestDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    MinimumNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestItemRequirement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestItemRequirement_DropItemGroup_DropItemGroupId",
                        column: x => x.DropItemGroupId,
                        principalSchema: "config",
                        principalTable: "DropItemGroup",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestItemRequirement_ItemDefinition_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestItemRequirement_QuestDefinition_QuestDefinitionId",
                        column: x => x.QuestDefinitionId,
                        principalSchema: "config",
                        principalTable: "QuestDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuestMonsterKillRequirement",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonsterId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuestDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    MinimumNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestMonsterKillRequirement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestMonsterKillRequirement_MonsterDefinition_MonsterId",
                        column: x => x.MonsterId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestMonsterKillRequirement_QuestDefinition_QuestDefinition~",
                        column: x => x.QuestDefinitionId,
                        principalSchema: "config",
                        principalTable: "QuestDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuestReward",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemRewardId = table.Column<Guid>(type: "uuid", nullable: true),
                    AttributeRewardId = table.Column<Guid>(type: "uuid", nullable: true),
                    SkillRewardId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuestDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    RewardType = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestReward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestReward_AttributeDefinition_AttributeRewardId",
                        column: x => x.AttributeRewardId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestReward_Item_ItemRewardId",
                        column: x => x.ItemRewardId,
                        principalSchema: "data",
                        principalTable: "Item",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestReward_QuestDefinition_QuestDefinitionId",
                        column: x => x.QuestDefinitionId,
                        principalSchema: "config",
                        principalTable: "QuestDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestReward_Skill_SkillRewardId",
                        column: x => x.SkillRewardId,
                        principalSchema: "config",
                        principalTable: "Skill",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MiniGameTerrainChange",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MiniGameChangeEventId = table.Column<Guid>(type: "uuid", nullable: true),
                    TerrainAttribute = table.Column<int>(type: "integer", nullable: false),
                    SetTerrainAttribute = table.Column<bool>(type: "boolean", nullable: false),
                    StartX = table.Column<byte>(type: "smallint", nullable: false),
                    StartY = table.Column<byte>(type: "smallint", nullable: false),
                    EndX = table.Column<byte>(type: "smallint", nullable: false),
                    EndY = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniGameTerrainChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniGameTerrainChange_MiniGameChangeEvent_MiniGameChangeEve~",
                        column: x => x.MiniGameChangeEventId,
                        principalSchema: "config",
                        principalTable: "MiniGameChangeEvent",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuestMonsterKillRequirementState",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequirementId = table.Column<Guid>(type: "uuid", nullable: true),
                    CharacterQuestStateId = table.Column<Guid>(type: "uuid", nullable: true),
                    KillCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestMonsterKillRequirementState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestMonsterKillRequirementState_CharacterQuestState_Charac~",
                        column: x => x.CharacterQuestStateId,
                        principalSchema: "data",
                        principalTable: "CharacterQuestState",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestMonsterKillRequirementState_QuestMonsterKillRequiremen~",
                        column: x => x.RequirementId,
                        principalSchema: "config",
                        principalTable: "QuestMonsterKillRequirement",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_LoginName",
                schema: "data",
                table: "Account",
                column: "LoginName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_VaultId",
                schema: "data",
                table: "Account",
                column: "VaultId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCharacterClass_CharacterClassId",
                schema: "data",
                table: "AccountCharacterClass",
                column: "CharacterClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AppearanceData_CharacterClassId",
                schema: "data",
                table: "AppearanceData",
                column: "CharacterClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinition_GameConfigurationId",
                schema: "config",
                table: "AttributeDefinition",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRelationship_CharacterClassId",
                schema: "config",
                table: "AttributeRelationship",
                column: "CharacterClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRelationship_InputAttributeId",
                schema: "config",
                table: "AttributeRelationship",
                column: "InputAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRelationship_PowerUpDefinitionValueId",
                schema: "config",
                table: "AttributeRelationship",
                column: "PowerUpDefinitionValueId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRelationship_TargetAttributeId",
                schema: "config",
                table: "AttributeRelationship",
                column: "TargetAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRequirement_AttributeId",
                schema: "config",
                table: "AttributeRequirement",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRequirement_GameMapDefinitionId",
                schema: "config",
                table: "AttributeRequirement",
                column: "GameMapDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRequirement_ItemDefinitionId",
                schema: "config",
                table: "AttributeRequirement",
                column: "ItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRequirement_SkillId",
                schema: "config",
                table: "AttributeRequirement",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeRequirement_SkillId1",
                schema: "config",
                table: "AttributeRequirement",
                column: "SkillId1");

            migrationBuilder.CreateIndex(
                name: "IX_BattleZoneDefinition_GroundId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "GroundId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleZoneDefinition_LeftGoalId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "LeftGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleZoneDefinition_RightGoalId",
                schema: "config",
                table: "BattleZoneDefinition",
                column: "RightGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_AccountId",
                schema: "data",
                table: "Character",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_CharacterClassId",
                schema: "data",
                table: "Character",
                column: "CharacterClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_CurrentMapId",
                schema: "data",
                table: "Character",
                column: "CurrentMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_InventoryId",
                schema: "data",
                table: "Character",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_Name",
                schema: "data",
                table: "Character",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterClass_GameConfigurationId",
                schema: "config",
                table: "CharacterClass",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterClass_HomeMapId",
                schema: "config",
                table: "CharacterClass",
                column: "HomeMapId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterClass_NextGenerationClassId",
                schema: "config",
                table: "CharacterClass",
                column: "NextGenerationClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterDropItemGroup_DropItemGroupId",
                schema: "data",
                table: "CharacterDropItemGroup",
                column: "DropItemGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterQuestState_ActiveQuestId",
                schema: "data",
                table: "CharacterQuestState",
                column: "ActiveQuestId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterQuestState_CharacterId",
                schema: "data",
                table: "CharacterQuestState",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterQuestState_LastFinishedQuestId",
                schema: "data",
                table: "CharacterQuestState",
                column: "LastFinishedQuestId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatServerEndpoint_ChatServerDefinitionId",
                schema: "config",
                table: "ChatServerEndpoint",
                column: "ChatServerDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatServerEndpoint_ClientId",
                schema: "config",
                table: "ChatServerEndpoint",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CombinationBonusRequirement_ItemOptionCombinationBonusId",
                schema: "config",
                table: "CombinationBonusRequirement",
                column: "ItemOptionCombinationBonusId");

            migrationBuilder.CreateIndex(
                name: "IX_CombinationBonusRequirement_OptionTypeId",
                schema: "config",
                table: "CombinationBonusRequirement",
                column: "OptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectServerDefinition_ClientId",
                schema: "config",
                table: "ConnectServerDefinition",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ConstValueAttribute_CharacterClassId",
                schema: "config",
                table: "ConstValueAttribute",
                column: "CharacterClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ConstValueAttribute_DefinitionId",
                schema: "config",
                table: "ConstValueAttribute",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_DropItemGroup_GameConfigurationId",
                schema: "config",
                table: "DropItemGroup",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_DropItemGroup_MonsterId",
                schema: "config",
                table: "DropItemGroup",
                column: "MonsterId");

            migrationBuilder.CreateIndex(
                name: "IX_DropItemGroupItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "DropItemGroupItemDefinition",
                column: "ItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_EnterGate_GameMapDefinitionId",
                schema: "config",
                table: "EnterGate",
                column: "GameMapDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_EnterGate_TargetGateId",
                schema: "config",
                table: "EnterGate",
                column: "TargetGateId");

            migrationBuilder.CreateIndex(
                name: "IX_ExitGate_MapId",
                schema: "config",
                table: "ExitGate",
                column: "MapId");

            migrationBuilder.CreateIndex(
                name: "IX_GameMapDefinition_BattleZoneId",
                schema: "config",
                table: "GameMapDefinition",
                column: "BattleZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_GameMapDefinition_GameConfigurationId",
                schema: "config",
                table: "GameMapDefinition",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_GameMapDefinition_SafezoneMapId",
                schema: "config",
                table: "GameMapDefinition",
                column: "SafezoneMapId");

            migrationBuilder.CreateIndex(
                name: "IX_GameMapDefinitionDropItemGroup_DropItemGroupId",
                schema: "config",
                table: "GameMapDefinitionDropItemGroup",
                column: "DropItemGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GameServerConfigurationGameMapDefinition_GameMapDefinitionId",
                schema: "config",
                table: "GameServerConfigurationGameMapDefinition",
                column: "GameMapDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_GameServerDefinition_GameConfigurationId",
                schema: "config",
                table: "GameServerDefinition",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_GameServerDefinition_ServerConfigurationId",
                schema: "config",
                table: "GameServerDefinition",
                column: "ServerConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_GameServerEndpoint_ClientId",
                schema: "config",
                table: "GameServerEndpoint",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_GameServerEndpoint_GameServerDefinitionId",
                schema: "config",
                table: "GameServerEndpoint",
                column: "GameServerDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Guild_AllianceGuildId",
                schema: "guild",
                table: "Guild",
                column: "AllianceGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Guild_HostilityId",
                schema: "guild",
                table: "Guild",
                column: "HostilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Guild_Name",
                schema: "guild",
                table: "Guild",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuildMember_GuildId",
                schema: "guild",
                table: "GuildMember",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_IncreasableItemOption_ItemOptionDefinitionId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "ItemOptionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_IncreasableItemOption_ItemSetGroupId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "ItemSetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_IncreasableItemOption_OptionTypeId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "OptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IncreasableItemOption_PowerUpDefinitionId",
                schema: "config",
                table: "IncreasableItemOption",
                column: "PowerUpDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_DefinitionId",
                schema: "data",
                table: "Item",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_ItemStorageId",
                schema: "data",
                table: "Item",
                column: "ItemStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAppearance_AppearanceDataId",
                schema: "data",
                table: "ItemAppearance",
                column: "AppearanceDataId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAppearance_DefinitionId",
                schema: "data",
                table: "ItemAppearance",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAppearanceItemOptionType_ItemOptionTypeId",
                schema: "data",
                table: "ItemAppearanceItemOptionType",
                column: "ItemOptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemBasePowerUpDefinition_BonusPerLevelTableId",
                schema: "config",
                table: "ItemBasePowerUpDefinition",
                column: "BonusPerLevelTableId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemBasePowerUpDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemBasePowerUpDefinition",
                column: "ItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemBasePowerUpDefinition_TargetAttributeId",
                schema: "config",
                table: "ItemBasePowerUpDefinition",
                column: "TargetAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCrafting_MonsterDefinitionId",
                schema: "config",
                table: "ItemCrafting",
                column: "MonsterDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCrafting_SimpleCraftingSettingsId",
                schema: "config",
                table: "ItemCrafting",
                column: "SimpleCraftingSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCraftingRequiredItem_SimpleCraftingSettingsId",
                schema: "config",
                table: "ItemCraftingRequiredItem",
                column: "SimpleCraftingSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCraftingRequiredItemItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemCraftingRequiredItemItemDefinition",
                column: "ItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCraftingRequiredItemItemOptionType_ItemOptionTypeId",
                schema: "config",
                table: "ItemCraftingRequiredItemItemOptionType",
                column: "ItemOptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCraftingResultItem_ItemDefinitionId",
                schema: "config",
                table: "ItemCraftingResultItem",
                column: "ItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCraftingResultItem_SimpleCraftingSettingsId",
                schema: "config",
                table: "ItemCraftingResultItem",
                column: "SimpleCraftingSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinition_ConsumeEffectId",
                schema: "config",
                table: "ItemDefinition",
                column: "ConsumeEffectId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinition_GameConfigurationId",
                schema: "config",
                table: "ItemDefinition",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinition_ItemSlotId",
                schema: "config",
                table: "ItemDefinition",
                column: "ItemSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinition_SkillId",
                schema: "config",
                table: "ItemDefinition",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinitionCharacterClass_CharacterClassId",
                schema: "config",
                table: "ItemDefinitionCharacterClass",
                column: "CharacterClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinitionItemOptionDefinition_ItemOptionDefinitionId",
                schema: "config",
                table: "ItemDefinitionItemOptionDefinition",
                column: "ItemOptionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinitionItemSetGroup_ItemSetGroupId",
                schema: "config",
                table: "ItemDefinitionItemSetGroup",
                column: "ItemSetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDropItemGroup_ItemDefinitionId",
                schema: "config",
                table: "ItemDropItemGroup",
                column: "ItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDropItemGroup_MonsterId",
                schema: "config",
                table: "ItemDropItemGroup",
                column: "MonsterId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDropItemGroupItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemDropItemGroupItemDefinition",
                column: "ItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemItemOfItemSet_ItemOfItemSetId",
                schema: "data",
                table: "ItemItemOfItemSet",
                column: "ItemOfItemSetId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLevelBonusTable_GameConfigurationId",
                schema: "config",
                table: "ItemLevelBonusTable",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOfItemSet_BonusOptionId",
                schema: "config",
                table: "ItemOfItemSet",
                column: "BonusOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOfItemSet_ItemDefinitionId",
                schema: "config",
                table: "ItemOfItemSet",
                column: "ItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOfItemSet_ItemSetGroupId",
                schema: "config",
                table: "ItemOfItemSet",
                column: "ItemSetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptionCombinationBonus_BonusId",
                schema: "config",
                table: "ItemOptionCombinationBonus",
                column: "BonusId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptionCombinationBonus_GameConfigurationId",
                schema: "config",
                table: "ItemOptionCombinationBonus",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptionDefinition_GameConfigurationId",
                schema: "config",
                table: "ItemOptionDefinition",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptionLink_ItemId",
                schema: "data",
                table: "ItemOptionLink",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptionLink_ItemOptionId",
                schema: "data",
                table: "ItemOptionLink",
                column: "ItemOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptionOfLevel_IncreasableItemOptionId",
                schema: "config",
                table: "ItemOptionOfLevel",
                column: "IncreasableItemOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptionOfLevel_PowerUpDefinitionId",
                schema: "config",
                table: "ItemOptionOfLevel",
                column: "PowerUpDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptionType_GameConfigurationId",
                schema: "config",
                table: "ItemOptionType",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSetGroup_GameConfigurationId",
                schema: "config",
                table: "ItemSetGroup",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSlotType_GameConfigurationId",
                schema: "config",
                table: "ItemSlotType",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_JewelMix_GameConfigurationId",
                schema: "config",
                table: "JewelMix",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_JewelMix_MixedJewelId",
                schema: "config",
                table: "JewelMix",
                column: "MixedJewelId");

            migrationBuilder.CreateIndex(
                name: "IX_JewelMix_SingleJewelId",
                schema: "config",
                table: "JewelMix",
                column: "SingleJewelId");

            migrationBuilder.CreateIndex(
                name: "IX_LetterBody_HeaderId",
                schema: "data",
                table: "LetterBody",
                column: "HeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_LetterBody_SenderAppearanceId",
                schema: "data",
                table: "LetterBody",
                column: "SenderAppearanceId");

            migrationBuilder.CreateIndex(
                name: "IX_LetterHeader_ReceiverId",
                schema: "data",
                table: "LetterHeader",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_LevelBonus_ItemLevelBonusTableId",
                schema: "config",
                table: "LevelBonus",
                column: "ItemLevelBonusTableId");

            migrationBuilder.CreateIndex(
                name: "IX_MagicEffectDefinition_DurationId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "DurationId");

            migrationBuilder.CreateIndex(
                name: "IX_MagicEffectDefinition_GameConfigurationId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterSkillDefinition_ReplacedSkillId",
                schema: "config",
                table: "MasterSkillDefinition",
                column: "ReplacedSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterSkillDefinition_RootId",
                schema: "config",
                table: "MasterSkillDefinition",
                column: "RootId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterSkillDefinition_TargetAttributeId",
                schema: "config",
                table: "MasterSkillDefinition",
                column: "TargetAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterSkillDefinitionSkill_SkillId",
                schema: "config",
                table: "MasterSkillDefinitionSkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterSkillRoot_GameConfigurationId",
                schema: "config",
                table: "MasterSkillRoot",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameChangeEvent_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameChangeEvent",
                column: "MiniGameDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameChangeEvent_SpawnAreaId",
                schema: "config",
                table: "MiniGameChangeEvent",
                column: "SpawnAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameChangeEvent_TargetDefinitionId",
                schema: "config",
                table: "MiniGameChangeEvent",
                column: "TargetDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameDefinition_EntranceId",
                schema: "config",
                table: "MiniGameDefinition",
                column: "EntranceId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameDefinition_GameConfigurationId",
                schema: "config",
                table: "MiniGameDefinition",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameDefinition_TicketItemId",
                schema: "config",
                table: "MiniGameDefinition",
                column: "TicketItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameRankingEntry_CharacterId",
                schema: "data",
                table: "MiniGameRankingEntry",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameRankingEntry_MiniGameId",
                schema: "data",
                table: "MiniGameRankingEntry",
                column: "MiniGameId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameReward_ItemRewardId",
                schema: "config",
                table: "MiniGameReward",
                column: "ItemRewardId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameReward_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameReward",
                column: "MiniGameDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameReward_RequiredKillId",
                schema: "config",
                table: "MiniGameReward",
                column: "RequiredKillId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameSpawnWave_MiniGameDefinitionId",
                schema: "config",
                table: "MiniGameSpawnWave",
                column: "MiniGameDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniGameTerrainChange_MiniGameChangeEventId",
                schema: "config",
                table: "MiniGameTerrainChange",
                column: "MiniGameChangeEventId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterAttribute_AttributeDefinitionId",
                schema: "config",
                table: "MonsterAttribute",
                column: "AttributeDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterAttribute_MonsterDefinitionId",
                schema: "config",
                table: "MonsterAttribute",
                column: "MonsterDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterDefinition_AttackSkillId",
                schema: "config",
                table: "MonsterDefinition",
                column: "AttackSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterDefinition_GameConfigurationId",
                schema: "config",
                table: "MonsterDefinition",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterDefinition_MerchantStoreId",
                schema: "config",
                table: "MonsterDefinition",
                column: "MerchantStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterDefinitionDropItemGroup_DropItemGroupId",
                schema: "config",
                table: "MonsterDefinitionDropItemGroup",
                column: "DropItemGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterSpawnArea_GameMapId",
                schema: "config",
                table: "MonsterSpawnArea",
                column: "GameMapId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterSpawnArea_MonsterDefinitionId",
                schema: "config",
                table: "MonsterSpawnArea",
                column: "MonsterDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlugInConfiguration_GameConfigurationId",
                schema: "config",
                table: "PlugInConfiguration",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerUpDefinition_BoostId",
                schema: "config",
                table: "PowerUpDefinition",
                column: "BoostId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerUpDefinition_MagicEffectDefinitionId",
                schema: "config",
                table: "PowerUpDefinition",
                column: "MagicEffectDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerUpDefinition_TargetAttributeId",
                schema: "config",
                table: "PowerUpDefinition",
                column: "TargetAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestDefinition_MonsterDefinitionId",
                schema: "config",
                table: "QuestDefinition",
                column: "MonsterDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestDefinition_QualifiedCharacterId",
                schema: "config",
                table: "QuestDefinition",
                column: "QualifiedCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestDefinition_QuestGiverId",
                schema: "config",
                table: "QuestDefinition",
                column: "QuestGiverId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestItemRequirement_DropItemGroupId",
                schema: "config",
                table: "QuestItemRequirement",
                column: "DropItemGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestItemRequirement_ItemId",
                schema: "config",
                table: "QuestItemRequirement",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestItemRequirement_QuestDefinitionId",
                schema: "config",
                table: "QuestItemRequirement",
                column: "QuestDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestMonsterKillRequirement_MonsterId",
                schema: "config",
                table: "QuestMonsterKillRequirement",
                column: "MonsterId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestMonsterKillRequirement_QuestDefinitionId",
                schema: "config",
                table: "QuestMonsterKillRequirement",
                column: "QuestDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestMonsterKillRequirementState_CharacterQuestStateId",
                schema: "data",
                table: "QuestMonsterKillRequirementState",
                column: "CharacterQuestStateId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestMonsterKillRequirementState_RequirementId",
                schema: "data",
                table: "QuestMonsterKillRequirementState",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestReward_AttributeRewardId",
                schema: "config",
                table: "QuestReward",
                column: "AttributeRewardId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestReward_ItemRewardId",
                schema: "config",
                table: "QuestReward",
                column: "ItemRewardId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestReward_QuestDefinitionId",
                schema: "config",
                table: "QuestReward",
                column: "QuestDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestReward_SkillRewardId",
                schema: "config",
                table: "QuestReward",
                column: "SkillRewardId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_ElementalModifierTargetId",
                schema: "config",
                table: "Skill",
                column: "ElementalModifierTargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_GameConfigurationId",
                schema: "config",
                table: "Skill",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_MagicEffectDefId",
                schema: "config",
                table: "Skill",
                column: "MagicEffectDefId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_MasterDefinitionId",
                schema: "config",
                table: "Skill",
                column: "MasterDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillCharacterClass_CharacterClassId",
                schema: "config",
                table: "SkillCharacterClass",
                column: "CharacterClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillEntry_CharacterId",
                schema: "data",
                table: "SkillEntry",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillEntry_SkillId",
                schema: "data",
                table: "SkillEntry",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_StatAttribute_CharacterId",
                schema: "data",
                table: "StatAttribute",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_StatAttribute_DefinitionId",
                schema: "data",
                table: "StatAttribute",
                column: "DefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_StatAttributeDefinition_AttributeId",
                schema: "config",
                table: "StatAttributeDefinition",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_StatAttributeDefinition_CharacterClassId",
                schema: "config",
                table: "StatAttributeDefinition",
                column: "CharacterClassId");

            migrationBuilder.CreateIndex(
                name: "IX_WarpInfo_GameConfigurationId",
                schema: "config",
                table: "WarpInfo",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_WarpInfo_GateId",
                schema: "config",
                table: "WarpInfo",
                column: "GateId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterDropItemGroup_DropItemGroup_DropItemGroupId",
                schema: "data",
                table: "CharacterDropItemGroup",
                column: "DropItemGroupId",
                principalSchema: "config",
                principalTable: "DropItemGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterQuestState_QuestDefinition_ActiveQuestId",
                schema: "data",
                table: "CharacterQuestState",
                column: "ActiveQuestId",
                principalSchema: "config",
                principalTable: "QuestDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterQuestState_QuestDefinition_LastFinishedQuestId",
                schema: "data",
                table: "CharacterQuestState",
                column: "LastFinishedQuestId",
                principalSchema: "config",
                principalTable: "QuestDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DropItemGroup_MonsterDefinition_MonsterId",
                schema: "config",
                table: "DropItemGroup",
                column: "MonsterId",
                principalSchema: "config",
                principalTable: "MonsterDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DropItemGroupItemDefinition_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "DropItemGroupItemDefinition",
                column: "ItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_ItemDefinition_DefinitionId",
                schema: "data",
                table: "Item",
                column: "DefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemAppearance_ItemDefinition_DefinitionId",
                schema: "data",
                table: "ItemAppearance",
                column: "DefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBasePowerUpDefinition_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemBasePowerUpDefinition",
                column: "ItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCrafting_MonsterDefinition_MonsterDefinitionId",
                schema: "config",
                table: "ItemCrafting",
                column: "MonsterDefinitionId",
                principalSchema: "config",
                principalTable: "MonsterDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCraftingRequiredItemItemDefinition_ItemDefinition_ItemD~",
                schema: "config",
                table: "ItemCraftingRequiredItemItemDefinition",
                column: "ItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCraftingResultItem_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemCraftingResultItem",
                column: "ItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDefinition_Skill_SkillId",
                schema: "config",
                table: "ItemDefinition",
                column: "SkillId",
                principalSchema: "config",
                principalTable: "Skill",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_Skill_SkillId",
                schema: "config",
                table: "AttributeRequirement",
                column: "SkillId",
                principalSchema: "config",
                principalTable: "Skill",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_Skill_SkillId1",
                schema: "config",
                table: "AttributeRequirement",
                column: "SkillId1",
                principalSchema: "config",
                principalTable: "Skill",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDropItemGroup_MonsterDefinition_MonsterId",
                schema: "config",
                table: "ItemDropItemGroup",
                column: "MonsterId",
                principalSchema: "config",
                principalTable: "MonsterDefinition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MasterSkillDefinition_Skill_ReplacedSkillId",
                schema: "config",
                table: "MasterSkillDefinition",
                column: "ReplacedSkillId",
                principalSchema: "config",
                principalTable: "Skill",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MagicEffectDefinition_PowerUpDefinitionValue_DurationId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MasterSkillDefinition_Skill_ReplacedSkillId",
                schema: "config",
                table: "MasterSkillDefinition");

            migrationBuilder.DropTable(
                name: "AccountCharacterClass",
                schema: "data");

            migrationBuilder.DropTable(
                name: "AttributeRelationship",
                schema: "config");

            migrationBuilder.DropTable(
                name: "AttributeRequirement",
                schema: "config");

            migrationBuilder.DropTable(
                name: "CharacterDropItemGroup",
                schema: "data");

            migrationBuilder.DropTable(
                name: "ChatServerEndpoint",
                schema: "config");

            migrationBuilder.DropTable(
                name: "CombinationBonusRequirement",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ConnectServerDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ConstValueAttribute",
                schema: "config");

            migrationBuilder.DropTable(
                name: "DropItemGroupItemDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "EnterGate",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Friend",
                schema: "friend");

            migrationBuilder.DropTable(
                name: "GameMapDefinitionDropItemGroup",
                schema: "config");

            migrationBuilder.DropTable(
                name: "GameServerConfigurationGameMapDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "GameServerEndpoint",
                schema: "config");

            migrationBuilder.DropTable(
                name: "GuildMember",
                schema: "guild");

            migrationBuilder.DropTable(
                name: "ItemAppearanceItemOptionType",
                schema: "data");

            migrationBuilder.DropTable(
                name: "ItemBasePowerUpDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemCrafting",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemCraftingRequiredItemItemDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemCraftingRequiredItemItemOptionType",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemCraftingResultItem",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemDefinitionCharacterClass",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemDefinitionItemOptionDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemDefinitionItemSetGroup",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemDropItemGroupItemDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemItemOfItemSet",
                schema: "data");

            migrationBuilder.DropTable(
                name: "ItemOptionLink",
                schema: "data");

            migrationBuilder.DropTable(
                name: "ItemOptionOfLevel",
                schema: "config");

            migrationBuilder.DropTable(
                name: "JewelMix",
                schema: "config");

            migrationBuilder.DropTable(
                name: "LetterBody",
                schema: "data");

            migrationBuilder.DropTable(
                name: "LevelBonus",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MasterSkillDefinitionSkill",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MiniGameRankingEntry",
                schema: "data");

            migrationBuilder.DropTable(
                name: "MiniGameReward",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MiniGameSpawnWave",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MiniGameTerrainChange",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MonsterAttribute",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MonsterDefinitionDropItemGroup",
                schema: "config");

            migrationBuilder.DropTable(
                name: "PlugInConfiguration",
                schema: "config");

            migrationBuilder.DropTable(
                name: "QuestItemRequirement",
                schema: "config");

            migrationBuilder.DropTable(
                name: "QuestMonsterKillRequirementState",
                schema: "data");

            migrationBuilder.DropTable(
                name: "QuestReward",
                schema: "config");

            migrationBuilder.DropTable(
                name: "SkillCharacterClass",
                schema: "config");

            migrationBuilder.DropTable(
                name: "SkillEntry",
                schema: "data");

            migrationBuilder.DropTable(
                name: "StatAttribute",
                schema: "data");

            migrationBuilder.DropTable(
                name: "StatAttributeDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "WarpInfo",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ChatServerDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemOptionCombinationBonus",
                schema: "config");

            migrationBuilder.DropTable(
                name: "GameClientDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "GameServerDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Guild",
                schema: "guild");

            migrationBuilder.DropTable(
                name: "ItemAppearance",
                schema: "data");

            migrationBuilder.DropTable(
                name: "ItemCraftingRequiredItem",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemDropItemGroup",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemOfItemSet",
                schema: "config");

            migrationBuilder.DropTable(
                name: "LetterHeader",
                schema: "data");

            migrationBuilder.DropTable(
                name: "ItemLevelBonusTable",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MiniGameChangeEvent",
                schema: "config");

            migrationBuilder.DropTable(
                name: "DropItemGroup",
                schema: "config");

            migrationBuilder.DropTable(
                name: "CharacterQuestState",
                schema: "data");

            migrationBuilder.DropTable(
                name: "QuestMonsterKillRequirement",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Item",
                schema: "data");

            migrationBuilder.DropTable(
                name: "GameServerConfiguration",
                schema: "config");

            migrationBuilder.DropTable(
                name: "AppearanceData",
                schema: "data");

            migrationBuilder.DropTable(
                name: "SimpleCraftingSettings",
                schema: "config");

            migrationBuilder.DropTable(
                name: "IncreasableItemOption",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MiniGameDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MonsterSpawnArea",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Character",
                schema: "data");

            migrationBuilder.DropTable(
                name: "QuestDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemOptionDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemOptionType",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemSetGroup",
                schema: "config");

            migrationBuilder.DropTable(
                name: "PowerUpDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ExitGate",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Account",
                schema: "data");

            migrationBuilder.DropTable(
                name: "CharacterClass",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MonsterDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemSlotType",
                schema: "config");

            migrationBuilder.DropTable(
                name: "GameMapDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemStorage",
                schema: "data");

            migrationBuilder.DropTable(
                name: "BattleZoneDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Rectangle",
                schema: "config");

            migrationBuilder.DropTable(
                name: "PowerUpDefinitionValue",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Skill",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MagicEffectDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MasterSkillDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "AttributeDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MasterSkillRoot",
                schema: "config");

            migrationBuilder.DropTable(
                name: "GameConfiguration",
                schema: "config");
        }
    }
}
