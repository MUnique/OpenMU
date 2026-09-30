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
    private static readonly CultureInfo ChineseCulture = CultureInfo.GetCultureInfo("zh-CN");
    private static readonly IReadOnlyDictionary<byte, (string NeutralName, string ChineseName)> NamesByNumber =
        new Dictionary<byte, (string NeutralName, string ChineseName)>
        {
            [0] = ("Dark Wizard", "魔法师"),
            [2] = ("Soul Master", "魔导师"),
            [3] = ("Grand Master", "神导师"),
            [4] = ("Dark Knight", "剑士"),
            [6] = ("Blade Knight", "骑士"),
            [7] = ("Blade Master", "神骑士"),
            [8] = ("Fairy Elf", "弓箭手"),
            [10] = ("Muse Elf", "圣射手"),
            [11] = ("High Elf", "神射手"),
            [12] = ("Magic Gladiator", "魔剑士"),
            [13] = ("Duel Master", "剑圣"),
            [16] = ("Dark Lord", "圣导师"),
            [17] = ("Lord Emperor", "祭祀"),
            [20] = ("Summoner", "召唤术师"),
            [22] = ("Bloody Summoner", "召唤导师"),
            [23] = ("Dimension Master", "召唤巫师"),
            [24] = ("Rage Fighter", "格斗家"),
            [25] = ("Fist Master", "格斗大师"),
        };

    /// <summary>
    /// Fills missing translations and replaces English copies of built-in classes.
    /// </summary>
    /// <param name="configuration">The game configuration to update.</param>
    public static void AddMissingTranslations(GameConfiguration configuration)
    {
        foreach (var characterClass in configuration.CharacterClasses)
        {
            if (!NamesByNumber.TryGetValue(characterClass.Number, out var names)
                || characterClass.Name.ValueInNeutralLanguage != names.NeutralName)
            {
                continue;
            }

            var current = characterClass.Name.GetTranslation(ChineseCulture, fallbackToNeutral: false);
            if (string.IsNullOrEmpty(current) || current == names.NeutralName)
            {
                characterClass.Name = characterClass.Name.WithTranslation(ChineseCulture, names.ChineseName);
            }
        }
    }
}
