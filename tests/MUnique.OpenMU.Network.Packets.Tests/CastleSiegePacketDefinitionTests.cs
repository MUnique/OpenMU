// <copyright file="CastleSiegePacketDefinitionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets.Tests;

using System.IO;
using System.Xml.Linq;

/// <summary>
/// Verifies the Castle Siege packet layouts which are consumed and produced by MuMain.
/// </summary>
[TestFixture]
public class CastleSiegePacketDefinitionTests
{
    private const string ClientPacketDefinitions = "src/Network/Packets/ClientToServer/ClientToServerPackets.xml";
    private const string ServerPacketDefinitions = "src/Network/Packets/ServerToClient/ServerToClientPackets.xml";
    private static readonly XNamespace PacketDefinitionNamespace = "http://www.munique.net/OpenMU/PacketDefinitions";

    /// <summary>
    /// Verifies a client-to-server packet definition against MuMain's sending layout.
    /// </summary>
    /// <param name="name">The packet name.</param>
    /// <param name="headerType">The expected header type.</param>
    /// <param name="code">The expected packet code.</param>
    /// <param name="subCode">The expected packet sub-code.</param>
    /// <param name="length">The expected packet length.</param>
    /// <param name="fields">The expected field layout.</param>
    [TestCase("CastleSiegeStatusRequest", "C1HeaderWithSubCode", "B2", "00", "4", "")]
    [TestCase("CastleSiegeRegistrationRequest", "C1HeaderWithSubCode", "B2", "01", "4", "")]
    [TestCase("CastleSiegeUnregisterRequest", "C1HeaderWithSubCode", "B2", "02", "5", "4:Boolean:IsGivingUp")]
    [TestCase("CastleSiegeRegistrationStateRequest", "C1HeaderWithSubCode", "B2", "03", "4", "")]
    [TestCase("CastleSiegeMarkRegistration", "C1HeaderWithSubCode", "B2", "04", "5", "4:Byte:ItemIndex")]
    [TestCase("CastleSiegeDefenseBuyRequest", "C1HeaderWithSubCode", "B2", "05", "12", "4:IntegerLittleEndian:NpcNumber;8:IntegerLittleEndian:NpcIndex")]
    [TestCase("CastleSiegeDefenseRepairRequest", "C1HeaderWithSubCode", "B2", "06", "12", "4:IntegerLittleEndian:NpcNumber;8:IntegerLittleEndian:NpcIndex")]
    [TestCase(
        "CastleSiegeDefenseUpgradeRequest",
        "C1HeaderWithSubCode",
        "B2",
        "07",
        "20",
        "4:IntegerLittleEndian:NpcNumber;8:IntegerLittleEndian:NpcIndex;"
        + "12:IntegerLittleEndian:NpcUpgradeType;16:IntegerLittleEndian:NpcUpgradeValue")]
    [TestCase("CastleSiegeTaxInfoRequest", "C1HeaderWithSubCode", "B2", "08", "4", "")]
    [TestCase("CastleSiegeTaxChangeRequest", "C1HeaderWithSubCode", "B2", "09", "9", "4:Byte:TaxType;5:IntegerBigEndian:TaxRate")]
    [TestCase("CastleSiegeTaxMoneyWithdraw", "C1HeaderWithSubCode", "B2", "10", "8", "4:IntegerBigEndian:Amount")]
    [TestCase("ToggleCastleGateRequest", "C1HeaderWithSubCode", "B2", "12", "7", "4:Boolean:IsOpen;5:ShortBigEndian:GateId")]
    [TestCase("CastleGuildCommand", "C1HeaderWithSubCode", "B2", "1D", "8", "4:Byte:Team;5:Byte:PositionX;6:Byte:PositionY;7:Byte:Command")]
    [TestCase("CastleSiegeHuntingZoneEntranceSetting", "C1HeaderWithSubCode", "B2", "1F", "5", "4:Boolean:IsPublic")]
    [TestCase("CastleSiegeGateListRequest", "C1HeaderWithSubCode", "B3", "01", "4", "")]
    [TestCase("CastleSiegeStatueListRequest", "C1HeaderWithSubCode", "B3", "02", "4", "")]
    [TestCase("CastleSiegeRegisteredGuildsListRequest", "C1Header", "B4", "", "3", "")]
    [TestCase("CastleOwnerListRequest", "C1Header", "B5", "", "3", "")]
    [TestCase("FireCatapultRequest", "C1HeaderWithSubCode", "B7", "01", "7", "4:ShortBigEndian:CatapultId;6:Byte:TargetAreaIndex")]
    [TestCase("WeaponExplosionRequest", "C1HeaderWithSubCode", "B7", "04", "6", "4:ShortBigEndian:CatapultId")]
    [TestCase("GuildLogoOfCastleOwnerRequest", "C1HeaderWithSubCode", "B9", "02", "4", "")]
    [TestCase("CastleSiegeHuntingZoneEnterRequest", "C1HeaderWithSubCode", "B9", "05", "8", "4:IntegerLittleEndian:Money")]
    [TestCase("GuildRelationshipChangeRequest", "C1Header", "E5", "", "7", "3:Enum:RelationshipType;4:Enum:RequestType;5:ShortBigEndian:TargetPlayerId")]
    [TestCase("GuildRelationshipChangeResponse", "C1Header", "E6", "", "8", "3:Enum:RelationshipType;4:Enum:RequestType;5:Boolean:Response;6:ShortBigEndian:TargetPlayerId")]
    [TestCase("RequestAllianceList", "C1Header", "E9", "", "3", "")]
    [TestCase("RemoveAllianceGuildRequest", "C1HeaderWithSubCode", "EB", "01", "12", "4:String:GuildName[8]")]
    public void ClientPacketMatchesMuMain(
        string name,
        string headerType,
        string code,
        string subCode,
        string length,
        string fields)
    {
        AssertPacketDefinition(
            ClientPacketDefinitions,
            name,
            headerType,
            code,
            subCode,
            length,
            fields);
    }

