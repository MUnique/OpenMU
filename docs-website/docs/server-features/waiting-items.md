---
title: Waiting items
sidebar_label: Waiting items
sidebar_position: 6
description: Items which find no place in their storage wait for space in the inventory, instead of blocking the character.
---

When the size of an item definition grows, e.g. after an update of the game
configuration, an item of that kind may no longer fit where it is: in the
inventory, the personal store or the vault. Such an item doesn't stop the
character from entering the game. It's taken out of its storage and waits for
space in the inventory of the character. The server log gets a warning for each
of these items.

An item in the inventory first tries the other places of the inventory,
including the inventory extensions, before it waits.

## Delivery

The plugin **Waiting items delivery** puts the waiting items into the inventory
as soon as they fit there:

* when the character enters the game,
* when the character opens the vault and one of its items found no place,
* and then periodically, by default every 30 seconds, but only while no dialog
  is open, e.g. no trade, chaos machine, merchant or vault.

The oldest item comes first. An item which doesn't fit doesn't hold back a
later, smaller one. The client gets a delivered item like any other new item of
the inventory, so this works with every client.

When the plugin is deactivated, the items keep waiting, and they are delivered
when it's activated again.

### Settings

| Setting | Default | Meaning |
|---|---|---|
| Check interval | 30 seconds | How often the waiting items are put into the inventories |
| Named items | 3 | How many waiting items are named when the player is told about them |

## Messages

When the character enters the game or opens the vault, the player is told which
items wait: one line with their number, one line per item (name, level and
size), and "... and N more." for the rest. Each delivered item is announced with
its own line.

The chat command `/waiting` lists the waiting items, by default up to 10 (its
setting **Named items**). The automatic message points to it when the command is
active.

## Quest rewards

Quest rewards don't become waiting items. Instead, a quest isn't completed while
its rewards don't fit:

* the item rewards must fit into the inventory, where the items which the quest
  takes count as free space, because they are taken first,
* and the Zen rewards must not take the Zen of the inventory over its limit,
  the setting **Maximum Inventory Money** of the
  [game configuration](../admin-panel/game-configuration.md#general).

Then nothing is taken, and the quest stays active, so the player can make room
and try to complete it again. For the newer quests, the Season 6 client shows
its own message that the inventory is full or that the Zen limit is reached,
and disables the button to complete the quest. The older quests, i.e. the
quests of the second and third class changes and of Marlon, get a blue message
with the reason instead.

When a reward still doesn't fit in the end, e.g. because an inventory extension
of a plugin accepted it although it has no room, it's dropped at the feet of
the player, not added to the waiting items.

## Deleting a character

The waiting items belong to the character and are deleted with it. Because an
item from the vault belongs to the whole account, a character with waiting items
can't be deleted. The player is asked to enter the game with it and make room
first.

An item whose item definition no longer exists isn't delivered or listed. It
stays in the database until the definition is back, like an inventory item
without a definition, and it prevents the deletion, too. Then, the player is
asked to contact an admin.
