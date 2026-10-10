---
title: Configuration captions
sidebar_position: 4
description: Keep the translated names of the game configuration in sync with the translations which ship with OpenMU.
---

# Configuration captions

**Navigation:** *Captions* — route `/config-captions`

The names of built-in configuration objects, e.g. maps, monsters and character
classes, can contain translations for several languages. New OpenMU versions can
ship additional languages or corrected translations. This page compares the
captions of your configuration with these built-in sources and lets you decide
which differences to apply. No configuration update is required for that.

The page guides you through two steps:

1. If none of your captions is linked to its source yet (see
   [Link built-in captions](#link-built-in-captions)), the page only offers to link them.
2. Afterwards it lists the differences, which you can review and apply. If there
   are no differences, it shows *All available localizations are in place*.

When an update of OpenMU adds sources for captions which weren't linked before
(e.g. the item names), the page shows *Link new built-in captions* with the
number of these captions by type, so they can be linked as well. See
[Link built-in captions](#link-built-in-captions).

## Kinds of changes

| Kind | Meaning | Selected by default |
| --- | --- | --- |
| New | A translation is available, but missing in your configuration (or it's just a copy of the English text). | Yes |
| Updated | The built-in text was changed and your text is still the one which was originally taken over. | Yes |
| Removed | The built-in translation doesn't exist anymore and your text is still the one which was originally taken over. | Yes |
| Customized | Your text differs from the built-in one, because it was changed (e.g. in the admin panel) or its origin is unknown. | No |

You can filter the list by language and kind, select or deselect single changes
and apply the selection. Customized texts are only overwritten if you select
them explicitly. Like configuration updates, the changes require a restart of
the server process to take effect.

## Link built-in captions

To know where a caption comes from, OpenMU stores a source reference with it
(for example `MapNames/Lorencia`). Configurations which were created with an
older version don't have these references yet, so they don't show any changes.

*Link built-in captions* executes the data initialization of your configuration
in memory and adds the references to the matching captions of your
configuration. It doesn't change any text. Captions whose English text was
customized are skipped. This is only required once; afterwards the page shows
the available differences. *Link captions again* (at the bottom of the page) links
captions which aren't linked yet, e.g. after configuration updates added new objects.

When you open the page, it checks in the background if linking would link
captions which aren't linked yet. That's the case when a newer version of OpenMU
added sources, e.g. for the item names. Then it shows *Link new built-in captions*
at the top of the page, so you don't need to remember to link again. The first
check after a server start takes a while, because it executes the data
initialization in memory. The built-in captions which it determines are kept until
the server process is restarted, so later checks and linking are quick.

New source-backed captions also include the seven Imperial Guardian day names
and descriptions, Fenrir material drop-group descriptions, and the four Selupan
skill names (250–253). On an existing database, use *Link captions again*, review
the Chinese translations, and apply the recommended changes. Restart the server
process afterwards. Linking alone does not apply translations.

The Chinese Fenrir material labels reuse the item-name resources. The Selupan
skill labels are descriptive translations of the internal English skill names;
they are not asserted to be official Chinese client skill names.

Localized name editors show full culture codes, such as `zh-CN`, for each input.
Regional translations have separate fields and validation messages, even when
they share a language. Each field edits only the translation for its exact culture.

## Item option and set captions

Built-in item option types, their guardian-option description, item option
names, ancient set names and ordinary full-armor bonus names include Simplified
Chinese (`zh-CN`) resources. Fresh configurations include these translations.
For an existing configuration, link the names again on this page, review the
Chinese changes and apply the recommended entries. Customized text remains
unselected unless you explicitly choose to replace it.

Ordinary armor sets are matched by armor number and minimum equipment level,
since their existing IDs are not deterministic. Older wing option definitions
without stable IDs can be linked by their exact neutral name when unique in both
configurations. Ambiguous matches are skipped.
The neutral English names are retained, including names shared by different
armor families; separate source keys provide the appropriate Chinese names.
The level in a name such as `Adamantine Defense Bonus (Level 10)` refers to
item enhancement (+10), not character level. Names do not change bonus values,
requirements or equipment membership.

Ancient set terminology follows the older names documented in the official
[Season 4.5 set guide](https://mu.zhaouc.com/01_news/updatecn/s4_5/s4_5_3.htm)
and [set introduction](https://mu.zhaouc.com/01_news/updatecn/newitem/newitem.htm),
rather than the replacements in the later
[Season X renaming notice](https://mu.zhaouc.com/news/Notice/1265.html).
Technical option and armor-bonus labels describe the server configuration;
they are not presented as verbatim client UI text.

Item option combination bonus descriptions (Fenrir movement bonuses and socket
package bonuses) also use built-in resources. For an existing configuration, link
the names and review the translations on this page before applying them. Custom
descriptions are preserved unless explicitly selected.

## Full configuration captions

The same workflow also covers crafting names, quest titles, skill and
magic-effect
names, master-skill branches, combo names, warp destinations, equipment slots,
item-level bonus table names and descriptions, drop-group descriptions, and
mini-game stages, waves and announcements. These fields are visible in
**Game Configuration → Full Configuration** and their nested editors. Existing
installations must link the new captions and apply their selected translations;
linking alone only records their resource keys. Restart the server process after
applying changes, as described above.

Some older drop groups, events and bonus tables have random IDs. Their captions
can be linked by object type, property and exact neutral text when the reference
has a single source for that text. Repeated descriptions can share that source;
conflicting sources are skipped. This links captions only: it does not merge
configuration objects or change their gameplay settings.

Chinese warp labels use the same place names as the map resources. Existing
Season 6 terminology is retained for items and activities. New quest titles and
technical descriptions are translations of OpenMU's English configuration text,
not claims of verbatim official client wording. The three legacy candy-box
labels are descriptive translations pending confirmation from a matching Season
6
client. The Chinese name `次元标识` for `Sign of Dimensions` was confirmed by a
contributor from their experience playing the official Chinese game. The
official
[family-system guide](https://mu.zhaouc.com/01_news/updatecn/s5/s5_3.htm) and
[Doppelganger guide](https://mu.zhaouc.com/Guide/GameFeature/08_feature.html)
provide terminology for the family and event plugin captions; their newer
mechanics are not copied into the Season 6 configuration.

Skill labels were also checked against the official
[Season 6 Rage Fighter introduction](https://mu.zhaouc.com/news/Update/212.html)
and the character guides for
[Dark Knight](https://mu.zhaouc.com/Guide/GameIntro/01_character01.html),
[Dark Wizard](https://mu.zhaouc.com/Guide/GameIntro/01_character02.html),
[Fairy Elf](https://mu.zhaouc.com/Guide/GameIntro/01_character03.html),
[Magic Gladiator](https://mu.zhaouc.com/Guide/GameIntro/01_character04.html),
[Dark Lord](https://mu.zhaouc.com/Guide/GameIntro/01_character05.html) and
[Summoner](https://mu.zhaouc.com/Guide/GameIntro/01_character06.html).
Only names of skills present in the existing configuration are used. Technical
master-skill variants retain descriptive suffixes rather than claiming a
verified
client label for every variant.
