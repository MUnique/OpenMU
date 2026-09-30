// <copyright file="ChineseCharacterClassNames.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Adds official Simplified Chinese class names while preserving customized translations.
/// </summary>
internal static class ChineseCharacterClassNames
{
    private static readonly CultureInfo Chinese = CultureInfo.GetCultureInfo("zh-CN");
    private static readonly IReadOnlyDictionary<byte, (string Neutral, string Chinese, string Legacy)> Names =
        new Dictionary<byte, (string Neutral, string Chinese, string Legacy)>
        {
            [0] = ("Dark Wizard", "魔法师", "黑暗巫师"),
            [2] = ("Soul Master", "魔导师", "灵魂大师"),
            [3] = ("Grand Master", "神导师", "至尊大师"),
            [4] = ("Dark Knight", "剑士", "黑暗骑士"),
            [6] = ("Blade Knight", "骑士", "剑圣"),
            [7] = ("Blade Master", "神骑士", "剑魂"),
            [8] = ("Fairy Elf", "弓箭手", "圣射手"),
            [10] = ("Muse Elf", "圣射手", "精灵导师"),
            [11] = ("High Elf", "神射手", "高阶精灵"),
            [12] = ("Magic Gladiator", "魔剑士", "魔剑士"),
            [13] = ("Duel Master", "剑圣", "决战骑士"),
            [16] = ("Dark Lord", "圣导师", "黑暗领主"),
            [17] = ("Lord Emperor", "祭祀", "帝王"),
            [20] = ("Summoner", "召唤术师", "召唤术士"),
            [22] = ("Bloody Summoner", "召唤导师", "血召唤"),
            [23] = ("Dimension Master", "召唤巫师", "次元大师"),
            [24] = ("Rage Fighter", "格斗家", "圣导师"),
            [25] = ("Fist Master", "格斗大师", "拳术大师"),
        };

    /// <summary>
    /// Fills missing translations and corrects known legacy translations of built-in classes.
    /// </summary>
    /// <param name="configuration">The game configuration to update.</param>
    public static void Apply(GameConfiguration configuration)
    {
        foreach (var characterClass in configuration.CharacterClasses)
        {
            if (!Names.TryGetValue(characterClass.Number, out var names)
                || characterClass.Name.ValueInNeutralLanguage != names.Neutral)
            {
                continue;
            }

            var current = characterClass.Name.GetTranslation(Chinese, fallbackToNeutral: false);
            if (current != names.Chinese
                && (string.IsNullOrEmpty(current) || current == names.Legacy || current == names.Neutral))
            {
                characterClass.Name = characterClass.Name.WithTranslation(Chinese, names.Chinese);
            }
        }
    }
}
