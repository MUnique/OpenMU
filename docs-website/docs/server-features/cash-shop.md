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
  special cash shop items, like talismans;
* packages with time-limited items (for example pets or seals for 1 day),
  because items can't expire yet;
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

### Gold and Silver Keys

The Gold Key and the Silver Key open a Sealed Golden Box and a Sealed Silver
Box. Put a key together with its sealed box into the chaos machine of the
chaos goblin; the combination always succeeds, costs no money and creates a
Golden Box or a Silver Box. Like a *Box of Luck*, an opened box is used by
dropping it: it gives an excellent item, a jewel, or money otherwise. The
Golden Box has the better contents. Their drop groups can be changed on the
**Item drops** page (*Item box drops*).

The sealed boxes don't drop from monsters by default, because their original
source and chances aren't known. Add them to a drop group to make the keys
useful. Existing databases get the keys and boxes with the configuration update
*Add golden and silver boxes*.

### Reset fruits

The reset fruits of strength, agility, vitality, energy and command (the last
one only for Dark Lords) take back invested points of their stat and return them
as level up points. Each durability of a fruit takes back 10 points, and a
bought fruit has a durability of 100, like in the game client, so it takes back
up to 1000 points, but never below the base value of the character class. The
fruit is used up, even if fewer points were invested. All items have to be
unequipped, and the points don't count towards the maximum of the usual fruits
which remove points. The fruits can't be traded, dropped or sold.

Existing databases get the fruits with the optional configuration update *Add
reset fruits*, which also puts their packages on sale. Each fruit is handled by its own plugin, for example *Reset Fruit of
Strength Consume Handler*, which can be deactivated on the **Plugins** page. Its
configuration there also sets the *Points per durability* (10 by default).

### Elite potions

The elite potions recover a percentage of the maximum value immediately,
independent of the character level, as the game client describes them:

| Potion | Recovers |
|---|---|
| Elite Healing Potion | 100% health |
| Elite Mana Potion | 100% mana |
| Medium Elite Healing Potion | 65% health |
| Elite SD Potion | 65% shield |

They share the cooldown of the usual potions, stack up to 50 pieces, and can't
be traded, dropped or sold. The percentage and the cooldown can be changed in
the configuration of their consume handler plugins, for example *Elite Healing
Potion Consume Handler*, on the **Plugins** page.

Existing databases get the potions with the configuration update *Add elite
potions*.

### Rings and necklaces

The rings and necklaces of the shop are defined as items with fixed bonuses,
like the game client shows them. They can't be upgraded, traded, dropped or
repaired. A player can sell them to an NPC, for example to get rid of one which
wore out.

| Item | Bonus |
|---|---|
| Lethal Wizard's Ring | damage +15%, wizardry damage +15%, attack speed +10 |
| Sapphire Ring | automatic HP recovery 3%, maximum mana +4% |
| Ruby Ring | automatic HP recovery 3%, maximum HP +4% |
| Topaz Ring | automatic HP recovery 3%, zen from monsters +30% |
| Amethyst Ring | automatic HP recovery 3%, damage decrease 4% |
| Ruby Necklace | automatic HP recovery 3%, excellent damage rate +10% |
| Emerald Necklace | automatic HP recovery 3%, attack speed +7 |
| Sapphire Necklace | automatic HP recovery 3%, mana after hunting monsters +mana/8 |