    /// <summary>
    /// Verifies a server-to-client packet definition against MuMain's receiving layout.
    /// </summary>
    /// <param name="name">The packet name.</param>
    /// <param name="headerType">The expected header type.</param>
    /// <param name="code">The expected packet code.</param>
    /// <param name="subCode">The expected packet sub-code.</param>
    /// <param name="length">The expected packet length.</param>
    /// <param name="fields">The expected field layout.</param>
    [TestCase(
        "CastleSiegeStatusResponse",
        "C1HeaderWithSubCode",
        "B2",
        "00",
        "46",
        "4:Byte:Result;5:Byte:State;6:ShortBigEndian:StartYear;8:Byte:StartMonth;9:Byte:StartDay;"
        + "10:Byte:StartHour;11:Byte:StartMinute;12:ShortBigEndian:EndYear;14:Byte:EndMonth;"
        + "15:Byte:EndDay;16:Byte:EndHour;17:Byte:EndMinute;18:ShortBigEndian:SiegeStartYear;"
        + "20:Byte:SiegeStartMonth;21:Byte:SiegeStartDay;22:Byte:SiegeStartHour;"
        + "23:Byte:SiegeStartMinute;24:String:GuildName[8];32:String:GuildMasterName[10];"
        + "42:IntegerBigEndian:RemainingTime")]
    [TestCase("CastleSiegeRegistrationResponse", "C1HeaderWithSubCode", "B2", "01", "13", "4:Byte:Result;5:String:GuildName[8]")]
    [TestCase("CastleSiegeUnregisterResponse", "C1HeaderWithSubCode", "B2", "02", "14", "4:Byte:Result;5:Boolean:IsGivingUp;6:String:GuildName[8]")]
    [TestCase(
        "CastleSiegeRegistrationStateResponse",
        "C1HeaderWithSubCode",
        "B2",
        "03",
        "19",
        "4:Byte:Result;5:String:GuildName[8];13:IntegerBigEndian:GuildMarkCount;"
        + "17:Boolean:IsGivingUp;18:Byte:RegistrationRank")]
    [TestCase("CastleSiegeMarkRegistrationResponse", "C1HeaderWithSubCode", "B2", "04", "17", "4:Byte:Result;5:String:GuildName[8];13:IntegerBigEndian:GuildMarkCount")]
    [TestCase("CastleSiegeDefenseBuyResponse", "C1HeaderWithSubCode", "B2", "05", "13", "4:Byte:Result;5:IntegerLittleEndian:NpcNumber;9:IntegerLittleEndian:NpcIndex")]
    [TestCase(
        "CastleSiegeDefenseRepairResponse",
        "C1HeaderWithSubCode",
        "B2",
        "06",
        "21",
        "4:Byte:Result;5:IntegerLittleEndian:NpcNumber;9:IntegerLittleEndian:NpcIndex;"
        + "13:IntegerLittleEndian:CurrentHp;17:IntegerLittleEndian:MaxHp")]
    [TestCase(
        "CastleSiegeDefenseUpgradeResponse",
        "C1HeaderWithSubCode",
        "B2",
        "07",
        "21",
        "4:Byte:Result;5:IntegerLittleEndian:NpcNumber;9:IntegerLittleEndian:NpcIndex;"
        + "13:IntegerLittleEndian:NpcUpgradeType;17:IntegerLittleEndian:NpcUpgradeValue")]
    [TestCase("CastleSiegeTaxInfoResponse", "C1HeaderWithSubCode", "B2", "08", "15", "4:Byte:Result;5:Byte:TaxRateChaosMachine;6:Byte:TaxRateNormal;7:LongBigEndian:Treasury")]
    [TestCase("CastleSiegeTaxChangeResponse", "C1HeaderWithSubCode", "B2", "09", "10", "4:Byte:Result;5:Byte:TaxType;6:IntegerBigEndian:TaxRate")]
    [TestCase("CastleSiegeTributeWithdrawResponse", "C1HeaderWithSubCode", "B2", "10", "13", "4:Byte:Result;5:LongBigEndian:Money")]
    [TestCase("CastleSiegeGateInterfaceResponse", "C1HeaderWithSubCode", "B2", "11", "7", "4:Byte:Result;5:ShortBigEndian:GateIndex")]
    [TestCase("CastleSiegeGateOperateResponse", "C1HeaderWithSubCode", "B2", "12", "8", "4:Byte:Result;5:Boolean:IsOpen;6:ShortBigEndian:GateIndex")]
    [TestCase("CastleSiegeGateStateNotification", "C1HeaderWithSubCode", "B2", "13", "7", "4:Boolean:IsOpen;5:ShortBigEndian:GateIndex")]
    [TestCase("CastleSiegeCrownSwitchState", "C1HeaderWithSubCode", "B2", "14", "9", "4:ShortBigEndian:SwitchIndex;6:ShortBigEndian:PlayerIndex;8:Byte:State")]
    [TestCase("CastleSiegeCrownAccessState", "C1HeaderWithSubCode", "B2", "15", "9", "4:Byte:State;5:IntegerLittleEndian:AccumulatedTimeMs")]
    [TestCase("CastleSiegeCrownStateUpdate", "C1HeaderWithSubCode", "B2", "16", "5", "4:Byte:State")]
    [TestCase("CastleSiegeBattleStartEnd", "C1HeaderWithSubCode", "B2", "17", "5", "4:Boolean:IsStarted")]
    [TestCase("CastleSiegeBattleProcess", "C1HeaderWithSubCode", "B2", "18", "13", "4:Byte:State;5:String:GuildName[8]")]
    [TestCase("CastleSiegeJoinSideNotification", "C1HeaderWithSubCode", "B2", "19", "5", "4:Byte:Side")]
    [TestCase("CastleSiegeTaxRateNotification", "C1HeaderWithSubCode", "B2", "1A", "6", "4:Byte:TaxType;5:Byte:TaxRate")]
    [TestCase("CastleSiegeMiniMapResponse", "C1HeaderWithSubCode", "B2", "1B", "5", "4:Byte:Result")]
    [TestCase("CastleSiegeGuildCommand", "C1HeaderWithSubCode", "B2", "1D", "8", "4:Byte:Team;5:Byte:PositionX;6:Byte:PositionY;7:Byte:Command")]
    [TestCase("CastleSiegeRemainingTime", "C1HeaderWithSubCode", "B2", "1E", "6", "4:Byte:Hour;5:Byte:Minute")]
    [TestCase("CastleSiegeHuntingZoneEntranceSettingResponse", "C1HeaderWithSubCode", "B2", "1F", "6", "4:Byte:Result;5:Boolean:IsPublic")]
    [TestCase("CastleSiegeSwitchInfo", "C1HeaderWithSubCode", "B2", "20", "27", "4:ShortBigEndian:SwitchIndex;6:Byte:State;7:Byte:JoinSide;8:String:GuildName[8];16:String:UserName[11]")]
    [TestCase("CastleSiegeNpcList", "C2Header", "B3", "", "", "4:Byte:Result;5:IntegerLittleEndian:NpcCount;9:Structure[]:NpcList")]
    [TestCase("CastleSiegeRegisteredGuildList", "C2Header", "B4", "", "", "4:Byte:Result;5:IntegerLittleEndian:GuildCount;9:Structure[]:Guilds")]
    [TestCase("CastleSiegeGuildList", "C2Header", "B5", "", "", "4:Byte:Result;5:IntegerLittleEndian:GuildCount;9:Structure[]:Guilds")]
    [TestCase("CastleSiegeMiniMapPlayerPositions", "C2Header", "B6", "", "", "4:IntegerLittleEndian:PlayerCount;8:Structure[]:Players")]
    [TestCase("CastleSiegeMachineInterface", "C1HeaderWithSubCode", "B7", "00", "8", "4:Byte:Result;5:Byte:MachineType;6:ShortBigEndian:NpcIndex")]
    [TestCase("CastleSiegeMachineUseResult", "C1HeaderWithSubCode", "B7", "01", "10", "4:Byte:Result;5:ShortBigEndian:NpcIndex;7:Byte:MachineType;8:Byte:TargetX;9:Byte:TargetY")]
    [TestCase("CastleSiegeMachineRegionNotify", "C1HeaderWithSubCode", "B7", "02", "7", "4:Byte:MachineType;5:Byte:TargetX;6:Byte:TargetY")]
    [TestCase("CastleSiegeLifeStoneBuildTime", "C1HeaderWithSubCode", "B9", "01", "7", "4:ShortBigEndian:NpcIndex;6:Byte:BuildTime")]
    [TestCase("CastleOwnerLogo", "C1HeaderWithSubCode", "B9", "02", "36", "4:Binary:Logo[32]")]
    [TestCase(
        "HuntingZoneGuardInfo",
        "C1HeaderWithSubCode",
        "B9",
        "03",
        "18",
        "4:Byte:Result;5:Boolean:IsEnabled;6:IntegerLittleEndian:CurrentPrice;"
        + "10:IntegerLittleEndian:MaxPrice;14:IntegerLittleEndian:UnitPrice")]
    [TestCase("CastleSiegeHuntingZoneEnterResponse", "C1HeaderWithSubCode", "B9", "05", "5", "4:Byte:Result")]
    [TestCase("CastleSiegeMiniMapNpcPositions", "C2Header", "BB", "", "", "4:Byte:NpcCount;5:Structure[]:Npcs")]
    [TestCase("GuildRelationshipRequest", "C1Header", "E5", "", "7", "3:Enum:RelationshipType;4:Enum:RequestType;5:ShortBigEndian:SenderId")]
    [TestCase("GuildRelationshipChangeResult", "C1Header", "E6", "", "8", "3:Enum:RelationshipType;4:Enum:RequestType;5:Enum:Result;6:ShortBigEndian:GuildMasterId")]
    [TestCase("AllianceList", "C2Header", "E9", "", "", "4:Byte:GuildCount;5:Boolean:Success;6:Byte:__RivalCount;7:Byte:__UnionCount;8:Structure[]:Guilds")]
    [TestCase("RemoveAllianceGuildResult", "C1HeaderWithSubCode", "EB", "01", "7", "4:Boolean:Result;5:Enum:RequestType;6:Enum:RelationshipType")]
    public void ServerPacketMatchesMuMain(
        string name,
        string headerType,
        string code,
        string subCode,
        string length,
        string fields)
    {
        AssertPacketDefinition(
            ServerPacketDefinitions,
            name,
            headerType,
            code,
            subCode,
            length,
            fields);
    }

