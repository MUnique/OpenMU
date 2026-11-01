---
title: Game configuration
sidebar_position: 7
description: Edit rates, items, monsters, maps, drops and everything else about the game world.
---

# Game configuration

**Navigation:** *Configuration* (drop-down menu)

Everything about the game world is stored in the database as configuration data,
and all of it can be edited here. The drop-down groups the most-used parts of the
configuration into their own pages; below that you always have the full
configuration tree as a fallback.

:::warning[Changes need a reload]
Most configuration data is loaded when a game server starts. After a change, use
**Reload configuration and restart all game servers** on the
[Servers page](servers.md), or restart the server process.
:::

## System

Route `edit-config/…SystemConfiguration/`

The system settings are **not** part of the game configuration — they apply to
the server process itself.

| Setting | Meaning |
|---|---|
| IP resolver | How the server determines the address it reports to the game client: `Auto`, `Public`, `Local`, `Loopback` or `Custom`. See [Connect a game client](../getting-started/game-client.md#disconnects-after-selecting-a-server). |
| IP resolver parameter | The fixed IP or host name, when the resolver is `Custom` |
| Auto start | Start the listeners of all servers when the process starts. Only applies to the all-in-one startup — distributed processes always start their listeners. |
| Auto update schema | Update the database schema, and apply the mandatory [configuration updates](configuration-updates.md#at-startup), automatically at startup. Only applies to the all-in-one startup; in a distributed deployment the updates are started manually on the [Setup page](setup.md) and the [Configuration updates page](configuration-updates.md). |
| Read console input | Whether the all-in-one process reacts to commands typed in its console |
| Time zone id | The time zone used to interpret the time-of-day entries of periodic event schedules (invasions, Blood Castle, Devil Square, …) and of the Castle Siege schedule. IANA ids such as `Europe/Warsaw` are recommended; when empty or unresolvable, UTC is used. |

These settings can also be given as
[start parameters or environment variables](../deployment/startup-parameters.md),
which take precedence over what is configured here.

## Game clients

Route `edit-config-grid/…GameClientDefinition/`

The client versions your server knows: version bytes, description, and the
protocol/serializer which is used for them. A connect server and the endpoints of
a game server each reference one of these definitions, which is how one server
can serve several client versions on different ports.

## General

Route `edit-config/…GameConfiguration/{id}/hide-collections`

The settings of the game configuration itself, without its (huge) collections.
This is where the values live which most server operators want to change:

* **Rates** — experience rate, master experience rate
* **Levels** — maximum level, maximum master level, minimum monster level for
  master experience
* **Money and items** — maximum inventory/vault money, whether monsters drop
  money, item drop duration, maximum item option level on drop, excellent item
  drop level delta
* **Characters** — maximum characters per account, character name regex,
  maximum password length, maximum party size
* **Formulas** — the experience formula and master experience formula, the
  excellent and ancient drop level bonuses of the
  [item formulas](#item-stat-profiles)
* **Misc** — info range, recovery interval, letter settings, durability
  consumption

## Monsters

Route `edit-config-grid/…MonsterDefinition/`

All monsters and NPCs with their attributes (health, defense, damage, movement
speed, attack range), the attribute set used for battle, their drops, their
intelligence (AI) and their merchant/quest role.

## Merchant stores

Route `merchants`

A more comfortable editor for NPC merchants: pick a merchant and edit the items
it offers with the graphical item storage editor instead of the generic form.

## Character classes

Route `edit-config-grid/…CharacterClass/`

Stat attributes and their growth per level, level requirements, the evolution
path from one class to the next, the home map, and the stat point gain per level.

## Skills

Route `edit-config-grid/…Skill/`

Damage, mana and ability cost, range, target restrictions, the classes which can
learn a skill, the master skill definitions and their dependencies.

## Items

Route `edit-config-grid/…ItemDefinition/`

Item definitions with their requirements, level tables, possible options, sockets
and the classes which may equip them. Items use a dedicated editor with a
graphical representation instead of the plain generic form.

### Item prices

The price of an item is used by the NPC merchants (buying, selling, repairing)
and by the success rate of some crafting recipes. By default, an item gets the
price of usual equipment: a base price from its drop level, increased by its
skill and options. Items with a different price reference a *price definition*
(the `PriceDefinition` of the item). The price definitions are in the full
configuration tree (`ItemPriceDefinitions` of the game configuration), and one
can be shared by several items.

| Setting | Meaning |
|---|---|
| Base price formula | The price of one unit, for example `9000000`, or `(level + 1) * 200000`. Without a formula, the equipment price is used. |
| Price per level | Fixed prices for specific item levels. They take precedence over the formula. |
| Quantity scaling | `PerPiece` multiplies the price by the number of pieces (potions), `ByFillRatio` by the fill ratio (arrows). |
| Modifiers | Which options increase the price, like for equipment: skill, luck, option, wing option, excellent. |
| Selling price rounding | `Tens` rounds the selling price to tens and ignores the durability, like for potions. |
| Crafting reference price | A different price for the success rate of crafting (e.g. the jewels). |

The formula can use the variables `level`, `durability`, `maxDurability`,
`value` (the value of the item), `dropLevel`, `optionLevel`, `optionCount`,
`healthRecoveryOptionLevel` and `automaticPrice` (the equipment price). The game
client calculates the prices it shows by itself, so changed prices only show up
correctly in clients which calculate them the same way.

### Item stat profiles

The item formulas decide how the stat requirements of an item grow with its drop
level, item level and option, and what excellent and ancient items get on top of
their base values. They are kept in named *stat profiles* (`ItemStatProfiles` of
the game configuration, in the full configuration tree), and each item names the
profile of its kind (the `StatProfile` of the item). The profile `default` holds
every formula. Another profile only holds what differs and takes the rest from
`default`: for example, `armor` only says that each option level adds 5 to the
strength requirement instead of 4. An item without a profile has the formulas of
`default`, and a change to a profile applies to every item which uses it. A new
item doesn't get a profile by itself: give it the profile of its kind, e.g.
`armor`, `shield` or `book`, or it lacks the formulas of that kind, like the
ancient defense of shields. A game server lists the items which can be worn but
have no profile in a warning when it starts.

| Setting | Meaning |
|---|---|
| Requirement formulas | One per stat requirement: `add + base × (drop level + item level × level weight) × percent ÷ 100`. A base of 0 stays 0. *Keep base* keeps the value of the item; *level requirement basis* takes the stored level requirement of the item (without its growth per item level and the excellent addition, as in the client) instead of the drop level and the item level. |
| Level per item level | What each item level adds to the level requirement. |
| Excellent level add | What an excellent item adds to the level requirement, unless *Excellent raises level* of the item is off. |
| Strength per option level | What each option level adds to the strength requirement. |
| Stat terms | One per stat: defense, defense rate and defense of shields, physical damage (the minimum, which also applies to the maximum), and the rises of staffs, sceptres and books. The excellent part and the ancient part are each `value × times ÷ drop level + add + drop level ÷ drop level divisor`; a divisor of 0 leaves out the last term. A part is *not written* (the part of `default` applies), *none* (the item gets nothing more) or has values. *After level bonus* lets the ancient part start from the value with the level bonus. |

For an excellent item, the formulas add the *excellent drop level bonus* (25) of
the game configuration to the drop level of its requirements, and for an ancient
item the *ancient drop level bonus* (30) to the drop level of its requirements
and of its ancient part. An item with an *Excellent bonus* gets this fixed value
on its damage and magic power instead of the excellent part.

The level bonus of these stats still comes from the level table of the item's
base power-up. The table which a profile names for a stat, and *per level after
halving* of the rises, aren't used by the game for now. Which kinds of items get
which part and the halving of the magic power stay part of the game logic.

A game server doesn't start without the profile `default` (see
[Configuration updates](configuration-updates.md)). The game client calculates
the values it shows by itself, so changed formulas only show up correctly in
clients which calculate them the same way.

## Item options, option types and item sets

The game configuration menu provides separate entries for these lists:

* **Item options** (`edit-config-grid/…ItemOptionDefinition/`): available item
  bonuses, their effects and level-dependent values.
* **Item option types** (`edit-config-grid/…ItemOptionType/`): categories such as
  excellent or ancient options.
* **Item set groups** (`edit-config-grid/…ItemSetGroup/`): item set names, member
  items and set bonuses.

Each entry opens the existing configuration grid, where entries can be inspected
and edited without navigating through the full configuration.

## Item drops

Route `edit-item-drops`

All drops on one page: the drop item groups of maps, monsters, quests and boxes,
the drop chances per monster, the randomly dropped items and the item option
chances. See [Item drops](item-drops.md).

## Drop item groups

Route `edit-config-grid/…DropItemGroup/`

Which items drop with which chance, as generic grid. Groups can be assigned to a
map or to a monster, which is how the drop tables are composed. The
[Item drops](item-drops.md) page is usually more comfortable.

:::tip[Changing drop rates]
The chance of a group is a value between 0 and 1. The chances of all groups which
apply to a monster are added up, and one of them is selected per drop — so
increasing one group's chance lowers the chance that nothing drops, and once the
sum exceeds 1, it lowers the share of all other groups. See
[How drops are determined](item-drops.md#how-drops-are-determined).
:::

## Game maps

Route `edit-config-grid/…GameMapDefinition/`

Maps with their number, name, terrain, safe zone, level requirements, monster
spawn areas, enter and exit gates, and the battle zone of event maps.

The spawn areas and gates are much easier to edit graphically — an edit page of a
map links to the [map editor](map-editor.md).

## Mini games

Route `edit-config-grid/…MiniGameDefinition/`

Blood Castle, Devil Square, Chaos Castle, Illusion Temple and the other events:
entry requirements, ticket items, the spawn waves, rewards and the schedule at
which they start. The time-of-day entries are interpreted in the time zone
configured under [System](#system).

## Warp list

Route `edit-config-grid/…WarpInfo/`

The entries of the in-game warp list: index, name, target gate, level requirement
and cost.

## Jewel mixes

Route `edit-config-grid/…JewelMix/`

The jewel stacking (mix/unmix) definitions.

## Content activation

Route `content-activation`

Maps, character classes, monsters, items and mini games can be deactivated
without deleting them from the configuration. This is how a server can be
restricted to the content of a certain season: deactivate everything which was
introduced later, and activate it again when you want to offer it.

Each entry knows in which version of the original game it was introduced, from
the early version numbers before Season 1 (like 0.34, 0.95k or 0.99G+, as listed
by the [MU Online history](https://github.com/Khdoop/mu-online-history)) up to
Season 6 Episode 3. Content of patches without a known version number counts
as introduced in the next version with a known number.
Select a version at the top and click **Restrict to game version** to activate
all content up to this version and to deactivate everything which was
introduced later — for example *Season 2* for a server without Summoners,
Raklion, socket items and the other later content. Entries with an unknown
version are left unchanged, so check them afterwards with the version column.
The page shows how many entries will be activated and deactivated, and changes
them after you confirm.

:::note[The versions are best effort]
The versions were compiled from the original patch notes and OpenMU's data of
the older versions. Some entries, like a few event items, are *Unknown*. You can
correct the version of an entry with *Introduced In* on its edit page.
:::

Pick the kind of content in the second row. The list can be narrowed down
by its active state and by a search text (which also matches the version),
and **Activate all shown** / **Deactivate all shown** change all entries of
the narrowed list at once — for example, search for `Kanturu` and deactivate
all shown entries. The changes are only stored after **Save changes** and the
confirmation; **Discard changes** drops them. Save or discard them before
restricting to a game version.

Both actions are recorded in the
[audit history](mcp.md#operational-safety-and-audit-history), and they are
the same operations which agents use through the
[MCP tools](mcp.md#configuration-and-offline-character-changes)
`content_restrict_to_version` and `content_set_active`. Like other configuration
changes, they require the *Administrator* role.

What an inactive entry means in the game:

| Content | Effect when inactive |
|---|---|
| Map | It can't be entered by warps or gates, no instance of it is created, and invasions skip it. A character which logs in on an inactive map is moved to the safezone map of it, or to the home map of its class. |
| Character class | No new character of this class can be created, and its creation isn't unlocked on the character selection screen. Quests which would evolve a character into it aren't offered and can't be completed. Existing characters stay playable. |
| Monster / NPC | It isn't spawned on the maps, also not by invasions. Monsters which are already spawned remain until the game server is restarted. |
| Item | Monsters and item boxes don't drop it, and merchants don't offer it. Existing items stay usable. |
| Mini game | It can't be entered, and its periodic start doesn't open it. |

Entries are active by default, also after updating an existing installation;
the configuration update *Set the game versions of the content* adds the versions
to an existing installation. The settings are also available as *Is Active* and
*Introduced In* on the edit page of each entry.

:::tip[Deactivate the plugins, too]
Invasions and the periodic starts of the mini games skip inactive content and
don't announce an event when nothing of it is active. Other features are
implemented as plugins without such content, for example the chat commands or
the custom gameplay plugins. Deactivate them on the [Plugins](plugins.md) page
if they don't fit the season of your server.
:::

## Plugins, chat commands, map editor

The lower part of the drop-down leads to [Content activation](#content-activation),
[Plugins](plugins.md), [Chat commands](chat-commands.md) and the
[Map editor](map-editor.md).

## Full configuration

Route `edit-config/…GameConfiguration/`

The complete game configuration as one editable tree, including all collections
which the *General* page hides. Everything that has no dedicated page can be
reached from here — item options, item sets, quests, attributes, magic effects,
crafting definitions, and so on.

## The generic edit pages

Most configuration pages are generated automatically from the data model by
reflection. There are two of them:

### The grid (`edit-config-grid/{type}`)

A sortable, searchable and paged list of all objects of one type, with these
actions per row:

* **Edit** — open the object in the form
* **Duplicate** — create a copy, which is the fastest way to add a variant of an
  existing monster or item
* **Delete** — remove the object

…and an **Add new** button at the bottom.

### The form (`edit-config/{type}/{id}`)

A form with one input per property of the object, grouped by category. Referenced
objects are shown as drop-downs, collections as editable sub-lists which lead
deeper into the tree. The breadcrumb at the top is how you find your way back.
The sub-lists of items, monsters, skills and maps additionally show their number
(and the group of an item) next to the name, so that entries with similar
names are easy to tell apart.

Some types have a specialized editor instead of (or in addition to) the generic
form — items, item storages and maps.

:::warning[It is a direct view of the data model]
The generic pages are a technical view of the data. They do not know the game
rules, so it is possible to create inconsistent or invalid configurations with
them. Some fields also can't be edited or created yet, because not every type has
a corresponding component.

Change one thing at a time and test it, and take a database backup before larger
edits.
:::

## Configuration search

The search box in the navigation area searches the whole configuration by name
and jumps directly to the edit page of a result. It is usually much faster than
walking the tree — if you know an item, monster or skill by name, search for it.