The shop sells them only for a limited time, so their packages can't be sold yet
(see [product catalog](#product-catalog)). An administrator can still give them
to a character. Existing databases get them with the configuration update *Add
cash shop rings and necklaces*.

### Crafting talismans

The **Talisman of Luck** and the **Talisman of Chaos Assembly** are put into the
chaos machine together with the items of a crafting:

* The Talisman of Luck increases the success rate by its durability in percent
  (1, 5 or 10, depending on the bought product). Several talismans can be put
  in, up to a total durability of 10, and the rate never exceeds the maximum
  rate of the crafting. It's accepted by the +10 to +15 item upgrades, the
  fruits, the wings, capes and 3rd wings, the Fenrir stages, the shield potions,
  and the Dark Horse and Dark Raven of the pet trainer. The craftings of the
  event tickets and the Fenrir upgrade don't accept it yet.
* The Talisman of Chaos Assembly keeps the item of a +10 to +15 item upgrade
  when the upgrade fails. Only one can be put in.

Both are used up by the crafting, whether it succeeds or fails. In the admin
panel, they are optional required items of the craftings: the Talisman of Luck
adds its percentage per durability (*Add Percentage*), and the Talisman of Chaos
Assembly has the fail result *Disappear And Protect Referenced Items*. Existing
databases get them with the configuration update *Add crafting talismans*.

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

### Invitation to Santa Village

The Invitation to Santa Village can be used ten times. Each use warps the
player to the safezone of the Santa Village. It isn't used up when the player
is already there, or when the map is deactivated. The target map can be changed
in the configuration of the plugin *Invitation to Santa Village Consume
Handler* on the **Plugins** page.

### Lucky item tickets

The ten lucky item tickets (1st and 2nd Lucky Armor, Pants, Helm, Gloves and
Boots Ticket) are put into the inventory like other items. The player exchanges
a ticket at David in Elveland, in the *Exchange Lucky Item* window: he gives the
lucky armor of the ticket's tier and kind which the character class can wear.
The 1st tickets give the Scale, Silk, Sphinx and Violent Wind sets (level 1),
the 2nd tickets the Adamantine, Dragon, Guardian, Legendary, Red Wing, Atlans
and Storm Jahad sets (level 150). When more than one set fits the class, the
one made for the fewest classes is given, for example the Atlans Armor for a
Magic Gladiator. A ticket without an armor for the class, like a helm ticket for
a Magic Gladiator, isn't accepted.

Lucky armors can't be traded, dropped, stored, sold in a personal store,
repaired or upgraded with jewels. The exchange is configured as the crafting
*Lucky Item Exchange* of David. Existing databases get the items, the exchange
and the ticket packages with the configuration update *Add lucky items*. The
refinery window of David, and the Jewel of Extension and Jewel of Elevation,
don't have a function yet.

### Buff items

Seals, scrolls, elixirs and auras are put into the inventory. When a player uses
one, it's consumed and gives a buff for a limited time. The values, durations and
buff icons are the ones which the game client shows for these items:

| Item | Effect | Duration |
|---|---|---|
| Seal of Ascension | experience ×1.5 | 30 minutes |
| Seal of Wealth | experience ×2 | 30 minutes |
| Seal of Sustenance | no experience (also no master or bonus experience) | 30 minutes |
| Seal of Healing | experience ×1.5, automatic life recovery +5% | 30 minutes |
| Seal of Divinity | automatic mana recovery +5% | 30 minutes |
| Master Seal of Ascension | master experience ×1.5 | 30 minutes |
| Master Seal of Wealth | master experience ×2 | 30 minutes |
| Max AG Boost Aura | maximum AG + character level | 30 minutes |
| Max SD Boost Aura | maximum SD + character level × 10 | 30 minutes |
| Scroll of Quickness | attack speed +15 | 30 minutes |
| Scroll of Defense | defense +60 | 30 minutes |
| Scroll of Wrath | physical damage +30 | 30 minutes |
| Scroll of Wizardry | wizardry damage +30 | 30 minutes |
| Scroll of Health | maximum life +200 | 30 minutes |
| Scroll of Mana | maximum mana +300 | 30 minutes |
| Scroll of Battle | critical damage chance +20% | 30 minutes |
| Scroll of Strength | excellent damage chance +20% | 30 minutes |
| Scroll of Healing | automatic life recovery +3%, maximum life +100 | 30 minutes |
| Elixir of Strength, Agility, Health, Energy, Control | the stat +50 | 10 days |

Only one seal of experience (Ascension, Wealth, Sustenance, Healing and the
master seals) and one elixir is active at the same time; using another one
replaces it. The same applies to the Scrolls of Battle and Strength. The Seal of
Sustenance also blocks the bonus experience of pets and rings. The buffs of
seals, elixirs and auras stay when the character dies, the buffs of scrolls end.
All of these buffs end when the character leaves the game.

:::note
Not implemented yet: the higher item drop rate of the Seals of Wealth and
Divinity, the Seal of Mobility, the Party EXP Scroll and the second Seal of
Wealth (13/116), which only raises the item drop rate.
:::

The items and their packages exist in a new Season 6 database. Existing
databases get the items with the configuration update *Add buff items*. Their
packages in the cash shop are time-limited, so they can't be sold until items
can expire. An administrator can still give the items to players, for example
in a merchant store.

### Kalima Ticket

The Kalima Ticket can be used ten times. A right click outside of the safezone
opens a gate to Kalima for the player and its party, like a Lost Map. The level
of Kalima is the highest one whose minimum character level the character
reached. The minimum levels are the ones the game client shows: 40, 131, 181,
231, 281, 331 and 350, and 20, 111, 161, 211, 261, 311 and 350 for Magic
Gladiators, Dark Lords and Rage Fighters. They can be changed in the
configuration of the plugin *Kalima Ticket Consume Handler* on the **Plugins**
page. The ticket isn't used up when no gate can be opened.

The optional update *Add kalima ticket* adds the ticket to an existing
configuration and puts its packages on sale. Deselect it to keep them off sale.
