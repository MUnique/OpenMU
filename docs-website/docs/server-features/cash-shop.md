---
title: Cash shop
sidebar_label: Cash shop
sidebar_position: 4
description: The in-game cash shop of Season 6, in which players spend WCoin and Goblin Points.
---

# Cash shop

The cash shop (in-game shop, key `X`) of the Season 6 client offers items for
**WCoin (C)**, **WCoin (P)** and **Goblin Points**. Every account has its own
balance of these three coins, and a storage which holds the bought and the
gifted items until the player uses them.

:::note
Not implemented yet: the event item list and items which expire.
:::

## Settings

The plugin **Cash shop** on the [Plugins](../admin-panel/plugins.md) page switches
the cash shop on and off — while it's deactivated, the shop can't be opened — and
holds its settings:

| Setting | Default | Meaning |
|---|---|---|
| Is Gifting Enabled | yes | Players can send packages as gift. |
| Can Gift To Own Account | yes | Players can send gifts to the characters of their own account. |
| Maximum Storage Items | 0 | Maximum number of items in the storage of an account, including gifts; 0 means unlimited. A purchase or gift which doesn't fit is refused. |

## Opening the shop

A player can open the cash shop when the character stands in a safezone, and
doesn't trade or talk to an npc at the same time.

## Coins

The balances are properties of the account. Coins are given by grants, which a
payment provider or a website creates through the
[web API](../admin-panel/authentication.md#granting-cash-shop-coins). The game
server applies a grant the next time the player opens the cash shop.

The plugin **Goblin Points for play time** (deactivated by default) gives players
Goblin Points each time they played for a configured interval. It's configured on
the Plugins page: the interval (default 1 hour), the points (default 10), the
minimum character level, and whether characters which level offline get them,
too.

## Product catalog

The game client doesn't get the products from the server. It loads them from
script files in its `Data\InGameShopScript` folder, and the banner from
`Data\InGameShopBanner`. The server only tells the client which version of the
script and banner to load, when the character enters the game.

The server needs its own copy of the catalog, because it decides about the
prices and the items which a player gets. It's part of the game configuration
as *Cash Shop Configuration*:

* the script version (sale zone, year and id within the year) and the banner
  version, which the server sends to the client;
* the packages, with the coin type, the price and whether they're for sale or
  can be sent as gift;
* the products of each package — either price options of which the player buys
  one, or the contents of a bundle — with the item, its level, the quantity and
  the duration.

The packages and products are identified by the sequence numbers of the client
script. If you change the catalog, change the script files of your clients the
same way, and give them a new version.

A new Season 6 database contains the catalog of the script `512.2012.084`,
which comes with the game client. Existing databases get it with the optional
configuration update *Add cash shop*.

Some packages can't be sold, and the server refuses them even if they're marked
for sale:

* packages with items which aren't defined in the game configuration — most
  special cash shop items, like seals, scrolls and talismans;
* packages with time-limited items (for example pets for 1 day), because items
  can't expire yet;
* packages with a [special item](#special-items) whose plugin is deactivated.

### Editing the catalog

**Game configuration → Cash shop** in the admin panel (administrators only)
edits the catalog:

* The list shows every package with its coin type, price and flags. The column
  *Sellable* shows whether a player can buy it, or why not: a product without
  item, a time-limited product, no products, or a number used twice.
* **Edit** opens a package: number, name, coin type, bundle price, the flags,
  and its products with their numbers, price, item, item level, quantity and
  duration.

Changes take effect when you save. The page doesn't change what the game client
shows: a package you put on sale must exist in the client script, and a changed
price is charged, but the client still shows the price of its script.

## Buying

A purchase takes the price from the coin balance of the package's coin type and
adds the products to the storage of the account. A bundle package adds all of
its products for the price of the package; the other packages are price options
(for example 1, 3, 7 or 30 days) of which the player buys one.

## Gifts

A player can buy a package as gift for any character, also of an offline player.
The sender pays, and the products go to the gift storage of the recipient's
account, together with the name of the sender and the message. Only packages
which are marked as giftable can be sent; in the catalog of the game client
these are the Goblin Point packages and some WCoin (P) packages.

## Using items

A bought or gifted item waits in the storage of the account until the player
uses it with any character of the account. Using it puts its items into the
inventory, with the item level and quantity of the product. If the inventory
doesn't have space for all of them, nothing is added and the item stays in the
storage.

### Special items

Some items aren't put into the inventory. A plugin applies them when the player
uses them from the storage, and each of them can be deactivated and configured
on the **Plugins** page:

| Plugin | Item | Effect |
|---|---|---|
| Cash shop: Vault Expansion Certificate | Vault Expansion Certificate | extends the vault of the account |
| Cash shop: Magic Backpack | Magic Backpack | adds an inventory extension to the character, up to *Maximum Extensions* (at most 4) |
| Cash shop: Summoner Character Card | Summoner Character Card | lets the account create Summoners |
| Cash shop: Rage Fighter Character Card | Rage Fighter Character Card | lets the account create Rage Fighters |

An item can't be used when its effect is already applied, for example when the
vault is already extended; it stays in the storage then. A new inventory
extension can be used after selecting the character again.

The plugin *UnlockSummonerAtLevel1* unlocks the Summoner for every account,
which makes the Summoner Character Card useless. Deactivate that plugin if you
sell the card.

While the plugin of a special item is deactivated, its packages can't be bought,
and bought items stay in the storage until the plugin is active again.
