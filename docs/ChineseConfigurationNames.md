# Simplified Chinese configuration names

This mapping supplements the existing `LocalizedString` data for character classes,
maps, merchants, monsters and NPCs in the 0.75, 0.95d and Season 6 initializers.
It does not translate client language files, item names or attribute resources.

## Matching and preservation

Mappings require both the built-in numeric identifier and the exact neutral English
name. Shared map numbers are distinguished by that name. Only a missing Chinese
translation, a copy of the English name, or the explicitly listed legacy translation
is replaced. Other translations and custom names are preserved. A custom name equal
to a listed legacy translation cannot be distinguished from that legacy value.
The update is therefore optional. No identifiers, progression links, stats, shops,
drops, spawn areas or map gates are changed.

New initializations apply these mappings before recording installed updates. Existing
installations use one optional configuration update for their initialization version.
See the [operator instructions](../docs-website/docs/admin-panel/configuration-updates.md).
No database schema migration or direct SQL is required.

The exact legacy replacements are listed alongside each mapping in
[classes](../src/Persistence/Initialization/CharacterClasses/ChineseCharacterClassNames.cs),
[maps](../src/Persistence/Initialization/ChineseMapNames.cs),
[merchants](../src/Persistence/Initialization/ChineseMerchantNames.cs) and
[monsters](../src/Persistence/Initialization/ChineseMonsterNames.cs).

`LocalizedString` currently stores these values under its two-letter `zh` key;
it does not distinguish Simplified and Traditional Chinese database translations.
This change follows that existing behavior and does not alter language resolution.

## Evidence and limits

The tables below carry forward the translation audit completed on 2026-09-29,
using the Taren-operated Chinese MU website. They are name references, not evidence
for changing game mechanics. Numbered variants retain OpenMU's distinctions.
Entries labeled **Derived** adapt an official base name to an OpenMU variant;
they are not claimed to be verbatim official names. Format placeholders are preserved.

Provisional monster translations and entries supported only by an existing local
translation are excluded. Merchant NPCs 376, 377 and 545 are excluded because their
personal names lack official evidence in the audit. Maps 5, 6, 40 and 62 are also
excluded. Exclusion leaves existing upstream data untouched; missing Chinese falls
back to the neutral name. This is intentionally partial coverage.

## Character classes

