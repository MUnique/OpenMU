// <copyright file="00000000000000_Initial.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <summary>
    /// The initial schema migration.
    /// </summary>
    public partial class Initial : Migration
    {
        /// <inheritdoc/>
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
                    ServerId = table.Column<byte>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    MaximumConnections = table.Column<int>(nullable: false),
                    ClientTimeout = table.Column<TimeSpan>(nullable: false),
                    ClientCleanUpInterval = table.Column<TimeSpan>(nullable: false),
                    RoomCleanUpInterval = table.Column<TimeSpan>(nullable: false),
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatServerDefinition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameClientDefinition",
                schema: "config",
                columns: table => new
                {
                    Episode = table.Column<byte>(nullable: false),
                    Language = table.Column<int>(nullable: false),
                    Season = table.Column<byte>(nullable: false),
                    Version = table.Column<byte[]>(nullable: true),
                    Serial = table.Column<byte[]>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false)
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
                    AreaSkillHitsPlayer = table.Column<bool>(nullable: false),
                    InfoRange = table.Column<byte>(nullable: false),
                    LetterSendPrice = table.Column<int>(nullable: false),
                    MaximumCharactersPerAccount = table.Column<byte>(nullable: false),
                    MaximumInventoryMoney = table.Column<int>(nullable: false),
                    MaximumLetters = table.Column<int>(nullable: false),
                    MaximumLevel = table.Column<short>(nullable: false),
                    MaximumPartySize = table.Column<byte>(nullable: false),
                    MaximumPasswordLength = table.Column<int>(nullable: false),
                    MaximumVaultMoney = table.Column<int>(nullable: false),
                    RecoveryInterval = table.Column<int>(nullable: false),
                    CharacterNameRegex = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false)
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
                    MaximumPlayers = table.Column<short>(nullable: false),
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameServerConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerUpDefinitionValue",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Value = table.Column<float>(nullable: false),
                    AggregateType = table.Column<int>(nullable: false),
                    ParentAsBoostId = table.Column<Guid>(nullable: true),
                    ParentAsDurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerUpDefinitionValue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SimpleCraftingSettings",
                schema: "config",
                columns: table => new
                {
                    Money = table.Column<int>(nullable: false),
                    SuccessPercent = table.Column<byte>(nullable: false),
                    MultipleAllowed = table.Column<bool>(nullable: false),
                    ResultItemSelect = table.Column<int>(nullable: false),
                    LuckOptionChance = table.Column<byte>(nullable: false),
                    SkillOptionChance = table.Column<byte>(nullable: false),
                    ExcOptionChance = table.Column<byte>(nullable: false),
                    MaxExcOptions = table.Column<byte>(nullable: false),
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleCraftingSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemStorage",
                schema: "data",
                columns: table => new
                {
                    Money = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemStorage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Friend",
                schema: "friend",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CharacterId = table.Column<Guid>(nullable: false),
                    FriendId = table.Column<Guid>(nullable: false),
                    Accepted = table.Column<bool>(nullable: false),
                    RequestOpen = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friend", x => x.Id);
                    table.UniqueConstraint("AK_Friend_CharacterId_FriendId", x => new { x.CharacterId, x.FriendId });
                });

            migrationBuilder.CreateTable(
                name: "Guild",
                schema: "guild",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 8, nullable: false),
                    Logo = table.Column<byte[]>(nullable: true),
                    Score = table.Column<int>(nullable: false),
                    Notice = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    HostilityId = table.Column<Guid>(nullable: true),
                    AllianceGuildId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guild", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guild_Guild_AllianceGuildId",
                        column: x => x.AllianceGuildId,
                        principalSchema: "guild",
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guild_Guild_HostilityId",
                        column: x => x.HostilityId,
                        principalSchema: "guild",
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatServerEndpoint",
                schema: "config",
                columns: table => new
                {
                    NetworkPort = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: true),
                    ChatServerDefinitionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatServerEndpoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatServerEndpoint_ChatServerDefinition_ChatServerDefinitio~",
                        column: x => x.ChatServerDefinitionId,
                        principalSchema: "config",
                        principalTable: "ChatServerDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatServerEndpoint_GameClientDefinition_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "config",
                        principalTable: "GameClientDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConnectServerDefinition",
                schema: "config",
                columns: table => new
                {
                    ServerId = table.Column<byte>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DisconnectOnUnknownPacket = table.Column<bool>(nullable: false),
                    MaximumReceiveSize = table.Column<byte>(nullable: false),
                    ClientListenerPort = table.Column<int>(nullable: false),
                    Timeout = table.Column<TimeSpan>(nullable: false),
                    CurrentPatchVersion = table.Column<byte[]>(nullable: true),
                    PatchAddress = table.Column<string>(nullable: true),
                    MaxConnectionsPerAddress = table.Column<int>(nullable: false),
                    CheckMaxConnectionsPerAddress = table.Column<bool>(nullable: false),
                    MaxConnections = table.Column<int>(nullable: false),
                    ListenerBacklog = table.Column<int>(nullable: false),
                    MaxFtpRequests = table.Column<int>(nullable: false),
                    MaxIpRequests = table.Column<int>(nullable: false),
                    MaxServerListRequests = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectServerDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConnectServerDefinition_GameClientDefinition_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "config",
                        principalTable: "GameClientDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttributeDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Designation = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DropItemGroup",
                schema: "config",
                columns: table => new
                {
                    Chance = table.Column<double>(nullable: false),
                    ItemType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DropItemGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DropItemGroup_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameMapDefinition",
                schema: "config",
                columns: table => new
                {
                    ExpMultiplier = table.Column<double>(nullable: false),
                    Number = table.Column<short>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TerrainData = table.Column<byte[]>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    SafezoneMapId = table.Column<Guid>(nullable: true),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameMapDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameMapDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameMapDefinition_GameMapDefinition_SafezoneMapId",
                        column: x => x.SafezoneMapId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemOptionDefinition",
                schema: "config",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: true),
                    AddsRandomly = table.Column<bool>(nullable: false),
                    AddChance = table.Column<float>(nullable: false),
                    MaximumOptionsPerItem = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOptionDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOptionDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemOptionType",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsVisible = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOptionType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOptionType_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemSetGroup",
                schema: "config",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: true),
                    AlwaysApplies = table.Column<bool>(nullable: false),
                    CountDistinct = table.Column<bool>(nullable: false),
                    MinimumItemCount = table.Column<int>(nullable: false),
                    SetLevel = table.Column<int>(nullable: false),
                    AncientSetDiscriminator = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSetGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemSetGroup_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemSlotType",
                schema: "config",
                columns: table => new
                {
                    Description = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    ItemSlots = table.Column<string>(nullable: true),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSlotType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemSlotType_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MasterSkillRoot",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterSkillRoot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterSkillRoot_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlugInConfiguration",
                schema: "config",
                columns: table => new
                {
                    TypeId = table.Column<Guid>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CustomPlugInSource = table.Column<string>(nullable: true),
                    ExternalAssemblyName = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlugInConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlugInConfiguration_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameServerDefinition",
                schema: "config",
                columns: table => new
                {
                    ServerID = table.Column<byte>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    ServerConfigurationId = table.Column<Guid>(nullable: true),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameServerDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameServerDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameServerDefinition_GameServerConfiguration_ServerConfigur~",
                        column: x => x.ServerConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameServerConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                schema: "data",
                columns: table => new
                {
                    TimeZone = table.Column<short>(nullable: false),
                    VaultPassword = table.Column<string>(nullable: true),
                    IsVaultExtended = table.Column<bool>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    State = table.Column<int>(nullable: false),
                    LoginName = table.Column<string>(maxLength: 10, nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityCode = table.Column<string>(nullable: true),
                    EMail = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    VaultId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_ItemStorage_VaultId",
                        column: x => x.VaultId,
                        principalSchema: "data",
                        principalTable: "ItemStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PowerUpDefinition",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TargetAttributeId = table.Column<Guid>(nullable: true),
                    BoostId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerUpDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerUpDefinition_PowerUpDefinitionValue_BoostId",
                        column: x => x.BoostId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinitionValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerUpDefinition_AttributeDefinition_TargetAttributeId",
                        column: x => x.TargetAttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PowerUpDefinitionWithDuration",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DurationId = table.Column<Guid>(nullable: true),
                    TargetAttributeId = table.Column<Guid>(nullable: true),
                    BoostId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerUpDefinitionWithDuration", x => x.Id);
                    table.ForeignKey(
                        name: "PowerUpDefinitionWithDuration_Boost",
                        column: x => x.BoostId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinitionValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "PowerUpDefinitionWithDuration_Duration",
                        column: x => x.DurationId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinitionValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PowerUpDefinitionWithDuration_AttributeDefinition_TargetAtt~",
                        column: x => x.TargetAttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterClass",
                schema: "config",
                columns: table => new
                {
                    CanGetCreated = table.Column<bool>(nullable: false),
                    CreationAllowedFlag = table.Column<byte>(nullable: false),
                    FruitCalculation = table.Column<int>(nullable: false),
                    IsMasterClass = table.Column<bool>(nullable: false),
                    LevelRequirementByCreation = table.Column<short>(nullable: false),
                    LevelWarpRequirementReductionPercent = table.Column<int>(nullable: false),
                    Number = table.Column<byte>(nullable: false),
                    PointsPerLevelUp = table.Column<short>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    NextGenerationClassId = table.Column<Guid>(nullable: true),
                    HomeMapId = table.Column<Guid>(nullable: true),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterClass_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterClass_GameMapDefinition_HomeMapId",
                        column: x => x.HomeMapId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterClass_CharacterClass_NextGenerationClassId",
                        column: x => x.NextGenerationClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExitGate",
                schema: "config",
                columns: table => new
                {
                    X1 = table.Column<byte>(nullable: false),
                    X2 = table.Column<byte>(nullable: false),
                    Y1 = table.Column<byte>(nullable: false),
                    Y2 = table.Column<byte>(nullable: false),
                    Direction = table.Column<int>(nullable: false),
                    IsSpawnGate = table.Column<bool>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    MapId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExitGate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExitGate_GameMapDefinition_MapId",
                        column: x => x.MapId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameMapDefinitionDropItemGroup",
                schema: "config",
                columns: table => new
                {
                    GameMapDefinitionId = table.Column<Guid>(nullable: false),
                    DropItemGroupId = table.Column<Guid>(nullable: false)
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
                name: "GameServerConfigurationGameMapDefinition",
                schema: "config",
                columns: table => new
                {
                    GameServerConfigurationId = table.Column<Guid>(nullable: false),
                    GameMapDefinitionId = table.Column<Guid>(nullable: false)
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
                name: "GameServerEndpoint",
                schema: "config",
                columns: table => new
                {
                    NetworkPort = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: true),
                    GameServerDefinitionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameServerEndpoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameServerEndpoint_GameClientDefinition_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "config",
                        principalTable: "GameClientDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameServerEndpoint_GameServerDefinition_GameServerDefinitio~",
                        column: x => x.GameServerDefinitionId,
                        principalSchema: "config",
                        principalTable: "GameServerDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncreasableItemOption",
                schema: "config",
                columns: table => new
                {
                    Number = table.Column<int>(nullable: false),
                    LevelType = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    OptionTypeId = table.Column<Guid>(nullable: true),
                    PowerUpDefinitionId = table.Column<Guid>(nullable: true),
                    ItemOptionDefinitionId = table.Column<Guid>(nullable: true),
                    ItemSetGroupId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncreasableItemOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncreasableItemOption_ItemOptionDefinition_ItemOptionDefini~",
                        column: x => x.ItemOptionDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemOptionDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncreasableItemOption_ItemSetGroup_ItemSetGroupId",
                        column: x => x.ItemSetGroupId,
                        principalSchema: "config",
                        principalTable: "ItemSetGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncreasableItemOption_ItemOptionType_OptionTypeId",
                        column: x => x.OptionTypeId,
                        principalSchema: "config",
                        principalTable: "ItemOptionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncreasableItemOption_PowerUpDefinition_PowerUpDefinitionId",
                        column: x => x.PowerUpDefinitionId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MagicEffectDefinition",
                schema: "config",
                columns: table => new
                {
                    InformObservers = table.Column<bool>(nullable: false),
                    Number = table.Column<short>(nullable: false),
                    SendDuration = table.Column<bool>(nullable: false),
                    StopByDeath = table.Column<bool>(nullable: false),
                    SubType = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    PowerUpDefinitionId = table.Column<Guid>(nullable: true),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicEffectDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MagicEffectDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MagicEffectDefinition_PowerUpDefinitionWithDuration_PowerUp~",
                        column: x => x.PowerUpDefinitionId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinitionWithDuration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttributeRelationship",
                schema: "config",
                columns: table => new
                {
                    InputOperand = table.Column<float>(nullable: false),
                    InputOperator = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    TargetAttributeId = table.Column<Guid>(nullable: true),
                    InputAttributeId = table.Column<Guid>(nullable: true),
                    CharacterClassId = table.Column<Guid>(nullable: true),
                    PowerUpDefinitionValueId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeRelationship", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeRelationship_CharacterClass_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttributeRelationship_AttributeDefinition_InputAttributeId",
                        column: x => x.InputAttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttributeRelationship_PowerUpDefinitionValue_PowerUpDefinit~",
                        column: x => x.PowerUpDefinitionValueId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinitionValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttributeRelationship_AttributeDefinition_TargetAttributeId",
                        column: x => x.TargetAttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConstValueAttribute",
                schema: "config",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DefinitionId = table.Column<Guid>(nullable: true),
                    CharacterClassId = table.Column<Guid>(nullable: false),
                    Value = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstValueAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConstValueAttribute_CharacterClass_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConstValueAttribute_AttributeDefinition_DefinitionId",
                        column: x => x.DefinitionId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatAttributeDefinition",
                schema: "config",
                columns: table => new
                {
                    BaseValue = table.Column<float>(nullable: false),
                    IncreasableByPlayer = table.Column<bool>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    AttributeId = table.Column<Guid>(nullable: true),
                    CharacterClassId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatAttributeDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatAttributeDefinition_AttributeDefinition_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatAttributeDefinition_CharacterClass_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountCharacterClass",
                schema: "data",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(nullable: false),
                    CharacterClassId = table.Column<Guid>(nullable: false)
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
                    FullAncientSetEquipped = table.Column<bool>(nullable: false),
                    Pose = table.Column<byte>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    CharacterClassId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppearanceData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppearanceData_CharacterClass_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalSchema: "config",
                        principalTable: "CharacterClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Character",
                schema: "data",
                columns: table => new
                {
                    CharacterSlot = table.Column<byte>(nullable: false),
                    CharacterStatus = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Experience = table.Column<long>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    InventoryExtensions = table.Column<int>(nullable: false),
                    LevelUpPoints = table.Column<int>(nullable: false),
                    MasterExperience = table.Column<long>(nullable: false),
                    MasterLevelUpPoints = table.Column<int>(nullable: false),
                    PlayerKillCount = table.Column<int>(nullable: false),
                    Pose = table.Column<byte>(nullable: false),
                    PositionX = table.Column<byte>(nullable: false),
                    PositionY = table.Column<byte>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    StateRemainingSeconds = table.Column<int>(nullable: false),
                    UsedFruitPoints = table.Column<int>(nullable: false),
                    UsedNegFruitPoints = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 10, nullable: false),
                    QuestInfo = table.Column<byte[]>(nullable: true),
                    KeyConfiguration = table.Column<byte[]>(nullable: true),
                    CharacterClassId = table.Column<Guid>(nullable: false),
                    CurrentMapId = table.Column<Guid>(nullable: true),
                    InventoryId = table.Column<Guid>(nullable: true),
                    AccountId = table.Column<Guid>(nullable: true)
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Character_ItemStorage_InventoryId",
                        column: x => x.InventoryId,
                        principalSchema: "data",
                        principalTable: "ItemStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EnterGate",
                schema: "config",
                columns: table => new
                {
                    X1 = table.Column<byte>(nullable: false),
                    X2 = table.Column<byte>(nullable: false),
                    Y1 = table.Column<byte>(nullable: false),
                    Y2 = table.Column<byte>(nullable: false),
                    LevelRequirement = table.Column<short>(nullable: false),
                    Number = table.Column<short>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    TargetGateId = table.Column<Guid>(nullable: true),
                    GameMapDefinitionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnterGate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnterGate_GameMapDefinition_GameMapDefinitionId",
                        column: x => x.GameMapDefinitionId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EnterGate_ExitGate_TargetGateId",
                        column: x => x.TargetGateId,
                        principalSchema: "config",
                        principalTable: "ExitGate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WarpInfo",
                schema: "config",
                columns: table => new
                {
                    Index = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Costs = table.Column<int>(nullable: false),
                    LevelRequirement = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    GateId = table.Column<Guid>(nullable: true),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarpInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarpInfo_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarpInfo_ExitGate_GateId",
                        column: x => x.GateId,
                        principalSchema: "config",
                        principalTable: "ExitGate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemOptionOfLevel",
                schema: "config",
                columns: table => new
                {
                    Level = table.Column<int>(nullable: false),
                    RequiredItemLevel = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    PowerUpDefinitionId = table.Column<Guid>(nullable: true),
                    IncreasableItemOptionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOptionOfLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOptionOfLevel_IncreasableItemOption_IncreasableItemOpti~",
                        column: x => x.IncreasableItemOptionId,
                        principalSchema: "config",
                        principalTable: "IncreasableItemOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemOptionOfLevel_PowerUpDefinition_PowerUpDefinitionId",
                        column: x => x.PowerUpDefinitionId,
                        principalSchema: "config",
                        principalTable: "PowerUpDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterDropItemGroup",
                schema: "data",
                columns: table => new
                {
                    CharacterId = table.Column<Guid>(nullable: false),
                    DropItemGroupId = table.Column<Guid>(nullable: false)
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
                    table.ForeignKey(
                        name: "FK_CharacterDropItemGroup_DropItemGroup_DropItemGroupId",
                        column: x => x.DropItemGroupId,
                        principalSchema: "config",
                        principalTable: "DropItemGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LetterHeader",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LetterDate = table.Column<DateTime>(nullable: false),
                    ReadFlag = table.Column<bool>(nullable: false),
                    SenderName = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    ReceiverId = table.Column<Guid>(nullable: false)
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
                    Value = table.Column<float>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    DefinitionId = table.Column<Guid>(nullable: true),
                    CharacterId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatAttribute_Character_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "data",
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatAttribute_AttributeDefinition_DefinitionId",
                        column: x => x.DefinitionId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuildMember",
                schema: "guild",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GuildId = table.Column<Guid>(nullable: false),
                    Status = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuildMember_Guild_GuildId",
                        column: x => x.GuildId,
                        principalSchema: "guild",
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildMember_Character_Id",
                        column: x => x.Id,
                        principalSchema: "data",
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LetterBody",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Rotation = table.Column<byte>(nullable: false),
                    Animation = table.Column<byte>(nullable: false),
                    HeaderId = table.Column<Guid>(nullable: true),
                    SenderAppearanceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterBody", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LetterBody_LetterHeader_HeaderId",
                        column: x => x.HeaderId,
                        principalSchema: "data",
                        principalTable: "LetterHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LetterBody_AppearanceData_SenderAppearanceId",
                        column: x => x.SenderAppearanceId,
                        principalSchema: "data",
                        principalTable: "AppearanceData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttributeRequirement",
                schema: "config",
                columns: table => new
                {
                    MinimumValue = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    AttributeId = table.Column<Guid>(nullable: true),
                    GameMapDefinitionId = table.Column<Guid>(nullable: true),
                    ItemDefinitionId = table.Column<Guid>(nullable: true),
                    SkillId = table.Column<Guid>(nullable: true),
                    SkillId1 = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeRequirement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeRequirement_AttributeDefinition_AttributeId",
                        column: x => x.AttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttributeRequirement_GameMapDefinition_GameMapDefinitionId",
                        column: x => x.GameMapDefinitionId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemBasePowerUpDefinition",
                schema: "config",
                columns: table => new
                {
                    BaseValue = table.Column<float>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    TargetAttributeId = table.Column<Guid>(nullable: true),
                    ItemDefinitionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemBasePowerUpDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemBasePowerUpDefinition_AttributeDefinition_TargetAttribu~",
                        column: x => x.TargetAttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LevelBonus",
                schema: "config",
                columns: table => new
                {
                    Level = table.Column<int>(nullable: false),
                    AdditionalValue = table.Column<float>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    ItemBasePowerUpDefinitionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelBonus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LevelBonus_ItemBasePowerUpDefinition_ItemBasePowerUpDefinit~",
                        column: x => x.ItemBasePowerUpDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemBasePowerUpDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MasterSkillDefinition",
                schema: "config",
                columns: table => new
                {
                    Aggregation = table.Column<int>(nullable: false),
                    MaximumLevel = table.Column<byte>(nullable: false),
                    MinimumLevel = table.Column<byte>(nullable: false),
                    Rank = table.Column<byte>(nullable: false),
                    ValueFormula = table.Column<string>(nullable: true),
                    DisplayValueFormula = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    RootId = table.Column<Guid>(nullable: true),
                    TargetAttributeId = table.Column<Guid>(nullable: true),
                    ReplacedSkillId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterSkillDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterSkillDefinition_MasterSkillRoot_RootId",
                        column: x => x.RootId,
                        principalSchema: "config",
                        principalTable: "MasterSkillRoot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MasterSkillDefinition_AttributeDefinition_TargetAttributeId",
                        column: x => x.TargetAttributeId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                schema: "config",
                columns: table => new
                {
                    AttackDamage = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    ImplicitTargetRange = table.Column<short>(nullable: false),
                    MovesTarget = table.Column<bool>(nullable: false),
                    MovesToTarget = table.Column<bool>(nullable: false),
                    Number = table.Column<short>(nullable: false),
                    Range = table.Column<short>(nullable: false),
                    SkillType = table.Column<int>(nullable: false),
                    Target = table.Column<int>(nullable: false),
                    TargetRestriction = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    ElementalModifierTargetId = table.Column<Guid>(nullable: true),
                    MagicEffectDefId = table.Column<Guid>(nullable: true),
                    MasterDefinitionId = table.Column<Guid>(nullable: true),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skill_AttributeDefinition_ElementalModifierTargetId",
                        column: x => x.ElementalModifierTargetId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_MagicEffectDefinition_MagicEffectDefId",
                        column: x => x.MagicEffectDefId,
                        principalSchema: "config",
                        principalTable: "MagicEffectDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skill_MasterSkillDefinition_MasterDefinitionId",
                        column: x => x.MasterDefinitionId,
                        principalSchema: "config",
                        principalTable: "MasterSkillDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemDefinition",
                schema: "config",
                columns: table => new
                {
                    DropLevel = table.Column<byte>(nullable: false),
                    DropsFromMonsters = table.Column<bool>(nullable: false),
                    Durability = table.Column<byte>(nullable: false),
                    Group = table.Column<byte>(nullable: false),
                    Height = table.Column<byte>(nullable: false),
                    IsAmmunition = table.Column<bool>(nullable: false),
                    MaximumItemLevel = table.Column<byte>(nullable: false),
                    MaximumSockets = table.Column<int>(nullable: false),
                    Number = table.Column<short>(nullable: false),
                    Value = table.Column<int>(nullable: false),
                    Width = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ConsumeHandlerClass = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    ItemSlotId = table.Column<Guid>(nullable: true),
                    SkillId = table.Column<Guid>(nullable: true),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemDefinition_ItemSlotType_ItemSlotId",
                        column: x => x.ItemSlotId,
                        principalSchema: "config",
                        principalTable: "ItemSlotType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemDefinition_Skill_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "config",
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MasterSkillDefinitionSkill",
                schema: "config",
                columns: table => new
                {
                    MasterSkillDefinitionId = table.Column<Guid>(nullable: false),
                    SkillId = table.Column<Guid>(nullable: false)
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
                    AttackDelay = table.Column<TimeSpan>(nullable: false),
                    AttackRange = table.Column<byte>(nullable: false),
                    Attribute = table.Column<byte>(nullable: false),
                    MoveDelay = table.Column<TimeSpan>(nullable: false),
                    MoveRange = table.Column<byte>(nullable: false),
                    NpcWindow = table.Column<int>(nullable: false),
                    Number = table.Column<short>(nullable: false),
                    NumberOfMaximumItemDrops = table.Column<int>(nullable: false),
                    RespawnDelay = table.Column<TimeSpan>(nullable: false),
                    Skill = table.Column<short>(nullable: false),
                    ViewRange = table.Column<short>(nullable: false),
                    Designation = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    AttackSkillId = table.Column<Guid>(nullable: true),
                    MerchantStoreId = table.Column<Guid>(nullable: true),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterDefinition_Skill_AttackSkillId",
                        column: x => x.AttackSkillId,
                        principalSchema: "config",
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonsterDefinition_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonsterDefinition_ItemStorage_MerchantStoreId",
                        column: x => x.MerchantStoreId,
                        principalSchema: "data",
                        principalTable: "ItemStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillCharacterClass",
                schema: "config",
                columns: table => new
                {
                    SkillId = table.Column<Guid>(nullable: false),
                    CharacterClassId = table.Column<Guid>(nullable: false)
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
                    Level = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    SkillId = table.Column<Guid>(nullable: true),
                    CharacterId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillEntry_Character_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "data",
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillEntry_Skill_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "config",
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DropItemGroupItemDefinition",
                schema: "config",
                columns: table => new
                {
                    DropItemGroupId = table.Column<Guid>(nullable: false),
                    ItemDefinitionId = table.Column<Guid>(nullable: false)
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
                    table.ForeignKey(
                        name: "FK_DropItemGroupItemDefinition_ItemDefinition_ItemDefinitionId",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemCraftingRequiredItem",
                schema: "config",
                columns: table => new
                {
                    MinLvl = table.Column<byte>(nullable: false),
                    MinAmount = table.Column<byte>(nullable: false),
                    SuccessResult = table.Column<int>(nullable: false),
                    FailResult = table.Column<int>(nullable: false),
                    NpcPriceDivisor = table.Column<int>(nullable: false),
                    AddPercentage = table.Column<byte>(nullable: false),
                    RefID = table.Column<byte>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    ItemDefinitionId = table.Column<Guid>(nullable: true),
                    SimpleCraftingSettingsId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCraftingRequiredItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemCraftingRequiredItem_ItemDefinition_ItemDefinitionId",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemCraftingRequiredItem_SimpleCraftingSettings_SimpleCraft~",
                        column: x => x.SimpleCraftingSettingsId,
                        principalSchema: "config",
                        principalTable: "SimpleCraftingSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemCraftingResultItem",
                schema: "config",
                columns: table => new
                {
                    RandLvlMin = table.Column<byte>(nullable: false),
                    RandLvlMax = table.Column<byte>(nullable: false),
                    RefID = table.Column<byte>(nullable: false),
                    AddLevel = table.Column<byte>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    ItemDefinitionId = table.Column<Guid>(nullable: true),
                    SimpleCraftingSettingsId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCraftingResultItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemCraftingResultItem_ItemDefinition_ItemDefinitionId",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemCraftingResultItem_SimpleCraftingSettings_SimpleCraftin~",
                        column: x => x.SimpleCraftingSettingsId,
                        principalSchema: "config",
                        principalTable: "SimpleCraftingSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemDefinitionCharacterClass",
                schema: "config",
                columns: table => new
                {
                    ItemDefinitionId = table.Column<Guid>(nullable: false),
                    CharacterClassId = table.Column<Guid>(nullable: false)
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
                    ItemDefinitionId = table.Column<Guid>(nullable: false),
                    ItemOptionDefinitionId = table.Column<Guid>(nullable: false)
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
                    ItemDefinitionId = table.Column<Guid>(nullable: false),
                    ItemSetGroupId = table.Column<Guid>(nullable: false)
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
                    Id = table.Column<Guid>(nullable: false),
                    ItemDefinitionId = table.Column<Guid>(nullable: true),
                    BonusOptionId = table.Column<Guid>(nullable: true),
                    ItemSetGroupId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOfItemSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOfItemSet_IncreasableItemOption_BonusOptionId",
                        column: x => x.BonusOptionId,
                        principalSchema: "config",
                        principalTable: "IncreasableItemOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemOfItemSet_ItemDefinition_ItemDefinitionId",
                        column: x => x.ItemDefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemOfItemSet_ItemSetGroup_ItemSetGroupId",
                        column: x => x.ItemSetGroupId,
                        principalSchema: "config",
                        principalTable: "ItemSetGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JewelMix",
                schema: "config",
                columns: table => new
                {
                    Number = table.Column<byte>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    SingleJewelId = table.Column<Guid>(nullable: true),
                    MixedJewelId = table.Column<Guid>(nullable: true),
                    GameConfigurationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JewelMix", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JewelMix_GameConfiguration_GameConfigurationId",
                        column: x => x.GameConfigurationId,
                        principalSchema: "config",
                        principalTable: "GameConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JewelMix_ItemDefinition_MixedJewelId",
                        column: x => x.MixedJewelId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JewelMix_ItemDefinition_SingleJewelId",
                        column: x => x.SingleJewelId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                schema: "data",
                columns: table => new
                {
                    ItemSlot = table.Column<byte>(nullable: false),
                    Durability = table.Column<byte>(nullable: false),
                    Level = table.Column<byte>(nullable: false),
                    HasSkill = table.Column<bool>(nullable: false),
                    SocketCount = table.Column<int>(nullable: false),
                    StorePrice = table.Column<int>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    DefinitionId = table.Column<Guid>(nullable: true),
                    ItemStorageId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_ItemDefinition_DefinitionId",
                        column: x => x.DefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Item_ItemStorage_ItemStorageId",
                        column: x => x.ItemStorageId,
                        principalSchema: "data",
                        principalTable: "ItemStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemAppearance",
                schema: "data",
                columns: table => new
                {
                    ItemSlot = table.Column<byte>(nullable: false),
                    Level = table.Column<byte>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    DefinitionId = table.Column<Guid>(nullable: true),
                    AppearanceDataId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAppearance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemAppearance_AppearanceData_AppearanceDataId",
                        column: x => x.AppearanceDataId,
                        principalSchema: "data",
                        principalTable: "AppearanceData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemAppearance_ItemDefinition_DefinitionId",
                        column: x => x.DefinitionId,
                        principalSchema: "config",
                        principalTable: "ItemDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemCrafting",
                schema: "config",
                columns: table => new
                {
                    Number = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ItemCraftingHandlerClassName = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    SimpleCraftingSettingsId = table.Column<Guid>(nullable: true),
                    MonsterDefinitionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCrafting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemCrafting_MonsterDefinition_MonsterDefinitionId",
                        column: x => x.MonsterDefinitionId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemCrafting_SimpleCraftingSettings_SimpleCraftingSettingsId",
                        column: x => x.SimpleCraftingSettingsId,
                        principalSchema: "config",
                        principalTable: "SimpleCraftingSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonsterAttribute",
                schema: "config",
                columns: table => new
                {
                    Value = table.Column<float>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    AttributeDefinitionId = table.Column<Guid>(nullable: true),
                    MonsterDefinitionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterAttribute_AttributeDefinition_AttributeDefinitionId",
                        column: x => x.AttributeDefinitionId,
                        principalSchema: "config",
                        principalTable: "AttributeDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonsterAttribute_MonsterDefinition_MonsterDefinitionId",
                        column: x => x.MonsterDefinitionId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonsterDefinitionDropItemGroup",
                schema: "config",
                columns: table => new
                {
                    MonsterDefinitionId = table.Column<Guid>(nullable: false),
                    DropItemGroupId = table.Column<Guid>(nullable: false)
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
                    Direction = table.Column<int>(nullable: false),
                    Quantity = table.Column<short>(nullable: false),
                    SpawnTrigger = table.Column<int>(nullable: false),
                    X1 = table.Column<byte>(nullable: false),
                    X2 = table.Column<byte>(nullable: false),
                    Y1 = table.Column<byte>(nullable: false),
                    Y2 = table.Column<byte>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    MonsterDefinitionId = table.Column<Guid>(nullable: true),
                    GameMapId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterSpawnArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterSpawnArea_GameMapDefinition_GameMapId",
                        column: x => x.GameMapId,
                        principalSchema: "config",
                        principalTable: "GameMapDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonsterSpawnArea_MonsterDefinition_MonsterDefinitionId",
                        column: x => x.MonsterDefinitionId,
                        principalSchema: "config",
                        principalTable: "MonsterDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemCraftingRequiredItemItemOptionType",
                schema: "config",
                columns: table => new
                {
                    ItemCraftingRequiredItemId = table.Column<Guid>(nullable: false),
                    ItemOptionTypeId = table.Column<Guid>(nullable: false)
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
                name: "ItemItemSetGroup",
                schema: "data",
                columns: table => new
                {
                    ItemId = table.Column<Guid>(nullable: false),
                    ItemSetGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemItemSetGroup", x => new { x.ItemId, x.ItemSetGroupId });
                    table.ForeignKey(
                        name: "FK_ItemItemSetGroup_Item_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "data",
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemItemSetGroup_ItemSetGroup_ItemSetGroupId",
                        column: x => x.ItemSetGroupId,
                        principalSchema: "config",
                        principalTable: "ItemSetGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemOptionLink",
                schema: "data",
                columns: table => new
                {
                    Level = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    ItemOptionId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOptionLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemOptionLink_Item_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "data",
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemOptionLink_IncreasableItemOption_ItemOptionId",
                        column: x => x.ItemOptionId,
                        principalSchema: "config",
                        principalTable: "IncreasableItemOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemAppearanceItemOptionType",
                schema: "data",
                columns: table => new
                {
                    ItemAppearanceId = table.Column<Guid>(nullable: false),
                    ItemOptionTypeId = table.Column<Guid>(nullable: false)
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
                name: "IX_ItemCraftingRequiredItem_ItemDefinitionId",
                schema: "config",
                table: "ItemCraftingRequiredItem",
                column: "ItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCraftingRequiredItem_SimpleCraftingSettingsId",
                schema: "config",
                table: "ItemCraftingRequiredItem",
                column: "SimpleCraftingSettingsId");

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
                name: "IX_ItemOptionDefinition_GameConfigurationId",
                schema: "config",
                table: "ItemOptionDefinition",
                column: "GameConfigurationId");

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
                name: "IX_LevelBonus_ItemBasePowerUpDefinitionId",
                schema: "config",
                table: "LevelBonus",
                column: "ItemBasePowerUpDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MagicEffectDefinition_GameConfigurationId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "GameConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_MagicEffectDefinition_PowerUpDefinitionId",
                schema: "config",
                table: "MagicEffectDefinition",
                column: "PowerUpDefinitionId");

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
                name: "IX_PowerUpDefinition_TargetAttributeId",
                schema: "config",
                table: "PowerUpDefinition",
                column: "TargetAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerUpDefinitionWithDuration_BoostId",
                schema: "config",
                table: "PowerUpDefinitionWithDuration",
                column: "BoostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PowerUpDefinitionWithDuration_DurationId",
                schema: "config",
                table: "PowerUpDefinitionWithDuration",
                column: "DurationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PowerUpDefinitionWithDuration_TargetAttributeId",
                schema: "config",
                table: "PowerUpDefinitionWithDuration",
                column: "TargetAttributeId");

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
                name: "IX_CharacterDropItemGroup_DropItemGroupId",
                schema: "data",
                table: "CharacterDropItemGroup",
                column: "DropItemGroupId");

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
                name: "IX_ItemItemSetGroup_ItemSetGroupId",
                schema: "data",
                table: "ItemItemSetGroup",
                column: "ItemSetGroupId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "AttributeRequirement",
                column: "ItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_Skill_SkillId",
                schema: "config",
                table: "AttributeRequirement",
                column: "SkillId",
                principalSchema: "config",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeRequirement_Skill_SkillId1",
                schema: "config",
                table: "AttributeRequirement",
                column: "SkillId1",
                principalSchema: "config",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemBasePowerUpDefinition_ItemDefinition_ItemDefinitionId",
                schema: "config",
                table: "ItemBasePowerUpDefinition",
                column: "ItemDefinitionId",
                principalSchema: "config",
                principalTable: "ItemDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MasterSkillDefinition_Skill_ReplacedSkillId",
                schema: "config",
                table: "MasterSkillDefinition",
                column: "ReplacedSkillId",
                principalSchema: "config",
                principalTable: "Skill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "AttributeDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MagicEffectDefinition_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MagicEffectDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_MasterSkillRoot_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "MasterSkillRoot");

            migrationBuilder.DropForeignKey(
                name: "FK_Skill_GameConfiguration_GameConfigurationId",
                schema: "config",
                table: "Skill");

            migrationBuilder.DropForeignKey(
                name: "FK_MasterSkillDefinition_AttributeDefinition_TargetAttributeId",
                schema: "config",
                table: "MasterSkillDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_PowerUpDefinitionWithDuration_AttributeDefinition_TargetAtt~",
                schema: "config",
                table: "PowerUpDefinitionWithDuration");

            migrationBuilder.DropForeignKey(
                name: "FK_Skill_AttributeDefinition_ElementalModifierTargetId",
                schema: "config",
                table: "Skill");

            migrationBuilder.DropForeignKey(
                name: "PowerUpDefinitionWithDuration_Boost",
                schema: "config",
                table: "PowerUpDefinitionWithDuration");

            migrationBuilder.DropForeignKey(
                name: "PowerUpDefinitionWithDuration_Duration",
                schema: "config",
                table: "PowerUpDefinitionWithDuration");

            migrationBuilder.DropForeignKey(
                name: "FK_MasterSkillDefinition_Skill_ReplacedSkillId",
                schema: "config",
                table: "MasterSkillDefinition");

            migrationBuilder.DropTable(
                name: "AttributeRelationship",
                schema: "config");

            migrationBuilder.DropTable(
                name: "AttributeRequirement",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ChatServerEndpoint",
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
                name: "GameMapDefinitionDropItemGroup",
                schema: "config");

            migrationBuilder.DropTable(
                name: "GameServerConfigurationGameMapDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "GameServerEndpoint",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemCrafting",
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
                name: "ItemOfItemSet",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemOptionOfLevel",
                schema: "config");

            migrationBuilder.DropTable(
                name: "JewelMix",
                schema: "config");

            migrationBuilder.DropTable(
                name: "LevelBonus",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MasterSkillDefinitionSkill",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MonsterAttribute",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MonsterDefinitionDropItemGroup",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MonsterSpawnArea",
                schema: "config");

            migrationBuilder.DropTable(
                name: "PlugInConfiguration",
                schema: "config");

            migrationBuilder.DropTable(
                name: "SkillCharacterClass",
                schema: "config");

            migrationBuilder.DropTable(
                name: "StatAttributeDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "WarpInfo",
                schema: "config");

            migrationBuilder.DropTable(
                name: "AccountCharacterClass",
                schema: "data");

            migrationBuilder.DropTable(
                name: "CharacterDropItemGroup",
                schema: "data");

            migrationBuilder.DropTable(
                name: "ItemAppearanceItemOptionType",
                schema: "data");

            migrationBuilder.DropTable(
                name: "ItemItemSetGroup",
                schema: "data");

            migrationBuilder.DropTable(
                name: "ItemOptionLink",
                schema: "data");

            migrationBuilder.DropTable(
                name: "LetterBody",
                schema: "data");

            migrationBuilder.DropTable(
                name: "SkillEntry",
                schema: "data");

            migrationBuilder.DropTable(
                name: "StatAttribute",
                schema: "data");

            migrationBuilder.DropTable(
                name: "Friend",
                schema: "friend");

            migrationBuilder.DropTable(
                name: "GuildMember",
                schema: "guild");

            migrationBuilder.DropTable(
                name: "ChatServerDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "GameClientDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "GameServerDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemCraftingRequiredItem",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemBasePowerUpDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MonsterDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ExitGate",
                schema: "config");

            migrationBuilder.DropTable(
                name: "DropItemGroup",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemAppearance",
                schema: "data");

            migrationBuilder.DropTable(
                name: "Item",
                schema: "data");

            migrationBuilder.DropTable(
                name: "IncreasableItemOption",
                schema: "config");

            migrationBuilder.DropTable(
                name: "LetterHeader",
                schema: "data");

            migrationBuilder.DropTable(
                name: "Guild",
                schema: "guild");

            migrationBuilder.DropTable(
                name: "GameServerConfiguration",
                schema: "config");

            migrationBuilder.DropTable(
                name: "SimpleCraftingSettings",
                schema: "config");

            migrationBuilder.DropTable(
                name: "AppearanceData",
                schema: "data");

            migrationBuilder.DropTable(
                name: "ItemDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemOptionDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemSetGroup",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemOptionType",
                schema: "config");

            migrationBuilder.DropTable(
                name: "PowerUpDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Character",
                schema: "data");

            migrationBuilder.DropTable(
                name: "ItemSlotType",
                schema: "config");

            migrationBuilder.DropTable(
                name: "Account",
                schema: "data");

            migrationBuilder.DropTable(
                name: "CharacterClass",
                schema: "config");

            migrationBuilder.DropTable(
                name: "ItemStorage",
                schema: "data");

            migrationBuilder.DropTable(
                name: "GameMapDefinition",
                schema: "config");

            migrationBuilder.DropTable(
                name: "GameConfiguration",
                schema: "config");

            migrationBuilder.DropTable(
                name: "AttributeDefinition",
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
                name: "PowerUpDefinitionWithDuration",
                schema: "config");

            migrationBuilder.DropTable(
                name: "MasterSkillRoot",
                schema: "config");
        }
    }
}
