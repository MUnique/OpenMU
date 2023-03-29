// <copyright file="GuidHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

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

/// <summary>
/// This class helps to create the same GUIDs for the same initial configuration data.
/// </summary>
internal static class GuidHelper
{
    private static readonly Dictionary<Type, short> TypeIds = new();

    /// <summary>
    /// Initializes the <see cref="GuidHelper" /> class.
    /// </summary>
    static GuidHelper()
    {
        TypeIds.Add(typeof(GameConfiguration), 1);
        TypeIds.Add(typeof(GameClientDefinition), 2);
        TypeIds.Add(typeof(GameServerDefinition), 3);
        TypeIds.Add(typeof(GameServerConfiguration), 4);
        TypeIds.Add(typeof(GameServerEndpoint), 5);
        TypeIds.Add(typeof(ConnectServerDefinition), 6);

        TypeIds.Add(typeof(ChatServerDefinition), 8);
        TypeIds.Add(typeof(ChatServerEndpoint), 9);

        TypeIds.Add(typeof(SystemConfiguration), 0x10);

        TypeIds.Add(typeof(AttributeDefinition), 0x20);
        TypeIds.Add(typeof(AttributeRelationship), 0x21);
        TypeIds.Add(typeof(AttributeRequirement), 0x22);
        TypeIds.Add(typeof(ConstValueAttribute), 0x23);
        TypeIds.Add(typeof(PowerUpDefinition), 0x24);
        TypeIds.Add(typeof(PowerUpDefinitionValue), 0x25);

        TypeIds.Add(typeof(CharacterClass), 0x40);
        TypeIds.Add(typeof(StatAttributeDefinition), 0x41);

        TypeIds.Add(typeof(ItemDefinition), 0x80);
        TypeIds.Add(typeof(ItemOption), 0x81);
        TypeIds.Add(typeof(ItemOptionCombinationBonus), 0x82);
        TypeIds.Add(typeof(ItemOptionDefinition), 0x83);
        TypeIds.Add(typeof(ItemOptionOfLevel), 0x84);
        TypeIds.Add(typeof(ItemOptionType), 0x85);
        TypeIds.Add(typeof(ItemSlotType), 0x86);
        TypeIds.Add(typeof(CombinationBonusRequirement), 0x87);
        TypeIds.Add(typeof(IncreasableItemOption), 0x88);
        TypeIds.Add(typeof(ItemBasePowerUpDefinition), 0x89);
        TypeIds.Add(typeof(ItemLevelBonusTable), 0x90);
        TypeIds.Add(typeof(LevelBonus), 0x91);
        TypeIds.Add(typeof(ItemSetGroup), 0x92);
        TypeIds.Add(typeof(ItemOfItemSet), 0x93);

        TypeIds.Add(typeof(ItemCrafting), 0x100);
        TypeIds.Add(typeof(ItemCraftingRequiredItem), 0x101);
        TypeIds.Add(typeof(ItemCraftingResultItem), 0x102);
        TypeIds.Add(typeof(SimpleCraftingSettings), 0x103);
        TypeIds.Add(typeof(JewelMix), 0x104);

        TypeIds.Add(typeof(DropItemGroup), 0x200);
        TypeIds.Add(typeof(ItemDropItemGroup), 0x201);

        TypeIds.Add(typeof(GameMapDefinition), 0x300);
        TypeIds.Add(typeof(EnterGate), 0x301);
        TypeIds.Add(typeof(ExitGate), 0x302);
        TypeIds.Add(typeof(BattleZoneDefinition), 0x310);
        TypeIds.Add(typeof(WarpInfo), 0x320);

        TypeIds.Add(typeof(Skill), 0x400);
        TypeIds.Add(typeof(SkillComboDefinition), 0x401);
        TypeIds.Add(typeof(SkillComboStep), 0x402);
        TypeIds.Add(typeof(MagicEffectDefinition), 0x410);
        TypeIds.Add(typeof(MasterSkillDefinition), 0x420);
        TypeIds.Add(typeof(MasterSkillRoot), 0x421);

        TypeIds.Add(typeof(MiniGameDefinition), 0x500);
        TypeIds.Add(typeof(MiniGameChangeEvent), 0x501);
        TypeIds.Add(typeof(MiniGameReward), 0x502);
        TypeIds.Add(typeof(MiniGameSpawnWave), 0x503);
        TypeIds.Add(typeof(MiniGameTerrainChange), 0x504);
        TypeIds.Add(typeof(Rectangle), 0x505);

        TypeIds.Add(typeof(MonsterDefinition), 0x600);
        TypeIds.Add(typeof(MonsterAttribute), 0x601);
        TypeIds.Add(typeof(MonsterSpawnArea), 0x602);

        TypeIds.Add(typeof(QuestDefinition), 0x700);
        TypeIds.Add(typeof(QuestItemRequirement), 0x701);
        TypeIds.Add(typeof(QuestMonsterKillRequirement), 0x702);
        TypeIds.Add(typeof(QuestReward), 0x703);

        TypeIds.Add(typeof(PlugInConfiguration), 0x800);

        TypeIds.Add(typeof(ItemStorage), 0x1000);
        TypeIds.Add(typeof(Item), 0x1001);
    }