    /// <summary>
    /// Verifies a variable packet's item structure against MuMain's receiving layout.
    /// </summary>
    /// <param name="packetName">The packet name.</param>
    /// <param name="structureName">The structure name.</param>
    /// <param name="length">The expected structure length.</param>
    /// <param name="fields">The expected structure field layout.</param>
    [TestCase(
        "CastleSiegeNpcList",
        "CastleSiegeNpcInfo",
        "27",
        "0:IntegerLittleEndian:NpcNumber;4:IntegerLittleEndian:NpcIndex;"
        + "8:IntegerLittleEndian:DefenseUpgradeLevel;12:IntegerLittleEndian:RegenerationLevel;"
        + "16:IntegerLittleEndian:MaxHp;20:IntegerLittleEndian:CurrentHp;"
        + "24:Byte:PositionX;25:Byte:PositionY;26:Boolean:IsAlive")]
    [TestCase("CastleSiegeRegisteredGuildList", "RegisteredGuildEntry", "14", "0:String:GuildName[8];8:IntegerBigEndian:GuildMarkCount;12:Boolean:IsGivingUp;13:Byte:SequenceNumber")]
    [TestCase("CastleSiegeGuildList", "CastleSiegeGuildEntry", "14", "0:Byte:Side;1:Boolean:IsInvolved;2:String:GuildName[8];10:IntegerLittleEndian:Score")]
    [TestCase("CastleSiegeMiniMapPlayerPositions", "MiniMapPlayerPosition", "2", "0:Byte:PositionX;1:Byte:PositionY")]
    [TestCase("CastleSiegeMiniMapNpcPositions", "MiniMapNpcPosition", "3", "0:Byte:NpcType;1:Byte:PositionX;2:Byte:PositionY")]
    [TestCase("AllianceList", "AllianceGuildEntry", "41", "0:Byte:MemberCount;1:Binary:Logo[32];33:String:GuildName[8]")]
    public void ServerPacketStructureMatchesMuMain(
        string packetName,
        string structureName,
        string length,
        string fields)
    {
        var definitions = LoadPacketDefinitions(ServerPacketDefinitions);
        var packet = FindPacket(definitions, packetName);
        var structure = packet.Element(PacketDefinitionNamespace + "Structures")!
            .Elements(PacketDefinitionNamespace + "Structure")
            .Single(element => element.Element(PacketDefinitionNamespace + "Name")!.Value == structureName);

        Assert.Multiple(() =>
        {
            Assert.That(structure.Element(PacketDefinitionNamespace + "Length")!.Value, Is.EqualTo(length));
            Assert.That(GetFieldLayout(structure), Is.EqualTo(fields));
        });
    }

