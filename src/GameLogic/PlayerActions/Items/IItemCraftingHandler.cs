// <copyright file="IItemCraftingHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.NPC;

    /// <summary>
    /// Interface for an item crafting handler.
    /// </summary>
    public interface IItemCraftingHandler
    {
        /// <summary>
        /// Mixes the items of the <see cref="Player.TemporaryStorage"/> with this crafting handler.
        /// </summary>
        /// <param name="player">The mixing player.</param>
        /// <returns>The crafting result and the resulting item; if there are multiple, only the last one is returned.</returns>
        (CraftingResult, Item) DoMix(Player player);
    }

    /*
switch (mixTypeId)
{
    case 0x01:  //Make Chaos Weapon +
    case 0x03:  //Item+10 Combination +
    case 0x04:  //Item+11 Combination +
    case 0x16:  //Item+12 Combination +
    case 0x17:  //Item+13 Combination +
    case 0x31:  //Item+14 Combination +
    case 0x32:  //Item+15 Combination +
    case 0x06:  //Fruits +
    case 0x21:  //Gemstone -> JOH +
    case 0x22:  //Making Lower/Higher Refine Stone
    case 0x23:  //Removing JOH Opt from Item
    case 0x2A:  //Seeds?
    case 0x1E: //SD Potion
    case 0x0F: //Potion of Bless +
    case 0x10: //Potion of Soul 50k zen +
        break;
}*/
/*
    enum CHAOS_TYPE
    {
        CHAOS_TYPE_DEFAULT = 0x1,
        CHAOS_TYPE_DEVILSQUARE = 0x2,
        CHAOS_TYPE_UPGRADE_10 = 0x3,
        CHAOS_TYPE_UPGRADE_11 = 0x4,
        CHAOS_TYPE_UPGRADE_12 = 0x16,
        CHAOS_TYPE_UPGRADE_13 = 0x17,
        CHAOS_TYPE_DINORANT = 0x5,
        CHAOS_TYPE_FRUIT = 0x6,
        CHAOS_TYPE_SECOND_WING = 0x7,
        CHAOS_TYPE_BLOODCATLE = 0x8,
        CHAOS_TYPE_FIRST_WING = 0xb,
        CHAOS_TYPE_SETITEM = 0xc,
        CHAOS_TYPE_DARKHORSE = 0xd,
        CHAOS_TYPE_DARKSPIRIT = 0xe,
        CHAOS_TYPE_CLOAK = 0x18,
        CHAOS_TYPE_BLESS_POTION = 0xf,
        CHAOS_TYPE_SOUL_POTION = 0x10,
        CHAOS_TYPE_LIFE_STONE = 0x11,

            CHAOS_TYPE_CASTLE_ITEM = 0x12,

            CHAOS_TYPE_HT_BOX = 0x14,
            CHAOS_TYPE_FENRIR_01 = 0x19,
            CHAOS_TYPE_FENRIR_02 = 0x1a,
            CHAOS_TYPE_FENRIR_03 = 0x1b,
            CHAOS_TYPE_FENRIR_04 = 0x1c,
            CHAOS_TYPE_COMPOUNDPOTION_LV1 = 0x1e,
            CHAOS_TYPE_COMPOUNTPOTION_LV2 = 0x1f,
            CHAOS_TYPE_COMPOUNTPOTION_LV3 = 0x20,
            CHAOS_TYPE_JEWELOFHARMONY_PURITY = 0x21,
            CHAOS_TYPE_JEWELOFHARMONY_MIX_SMELTINGITEM = 0x22,
            CHAOS_TYPE_JEWELOFHARMONY_RESTORE_ITEM = 0x23,
            CHAOS_TYPE_380_OPTIONITEM = 0x24,
            CHAOS_TYPE_LOTTERY_MIX = 0x25,
            CHAOS_TYPE_CONDOR_FEATHER = 0x26,
            CHAOS_TYPE_THIRD_WING = 0x27,
            SEED_TYPE_SEEDEXTRACT = 0x2a,
            SEED_TYPE_SEEDSPHERE = 0x2b,
            SEED_TYPE_SEEDCALC = 0x2c,
            SEED_TYPE_NEWMIX2 = 0x2d,
        }*/
}