Sources: [class advancement](https://mu.zhaouc.com/Guide/GameSystem/08_quest.html)
and [master classes](https://mu.zhaouc.com/Guide/GameSystem/09_master.html).

| Number | Neutral name | Chinese name |
| --- | --- | --- |
| 0 | Dark Wizard | 魔法师 |
| 2 | Soul Master | 魔导师 |
| 3 | Grand Master | 神导师 |
| 4 | Dark Knight | 剑士 |
| 6 | Blade Knight | 骑士 |
| 7 | Blade Master | 神骑士 |
| 8 | Fairy Elf | 弓箭手 |
| 10 | Muse Elf | 圣射手 |
| 11 | High Elf | 神射手 |
| 12 | Magic Gladiator | 魔剑士 |
| 13 | Duel Master | 剑圣 |
| 16 | Dark Lord | 圣导师 |
| 17 | Lord Emperor | 祭祀 |
| 20 | Summoner | 召唤术师 |
| 22 | Bloody Summoner | 召唤导师 |
| 23 | Dimension Master | 召唤巫师 |
| 24 | Rage Fighter | 格斗家 |
| 25 | Fist Master | 格斗大师 |

## Maps

| Number | Neutral name | Chinese name | Evidence |
| --- | --- | --- | --- |
| 0 | Lorencia | 勇者大陆 | [Official guide][source-1] |
| 1 | Dungeon | 地下城 | [Official guide][source-2] |
| 2 | Devias | 冰风谷 | [Official guide][source-3] |
| 3 | Noria | 仙踪林 | [Official guide][source-4] |
| 4 | Lost Tower | 失落之塔 | [Official guide][source-5] |
| 7 | Atlans | 亚特兰蒂斯 | [Official guide][source-6] |
| 8 | Tarkan | 死亡沙漠 | [Official guide][source-7] |
| 9 | Devil Square 1 | 恶魔广场 1 | [Official guide][source-8] |
| 9 | Devil Square 2 | 恶魔广场 2 | [Official guide][source-8] |
| 9 | Devil Square 3 | 恶魔广场 3 | [Official guide][source-8] |
| 9 | Devil Square 4 | 恶魔广场 4 | [Official guide][source-8] |
| 10 | Icarus | 天空之城 | [Official guide][source-9] |
| 11 | Blood Castle 1 | 血色城堡 1 | [Official guide][source-10] |
| 12 | Blood Castle 2 | 血色城堡 2 | [Official guide][source-10] |
| 13 | Blood Castle 3 | 血色城堡 3 | [Official guide][source-10] |
| 14 | Blood Castle 4 | 血色城堡 4 | [Official guide][source-10] |
| 15 | Blood Castle 5 | 血色城堡 5 | [Official guide][source-10] |
| 16 | Blood Castle 6 | 血色城堡 6 | [Official guide][source-10] |
| 17 | Blood Castle 7 | 血色城堡 7 | [Official guide][source-10] |
| 18 | Chaos Castle 1 | 赤色要塞 1 | [Official guide][source-11] |
| 19 | Chaos Castle 2 | 赤色要塞 2 | [Official guide][source-11] |
| 20 | Chaos Castle 3 | 赤色要塞 3 | [Official guide][source-11] |
| 21 | Chaos Castle 4 | 赤色要塞 4 | [Official guide][source-11] |
| 22 | Chaos Castle 5 | 赤色要塞 5 | [Official guide][source-11] |
| 23 | Chaos Castle 6 | 赤色要塞 6 | [Official guide][source-11] |
| 24 | Kalima 1 | 卡利玛神庙 1 | [Official guide][source-12] |
| 25 | Kalima 2 | 卡利玛神庙 2 | [Official guide][source-12] |
| 26 | Kalima 3 | 卡利玛神庙 3 | [Official guide][source-12] |
| 27 | Kalima 4 | 卡利玛神庙 4 | [Official guide][source-12] |
| 28 | Kalima 5 | 卡利玛神庙 5 | [Official guide][source-12] |
| 29 | Kalima 6 | 卡利玛神庙 6 | [Official guide][source-12] |
| 30 | Valley of Loren | 罗兰峡谷 | [Official guide][source-13] |
| 31 | Land_of_Trials | 魔炼之地 | [Official guide][source-14] |
| 32 | Devil Square 5 | 恶魔广场 5 | [Official guide][source-8] |
| 32 | Devil Square 6 | 恶魔广场 6 | [Official guide][source-8] |
| 32 | Devil Square 7 | 恶魔广场 7 | [Official guide][source-8] |
| 33 | Aida | 幽暗森林 | [Official guide][source-15] |
| 34 | Crywolf Fortress | 狼魂要塞 | [Official guide][source-16] |
| 36 | Kalima 7 | 卡利玛神庙 7 | [Official guide][source-12] |
| 37 | Kanturu_I | 坎特鲁废墟 | [Official guide][source-17] |
| 38 | Kanturu_III | 坎特鲁遗址 | [Official guide][source-18] |
| 39 | Kanturu Event | 提炼之塔 | [Official guide][source-18] |
| 41 | Barracks of Balgass | 巴卡斯兵营 | [Official guide][source-19] |
| 42 | Balgass Refuge | 巴卡斯休息室 | [Official guide][source-19] |
| 45 | Illusion Temple 1 | 幻影寺院 1 | [Official guide][source-20] |
| 46 | Illusion Temple 2 | 幻影寺院 2 | [Official guide][source-20] |
| 47 | Illusion Temple 3 | 幻影寺院 3 | [Official guide][source-20] |
| 48 | Illusion Temple 4 | 幻影寺院 4 | [Official guide][source-20] |
| 49 | Illusion Temple 5 | 幻影寺院 5 | [Official guide][source-20] |
| 50 | Illusion Temple 6 | 幻影寺院 6 | [Official guide][source-20] |
| 51 | Elvenland | 幻术园 | [Official guide][source-21] |
| 52 | Blood Castle 8 | 血色城堡 8 | [Official guide][source-10] |
| 53 | Chaos Castle 7 | 赤色要塞 7 | [Official guide][source-11] |
| 56 | Swamp Of Calmness | 安宁池 | [Official guide][source-22] |
| 57 | LaCleon | 冰霜之城 | [Official guide][source-23] |
| 58 | LaCleon Boss | 孵化魔地 | [Official guide][source-23] |
| 63 | Vulcanus | 囚禁之岛 | [Official guide][source-24] |
| 64 | Duel Arena | 角斗场 | [Official guide][source-24] |
| 65 | Doppelgaenger 1 | 生魂广场 1 | [Official guide][source-25] |
| 66 | Doppelgaenger 2 | 生魂广场 2 | [Official guide][source-25] |
| 67 | Doppelgaenger 3 | 生魂广场 3 | [Official guide][source-25] |
| 68 | Doppelgaenger 4 | 生魂广场 4 | [Official guide][source-25] |
| 69 | Fortress of Imperial Guardian 1 | 帝国要塞 1 | [Official guide][source-26] |
| 70 | Fortress of Imperial Guardian 2 | 帝国要塞 2 | [Official guide][source-26] |
| 71 | Fortress of Imperial Guardian 3 | 帝国要塞 3 | [Official guide][source-26] |
| 72 | Fortress of Imperial Guardian 4 | 帝国要塞 4 | [Official guide][source-26] |
| 79 | LorenMarket | 罗兰市集 | [Official guide][source-27] |
| 80 | Karutan 1 | 卡伦特 1 | [Official guide][source-28] |
| 81 | Karutan 2 | 卡伦特 2 | [Official guide][source-28] |

## Merchants

Official references:

- [A: NPC guide](https://mu.zhaouc.com/Guide/npc/NPC.html)
- [B: Devias](https://mu.zhaouc.com/Guide/GameIntro/02_area04.html)
- [C: Kalima](https://mu.zhaouc.com/Guide/GameIntro/02_area10.html)
- [D: Valley of Loren](https://mu.zhaouc.com/Guide/GameIntro/02_area11.html)
- [E: Elbeland](https://mu.zhaouc.com/Guide/GameIntro/02_area03.html)
- [F: Karutan](https://mu.zhaouc.com/Guide/GameIntro/02_area19.html)

| NPC | Neutral name | Chinese name | Evidence |
| --- | --- | --- | --- |
| 230 | Alex | 流浪商人阿莱斯 | A |
| 231 | Thompson the Merchant | 武器商人托姆绅 | A |
| 242 | Elf Lala | 精灵安吉拉 | A |
| 243 | Eo the Craftsman | 工匠尤达 | A |
| 244 | Caren the Barmaid | 老板娘莉娜 | B |
| 245 | Izabel The Wizard | 魔导师露茜 | A |
| 246 | Zienna The Weapons Merchant | 武器商人苏菲 | A |
| 248 | Wandering Merchant Martin | 流浪商人马丁 | A |
| 250 | Wandering Merchant Harold | 流浪商人海罗德 | A |
| 251 | Hanzo The Blacksmith | 铁匠汉斯 | A |
| 253 | Potion Girl Amy | 少女安娜 | A |
| 254 | Pasi The Mage | 魔导师帕希 | A |
| 255 | Lumen the Barmaid | 老板娘莉雅 | A |
| 259 | Oracle Layla | 雷拉 | C |
| 415 | Silvia | 塞尔维亚 | E |
| 416 | Rhea | 雷亚 | E |
| 417 | Marce | 摩尔塞 | E |
| 577 | Leina the General Goods Merchant | 蕾娜 | F |
| 578 | Weapons Merchant Bolo | 贝莱 | F |

## Monsters and other NPCs

| Number | Neutral name | Chinese name | Evidence |
| --- | --- | --- | --- |
| 0 | Bull Fighter | 牛怪 | [Official guide][source-1] |
| 1 | Hound | 猎犬怪 | [Official guide][source-1] |
| 2 | Budge Dragon | 幼龙 | [Official guide][source-1] |
| 3 | Spider | 蜘蛛 | [Official guide][source-1] |
| 4 | Elite Bull Fighter | 蛮牛怪 | [Official guide][source-1] |
| 5 | Hell Hound | 地狱猎犬怪 | [Official guide][source-2] |
| 6 | Lich | 黑巫师 | [Official guide][source-1] |
| 7 | Giant | 巨人 | [Official guide][source-1] |
| 8 | Poison Bull | 毒牛怪 | [Official guide][source-2] |
| 9 | Thunder Lich | 死灵巫师 | [Official guide][source-2] |
| 10 | Dark Knight | 暗黑骑士 | [Official guide][source-2] |
| 11 | Ghost | 幽灵 | [Official guide][source-2] |
| 12 | Larva | 毒虫 | [Official guide][source-2] |
| 13 | Hell Spider | 地狱蜘蛛 | [Official guide][source-2] |
| 14 | Skeleton Warrior | 骷髅兵 | [Official guide][source-1] |
| 15 | Skeleton Archer | 骷髅弓箭手 | [Official guide][source-2] |
| 16 | Elite Skeleton | 骷髅战士 | [Official guide][source-2] |
| 17 | Cyclops | 独眼巨人 | [Official guide][source-2] |
| 18 | Gorgon | 魔鬼戈登 | [Official guide][source-2] |
| 19 | Yeti | 雪人 | [Official guide][source-3] |
| 20 | Elite Yeti | 雪人王 | [Official guide][source-3] |
| 21 | Assassin | 暗杀者 | [Official guide][source-3] |
| 22 | Ice Monster | 寒冰魔 | [Official guide][source-3] |
| 23 | Hommerd | 蓝魔怪 | [Official guide][source-3] |
| 24 | Worm | 雪虫 | [Official guide][source-3] |
| 25 | Ice Queen | 冰后 | [Official guide][source-3] |
| 26 | Goblin | 小哥布林 | [Official guide][source-4] |
| 27 | Chain Scorpion | 勾尾蝎 | [Official guide][source-4] |
| 28 | Beetle Monster | 瓢虫怪 | [Official guide][source-4] |
| 29 | Hunter | 偷猎者 | [Official guide][source-4] |
| 30 | Forest Monster | 树妖 | [Official guide][source-4] |
| 31 | Agon | 亚昆 | [Official guide][source-4] |
| 32 | Stone Golem | 石巨人 | [Official guide][source-4] |
| 33 | Elite Goblin | 大哥布林 | [Official guide][source-4] |
| 34 | Cursed Wizard | 诅咒巫师 | [Official guide][source-5] |
| 35 | Death Gorgon | 死神戈登 | [Official guide][source-5] |
| 36 | Shadow | 鬼魅 | [Official guide][source-5] |
| 37 | Devil | 恶魔 | [Official guide][source-5] |
| 38 | Balrog | 魔王巴洛克 | [Official guide][source-5] |
| 39 | Poison Shadow | 剧毒鬼魅 | [Official guide][source-5] |
| 40 | Death Knight | 死神骑士 | [Official guide][source-5] |
| 41 | Death Cow | 牛魔王 | [Official guide][source-5] |
| 45 | Bahamut | 小巴哈姆特 | [Official guide][source-6] |
| 46 | Vepar | 死亡美人鱼 | [Official guide][source-6] |
| 47 | Valkyrie | 蓝翼海怪 | [Official guide][source-6] |
| 48 | Lizard King | 巫师王 | [Official guide][source-6] |
| 49 | Hydra | 海魔希特拉 | [Official guide][source-6] |
| 51 | Great Bahamut | 大巴哈姆特 | [Official guide][source-6] |
| 52 | Silver Valkyrie | 银弓海怪 | [Official guide][source-6] |
| 57 | Iron Wheel | 铁轮战士 | [Official guide][source-7] |
| 58 | Tantallos | 破坏骑士 | [Official guide][source-7] |
| 59 | Zaikan | 魔王扎坎 | [Official guide][source-7] |
| 60 | Bloody Wolf | 铁脊怪 | [Official guide][source-7] |
| 61 | Beam Knight | 黑炎魔 | [Official guide][source-7] |
| 62 | Mutant | 巨齿兽 | [Official guide][source-7] |
| 63 | Death Beam Knight | 炽炎魔 | [Official guide][source-7] |
| 69 | Alquamos | 阿卡摩斯 | [Official guide][source-9] |
| 70 | Queen Rainer | 风后 | [Official guide][source-9] |
| 71 | Mega Crust | 恶灵 | [Official guide][source-9] |
| 72 | Phantom Knight | 幻影骑士 | [Official guide][source-9] |
| 73 | Drakan | 蓝魔龙 | [Official guide][source-9] |
| 74 | Alpha Crust | 恶灵王 | [Official guide][source-9] |
| 75 | Great Drakan | 红魔龙 | [Official guide][source-9] |
| 76 | Dark Phoenix Shield | 天魔菲尼斯（护盾阶段） | [Derived: official guide][source-9] |
| 77 | Dark Phoenix | 天魔菲尼斯 | [Official guide][source-9] |
| 144 | Death Angel 1 | 死亡天使 1 | [Derived: official guide][source-12] |
| 145 | Death Centurion 1 | 狂武士 1 | [Derived: official guide][source-12] |
| 146 | Blood Soldier 1 | 龙虾守卫 1 | [Derived: official guide][source-12] |
| 147 | Aegis 1 | 阿卡斯 1 | [Derived: official guide][source-12] |
| 148 | Rogue Centurion 1 | 武士 1 | [Derived: official guide][source-12] |
| 149 | Necron 1 | 暗黑巫师 1 | [Derived: official guide][source-12] |
| 152 | Gate to Kalima 1 of {0} | {0}的卡利玛1入口 | [Derived: official guide][source-12] |
| 153 | Gate to Kalima 2 of {0} | {0}的卡利玛2入口 | [Derived: official guide][source-12] |
| 154 | Gate to Kalima 3 of {0} | {0}的卡利玛3入口 | [Derived: official guide][source-12] |
| 155 | Gate to Kalima 4 of {0} | {0}的卡利玛4入口 | [Derived: official guide][source-12] |
| 156 | Gate to Kalima 5 of {0} | {0}的卡利玛5入口 | [Derived: official guide][source-12] |
| 157 | Gate to Kalima 6 of {0} | {0}的卡利玛6入口 | [Derived: official guide][source-12] |
| 158 | Gate to Kalima 7 of {0} | {0}的卡利玛7入口 | [Derived: official guide][source-12] |
| 160 | Schriker 1 | 暗黑傀儡 1 | [Derived: official guide][source-12] |
| 161 | Illusion of Kundun 1 | 昆顿幻影 1 | [Derived: official guide][source-12] |
| 174 | Death Angel 2 | 死亡天使 2 | [Derived: official guide][source-12] |
| 175 | Death Centurion 2 | 狂武士 2 | [Derived: official guide][source-12] |
| 176 | Blood Soldier 2 | 龙虾守卫 2 | [Derived: official guide][source-12] |
| 177 | Aegis 2 | 阿卡斯 2 | [Derived: official guide][source-12] |
| 178 | Rogue Centurion 2 | 武士 2 | [Derived: official guide][source-12] |
| 179 | Necron 2 | 暗黑巫师 2 | [Derived: official guide][source-12] |
| 180 | Schriker 2 | 暗黑傀儡 2 | [Derived: official guide][source-12] |
| 181 | Illusion of Kundun 2 | 昆顿幻影 2 | [Derived: official guide][source-12] |
| 182 | Death Angel 3 | 死亡天使 3 | [Derived: official guide][source-12] |
| 183 | Death Centurion 3 | 狂武士 3 | [Derived: official guide][source-12] |
| 184 | Blood Soldier 3 | 龙虾守卫 3 | [Derived: official guide][source-12] |
| 185 | Aegis 3 | 阿卡斯 3 | [Derived: official guide][source-12] |
| 186 | Rogue Centurion 3 | 武士 3 | [Derived: official guide][source-12] |
| 187 | Necron 3 | 暗黑巫师 3 | [Derived: official guide][source-12] |
| 188 | Schriker 3 | 暗黑傀儡 3 | [Derived: official guide][source-12] |
| 189 | Illusion of Kundun 3 | 昆顿幻影 3 | [Derived: official guide][source-12] |
| 190 | Death Angel 4 | 死亡天使 4 | [Derived: official guide][source-12] |
| 191 | Death Centurion 4 | 狂武士 4 | [Derived: official guide][source-12] |
| 192 | Blood Soldier 4 | 龙虾守卫 4 | [Derived: official guide][source-12] |
| 193 | Aegis 4 | 阿卡斯 4 | [Derived: official guide][source-12] |
| 194 | Rogue Centurion 4 | 武士 4 | [Derived: official guide][source-12] |
| 195 | Necron 4 | 暗黑巫师 4 | [Derived: official guide][source-12] |
| 196 | Schriker 4 | 暗黑傀儡 4 | [Derived: official guide][source-12] |
| 197 | Illusion of Kundun 4 | 昆顿幻影 4 | [Derived: official guide][source-12] |
| 204 | Wolf Status | 神狼雕像 | [Official guide][source-29] |
| 205 | Wolf Altar1 | 神狼护台1 | [Derived: official guide][source-29] |
| 206 | Wolf Altar2 | 神狼护台2 | [Derived: official guide][source-29] |
| 207 | Wolf Altar3 | 神狼护台3 | [Derived: official guide][source-29] |
| 208 | Wolf Altar4 | 神狼护台4 | [Derived: official guide][source-29] |
| 209 | Wolf Altar5 | 神狼护台5 | [Derived: official guide][source-29] |
| 223 | Senior | 长老 | [Official guide][source-30] |
| 224 | Guardsman | 攻城卫兵 | [Official guide][source-13] |
| 226 | Pet Trainer | 驯兽师 | [Official guide][source-30] |
| 229 | Marlon | 警卫队长摩伦 | [Official guide][source-30] |
| 233 | Messenger of Arch. | 大天使的使者 | [Official guide][source-30] |
| 235 | Sevina the Priestess | 圣导士塞维娜 | [Official guide][source-30] |
| 237 | Charon | 卡隆 | [Official guide][source-30] |
| 238 | Chaos Goblin | 玛雅哥布林 | [Official guide][source-30] |
| 240 | Baz The Vault Keeper | 仓库使者赛佛特 | [Official guide][source-1] |
| 241 | Guild Master | 战盟使者罗兰斯 | [Official guide][source-30] |
| 256 | Lahap | 赛尔维斯 | [Official guide][source-21] |
| 257 | Elf Soldier | 幻影导师 | [Official guide][source-30] |
| 260 | Death Angel 5 | 死亡天使 5 | [Derived: official guide][source-12] |
| 261 | Death Centurion 5 | 狂武士 5 | [Derived: official guide][source-12] |
| 262 | Blood Soldier 5 | 龙虾守卫 5 | [Derived: official guide][source-12] |
| 263 | Aegis 5 | 阿卡斯 5 | [Derived: official guide][source-12] |
| 264 | Rogue Centurion 5 | 武士 5 | [Derived: official guide][source-12] |
| 265 | Necron 5 | 暗黑巫师 5 | [Derived: official guide][source-12] |
| 266 | Schriker 5 | 暗黑傀儡 5 | [Derived: official guide][source-12] |
| 267 | Illusion of Kundun 5 | 昆顿幻影 5 | [Derived: official guide][source-12] |
| 268 | Death Angel 6 | 死亡天使 6 | [Derived: official guide][source-12] |
| 269 | Death Centurion 6 | 狂武士 6 | [Derived: official guide][source-12] |
| 270 | Blood Soldier 6 | 龙虾守卫 6 | [Derived: official guide][source-12] |
| 271 | Aegis 6 | 阿卡斯 6 | [Derived: official guide][source-12] |
| 272 | Rogue Centurion 6 | 武士 6 | [Derived: official guide][source-12] |
| 273 | Necron 6 | 暗黑巫师 6 | [Derived: official guide][source-12] |
| 274 | Schriker 6 | 暗黑傀儡 6 | [Derived: official guide][source-12] |
| 275 | Illusion of Kundun 7 | 昆顿幻影 7 | [Derived: official guide][source-12] |
| 277 | Castle Gate1 | 城门1 | [Derived: official guide][source-31] |
| 278 | Life Stone | 生命之石 | [Official guide][source-31] |
| 283 | Guardian Statue | 守护石像 | [Official guide][source-31] |
| 288 | Canon Tower | 巨弩发射器 | [Official guide][source-16] |
| 290 | Lizard Warrior | 冷血变异者 | [Official guide][source-14] |
| 291 | Fire Golem | 熔岩巨魔 | [Official guide][source-14] |
| 292 | Queen Bee | 嗜血蜂后 | [Official guide][source-14] |
| 293 | Poison Golem | 毒巨魔 | [Official guide][source-14] |
| 294 | Axe Warrior | 巨斧战士 | [Official guide][source-14] |
| 295 | Erohim | 炼狱魔王 | [Official guide][source-14] |
| 304 | Witch Queen | 丛林女巫师 | [Official guide][source-15] |
| 305 | Blue Golem | 丛林残暴者 | [Official guide][source-15] |
| 306 | Death Rider | 丛林暗杀者 | [Official guide][source-15] |
| 307 | Forest Orc | 丛林生命体 | [Official guide][source-15] |
| 308 | Death Tree | 丛林树精灵 | [Official guide][source-15] |
| 309 | Hell Maine | 丛林召唤者 | [Official guide][source-15] |
| 310 | Hammer Scout | 刺锤侦察兵 | [Official guide][source-16] |
| 311 | Lance Scout | 长矛侦察兵 | [Official guide][source-16] |
| 312 | Bow Scout | 魔弓侦察兵 | [Official guide][source-16] |
| 313 | Werewolf | 暗黑血狼人 | [Official guide][source-16] |
| 314 | Scout(Hero) | 英雄侦察兵 | [Official guide][source-16] |
| 315 | Werewolf(Hero) | 英雄血狼人 | [Official guide][source-16] |
| 316 | Balram | 暗黑防御者 | [Official guide][source-16] |
| 317 | Soram | 暗黑扫荡者 | [Official guide][source-16] |
| 331 | Aegis 7 | 阿卡斯 7 | [Derived: official guide][source-12] |
| 332 | Rogue Centurion 7 | 武士 7 | [Derived: official guide][source-12] |
| 333 | Blood Soldier 7 | 龙虾守卫 7 | [Derived: official guide][source-12] |
| 334 | Death Angel 7 | 死亡天使 7 | [Derived: official guide][source-12] |
| 335 | Necron 7 | 暗黑巫师 7 | [Derived: official guide][source-12] |
| 336 | Death Centurion 7 | 狂武士 7 | [Derived: official guide][source-12] |
| 337 | Schriker 7 | 暗黑傀儡 7 | [Derived: official guide][source-12] |
| 338 | Illusion of Kundun 6 | 昆顿幻影 6 | [Derived: official guide][source-12] |
| 350 | Berserker | 疯魔 | [Official guide][source-32] |
| 351 | Splinter Wolf | 裂角狼 | [Official guide][source-32] |
| 352 | Iron Rider | 金甲兽 | [Official guide][source-32] |
| 353 | Satyros | 半兽人 | [Official guide][source-32] |
| 354 | Blade Hunter | 刀刃猎手 | [Official guide][source-32] |
| 355 | Kentauros | 寒冰刺客 | [Official guide][source-32] |
| 356 | Gigantis | 狂巨人 | [Official guide][source-32] |
| 357 | Genocider | 屠杀者 | [Official guide][source-32] |
| 358 | Persona | 假面巫 | [Official guide][source-18] |
| 359 | Twin Tale | 毒步妖 | [Official guide][source-18] |
| 360 | Dreadfear | 恐惧天使 | [Official guide][source-18] |
| 361 | Nightmare | 咒怨魔王 | [Official guide][source-18] |
| 362 | Maya (Hand Left) | 玛雅左手 | [Official guide][source-18] |
| 363 | Maya (Hand Right) | 玛雅右手 | [Official guide][source-18] |
| 364 | Maya | 玛雅 | [Official guide][source-18] |
| 367 | Gateway Machine | 传送台 | [Official guide][source-30] |
| 368 | Elphis | 艾尔菲丝 | [Official guide][source-30] |
| 369 | Osbourne | 奥斯本 | [Official guide][source-30] |
| 370 | Jerridon | 杰瑞敦 | [Official guide][source-30] |
| 375 | Chaos Card Master | 玛雅使者 | [Official guide][source-1] |
| 385 | Mirage | 弥拉邱 | [Official guide][source-21] |
| 406 | Priest Devin | 弟子黛彬 | [Official guide][source-16] |
| 407 | Werewolf Quarrel | 暗黑血狼人扩雷 | [Official guide][source-16] |
| 409 | Balram (Trainee Soldier) | 暗黑防御者（训练兵） | [Derived: official guide][source-16] |
| 410 | Death Spirit (Trainee Soldier) | 暗黑咒术师（训练兵） | [Derived: official guide][source-16] |
| 411 | Soram (Trainee Soldier) | 暗黑扫荡者（训练兵） | [Derived: official guide][source-16] |
| 412 | Dark Elf (Trainee Soldier) | 暗黑指挥官（训练兵） | [Derived: official guide][source-16] |
| 418 | Strange Rabbit | 怪异的兔子 | [Official guide][source-21] |
| 419 | Polluted Butterfly | 污染之蝶 | [Official guide][source-21] |
| 420 | Hideous Rabbit | 疯狂的兔子 | [Official guide][source-21] |
| 421 | Werewolf | 狼人 | [Official guide][source-21] |
| 422 | Cursed Lich | 诅咒巫师 | [Official guide][source-21] |
| 423 | Totem Golem | 图腾树人 | [Official guide][source-21] |
| 424 | Grizzly | 灰熊 | [Official guide][source-21] |
| 425 | Captain Grizzly | 残暴的灰熊 | [Official guide][source-21] |
| 434 | Gigantis | 狂巨人 | [Derived: official guide][source-32] |
| 435 | Berserk | 疯魔 | [Derived: official guide][source-32] |
| 436 | Balram (Trainee) | 暗黑防御者（训练兵） | [Derived: official guide][source-16] |
| 437 | Soram (Trainee) | 暗黑扫荡者（训练兵） | [Derived: official guide][source-16] |
| 438 | Persona | 假面巫 | [Derived: official guide][source-18] |
| 439 | Dreadfear | 恐惧天使 | [Derived: official guide][source-18] |
| 440 | Dark_Elf | 暗黑指挥官 | [Derived: official guide][source-16] |
| 441 | Sapi-Unus | 树妖 | [Official guide][source-22] |
| 442 | Sapi-Duo | 毒树妖 | [Official guide][source-22] |
| 443 | Sapi-Tres | 残忍的树妖 | [Official guide][source-22] |
| 444 | Shadow Pawn | 暗影爪牙 | [Official guide][source-22] |
| 445 | Shadow Knight | 暗影骑士 | [Official guide][source-22] |
| 446 | Shadow Look | 暗影武士 | [Official guide][source-22] |
| 447 | Thunder Napin | 闪电巨人 | [Official guide][source-22] |
| 448 | Ghost Napin | 幽灵巨人 | [Official guide][source-22] |
| 449 | Blaze Napin | 寒冰巨人 | [Official guide][source-22] |
| 452 | Seed Master | 萤之石管理员 | [Official guide][source-33] |
| 453 | Seed Researcher | 萤之石研究员 | [Derived: official guide][source-21] |
| 454 | Ice Walker | 冰之魔犬 | [Official guide][source-23] |
| 455 | Giant Mammoth | 冰之魔象 | [Official guide][source-23] |
| 456 | Ice Giant | 冰之巨魔 | [Official guide][source-23] |
| 457 | Coolutin | 冰之魔蛛 | [Official guide][source-23] |
| 458 | Iron Knight | 冰铁剑魂 | [Official guide][source-23] |
| 459 | Selupan | 冰霜巨蛛 | [Official guide][source-23] |
| 480 | Zombie Fighter | 幽灵斗士 | [Official guide][source-24] |
| 481 | Zombie Fighter | 幽灵斗士 | [Official guide][source-24] |
| 482 | Resurrected Gladiator | 幽灵角斗士 | [Official guide][source-24] |
| 483 | Resurrected Gladiator | 幽灵角斗士 | [Official guide][source-24] |
| 484 | Ash Slaughterer | 幽灵屠杀者 | [Official guide][source-24] |
| 485 | Ash Slaughterer | 幽灵屠杀者 | [Official guide][source-24] |
| 486 | Blood Assassin | 幽灵暗杀者 | [Official guide][source-24] |
| 487 | Cruel Blood Assassin | 幽灵残酷暗杀者 | [Official guide][source-24] |
| 488 | Cruel Blood Assassin | 幽灵残酷暗杀者 | [Official guide][source-24] |
| 489 | Burning Lava Giant | 幽灵巨人 | [Official guide][source-24] |
| 490 | Ruthless Lava Giant | 幽灵残酷巨人 | [Official guide][source-24] |
| 491 | Ruthless Lava Giant | 幽灵残酷巨人 | [Official guide][source-24] |
| 492 | Moss The Merchant | 摩斯 | [Official guide][source-34] |
| 522 | Adviser Jerinteu | 辅佐官杰林特 | [Official guide][source-26] |
| 529 | Terrible Butcher | 愤怒的屠夫 | [Official guide][source-25] |
| 530 | Mad Butcher | 屠夫 | [Official guide][source-25] |
| 531 | Ice Walker | 冰之魔犬 | [Official guide][source-25] |
| 532 | Larva | 毒虫（生魂广场） | [Derived: official guide][source-2] |
| 534 | Doppelganger Elf | 生魂弓箭手 | [Derived: official guide][source-25] |
| 535 | Doppelganger Knight | 生魂剑士 | [Official guide][source-25] |
| 536 | Doppelganger Wizard | 生魂魔法师 | [Official guide][source-25] |
| 537 | Doppelganger Magic Gladiator | 生魂魔剑士 | [Official guide][source-25] |
| 538 | Doppelganger Dark Lord | 生魂圣导师 | [Official guide][source-25] |
| 539 | Doppelganger Summoner | 生魂召唤术师 | [Official guide][source-25] |
| 540 | Lugard | 陆迦德 | [Official guide][source-25] |
| 549 | Bloody Orc | 血腥丛林生命体 | [Derived: official guide][source-15] |
| 550 | Bloody Death Rider | 血腥丛林暗杀者 | [Derived: official guide][source-15] |
| 551 | Bloody Golem | 血腥丛林残暴者 | [Derived: official guide][source-15] |
| 552 | Bloody Witch Queen | 血腥丛林女巫师 | [Derived: official guide][source-15] |
| 553 | Berserker Warrior | 疯魔战士 | [Derived: official guide][source-32] |
| 554 | Kentauros Warrior | 寒冰刺客战士 | [Derived: official guide][source-32] |
| 555 | Gigantis Warrior | 狂巨人战士 | [Derived: official guide][source-32] |
| 556 | Genocider Warrior | 屠杀者战士 | [Derived: official guide][source-32] |
| 557 | Sapi Queen | 树妖女王 | [Derived: official guide][source-22] |
| 562 | Dark Mammoth | 黑暗冰之魔象 | [Derived: official guide][source-23] |
| 563 | Dark Giant | 黑暗冰之巨魔 | [Derived: official guide][source-23] |
| 564 | Dark Coolutin | 黑暗冰之魔蛛 | [Derived: official guide][source-23] |
| 565 | Dark Iron Knight | 黑暗冰铁剑魂 | [Derived: official guide][source-23] |
| 569 | Venomous Chain Scorpion | 剧毒勾尾蝎 | [Official guide][source-28] |
| 570 | Bone Scorpion | 白骨勾尾蝎 | [Official guide][source-28] |
| 571 | Orcus | 黑暗独角猿 | [Official guide][source-28] |
| 572 | Gollock | 黑暗四臂猿 | [Official guide][source-28] |
| 573 | Crypta | 金甲勇士 | [Official guide][source-28] |
| 574 | Crypos | 黄蜂女王 | [Official guide][source-28] |
| 575 | Condra | 幽灵石巨人 | [Official guide][source-28] |
| 576 | Narcondra | 邪灵石巨人 | [Official guide][source-28] |
| 579 | David | 戴彼得 | [Official guide][source-35] |

[source-1]: https://mu.zhaouc.com/Guide/GameIntro/02_area01.html
[source-2]: https://mu.zhaouc.com/Guide/GameIntro/02_area05.html
[source-3]: https://mu.zhaouc.com/Guide/GameIntro/02_area04.html
[source-4]: https://mu.zhaouc.com/Guide/GameIntro/02_area02.html
[source-5]: https://mu.zhaouc.com/Guide/GameIntro/02_area06.html
[source-6]: https://mu.zhaouc.com/Guide/GameIntro/02_area07.html
[source-7]: https://mu.zhaouc.com/Guide/GameIntro/02_area08.html
[source-8]: https://mu.zhaouc.com/Guide/GameFeature/01_feature.html
[source-9]: https://mu.zhaouc.com/Guide/GameIntro/02_area09.html
[source-10]: https://mu.zhaouc.com/Guide/GameFeature/03_feature.html
[source-11]: https://mu.zhaouc.com/Guide/GameFeature/02_feature.html
[source-12]: https://mu.zhaouc.com/Guide/GameIntro/02_area10.html
[source-13]: https://mu.zhaouc.com/Guide/GameIntro/02_area11.html
[source-14]: https://mu.zhaouc.com/Guide/GameIntro/02_area12.html
[source-15]: https://mu.zhaouc.com/Guide/GameIntro/02_area13.html
[source-16]: https://mu.zhaouc.com/Guide/GameIntro/02_area14.html
[source-17]: https://mu.zhaouc.com/news/Notice/12776.html
[source-18]: https://mu.zhaouc.com/Guide/GameIntro/02_area16.html
[source-19]: https://mu.zhaouc.com/Guide/GameSystem/08_quest.html
[source-20]: https://mu.zhaouc.com/Guide/GameFeature/07_feature.html
[source-21]: https://mu.zhaouc.com/Guide/GameIntro/02_area03.html
[source-22]: https://mu.zhaouc.com/Guide/GameIntro/02_area17.html
[source-23]: https://mu.zhaouc.com/Guide/GameIntro/02_area18.html
[source-24]: https://mu.zhaouc.com/01_news/updatecn/S5/S5_1.htm
[source-25]: https://mu.zhaouc.com/Guide/GameFeature/08_feature.html
[source-26]: https://mu.zhaouc.com/News/Update/151125_event/index3.html
[source-27]: https://mu.zhaouc.com/news/Update/225.html
[source-28]: https://mu.zhaouc.com/Guide/GameIntro/02_area19.html
[source-29]: https://mu.zhaouc.com/news/Update/203.html
[source-30]: https://mu.zhaouc.com/Guide/npc/NPC.html
[source-31]: https://mu.zhaouc.com/Guide/GameFeature/06_feature.html
[source-32]: https://mu.zhaouc.com/Guide/GameIntro/02_area15.html
[source-33]: https://mu.zhaouc.com/Guide/ItemSystem/02_itemSys.html
[source-34]: https://mu.zhaouc.com/01_news/updatecn/s5/s5_4.htm
[source-35]: https://mu.zhaouc.com/news/Update/214.html
