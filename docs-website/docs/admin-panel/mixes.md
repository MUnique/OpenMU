---
title: Mixes
sidebar_position: 7.6
description: Configure the success rates and the fail results of the chaos machine mixes on one page.
---

# Mixes

**Navigation:** *Configuration → Mixes* — route `/mixes`

The chaos machine — the chaos goblin, and the other NPCs which use the same
window — offers a list of mixes: combine a jewel with an item to upgrade it,
combine items to create wings, and so on. Each mix is an `ItemCrafting` in the
game configuration and is attached to the monster which offers it.

The page shows all mix definitions of the game configuration, grouped by the NPC
which offers them. The categories and their mixes are collapsed by default: open
an NPC to see its mixes, which are sorted by their number, and open a mix to edit
it. The search field filters by NPC name, mix name or number, and handler class.

## What a mix consists of

A mix has a number (the client references it with this number), a name and the
name of the handler class which implements it. Most mixes are implemented by the
`SimpleItemCraftingHandler`, whose behaviour is described by the
`SimpleCraftingSettings`:

* **Success percent** — the base chance of the mix.
* **Maximum success percent** — the upper limit after all additions.
* **Additions for the target item** — the extra chance when the item which gets
  modified has the luck option, is excellent, ancient, a "380 item" or a socket
  item.
* **Required items** — the items which are consumed, each with an amount range,
  an optional chance addition per additional item and an optional price divisor
  that turns the NPC price of the items into chance.
* **Price** — a fixed amount of money, optionally plus an amount per success
  percentage point, and a price divisor for the sum of the required items'
  prices.
* **Result** — the result items, whether one random or all of them are created,
  and the chances for luck, skill and excellent options in a random result item.

Mixes which are not implemented by the simple handler have no simple crafting
settings; the page offers to create them, because their behaviour is defined by
their handler class.

## Editing and saving

Every mix is shown with its own form and its own **Save** button, so a change is
saved for that mix only — the other mixes of the page are not affected. **Cancel**
discards the changes of the mix and loads it again from the database.

The form contains all properties of the mix: its number and name, the handler
class, the success rates and the other crafting settings, and the lists of
required and result items. The items are edited with the same controls as in the
full configuration:

* **Create** adds a new entry to a list, e.g. a new required item, which gets
  its amount and item level ranges, the chance addition per additional item, the
  NPC price divisor, the reference number, and what happens to the item when the
  mix succeeds or fails.
* **Remove** deletes an entry and its own children.
* References such as the possible items of a required item, or the items of a
  result item, are picked from a searchable list of the existing configuration
  entries.

## How the success rate is calculated

The success rate is the base *success percent*, plus the additions of the
required items (per additional item and per NPC price divisor), plus the
additions for the target item's options. The result is capped by the *maximum
success percent*, and the player's *crafting bonus* attribute is applied on top
of it. A rate of 0 or 100 percent is kept, because such mixes either can't
succeed or can't fail by design.

## What happens when a mix fails

Every required item defines what happens to it when the mix succeeds
(`Success Result`) and when it fails (`Fail Result`):

| Value | Effect on the item |
|---|---|
| *Disappear* | The item is destroyed. |
| *Stays As Is* | The item is kept, e.g. the item of a successful item upgrade. |
| *Chaos Weapon And First Wings Downgraded Random* | The item loses levels, skill and one option level at random. |
| *Third Wings Downgraded Random* | The item is downgraded by 2 or 3 levels and its option is removed. |
| *Disappear And Protect Referenced Items* | The item is destroyed, but the referenced items are kept as is, e.g. for the Talisman of Chaos Assembly. |

The fail results are the typical reason to open this page: a server owner can
decide whether a failed mix destroys the items, keeps them or only downgrades
them, per required item.

## Related pages

* [Game configuration](game-configuration.md) — the full configuration, where
  all other properties of the mixes are editable.
* [Item drops](item-drops.md) — the drops of the chaos machine and the other
  monsters.
