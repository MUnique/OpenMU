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

## Deleting a character

The waiting items belong to the character and are deleted with it. Because an
item from the vault belongs to the whole account, a character with waiting items
can't be deleted. The player is asked to enter the game with it and make room
first.

An item whose item definition no longer exists isn't delivered or listed. It
stays in the database until the definition is back, like an inventory item
without a definition, and it prevents the deletion, too. Then, the player is
asked to contact an admin.
