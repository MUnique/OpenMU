# Item name localization

The initialization project's `Properties/ItemNames.resx` contains the neutral
names; `ItemNames.zh-CN.resx` contains Simplified Chinese translations for 683
item definitions, and `ItemNames.de.resx` German translations (see
[German names](#german-names)). Initializers use `LocalizedString.FromResource`, so names carry
stable `ItemNames/<resource key>` source keys. English names, item identifiers,
level-dependent name ordering, and gameplay settings are preserved.

Fresh configurations receive the available translations during initialization.
Existing installations use the admin panel's Configuration names page: link
names to their sources, review the differences, and apply selected translations.
Custom text remains subject to the existing caption workflow's selection rules.
This change does not introduce an update plugin or alter client-side item text.

## Translation evidence

Names were reviewed on 2026-10-04 against the Chinese operator's historical
[item dictionary](https://mu.zhaouc.com/Guide/M_Guide/S_Dictionary/DefaultUnit.html)
and related event and class pages. Dictionary entry indexes are not game item
numbers. Armor names retain the operator's spelling for each piece rather than
adding a uniform suffix. These sources span different releases; this is not a
claim of comparison against every entry in a current Chinese client.

Additional references include:

- [Magic Gladiator skills](https://mu.zhaouc.com/Guide/GameIntro/01_character04.html):
  `Orb of Fire Slash` (12/16) is 玄月斩之石, not 天雷闪之石. The latter conflates
  the orb with the separate weapon skill 天雷闪.
- [Christmas event](https://mu.zhaouc.com/event/2015sd/): Christmas box names.
- [Halloween event](https://mu.zhaouc.com/news/event/683.html): pumpkin consumables.
- [Illusion Temple](https://mu.zhaouc.com/Guide/GameFeature/07_feature.html):
  ticket materials.
- [Third-party Frost Mace entry](https://mu.dvg.cn/item_info.php?id=2423):
  2/16, 巨毒之刺.
- [Third-party Divine Stick entry](https://mu.dvg.cn/item_info.php?id=2544):
  5/36, 大天使绝对魔杖.
- [Third-party box list](https://mu.dvg.cn/v2_item_list.php?category=box&view=list)
  and [ring list](https://mu.dvg.cn/v2_item_list.php?category=ring&view=list):
  item-number cross-checks for event items.
- [Third-party Secromicon recipe](https://mu.dvg.cn/zhuanti.php?id=10675):
  希克拉碎片1–6 and 完整的希克拉, also confirmed by the contributor's experience
  playing the Chinese official game.

The contributor also confirmed 奇怪的小纸条 (14/101) and 凯文的指令 (14/102).
The [third-party paper entry](https://mu.dvg.cn/item_info.php?id=4335) spells the
former 奇怪的纸条; the translation retains the contributor-confirmed wording.
Third-party and player evidence is recorded separately from operator evidence.

## Provisional names

These 16 entries still lack a sufficiently reliable Chinese item-name or variant
match. Their translations are provisional, not asserted to be official names.
Generic definitions must not be renamed after a single level-dependent variant.

| Group/number | Neutral name | Provisional Chinese name |
| --- | --- | --- |
| 12/11 | Orb of Summoning | 召唤魔石 |
| 13/10 | Transformation Ring | 变身指环 |
| 13/19 | Weapon of Archangel | 大天使武器 |
| 13/31 | Spirit | 宠物灵魂 |
| 14/21 | Rena | 蕾娜 |
| 14/32 | Pink Chocolate Box | 粉色巧克力盒 |
| 14/33 | Red Chocolate Box | 红色巧克力盒 |
| 14/34 | Blue Chocolate Box | 蓝色巧克力盒 |
| 14/38 | Small Complex Potion | 小瓶复合药水 |
| 14/39 | Medium Complex Potion | 中瓶复合药水 |
| 14/40 | Large Complex Potion | 大瓶复合药水 |
| 14/63 | Firecracker | 烟花 |
| 14/85 | Cherry Blossom Wine | 樱花酒 |
| 14/86 | Cherry Blossom Rice Cake | 樱花年糕 |
| 14/87 | Cherry Blossom Flower Petal | 樱花花瓣 |
| 14/99 | Christmas Firecracker | 圣诞烟花 |

In particular, Rena must not be equated with a level-dependent castle siege
item solely from a database label. Modern cherry blossom consumable labels
with boost/enhancement prefixes need a version cross-check before adoption.

On existing installations, complete pending configuration updates before applying
translations. Some historical update plugins still compare full item-name strings;
this PR does not rewrite already-released update plugins.

The newer Doppelganger definitions (13/125, 14/110, 14/111) are outside this
reviewed resource set and retain their existing neutral names.

## German names

`ItemNames.de.resx` uses the same German item names as the open source game
client ([sven-n/MuMain](https://github.com/sven-n/MuMain), the `"de"` names in
`src/bin/Data/Items/*.json`), so players see the same names in server messages
and in the client. Names which are equal in German (e.g. *Kris*, *Zen*, *Rena*)
are left out and fall back to the neutral name.

Where the neutral name of an item differs from the client's English name for the
same group and number, the German name follows the client. For example, the
armor sets 39, 42, 43, 44, 50, 51 and 59–61 (*Mistery*, *Black Rose*, *Aura*,
*Lilium*, *Faith*, *Paewang*, *Sacred*, *Storm Hard*, *Piercing*) are called
*Violent Wind*, *Demonic*, *Storm Blitz*, *Eternal Wing*, *Divine*, *Royal*,
*Sacred Fire*, *Storm Jahad* and *Piercing Groove* in the client, and get the
German names of those (e.g. *Sturmwindhelm*). The sets 47 and 48 are an
exception: the neutral names *Destory* (47) and *Phantom* (48) are swapped
compared to the client (*Phantom* at 47, *Destroy* at 48). Each of these keeps the
translation of its own name (*Zerstörerrüstung*, *Phantom-Rüstung*).

Conventions:

- Armor pieces are *Helm*, *Rüstung*, *Hose*, *Handschuhe*, *Stiefel* and
  *Maske*. Set names are joined with the piece (*Drachenhelm*), proper names get
  a hyphen (*Hades-Helm*) and adjectives are declined (*Legendärer Helm*,
  *Legendäre Rüstung*). *Great* is translated as *großartig*
  (*Großartiger Drachenhelm*), *Grand* as *Groß-* (*Großseelenhelm*).
- Jewels use the client's terms (*Juwel des Segens*, *Juwel der Seele*).
- Skill names stay English, like in the client. Items which teach a skill name it
  after a colon: *Schriftrolle: Evil Spirit*, *Pergament: Chain Lightning*,
  *Kugel: Twisting Slash*, *Kristall: Multi-Shot*.
- Monster and event names stay English, like in the client
  (*Flamme des Death Beam Knight*, *Devil-Square-Ticket*), except *Blutburg* and
  *Illusionstempel*.

These set names are provisional and open for review: *Großseelen-* (18),
*Tapfer* (46), *Ewigschwingen-* (44), *Heiligfeuer-* (59), *Sturm-Jahad-* (60)
and *Durchbohrer-* (61).
