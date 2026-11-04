---
title: Player shop currencies
sidebar_label: Player shop currencies
sidebar_position: 5
description: Prices of items in personal stores in items like jewels, and in account coins, in addition to zen.
---

In a personal store, the price of an item is usually defined in zen. With the
plugin **Player shop currencies**, players can define the price of each item in
one of these currencies, too:

* items which are configured as price items, by default the Jewels of Bless,
  Soul, Chaos, Creation and Life;
* the account coins WCoin (C), WCoin (P) and Goblin Points (see [Cash shop](cash-shop.md)).

The plugin is deactivated by default. Then, the personal stores work exactly as
before: only zen prices are possible, and the server sends nothing new to the
clients.

## Clients

Only the open source client ([MuMain](https://github.com/sven-n/MuMain)) shows
and buys items with prices in other currencies than zen. When the plugin is
active, the server tells it which price items and coins are allowed, so it
offers them when the player sets the prices of the store.

The original Season 6 client can only show zen prices. Items with prices in
other currencies are hidden in its store list, and the server refuses to sell
them to it.

## Settings

The plugin is configured on the [Plugins](../admin-panel/plugins.md) page:

| Setting | Default | Meaning |
|---|---|---|
| Price Items | the five jewels | The items with which prices can be paid. |
| Is WCoin C/WCoin P/Goblin Points Allowed | no | Prices in this account coin are allowed. The coins are usually bought with real money, so trading them has to be allowed explicitly. |
| Maximum Item Amount | 1000 | The maximum number of item pieces of a price; at most 65535. |
| Maximum Coin Amount | 1000000 | The maximum amount of account coins of a price. |

A new database gets the five jewels as price items. On an existing database, the
plugin is created without price items, because its default configuration can't
reference items: add them when you activate the plugin.

When a price item or coin is disallowed while players have prices in it, their
stores can't be opened until the prices are changed, and these items can't be
bought from stores which are already open.

## Paying with items

The buyer pays with the price items of the inventory. A single item counts as
one piece, and the bundles of the jewel mix count as their number of pieces
(10, 20 or 30). Only tradable items without level and options count.

Bundles are never split, and there is no change: the buyer needs a combination
of items and bundles which matches the price exactly. The items move into the
inventory of the seller, which needs a free slot for each of them.

## Paying with account coins

The coins move from the account of the buyer to the account of the seller.
Pending coin grants of the buyer are applied first.

## Chat command

The plugin **Shop price chat command** (deactivated by default) sets prices with
the chat, which works with every client:

* `/shopprice` lists the items of the own store with their prices;
* `/shopprice <position> <amount> [currency]` sets the price of the item at the
  position 1 to 32 of the store (or its inventory slot 204 to 235). Without a
  currency, the price is in zen. A price of 0 removes the price.

The currency can be `zen`, a price item by its group and number (e.g. `14/13`),
one of the jewels by name (`bless`/`job`, `soul`/`jos`, `chaos`/`joc`,
`creation`/`jocr`, `life`/`jol`) or an account coin (`wcoinc`/`wcc`,
`wcoinp`/`wcp`, `goblin`/`gp`). The store has to be closed to change its prices.
