---
title: Figurines and charms
sidebar_label: Figurines and charms
description: Figurines and charms give a bonus while used in the inventory.
---

# Figurines and charms

The figurines and charms are items of the in-game shop. They are not worn in
an equipment slot. Instead, the player right clicks them inside the inventory
to use them, and the bonus applies as long as they stay in use there. Another
right click stops the use.

| Item                     | Number | Bonus                          |
| ------------------------ | ------ | ------------------------------ |
| Hawk Figurine            | 13/128 | +10 critical damage            |
| Goat Figurine            | 13/129 | +10 excellent damage           |
| Oak Charm                | 13/130 | +50 maximum health             |
| Maple Charm              | 13/131 | +50 maximum mana               |
| Golden Oak Charm         | 13/132 | +150 health, +50 shield        |
| Golden Maple Charm       | 13/133 | +150 mana, +50 ability (AG)    |

The values are the ones which the game client shows in the item tooltip. They
are stored as power-ups of the item definitions, so they can be changed in the
admin panel.

## Rules

* Only one item of the same kind can be in use at the same time. Using a
  second one is refused with a message.
* The item keeps its state when it is moved inside the inventory.
* When the item leaves the inventory, for example into the vault, it is no
  longer in use, and the bonus is removed.
* Like in the original game, the state is stored in the durability of the
  item: 254 means in use, 255 means not in use.

## Protocol

The client sends the request `C1 BF 20` with the inventory slot and the
requested state. The server answers with the same packet and the new state of
the item.

:::note
The Worn Horseshoe (13/134) is not added yet, because there is no attribute
for the item drop rate yet.
:::