    private static XDocument LoadPacketDefinitions(string relativePath)
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null)
        {
            var path = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(path))
            {
                return XDocument.Load(path);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not locate '{relativePath}'.");
    }

    private static XElement FindPacket(XDocument definitions, string packetName)
        => definitions.Root!
            .Element(PacketDefinitionNamespace + "Packets")!
            .Elements(PacketDefinitionNamespace + "Packet")
            .Single(element => element.Element(PacketDefinitionNamespace + "Name")!.Value == packetName);

    private static string GetFieldLayout(XElement parent)
        => string.Join(
            ';',
            parent.Element(PacketDefinitionNamespace + "Fields")!
                .Elements(PacketDefinitionNamespace + "Field")
                .Select(field =>
                {
                    var length = field.Element(PacketDefinitionNamespace + "Length")?.Value;
                    var lengthSuffix = length is null ? string.Empty : $"[{length}]";
                    var index = field.Element(PacketDefinitionNamespace + "Index")!.Value;
                    var type = field.Element(PacketDefinitionNamespace + "Type")!.Value;
                    var name = field.Element(PacketDefinitionNamespace + "Name")!.Value;
                    return $"{index}:{type}:{name}{lengthSuffix}";
                }));

    private static void AssertPacketDefinition(
        string relativePath,
        string name,
        string headerType,
        string code,
        string subCode,
        string length,
        string fields)
    {
        var definitions = LoadPacketDefinitions(relativePath);
        var packet = FindPacket(definitions, name);

        Assert.Multiple(() =>
        {
            Assert.That(packet.Element(PacketDefinitionNamespace + "HeaderType")!.Value, Is.EqualTo(headerType));
            Assert.That(packet.Element(PacketDefinitionNamespace + "Code")!.Value, Is.EqualTo(code));
            Assert.That(packet.Element(PacketDefinitionNamespace + "SubCode")?.Value ?? string.Empty, Is.EqualTo(subCode));
            Assert.That(packet.Element(PacketDefinitionNamespace + "Length")?.Value ?? string.Empty, Is.EqualTo(length));
            Assert.That(GetFieldLayout(packet), Is.EqualTo(fields));
        });
    }
}
