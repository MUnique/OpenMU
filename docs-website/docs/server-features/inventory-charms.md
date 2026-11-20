---
title: Figurines and charms
sidebar_label: Figurines and charms
description: Figurines and charms give a bonus while used in the inventory.
---

The figurines and charms are items of the in-game shop. They are not worn in
an equipment slot. Instead, the player right clicks them inside the inventory
to use them, and the bonus applies as long as they stay in use there. Another
right click stops the use.

| Item                     | Number | Bonus                          |
| ------------------------ | ------ | ------------------------------ |
| Hawk Figurine            | 13/128 | +10% critical damage chance    |
| Goat Figurine            | 13/129 | +10% excellent damage chance   |
| Oak Charm                | 13/130 | +50 maximum health             |
| Maple Charm              | 13/131 | +50 maximum mana               |
| Golden Oak Charm         | 13/132 | +100 health, +500 shield       |
| Golden Maple Charm       | 13/133 | +150 mana, +50 ability (AG)    |

The values are the ones which the game client shows in the item tooltip. For
the Golden Oak Charm, the Season 6 Episode 3 client shows +100 health and +500
shield, which matches its `ItemAddOption.bmd`; older clients showed +150 and
+50. The values are stored as power-ups of the item definitions, so they can
be changed in the admin panel.

## Rules

* Only one item of the same kind can be in use at the same time. Using a
  second one is refused with a message. The Hawk and Goat Figurines, the Oak
  and Golden Oak Charms, and the Maple and Golden Maple Charms count as the
  same kind: they form an exclusive group in the configuration.
* The item keeps its state when it is moved inside the inventory.
* When the item leaves the inventory, for example when it is dropped or moved
  into the vault, it is no longer in use, and the bonus is removed. This also
  happens when the item was removed from the configuration in the meantime.
* Using an item which is already in use keeps its bonus, so the current
  health, mana and so on stay as they are.
* Like in the original game, the state is stored in the durability of the
  item: 254 means in use, 255 means not in use. A new item starts with 255
  when it is added to the inventory. The item definitions have a durability
  of 1, so the items are not stackable.

## Configuration

The *Items used inside the inventory* feature plugin defines which items can
be used inside the inventory. By default, these are the items 13/128 to
13/134. Each item can have an exclusive group: only one item of a group can
be in use at the same time. By default, the figurines are group 1, the oak
charms group 2 and the maple charms group 3. It also defines the
durability values of an item which is in use (254) and not in use (255).
They have to match the game client. When the plugin is deactivated, no item
can be used this way.

## Update of existing databases

The update *Add the figurines and charms* adds the items, and links the
products of the cash shop to them. Their packages were not for sale before,
because their products had no item. The update leaves them that way, so the
administrator can put them on sale in the admin panel.

## Protocol

The client sends the request `C1 BF 20` with the inventory slot and the
requested state. The server answers with the same packet and the new state of
the item.

:::note
The Worn Horseshoe (13/134) is not added yet, because there is no attribute
for the item drop rate yet.
:::
