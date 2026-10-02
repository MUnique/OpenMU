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
| Auto update schema | Update the database schema automatically at startup. Only applies to the all-in-one startup; in a distributed deployment the update is started manually on the [Setup page](setup.md). |
| Read console input | Whether the all-in-one process reacts to commands typed in its console |
| Time zone id | The time zone used to interpret the time-of-day entries of periodic event schedules (invasions, Blood Castle, Devil Square, …). IANA ids such as `Europe/Warsaw` are recommended; when empty or unresolvable, UTC is used. |

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
* **Formulas** — the experience formula and master experience formula
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

Pick the kind of content in the first drop-down. The list can be narrowed down
by its active state and by a search text, and **Activate all shown** /
**Deactivate all shown** change all entries of the narrowed list at once — for
example, search for `Kanturu` and deactivate all shown entries. The changes are
only stored after **Save changes**.

What an inactive entry means in the game:

| Content | Effect when inactive |
|---|---|
| Map | It can't be entered by warps or gates, and no instance of it is created. A character which logs in on an inactive map is moved to the safezone map of it, or to the home map of its class. |
| Character class | No new character of this class can be created, and its creation isn't unlocked on the character selection screen. Existing characters stay playable. |
| Monster / NPC | It isn't spawned on the maps. Monsters which are already spawned remain until the game server is restarted. |
| Item | Monsters and item boxes don't drop it, and merchants don't offer it. Existing items stay usable. |
| Mini game | It can't be entered, and its periodic start doesn't open it. |

Entries are active by default, also after updating an existing installation.
The setting is also available as *Is Active* on the edit page of each entry.

:::tip[Deactivate the plugins, too]
Some features are implemented as plugins, for example the invasions and the
periodic starts of the mini games. Deactivate them on the [Plugins](plugins.md)
page as well, so that players don't get announcements of events which aren't
available.
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
