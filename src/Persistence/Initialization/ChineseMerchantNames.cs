// <copyright file="ChineseMerchantNames.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Adds Simplified Chinese merchant names while preserving customized translations.
/// </summary>
internal static class ChineseMerchantNames
{
    private static readonly CultureInfo Chinese = CultureInfo.GetCultureInfo("zh-CN");
    private static readonly IReadOnlyDictionary<short, (string Neutral, string Chinese, string Legacy)> Names =
        new Dictionary<short, (string Neutral, string Chinese, string Legacy)>
        {
            [230] = ("Alex", "流浪商人阿莱斯", string.Empty),
            [231] = ("Thompson the Merchant", "武器商人托姆绅", string.Empty),
            [242] = ("Elf Lala", "精灵安吉拉", string.Empty),
            [243] = ("Eo the Craftsman", "工匠尤达", string.Empty),
            [244] = ("Caren the Barmaid", "老板娘莉娜", string.Empty),
            [245] = ("Izabel The Wizard", "魔导师露茜", string.Empty),
            [246] = ("Zienna The Weapons Merchant", "武器商人苏菲", string.Empty),
            [248] = ("Wandering Merchant Martin", "流浪商人马丁", string.Empty),
            [250] = ("Wandering Merchant Harold", "流浪商人海罗德", string.Empty),
            [251] = ("Hanzo The Blacksmith", "铁匠汉斯", string.Empty),
            [253] = ("Potion Girl Amy", "少女安娜", string.Empty),
            [254] = ("Pasi The Mage", "魔导师帕希", string.Empty),
            [255] = ("Lumen the Barmaid", "老板娘莉雅", string.Empty),
            [259] = ("Oracle Layla", "雷拉", "神谕者莱拉"),
            [415] = ("Silvia", "塞尔维亚", "西尔维娅"),
            [416] = ("Rhea", "雷亚", string.Empty),
            [417] = ("Marce", "摩尔塞", string.Empty),
            [577] = ("Leina the General Goods Merchant", "蕾娜", string.Empty),
            [578] = ("Weapons Merchant Bolo", "贝莱", string.Empty),
        };

    /// <summary>
    /// Fills missing translations and corrects known legacy translations of built-in merchants.
    /// </summary>
    /// <param name="configuration">The game configuration to update.</param>
    public static void Apply(GameConfiguration configuration)
    {
        foreach (var merchant in configuration.Monsters)
        {
            if (merchant.MerchantStore is null
                || !Names.TryGetValue(merchant.Number, out var names)
                || merchant.Designation.ValueInNeutralLanguage != names.Neutral)
            {
                continue;
            }

            var current = merchant.Designation.GetTranslation(Chinese, fallbackToNeutral: false);
            if (current != names.Chinese
                && (string.IsNullOrEmpty(current) || current == names.Legacy || current == names.Neutral))
            {
                merchant.Designation = merchant.Designation.WithTranslation(Chinese, names.Chinese);
            }
        }
    }
}
