---
title: Lucky coins
sidebar_label: Lucky coins
sidebar_position: 6
description: Registering lucky coins at Delgado and exchanging them for items.
---

Delgado, an NPC in Devias, collects lucky coins. When a player talks to him,
the game client offers two dialogs:

* **Lucky coin registration**: each click registers one lucky coin of the
  inventory. The coin is used up, and the count of the registered coins of the
  character goes up. The dialog shows this count. In the original game, the
  registered coins took part in a lottery, which isn't part of the server; the
  count is kept in the character attribute *Registered Lucky Coins*, so that a
  game master can see it.
* **Lucky coin exchange**: the player exchanges 10, 20 or 30 coins for an item.
  The coins can be spread over several stacks. When the inventory has no space
  for the item, the coins are kept.

Lucky coins are stacked, up to 255 coins per stack. They don't drop from
monsters by default; add them to a drop item group to make them obtainable
(see [Item drops](../admin-panel/item-drops.md)).

## Settings

The plugin **Lucky coin event** is configured on the
[Plugins](../admin-panel/plugins.md) page. When it's deactivated, the coins
can't be registered or exchanged.

* **Rewards** (default: none): the possible items of the exchange. Each reward
  has the count of coins (10, 20 or 30), the group and number of the item and
  its level. One of the rewards for the exchanged count of coins is chosen
  randomly.

The rewards of the original game are not known, so there are none by default.
Without a reward for a count of coins, the exchange of that count is refused,
and the client says that there are not enough coins.
