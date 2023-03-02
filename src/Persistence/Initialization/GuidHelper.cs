// <copyright file="GuidHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.PlugIns;

namespace MUnique.OpenMU.Persistence.Initialization;

/// <summary>
/// This class helps to create the same GUIDs for the same initial configuration data.
/// </summary>
internal static class GuidHelper
{
    private static readonly Dictionary<Type, short> _typeIds = new();

    private static int _lastTypeId;

    static GuidHelper()
    {
        _typeIds.Add(typeof(GameConfiguration), 1);
        _typeIds.Add(typeof(GameClientDefinition), 2);
        _typeIds.Add(typeof(GameServerDefinition), 3);
        _typeIds.Add(typeof(GameServerConfiguration), 4);
        _typeIds.Add(typeof(GameServerEndpoint), 5);
        _typeIds.Add(typeof(ConnectServerDefinition), 6);
        
        _typeIds.Add(typeof(ChatServerDefinition), 8);
        _typeIds.Add(typeof(ChatServerEndpoint), 9);

        _typeIds.Add(typeof(AttributeDefinition), 0x20);
        _typeIds.Add(typeof(AttributeRelationship), 0x21);
        _typeIds.Add(typeof(AttributeRequirement), 0x22);
        _typeIds.Add(typeof(ConstValueAttribute), 0x23);
        _typeIds.Add(typeof(PowerUpDefinition), 0x24);
        _typeIds.Add(typeof(PowerUpDefinitionValue), 0x25);

        _typeIds.Add(typeof(CharacterClass), 0x40);
        _typeIds.Add(typeof(StatAttributeDefinition), 0x41);


        // Item Definition data
        _typeIds.Add(typeof(ItemDefinition), 0x80);
        _typeIds.Add(typeof(ItemOption), 0x81);
        _typeIds.Add(typeof(ItemOptionCombinationBonus), 0x82);
        _typeIds.Add(typeof(ItemOptionDefinition), 0x83);
        _typeIds.Add(typeof(ItemOptionOfLevel), 0x84);
        _typeIds.Add(typeof(ItemOptionType), 0x85);
        _typeIds.Add(typeof(ItemSlotType), 0x86);
        _typeIds.Add(typeof(CombinationBonusRequirement), 0x87);
        _typeIds.Add(typeof(IncreasableItemOption), 0x88);
        _typeIds.Add(typeof(ItemBasePowerUpDefinition), 0x89);
        _typeIds.Add(typeof(ItemLevelBonusTable), 0x90);
        _typeIds.Add(typeof(LevelBonus), 0x91);
        _typeIds.Add(typeof(ItemSetGroup), 0x92);
        _typeIds.Add(typeof(ItemOfItemSet), 0x93);
        
        _typeIds.Add(typeof(ItemCrafting), 0x100);
        _typeIds.Add(typeof(ItemCraftingRequiredItem), 0x101);
        _typeIds.Add(typeof(ItemCraftingResultItem), 0x102);
        _typeIds.Add(typeof(SimpleCraftingSettings), 0x103);
        _typeIds.Add(typeof(JewelMix), 0x104);

        _typeIds.Add(typeof(DropItemGroup), 0x200);
        _typeIds.Add(typeof(ItemDropItemGroup), 0x201);
        // Maps
        _typeIds.Add(typeof(GameMapDefinition), 0x300);
        _typeIds.Add(typeof(EnterGate), 0x301);
        _typeIds.Add(typeof(ExitGate), 0x302);
        _typeIds.Add(typeof(BattleZoneDefinition), 0x310);
        _typeIds.Add(typeof(WarpInfo), 0x320);
        
        _typeIds.Add(typeof(Skill), 0x400);
        _typeIds.Add(typeof(SkillComboDefinition), 0x401);
        _typeIds.Add(typeof(SkillComboStep), 0x402);
        _typeIds.Add(typeof(MagicEffectDefinition), 0x410);
        _typeIds.Add(typeof(MasterSkillDefinition), 0x420);
        _typeIds.Add(typeof(MasterSkillRoot), 0x421);

        _typeIds.Add(typeof(MiniGameDefinition), 0x500);
        _typeIds.Add(typeof(MiniGameChangeEvent), 0x501);
        _typeIds.Add(typeof(MiniGameReward), 0x502);
        _typeIds.Add(typeof(MiniGameSpawnWave), 0x503);
        _typeIds.Add(typeof(MiniGameTerrainChange), 0x504);
        _typeIds.Add(typeof(Rectangle), 0x505);

        _typeIds.Add(typeof(MonsterDefinition), 0x600);
        _typeIds.Add(typeof(MonsterAttribute), 0x601);
        _typeIds.Add(typeof(MonsterSpawnArea), 0x602);

        _typeIds.Add(typeof(QuestDefinition), 0x700);
        _typeIds.Add(typeof(QuestItemRequirement), 0x701);
        _typeIds.Add(typeof(QuestMonsterKillRequirement), 0x702);
        _typeIds.Add(typeof(QuestReward), 0x703);

        _typeIds.Add(typeof(PlugInConfiguration), 0x800);

        _typeIds.Add(typeof(ItemStorage), 0x1000);
        _typeIds.Add(typeof(Item), 0x1001);
        
    }

