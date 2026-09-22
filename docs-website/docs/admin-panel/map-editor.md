---
title: Map editor
sidebar_position: 10
description: Edit monster spawn areas and gates of a map graphically.
---

# Map editor

**Navigation:** *Configuration → Map editor* — route `/map-editor`, or the
*Map editor* link on the edit page of a game map.

Spawn areas and gates are coordinates on a terrain. Editing them in the
[generic form](game-configuration.md#the-generic-edit-pages) means typing numbers;
the map editor lets you place them on the rendered terrain of the map instead.

## Selecting a map

The drop-down at the top selects the map to edit. Next to it are the buttons to
export and import a map, see [below](#export-and-import).

## What you can create

| Object | Purpose |
|---|---|
| **Monster spawn area** | Where a monster (or NPC) spawns, how many of them, and in which rectangle they may appear |
| **Enter gate** | The area a player arrives in when entering the map through a gate |
| **Exit gate** | The area which sends a player to another map, and its target |

Click one of the create buttons, then draw the object on the map. Confirm it with
the ✔ button or discard it with ✘.

## Editing existing objects

Click an object on the map to focus it. The properties of the focused object are
shown next to the map, so you can adjust the details — the monster and its
quantity for a spawn area, the target gate and the direction for an exit gate.

With an object focused you can also:

* **Duplicate** it, which is the quickest way to add a second, similar spawn area
* **Remove** it

**Undo** reverts your last action; it works for the whole editing session, until
you save.

## Saving

Nothing is written to the database until you press **Save**. Leaving the page
without saving discards the changes.

:::warning[A reload is needed]
Like the other configuration changes, a modified map takes effect after the game
servers reload their configuration — use **Reload configuration and restart all
game servers** on the [Servers page](servers.md).
:::

## Export and import

**Export map** downloads the map's editable content as a JSON file, **Import map**
reads such a file back in.

This is how you

* back up a map before larger changes,
* move a map layout from a test server to your live server,
* share a custom map with someone else.

Import replaces the objects of the currently selected map, so export first if you
want to be able to go back.