    /// <summary>
    /// Sets the unique identifier for a <see cref="IIdentifiable"/> object.
    /// </summary>
    /// <typeparam name="T">The type of object which should get the id.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="guid">The unique identifier.</param>
    /// <exception cref="System.InvalidOperationException">Object is not identifiable</exception>
    public static void SetGuid<T>(this T obj, Guid guid)
    {
        if (obj is IIdentifiable identifiable)
        {
            identifiable.Id = guid;
            return;
        }

        throw new InvalidOperationException("Object is not identifiable");
    }

    /// <summary>
    /// Gets an item identifier.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The item identifier.</returns>
    public static short GetItemId(this ItemDefinition item)
    {
        return ((ushort)((item.Group << 12) | (ushort)item.Number)).ToSigned();
    }

    /// <summary>
    /// Creates and sets the unique identifier based on a single number.
    /// </summary>
    /// <typeparam name="T">The type of object which should get the id.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="number">The number.</param>
    public static void SetGuid<T>(this T obj, short number)
    {
        obj.SetGuid(CreateGuid<T>(number));
    }

    /// <summary>
    /// Creates and sets the unique identifier based on a parent and child number.
    /// </summary>
    /// <typeparam name="T">The type of object which should get the id.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="parentNumber">The parent number.</param>
    /// <param name="number">The child number.</param>
    public static void SetGuid<T>(this T obj, short parentNumber, short number)
    {
        obj.SetGuid(CreateGuid<T>(parentNumber, number));
    }

    /// <summary>
    /// Creates and sets the unique identifier based on three numbers.
    /// </summary>
    /// <typeparam name="T">The type of object which should get the id.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="parentNumber">The parent number.</param>
    /// <param name="number">The child number.</param>
    /// <param name="subNumber">The sub number.</param>
    public static void SetGuid<T>(this T obj, short parentNumber, short number, byte subNumber)
    {
        obj.SetGuid(CreateGuid<T>(parentNumber, number, subNumber));
    }

    /// <summary>
    /// Extracts the first two bytes of a guid.
    /// </summary>
    /// <param name="guid">The unique identifier.</param>
    /// <returns>The first two bytes of a guid.</returns>
    public static short ExtractFirstTwoBytes(this Guid guid)
    {
        var bytes = guid.ToByteArray();
        return NumberConversionExtensions.MakeWord(bytes[0], bytes[1]).ToSigned();
    }

    /// <summary>
    /// Creates the unique identifier based on a single number.
    /// </summary>
    /// <typeparam name="T">The type of object which should get the id.</typeparam>
    /// <param name="number">The number.</param>
    /// <returns>The created unique identifier.</returns>
    public static Guid CreateGuid<T>(short number)
    {
        if (!TypeIds.TryGetValue(typeof(T), out var typeId))
        {
            Debug.Fail($"unregistered type {typeof(T)}");
            return Guid.NewGuid();
        }

        return new Guid(typeId, number, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    }

    /// <summary>
    /// Creates the unique identifier based on a parent and child number.
    /// </summary>
    /// <typeparam name="T">The type of object which should get the id.</typeparam>
    /// <param name="parentNumber">The parent number.</param>
    /// <param name="number">The child number.</param>
    /// <returns>The created unique identifier.</returns>
    public static Guid CreateGuid<T>(short parentNumber, short number)
    {
        if (!TypeIds.TryGetValue(typeof(T), out var typeId))
        {
            Debug.Fail($"unregistered type {typeof(T)}");
            return Guid.NewGuid();
        }

        return new Guid(typeId, parentNumber, number, 0, 0, 0, 0, 0, 0, 0, 0);
    }

    /// <summary>
    /// Creates the unique identifier based on three numbers.
    /// </summary>
    /// <typeparam name="T">The type of object which should get the id.</typeparam>
    /// <param name="parentNumber">The parent number.</param>
    /// <param name="number">The child number.</param>
    /// <param name="subNumber">The sub number.</param>
    /// <returns>
    /// The created unique identifier.
    /// </returns>
    public static Guid CreateGuid<T>(short parentNumber, short number, byte subNumber)
    {
        if (!TypeIds.TryGetValue(typeof(T), out var typeId))
        {
            Debug.Fail($"unregistered type {typeof(T)}");
            return Guid.NewGuid();
        }

        return new Guid(typeId, parentNumber, number, subNumber, 0, 0, 0, 0, 0, 0, 0);
    }
}