    public static Guid CreateGuid<T>(short number)
    {
        if (!_typeIds.TryGetValue(typeof(T), out var typeId))
        {
            Debug.Fail($"unregistered type {typeof(T)}");
            return Guid.NewGuid();
        }

        return new Guid(typeId, number, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    }

    public static Guid CreateGuid<T>(short parentNumber, short number)
    {
        if (!_typeIds.TryGetValue(typeof(T), out var typeId))
        {
            Debug.Fail($"unregistered type {typeof(T)}");
            return Guid.NewGuid();
        }

        return new Guid(typeId, parentNumber, number, 0, 0, 0, 0, 0, 0, 0, 0);
    }

    public static Guid CreateGuid<T>(short parentNumber, short number, byte subNumber)
    {
        if (!_typeIds.TryGetValue(typeof(T), out var typeId))
        {
            Debug.Fail($"unregistered type {typeof(T)}");
            return Guid.NewGuid();
        }

        return new Guid(typeId, parentNumber, number, subNumber, 0, 0, 0, 0, 0, 0, 0);
    }

    public static void SetGuid<T>(this T obj, Guid guid)
    {
        if (obj is IIdentifiable identifiable)
        {
            identifiable.Id = guid;
            return;
        }

        throw new InvalidOperationException("Object is not identifiable");
    }

    public static short GetItemId(this ItemDefinition item)
    {
        return ((ushort)((item.Group << 12) | (ushort)item.Number)).ToSigned();
    }

    public static void SetGuid<T>(this T obj, short number)
    {
        obj.SetGuid(CreateGuid<T>(number));
    }

    public static void SetGuid<T>(this T obj, short parentNumber, short number)
    {
        obj.SetGuid(CreateGuid<T>(parentNumber, number));
    }

    public static void SetGuid<T>(this T obj, short parentNumber, short number, byte subnumber)
    {
        obj.SetGuid(CreateGuid<T>(parentNumber, number, subnumber));
    }

    public static short ExtractFirstTwoBytes(this Guid guid)
    {
        var bytes = guid.ToByteArray();
        return NumberConversionExtensions.MakeWord(bytes[0], bytes[1]).ToSigned();
    }
}

public static class ItemOptionDefinitionNumbers
{
    public static short Luck => 0x01;

    public static short DefenseOption => 0x02;

    public static short PhysicalAttack => 0x03;

    public static short WizardryAttack => 0x04;

    public static short CurseAttack => 0x05;

    public static short JeweleryHealth => 0x10;
    

    
    public static short ExcellentDefense => 0x12;
    public static short ExcellentPhysical => 0x13;
    public static short ExcellentWIzardry => 0x14;
    public static short ExcellentCurse => 0x15;

    public static short DefenseSetBonusOption => 0x20;
    public static short DefenseRateSetBonusOption => 0x21;

    public static short AncientOption => 0x29;
    public static short AncientBonus => 0x30;
    public static short SocketBonus => 0x31;
    public static short SocketFire => 0x32;

    public static short SocketWater => 0x33;
    public static short SocketIce => 0x34;

    public static short SocketWind => 0x35;

    public static short SocketLightning => 0x36;
    public static short SocketEarth => 0x37;

    public static short GuardianOption1 => 0x41;
    public static short GuardianOption2 => 0x42;

    public static short HarmonyDefense => 0x50;
    public static short HarmonyPhysical => 0x51;

    public static short HarmonyWizardry => 0x52;

    public static short HarmonyCurse => 0x53;

    public static short WingDefense => 0x60;
    public static short WingPhysical => 0x61;

    public static short WingWizardry => 0x62;

    public static short WingCurse => 0x63;
    public static short WingHealthRecover => 0x64;

    public static short Wing2nd => 0x65;
    public static short Cape => 0x66;

    public static short Wing3rd => 0x67;

    public static short EliteTransferSkeletonRing => 0x70;
    public static short SkeletonTransformationRing => 0x71;

    public static short PandaRing => 0x72;

    public static short WizardRing => 0x73;

    public static short Dino => 0x80;
    public static short Fenrir => 0x81;

}