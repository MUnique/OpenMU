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
    private static readonly CultureInfo ChineseCulture = CultureInfo.GetCultureInfo("zh-CN");
    private static readonly IReadOnlyDictionary<short, (string NeutralName, string ChineseName)> NamesByNumber =
        new Dictionary<short, (string NeutralName, string ChineseName)>
        {
            [230] = ("Alex", "流浪商人阿莱斯"),
            [231] = ("Thompson the Merchant", "武器商人托姆绅"),
            [242] = ("Elf Lala", "精灵安吉拉"),
            [243] = ("Eo the Craftsman", "工匠尤达"),
            [244] = ("Caren the Barmaid", "老板娘莉娜"),
            [245] = ("Izabel The Wizard", "魔导师露茜"),
            [246] = ("Zienna The Weapons Merchant", "武器商人苏菲"),
            [248] = ("Wandering Merchant Martin", "流浪商人马丁"),
            [250] = ("Wandering Merchant Harold", "流浪商人海罗德"),
            [251] = ("Hanzo The Blacksmith", "铁匠汉斯"),
            [253] = ("Potion Girl Amy", "少女安娜"),
            [254] = ("Pasi The Mage", "魔导师帕希"),
            [255] = ("Lumen the Barmaid", "老板娘莉雅"),
            [259] = ("Oracle Layla", "雷拉"),
            [415] = ("Silvia", "塞尔维亚"),
            [416] = ("Rhea", "雷亚"),
            [417] = ("Marce", "摩尔塞"),
            [577] = ("Leina the General Goods Merchant", "蕾娜"),
            [578] = ("Weapons Merchant Bolo", "贝莱"),
        };

    /// <summary>
    /// Fills missing translations and replaces English copies of built-in merchants.
    /// </summary>
    /// <param name="configuration">The game configuration to update.</param>
    public static void AddMissingTranslations(GameConfiguration configuration)
    {
        foreach (var merchant in configuration.Monsters)
        {
            if (merchant.MerchantStore is null
                || !NamesByNumber.TryGetValue(merchant.Number, out var names)
                || merchant.Designation.ValueInNeutralLanguage != names.NeutralName)
            {
                continue;
            }

            var current = merchant.Designation.GetTranslation(ChineseCulture, fallbackToNeutral: false);
            if (string.IsNullOrEmpty(current) || current == names.NeutralName)
            {
                merchant.Designation = merchant.Designation.WithTranslation(ChineseCulture, names.ChineseName);
            }
        }
    }
}
