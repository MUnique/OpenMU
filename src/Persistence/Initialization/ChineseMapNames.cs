// <copyright file="ChineseMapNames.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Adds Simplified Chinese map names while preserving customized translations.
/// </summary>
internal static class ChineseMapNames
{
    private static readonly CultureInfo ChineseCulture = CultureInfo.GetCultureInfo("zh-CN");
    private static readonly IReadOnlyDictionary<(short Number, string NeutralName), string> NamesByNumberAndNeutralName =
        new Dictionary<(short Number, string NeutralName), string>
        {
            [(0, "Lorencia")] = "勇者大陆",
            [(1, "Dungeon")] = "地下城",
            [(2, "Devias")] = "冰风谷",
            [(3, "Noria")] = "仙踪林",
            [(4, "Lost Tower")] = "失落之塔",
            [(7, "Atlans")] = "亚特兰蒂斯",
            [(8, "Tarkan")] = "死亡沙漠",
            [(9, "Devil Square 1")] = "恶魔广场 1",
            [(9, "Devil Square 2")] = "恶魔广场 2",
            [(9, "Devil Square 3")] = "恶魔广场 3",
            [(9, "Devil Square 4")] = "恶魔广场 4",
            [(10, "Icarus")] = "天空之城",
            [(11, "Blood Castle 1")] = "血色城堡 1",
            [(12, "Blood Castle 2")] = "血色城堡 2",
            [(13, "Blood Castle 3")] = "血色城堡 3",
            [(14, "Blood Castle 4")] = "血色城堡 4",
            [(15, "Blood Castle 5")] = "血色城堡 5",
            [(16, "Blood Castle 6")] = "血色城堡 6",
            [(17, "Blood Castle 7")] = "血色城堡 7",
            [(18, "Chaos Castle 1")] = "赤色要塞 1",
            [(19, "Chaos Castle 2")] = "赤色要塞 2",
            [(20, "Chaos Castle 3")] = "赤色要塞 3",
            [(21, "Chaos Castle 4")] = "赤色要塞 4",
            [(22, "Chaos Castle 5")] = "赤色要塞 5",
            [(23, "Chaos Castle 6")] = "赤色要塞 6",
            [(24, "Kalima 1")] = "卡利玛神庙 1",
            [(25, "Kalima 2")] = "卡利玛神庙 2",
            [(26, "Kalima 3")] = "卡利玛神庙 3",
            [(27, "Kalima 4")] = "卡利玛神庙 4",
            [(28, "Kalima 5")] = "卡利玛神庙 5",
            [(29, "Kalima 6")] = "卡利玛神庙 6",
            [(30, "Valley of Loren")] = "罗兰峡谷",
            [(31, "Land_of_Trials")] = "魔炼之地",
            [(32, "Devil Square 5")] = "恶魔广场 5",
            [(32, "Devil Square 6")] = "恶魔广场 6",
            [(32, "Devil Square 7")] = "恶魔广场 7",
            [(33, "Aida")] = "幽暗森林",
            [(34, "Crywolf Fortress")] = "狼魂要塞",
            [(36, "Kalima 7")] = "卡利玛神庙 7",
            [(37, "Kanturu_I")] = "坎特鲁废墟",
            [(38, "Kanturu_III")] = "坎特鲁遗址",
            [(39, "Kanturu Event")] = "提炼之塔",
            [(41, "Barracks of Balgass")] = "巴卡斯兵营",
            [(42, "Balgass Refuge")] = "巴卡斯休息室",
            [(45, "Illusion Temple 1")] = "幻影寺院 1",
            [(46, "Illusion Temple 2")] = "幻影寺院 2",
            [(47, "Illusion Temple 3")] = "幻影寺院 3",
            [(48, "Illusion Temple 4")] = "幻影寺院 4",
            [(49, "Illusion Temple 5")] = "幻影寺院 5",
            [(50, "Illusion Temple 6")] = "幻影寺院 6",
            [(51, "Elvenland")] = "幻术园",
            [(52, "Blood Castle 8")] = "血色城堡 8",
            [(53, "Chaos Castle 7")] = "赤色要塞 7",
            [(56, "Swamp Of Calmness")] = "安宁池",
            [(57, "LaCleon")] = "冰霜之城",
            [(58, "LaCleon Boss")] = "孵化魔地",
            [(63, "Vulcanus")] = "囚禁之岛",
            [(64, "Duel Arena")] = "角斗场",
            [(65, "Doppelgaenger 1")] = "生魂广场 1",
            [(66, "Doppelgaenger 2")] = "生魂广场 2",
            [(67, "Doppelgaenger 3")] = "生魂广场 3",
            [(68, "Doppelgaenger 4")] = "生魂广场 4",
            [(69, "Fortress of Imperial Guardian 1")] = "帝国要塞 1",
            [(70, "Fortress of Imperial Guardian 2")] = "帝国要塞 2",
            [(71, "Fortress of Imperial Guardian 3")] = "帝国要塞 3",
            [(72, "Fortress of Imperial Guardian 4")] = "帝国要塞 4",
            [(79, "LorenMarket")] = "罗兰市集",
            [(80, "Karutan 1")] = "卡伦特 1",
            [(81, "Karutan 2")] = "卡伦特 2",
        };

    /// <summary>
    /// Fills missing translations and replaces English copies of built-in maps.
    /// </summary>
    /// <param name="configuration">The game configuration to update.</param>
    public static void AddMissingTranslations(GameConfiguration configuration)
    {
        foreach (var map in configuration.Maps)
        {
            if (!NamesByNumberAndNeutralName.TryGetValue((map.Number, map.Name.ValueInNeutralLanguage), out var chineseName))
            {
                continue;
            }

            var current = map.Name.GetTranslation(ChineseCulture, fallbackToNeutral: false);
            if (string.IsNullOrEmpty(current) || current == map.Name.ValueInNeutralLanguage)
            {
                map.Name = map.Name.WithTranslation(ChineseCulture, chineseName);
            }
        }
    }
}
